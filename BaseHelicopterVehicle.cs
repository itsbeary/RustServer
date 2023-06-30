using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000482 RID: 1154
public class BaseHelicopterVehicle : BaseVehicle
{
	// Token: 0x06002623 RID: 9763 RVA: 0x000F0DE6 File Offset: 0x000EEFE6
	public virtual float GetServiceCeiling()
	{
		return 1000f;
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000ED499 File Offset: 0x000EB699
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x06002625 RID: 9765 RVA: 0x000F0DED File Offset: 0x000EEFED
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x06002626 RID: 9766 RVA: 0x000F0E0B File Offset: 0x000EF00B
	public float MouseToBinary(float amount)
	{
		return Mathf.Clamp(amount, -1f, 1f);
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x000F0E20 File Offset: 0x000EF020
	public virtual void PilotInput(InputState inputState, BasePlayer player)
	{
		this.currentInputState.Reset();
		this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
		this.currentInputState.throttle -= ((inputState.IsDown(BUTTON.BACKWARD) || inputState.IsDown(BUTTON.DUCK)) ? 1f : 0f);
		this.currentInputState.pitch = inputState.current.mouseDelta.y;
		this.currentInputState.roll = -inputState.current.mouseDelta.x;
		this.currentInputState.yaw = (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		this.currentInputState.yaw -= (inputState.IsDown(BUTTON.LEFT) ? 1f : 0f);
		this.currentInputState.pitch = this.MouseToBinary(this.currentInputState.pitch);
		this.currentInputState.roll = this.MouseToBinary(this.currentInputState.roll);
		this.lastPlayerInputTime = Time.time;
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x000F0F4B File Offset: 0x000EF14B
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.PilotInput(inputState, player);
		}
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x000F0F60 File Offset: 0x000EF160
	public virtual void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		if (base.HasDriver())
		{
			float num = Vector3.Dot(Vector3.up, base.transform.right);
			float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = ((num < 0f) ? 1f : 0f);
			this.currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < -0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (num2 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
		else
		{
			this.currentInputState.throttle = -1f;
		}
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsEnginePowered()
	{
		return true;
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x000F1038 File Offset: 0x000EF238
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (Time.time > this.lastPlayerInputTime + 0.5f)
		{
			this.SetDefaultInputState();
		}
		base.EnableGlobalBroadcast(this.IsEngineOn());
		this.MovementUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved6, TOD_Sky.Instance.IsNight, false, true);
		foreach (GameObject gameObject in this.killTriggers)
		{
			bool flag = this.rigidBody.velocity.y < 0f;
			gameObject.SetActive(flag);
		}
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x000F10C3 File Offset: 0x000EF2C3
	public override void LightToggle(BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, !base.HasFlag(BaseEntity.Flags.Reserved5), false, true);
		}
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldApplyHoverForce()
	{
		return true;
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsEngineOn()
	{
		return true;
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000F10E9 File Offset: 0x000EF2E9
	public void ClearDamageTorque()
	{
		this.SetDamageTorque(Vector3.zero);
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000F10F6 File Offset: 0x000EF2F6
	public void SetDamageTorque(Vector3 newTorque)
	{
		this.damageTorque = newTorque;
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000F10FF File Offset: 0x000EF2FF
	public void AddDamageTorque(Vector3 torqueToAdd)
	{
		this.damageTorque += torqueToAdd;
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000F1114 File Offset: 0x000EF314
	public virtual void MovementUpdate()
	{
		if (!this.IsEngineOn())
		{
			return;
		}
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, helicopterInputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.8f, 1f);
		if (helicopterInputState.pitch != 0f || helicopterInputState.roll != 0f || helicopterInputState.yaw != 0f)
		{
			this.rigidBody.AddRelativeTorque(new Vector3(helicopterInputState.pitch * this.torqueScale.x, helicopterInputState.yaw * this.torqueScale.y, helicopterInputState.roll * this.torqueScale.z), ForceMode.Force);
		}
		if (this.damageTorque != Vector3.zero)
		{
			this.rigidBody.AddRelativeTorque(new Vector3(this.damageTorque.x, this.damageTorque.y, this.damageTorque.z), ForceMode.Force);
		}
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime * this.thrustLerpSpeed);
		float num = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num2 = Mathf.InverseLerp(this.liftDotMax, 1f, num);
		float serviceCeiling = this.GetServiceCeiling();
		this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), Time.deltaTime);
		float num3 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + serviceCeiling - 20f, this.avgTerrainHeight + serviceCeiling, base.transform.position.y);
		num2 *= num3;
		float num4 = 1f - Mathf.InverseLerp(this.altForceDotMin, 1f, num);
		Vector3 vector = Vector3.up * this.engineThrustMax * this.liftFraction * this.currentThrottle * num2;
		Vector3 vector2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * num4;
		if (this.ShouldApplyHoverForce())
		{
			float num5 = this.rigidBody.mass * -Physics.gravity.y;
			this.rigidBody.AddForce(base.transform.up * num5 * num2 * this.hoverForceScale, ForceMode.Force);
		}
		this.rigidBody.AddForce(vector, ForceMode.Force);
		this.rigidBody.AddForce(vector2, ForceMode.Force);
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000F13D4 File Offset: 0x000EF5D4
	public void DelayedImpactDamage()
	{
		float explosionForceMultiplier = this.explosionForceMultiplier;
		this.explosionForceMultiplier = 0f;
		base.Hurt(this.pendingImpactDamage * this.MaxHealth(), DamageType.Explosion, this, false);
		this.pendingImpactDamage = 0f;
		this.explosionForceMultiplier = explosionForceMultiplier;
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CollisionDamageEnabled()
	{
		return true;
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000F141C File Offset: 0x000EF61C
	public void ProcessCollision(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.CollisionDamageEnabled())
		{
			return;
		}
		if (Time.time < this.nextDamageTime)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		if (collision.gameObject && ((1 << collision.collider.gameObject.layer) & 1218543873) <= 0)
		{
			return;
		}
		float num = Mathf.InverseLerp(5f, 30f, magnitude);
		if (num > 0f)
		{
			this.pendingImpactDamage += Mathf.Max(num, 0.15f);
			if (Vector3.Dot(base.transform.up, Vector3.up) < 0.5f)
			{
				this.pendingImpactDamage *= 5f;
			}
			if (Time.time > this.nextEffectTime)
			{
				this.nextEffectTime = Time.time + 0.25f;
				if (this.impactEffectSmall.isValid)
				{
					Vector3 vector = collision.GetContact(0).point;
					vector += (base.transform.position - vector) * 0.25f;
					Effect.server.Run(this.impactEffectSmall.resourcePath, vector, base.transform.up, null, false);
				}
			}
			this.rigidBody.AddForceAtPosition(collision.GetContact(0).normal * (1f + 3f * num), collision.GetContact(0).point, ForceMode.VelocityChange);
			this.nextDamageTime = Time.time + 0.333f;
			base.Invoke(new Action(this.DelayedImpactDamage), 0.015f);
		}
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000F15CE File Offset: 0x000EF7CE
	private void OnCollisionEnter(Collision collision)
	{
		this.ProcessCollision(collision);
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000F15D8 File Offset: 0x000EF7D8
	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnKilled(info);
			return;
		}
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		Vector3 vector = this.rigidBody.velocity * 0.25f;
		List<ServerGib> list = null;
		if (this.serverGibs.isValid)
		{
			GameObject gibSource = this.serverGibs.Get().GetComponent<ServerGib>()._gibSource;
			list = ServerGib.CreateGibs(this.serverGibs.resourcePath, base.gameObject, gibSource, vector, 3f);
		}
		Vector3 vector2 = base.CenterPoint();
		if (this.fireBall.isValid && !base.InSafeZone())
		{
			for (int i = 0; i < 12; i++)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, vector2, base.transform.rotation, true);
				if (baseEntity)
				{
					float num = 3f;
					float num2 = 10f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					onUnitSphere.Normalize();
					float num3 = UnityEngine.Random.Range(0.5f, 4f);
					RaycastHit raycastHit;
					bool flag = Physics.Raycast(vector2, onUnitSphere, out raycastHit, num3, 1218652417);
					Vector3 vector3 = raycastHit.point;
					if (!flag)
					{
						vector3 = vector2 + onUnitSphere * num3;
					}
					vector3 -= onUnitSphere * 0.5f;
					baseEntity.transform.position = vector3;
					Collider component = baseEntity.GetComponent<Collider>();
					baseEntity.Spawn();
					baseEntity.SetVelocity(vector + onUnitSphere * UnityEngine.Random.Range(num, num2));
					if (list != null)
					{
						foreach (ServerGib serverGib in list)
						{
							Physics.IgnoreCollision(component, serverGib.GetCollider(), true);
						}
					}
				}
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x04001E8B RID: 7819
	[Header("Helicopter")]
	public float engineThrustMax;

	// Token: 0x04001E8C RID: 7820
	public Vector3 torqueScale;

	// Token: 0x04001E8D RID: 7821
	public Transform com;

	// Token: 0x04001E8E RID: 7822
	public GameObject[] killTriggers;

	// Token: 0x04001E8F RID: 7823
	[Header("Effects")]
	public Transform[] GroundPoints;

	// Token: 0x04001E90 RID: 7824
	public Transform[] GroundEffects;

	// Token: 0x04001E91 RID: 7825
	public GameObjectRef serverGibs;

	// Token: 0x04001E92 RID: 7826
	public GameObjectRef explosionEffect;

	// Token: 0x04001E93 RID: 7827
	public GameObjectRef fireBall;

	// Token: 0x04001E94 RID: 7828
	public GameObjectRef impactEffectSmall;

	// Token: 0x04001E95 RID: 7829
	public GameObjectRef impactEffectLarge;

	// Token: 0x04001E96 RID: 7830
	[Header("Sounds")]
	public SoundDefinition flightEngineSoundDef;

	// Token: 0x04001E97 RID: 7831
	public SoundDefinition flightThwopsSoundDef;

	// Token: 0x04001E98 RID: 7832
	public float rotorGainModSmoothing = 0.25f;

	// Token: 0x04001E99 RID: 7833
	public float engineGainMin = 0.5f;

	// Token: 0x04001E9A RID: 7834
	public float engineGainMax = 1f;

	// Token: 0x04001E9B RID: 7835
	public float thwopGainMin = 0.5f;

	// Token: 0x04001E9C RID: 7836
	public float thwopGainMax = 1f;

	// Token: 0x04001E9D RID: 7837
	public float currentThrottle;

	// Token: 0x04001E9E RID: 7838
	public float avgThrust;

	// Token: 0x04001E9F RID: 7839
	public float liftDotMax = 0.75f;

	// Token: 0x04001EA0 RID: 7840
	public float altForceDotMin = 0.85f;

	// Token: 0x04001EA1 RID: 7841
	public float liftFraction = 0.25f;

	// Token: 0x04001EA2 RID: 7842
	public float thrustLerpSpeed = 1f;

	// Token: 0x04001EA3 RID: 7843
	private float avgTerrainHeight;

	// Token: 0x04001EA4 RID: 7844
	public const BaseEntity.Flags Flag_InternalLights = BaseEntity.Flags.Reserved6;

	// Token: 0x04001EA5 RID: 7845
	protected BaseHelicopterVehicle.HelicopterInputState currentInputState = new BaseHelicopterVehicle.HelicopterInputState();

	// Token: 0x04001EA6 RID: 7846
	protected float lastPlayerInputTime;

	// Token: 0x04001EA7 RID: 7847
	protected float hoverForceScale = 0.99f;

	// Token: 0x04001EA8 RID: 7848
	protected Vector3 damageTorque;

	// Token: 0x04001EA9 RID: 7849
	private float nextDamageTime;

	// Token: 0x04001EAA RID: 7850
	private float nextEffectTime;

	// Token: 0x04001EAB RID: 7851
	private float pendingImpactDamage;

	// Token: 0x02000D16 RID: 3350
	public class HelicopterInputState
	{
		// Token: 0x06005049 RID: 20553 RVA: 0x001A85D8 File Offset: 0x001A67D8
		public void Reset()
		{
			this.throttle = 0f;
			this.roll = 0f;
			this.yaw = 0f;
			this.pitch = 0f;
			this.groundControl = false;
		}

		// Token: 0x040046BA RID: 18106
		public float throttle;

		// Token: 0x040046BB RID: 18107
		public float roll;

		// Token: 0x040046BC RID: 18108
		public float yaw;

		// Token: 0x040046BD RID: 18109
		public float pitch;

		// Token: 0x040046BE RID: 18110
		public bool groundControl;
	}
}
