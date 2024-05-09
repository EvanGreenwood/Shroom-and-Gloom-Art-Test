using UnityEngine;
namespace MathBad
{
public class FrameRateUI : MonoBehaviourUI
{
    FieldList _fieldList;
    FieldUI _udt, _dtFixed, _timeScale;
    FieldUI _avg, _highest, _lowest;

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Awake()
    {
        _fieldList = GetComponent<FieldList>();

        // _udt = _fieldList.AppendNewField("UDT", $"{FrameRate.inst.udtAverage:0.0000}");
        // _dtFixed = _fieldList.AppendNewField("FDT", $"{FrameRate.inst.fpsLowest:0000}");
        _timeScale = _fieldList.AppendNewField("TSCALE", $"{FrameRate.inst.fpsLowest:0000}");

        _avg = _fieldList.AppendNewField("AVG", $"{FrameRate.inst.fpsAverage:0000}");
        // _highest = _fieldList.AppendNewField("HIGH", $"{FrameRate.inst.fpsHighest:0000}");
        // _lowest = _fieldList.AppendNewField("LOW", $"{FrameRate.inst.fpsLowest:0000}");
        // FrameRate.inst.onDelayComplete.Subscribe(OnDelayComplete);
        // _highest.SetActive(false);
        // _lowest.SetActive(false);
    }

    void OnDelayComplete()
    {
        _highest.gameObject.SetActive(true);
        _lowest.gameObject.SetActive(true);
    }

    // Step
    //----------------------------------------------------------------------------------------------------
    void LateUpdate() {Step();}
    void Step()
    {
        // _udt.Step($"{FrameRate.inst.udtAverage.Round(0.01f)}");
        // _dtFixed.Step($"{Time.fixedDeltaTime.Round(0.01f)}");
        _timeScale.Step($"{Time.timeScale.Round(0.01f)}");
        _avg.Step($"{FrameRate.inst.fpsAverage:0000}");
        // string highest = (FrameRate.inst.highest > 0) ? $": {_highest:0000}" : "";
        // string lowest = (FrameRate.inst.lowest > 0) ? $": {_lowest:0000}" : "";
        // _highest.Step($"{FrameRate.inst.fpsHighest:0000} {FrameRate.inst.highest}");
        // _lowest.Step($"{FrameRate.inst.fpsLowest:0000} {FrameRate.inst.lowest}");
    }
}
}
