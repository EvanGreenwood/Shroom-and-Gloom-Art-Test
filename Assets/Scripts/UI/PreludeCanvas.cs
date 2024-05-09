#region Usings
using UnityEngine;
using MathBad;
#endregion

public class PreludeCanvas : CanvasSingleton<PreludeCanvas>
{
    void Update()
    {
        if(INPUT.leftMouse.down)
            SCENE.LoadScene(1);
    }
}