using System;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000757 RID: 1879
[CreateAssetMenu(menuName = "Rust/ItemSkin")]
public class ItemSkin : SteamInventoryItem
{
	// Token: 0x06003468 RID: 13416 RVA: 0x00143BDF File Offset: 0x00141DDF
	public void ApplySkin(GameObject obj)
	{
		if (this.Skinnable == null)
		{
			return;
		}
		Skin.Apply(obj, this.Skinnable, this.Materials);
	}

	// Token: 0x06003469 RID: 13417 RVA: 0x00143C04 File Offset: 0x00141E04
	public override bool HasUnlocked(ulong playerId)
	{
		if (this.Redirect != null && this.Redirect.isRedirectOf != null && this.Redirect.isRedirectOf.steamItem != null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(playerId);
			if (basePlayer != null && basePlayer.blueprints.CheckSkinOwnership(this.Redirect.isRedirectOf.steamItem.id, basePlayer.userID))
			{
				return true;
			}
		}
		if (this.UnlockedViaSteamItem != null)
		{
			BasePlayer basePlayer2 = BasePlayer.FindByID(playerId);
			if (basePlayer2 != null && basePlayer2.blueprints.CheckSkinOwnership(this.UnlockedViaSteamItem.id, basePlayer2.userID))
			{
				return true;
			}
		}
		return base.HasUnlocked(playerId);
	}

	// Token: 0x04002AD8 RID: 10968
	public Skinnable Skinnable;

	// Token: 0x04002AD9 RID: 10969
	public Material[] Materials;

	// Token: 0x04002ADA RID: 10970
	[Tooltip("If set, whenever we make an item with this skin, we'll spawn this item without a skin instead")]
	public ItemDefinition Redirect;

	// Token: 0x04002ADB RID: 10971
	public SteamInventoryItem UnlockedViaSteamItem;
}
