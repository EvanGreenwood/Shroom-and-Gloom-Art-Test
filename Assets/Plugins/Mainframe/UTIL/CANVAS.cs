#region
using System;
using Framework;
using Mainframe;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#endregion

public static class CANVAS
{
  public static void StepLayout(this LayoutGroup layoutGroup, bool horizontal, bool vertical)
  {
    if(horizontal)
    {
      layoutGroup.CalculateLayoutInputHorizontal();
      layoutGroup.SetLayoutHorizontal();
    }
    if(vertical)
    {
      layoutGroup.CalculateLayoutInputVertical();
      layoutGroup.SetLayoutVertical();
    }
  }

  public static void ResizeToGrid(this RectTransform rt, GridLayoutGroup grid, Int2 size, Vector2 padding) {rt.ResizeToGrid(grid, size.x, size.y, padding.x, padding.y);}
  public static void ResizeToGrid(this RectTransform rt, GridLayoutGroup grid, int cellsX, int cellsY, float padX, float padY)
  {
    RectOffset offset = grid.padding;
    Vector2 spacing = grid.spacing;
    Vector2 cellSize = grid.cellSize;

    float width = offset.left + offset.right
                + cellSize.x * cellsX
                + spacing.x * (cellsX - 1);

    float height = offset.top + offset.bottom
                 + cellSize.y * cellsY
                 + spacing.y * (cellsY - 1);

    rt.sizeDelta = new Vector2(width, height) + new Vector2(padX * 2f, padY * 2f);
  }

  // Encapsulate
  //----------------------------------------------------------------------------------------------------//
  public static void EncapsulateWidth(this RectTransform target, float padding, params RectTransform[] rts)
  {
    if(rts.IsNullOrEmpty())
    {
      throw new NullReferenceException("RectTransform[] is null or empty.");
    }

    float min = float.MaxValue, max = float.MinValue;
    foreach(RectTransform rt in rts)
    {
      if(rt.rect.min.x < min) min = rt.rect.min.x;
      if(rt.rect.max.x > max) max = rt.rect.max.x;
    }

    target.position = target.position.WithX(min);
    target.sizeDelta = target.sizeDelta.WithX(max);
  }

  public static void Encapsulate(this RectTransform rt, Vector2 padding, params RectTransform[] rts)
  {
    if(rts.IsNullOrEmpty())
    {
      return;
    }

    Vector2 localPadding = Vector2.Scale(padding, rt.lossyScale);

    Vector2 minPoint = rts[0].anchoredPosition - rts[0].rect.size * 0.5f;
    Vector2 maxPoint = rts[0].anchoredPosition + rts[0].rect.size * 0.5f;

    foreach(RectTransform rectTransform in rts)
    {
      Vector2 size = rectTransform.rect.size;
      Vector2 position = rectTransform.anchoredPosition;

      minPoint = Vector2.Min(minPoint, position - size * 0.5f);
      maxPoint = Vector2.Max(maxPoint, position + size * 0.5f);
    }

    minPoint -= localPadding;
    maxPoint += localPadding;

    rt.sizeDelta = maxPoint - minPoint;
  }

  public static void Encapsulate(this TextMeshProUGUI label, Vector2 padding) {label.rectTransform.sizeDelta = label.GetPreferredValues(label.text) + padding;}

  public static void Encapsulate(this RectTransform rt, TextMeshProUGUI label, Vector2 padding) {rt.Encapsulate(label, true, true, padding.x, padding.y);}
  public static void Encapsulate(this RectTransform rt, TextMeshProUGUI label, Bit2 wrap, Vector2 padding) {rt.Encapsulate(label, wrap.x, wrap.y, padding.x, padding.y);}
  public static void Encapsulate(this RectTransform rt, TextMeshProUGUI label, bool wrapX, bool wrapY, float paddingX, float paddingY)
  {
    Vector3 size = label.GetPreferredValues(label.text) + new Vector2(paddingX, paddingY) * 2f;
    size = size.RoundCeil(1f);
    if(wrapX) rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
    if(wrapY) rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

#if UNITY_EDITOR
    EditorUtility.SetDirty(rt);
#endif
  }
}
