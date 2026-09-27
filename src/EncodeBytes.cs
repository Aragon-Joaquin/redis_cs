using System.Text;

namespace redis_cs;

public class EncodeBytes
{

  public static byte[] BulkStr(string str) { return Raw($"${str.Length}\r\n{str}\r\n"); }
  public static byte[] Raw(string str) { return Encoding.UTF8.GetBytes(str); }
  public static byte[] WithCRLF(string str) { return Encoding.UTF8.GetBytes(str + "\r\n"); }
  public static byte[] OK() { return WithCRLF("OK"); }
}
