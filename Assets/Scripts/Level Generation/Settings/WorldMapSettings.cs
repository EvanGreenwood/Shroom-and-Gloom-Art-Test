using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="S&G/Data/WorldMapSettings")]
public class WorldMapSettings : ScriptableObject
{
    public class TunnelMetaData
    {
        private string Name;
     
        public List<string> Connections;
        public TunnelSettings Tunnel;
    }

    public List<TunnelMetaData> Tunnels;
}
