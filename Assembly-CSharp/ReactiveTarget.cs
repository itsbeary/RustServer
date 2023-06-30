using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B8 RID: 184
public class ReactiveTarget : IOEntity
{
	// Token: 0x06001093 RID: 4243 RVA: 0x000890C8 File Offset: 0x000872C8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ReactiveTarget.OnRpcMessage", 0))
		{
			if (rpc == 1798082523U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lower ");
				}
				using (TimeWarning.New("RPC_Lower", 0))
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
							this.RPC_Lower(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Lower");
					}
				}
				return true;
			}
			if (rpc == 2169477377U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Reset ");
				}
				using (TimeWarning.New("RPC_Reset", 0))
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
							this.RPC_Reset(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Reset");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x00089328 File Offset: 0x00087528
	public void OnHitShared(HitInfo info)
	{
		if (this.IsKnockedDown())
		{
			return;
		}
		bool flag = info.HitBone == StringPool.Get("target_collider");
		bool flag2 = info.HitBone == StringPool.Get("target_collider_bullseye");
		if (!flag && !flag2)
		{
			return;
		}
		if (base.isServer)
		{
			float num = info.damageTypes.Total();
			if (flag2)
			{
				num *= 2f;
				Effect.server.Run(this.bullseyeEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
			}
			this.knockdownHealth -= num;
			if (this.knockdownHealth <= 0f)
			{
				Effect.server.Run(this.knockdownEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				this.QueueReset();
				this.SendPowerBurst();
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
			else
			{
				base.ClientRPC<NetworkableId>(null, "HitEffect", info.Initiator.net.ID);
			}
			base.Hurt(1f, DamageType.Suicide, info.Initiator, false);
		}
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x000622D3 File Offset: 0x000604D3
	public bool IsKnockedDown()
	{
		return !base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x00089447 File Offset: 0x00087647
	public override void OnAttacked(HitInfo info)
	{
		this.OnHitShared(info);
		base.OnAttacked(info);
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x00089457 File Offset: 0x00087657
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && this.CanToggle();
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0008946A File Offset: 0x0008766A
	public bool CanToggle()
	{
		return UnityEngine.Time.time > this.lastToggleTime + 1f;
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x0008947F File Offset: 0x0008767F
	public void QueueReset()
	{
		base.Invoke(new Action(this.ResetTarget), 6f);
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x00089498 File Offset: 0x00087698
	public void ResetTarget()
	{
		if (!this.IsKnockedDown() || !this.CanToggle())
		{
			return;
		}
		base.CancelInvoke(new Action(this.ResetTarget));
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.knockdownHealth = 100f;
		this.SendPowerBurst();
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x000894D8 File Offset: 0x000876D8
	private void LowerTarget()
	{
		if (this.IsKnockedDown() || !this.CanToggle())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.SendPowerBurst();
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x000894FB File Offset: 0x000876FB
	private void SendPowerBurst()
	{
		this.lastToggleTime = UnityEngine.Time.time;
		base.MarkDirtyForceUpdateOutputs();
		base.Invoke(new Action(base.MarkDirtyForceUpdateOutputs), this.activationPowerTime * 1.01f);
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x0000441C File Offset: 0x0000261C
	public override int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x0008952C File Offset: 0x0008772C
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			return;
		}
		if (inputAmount > 0)
		{
			if (inputSlot == 1)
			{
				this.ResetTarget();
				return;
			}
			if (inputSlot == 2)
			{
				this.LowerTarget();
			}
		}
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00089553 File Offset: 0x00087753
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.IsKnockedDown())
		{
			if (this.IsPowered())
			{
				return base.GetPassthroughAmount(0);
			}
			if (UnityEngine.Time.time < this.lastToggleTime + this.activationPowerTime)
			{
				return this.activationPowerAmount;
			}
		}
		return 0;
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00089589 File Offset: 0x00087789
	[BaseEntity.RPC_Server]
	public void RPC_Reset(BaseEntity.RPCMessage msg)
	{
		this.ResetTarget();
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00089591 File Offset: 0x00087791
	[BaseEntity.RPC_Server]
	public void RPC_Lower(BaseEntity.RPCMessage msg)
	{
		this.LowerTarget();
	}

	// Token: 0x04000A91 RID: 2705
	public Animator myAnimator;

	// Token: 0x04000A92 RID: 2706
	public GameObjectRef bullseyeEffect;

	// Token: 0x04000A93 RID: 2707
	public GameObjectRef knockdownEffect;

	// Token: 0x04000A94 RID: 2708
	public float activationPowerTime = 0.5f;

	// Token: 0x04000A95 RID: 2709
	public int activationPowerAmount = 1;

	// Token: 0x04000A96 RID: 2710
	private float lastToggleTime = float.NegativeInfinity;

	// Token: 0x04000A97 RID: 2711
	private float knockdownHealth = 100f;
}
