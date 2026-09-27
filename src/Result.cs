using System.Diagnostics.CodeAnalysis;

namespace redis_cs;

//golang inspired?
public record class Error(string Err)
{
  private readonly string _error = Err;
  public string String() => _error;
}

// #hasNoToExceptions (i hate them)

// https://medium.com/@luke04/errors-as-values-the-way-it-should-have-been-done-1707937fbc84
public record class Result<TValue>
    where TValue : notnull
{
  readonly TValue _value;
  readonly Error _error;
  readonly bool _faulted;

  private Result(TValue value)
  {
    _value = value;
    _error = default!;
    _faulted = false;
  }

  private Result(Error error)
  {
    _error = error;
    _value = default!;
    _faulted = true;
  }

  public bool HasValue([NotNullWhen(true)] out TValue? value)
  {
    if (_faulted)
    {
      value = default;
      return false;
    }
    value = _value;
    return true;
  }

  public bool HasError([NotNullWhen(true)] out Error? error)
  {
    if (!_faulted)
    {
      error = default;
      return false;
    }
    error = _error;
    return true;
  }

  public static implicit operator Result<TValue>(Error error) => new(error);
  public static implicit operator Result<TValue>(TValue value) => new(value);
}
