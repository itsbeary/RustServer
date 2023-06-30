using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class HitchTrough : StorageContainer
{
	// Token: 0x0600163D RID: 5693 RVA: 0x000AD95C File Offset: 0x000ABB5C
	public global::Item GetFoodItem()
	{
		foreach (global::Item item in base.inventory.itemList)
		{
			if (item.info.category == ItemCategory.Food && item.info.GetComponent<ItemModConsumable>())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x000AD9D4 File Offset: 0x000ABBD4
	public bool ValidHitchPosition(Vector3 pos)
	{
		return this.GetClosest(pos, false, 1f) != null;
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x000AD9E8 File Offset: 0x000ABBE8
	public bool HasSpace()
	{
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsOccupied(true))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x000ADA18 File Offset: 0x000ABC18
	public HitchTrough.HitchSpot GetClosest(Vector3 testPos, bool includeOccupied = false, float maxRadius = -1f)
	{
		float num = 10000f;
		HitchTrough.HitchSpot hitchSpot = null;
		for (int i = 0; i < this.hitchSpots.Length; i++)
		{
			float num2 = Vector3.Distance(testPos, this.hitchSpots[i].spot.position);
			if (num2 < num && (maxRadius == -1f || num2 <= maxRadius) && (includeOccupied || !this.hitchSpots[i].IsOccupied(true)))
			{
				num = num2;
				hitchSpot = this.hitchSpots[i];
			}
		}
		return hitchSpot;
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x000ADA88 File Offset: 0x000ABC88
	public void Unhitch(RidableHorse horse)
	{
		foreach (HitchTrough.HitchSpot hitchSpot in this.hitchSpots)
		{
			if (hitchSpot.GetHorse(base.isServer) == horse)
			{
				hitchSpot.SetOccupiedBy(null);
				horse.SetHitch(null);
			}
		}
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x000ADAD0 File Offset: 0x000ABCD0
	public int NumHitched()
	{
		int num = 0;
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsOccupied(true))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001643 RID: 5699 RVA: 0x000ADB04 File Offset: 0x000ABD04
	public bool AttemptToHitch(RidableHorse horse, HitchTrough.HitchSpot hitch = null)
	{
		if (horse == null)
		{
			return false;
		}
		if (hitch == null)
		{
			hitch = this.GetClosest(horse.transform.position, false, -1f);
		}
		if (hitch != null)
		{
			hitch.SetOccupiedBy(horse);
			horse.SetHitch(this);
			horse.transform.SetPositionAndRotation(hitch.spot.position, hitch.spot.rotation);
			horse.DismountAllPlayers();
			return true;
		}
		return false;
	}

	// Token: 0x06001644 RID: 5700 RVA: 0x000ADB74 File Offset: 0x000ABD74
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericEntRef1 = this.hitchSpots[0].horse.uid;
		info.msg.ioEntity.genericEntRef2 = this.hitchSpots[1].horse.uid;
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000ADBDC File Offset: 0x000ABDDC
	public override void PostServerLoad()
	{
		foreach (HitchTrough.HitchSpot hitchSpot in this.hitchSpots)
		{
			this.AttemptToHitch(hitchSpot.GetHorse(true), hitchSpot);
		}
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x000ADC14 File Offset: 0x000ABE14
	public void UnhitchAll()
	{
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			RidableHorse horse = array[i].GetHorse(true);
			if (horse)
			{
				this.Unhitch(horse);
			}
		}
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x000ADC4F File Offset: 0x000ABE4F
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.UnhitchAll();
		}
		base.DestroyShared();
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x000ADC65 File Offset: 0x000ABE65
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x000ADC70 File Offset: 0x000ABE70
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.hitchSpots[0].horse.uid = info.msg.ioEntity.genericEntRef1;
			this.hitchSpots[1].horse.uid = info.msg.ioEntity.genericEntRef2;
		}
	}

	// Token: 0x04000E8B RID: 3723
	public HitchTrough.HitchSpot[] hitchSpots;

	// Token: 0x04000E8C RID: 3724
	public float caloriesToDecaySeconds = 36f;

	// Token: 0x02000C34 RID: 3124
	[Serializable]
	public class HitchSpot
	{
		// Token: 0x06004E53 RID: 20051 RVA: 0x001A27CD File Offset: 0x001A09CD
		public RidableHorse GetHorse(bool isServer = true)
		{
			return this.horse.Get(isServer) as RidableHorse;
		}

		// Token: 0x06004E54 RID: 20052 RVA: 0x001A27E0 File Offset: 0x001A09E0
		public bool IsOccupied(bool isServer = true)
		{
			return this.horse.IsValid(isServer);
		}

		// Token: 0x06004E55 RID: 20053 RVA: 0x001A27EE File Offset: 0x001A09EE
		public void SetOccupiedBy(RidableHorse newHorse)
		{
			this.horse.Set(newHorse);
		}

		// Token: 0x040042E6 RID: 17126
		public HitchTrough owner;

		// Token: 0x040042E7 RID: 17127
		public Transform spot;

		// Token: 0x040042E8 RID: 17128
		public EntityRef horse;
	}
}
