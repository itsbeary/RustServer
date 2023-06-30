using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E2 RID: 226
public class TorchWeapon : BaseMelee
{
	// Token: 0x060013B6 RID: 5046 RVA: 0x0009DEE0 File Offset: 0x0009C0E0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TorchWeapon.OnRpcMessage", 0))
		{
			if (rpc == 2235491565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Extinguish ");
				}
				using (TimeWarning.New("Extinguish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2235491565U, "Extinguish", this, player))
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
							this.Extinguish(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Extinguish");
					}
				}
				return true;
			}
			if (rpc == 3010584743U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Ignite ");
				}
				using (TimeWarning.New("Ignite", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3010584743U, "Ignite", this, player))
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
							this.Ignite(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Ignite");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x0009E1D8 File Offset: 0x0009C3D8
	public override void GetAttackStats(HitInfo info)
	{
		base.GetAttackStats(info);
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			info.damageTypes.Add(DamageType.Heat, 1f);
		}
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x0009E1FB File Offset: 0x0009C3FB
	public override float GetConditionLoss()
	{
		return base.GetConditionLoss() + (base.HasFlag(BaseEntity.Flags.On) ? 6f : 0f);
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x0009E21C File Offset: 0x0009C41C
	public void SetIsOn(bool isOn)
	{
		if (isOn)
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.InvokeRepeating(new Action(this.UseFuel), 1f, 1f);
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.CancelInvoke(new Action(this.UseFuel));
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0009E26F File Offset: 0x0009C46F
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Ignite(BaseEntity.RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			this.SetIsOn(true);
		}
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x0009E285 File Offset: 0x0009C485
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Extinguish(BaseEntity.RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			this.SetIsOn(false);
		}
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x0009E29C File Offset: 0x0009C49C
	public void UseFuel()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(this.fuelTickAmount);
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0009E2C0 File Offset: 0x0009C4C0
	public override void OnHeldChanged()
	{
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			base.CancelInvoke(new Action(this.UseFuel));
		}
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x0009E2E8 File Offset: 0x0009C4E8
	public override string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < this.materialStrikeFX.Count; i++)
		{
			if (this.materialStrikeFX[i].materialName == materialName && this.materialStrikeFX[i].fx.isValid)
			{
				return this.materialStrikeFX[i].fx.resourcePath;
			}
		}
		if (base.HasFlag(BaseEntity.Flags.On) && this.litStrikeFX.isValid)
		{
			return this.litStrikeFX.resourcePath;
		}
		return this.strikeFX.resourcePath;
	}

	// Token: 0x04000C42 RID: 3138
	[NonSerialized]
	public float fuelTickAmount = 0.083333336f;

	// Token: 0x04000C43 RID: 3139
	[Header("TorchWeapon")]
	public AnimatorOverrideController LitHoldAnimationOverride;

	// Token: 0x04000C44 RID: 3140
	public bool ExtinguishUnderwater = true;

	// Token: 0x04000C45 RID: 3141
	public bool UseTurnOnOffAnimations;

	// Token: 0x04000C46 RID: 3142
	public GameObjectRef litStrikeFX;
}
