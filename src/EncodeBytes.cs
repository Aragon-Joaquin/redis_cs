using System.Text;
using redis_cs.src.RESP;

namespace redis_cs;

public class EncodeBytes
{
  public static byte[] BulkStr(List<BulkString> str) => BulkStr(string.Join(" ", str.Select(r => r.Content)));
  public static byte[] BulkStr(List<string> str) => BulkStr(string.Join(" ", str));
  public static byte[] BulkStr(string[] str) => BulkStr(string.Join(" ", str));
  public static byte[] BulkStr(string str) => Raw($"${str.Length}\r\n{str}\r\n");

  public static byte[] Raw(string str) => Encoding.UTF8.GetBytes(str);
  public static byte[] WithCRLF(string str) => Encoding.UTF8.GetBytes("+" + str + "\r\n");

  public static byte[] OK() => WithCRLF("OK");
  public static byte[] NullBulkStr() => Raw("$-1\r\n");

  public static byte[] Integer(uint val) => Raw($":{val}\r\n");

}
