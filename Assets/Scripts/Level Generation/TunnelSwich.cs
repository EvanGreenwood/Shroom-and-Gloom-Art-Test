using UnityEngine;

public class TunnelSwich : MonoBehaviour
{
    [SerializeField] private TunnelGenerator _tunnel1;
    [SerializeField] private TunnelGenerator _tunnel2;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Player.inst.SwitchTunnel(_tunnel1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Player.inst.SwitchTunnel(_tunnel2);
        }
    }
}
