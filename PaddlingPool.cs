using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class PaddlingPool : LiquidContainer, ISplashable
{
	// Token: 0x060017D0 RID: 6096 RVA: 0x000B3CBC File Offset: 0x000B1EBC
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		float normalisedFillLevel = this.GetNormalisedFillLevel();
		base.SetFlag(global::BaseEntity.Flags.Reserved4, normalisedFillLevel >= 1f, false, true);
		this.UpdatePoolFillAmount(normalisedFillLevel);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x000B3D00 File Offset: 0x000B1F00
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		float normalisedFillLevel = this.GetNormalisedFillLevel();
		this.UpdatePoolFillAmount(normalisedFillLevel);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x000B3D28 File Offset: 0x000B1F28
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (base.IsDestroyed)
		{
			return false;
		}
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved4) && splashType != null)
		{
			for (int i = 0; i < this.ValidItems.Length; i++)
			{
				if (this.ValidItems[i] != null && this.ValidItems[i].itemid == splashType.itemid)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x000B3D90 File Offset: 0x000B1F90
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		base.inventory.AddItem(splashType, amount, 0UL, global::ItemContainer.LimitStack.Existing);
		return amount;
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x000B3DA3 File Offset: 0x000B1FA3
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.WaterPool = Pool.Get<WaterPool>();
		info.msg.WaterPool.fillAmount = this.GetNormalisedFillLevel();
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x000B3DD4 File Offset: 0x000B1FD4
	private float GetNormalisedFillLevel()
	{
		if (base.inventory.itemList.Count <= 0 || base.inventory.itemList[0] == null)
		{
			return 0f;
		}
		return (float)base.inventory.itemList[0].amount / (float)this.maxStackSize;
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x000B3E2C File Offset: 0x000B202C
	private void UpdatePoolFillAmount(float normalisedAmount)
	{
		this.poolWaterVisual.gameObject.SetActive(normalisedAmount > 0f);
		this.waterVolume.waterEnabled = normalisedAmount > 0f;
		float num = Mathf.Lerp(this.minimumWaterHeight, this.maximumWaterHeight, normalisedAmount);
		Vector3 localPosition = this.poolWaterVolume.localPosition;
		localPosition.y = num;
		this.poolWaterVolume.localPosition = localPosition;
		if (this.alignWaterUp)
		{
			this.poolWaterVolume.up = Vector3.up;
		}
		if (normalisedAmount > 0f && this.lastFillAmount < normalisedAmount && this.waterVolume.entityContents != null)
		{
			using (HashSet<global::BaseEntity>.Enumerator enumerator = this.waterVolume.entityContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPoolVehicle poolVehicle;
					if ((poolVehicle = enumerator.Current as IPoolVehicle) != null)
					{
						poolVehicle.WakeUp();
					}
				}
			}
		}
		this.lastFillAmount = normalisedAmount;
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x000B3F28 File Offset: 0x000B2128
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			List<IPoolVehicle> list = Pool.GetList<IPoolVehicle>();
			if (this.waterVolume.entityContents != null)
			{
				using (HashSet<global::BaseEntity>.Enumerator enumerator = this.waterVolume.entityContents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IPoolVehicle poolVehicle;
						if ((poolVehicle = enumerator.Current as IPoolVehicle) != null)
						{
							list.Add(poolVehicle);
						}
					}
				}
			}
			foreach (IPoolVehicle poolVehicle2 in list)
			{
				poolVehicle2.OnPoolDestroyed();
			}
			Pool.FreeList<IPoolVehicle>(ref list);
		}
	}

	// Token: 0x04001092 RID: 4242
	public const global::BaseEntity.Flags FilledUp = global::BaseEntity.Flags.Reserved4;

	// Token: 0x04001093 RID: 4243
	public Transform poolWaterVolume;

	// Token: 0x04001094 RID: 4244
	public GameObject poolWaterVisual;

	// Token: 0x04001095 RID: 4245
	public float minimumWaterHeight;

	// Token: 0x04001096 RID: 4246
	public float maximumWaterHeight = 1f;

	// Token: 0x04001097 RID: 4247
	public WaterVolume waterVolume;

	// Token: 0x04001098 RID: 4248
	public bool alignWaterUp = true;

	// Token: 0x04001099 RID: 4249
	public GameObjectRef destroyedWithWaterEffect;

	// Token: 0x0400109A RID: 4250
	public Transform destroyedWithWaterEffectPos;

	// Token: 0x0400109B RID: 4251
	public Collider requireLookAt;

	// Token: 0x0400109C RID: 4252
	private float lastFillAmount = -1f;
}
