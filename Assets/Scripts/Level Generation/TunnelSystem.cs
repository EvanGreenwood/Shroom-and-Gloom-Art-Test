#region Usings
using UnityEngine;
using MathBad;
#endregion

public class TunnelSystem : MonoSingleton<TunnelSystem>
{
    TunnelGenerator[] _tunnels;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _tunnels = GetComponentsInChildren<TunnelGenerator>();
    }

    // Find Tunnel
    //----------------------------------------------------------------------------------------------------
    public bool TryGetTunnel(Vector3 pos, out TunnelGenerator tunnel)
    {
        // Find closest tunnel
        float closestDistance = float.MaxValue;
        TunnelGenerator closestGenerator = null;
        foreach(TunnelGenerator generator in _tunnels)
        {
            float distance = Vector3.Distance(generator.transform.position, pos);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestGenerator = generator;
            }
        }

        if(closestGenerator != null)
        {
            tunnel = closestGenerator;
            return true;
        }

        tunnel = null;
        return false;
    }

    public Vector3 GetDirectionAt(Vector3 worldPos, float lookaheadDst)
    {
        TryGetTunnel(worldPos, out TunnelGenerator tunnel);
        
        float t = tunnel.GetClosestPositionAndDirection(worldPos, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
        Vector3 lookaheadPoint = tunnel.GetLookaheadPoint(t, lookaheadDst);
        Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
        transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime * 0.25f, 0);

        return currentDirection;
    }
}
