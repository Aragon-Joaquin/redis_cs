using System.Text;
using redis_cs.src.RESP;

namespace redis_cs.RESP;

public struct ReaderResult
{
  public ReaderResult(BulkString command, List<BulkString> rest)
  {
    this.command = command;
    this.rest = rest;
    ok = true;
  }

  public ReaderResult()
  {
    command = default!;
    rest = default!;
    ok = false;

  }
  public BulkString command;
  public List<BulkString> rest;
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
          StringSplitOptions.None
      );

    ReaderResult res = new();
    if (lines.Length <= 0) return res;

    //array validation
    var array = ParseArray(lines);
    if (array.Type != RespPrefixes.Array || array.Length <= 0) return res;

    //getting the command
    var command = VerifyLength(lines[1..3]);
    if (command == null) return res;

    //skip the lines already readed
    List<BulkString> rest = [];

    int start_index = 3;

    for (int i = 1; i < array.Length; i++)
    {
      if (start_index + 1 >= lines.Length) break;

      var bs = VerifyLength(lines[start_index..(start_index + 2)]);
      if (bs != null)
      {
        rest.Add(bs);
      }
      start_index += 2;
    }
    res.command = command;
    res.rest = rest;
    res.ok = true;
    return res;
  }

  public static Data ParseDataType(string text)
  {
    bool ok = RespTypes.DataTypes.TryGetValue(text[..1], out var val);
    //TODO: handle NullBulkString???
    if (!ok || val == null) return new();

    var rest = text[val.Length..];
    return new(val, rest, rest.Length);
  }

  private static BulkString? VerifyLength(string[] text)
  {
    if (text.Length != 2) return null;

    var header = text[0].Split(RespPrefixes.BulkString);
    if (header.Length != 2) return null;

    var (_, length) = (header[0], header[1]);
    var body = text[1];

    bool ok = int.TryParse(length, out var val);
    if (!ok) return null;

    return new(body[..val]);
  }

  private static Data ParseArray(string[] lines)
  {
    string[] parts = lines.ElementAt(0).Split(RespPrefixes.Array, 2);
    if (parts.Length != 2) return new();

    var (array_symbol, array_length) = (parts[0], parts[1]);

    bool ok = RespTypes.DataTypes.TryGetValue(array_symbol, out var val);
    if (!ok || val != RespPrefixes.Array) return new();

    ok = int.TryParse(array_length, out var array_length_int);
    if (!ok) return new();

    return new(RespPrefixes.Array, "", array_length_int);
  }
}

