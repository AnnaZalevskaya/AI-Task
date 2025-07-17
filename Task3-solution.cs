public async Task<PaymentResult> ProcessPaymentAsync(int accountId, decimal amount)
{
    _logger.LogInformation("Start processing payment for account {AccountId}, amount {Amount}", accountId, amount);

    try
    {
        // 1) Check cache
        if (!_accounts.TryGetValue(accountId, out var account))
        {
            _logger.LogError("Account {AccountId} not found in cache", accountId);
            return PaymentResult.Failed("Account not found");
        }

        // 2) Send request to payment service with retry policy
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, delay, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount} after {Delay} due to {Outcome}", retryCount, delay, outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                });

        var request = new
        {
            AccountId = account.Id,
            Amount = amount
        };

        HttpResponseMessage response = await retryPolicy.ExecuteAsync(async () =>
            await _httpClient.PostAsync(
                "https://api.payment-gateway.com/v1/payments",
                new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json"))
            );

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Payment gateway returned status {StatusCode}", response.StatusCode);
            return PaymentResult.Failed("Gateway error");
        }

        var json = await response.Content.ReadAsStringAsync();
        GatewayResponse gatewayResult = null;

        try
        {
            gatewayResult = JsonSerializer.Deserialize<GatewayResponse>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning("Failed to deserialize gateway response: {Error}", ex.Message);
            return PaymentResult.Failed("Invalid gateway response");
        }

        if (gatewayResult == null)
        {
            _logger.LogWarning("Payment gateway returned null response");
            return PaymentResult.Failed("Gateway returned empty response");
        }

        if (!gatewayResult.Success)
        {
            _logger.LogWarning("Payment gateway failed: {Message}", gatewayResult.Message ?? "No error message");
            return PaymentResult.Failed("Gateway failure: " + gatewayResult.Message);
        }

        // 3) Correct balance locally
        if (amount < 0)
        {
            _logger.LogWarning("Negative payment amount: {Amount}", amount);
            return PaymentResult.Failed("Amount must be non-negative");
        }

        if (account.Balance < amount)
        {
            _logger.LogWarning("Insufficient funds for account {AccountId}", accountId);
            return PaymentResult.Failed("Insufficient funds");
        }

        account.Balance -= amount;
        _logger.LogInformation("Payment successful. New balance: {Balance}", account.Balance);

        return PaymentResult.Success(account.Balance);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Payment processing failed for account {AccountId}", accountId);
        return PaymentResult.Failed("Payment processing error");
    }
}
