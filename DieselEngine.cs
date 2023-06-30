using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000069 RID: 105
public class DieselEngine : StorageContainer
{
	// Token: 0x06000A6E RID: 2670 RVA: 0x0005FAE8 File Offset: 0x0005DCE8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DieselEngine.OnRpcMessage", 0))
		{
			if (rpc == 578721460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - EngineSwitch ");
				}
				using (TimeWarning.New("EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(578721460U, "EngineSwitch", this, player, 6f))
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
							this.EngineSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in EngineSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x0005FC50 File Offset: 0x0005DE50
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName);
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x0005FC5C File Offset: 0x0005DE5C
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsOn())
		{
			if (this.cachedFuelTime <= UnityEngine.Time.fixedDeltaTime && this.ConsumeFuelItem(1))
			{
				this.cachedFuelTime += this.runningTimePerFuelUnit;
			}
			this.cachedFuelTime -= UnityEngine.Time.fixedDeltaTime;
			if (this.cachedFuelTime <= 0f)
			{
				this.cachedFuelTime = 0f;
				this.EngineOff();
			}
		}
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0005FCD4 File Offset: 0x0005DED4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	public void EngineSwitch(global::BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			if (this.GetFuelAmount() > 0)
			{
				this.EngineOn();
				if (GameInfo.HasAchievements && msg.player != null)
				{
					msg.player.stats.Add("excavator_activated", 1, global::Stats.All);
					msg.player.stats.Save(true);
					return;
				}
			}
		}
		else
		{
			this.EngineOff();
		}
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0005FD41 File Offset: 0x0005DF41
	public void TimedShutdown()
	{
		this.EngineOff();
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0005FD4C File Offset: 0x0005DF4C
	public bool ConsumeFuelItem(int amount = 1)
	{
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < amount)
		{
			return false;
		}
		slot.UseItem(amount);
		Analytics.Azure.OnExcavatorConsumeFuel(slot, amount, this);
		this.UpdateHasFuelFlag();
		return true;
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0005FD8C File Offset: 0x0005DF8C
	public int GetFuelAmount()
	{
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0005FDBA File Offset: 0x0005DFBA
	public void UpdateHasFuelFlag()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.GetFuelAmount() > 0, false, true);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0005FDD2 File Offset: 0x0005DFD2
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateHasFuelFlag();
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0005FDE1 File Offset: 0x0005DFE1
	public void EngineOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.BroadcastEntityMessage("DieselEngineOff", 20f, 1218652417);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0005FE02 File Offset: 0x0005E002
	public void EngineOn()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.BroadcastEntityMessage("DieselEngineOn", 20f, 1218652417);
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0005FE24 File Offset: 0x0005E024
	public void RescheduleEngineShutdown()
	{
		float num = 120f;
		base.Invoke(new Action(this.TimedShutdown), num);
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0005FE4A File Offset: 0x0005E04A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.IsOn())
		{
			base.BroadcastEntityMessage("DieselEngineOn", 20f, 1218652417);
			return;
		}
		base.BroadcastEntityMessage("DieselEngineOff", 20f, 1218652417);
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0005FE85 File Offset: 0x0005E085
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.cachedFuelTime;
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0005FEB4 File Offset: 0x0005E0B4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.cachedFuelTime = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0003018A File Offset: 0x0002E38A
	public bool HasFuel()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x040006CB RID: 1739
	public GameObjectRef rumbleEffect;

	// Token: 0x040006CC RID: 1740
	public Transform rumbleOrigin;

	// Token: 0x040006CD RID: 1741
	public const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040006CE RID: 1742
	public float runningTimePerFuelUnit = 120f;

	// Token: 0x040006CF RID: 1743
	private float cachedFuelTime;

	// Token: 0x040006D0 RID: 1744
	private const float rumbleMaxDistSq = 100f;

	// Token: 0x040006D1 RID: 1745
	private const string EXCAVATOR_ACTIVATED_STAT = "excavator_activated";
}
