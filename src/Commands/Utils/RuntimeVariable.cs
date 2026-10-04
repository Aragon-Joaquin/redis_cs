namespace redis_cs.Commands.Utils;

internal class RuntimeVariables
{
  static private readonly Dictionary<string, string> _map_var = [];

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

    Add(key, val);
    return true;
  }

  // cant i just do:
  // err := RuntimeVars.Add() 
  // ?????
  static internal Result<bool> Add(string k, string v)
  {
    var key = k.Trim();
    if (key == "") return new Error("key is empty");
    _map_var.Add(key, v);
    return true;
  }


  static internal Result<string> Get(string k)
  {
    var key = k.Split(" ", 2)[0];
    bool ok = _map_var.TryGetValue(key, out string? val);
    if (!ok) return new Error("element doesnt exists");

    return val ?? "";
  }

}
