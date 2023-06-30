using System;
using Facepunch;
using ProtoBuf;

// Token: 0x020001A0 RID: 416
public class VehicleVendor : NPCTalking
{
	// Token: 0x06001888 RID: 6280 RVA: 0x000B70F8 File Offset: 0x000B52F8
	public override string GetConversationStartSpeech(global::BasePlayer player)
	{
		if (base.ProviderBusy())
		{
			return "startbusy";
		}
		return "intro";
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x000B710D File Offset: 0x000B530D
	public VehicleSpawner GetVehicleSpawner()
	{
		if (!this.spawnerRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.spawnerRef.Get(base.isServer).GetComponent<VehicleSpawner>();
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x000B713C File Offset: 0x000B533C
	public override void UpdateFlags()
	{
		base.UpdateFlags();
		VehicleSpawner vehicleSpawner = this.GetVehicleSpawner();
		bool flag = vehicleSpawner != null && vehicleSpawner.IsPadOccupied();
		base.SetFlag(global::BaseEntity.Flags.Reserved1, flag, false, true);
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x000B7178 File Offset: 0x000B5378
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.spawnerRef.IsValid(true) && this.vehicleSpawner == null)
		{
			this.vehicleSpawner = this.GetVehicleSpawner();
			return;
		}
		if (this.vehicleSpawner != null && !this.spawnerRef.IsValid(true))
		{
			this.spawnerRef.Set(this.vehicleSpawner);
		}
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x000B71E1 File Offset: 0x000B53E1
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleVendor = Pool.Get<ProtoBuf.VehicleVendor>();
		info.msg.vehicleVendor.spawnerRef = this.spawnerRef.uid;
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x000B7215 File Offset: 0x000B5415
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vehicleVendor != null)
		{
			this.spawnerRef.id_cached = info.msg.vehicleVendor.spawnerRef;
		}
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00081318 File Offset: 0x0007F518
	public override ConversationData GetConversationFor(global::BasePlayer player)
	{
		return this.conversations[0];
	}

	// Token: 0x04001134 RID: 4404
	public EntityRef spawnerRef;

	// Token: 0x04001135 RID: 4405
	public VehicleSpawner vehicleSpawner;
}
