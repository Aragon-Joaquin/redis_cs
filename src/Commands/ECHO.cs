namespace redis_cs.Commands;

public class CMDEcho : ICommand
{
  public CMDResponse Do(string line) => new(EncodeBytes.BulkStr(line));
}
