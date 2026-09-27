using System.ComponentModel.DataAnnotations;
using System.Text;

namespace redis_cs.RESP;


public class Data(string type, string content, int length)
{
  public readonly int Length = length;
  public readonly string Type = type;
  public readonly string Content = content;
}

public struct ReaderResult
{
  public ReaderResult(Data action, Data[] rest)
  {
    this.action = action;
    this.rest = rest;
    ok = true;
  }

  public ReaderResult()
  {
    action = default!;
    rest = default!;
    ok = false;

  }
  public Data action;
  public Data[] rest;
  public bool ok;
}

public class RespReader
{
  //the call goes like this:
  // *2\r\n
  // $4\r\n
  // echo\r\n
  // $11\r\n
  // hello_world\r\n
  public static ReaderResult ReadBytes(int bytes_read, byte[] buf)
  {
    string[] lines = Encoding.UTF8.GetString(buf, 0, bytes_read).Split(
          "\r\n",
          StringSplitOptions.TrimEntries
      );

    ReaderResult res = new();
    if (lines.Length <= 0) return res;

    //array validation
    string[] parts = lines.ElementAt(0).Split(RespPrefixes.Array, 2);
    if (parts.Length != 2) return res;
    var (array_symbol, array_length) = (parts[0], parts[1]);

    bool ok = RespTypes.DataTypes.TryGetValue(array_symbol, out var val);
    if (!ok || val != RespPrefixes.Array) return res;

    ok = int.TryParse(array_length, out var array_length_int);
    if (!ok) return res;

    //splitting 2 in 2
    var new_lines = lines[1..];
    for (int i = 0; i < new_lines.Length; i += 2)
    {
      ParseDataType(new_lines.ElementAt(i), new_lines.ElementAt(i + 1));
    }


    return res;
  }

  public static Data? ParseDataType(string text, string length)
  {
    bool ok = int.TryParse(length, out var l);

    //TODO: make the text[:l]
    if (!ok || l != text.Length) return null;

    return new(RespPrefixes.BulkString, text, l);
  }
}

