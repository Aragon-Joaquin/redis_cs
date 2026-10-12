using redis_cs.Commands.Utils;
using redis_cs.src.RESP;

namespace redis_cs.Commands;

public class CMDSet : ICommand
{
  private const string EXPIRES_EX = "EX";
  private const string EXPIRES_PX = "PX";

  public CMDResponse Do(List<BulkString> lines)
  {
    if (lines.Count < 2) return new(EncodeBytes.NullBulkStr());

    var key = lines[0].Content;
    var val = lines[1].Content;

    return ParseOpt(key, val, lines[2..]);
  }

  private static CMDResponse ParseOpt(string key, string val, List<BulkString> opts)
  {
    DateTime? expiry = null;
    var i = 0;

    while (i < opts.Count)
    {
      var action = opts[i].Content.ToUpper();

      switch (action)
      {
        case EXPIRES_EX:
        case EXPIRES_PX:
          if (i + 1 >= opts.Count) return new(EncodeBytes.NullBulkStr(), false);

          if (!long.TryParse(opts[++i].Content, out var numb) || numb <= 0)
            return new(EncodeBytes.NullBulkStr(), false);

          expiry = action == EXPIRES_EX
              ? DateTime.Now.AddSeconds(numb)
              : DateTime.Now.AddMilliseconds(numb);
          break;

        default:
          return new(EncodeBytes.NullBulkStr(), false);
      }
      i++;
    }
    return Add_to_dictionary(key, val, expiry) ? new(EncodeBytes.OK()) : new(EncodeBytes.NullBulkStr());
  }

  private static bool Add_to_dictionary(string key, string val, DateTime? time)
  {
    var runvar = RuntimeVariables.Add(key, val, time);
    return !runvar.HasError(out var _);
  }
}
