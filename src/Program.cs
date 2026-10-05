using System.Net;
using System.Net.Sockets;
using redis_cs;
using redis_cs.Commands;
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
    ReaderResult r = RespReader.ReadBytes(c);
    if (!r.ok)
    {
      c.Send(EncodeBytes.WithCRLF(r.errorMessage));
      continue;
    }

    Console.Write($"Action used: {r.command.Content}\n");

    var cmd = ALL_COMMANDS.Get(r.command.Content);
    if (cmd == null)
    {
      c.Send(EncodeBytes.NullBulkStr());
      continue;
    }

    var res = cmd.Do(r.rest);
    c.Send(res.Body);
  }
}
