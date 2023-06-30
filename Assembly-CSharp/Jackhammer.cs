using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008B RID: 139
public class Jackhammer : BaseMelee
{
	// Token: 0x06000D1C RID: 3356 RVA: 0x00070AB0 File Offset: 0x0006ECB0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Jackhammer.OnRpcMessage", 0))
		{
			if (rpc == 1699910227U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetEngineStatus ");
				}
				using (TimeWarning.New("Server_SetEngineStatus", 0))
				{
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
							this.Server_SetEngineStatus(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_SetEngineStatus");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x0000441C File Offset: 0x0000261C
	public bool HasAmmo()
	{
		return true;
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00070BD4 File Offset: 0x0006EDD4
	[BaseEntity.RPC_Server]
	public void Server_SetEngineStatus(BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(msg.read.Bit());
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x00070BE7 File Offset: 0x0006EDE7
	public void SetEngineStatus(bool on)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, on, false, true);
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x00070BF7 File Offset: 0x0006EDF7
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x04000862 RID: 2146
	public float HotspotBonusScale = 1f;
}
