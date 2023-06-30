using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A3 RID: 163
public class ModularCarOven : BaseOven
{
	// Token: 0x06000F16 RID: 3862 RVA: 0x0007F0BC File Offset: 0x0007D2BC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCarOven.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000F17 RID: 3863 RVA: 0x0007F224 File Offset: 0x0007D424
	private BaseVehicleModule ModuleParent
	{
		get
		{
			if (this.moduleParent != null)
			{
				return this.moduleParent;
			}
			this.moduleParent = base.GetParentEntity() as BaseVehicleModule;
			return this.moduleParent;
		}
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x0007F252 File Offset: 0x0007D452
	public override void ResetState()
	{
		base.ResetState();
		this.moduleParent = null;
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x0007F261 File Offset: 0x0007D461
	protected override void SVSwitch(BaseEntity.RPCMessage msg)
	{
		if (this.ModuleParent == null || !this.ModuleParent.CanBeLooted(msg.player) || WaterLevel.Test(base.transform.position, true, false, null))
		{
			return;
		}
		base.SVSwitch(msg);
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x0007F2A1 File Offset: 0x0007D4A1
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return !(this.ModuleParent == null) && this.ModuleParent.CanBeLooted(player) && base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x0007F2CA File Offset: 0x0007D4CA
	protected override void OnCooked()
	{
		base.OnCooked();
		if (WaterLevel.Test(base.transform.position, true, false, null))
		{
			this.StopCooking();
		}
	}

	// Token: 0x040009C9 RID: 2505
	private BaseVehicleModule moduleParent;
}
