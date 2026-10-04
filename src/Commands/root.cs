namespace redis_cs.Commands;

public static class COMMADS_NAMES
{
  public const string PING = "PING";
  public const string ECHO = "ECHO";
  public const string SET = "SET";
  public const string GET = "GET";
}

public struct CMDResponse
{
  public bool OK;
  public byte[] Body;

  public CMDResponse(byte[] res)
  {
    OK = true;
    Body = res;
  }

  public CMDResponse(byte[] res, bool ok)
  {
    OK = ok;
    Body = res;
  }
}

public interface ICommand
{
  CMDResponse Do(string line);
}

static public class ALL_COMMANDS
{
  public static ICommand? Get(string key) => key.Trim().ToUpper() switch
  {
    COMMADS_NAMES.PING => new CMDPing(),
    COMMADS_NAMES.ECHO => new CMDEcho(),
    COMMADS_NAMES.SET => new CMDSet(),
    COMMADS_NAMES.GET => new CMDGet(),
    _ => null
  };
}
