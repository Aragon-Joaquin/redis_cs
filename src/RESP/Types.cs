namespace redis_cs.RESP;

// you cannot do a typedef string Type, so you need a class for it
// and by making a class, you cannot prohibit to be imported by other namespaces
// so, the client code can extend the types
// what a poorly design choice from a C "successor".


//TODO: somehow make a type for this
public class RespPrefixes
{
  public const string SimpleString = "+";
  public const string SimpleError = "-";
  public const string Integer = ":";
  public const string BulkString = "$";
  public const string NullBulkString = "$-1\r\n";
  public const string Array = "*";
}

public static class RespTypes
{
  public readonly static Dictionary<string, string> DataTypes = new()
  {
        { RespPrefixes.SimpleString, "SimpleString" },
        { RespPrefixes.SimpleError, "Error" },
        { RespPrefixes.Integer, "Integer" },
        { RespPrefixes.BulkString, "BulkString" },
        { RespPrefixes.NullBulkString, "NullBulkString" },
        { RespPrefixes.Array, "Array" }
  };
}



