#region Usings
using Framework;
using UnityEngine;
using MathBad;
using NaughtyAttributes;
using TMPro;
#endregion

public class RandomTexts : MonoBehaviour
{
    [SerializeField] bool _chooseOnAwake = true;
    [SerializeField] string[] _texts;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        if(_chooseOnAwake)
            Choose();
    }

    [Button]
    public void Choose()
    {
        if(gameObject.TryGetComponent(out TextMeshPro label)) label.text = _texts.ChooseRandom();
        else if(gameObject.TryGetComponent(out TextMeshProUGUI labelUI)) labelUI.text = _texts.ChooseRandom();
    }
}
