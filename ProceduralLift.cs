using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B7 RID: 183
public class ProceduralLift : global::BaseEntity
{
	// Token: 0x06001085 RID: 4229 RVA: 0x00088C4C File Offset: 0x00086E4C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ProceduralLift.OnRpcMessage", 0))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2657791441U, "RPC_UseLift", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
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

	// Token: 0x06001086 RID: 4230 RVA: 0x00088DB4 File Offset: 0x00086FB4
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00088E02 File Offset: 0x00087002
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		this.MoveToFloor((this.floorIndex + 1) % this.stops.Length);
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x00088E32 File Offset: 0x00087032
	public override void ServerInit()
	{
		base.ServerInit();
		this.SnapToFloor(0);
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x00088E41 File Offset: 0x00087041
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lift = Facepunch.Pool.Get<ProtoBuf.Lift>();
		info.msg.lift.floor = this.floorIndex;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x00088E70 File Offset: 0x00087070
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.lift != null)
		{
			if (this.floorIndex == -1)
			{
				this.SnapToFloor(info.msg.lift.floor);
			}
			else
			{
				this.MoveToFloor(info.msg.lift.floor);
			}
		}
		base.Load(info);
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00088EC8 File Offset: 0x000870C8
	private void ResetLift()
	{
		this.MoveToFloor(0);
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00088ED4 File Offset: 0x000870D4
	private void MoveToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x00088F28 File Offset: 0x00087128
	private void SnapToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		this.cabin.transform.position = proceduralLiftStop.transform.position;
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x00088FA4 File Offset: 0x000871A4
	private void OnFinishedMoving()
	{
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			if (this.floorIndex != 0)
			{
				base.Invoke(new Action(this.ResetLift), this.resetDelay);
			}
		}
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x00088FE4 File Offset: 0x000871E4
	protected void Update()
	{
		if (this.floorIndex < 0 || this.floorIndex > this.stops.Length - 1)
		{
			return;
		}
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			return;
		}
		this.cabin.transform.position = Vector3.MoveTowards(this.cabin.transform.position, proceduralLiftStop.transform.position, this.movementSpeed * UnityEngine.Time.deltaTime);
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			this.OnFinishedMoving();
		}
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StartMovementSounds()
	{
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopMovementSounds()
	{
	}

	// Token: 0x04000A86 RID: 2694
	public float movementSpeed = 1f;

	// Token: 0x04000A87 RID: 2695
	public float resetDelay = 5f;

	// Token: 0x04000A88 RID: 2696
	public ProceduralLiftCabin cabin;

	// Token: 0x04000A89 RID: 2697
	public ProceduralLiftStop[] stops;

	// Token: 0x04000A8A RID: 2698
	public GameObjectRef triggerPrefab;

	// Token: 0x04000A8B RID: 2699
	public string triggerBone;

	// Token: 0x04000A8C RID: 2700
	private int floorIndex = -1;

	// Token: 0x04000A8D RID: 2701
	public SoundDefinition startSoundDef;

	// Token: 0x04000A8E RID: 2702
	public SoundDefinition stopSoundDef;

	// Token: 0x04000A8F RID: 2703
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x04000A90 RID: 2704
	private Sound movementLoopSound;
}
