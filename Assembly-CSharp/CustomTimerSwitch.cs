using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000064 RID: 100
public class CustomTimerSwitch : TimerSwitch
{
	// Token: 0x06000A2F RID: 2607 RVA: 0x0005E5B4 File Offset: 0x0005C7B4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomTimerSwitch.OnRpcMessage", 0))
		{
			if (rpc == 1019813162U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SetTime ");
				}
				using (TimeWarning.New("SERVER_SetTime", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1019813162U, "SERVER_SetTime", this, player, 3f))
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
							this.SERVER_SetTime(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SERVER_SetTime");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0005E71C File Offset: 0x0005C91C
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && inputSlot == 1)
		{
			base.SwitchPressed();
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0005E734 File Offset: 0x0005C934
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SERVER_SetTime(BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		float num = msg.read.Float();
		this.timerLength = num;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0005E4D8 File Offset: 0x0005C6D8
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x040006BA RID: 1722
	public GameObjectRef timerPanelPrefab;
}
