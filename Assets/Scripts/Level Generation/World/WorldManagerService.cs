using JetBrains.Annotations;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class WorldManagerService : MonoService
{
    public bool SingleTunnelTestMode;
    [HideIf("SingleTunnelTestMode")] public WorldMapSettings MapSettings;
    [HideIf("SingleTunnelTestMode")] public TunnelGenerator TunnelPrefab;
    [HideIf("SingleTunnelTestMode")] public TunnelJoin TunnelJoinPrefab;
    private List<TunnelGenerator> _tunnels;
    private List<TunnelJoin> _joins;

    public void Generate(Action onComplete)
    {
        if (SingleTunnelTestMode)
        {
            TunnelGenerator singleTunnel = FindObjectOfType<TunnelGenerator>();
            singleTunnel.Generate();
            onComplete?.Invoke();
            return;
        }
        
        _tunnels ??= new List<TunnelGenerator>();
        _joins ??= new List<TunnelJoin>();

        float startTime = Time.time;
        Debug.Log($"<color=green><b>==== BEGINNING WORLD GENERATION FOR MAP: {MapSettings.name.ToUpperInvariant()} ====</b></color>");
        
        //Clear existing children
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        _tunnels.Clear();
        _joins.Clear();

        if (Application.isPlaying)
        {
            StartCoroutine(BeginGeneration());
        }
        else
        {
#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutine(BeginGeneration(), this);
#endif
        }
        
        IEnumerator BeginGeneration()
        {
            if(MapSettings.Paths.Count > 0)
            {
                yield return GeneratePath(MapSettings.Paths[0]);
                yield return null;
            }
            float genTime = Time.time - startTime;
            Debug.Log($"<color=green><b>==== WORLD GENERATION COMPLETE IN {genTime:0.00}s====</b></color>");
            onComplete?.Invoke();
        }
    }

    [Button("Generate")][UsedImplicitly]
    private void Generate()
    {
        Generate(() =>
        {
            //Nothing its a test
        });
    }

    IEnumerator GeneratePath(WorldMapSettings.LevelPath path, TunnelGenerator inTunnel = null, TunnelJoin inJoin = null, WorldMapSettings.LevelPath inPath = null, int branchIndex = 1, int totalBranches = 1)
    {
        TunnelGenerator lastTunnel = inTunnel;
        TunnelJoin lastJoin = inJoin;
        
        Transform pathRoot = new GameObject(path.Name+ $" {branchIndex}/{totalBranches}").transform;
        pathRoot.SetParent(transform);

        if (lastJoin)
        {
            pathRoot.transform.position = lastJoin.transform.position;
        }

        for (int i = 0; i < path.Tunnels.Length; i++)
        {
            TunnelSettings settings = path.Tunnels[i];
            TunnelGenerator tunnelInstance = Instantiate(TunnelPrefab, pathRoot.transform);
            tunnelInstance.transform.position = Vector3.zero;
            tunnelInstance.transform.rotation = Quaternion.identity;

            string tunnelName = settings.name;
            if (totalBranches > 1)
            {
                if (branchIndex == 1)
                {
                    tunnelName = $"B1: {settings.name}";
                }
                else if(branchIndex == 2)
                {
                    tunnelName = $"B2: {settings.name}";
                }
                else if(branchIndex == 3)
                {
                    tunnelName = $"B3: {settings.name}";
                }
            }
            
            tunnelInstance.gameObject.name = tunnelName;
            tunnelInstance.GenerationSettings = settings;

            if (lastJoin)
            {
                int lastJoinOutPaths = (int)Mathf.Clamp01(i); //Either 0 (start of game), or 1 (linear tunnel segment in path)
                if (inPath != null)
                {
                    lastJoinOutPaths = inPath.ConnectingPathCount;
                }
                
                //Debug.Log($"{tunnelInstance.gameObject.name} Setting out path count for join:" + lastJoinOutPaths, tunnelInstance);
                lastJoin.AddOutTunnel(lastJoinOutPaths,
                    tunnelInstance,
                    out Vector3 tunnelPosition,
                    out Quaternion tunnelRotation);
                tunnelInstance.transform.position = tunnelPosition;
                tunnelInstance.transform.rotation = tunnelRotation;
            }

            tunnelInstance.GetEndData(out Vector3 endPoint, out Quaternion endRotation);

            TunnelJoin joinInstance = Instantiate(TunnelJoinPrefab, pathRoot.transform);
            joinInstance.gameObject.name = $"Join - In: {settings.name}";
            joinInstance.transform.position = endPoint;
            joinInstance.transform.rotation = endRotation;
            joinInstance.InTunnel = tunnelInstance;

            if (lastJoin != null)
            {
                if (lastJoin.gameObject.name.Contains("Out:"))
                {
                    lastJoin.gameObject.name += $", {settings.name}";
                }
                else
                {
                    lastJoin.gameObject.name += $" Out: {settings.name}";
                }
            }

            tunnelInstance.BackJoin = lastJoin;
            tunnelInstance.FrontJoin = joinInstance;

            _tunnels.Add(tunnelInstance);
            _joins.Add(joinInstance);

            lastTunnel = tunnelInstance;
            lastJoin = joinInstance;

            yield return null;
        }

        //Store new path start data, so that we dont loose it by doing
        List<(WorldMapSettings.LevelPath, TunnelGenerator, TunnelJoin, WorldMapSettings.LevelPath)> newPathStartData = new();
        int branchCount = 0;
        foreach (TunnelPath value in Enum.GetValues(typeof(TunnelPath)))
        {
            if (value == TunnelPath.None)
            {
                continue;
            }
            
            if (path.ConnectingPaths.HasFlag(value))
            {
                for (int i = 0; i < MapSettings.Paths.Count; i++)
                {
                    WorldMapSettings.LevelPath nextPath = MapSettings.Paths[i];
                    if (Enum.Parse<TunnelPath>(ScriptGenerationUtility.CreateVarName(nextPath.Name)) == value)
                    {
                        newPathStartData.Add((nextPath, lastTunnel, lastJoin, path));
                        branchCount++;
                        yield return null;
                    }
                }
            }
        }
        
        for (int i = 0; i < newPathStartData.Count; i++)
        {
            (WorldMapSettings.LevelPath pathToCreate, TunnelGenerator parentTunnel, TunnelJoin parentJoin,
                WorldMapSettings.LevelPath parentPath) = newPathStartData[i];

            yield return GeneratePath(pathToCreate, parentTunnel, parentJoin, 
                parentPath,branchIndex>i+1?branchIndex:i+1, 
                totalBranches>branchCount?totalBranches:branchCount);
        }

        foreach (TunnelGenerator tunnel in _tunnels)
        {
            tunnel.Generate();

            yield return null;
        }
    }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        _tunnels = new List<TunnelGenerator>(GetComponentsInChildren<TunnelGenerator>());
        _joins = new List<TunnelJoin>(GetComponentsInChildren<TunnelJoin>());
    }

    // Find Tunnel
    //----------------------------------------------------------------------------------------------------
    public bool TryGetTunnel(Vector3 pos, out TunnelGenerator tunnel, params TunnelGenerator[] toExclude)
    {
        // Find closest tunnel
        float closestDistance = float.MaxValue;
        TunnelGenerator closestGenerator = null;

        _tunnels ??= new List<TunnelGenerator>();
        foreach(TunnelGenerator generator in _tunnels)
        {
            Vector3 closest = generator.GetClosestPoint(pos);
            float distance = Vector3.Distance(closest, pos);

            //Debug.LogWarning($"{generator.name} {distance}");
            if (toExclude.Contains(generator))
            {
                continue;
            }
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

    public void CullForTunnel(TunnelGenerator active)
    {
        foreach (TunnelGenerator tunnel in _tunnels)
        {
            bool isActive = active == tunnel || active.BackJoin != null && tunnel == active.BackJoin.InTunnel;

            if (active.FrontJoin != null && active.FrontJoin.OutTunnels != null)
            {
                if (active.FrontJoin.OutTunnels.Any(outTunnel => tunnel == outTunnel))
                {
                    isActive = true;
                }
            }

            if (tunnel.BackJoin != null)
            {
                tunnel.BackJoin.gameObject.SetActive(isActive);
            }
            
            if (tunnel.FrontJoin != null)
            {
                tunnel.FrontJoin.gameObject.SetActive(isActive);
            }
            
            tunnel.gameObject.SetActive(isActive);
        }
    }

    public Vector3 GetDirectionAt(Vector3 worldPos, float lookaheadDst)
    {
        TryGetTunnel(worldPos, out TunnelGenerator tunnel);
        
        float t = tunnel.GetClosestPositionAndDirection(worldPos, out Vector3 currentPosition, out Vector3 currentDirection, out Vector3 currentUp);
        Vector3 lookaheadPoint = tunnel.GetLookaheadPoint(t, lookaheadDst);
        Vector3 towardsLookahead = (lookaheadPoint - currentPosition).normalized;
        transform.forward = Vector3.RotateTowards(transform.forward, towardsLookahead, UnityEngine.Time.deltaTime * 0.25f, 0);

        return currentDirection;
    } 
    
// =========================
#if UNITY_EDITOR

    //Very important.
    [HideIf("_areYouTakingCareOfYourself")] [SerializeField] private bool _areYouTakingCareOfYourself; 
    [ShowIf("ShowGreatJob")] [SerializeField] private bool _greatJobYouGotThis;
    [UsedImplicitly]
    bool ShowGreatJob()
    {
        return _areYouTakingCareOfYourself && !_greatJobYouGotThis;
    }
#endif
}
