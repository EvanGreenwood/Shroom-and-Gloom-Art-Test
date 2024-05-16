/* ------------------------ */
/* ---- AUTO GENERATED ---- */
/* ---- AVOID TOUCHING ---- */
/* ------------------------ */

using UnityEngine;
using System.Collections.Generic;
using Framework;

public enum LayerName
{
	Default = 0,
	TransparentFX = 1,
	IgnoreRaycast = -1,
	Water = 4,
	UI = 5,
	Tunnel = 6,
	Player = 10,
	PlayerHead = 11,
	VolumeUI = 30,
	VolumeMain = 31
}

public enum SortingLayerName
{
	Default = 0,
	UI = -1573755697
}

public static class Layer
{

	public const int Default = 0;
	public const int TransparentFX = 1;
	public const int IgnoreRaycast = -1;
	public const int Water = 4;
	public const int UI = 5;
	public const int Tunnel = 6;
	public const int Player = 10;
	public const int PlayerHead = 11;
	public const int VolumeUI = 30;
	public const int VolumeMain = 31;

}

public static class SortingLayer
{

	public const int Default = 0;
	public const int UI = -1573755697;

}

public static class Tag
{

	public const string Untagged = "Untagged";
	public const string Respawn = "Respawn";
	public const string Finish = "Finish";
	public const string EditorOnly = "EditorOnly";
	public const string MainCamera = "MainCamera";
	public const string Player = "Player";
	public const string GameController = "GameController";

}

public static partial class LayerMasks
{

	public static readonly LayerMask ALL_LAYERS = ~0;
	public static readonly LayerMask NO_LAYERS = 0;
	public static readonly LayerMask Default = 1;
	public static readonly LayerMask TransparentFX = 2;
	public static readonly LayerMask IgnoreRaycast = -2147483648;
	public static readonly LayerMask Water = 16;
	public static readonly LayerMask UI = 32;
	public static readonly LayerMask Tunnel = 64;
	public static readonly LayerMask Player = 1024;
	public static readonly LayerMask PlayerHead = 2048;
	public static readonly LayerMask VolumeUI = 1073741824;
	public static readonly LayerMask VolumeMain = -2147483648;

}

public static class CollisionMatrix
{

	public static readonly LayerMask ALL_LAYERS = ~0;
	public static readonly LayerMask NO_LAYERS = 0;
	public static readonly LayerMask DefaultCollisionMask = -1;
	public static readonly LayerMask TransparentFXCollisionMask = -1;
	public static readonly LayerMask IgnoreRaycastCollisionMask = -1;
	public static readonly LayerMask WaterCollisionMask = -1;
	public static readonly LayerMask UICollisionMask = -1;
	public static readonly LayerMask TunnelCollisionMask = -1;
	public static readonly LayerMask PlayerCollisionMask = -1;
	public static readonly LayerMask PlayerHeadCollisionMask = -1;
	public static readonly LayerMask VolumeUICollisionMask = -1;
	public static readonly LayerMask VolumeMainCollisionMask = -1;

}

public static class SceneNames
{

	public const string S0MainMenu = "S0_MainMenu";
	public const string S1TheMines = "S1_TheMines";
	public const string S3RockCave = "S3_RockCave";
	public const string S4Cavern = "S4_Cavern";
	public const string S5Shroomy = "S5_Shroomy";
	public const string S7Portal = "S7_Portal";
	public const string S8Gloom = "S8_Gloom";

}

[CreateAssetMenu(fileName = "Tunnel Settings", menuName = "Scriptable Enum/Tunnel Settings")]
public partial class TunnelSettings
{

	public static TunnelSettings[] AllTunnelSettings { get { if (__allTunnelSettings == null) __allTunnelSettings = GetValues<TunnelSettings>(); return __allTunnelSettings; } }
	public static TunnelSettings D1Ruins { get { if (__d1Ruins == null) __d1Ruins = GetValue<TunnelSettings>("D1 Ruins"); return __d1Ruins; } }
	public static TunnelSettings D2Mines { get { if (__d2Mines == null) __d2Mines = GetValue<TunnelSettings>("D2 Mines"); return __d2Mines; } }
	public static TunnelSettings D3Rock { get { if (__d3Rock == null) __d3Rock = GetValue<TunnelSettings>("D3 Rock"); return __d3Rock; } }
	public static TunnelSettings D4Caves { get { if (__d4Caves == null) __d4Caves = GetValue<TunnelSettings>("D4 Caves"); return __d4Caves; } }
	public static TunnelSettings D5AShroomy { get { if (__d5AShroomy == null) __d5AShroomy = GetValue<TunnelSettings>("D5A Shroomy"); return __d5AShroomy; } }
	public static TunnelSettings D5BShroomy { get { if (__d5BShroomy == null) __d5BShroomy = GetValue<TunnelSettings>("D5B Shroomy"); return __d5BShroomy; } }
	public static TunnelSettings D6Trippy { get { if (__d6Trippy == null) __d6Trippy = GetValue<TunnelSettings>("D6 Trippy"); return __d6Trippy; } }
	public static TunnelSettings D7Gloom { get { if (__d7Gloom == null) __d7Gloom = GetValue<TunnelSettings>("D7 Gloom"); return __d7Gloom; } }
	
	protected static TunnelSettings[] __allTunnelSettings;
	protected static TunnelSettings __d1Ruins;
	protected static TunnelSettings __d2Mines;
	protected static TunnelSettings __d3Rock;
	protected static TunnelSettings __d4Caves;
	protected static TunnelSettings __d5AShroomy;
	protected static TunnelSettings __d5BShroomy;
	protected static TunnelSettings __d6Trippy;
	protected static TunnelSettings __d7Gloom;

}

[CreateAssetMenu(fileName = "Spline Volume", menuName = "Scriptable Enum/Spline Volume")]
public partial class SplineVolume
{

	public static SplineVolume[] AllSplineVolumes { get { if (__allSplineVolumes == null) __allSplineVolumes = GetValues<SplineVolume>(); return __allSplineVolumes; } }
	
	protected static SplineVolume[] __allSplineVolumes;
	

}