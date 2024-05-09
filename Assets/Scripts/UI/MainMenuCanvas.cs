#region Usings
using UnityEngine;
using MathBad;
#endregion

public class MainMenuCanvas : CanvasSingleton<MainMenuCanvas>
{
    public void StartAdventure()
    {
        SCENE.LoadScene(2);
    }
}
