using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000073 RID: 115
public class EngineSwitch : BaseEntity
{
	// Token: 0x06000AF6 RID: 2806 RVA: 0x00062F40 File Offset: 0x00061140
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("EngineSwitch.OnRpcMessage", 0))
		{
			if (rpc == 1249530220U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartEngine ");
				}
				using (TimeWarning.New("StartEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1249530220U, "StartEngine", this, player, 3f))
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
							this.StartEngine(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in StartEngine");
					}
				}
				return true;
			}
			if (rpc == 1739656243U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopEngine ");
				}
				using (TimeWarning.New("StopEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1739656243U, "StopEngine", this, player, 3f))
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
							this.StopEngine(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StopEngine");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00063240 File Offset: 0x00061440
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void StopEngine(BaseEntity.RPCMessage msg)
	{
		MiningQuarry miningQuarry = base.GetParentEntity() as MiningQuarry;
		if (miningQuarry)
		{
			miningQuarry.EngineSwitch(false);
		}
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x00063268 File Offset: 0x00061468
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void StartEngine(BaseEntity.RPCMessage msg)
	{
		MiningQuarry miningQuarry = base.GetParentEntity() as MiningQuarry;
		if (miningQuarry)
		{
			miningQuarry.EngineSwitch(true);
		}
	}
}
