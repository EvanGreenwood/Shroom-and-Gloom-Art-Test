#region Usings
using UnityEngine;
using NaughtyAttributes;
#endregion

[RequireComponent(typeof(Light))]
public class TunnelLight : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] TunnelRig _rig;
    [SerializeField] float _splinePercent;
    [SerializeField] float _angle;
    [SerializeField] float _distance;

    public TunnelRig rig => _rig;
    public new Light light => _light;

    public float splinePercent { get => _splinePercent; set => _splinePercent = value; }
    public float angle { get => _angle; set => _angle = value; }
    public float distance { get => _distance; set => _distance = value; }

    public void Init(TunnelRig rig)
    {
        _rig = rig;
        _light = GetComponent<Light>();
    }

    void OnDestroy()
    {
        if(_rig == null)
            return;
        _rig.RemoveLight(this);
    }
}
