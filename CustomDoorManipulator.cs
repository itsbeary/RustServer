using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000063 RID: 99
public class CustomDoorManipulator : DoorManipulator
{
	// Token: 0x06000A24 RID: 2596 RVA: 0x0005E1D8 File Offset: 0x0005C3D8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0))
		{
			if (rpc == 1224330484U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoPair ");
				}
				using (TimeWarning.New("DoPair", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1224330484U, "DoPair", this, player, 3f))
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
							this.DoPair(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoPair");
					}
				}
				return true;
			}
			if (rpc == 3800726972U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerActionChange ");
				}
				using (TimeWarning.New("ServerActionChange", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3800726972U, "ServerActionChange", this, player, 3f))
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
							this.ServerActionChange(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ServerActionChange");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool PairWithLockedDoors()
	{
		return false;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0005E4D8 File Offset: 0x0005C6D8
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0005E4F6 File Offset: 0x0005C6F6
	public bool IsPaired()
	{
		return this.targetDoor != null;
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0005E504 File Offset: 0x0005C704
	public void RefreshDoor()
	{
		this.SetTargetDoor(this.targetDoor);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0005E512 File Offset: 0x0005C712
	private void OnPhysicsNeighbourChanged()
	{
		this.SetTargetDoor(this.targetDoor);
		base.Invoke(new Action(this.RefreshDoor), 0.1f);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0005E537 File Offset: 0x0005C737
	public override void SetupInitialDoorConnection()
	{
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0005E56C File Offset: 0x0005C76C
	public override void DoActionDoorMissing()
	{
		this.SetTargetDoor(null);
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0005E578 File Offset: 0x0005C778
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void DoPair(BaseEntity.RPCMessage msg)
	{
		Door targetDoor = this.targetDoor;
		Door door = base.FindDoor(this.PairWithLockedDoors());
		if (door != targetDoor)
		{
			this.SetTargetDoor(door);
		}
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerActionChange(BaseEntity.RPCMessage msg)
	{
	}
}
