#region Usings
using Framework;
using UnityEngine;
using MathBad;
using TMPro;
using UnityEngine.SceneManagement;
#endregion

public class HUD : CanvasSingleton<HUD>
{
    [SerializeField] TextMeshProUGUI _infoLabel;
    string _activeSceneName;
    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init() { }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake() {_activeSceneName = SceneManager.GetActiveScene().name;}
    void Update()
    {
        _infoLabel.text = $"{_activeSceneName}  |  " +
                          $"[Avg Fps: {FrameRate.inst.fpsAverage:000}]";
    }
}
