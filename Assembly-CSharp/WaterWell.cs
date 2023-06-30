using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F0 RID: 240
public class WaterWell : LiquidContainer
{
	// Token: 0x06001519 RID: 5401 RVA: 0x000A64A4 File Offset: 0x000A46A4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterWell.OnRpcMessage", 0))
		{
			if (rpc == 2538739344U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Pump ");
				}
				using (TimeWarning.New("RPC_Pump", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2538739344U, "RPC_Pump", this, player, 3f))
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
							this.RPC_Pump(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Pump");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x000A660C File Offset: 0x000A480C
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x000A6630 File Offset: 0x000A4830
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Pump(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || player.IsDead() || player.IsSleeping())
		{
			return;
		}
		if (player.metabolism.calories.value < this.caloriesPerPump)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		player.metabolism.calories.value -= this.caloriesPerPump;
		player.metabolism.SendChangesToClient();
		this.currentPressure = Mathf.Clamp01(this.currentPressure + this.pressurePerPump);
		base.Invoke(new Action(this.StopPump), 1.8f);
		if (this.currentPressure >= 0f)
		{
			base.CancelInvoke(new Action(this.Produce));
			base.Invoke(new Action(this.Produce), 1f);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x000A6727 File Offset: 0x000A4927
	public void StopPump()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x000A673E File Offset: 0x000A493E
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x000A674F File Offset: 0x000A494F
	public void Produce()
	{
		base.inventory.AddItem(this.defaultLiquid, this.waterPerPump, 0UL, global::ItemContainer.LimitStack.Existing);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
		this.ScheduleTapOff();
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x000A6786 File Offset: 0x000A4986
	public void ScheduleTapOff()
	{
		base.CancelInvoke(new Action(this.TapOff));
		base.Invoke(new Action(this.TapOff), 1f);
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x000A67B1 File Offset: 0x000A49B1
	private void TapOff()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x000A67C4 File Offset: 0x000A49C4
	public void ReducePressure()
	{
		float num = UnityEngine.Random.Range(0.1f, 0.2f);
		this.currentPressure = Mathf.Clamp01(this.currentPressure - num);
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000A67F4 File Offset: 0x000A49F4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.waterwell = Facepunch.Pool.Get<ProtoBuf.WaterWell>();
		info.msg.waterwell.pressure = this.currentPressure;
		info.msg.waterwell.waterLevel = this.GetWaterAmount();
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000A6844 File Offset: 0x000A4A44
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.waterwell != null)
		{
			this.currentPressure = info.msg.waterwell.pressure;
		}
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000A6870 File Offset: 0x000A4A70
	public float GetWaterAmount()
	{
		if (!base.isServer)
		{
			return 0f;
		}
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return 0f;
		}
		return (float)slot.amount;
	}

	// Token: 0x04000D4B RID: 3403
	public Animator animator;

	// Token: 0x04000D4C RID: 3404
	private const global::BaseEntity.Flags Pumping = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000D4D RID: 3405
	private const global::BaseEntity.Flags WaterFlow = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000D4E RID: 3406
	public float caloriesPerPump = 5f;

	// Token: 0x04000D4F RID: 3407
	public float pressurePerPump = 0.2f;

	// Token: 0x04000D50 RID: 3408
	public float pressureForProduction = 1f;

	// Token: 0x04000D51 RID: 3409
	public float currentPressure;

	// Token: 0x04000D52 RID: 3410
	public int waterPerPump = 50;

	// Token: 0x04000D53 RID: 3411
	public GameObject waterLevelObj;

	// Token: 0x04000D54 RID: 3412
	public float waterLevelObjFullOffset;
}
