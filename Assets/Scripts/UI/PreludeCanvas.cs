#region Usings
using UnityEngine;
using Mainframe;
#endregion

public class PreludeCanvas : CanvasSingleton<PreludeCanvas>
{
    void Update()
    {
        if(INPUT.leftMouse.down)
            SCENE.LoadScene(1);
    }
}