using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C6 RID: 198
public class RustigeEgg : BaseCombatEntity
{
	// Token: 0x060011C2 RID: 4546 RVA: 0x0009001C File Offset: 0x0008E21C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RustigeEgg.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1455840454U, "RPC_Spin", this, player, 3f))
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
							this.RPC_Spin(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool IsSpinning()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Spin(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x0009031C File Offset: 0x0008E51C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Open(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == base.IsOpen())
		{
			return;
		}
		if (flag)
		{
			base.ClientRPC<Vector3>(null, "FaceEggPosition", msg.player.eyes.position);
			base.Invoke(new Action(this.CloseEgg), 60f);
		}
		else
		{
			base.CancelInvoke(new Action(this.CloseEgg));
		}
		base.SetFlag(BaseEntity.Flags.Open, flag, false, false);
		if (this.IsSpinning() && flag)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, false);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x000903C1 File Offset: 0x0008E5C1
	public void CloseEgg()
	{
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x04000B04 RID: 2820
	public const BaseEntity.Flags Flag_Spin = BaseEntity.Flags.Reserved1;

	// Token: 0x04000B05 RID: 2821
	public Transform eggRotationTransform;
}
