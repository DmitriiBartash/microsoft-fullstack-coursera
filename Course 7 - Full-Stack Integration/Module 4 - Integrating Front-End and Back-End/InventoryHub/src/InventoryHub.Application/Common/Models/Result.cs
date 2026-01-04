namespace InventoryHub.Application.Common.Models;

public class Result<T>
{
    public T? Data { get; }
    public bool IsSuccess { get; }
    public Error? Error { get; }

    private Result(T data)
    {
        Data = data;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        Error = error;
        IsSuccess = false;
    }

    public static Result<T> Success(T data) => new(data);
    public static Result<T> Failure(Error error) => new(error);
    public static Result<T> Failure(string code, string message) => new(new Error(code, message));
}

public record Error(string Code, string Message, string[]? Details = null);

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this T data) => Result<T>.Success(data);
}
