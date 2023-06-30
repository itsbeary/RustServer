using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003DD RID: 989
public class PoweredWaterPurifier : WaterPurifier
{
	// Token: 0x0600221F RID: 8735 RVA: 0x00025634 File Offset: 0x00023834
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000DD90C File Offset: 0x000DBB0C
	public override bool CanPickup(BasePlayer player)
	{
		if (base.isClient)
		{
			return base.CanPickup(player);
		}
		return base.CanPickup(player) && !base.HasDirtyWater() && this.waterStorage != null && (this.waterStorage.inventory == null || this.waterStorage.inventory.itemList.Count == 0);
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000DD974 File Offset: 0x000DBB74
	protected override void SpawnStorageEnt(bool load)
	{
		if (load)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LiquidContainer liquidContainer;
					if ((liquidContainer = enumerator.Current as LiquidContainer) != null)
					{
						this.waterStorage = liquidContainer;
					}
				}
			}
		}
		if (this.waterStorage != null)
		{
			this.waterStorage.SetConnectedTo(this);
			return;
		}
		this.waterStorage = GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.position, this.storagePrefabAnchor.rotation, true) as LiquidContainer;
		this.waterStorage.SetParent(this, true, false);
		this.waterStorage.Spawn();
		this.waterStorage.SetConnectedTo(this);
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x000DDA4C File Offset: 0x000DBC4C
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (base.HasLiquidItem())
		{
			if (base.HasFlag(BaseEntity.Flags.Reserved8) && !base.IsInvoking(new Action(this.ConvertWater)))
			{
				base.InvokeRandomized(new Action(this.ConvertWater), this.ConvertInterval, this.ConvertInterval, this.ConvertInterval * 0.1f);
				return;
			}
		}
		else if (base.IsInvoking(new Action(this.ConvertWater)))
		{
			base.CancelInvoke(new Action(this.ConvertWater));
		}
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000DDADB File Offset: 0x000DBCDB
	private void ConvertWater()
	{
		if (!base.HasDirtyWater())
		{
			return;
		}
		base.ConvertWater(this.ConvertInterval);
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000DDAF2 File Offset: 0x000DBCF2
	public override int ConsumptionAmount()
	{
		return this.PowerDrain;
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000DDAFC File Offset: 0x000DBCFC
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			if (old.HasFlag(BaseEntity.Flags.Reserved8) != next.HasFlag(BaseEntity.Flags.Reserved8))
			{
				if (next.HasFlag(BaseEntity.Flags.Reserved8))
				{
					if (!base.IsInvoking(new Action(this.ConvertWater)))
					{
						base.InvokeRandomized(new Action(this.ConvertWater), this.ConvertInterval, this.ConvertInterval, this.ConvertInterval * 0.1f);
					}
				}
				else if (base.IsInvoking(new Action(this.ConvertWater)))
				{
					base.CancelInvoke(new Action(this.ConvertWater));
				}
			}
			if (this.waterStorage != null)
			{
				this.waterStorage.SetFlag(BaseEntity.Flags.Reserved8, base.HasFlag(BaseEntity.Flags.Reserved8), false, true);
			}
		}
	}

	// Token: 0x04001A66 RID: 6758
	public float ConvertInterval = 5f;

	// Token: 0x04001A67 RID: 6759
	public int PowerDrain = 5;

	// Token: 0x04001A68 RID: 6760
	public Material PoweredMaterial;

	// Token: 0x04001A69 RID: 6761
	public Material UnpoweredMaterial;

	// Token: 0x04001A6A RID: 6762
	public MeshRenderer TargetRenderer;
}
