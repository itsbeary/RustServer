using System;
using UnityEngine;

// Token: 0x020005F4 RID: 1524
public class ItemModEntity : ItemMod
{
	// Token: 0x06002DA8 RID: 11688 RVA: 0x001133B0 File Offset: 0x001115B0
	public override void OnChanged(Item item)
	{
		HeldEntity heldEntity = item.GetHeldEntity() as HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.OnItemChanged(item);
		}
		base.OnChanged(item);
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x001133E0 File Offset: 0x001115E0
	public override void OnItemCreated(Item item)
	{
		if (item.GetHeldEntity() == null)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity == null)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Couldn't create item entity ",
					item.info.displayName.english,
					" (",
					this.entityPrefab.resourcePath,
					")"
				}));
				return;
			}
			baseEntity.skinID = item.skin;
			baseEntity.Spawn();
			item.SetHeldEntity(baseEntity);
		}
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x00113494 File Offset: 0x00111694
	public override void OnRemove(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		heldEntity.Kill(BaseNetworkable.DestroyMode.None);
		item.SetHeldEntity(null);
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x001134C0 File Offset: 0x001116C0
	private bool ParentToParent(Item item, BaseEntity ourEntity)
	{
		if (item.parentItem == null)
		{
			return false;
		}
		BaseEntity baseEntity = item.parentItem.GetWorldEntity();
		if (baseEntity == null)
		{
			baseEntity = item.parentItem.GetHeldEntity();
		}
		ourEntity.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		ourEntity.limitNetworking = false;
		ourEntity.SetParent(baseEntity, this.defaultBone, false, false);
		return true;
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x0011351C File Offset: 0x0011171C
	private bool ParentToPlayer(Item item, BaseEntity ourEntity)
	{
		HeldEntity heldEntity = ourEntity as HeldEntity;
		if (heldEntity == null)
		{
			return false;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			heldEntity.SetOwnerPlayer(ownerPlayer);
			return true;
		}
		heldEntity.ClearOwnerPlayer();
		return true;
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x0011355C File Offset: 0x0011175C
	public override void OnParentChanged(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		if (this.ParentToParent(item, heldEntity))
		{
			return;
		}
		if (this.ParentToPlayer(item, heldEntity))
		{
			return;
		}
		heldEntity.SetParent(null, false, false);
		heldEntity.limitNetworking = true;
		heldEntity.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x001135AC File Offset: 0x001117AC
	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity2 = heldEntity as HeldEntity;
		if (heldEntity2 == null)
		{
			return;
		}
		heldEntity2.CollectedForCrafting(item, crafter);
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x001135E4 File Offset: 0x001117E4
	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity2 = heldEntity as HeldEntity;
		if (heldEntity2 == null)
		{
			return;
		}
		heldEntity2.ReturnedFromCancelledCraft(item, crafter);
	}

	// Token: 0x04002551 RID: 9553
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x04002552 RID: 9554
	public string defaultBone;
}
