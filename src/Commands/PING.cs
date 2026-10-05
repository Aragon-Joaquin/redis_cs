using redis_cs.src.RESP;

namespace redis_cs.Commands;

public class CMDPing : ICommand
{
  public CMDResponse Do(List<BulkString> _) => new(EncodeBytes.WithCRLF("PONG"));
}
