using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200045C RID: 1116
public class PlayerCorpse : global::LootableCorpse
{
	// Token: 0x06002521 RID: 9505 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsBuoyant()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved6);
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000EB1F4 File Offset: 0x000E93F4
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return (!baseEntity.InSafeZone() || baseEntity.userID == this.playerSteamID) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000EB218 File Offset: 0x000E9418
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.buoyancy == null)
		{
			Debug.LogWarning("Player corpse has no buoyancy assigned, searching at runtime :" + base.name);
			this.buoyancy = base.GetComponent<Buoyancy>();
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.SubmergedChanged = new Action<bool>(this.BuoyancyChanged);
			this.buoyancy.forEntity = this;
		}
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x000EB28B File Offset: 0x000E948B
	public void BuoyancyChanged(bool isSubmerged)
	{
		if (this.IsBuoyant())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, isSubmerged, false, false);
		base.SendNetworkUpdate_Flags();
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000EB2AC File Offset: 0x000E94AC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.lootableCorpse != null)
		{
			info.msg.lootableCorpse.underwearSkin = this.underwearSkin;
		}
		if (base.isServer && this.containers != null && this.containers.Length > 1 && !info.forDisk)
		{
			info.msg.storageBox = Pool.Get<StorageBox>();
			info.msg.storageBox.contents = this.containers[1].Save();
		}
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000EB333 File Offset: 0x000E9533
	public override string Categorize()
	{
		return "playercorpse";
	}

	// Token: 0x04001D63 RID: 7523
	public Buoyancy buoyancy;

	// Token: 0x04001D64 RID: 7524
	public const global::BaseEntity.Flags Flag_Buoyant = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04001D65 RID: 7525
	public uint underwearSkin;
}
