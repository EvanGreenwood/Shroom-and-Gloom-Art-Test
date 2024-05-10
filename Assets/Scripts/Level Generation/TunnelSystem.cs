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
    public TunnelGenerator FindNewTunnel(Vector3 pos)
    {
        //Slow and bad
        TunnelGenerator[] generators = FindObjectsByType<TunnelGenerator>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        // Find closest tunnel
        float closestDistance = float.MaxValue;
        TunnelGenerator closestGenerator = null;
        foreach(TunnelGenerator generator in generators)
        {
            float distance = Vector3.Distance(generator.transform.position, pos);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestGenerator = generator;
            }
        }

        return closestGenerator;
    }

    public Vector3 GetDirectionAt(Vector3 worldPos, float lookaheadDst)
    {
        TunnelGenerator tunnel = FindNewTunnel(worldPos);
        float t = tunnel.GetClosestPositionAndDirection(worldPos, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
        Vector3 lookaheadPoint = tunnel.GetLookaheadPoint(t, lookaheadDst);
        Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
        transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, Time.deltaTime * 0.25f, 0);

        return currentDirection;
    }
}
