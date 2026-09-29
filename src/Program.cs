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


    ReaderResult r;
    string join_rest;
    try
    {
      r = RespReader.ReadBytes(bytes_read, buf);
      join_rest = "assad";
    }
    catch (Exception e)
    {
      Console.WriteLine($"got error: {e}");
      return;
    }


    Console.Write($"Action used: {r.command.Content}\n");

    switch (r.command.Content.ToUpper())
    {
      case ACTIONS.PING:
        c.Send(EncodeBytes.WithCRLF("PONG"));
        break;

      case ACTIONS.ECHO:
        c.Send(EncodeBytes.BulkStr(join_rest));
        break;

      case ACTIONS.SET:
        {
          var runvar = RuntimeVariables.Add(join_rest);
          if (runvar.HasError(out var err))
          {
            c.Send(EncodeBytes.WithCRLF(err.String()));
            break;
          }
          c.Send(EncodeBytes.OK());
          break;
        }

      case ACTIONS.GET:
        {
          var runvar = RuntimeVariables.Get(join_rest);
          if (runvar.HasError(out var err))
          {
            c.Send(EncodeBytes.WithCRLF(err.String()));
            break;
          }

          runvar.HasValue(out string? v);

          c.Send(EncodeBytes.BulkStr(v ?? ""));
          break;
        }

      default:
        c.Send(EncodeBytes.WithCRLF("???"));
        break;
    }
  }
}
