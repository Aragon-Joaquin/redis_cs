using redis_cs.src.RESP;

namespace redis_cs.Commands;

public class CMDEcho : ICommand
{
  public CMDResponse Do(List<BulkString> lines) => new(EncodeBytes.BulkStr(lines));
}
