namespace MathBad
{
public interface IMenuAttribute
{
  public string menuPath { get; }
  public int priority { get; }
  public string separator { get; }
}
}
