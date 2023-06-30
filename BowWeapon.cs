using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004F RID: 79
public class BowWeapon : BaseProjectile
{
	// Token: 0x06000880 RID: 2176 RVA: 0x00051F04 File Offset: 0x00050104
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BowWeapon.OnRpcMessage", 0))
		{
			if (rpc == 4228048190U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BowReload ");
				}
				using (TimeWarning.New("BowReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4228048190U, "BowReload", this, player))
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
							this.BowReload(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BowReload");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x00052068 File Offset: 0x00050268
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void BowReload(BaseEntity.RPCMessage msg)
	{
		this.ReloadMagazine(-1);
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}
}
