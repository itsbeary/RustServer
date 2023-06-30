using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003D9 RID: 985
public class ElectricOven : global::BaseOven
{
	// Token: 0x0600220B RID: 8715 RVA: 0x000DD5F3 File Offset: 0x000DB7F3
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnIOEnt();
		}
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000DD608 File Offset: 0x000DB808
	private void SpawnIOEnt()
	{
		if (this.IoEntity.isValid && this.IoEntityAnchor != null)
		{
			global::IOEntity ioentity = GameManager.server.CreateEntity(this.IoEntity.resourcePath, this.IoEntityAnchor.position, this.IoEntityAnchor.rotation, true) as global::IOEntity;
			ioentity.SetParent(this, true, false);
			ioentity.Spawn();
			this.spawnedIo.Set(ioentity);
		}
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x0600220D RID: 8717 RVA: 0x000DD67D File Offset: 0x000DB87D
	protected override bool CanRunWithNoFuel
	{
		get
		{
			return this.spawnedIo.IsValid(true) && this.spawnedIo.Get(true).IsPowered();
		}
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000DD6A0 File Offset: 0x000DB8A0
	public void OnIOEntityFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (!next.HasFlag(global::BaseEntity.Flags.Reserved8) && base.IsOn())
		{
			this.StopCooking();
			this.resumeCookingWhenPowerResumes = true;
			return;
		}
		if (next.HasFlag(global::BaseEntity.Flags.Reserved8) && !base.IsOn() && this.resumeCookingWhenPowerResumes)
		{
			this.StartCooking();
			this.resumeCookingWhenPowerResumes = false;
		}
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x000DD710 File Offset: 0x000DB910
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Pool.Get<SimpleUID>();
		}
		info.msg.simpleUID.uid = this.spawnedIo.uid;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000DD75C File Offset: 0x000DB95C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.spawnedIo.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000DD78D File Offset: 0x000DB98D
	protected override bool CanPickupOven()
	{
		return this.children.Count == 1;
	}

	// Token: 0x04001A61 RID: 6753
	public GameObjectRef IoEntity;

	// Token: 0x04001A62 RID: 6754
	public Transform IoEntityAnchor;

	// Token: 0x04001A63 RID: 6755
	private EntityRef<global::IOEntity> spawnedIo;

	// Token: 0x04001A64 RID: 6756
	private bool resumeCookingWhenPowerResumes;
}
