using System;
using Rust;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class WaterPurifier : LiquidContainer
{
	// Token: 0x06002250 RID: 8784 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool IsBoiling()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000DE2CC File Offset: 0x000DC4CC
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnStorageEnt(false);
		}
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000DE2E2 File Offset: 0x000DC4E2
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.SpawnStorageEnt(true);
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000DE2F4 File Offset: 0x000DC4F4
	protected virtual void SpawnStorageEnt(bool load)
	{
		if (load)
		{
			BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity)
			{
				foreach (BaseEntity baseEntity in parentEntity.children)
				{
					LiquidContainer liquidContainer;
					if (baseEntity != this && (liquidContainer = baseEntity as LiquidContainer) != null)
					{
						this.waterStorage = liquidContainer;
						break;
					}
				}
			}
		}
		if (this.waterStorage != null)
		{
			this.waterStorage.SetConnectedTo(this);
			return;
		}
		this.waterStorage = GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.localPosition, this.storagePrefabAnchor.localRotation, true) as LiquidContainer;
		this.waterStorage.SetParent(base.GetParentEntity(), false, false);
		this.waterStorage.Spawn();
		this.waterStorage.SetConnectedTo(this);
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x00029C50 File Offset: 0x00027E50
	internal override void OnParentRemoved()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000DE3EC File Offset: 0x000DC5EC
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (!this.waterStorage.IsDestroyed)
		{
			this.waterStorage.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000063A5 File Offset: 0x000045A5
	public void ParentTemperatureUpdate(float temp)
	{
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000DE410 File Offset: 0x000DC610
	public void CheckCoolDown()
	{
		if (!base.GetParentEntity() || !base.GetParentEntity().IsOn() || !this.HasDirtyWater())
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.CancelInvoke(new Action(this.CheckCoolDown));
		}
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000DE460 File Offset: 0x000DC660
	public bool HasDirtyWater()
	{
		Item slot = base.inventory.GetSlot(0);
		return slot != null && slot.info.itemType == ItemContainer.ContentsType.Liquid && slot.amount > 0;
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000DE498 File Offset: 0x000DC698
	public void Cook(float timeCooked)
	{
		if (this.waterStorage == null)
		{
			return;
		}
		bool flag = this.HasDirtyWater();
		if (!this.IsBoiling() && flag)
		{
			base.InvokeRepeating(new Action(this.CheckCoolDown), 2f, 2f);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		if (!this.IsBoiling())
		{
			return;
		}
		if (flag)
		{
			this.ConvertWater(timeCooked);
		}
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000DE508 File Offset: 0x000DC708
	protected void ConvertWater(float timeCooked)
	{
		if (this.stopWhenOutputFull)
		{
			Item slot = this.waterStorage.inventory.GetSlot(0);
			if (slot != null && slot.amount >= slot.MaxStackable())
			{
				return;
			}
		}
		float num = timeCooked * ((float)this.waterToProcessPerMinute / 60f);
		this.dirtyWaterProcssed += num;
		if (this.dirtyWaterProcssed >= 1f)
		{
			Item slot2 = base.inventory.GetSlot(0);
			int num2 = Mathf.Min(Mathf.FloorToInt(this.dirtyWaterProcssed), slot2.amount);
			num = (float)num2;
			slot2.UseItem(num2);
			this.dirtyWaterProcssed -= (float)num2;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		this.pendingFreshWater += num / (float)this.freshWaterRatio;
		if (this.pendingFreshWater >= 1f)
		{
			int num3 = Mathf.FloorToInt(this.pendingFreshWater);
			this.pendingFreshWater -= (float)num3;
			Item slot3 = this.waterStorage.inventory.GetSlot(0);
			if (slot3 != null && slot3.info != this.freshWater)
			{
				slot3.RemoveFromContainer();
				slot3.Remove(0f);
			}
			if (slot3 == null)
			{
				Item item = ItemManager.Create(this.freshWater, num3, 0UL);
				if (!item.MoveToContainer(this.waterStorage.inventory, -1, true, false, null, true))
				{
					item.Remove(0f);
				}
			}
			else
			{
				slot3.amount += num3;
				slot3.amount = Mathf.Clamp(slot3.amount, 0, this.waterStorage.maxStackSize);
				this.waterStorage.inventory.MarkDirty();
			}
			this.waterStorage.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000DE6B8 File Offset: 0x000DC8B8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x04001A7B RID: 6779
	public GameObjectRef storagePrefab;

	// Token: 0x04001A7C RID: 6780
	public Transform storagePrefabAnchor;

	// Token: 0x04001A7D RID: 6781
	public ItemDefinition freshWater;

	// Token: 0x04001A7E RID: 6782
	public int waterToProcessPerMinute = 120;

	// Token: 0x04001A7F RID: 6783
	public int freshWaterRatio = 4;

	// Token: 0x04001A80 RID: 6784
	public bool stopWhenOutputFull;

	// Token: 0x04001A81 RID: 6785
	protected LiquidContainer waterStorage;

	// Token: 0x04001A82 RID: 6786
	private float dirtyWaterProcssed;

	// Token: 0x04001A83 RID: 6787
	private float pendingFreshWater;

	// Token: 0x02000CDE RID: 3294
	public static class WaterPurifierFlags
	{
		// Token: 0x040045AB RID: 17835
		public const BaseEntity.Flags Boiling = BaseEntity.Flags.Reserved1;
	}
}
