using UnityEngine;

public class TunnelSwich : MonoBehaviour
{
    [SerializeField] private FPSMovement _fpsMovement;
    [SerializeField] private TunnelGenerator _tunnel1;
    [SerializeField] private TunnelGenerator _tunnel2;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _fpsMovement.SwitchTunnel(_tunnel1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _fpsMovement.SwitchTunnel(_tunnel2);
        }
    }
}
