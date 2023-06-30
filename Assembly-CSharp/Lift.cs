using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000090 RID: 144
public class Lift : AnimatedBuildingBlock
{
	// Token: 0x06000D5D RID: 3421 RVA: 0x000721B4 File Offset: 0x000703B4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Lift.OnRpcMessage", 0))
		{
			if (rpc == 2657791441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLift ");
				}
				using (TimeWarning.New("RPC_UseLift", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2657791441U, "RPC_UseLift", this, player, 3f))
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
							this.RPC_UseLift(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UseLift");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x0007231C File Offset: 0x0007051C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		this.MoveUp();
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00072332 File Offset: 0x00070532
	private void MoveUp()
	{
		if (base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00072357 File Offset: 0x00070557
	private void MoveDown()
	{
		if (!base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x0007237C File Offset: 0x0007057C
	protected override void OnAnimatorDisabled()
	{
		if (base.isServer && base.IsOpen())
		{
			base.Invoke(new Action(this.MoveDown), this.resetDelay);
		}
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x000723A8 File Offset: 0x000705A8
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}

	// Token: 0x04000886 RID: 2182
	public GameObjectRef triggerPrefab;

	// Token: 0x04000887 RID: 2183
	public string triggerBone;

	// Token: 0x04000888 RID: 2184
	public float resetDelay = 5f;
}
