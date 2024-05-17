#region Usings
using Framework;
using UnityEngine;
using MathBad;
using TMPro;
#endregion

public class HUD : CanvasSingleton<HUD>
{
    [SerializeField] private TextMeshProUGUI _infoLabel;
    [SerializeField] private LetterBox _letterBox;

    private string _activeSceneName;

    private Service<SceneManager> _sceneController;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _letterBox.gameObject.SetActive(true);
        _activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
    private void Update()
    {
        _infoLabel.text = $"{_sceneController.Value.Data.title}  |  " +
                          $"FPS: {FrameRate.inst.fpsAverage:000}";
    }
}
