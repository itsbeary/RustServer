using System;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class Sled : BaseVehicle, INotifyTrigger
{
	// Token: 0x06001803 RID: 6147 RVA: 0x000B493C File Offset: 0x000B2B3C
	public override void ServerInit()
	{
		base.ServerInit();
		this.terrainHandler = new VehicleTerrainHandler(this);
		this.terrainHandler.RayLength = 0.6f;
		this.rigidBody.centerOfMass = this.CentreOfMassTransform.localPosition;
		base.InvokeRandomized(new Action(this.DecayOverTime), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x000B49AC File Offset: 0x000B2BAC
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		this.UpdateGroundedFlag();
		this.UpdatePhysicsMaterial();
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x000B49D4 File Offset: 0x000B2BD4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.AnyMounted())
		{
			this.terrainHandler.FixedUpdate();
			if (!this.terrainHandler.IsGrounded)
			{
				Quaternion quaternion = Quaternion.FromToRotation(base.transform.up, Vector3.up) * this.rigidBody.rotation;
				if (Quaternion.Angle(this.rigidBody.rotation, quaternion) > this.VerticalAdjustmentAngleThreshold)
				{
					this.rigidBody.MoveRotation(Quaternion.Slerp(this.rigidBody.rotation, quaternion, Time.fixedDeltaTime * this.VerticalAdjustmentForce));
				}
			}
		}
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000B4A70 File Offset: 0x000B2C70
	private void UpdatePhysicsMaterial()
	{
		this.cachedMaterial = this.GetPhysicMaterial();
		Collider[] physicsMaterialTargets = this.PhysicsMaterialTargets;
		for (int i = 0; i < physicsMaterialTargets.Length; i++)
		{
			physicsMaterialTargets[i].sharedMaterial = this.cachedMaterial;
		}
		if (!this.AnyMounted() && this.rigidBody.IsSleeping())
		{
			base.CancelInvoke(new Action(this.UpdatePhysicsMaterial));
		}
		base.SetFlag(BaseEntity.Flags.Reserved2, this.terrainHandler.IsOnSnowOrIce, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand, false, true);
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x000B4B08 File Offset: 0x000B2D08
	private void UpdateGroundedFlag()
	{
		if (!this.AnyMounted() && this.rigidBody.IsSleeping())
		{
			base.CancelInvoke(new Action(this.UpdateGroundedFlag));
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, this.terrainHandler.IsGrounded, false, true);
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x000B4B54 File Offset: 0x000B2D54
	private PhysicMaterial GetPhysicMaterial()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1) || !this.AnyMounted())
		{
			return this.BrakeMaterial;
		}
		bool flag = this.terrainHandler.IsOnSnowOrIce || this.terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand;
		if (flag)
		{
			this.leftIce = 0f;
		}
		else if (this.leftIce < 2f)
		{
			flag = true;
		}
		if (!flag)
		{
			return this.NonSnowMaterial;
		}
		return this.SnowMaterial;
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x000B4BD8 File Offset: 0x000B2DD8
	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			this.initialForceScale = 0f;
			base.InvokeRepeating(new Action(this.ApplyInitialForce), 0f, 0.1f);
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		}
		if (!base.IsInvoking(new Action(this.UpdatePhysicsMaterial)))
		{
			base.InvokeRepeating(new Action(this.UpdatePhysicsMaterial), 0f, 0.5f);
		}
		if (!base.IsInvoking(new Action(this.UpdateGroundedFlag)))
		{
			base.InvokeRepeating(new Action(this.UpdateGroundedFlag), 0f, 0.1f);
		}
		if (this.rigidBody.IsSleeping())
		{
			this.rigidBody.WakeUp();
		}
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x000B4CA8 File Offset: 0x000B2EA8
	private void ApplyInitialForce()
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = ((Vector3.Dot(forward, -Vector3.up) > Vector3.Dot(-forward, -Vector3.up)) ? forward : (-forward));
		this.rigidBody.AddForce(vector * this.initialForceScale * (this.terrainHandler.IsOnSnowOrIce ? 1f : 0.25f), ForceMode.Acceleration);
		this.initialForceScale += this.InitialForceIncreaseRate;
		if (this.initialForceScale >= this.InitialForceCutoff && (this.rigidBody.velocity.magnitude > 1f || !this.terrainHandler.IsOnSnowOrIce))
		{
			base.CancelInvoke(new Action(this.ApplyInitialForce));
		}
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x000B4D84 File Offset: 0x000B2F84
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.1f || this.WaterFactor() > 0.25f)
		{
			this.DismountAllPlayers();
			return;
		}
		float num = (inputState.IsDown(BUTTON.LEFT) ? (-1f) : 0f);
		num += (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		if (inputState.IsDown(BUTTON.FORWARD) && this.lastNudge > this.NudgeCooldown && this.rigidBody.velocity.magnitude < this.MaxNudgeVelocity)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(base.transform.forward * this.NudgeForce, ForceMode.Impulse);
			this.rigidBody.AddForce(base.transform.up * this.NudgeForce * 0.5f, ForceMode.Impulse);
			this.lastNudge = 0f;
		}
		num *= this.TurnForce;
		Vector3 velocity = this.rigidBody.velocity;
		if (num != 0f)
		{
			base.transform.Rotate(Vector3.up * num * Time.deltaTime * velocity.magnitude, Space.Self);
		}
		if (this.terrainHandler.IsGrounded && Vector3.Dot(this.rigidBody.velocity.normalized, base.transform.forward) >= 0.5f)
		{
			this.rigidBody.velocity = Vector3.Lerp(this.rigidBody.velocity, base.transform.forward * velocity.magnitude, Time.deltaTime * this.DirectionMatchForce);
		}
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x000B4F5D File Offset: 0x000B315D
	private void DecayOverTime()
	{
		if (this.AnyMounted())
		{
			return;
		}
		base.Hurt(this.DecayAmount);
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x000B4F74 File Offset: 0x000B3174
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !player.isMounted;
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000B4F8C File Offset: 0x000B318C
	public void OnObjects(TriggerNotify trigger)
	{
		foreach (BaseEntity baseEntity in trigger.entityContents)
		{
			if (!(baseEntity is Sled))
			{
				BaseVehicleModule baseVehicleModule;
				if ((baseVehicleModule = baseEntity as BaseVehicleModule) != null && baseVehicleModule.Vehicle != null && (baseVehicleModule.Vehicle.IsOn() || !baseVehicleModule.Vehicle.IsStationary()))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
				BaseVehicle baseVehicle;
				if ((baseVehicle = baseEntity as BaseVehicle) != null && baseVehicle.HasDriver() && (baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
			}
		}
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEmpty()
	{
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001810 RID: 6160 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool BlocksDoors
	{
		get
		{
			return false;
		}
	}

	// Token: 0x040010C4 RID: 4292
	private const BaseEntity.Flags BrakeOn = BaseEntity.Flags.Reserved1;

	// Token: 0x040010C5 RID: 4293
	private const BaseEntity.Flags OnSnow = BaseEntity.Flags.Reserved2;

	// Token: 0x040010C6 RID: 4294
	private const BaseEntity.Flags IsGrounded = BaseEntity.Flags.Reserved3;

	// Token: 0x040010C7 RID: 4295
	private const BaseEntity.Flags OnSand = BaseEntity.Flags.Reserved4;

	// Token: 0x040010C8 RID: 4296
	public PhysicMaterial BrakeMaterial;

	// Token: 0x040010C9 RID: 4297
	public PhysicMaterial SnowMaterial;

	// Token: 0x040010CA RID: 4298
	public PhysicMaterial NonSnowMaterial;

	// Token: 0x040010CB RID: 4299
	public Transform CentreOfMassTransform;

	// Token: 0x040010CC RID: 4300
	public Collider[] PhysicsMaterialTargets;

	// Token: 0x040010CD RID: 4301
	public float InitialForceCutoff = 3f;

	// Token: 0x040010CE RID: 4302
	public float InitialForceIncreaseRate = 0.05f;

	// Token: 0x040010CF RID: 4303
	public float TurnForce = 1f;

	// Token: 0x040010D0 RID: 4304
	public float DirectionMatchForce = 1f;

	// Token: 0x040010D1 RID: 4305
	public float VerticalAdjustmentForce = 1f;

	// Token: 0x040010D2 RID: 4306
	public float VerticalAdjustmentAngleThreshold = 15f;

	// Token: 0x040010D3 RID: 4307
	public float NudgeCooldown = 3f;

	// Token: 0x040010D4 RID: 4308
	public float NudgeForce = 2f;

	// Token: 0x040010D5 RID: 4309
	public float MaxNudgeVelocity = 2f;

	// Token: 0x040010D6 RID: 4310
	public const float DecayFrequency = 60f;

	// Token: 0x040010D7 RID: 4311
	public float DecayAmount = 10f;

	// Token: 0x040010D8 RID: 4312
	public ParticleSystemContainer TrailEffects;

	// Token: 0x040010D9 RID: 4313
	public SoundDefinition enterSnowSoundDef;

	// Token: 0x040010DA RID: 4314
	public SoundDefinition snowSlideLoopSoundDef;

	// Token: 0x040010DB RID: 4315
	public SoundDefinition dirtSlideLoopSoundDef;

	// Token: 0x040010DC RID: 4316
	public AnimationCurve movementLoopGainCurve;

	// Token: 0x040010DD RID: 4317
	public AnimationCurve movementLoopPitchCurve;

	// Token: 0x040010DE RID: 4318
	private VehicleTerrainHandler terrainHandler;

	// Token: 0x040010DF RID: 4319
	private PhysicMaterial cachedMaterial;

	// Token: 0x040010E0 RID: 4320
	private float initialForceScale;

	// Token: 0x040010E1 RID: 4321
	private TimeSince leftIce;

	// Token: 0x040010E2 RID: 4322
	private TimeSince lastNudge;
}
