#region
using System;
#endregion

namespace MathBad
{
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class MenuCreateAttribute : Attribute, IMenuAttribute
{
  public string menuPath { get; }
  public int priority { get; }
  public string separator { get; }

  public MenuCreateAttribute(string menuPath, int priority = 0, string separator = null)
  {
    this.menuPath = menuPath;
    this.priority = priority;
    this.separator = separator;
  }
}
}
