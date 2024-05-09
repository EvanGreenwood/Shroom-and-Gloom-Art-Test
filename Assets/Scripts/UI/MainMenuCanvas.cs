#region Usings
using UnityEngine;
using Mainframe;
#endregion

public class MainMenuCanvas: CanvasSingleton<MainMenuCanvas>
{
    void Update()
    {
        if(INPUT.leftMouse.down)
            SCENE.LoadScene(2);
    }
}