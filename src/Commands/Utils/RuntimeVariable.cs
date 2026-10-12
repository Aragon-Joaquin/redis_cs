namespace redis_cs.Commands.Utils;

public struct Variable
{
  public string Content;
  public readonly DateTime? ExpiresIn = null;

  public Variable(string c)
  {
    Content = c;
  }

  public Variable(string c, DateTime? e)
  {
    Content = c;
    ExpiresIn = e;
  }
}

internal class RuntimeVariables
{
  static private readonly Dictionary<string, Variable> _map_var = [];

  static internal Result<bool> Add(string rest)
  {
    string[] r = rest.Split(" ");
    if (r.Length != 2) return new Error($"Invalid length, have {r.Length} and needed 2");

    var (key, val) = r switch
    {
      [var k, var v] => (k, v),
      [var k] => (k, ""),
      _ => ("", "")
    };

    return AddPrivate(key, val, null);
  }

  static private Result<bool> AddPrivate(string k, string v, DateTime? t)
  {
    var key = k.Trim();
    if (key == "") return new Error("key is empty");
    _map_var[key] = new(v, t);
    return true;
  }

  static internal Result<bool> Add(string k, string v) => AddPrivate(k, v, null);
  static internal Result<bool> Add(string k, string v, DateTime? t) => AddPrivate(k, v, t);

  static internal Result<Variable> Get(string k)
  {
    var key = k.Split(" ", 2)[0];
    bool ok = _map_var.TryGetValue(key, out var val);
    if (!ok) return new Error("element doesnt exists");

    if (val.ExpiresIn != null && val.ExpiresIn < DateTime.Now) return new Error("Element expired");

    return val;
  }
}
