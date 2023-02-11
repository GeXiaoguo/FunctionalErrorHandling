namespace FunctioalErrorHandling;

using System;
using System.Threading.Tasks;


public sealed record Unit
{
    private Unit() { }
    public static Unit Instance = new Unit();
}
public enum ResultKind
{
    OK = 0,
    Error,
}
public abstract record Result<T, ErrorT> : UnionType<ResultKind>
{
    public void Deconstruct(out T? value, out ErrorT? error)
    {
        if (this is Result<T, ErrorT>.Ok)
        {
            (value, error) = ((this as Result<T, ErrorT>.Ok).Value, default);
        }
        else
        {
            (value, error) = (default, (this as Result<T, ErrorT>.Error).ErrorValue);
        }
    }
    public sealed record Ok(T Value) : Result<T, ErrorT>
    {
        public override ResultKind Kind => ResultKind.OK;
    };
    public sealed record Error(ErrorT ErrorValue) : Result<T, ErrorT>
    {
        public override ResultKind Kind => ResultKind.Error;
    };
    public static implicit operator Result<T, ErrorT>(T val) => new Result<T, ErrorT>.Ok(val);
    public static implicit operator Result<T, ErrorT>(ErrorT error) => new Result<T, ErrorT>.Error(error);
    public static Result<T, Exception> Invoke(Func<T> func)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            return e;
        }
    }
    public static async Task<Result<T, Exception>> InvokeAsync(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
public static class TaskMonad
{
    public static async Task<T1> SelectMany<T, T1, ErrorT>(this Task<T> self, Func<T, T1> selector) => selector(await self);
}
public static class ResultMonad
{
    public static Result<T1, ErrorT> Select<T, T1, ErrorT>(this Result<T, ErrorT> self, Func<T, T1> selector)
        => self switch
        {
            Result<T, ErrorT>.Ok ok => selector(ok.Value),
            Result<T, ErrorT>.Error error => error.ErrorValue,
            _ => throw new NotImplementedException(),
        };
    public static async Task<Result<T1, ErrorT>> SelectManyAsync<T, T1, ErrorT>(this Result<T, ErrorT> self, Func<T, Task<Result<T1, ErrorT>>> asyncSelector)
    {
        switch (self)
        {
            case Result<T, ErrorT>.Ok ok:
            {
                var val = await asyncSelector(ok.Value);
                return val switch
                {
                    Result<T1, ErrorT>.Ok newOK => newOK.Value,
                    Result<T1, ErrorT>.Error error => error.ErrorValue,
                    _ => throw new NotImplementedException()
                };
            }
            case Result<T, ErrorT>.Error error:
            {
                return error.ErrorValue;
            }
            default: throw new NotImplementedException();
        }
    }
    public static Result<T1, ErrorT> SelectMany<T, T1, ErrorT>(this Result<T, ErrorT> self, Func<T, Result<T1, ErrorT>> selector)
    {
        switch (self)
        {
            case Result<T, ErrorT>.Ok ok:
            {
                var val = selector(ok.Value);
                return val switch
                {
                    Result<T1, ErrorT>.Ok newOK => newOK.Value,
                    Result<T1, ErrorT>.Error error => error.ErrorValue,
                    _ => throw new NotImplementedException()
                };
            }
            case Result<T, ErrorT>.Error error:
            {
                return error.ErrorValue;
            }
            default: throw new NotImplementedException();
        }
    }
    public static Result<T, ErrorT1> SelectError<T, ErrorT, ErrorT1>(this Result<T, ErrorT> self, Func<ErrorT, ErrorT1> selector)
        => self switch
        {
            Result<T, ErrorT>.Ok ok => ok.Value,
            Result<T, ErrorT>.Error error => selector(error.ErrorValue),
            _ => throw new NotImplementedException(),
        };
}
public abstract record UnionType
{
    // for json serization/deserialization
    public string ConcreteType => GetType().FullName;
}
public abstract record UnionType<TypeEnum> : UnionType
    where TypeEnum : System.Enum
{
    // a workaround for compile type switch statement completeness gurantee in C#
    public abstract TypeEnum Kind { get; }
}