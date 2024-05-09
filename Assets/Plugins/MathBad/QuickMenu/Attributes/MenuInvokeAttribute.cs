#region
using System;
#endregion

namespace MathBad
{
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MenuInvokeAttribute : Attribute, IMenuAttribute
{
  public string menuPath { get; } = "";
  public int priority { get; } = 0;
  public string separator { get; } = null;

  public MenuInvokeAttribute(string path, int priority, string separator)
  {
    this.menuPath = path;
    this.priority = priority;
    this.separator = separator;
  }
  public MenuInvokeAttribute(string path) : this(path, 0, null) { }
  public MenuInvokeAttribute(string path, int priority) : this(path, priority, null) { }
  public MenuInvokeAttribute(string path, string separator) : this(path, 0, separator) { }
}
}
