using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DD RID: 221
public class SurveyCrater : BaseCombatEntity
{
	// Token: 0x06001376 RID: 4982 RVA: 0x0009B550 File Offset: 0x00099750
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SurveyCrater.OnRpcMessage", 0))
		{
			if (rpc == 3491246334U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AnalysisComplete ");
				}
				using (TimeWarning.New("AnalysisComplete", 0))
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
							this.AnalysisComplete(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AnalysisComplete");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x0009B674 File Offset: 0x00099874
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x0009B693 File Offset: 0x00099893
	public override void OnAttacked(HitInfo info)
	{
		bool isServer = base.isServer;
		base.OnAttacked(info);
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	public void AnalysisComplete(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x00032D46 File Offset: 0x00030F46
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x04000C24 RID: 3108
	private ResourceDispenser resourceDispenser;
}
