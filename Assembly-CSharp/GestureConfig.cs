using System;
using UnityEngine;

// Token: 0x020005C1 RID: 1473
[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Config")]
public class GestureConfig : ScriptableObject
{
	// Token: 0x06002C71 RID: 11377 RVA: 0x0010D678 File Offset: 0x0010B878
	public bool IsOwnedBy(BasePlayer player)
	{
		if (this.forceUnlock)
		{
			return true;
		}
		if (this.gestureType == GestureConfig.GestureType.NPC)
		{
			return player.IsNpc;
		}
		if (this.gestureType == GestureConfig.GestureType.Cinematic)
		{
			return player.IsAdmin;
		}
		return (this.dlcItem != null && this.dlcItem.CanUse(player)) || (this.inventoryItem != null && player.blueprints.steamInventory.HasItem(this.inventoryItem.id));
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x0010D6FC File Offset: 0x0010B8FC
	public bool CanBeUsedBy(BasePlayer player)
	{
		if (player.isMounted)
		{
			if (this.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody)
			{
				return false;
			}
			if (player.GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
			{
				return false;
			}
		}
		return (!player.IsSwimming() || this.playerModelLayer != GestureConfig.PlayerModelLayer.FullBody) && (this.playerModelLayer != GestureConfig.PlayerModelLayer.FullBody || !player.modelState.ducked);
	}

	// Token: 0x0400242C RID: 9260
	[ReadOnly]
	public uint gestureId;

	// Token: 0x0400242D RID: 9261
	public string gestureCommand;

	// Token: 0x0400242E RID: 9262
	public string convarName;

	// Token: 0x0400242F RID: 9263
	public Translate.Phrase gestureName;

	// Token: 0x04002430 RID: 9264
	public Sprite icon;

	// Token: 0x04002431 RID: 9265
	public int order = 1;

	// Token: 0x04002432 RID: 9266
	public float duration = 1.5f;

	// Token: 0x04002433 RID: 9267
	public bool canCancel = true;

	// Token: 0x04002434 RID: 9268
	[Header("Player model setup")]
	public GestureConfig.PlayerModelLayer playerModelLayer = GestureConfig.PlayerModelLayer.UpperBody;

	// Token: 0x04002435 RID: 9269
	public GestureConfig.GestureType gestureType;

	// Token: 0x04002436 RID: 9270
	public bool hideHeldEntity = true;

	// Token: 0x04002437 RID: 9271
	public bool canDuckDuringGesture;

	// Token: 0x04002438 RID: 9272
	public GestureConfig.MovementCapabilities movementMode;

	// Token: 0x04002439 RID: 9273
	public GestureConfig.AnimationType animationType;

	// Token: 0x0400243A RID: 9274
	public BasePlayer.CameraMode viewMode;

	// Token: 0x0400243B RID: 9275
	public bool useRootMotion;

	// Token: 0x0400243C RID: 9276
	[Header("Ownership")]
	public GestureConfig.GestureActionType actionType;

	// Token: 0x0400243D RID: 9277
	public bool forceUnlock;

	// Token: 0x0400243E RID: 9278
	public SteamDLCItem dlcItem;

	// Token: 0x0400243F RID: 9279
	public SteamInventoryItem inventoryItem;

	// Token: 0x02000D85 RID: 3461
	public enum GestureType
	{
		// Token: 0x0400484F RID: 18511
		Player,
		// Token: 0x04004850 RID: 18512
		NPC,
		// Token: 0x04004851 RID: 18513
		Cinematic
	}

	// Token: 0x02000D86 RID: 3462
	public enum PlayerModelLayer
	{
		// Token: 0x04004853 RID: 18515
		UpperBody = 3,
		// Token: 0x04004854 RID: 18516
		FullBody
	}

	// Token: 0x02000D87 RID: 3463
	public enum MovementCapabilities
	{
		// Token: 0x04004856 RID: 18518
		FullMovement,
		// Token: 0x04004857 RID: 18519
		NoMovement
	}

	// Token: 0x02000D88 RID: 3464
	public enum AnimationType
	{
		// Token: 0x04004859 RID: 18521
		OneShot,
		// Token: 0x0400485A RID: 18522
		Loop
	}

	// Token: 0x02000D89 RID: 3465
	public enum ViewMode
	{
		// Token: 0x0400485C RID: 18524
		FirstPerson,
		// Token: 0x0400485D RID: 18525
		ThirdPerson
	}

	// Token: 0x02000D8A RID: 3466
	public enum GestureActionType
	{
		// Token: 0x0400485F RID: 18527
		None,
		// Token: 0x04004860 RID: 18528
		ShowNameTag,
		// Token: 0x04004861 RID: 18529
		DanceAchievement
	}
}
