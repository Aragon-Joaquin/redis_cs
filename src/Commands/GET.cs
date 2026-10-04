using redis_cs.Commands.Utils;

namespace redis_cs.Commands;

public class CMDGet : ICommand
{
  public CMDResponse Do(string line)
  {
    var runvar = RuntimeVariables.Get(line);
    if (runvar.HasError(out var _))
    {
      return new(EncodeBytes.NullBulkStr(), false);
    }

    runvar.HasValue(out string? v);
    return new(EncodeBytes.BulkStr(v ?? ""));
  }
}
