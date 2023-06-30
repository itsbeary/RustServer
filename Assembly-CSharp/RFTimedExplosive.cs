using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C3 RID: 195
public class RFTimedExplosive : global::TimedExplosive, IRFObject
{
	// Token: 0x0600116D RID: 4461 RVA: 0x0008E734 File Offset: 0x0008C934
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFTimedExplosive.OnRpcMessage", 0))
		{
			if (rpc == 2778075470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Pickup ");
				}
				using (TimeWarning.New("Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778075470U, "Pickup", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Pickup(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0008207B File Offset: 0x0008027B
	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x0008E89C File Offset: 0x0008CA9C
	public void RFSignalUpdate(bool on)
	{
		if (this.IsArmed() && on && !base.IsInvoking(new Action(this.Explode)))
		{
			base.Invoke(new Action(this.Explode), UnityEngine.Random.Range(0f, 0.2f));
		}
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0008E8EA File Offset: 0x0008CAEA
	public void SetFrequency(int newFreq)
	{
		RFManager.RemoveListener(this.RFFrequency, this);
		this.RFFrequency = newFreq;
		if (this.RFFrequency > 0)
		{
			RFManager.AddListener(this.RFFrequency, this);
		}
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0008E914 File Offset: 0x0008CB14
	public int GetFrequency()
	{
		return this.RFFrequency;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0008E91C File Offset: 0x0008CB1C
	public override void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			if (this.GetFrequency() > 0)
			{
				if (base.IsInvoking(new Action(this.Explode)))
				{
					base.CancelInvoke(new Action(this.Explode));
				}
				base.Invoke(new Action(this.ArmRF), fuseLength);
				base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, false);
				base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				return;
			}
			base.SetFuse(fuseLength);
		}
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0008E999 File Offset: 0x0008CB99
	public void ArmRF()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x00051BA4 File Offset: 0x0004FDA4
	public void DisarmRF()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x0008E9BC File Offset: 0x0008CBBC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.explosive == null)
		{
			info.msg.explosive = Facepunch.Pool.Get<ProtoBuf.TimedExplosive>();
		}
		if (info.forDisk)
		{
			info.msg.explosive.freq = this.GetFrequency();
		}
		info.msg.explosive.creatorID = this.creatorPlayerID;
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0008EA21 File Offset: 0x0008CC21
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetFrequency(this.RFFrequency);
		base.InvokeRandomized(new Action(this.DecayCheck), this.decayTickDuration, this.decayTickDuration, 10f);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0008EA58 File Offset: 0x0008CC58
	public void DecayCheck()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		global::BasePlayer basePlayer = global::BasePlayer.FindByID(this.creatorPlayerID);
		if (basePlayer != null && (buildingPrivilege == null || !buildingPrivilege.IsAuthed(basePlayer)))
		{
			this.minutesDecayed += this.decayTickDuration / 60f;
		}
		if (this.minutesDecayed >= this.minutesUntilDecayed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0008EAC4 File Offset: 0x0008CCC4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.RFFrequency > 0)
		{
			if (base.IsInvoking(new Action(this.Explode)))
			{
				base.CancelInvoke(new Action(this.Explode));
			}
			this.SetFrequency(this.RFFrequency);
			this.ArmRF();
		}
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x0008EB1A File Offset: 0x0008CD1A
	internal override void DoServerDestroy()
	{
		if (this.RFFrequency > 0)
		{
			RFManager.RemoveListener(this.RFFrequency, this);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0008EB37 File Offset: 0x0008CD37
	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(this.RFFrequency, newFreq, this, true, true);
		this.RFFrequency = newFreq;
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0008EB50 File Offset: 0x0008CD50
	public override void SetCreatorEntity(global::BaseEntity newCreatorEntity)
	{
		base.SetCreatorEntity(newCreatorEntity);
		global::BasePlayer component = newCreatorEntity.GetComponent<global::BasePlayer>();
		if (component)
		{
			this.creatorPlayerID = component.userID;
			if (this.GetFrequency() > 0)
			{
				component.ConsoleMessage("Frequency is:" + this.GetFrequency());
			}
		}
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0008EBA4 File Offset: 0x0008CDA4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.IsArmed())
		{
			return;
		}
		global::Item item = ItemManager.Create(this.pickupDefinition, 1, 0UL);
		if (item == null)
		{
			return;
		}
		item.instanceData.dataInt = this.GetFrequency();
		item.SetFlag(global::Item.Flag.IsOn, this.IsArmed());
		msg.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0002A700 File Offset: 0x00028900
	public bool IsArmed()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0008EC10 File Offset: 0x0008CE10
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.explosive != null)
		{
			this.creatorPlayerID = info.msg.explosive.creatorID;
			if (base.isServer)
			{
				if (info.fromDisk)
				{
					this.RFFrequency = info.msg.explosive.freq;
				}
				this.creatorEntity = global::BasePlayer.FindByID(this.creatorPlayerID);
			}
		}
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0008EC7E File Offset: 0x0008CE7E
	public bool CanPickup(global::BasePlayer player)
	{
		return this.IsArmed();
	}

	// Token: 0x04000ADA RID: 2778
	public SoundPlayer beepLoop;

	// Token: 0x04000ADB RID: 2779
	private ulong creatorPlayerID;

	// Token: 0x04000ADC RID: 2780
	public ItemDefinition pickupDefinition;

	// Token: 0x04000ADD RID: 2781
	public float minutesUntilDecayed = 1440f;

	// Token: 0x04000ADE RID: 2782
	private int RFFrequency = -1;

	// Token: 0x04000ADF RID: 2783
	private float decayTickDuration = 3600f;

	// Token: 0x04000AE0 RID: 2784
	private float minutesDecayed;
}
