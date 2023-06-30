using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DA RID: 218
public class StaticInstrument : BaseMountable
{
	// Token: 0x0600133B RID: 4923 RVA: 0x0009A644 File Offset: 0x00098844
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StaticInstrument.OnRpcMessage", 0))
		{
			if (rpc == 1625188589U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_PlayNote ");
				}
				using (TimeWarning.New("Server_PlayNote", 0))
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
							this.Server_PlayNote(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_PlayNote");
					}
				}
				return true;
			}
			if (rpc == 705843933U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopNote ");
				}
				using (TimeWarning.New("Server_StopNote", 0))
				{
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
							this.Server_StopNote(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_StopNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x0009A8A4 File Offset: 0x00098AA4
	[BaseEntity.RPC_Server]
	private void Server_PlayNote(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		float num4 = msg.read.Float();
		this.KeyController.ProcessServerPlayedNote(base.GetMounted());
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", num, num2, num3, num4);
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x0009A904 File Offset: 0x00098B04
	[BaseEntity.RPC_Server]
	private void Server_StopNote(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		base.ClientRPC<int, int, int>(null, "Client_StopNote", num, num2, num3);
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsInstrument()
	{
		return true;
	}

	// Token: 0x04000C06 RID: 3078
	public AnimatorOverrideController AnimatorOverride;

	// Token: 0x04000C07 RID: 3079
	public bool ShowDeployAnimation;

	// Token: 0x04000C08 RID: 3080
	public InstrumentKeyController KeyController;

	// Token: 0x04000C09 RID: 3081
	public bool ShouldSuppressHandsAnimationLayer;
}
