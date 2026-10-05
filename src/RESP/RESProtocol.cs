using System.Net.Sockets;
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
    errorMessage = "";
  }

  public ReaderResult(string message)
  {
    command = default!;
    rest = default!;
    ok = false;
    errorMessage = message;

  }
  public BulkString command;
  public List<BulkString> rest;
  public bool ok;
  public string errorMessage;
}

public class RespReader
{
  //the call goes like this:
  // *2\r\n
  // $4\r\n
  // echo\r\n
  // $11\r\n
  // hello_world\r\n
  public static ReaderResult ReadBytes(Socket c)
  {
    byte[] buf = new byte[1024];
    int bytes_read = c.Receive(buf);
    if (bytes_read == 0) return new("Readed 0 bytes");

    string[] lines = Encoding.UTF8.GetString(buf, 0, bytes_read).Split(
          "\r\n",
          StringSplitOptions.RemoveEmptyEntries
      );

    Console.WriteLine("Request ---------------------");
    foreach (var l in lines)
    {
      Console.WriteLine(l);

    }
    Console.WriteLine("-----------------------------\n");

    if (lines.Length <= 0) return new($"Lines length were {lines.Length}");

    //array validation
    var array = ParseArray(lines.ElementAt(0));
    if (array.Type != RespPrefixes.Array || array.Length <= 0) return new($"Didnt passed an array as first argument: recieved {array.Type}, {array.Length}");

    //getting the command
    var command = VerifyLength(lines[1..3]);
    if (command == null) return new("Command is null");

    //skip the lines already readed
    List<BulkString> rest = [];

    int start_index = 3;

    for (int i = 1; i < array.Length; i++)
    {
      if (start_index + 1 >= lines.Length) break;

      var bs = VerifyLength(lines[start_index..(start_index + 2)]);
      if (bs != null) rest.Add(bs);

      start_index += 2;
    }

    return new(command, rest);
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

  private static Data ParseArray(string line)
  {
    if (line.Length < 2) return new();

    var (array_symbol, array_length) = (line[..1], line[1..]);

    if (array_symbol != RespPrefixes.Array) return new();

    var ok = int.TryParse(array_length, out var array_length_int);
    if (!ok) return new();

    return new(RespPrefixes.Array, "", array_length_int);
  }
}

