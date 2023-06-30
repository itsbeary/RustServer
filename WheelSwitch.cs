using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F1 RID: 241
public class WheelSwitch : global::IOEntity
{
	// Token: 0x06001526 RID: 5414 RVA: 0x000A68DC File Offset: 0x000A4ADC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WheelSwitch.OnRpcMessage", 0))
		{
			if (rpc == 2223603322U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginRotate ");
				}
				using (TimeWarning.New("BeginRotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2223603322U, "BeginRotate", this, player, 3f))
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
							this.BeginRotate(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BeginRotate");
					}
				}
				return true;
			}
			if (rpc == 434251040U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CancelRotate ");
				}
				using (TimeWarning.New("CancelRotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(434251040U, "CancelRotate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CancelRotate(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in CancelRotate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x000A6BDC File Offset: 0x000A4DDC
	public override void ResetIOState()
	{
		this.CancelPlayerRotation();
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x000A6BE4 File Offset: 0x000A4DE4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void BeginRotate(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingRotated())
		{
			return;
		}
		base.SetFlag(this.BeingRotated, true, false, true);
		this.rotatorPlayer = msg.player;
		base.InvokeRepeating(new Action(this.RotateProgress), 0f, this.progressTickRate);
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x000A6C34 File Offset: 0x000A4E34
	public void CancelPlayerRotation()
	{
		base.CancelInvoke(new Action(this.RotateProgress));
		base.SetFlag(this.BeingRotated, false, false, true);
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioslot.connectedTo.Get(true).IOInput(this, this.ioType, 0f, ioslot.connectedToSlot);
			}
		}
		this.rotatorPlayer = null;
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x000A6CBC File Offset: 0x000A4EBC
	public void RotateProgress()
	{
		if (!this.rotatorPlayer || this.rotatorPlayer.IsDead() || this.rotatorPlayer.IsSleeping() || Vector3Ex.Distance2D(this.rotatorPlayer.transform.position, base.transform.position) > 2f)
		{
			this.CancelPlayerRotation();
			return;
		}
		float num = this.kineticEnergyPerSec * this.progressTickRate;
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				num = ioslot.connectedTo.Get(true).IOInput(this, this.ioType, num, ioslot.connectedToSlot);
			}
		}
		if (num == 0f)
		{
			this.SetRotateProgress(this.rotateProgress + 0.1f);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x000A6D9C File Offset: 0x000A4F9C
	public void SetRotateProgress(float newValue)
	{
		float num = this.rotateProgress;
		this.rotateProgress = newValue;
		base.SetFlag(global::BaseEntity.Flags.Reserved4, num != newValue, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(this.StoppedRotatingCheck));
		base.Invoke(new Action(this.StoppedRotatingCheck), 0.25f);
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x000A6DFB File Offset: 0x000A4FFB
	public void StoppedRotatingCheck()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, true);
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x000A6BDC File Offset: 0x000A4DDC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void CancelRotate(global::BaseEntity.RPCMessage msg)
	{
		this.CancelPlayerRotation();
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x000A6E0C File Offset: 0x000A500C
	public void Powered()
	{
		float num = this.kineticEnergyPerSec * this.progressTickRate;
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				num = ioslot.connectedTo.Get(true).IOInput(this, this.ioType, num, ioslot.connectedToSlot);
			}
		}
		this.SetRotateProgress(this.rotateProgress + 0.1f);
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x000A6E88 File Offset: 0x000A5088
	public override float IOInput(global::IOEntity from, global::IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount < 0f)
		{
			this.SetRotateProgress(this.rotateProgress + inputAmount);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (inputType == global::IOEntity.IOType.Electric && slot == 1)
		{
			if (inputAmount == 0f)
			{
				base.CancelInvoke(new Action(this.Powered));
			}
			else
			{
				base.InvokeRepeating(new Action(this.Powered), 0f, this.progressTickRate);
			}
		}
		return Mathf.Clamp(inputAmount - 1f, 0f, inputAmount);
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x000A6F05 File Offset: 0x000A5105
	public bool IsBeingRotated()
	{
		return base.HasFlag(this.BeingRotated);
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x000A6F13 File Offset: 0x000A5113
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity == null)
		{
			return;
		}
		this.rotateProgress = info.msg.sphereEntity.radius;
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000A6F40 File Offset: 0x000A5140
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Facepunch.Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.rotateProgress;
	}

	// Token: 0x04000D55 RID: 3413
	public Transform wheelObj;

	// Token: 0x04000D56 RID: 3414
	public float rotateSpeed = 90f;

	// Token: 0x04000D57 RID: 3415
	public global::BaseEntity.Flags BeingRotated = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000D58 RID: 3416
	public global::BaseEntity.Flags RotatingLeft = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000D59 RID: 3417
	public global::BaseEntity.Flags RotatingRight = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000D5A RID: 3418
	public float rotateProgress;

	// Token: 0x04000D5B RID: 3419
	public Animator animator;

	// Token: 0x04000D5C RID: 3420
	public float kineticEnergyPerSec = 1f;

	// Token: 0x04000D5D RID: 3421
	private global::BasePlayer rotatorPlayer;

	// Token: 0x04000D5E RID: 3422
	private float progressTickRate = 0.1f;
}
