namespace MAUI_app.Data;


public sealed class Result : IResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    public static Result Ok(string message) =>
        new() { Success = true, Message = message };

    public static Result Fail(string message) =>
        new() { Success = false, Message = message };
}

public sealed class Result<T> : IResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public required T Data { get; init; }

    public static Result<T> Ok(T data, string message) =>
        new() { Success = true, Message = message, Data = data };

    public static Result<T> Fail(string message) =>
        new() { Success = false, Message = message, Data = default! };
}