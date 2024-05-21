using UnityEngine;

public class TunnelEncounter : TunnelRigComponent
{
    public enum EncounterType
    {
        Combat,
        Explore,
        Blocker,
        DoorChoice
    }

    public EncounterType EncounterMode;

    public override bool UsesAngleDistance()
    {
        return false;
    }
}
