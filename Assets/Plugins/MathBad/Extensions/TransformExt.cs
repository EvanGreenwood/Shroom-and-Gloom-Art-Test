#region Usings
using UnityEngine;
#endregion

namespace MathBad
{
public static class TransformExt
{
    public static void ResetLocal(this Transform transform, bool resetLocalScale = false)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if(resetLocalScale) transform.localScale = Vector3.one;
    }

    public static Transform CreateChild(this Transform parent, string name)
    {
        Transform child = new GameObject(name).transform;
        parent.TakeChild(child, true);
        return child;
    }
    public static RectTransform CreateChild(this RectTransform parent, string name)
    {
        RectTransform child = new GameObject(name).AddComponent<RectTransform>();
        parent.TakeChild(child, true);
        child.localScale = Vector3.one;
        child.anchorMin = new Vector2(0f, 0f);
        child.anchorMax = new Vector2(1f, 1f);
        return child;
    }
    public static void TakeChild(this Transform parent, GameObject child, bool resetLocal = false)
    {
        if(child == null)
            return;
        child.transform.SetParent(parent);
        if(resetLocal)
        {
            child.transform.ResetLocal();
        }
    }
    public static void TakeChild<T, U>(this T parent, U child, bool resetLocal = false)
        where T : Component
        where U : Component
    {
        if(child == null)
            return;
        child.transform.SetParent(parent.transform);
        if(resetLocal)
        {
            child.transform.ResetLocal();
        }
    }
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
