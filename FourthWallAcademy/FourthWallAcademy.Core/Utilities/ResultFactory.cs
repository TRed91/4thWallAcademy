namespace _4thWallCafe.Core.Utilities;

public class Result
{
    public bool Ok { get; private set; }
    public string Message { get; private set; }

    public Result(bool ok, string message)
    {
        Ok = ok;
        Message = message;
    }
}

public class Result<T> : Result
{
    public T Data { get; private set; }

    public Result(T data, bool ok, string message) : base(ok, message)
    {
        Data = data;
    }
}

public class ResultFactory
{
    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Fail(string message)
    {
        return new Result(false, message);
    }

    public static Result<T> Success<T>(T data)
    {
        return new Result<T>(data, true, string.Empty);
    }

    public static Result<T> Fail<T>(string message)
    {
        return new Result<T>(default, false, message);
    }
}