#region
using UnityEngine.SceneManagement;
#endregion
namespace Mainframe
{
public static class SCENE
{
  public static void LoadScene(int buildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
  {
    SceneManager.LoadScene(buildIndex, loadSceneMode);
  }
}
}
