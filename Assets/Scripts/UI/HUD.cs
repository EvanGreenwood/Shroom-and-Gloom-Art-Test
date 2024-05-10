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
    [SerializeField] LetterBox _letterBox;

    string _activeSceneName;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init() { }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _letterBox.gameObject.SetActive(true);
        _activeSceneName = SceneManager.GetActiveScene().name;
    }
    void Update()
    {
        _infoLabel.text = $"{SceneController.inst.sceneData.title}  |  " +
                          $"FPS: {FrameRate.inst.fpsAverage:000}";
    }
}
