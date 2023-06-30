using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000053 RID: 83
public class CameraTool : HeldEntity
{
	// Token: 0x0600092C RID: 2348 RVA: 0x00057D04 File Offset: 0x00055F04
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CameraTool.OnRpcMessage", 0))
		{
			if (rpc == 3167878597U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVNoteScreenshot ");
				}
				using (TimeWarning.New("SVNoteScreenshot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3167878597U, "SVNoteScreenshot", this, player))
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
							this.SVNoteScreenshot(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVNoteScreenshot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void SVNoteScreenshot(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x0400061B RID: 1563
	public GameObjectRef screenshotEffect;
}
