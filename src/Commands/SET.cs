using redis_cs.Commands.Utils;

namespace redis_cs.Commands;

public class CMDSet : ICommand
{
  public CMDResponse Do(string line)
  {
    var runvar = RuntimeVariables.Add(line);
    if (runvar.HasError(out var _)) return new(EncodeBytes.NullBulkStr(), false);

    return new(EncodeBytes.OK());
  }
}
