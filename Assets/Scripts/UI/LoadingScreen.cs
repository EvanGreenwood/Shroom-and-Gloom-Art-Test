#region Usings
using Framework;
using UnityEngine;
using MathBad;
using TMPro;
#endregion

public class LoadingScreen : MonoBehaviourUI
{
    [SerializeField] TextBank _shroomFacts;
    [SerializeField] TextBank _gloomFacts;
    [SerializeField] TextMeshProUGUI _factLabel;
    [SerializeField] string _factTags = "<bounce>$</bounce>";

    private void Awake()
    {
        bool coin = RandomUtils.CoinToss();
        string fact = coin ? _shroomFacts.NextRandom() : _gloomFacts.NextRandom();
        _factLabel.text = _factTags.Replace("$", fact);
    }
}
