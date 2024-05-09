#region Usings
using UnityEngine;
#endregion

namespace MathBad
{
public static class TransformExt
{
    // Rect Transform
    //----------------------------------------------------------------------------------------------------
    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        Vector2 worldPos = new Vector3(worldCorners[0].x, worldCorners[0].y);

        float width = worldCorners[2].x - worldCorners[0].x;
        float height = worldCorners[2].y - worldCorners[0].y;
        Vector2 worldSize = new Vector3(width, height);

        return new Rect(worldPos, worldSize);
    }

    public static Rect GetScreenRect(this RectTransform rectTransform, Camera camera)
    {
        Rect worldRect = rectTransform.GetWorldRect();
        Vector3 min = camera.WorldToScreenPoint(new Vector2(worldRect.xMin, worldRect.yMin));
        Vector3 max = camera.WorldToScreenPoint(new Vector2(worldRect.xMax, worldRect.yMax));

        float x = min.x;
        float y = Screen.height - max.y;

        return Rect.MinMaxRect(x, Screen.height - y, max.x, max.y);
    }
}
}
