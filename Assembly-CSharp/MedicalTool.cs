using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009B RID: 155
public class MedicalTool : AttackEntity
{
	// Token: 0x06000E17 RID: 3607 RVA: 0x000772C4 File Offset: 0x000754C4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MedicalTool.OnRpcMessage", 0))
		{
			if (rpc == 789049461U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UseOther ");
				}
				using (TimeWarning.New("UseOther", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(789049461U, "UseOther", this, player))
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
							this.UseOther(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in UseOther");
					}
				}
				return true;
			}
			if (rpc == 2918424470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UseSelf ");
				}
				using (TimeWarning.New("UseSelf", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2918424470U, "UseSelf", this, player))
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
							this.UseSelf(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in UseSelf");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x000775BC File Offset: 0x000757BC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void UseOther(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount() || !this.canUseOnOther)
		{
			return;
		}
		BasePlayer basePlayer = BaseNetworkable.serverEntities.Find(msg.read.EntityID()) as BasePlayer;
		if (basePlayer != null && Vector3.Distance(basePlayer.transform.position, player.transform.position) < 4f)
		{
			base.ClientRPCPlayer(null, player, "Reset");
			this.GiveEffectsTo(basePlayer);
			base.UseItemAmount(1);
			base.StartAttackCooldown(this.repeatDelay);
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0007766C File Offset: 0x0007586C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void UseSelf(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		base.ClientRPCPlayer(null, player, "Reset");
		this.GiveEffectsTo(player);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x000776CC File Offset: 0x000758CC
	public override void ServerUse()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!ownerPlayer.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		this.GiveEffectsTo(ownerPlayer);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		if (ownerPlayer.IsNpc)
		{
			ownerPlayer.SignalBroadcast(BaseEntity.Signal.Attack, null);
		}
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00077740 File Offset: 0x00075940
	private void GiveEffectsTo(BasePlayer player)
	{
		if (!player)
		{
			return;
		}
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		ItemModConsumable component = ownerItemDefinition.GetComponent<ItemModConsumable>();
		if (!component)
		{
			Debug.LogWarning("No consumable for medicaltool :" + base.name);
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		Analytics.Azure.OnMedUsed(ownerItemDefinition.shortname, ownerPlayer, player);
		if (player != ownerPlayer && player.IsWounded() && this.canRevive)
		{
			player.StopWounded(ownerPlayer);
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			if (consumableEffect.type == MetabolismAttribute.Type.Health)
			{
				player.health += consumableEffect.amount;
			}
			else
			{
				player.metabolism.ApplyChange(consumableEffect.type, consumableEffect.amount, consumableEffect.time);
			}
		}
		if (player is BasePet)
		{
			player.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x04000920 RID: 2336
	public float healDurationSelf = 4f;

	// Token: 0x04000921 RID: 2337
	public float healDurationOther = 4f;

	// Token: 0x04000922 RID: 2338
	public float healDurationOtherWounded = 7f;

	// Token: 0x04000923 RID: 2339
	public float maxDistanceOther = 2f;

	// Token: 0x04000924 RID: 2340
	public bool canUseOnOther = true;

	// Token: 0x04000925 RID: 2341
	public bool canRevive = true;
}
