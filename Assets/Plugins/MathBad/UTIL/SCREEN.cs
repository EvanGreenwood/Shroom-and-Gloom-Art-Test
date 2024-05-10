//  ___   ___  ___  ___  ___  _  _                                                                    
// / __| / __|| _ \| __|| __|| \| |                                                                   
// \__ \| (__ |   /| _| | _| | .` |                                                                   
// |___/ \___||_|_\|___||___||_|\_|                                                                   
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using Framework;
using UnityEngine;
#endregion

namespace MathBad
{
public static class SCREEN
{
    public static Vector2 center => new Vector2(Screen.width.Half(), Screen.height.Half());
    public static Vector2 size => new Vector2(Screen.width, Screen.height);
    public static Rect rect => new Rect(0, 0, Screen.width, Screen.height);

    public static Vector3 ScreenToWorldPoint(this Canvas canvas, Vector2 screenPos)
    {
        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return screenPos;
        }
        if(canvas.worldCamera == null)
        {
            Debug.LogError($"{canvas.name} has no camera assigned.");
            return Vector3.zero;
        }
        if(canvas.worldCamera.orthographic)
        {
            float dst = canvas.planeDistance;

            return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y)).WithZ(canvas.transform.position.z);
        }
        else
        {
            float dist = Vector3.Distance(canvas.transform.position, canvas.worldCamera.transform.position);
            return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, dist));
        }
    }
}
}
