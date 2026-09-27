using System.Net;
using System.Net.Sockets;
using redis_cs;
using redis_cs.RESP;


Console.WriteLine("Logs from your program will appear here!");

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

while (true)
{
  Socket c = server.AcceptSocket();
  _ = Task.Run(() => HandleClient(c));
}


static void HandleClient(Socket c)
{
  while (true)
  {
    byte[] buf = new byte[1024];
    int bytes_read = c.Receive(buf);
    if (bytes_read == 0) return;


    RespReader.ReadBytes(bytes_read, buf);

    string action = "";
    string rest = "";

    Console.Write($"Action used: {action}\n");

    switch (action.ToUpper())
    {
      case ACTIONS.PING:
        c.Send(EncodeBytes.WithCRLF("PONG"));
        break;

      case ACTIONS.ECHO:
        c.Send(EncodeBytes.BulkStr(rest));
        break;

      case ACTIONS.SET:
        {
          var r = RuntimeVariables.Add(rest);
          if (r.HasError(out var err))
          {
            c.Send(EncodeBytes.WithCRLF(err.String()));
            break;
          }
          c.Send(EncodeBytes.OK());
          break;
        }

      case ACTIONS.GET:
        {
          var r = RuntimeVariables.Get(rest);
          if (r.HasError(out var err))
          {
            c.Send(EncodeBytes.WithCRLF(err.String()));
            break;
          }

          r.HasValue(out string? v);

          c.Send(EncodeBytes.BulkStr(v ?? ""));
          break;
        }

      default:
        c.Send(EncodeBytes.WithCRLF("???"));
        break;
    }
  }
}
