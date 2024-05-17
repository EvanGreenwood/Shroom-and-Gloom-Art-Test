#region Usings
using Framework;
using UnityEngine;
using MathBad;
using NaughtyAttributes;
using TMPro;
#endregion

public class RandomTexts : MonoBehaviour
{
    [SerializeField] string[] _texts;
    TextMeshPro _label;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _label = GetComponent<TextMeshPro>();
        _label.text = _texts.ChooseRandom();
    }

    [Button]
    void Choose()
    {
        if(_label == null)
            _label = GetComponent<TextMeshPro>();
        _label.text = _texts.ChooseRandom();
    }
}
