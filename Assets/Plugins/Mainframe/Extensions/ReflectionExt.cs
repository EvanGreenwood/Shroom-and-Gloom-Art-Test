#region
using System;
using System.Reflection;
#endregion
namespace Mainframe
{
public static class ReflectionExt
{
  public static bool TryGetMethodAttribute<T>(this MethodInfo methodInfo, out T result) where T : Attribute
  {
    Attribute attribute = Attribute.GetCustomAttribute(methodInfo, typeof(T), true);
    if(attribute != null)
    {
      result = (T)attribute;
      return true;
    }
    result = null;
    return false;
  }
}
}
