using System;
using ConVar;
using Facepunch.Rust;
using Network;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DF RID: 223
public class ThrownWeapon : AttackEntity
{
	// Token: 0x0600139C RID: 5020 RVA: 0x0009CF08 File Offset: 0x0009B108
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ThrownWeapon.OnRpcMessage", 0))
		{
			if (rpc == 1513023343U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDrop ");
				}
				using (TimeWarning.New("DoDrop", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1513023343U, "DoDrop", this, player))
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
							this.DoDrop(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDrop");
					}
				}
				return true;
			}
			if (rpc == 1974840882U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoThrow ");
				}
				using (TimeWarning.New("DoThrow", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1974840882U, "DoThrow", this, player))
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
							this.DoThrow(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in DoThrow");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x00033068 File Offset: 0x00031268
	public override Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		return player.GetInheritedThrowVelocity(direction);
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0009D200 File Offset: 0x0009B400
	public void ServerThrow(Vector3 targetPosition)
	{
		if (base.isClient)
		{
			return;
		}
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!this.canThrowUnderwater && ownerPlayer.IsHeadUnderwater())
		{
			return;
		}
		Vector3 position = ownerPlayer.eyes.position;
		Vector3 vector = ownerPlayer.eyes.BodyForward();
		float num = 1f;
		base.SignalBroadcast(BaseEntity.Signal.Throw, string.Empty, null);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, position, Quaternion.LookRotation((this.overrideAngle == Vector3.zero) ? (-vector) : this.overrideAngle), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.SetCreatorEntity(ownerPlayer);
		Vector3 vector2 = vector + Quaternion.AngleAxis(10f, Vector3.right) * Vector3.up;
		float num2 = this.GetThrowVelocity(position, targetPosition, vector2);
		if (float.IsNaN(num2))
		{
			vector2 = vector + Quaternion.AngleAxis(20f, Vector3.right) * Vector3.up;
			num2 = this.GetThrowVelocity(position, targetPosition, vector2);
			if (float.IsNaN(num2))
			{
				num2 = 5f;
			}
		}
		baseEntity.SetVelocity(vector2 * num2 * num);
		if (this.tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * this.tumbleVelocity);
		}
		baseEntity.Spawn();
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		if (timedExplosive != null)
		{
			Analytics.Azure.OnExplosiveLaunched(ownerPlayer, baseEntity, null);
			float num3 = 0f;
			foreach (DamageTypeEntry damageTypeEntry in timedExplosive.damageTypes)
			{
				num3 += damageTypeEntry.amount;
			}
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.ThrownWeapon,
				Position = ownerPlayer.transform.position,
				Radius = 50f,
				DamagePotential = num3,
				InitiatorPlayer = ownerPlayer,
				Initiator = ownerPlayer,
				UsedEntity = timedExplosive
			});
			return;
		}
		Sense.Stimulate(new Sensation
		{
			Type = SensationType.ThrownWeapon,
			Position = ownerPlayer.transform.position,
			Radius = 50f,
			DamagePotential = 0f,
			InitiatorPlayer = ownerPlayer,
			Initiator = ownerPlayer,
			UsedEntity = this
		});
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0009D4E0 File Offset: 0x0009B6E0
	private float GetThrowVelocity(Vector3 throwPos, Vector3 targetPos, Vector3 aimDir)
	{
		Vector3 vector = targetPos - throwPos;
		float magnitude = new Vector2(vector.x, vector.z).magnitude;
		float y = vector.y;
		float magnitude2 = new Vector2(aimDir.x, aimDir.z).magnitude;
		float y2 = aimDir.y;
		float y3 = UnityEngine.Physics.gravity.y;
		return Mathf.Sqrt(0.5f * y3 * magnitude * magnitude / (magnitude2 * (magnitude2 * y - y2 * magnitude)));
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x0009D564 File Offset: 0x0009B764
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoThrow(BaseEntity.RPCMessage msg)
	{
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 normalized = msg.read.Vector3().normalized;
		float num = Mathf.Clamp01(msg.read.Float());
		if (msg.player.isMounted || msg.player.HasParent())
		{
			vector = msg.player.eyes.position;
		}
		else if (!base.ValidateEyePos(msg.player, vector))
		{
			return;
		}
		if (!this.canThrowUnderwater && msg.player.IsHeadUnderwater())
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector, Quaternion.LookRotation((this.overrideAngle == Vector3.zero) ? (-normalized) : this.overrideAngle), true);
		if (baseEntity == null)
		{
			return;
		}
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null && ownerItem.instanceData != null && ownerItem.HasFlag(Item.Flag.IsOn))
		{
			baseEntity.gameObject.SendMessage("SetFrequency", base.GetOwnerItem().instanceData.dataInt, SendMessageOptions.DontRequireReceiver);
		}
		baseEntity.SetCreatorEntity(msg.player);
		baseEntity.skinID = this.skinID;
		baseEntity.SetVelocity(this.GetInheritedVelocity(msg.player, normalized) + normalized * this.maxThrowVelocity * num + msg.player.estimatedVelocity * 0.5f);
		if (this.tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * this.tumbleVelocity);
		}
		baseEntity.Spawn();
		this.SetUpThrownWeapon(baseEntity);
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
		BasePlayer player = msg.player;
		if (player != null)
		{
			TimedExplosive timedExplosive = baseEntity as TimedExplosive;
			if (timedExplosive != null)
			{
				float num2 = 0f;
				foreach (DamageTypeEntry damageTypeEntry in timedExplosive.damageTypes)
				{
					num2 += damageTypeEntry.amount;
				}
				Sense.Stimulate(new Sensation
				{
					Type = SensationType.ThrownWeapon,
					Position = player.transform.position,
					Radius = 50f,
					DamagePotential = num2,
					InitiatorPlayer = player,
					Initiator = player,
					UsedEntity = timedExplosive
				});
				return;
			}
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.ThrownWeapon,
				Position = player.transform.position,
				Radius = 50f,
				DamagePotential = 0f,
				InitiatorPlayer = player,
				Initiator = player,
				UsedEntity = this
			});
		}
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x0009D898 File Offset: 0x0009BA98
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoDrop(BaseEntity.RPCMessage msg)
	{
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		if (!this.canThrowUnderwater && msg.player.IsHeadUnderwater())
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 normalized = msg.read.Vector3().normalized;
		if (msg.player.isMounted || msg.player.HasParent())
		{
			vector = msg.player.eyes.position;
		}
		else if (!base.ValidateEyePos(msg.player, vector))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector, Quaternion.LookRotation(Vector3.up), true);
		if (baseEntity == null)
		{
			return;
		}
		RaycastHit raycastHit;
		if (this.canStick && UnityEngine.Physics.SphereCast(new Ray(vector, normalized), 0.05f, out raycastHit, 1.5f, 1237003025))
		{
			Vector3 point = raycastHit.point;
			Vector3 normal = raycastHit.normal;
			BaseEntity baseEntity2 = raycastHit.GetEntity();
			Collider collider = raycastHit.collider;
			if (baseEntity2 && baseEntity2 is StabilityEntity && baseEntity is TimedExplosive)
			{
				baseEntity2 = baseEntity2.ToServer<BaseEntity>();
				TimedExplosive timedExplosive = baseEntity as TimedExplosive;
				timedExplosive.onlyDamageParent = true;
				timedExplosive.DoStick(point, normal, baseEntity2, collider);
				Analytics.Azure.OnExplosiveLaunched(msg.player, timedExplosive, null);
			}
			else
			{
				baseEntity.SetVelocity(normalized);
			}
		}
		else
		{
			baseEntity.SetVelocity(normalized);
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.skinID = this.skinID;
		baseEntity.Spawn();
		this.SetUpThrownWeapon(baseEntity);
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void SetUpThrownWeapon(BaseEntity ent)
	{
	}

	// Token: 0x04000C32 RID: 3122
	[Header("Throw Weapon")]
	public GameObjectRef prefabToThrow;

	// Token: 0x04000C33 RID: 3123
	public float maxThrowVelocity = 10f;

	// Token: 0x04000C34 RID: 3124
	public float tumbleVelocity;

	// Token: 0x04000C35 RID: 3125
	public Vector3 overrideAngle = Vector3.zero;

	// Token: 0x04000C36 RID: 3126
	public bool canStick = true;

	// Token: 0x04000C37 RID: 3127
	public bool canThrowUnderwater = true;
}
