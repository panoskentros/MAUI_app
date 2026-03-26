namespace MAUI_app.Data;

public interface IResult
{
    bool Success { get; }
    string? Message { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}