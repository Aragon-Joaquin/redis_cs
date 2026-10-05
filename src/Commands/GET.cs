using redis_cs.Commands.Utils;
using redis_cs.src.RESP;

namespace redis_cs.Commands;

public class CMDGet : ICommand
{
  public CMDResponse Do(List<BulkString> lines)
  {
    if (lines.Count < 1) return new(EncodeBytes.NullBulkStr());

    var runvar = RuntimeVariables.Get(lines.ElementAt(0).Content);
    if (runvar.HasError(out var _)) return new(EncodeBytes.NullBulkStr(), false);

    runvar.HasValue(out string? v);
    return new(EncodeBytes.BulkStr(v ?? ""));
  }
}
