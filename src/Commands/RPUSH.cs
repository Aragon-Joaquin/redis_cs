using redis_cs.src.RESP;

namespace redis_cs.Commands;

public class RPush : ICommand
{
  private static readonly Dictionary<string, List<string>> _lists = [];

  public CMDResponse Do(List<BulkString> lines)
  {
    if (lines.Count < 2) return new(EncodeBytes.NullBulkStr(), false);
    var key = lines.ElementAt(0).Content.Trim();

    List<string> l = [];

    for (var i = 1; i < lines.Count; i++)
    {
      var c = lines.ElementAt(i).Content.Trim();
      if (c != null) l.Add(c);
    }


    var count = (uint)AppendToList(key, l);
    return new(EncodeBytes.Integer(count));
  }

  private static int AppendToList(string key, List<string> values)
  {
    if (!_lists.TryGetValue(key, out var val) || val == null)
    {
      _lists.Add(key, values);
      return values.Count;
    }

    var n = val.Concat(values).ToList();
    _lists[key] = n;
    return n.Count;
  }
}
