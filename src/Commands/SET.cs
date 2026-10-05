using redis_cs.Commands.Utils;
using redis_cs.src.RESP;

namespace redis_cs.Commands;

public class CMDSet : ICommand
{
  public CMDResponse Do(List<BulkString> lines)
  {
    if (lines.Count < 2) return new(EncodeBytes.NullBulkStr());

    var runvar = RuntimeVariables.Add(lines.ElementAt(0).Content, lines.ElementAt(1).Content);
    if (runvar.HasError(out var _)) return new(EncodeBytes.NullBulkStr(), false);

    return new(EncodeBytes.OK());
  }
}
