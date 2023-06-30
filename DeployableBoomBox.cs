using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000065 RID: 101
public class DeployableBoomBox : ContainerIOEntity, ICassettePlayer, IAudioConnectionSource
{
	// Token: 0x06000A34 RID: 2612 RVA: 0x0005E774 File Offset: 0x0005C974
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DeployableBoomBox.OnRpcMessage", 0))
		{
			if (rpc == 1918716764U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateRadioIP ");
				}
				using (TimeWarning.New("Server_UpdateRadioIP", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1918716764U, "Server_UpdateRadioIP", this, player, 2UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsVisible.Test(1918716764U, "Server_UpdateRadioIP", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_UpdateRadioIP(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_UpdateRadioIP");
					}
				}
				return true;
			}
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1785864031U, "ServerTogglePlay", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerTogglePlay(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000A35 RID: 2613 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x000037E7 File Offset: 0x000019E7
	public IOEntity ToEntity()
	{
		return this;
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0005EA90 File Offset: 0x0005CC90
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.PowerUsageWhilePlaying;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0005EA90 File Offset: 0x0005CC90
	public override int DesiredPower()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.PowerUsageWhilePlaying;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0005EAA4 File Offset: 0x0005CCA4
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsOn())
			{
				this.BoxController.ServerTogglePlay(false);
			}
			return;
		}
		if (this.IsPowered() && !base.IsConnectedToAnySlot(this, inputSlot, IOEntity.backtracking, false))
		{
			this.BoxController.ServerTogglePlay(inputAmount > 0);
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0005EB00 File Offset: 0x0005CD00
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
		this.BoxController.HurtCallback = new Action<float>(this.HurtCallback);
		if (this.IsStatic)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0005EB58 File Offset: 0x0005CD58
	private bool ItemFilter(Item item, int count)
	{
		ItemDefinition[] validCassettes = this.BoxController.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x000582BA File Offset: 0x000564BA
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0005EB92 File Offset: 0x0005CD92
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (inputSlot != 0)
		{
			return this.currentEnergy;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0005EBA6 File Offset: 0x0005CDA6
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		this.BoxController.ServerTogglePlay(msg);
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0005EBB4 File Offset: 0x0005CDB4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		this.BoxController.Server_UpdateRadioIP(msg);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0005EBC2 File Offset: 0x0005CDC2
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.BoxController.Save(info);
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0005EBD7 File Offset: 0x0005CDD7
	public bool ClearRadioByUserId(ulong id)
	{
		return this.BoxController.ClearRadioByUserId(id);
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0005EBE5 File Offset: 0x0005CDE5
	public void OnCassetteInserted(Cassette c)
	{
		this.BoxController.OnCassetteInserted(c);
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0005EBF3 File Offset: 0x0005CDF3
	public void OnCassetteRemoved(Cassette c)
	{
		this.BoxController.OnCassetteRemoved(c);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0005EC01 File Offset: 0x0005CE01
	public void HurtCallback(float amount)
	{
		base.Hurt(amount, DamageType.Decay, null, true);
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0005EC0E File Offset: 0x0005CE0E
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		this.BoxController.Load(info);
		base.Load(info);
		if (base.isServer && this.IsStatic)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
	}

	// Token: 0x040006BB RID: 1723
	public BoomBox BoxController;

	// Token: 0x040006BC RID: 1724
	public int PowerUsageWhilePlaying = 10;

	// Token: 0x040006BD RID: 1725
	public const int MaxBacktrackHopsClient = 30;

	// Token: 0x040006BE RID: 1726
	public bool IsStatic;
}
