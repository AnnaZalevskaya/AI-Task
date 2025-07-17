Prompt: 

You are an experienced C# developer  tasked with fixing a runtime bug and improving the resilience of a payment processing service.

The PaymentProcessingService communicates with an external payment gateway using HttpClient and deserializes the JSON response into a GatewayResponse object. Occasionally, the service crashes with a NullReferenceException when attempting to access properties of the deserialized object. This happens when the gateway returns malformed or empty JSON. Additionally, the service lacks resilience mechanisms like retries or a circuit breaker, causing unhandled failures when the gateway is down or unstable.

Refer to the following class and observe exceptions from logs:

Your task:

- Fix the bug causing the NullReferenceException by safely handling malformed or empty JSON responses.
- Improve resilience: Add retry logic or implement a circuit breaker to prevent repeated failures when the gateway is temporarily unavailable.
- Ensure logging remains informative for debugging, and no exception is thrown due to bad external input.

Use C# best practices for fault tolerance and clean error handling. Avoid over-complicating the fix; prefer clarity and maintainability.
