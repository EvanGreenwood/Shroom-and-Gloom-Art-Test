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