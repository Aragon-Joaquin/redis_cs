using redis_cs.RESP;

namespace redis_cs.src.RESP;

public class Data
{
  public Data(string type, string content, int length)
  {
    Length = length;
    Type = type;
    Content = content;
  }
  public Data()
  {
    Length = 0;
    Type = RespPrefixes.NullBulkString;
    Content = "";
  }
  public readonly int Length;
  public readonly string Type;
  public readonly string Content;
}


public class BulkString(string content)
{
  public readonly string Content = content;
}
