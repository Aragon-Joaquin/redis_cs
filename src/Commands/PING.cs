namespace redis_cs.Commands;

public class CMDPing : ICommand
{
  public CMDResponse Do(string line) => new(EncodeBytes.WithCRLF("PONG"));
}
