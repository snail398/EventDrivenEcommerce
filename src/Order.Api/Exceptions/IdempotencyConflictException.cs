namespace Order.Api.Exceptions;

public sealed class IdempotencyConflictException : Exception
{
    public IdempotencyConflictException() : base("Idempotency key was already used with a different request body.") { }
}