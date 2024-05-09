#region
using System.Runtime.CompilerServices;
using UnityEngine;
#endregion

namespace MathBad
{
public static class RectExt
{
  [MethodImpl(256)]
  public static Vector2 PositionRelative(this Rect rect, Vector2 position)
  {
    float x = MATH.Unlerp(rect.min.x, rect.max.x, position.x);
    float y = MATH.Unlerp(rect.min.y, rect.max.y, position.y);
    return new Vector2(x, y);
  }

  public static Rect Encapsulate(this Rect rect, Vector2 position)
  {
    float minX = MATH.Min(rect.xMin, position.x);
    float maxX = MATH.Max(rect.xMax, position.x);
    float minY = MATH.Min(rect.yMin, position.y);
    float maxY = MATH.Max(rect.yMax, position.y);

    rect.xMin = minX;
    rect.yMin = minY;
    rect.width = maxX - minX;
    rect.height = maxY - minY;

    return rect;
  }

  // Components
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)] public static Rect WithX(this Rect rect, float x) => new Rect(x, rect.y, rect.width, rect.height);
  [MethodImpl(256)] public static Rect WithY(this Rect rect, float y) => new Rect(rect.x, y, rect.width, rect.height);
  [MethodImpl(256)] public static Rect WithPosition(this Rect rect, float x, float y) => new Rect(x, y, rect.width, rect.height);

  [MethodImpl(256)] public static Rect WithWidth(this Rect rect, float w) => new Rect(rect.x, rect.y, w, rect.height);
  [MethodImpl(256)] public static Rect WithHeight(this Rect rect, float h) => new Rect(rect.x, rect.y, rect.width, h);
  [MethodImpl(256)] public static Rect WithSize(this Rect rect, float w, float h) => new Rect(rect.x, rect.y, w, h);

  [MethodImpl(256)] public static Rect WithMin(this Rect rect, Vector2 min) => Rect.MinMaxRect(min.x, min.y, rect.max.x, rect.max.y);
  [MethodImpl(256)] public static Rect WithMax(this Rect rect, Vector2 max) => Rect.MinMaxRect(rect.min.x, rect.min.y, max.x, max.y);
  [MethodImpl(256)] public static Rect WithMinMax(this Rect rect, Vector2 min, Vector2 max) => Rect.MinMaxRect(min.x, min.y, max.x, max.y);

  // Size
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)] public static Rect MoveUp(this Rect rect, float delta) => new Rect(rect.x, rect.y - delta, rect.width, rect.height);
  [MethodImpl(256)] public static Rect MoveDown(this Rect rect, float delta) => new Rect(rect.x, rect.y + delta, rect.width, rect.height);
  [MethodImpl(256)] public static Rect MoveLeft(this Rect rect, float delta) => new Rect(rect.x - delta, rect.y, rect.width, rect.height);
  [MethodImpl(256)] public static Rect MoveRight(this Rect rect, float delta) => new Rect(rect.x + delta, rect.y, rect.width, rect.height);

  [MethodImpl(256)] public static Rect MoveTopEdge(this Rect rect, float delta) => new Rect(rect.x, rect.y - delta, rect.width, rect.height + delta);
  [MethodImpl(256)] public static Rect MoveBottomEdge(this Rect rect, float delta) => new Rect(rect.x, rect.y, rect.width, rect.height + delta);
  [MethodImpl(256)] public static Rect MoveLeftEdge(this Rect rect, float delta) => new Rect(rect.x - delta, rect.y, rect.width + delta, rect.height);
  [MethodImpl(256)] public static Rect MoveRightEdge(this Rect rect, float delta) => new Rect(rect.x, rect.y, rect.width + delta, rect.height);

  [MethodImpl(256)] public static Rect Grow(this Rect rect, float padding) => new Rect(rect.x - padding * 0.5f, rect.y - padding * 0.5f, rect.width + padding, rect.height + padding);
  [MethodImpl(256)] public static Rect Shrink(this Rect rect, float padding) => new Rect(rect.x + padding * 0.5f, rect.y + padding * 0.5f, rect.width - padding, rect.height - padding);
  [MethodImpl(256)] public static Rect ShrinkWidthFromCenter(this Rect rect, float delta) => new Rect(rect.x + delta * 0.5f, rect.y, rect.width - delta * 0.5f, rect.height);
  [MethodImpl(256)] public static Rect ShrinkWidthFromCenter(this Rect rect, float left, float right) => new Rect(rect.x + left, rect.y, rect.width - right, rect.height);
  [MethodImpl(256)] public static Rect ShrinkHeightFromCenter(this Rect rect, float delta) => new Rect(rect.x, rect.y + delta * 0.5f, rect.width, rect.height - delta * 0.5f);
  [MethodImpl(256)] public static Rect ShrinkHeightFromCenter(this Rect rect, float left, float right) => new Rect(rect.x, rect.y + left, rect.width, rect.height - right);

  // Split
  //----------------------------------------------------------------------------------------------------
  public static Rect[] SplitWidthEqually(this Rect rect, int count, float spacing = 5f)
  {
    count = MATH.Max(1, count);

    Rect[] rects = new Rect[count];
    float totalSpacing = spacing * (count - 1);
    float width = (rect.width - totalSpacing) / count;

    for(int i = 0; i < count; i++)
    {
      float xPosition = rect.x + i * (width + spacing);
      rects[i] = new Rect(xPosition, rect.y, width, rect.height);
    }

    return rects;
  }

  public static Rect[] SplitWidthPercent(this Rect rect, float percent, float spacing = 5f)
  {
    float rw = rect.width * percent - spacing.Half();
    Rect lr = Rect.MinMaxRect(rect.min.x, rect.min.y, rect.min.x + rw, rect.max.y);
    Rect rr = Rect.MinMaxRect(lr.max.x + spacing.Half(), lr.min.y, rect.max.x, rect.max.y);
    return new[] {lr, rr};
  }

  public static Rect[] SplitHeightPercent(this Rect rect, float percent, float spacing = 5f)
  {
    float rh = rect.height * percent - spacing.Half();
    Rect br = Rect.MinMaxRect(rect.min.x, rect.min.y, rect.max.x, rect.min.y + rh);
    Rect tr = Rect.MinMaxRect(br.min.x, br.max.y + spacing.Half(), rect.max.x, rect.max.y);
    return new[] {br, tr};
  }

  public static Rect[] SplitFromLeft(this Rect rect, float delta, float spacing = 5f)
  {
    Rect lr = Rect.MinMaxRect(rect.min.x, rect.min.y, rect.min.x + delta - spacing.Half(), rect.max.y);
    Rect rr = Rect.MinMaxRect(lr.max.x + spacing.Half(), lr.min.y, rect.max.x, rect.max.y);
    return new[] {lr, rr};
  }

  public static Rect[] SplitFromRight(this Rect rect, float delta, float spacing = 5f)
  {
    Rect lr = Rect.MinMaxRect(rect.min.x, rect.min.y, rect.max.x - delta - spacing.Half(), rect.max.y);
    Rect rr = Rect.MinMaxRect(lr.max.x + spacing.Half(), lr.min.y, rect.max.x, rect.max.y);
    return new[] {lr, rr};
  }
}
}
