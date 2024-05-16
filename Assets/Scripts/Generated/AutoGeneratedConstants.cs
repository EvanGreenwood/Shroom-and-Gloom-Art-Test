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
	public static TunnelSettings D1BuriedCity { get { if (__d1BuriedCity == null) __d1BuriedCity = GetValue<TunnelSettings>("D1 BuriedCity"); return __d1BuriedCity; } }
	public static TunnelSettings D2ThornyTrees { get { if (__d2ThornyTrees == null) __d2ThornyTrees = GetValue<TunnelSettings>("D2 ThornyTrees"); return __d2ThornyTrees; } }
	public static TunnelSettings D3DirtTunnels { get { if (__d3DirtTunnels == null) __d3DirtTunnels = GetValue<TunnelSettings>("D3 DirtTunnels"); return __d3DirtTunnels; } }
	public static TunnelSettings D4Mines { get { if (__d4Mines == null) __d4Mines = GetValue<TunnelSettings>("D4 Mines"); return __d4Mines; } }
	public static TunnelSettings D5ShroomyCaves { get { if (__d5ShroomyCaves == null) __d5ShroomyCaves = GetValue<TunnelSettings>("D5 ShroomyCaves"); return __d5ShroomyCaves; } }
	public static TunnelSettings D6Shroomy { get { if (__d6Shroomy == null) __d6Shroomy = GetValue<TunnelSettings>("D6 Shroomy"); return __d6Shroomy; } }
	public static TunnelSettings D7Caves { get { if (__d7Caves == null) __d7Caves = GetValue<TunnelSettings>("D7 Caves"); return __d7Caves; } }
	
	protected static TunnelSettings[] __allTunnelSettings;
	protected static TunnelSettings __d1BuriedCity;
	protected static TunnelSettings __d2ThornyTrees;
	protected static TunnelSettings __d3DirtTunnels;
	protected static TunnelSettings __d4Mines;
	protected static TunnelSettings __d5ShroomyCaves;
	protected static TunnelSettings __d6Shroomy;
	protected static TunnelSettings __d7Caves;

}