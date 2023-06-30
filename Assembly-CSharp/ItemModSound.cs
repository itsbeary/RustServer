using System;
using Rust;
using UnityEngine;

// Token: 0x02000604 RID: 1540
public class ItemModSound : ItemMod
{
	// Token: 0x06002DDA RID: 11738 RVA: 0x0011407C File Offset: 0x0011227C
	public override void OnParentChanged(Item item)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.actionType == ItemModSound.Type.OnAttachToWeapon)
		{
			if (item.parentItem == null)
			{
				return;
			}
			if (item.parentItem.info.category != ItemCategory.Weapon)
			{
				return;
			}
			BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
			if (ownerPlayer == null)
			{
				return;
			}
			if (ownerPlayer.IsNpc)
			{
				return;
			}
			Effect.server.Run(this.effect.resourcePath, ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x04002595 RID: 9621
	public GameObjectRef effect = new GameObjectRef();

	// Token: 0x04002596 RID: 9622
	public ItemModSound.Type actionType;

	// Token: 0x02000D9F RID: 3487
	public enum Type
	{
		// Token: 0x040048AF RID: 18607
		OnAttachToWeapon
	}
}
