using UnityEngine;

public class TunnelSwich : MonoBehaviour
{
    [SerializeField] private TunnelGenerator _tunnel1;
    [SerializeField] private TunnelGenerator _tunnel2;

    private Service<Player> _player;
    
    void Update()
    {
        if (!_player.Exists)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            _player.Value.SwitchTunnel(_tunnel1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _player.Value.SwitchTunnel(_tunnel2);
        }
    }
}
