using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class WaterInflatable : BaseMountable, IPoolVehicle, INotifyTrigger
{
	// Token: 0x06001509 RID: 5385 RVA: 0x000A5CA4 File Offset: 0x000A3EA4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterInflatable.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x000A5CE4 File Offset: 0x000A3EE4
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.prevSleeping = false;
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x000A5D0C File Offset: 0x000A3F0C
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		if (deployedBy != null)
		{
			Vector3 estimatedVelocity = deployedBy.estimatedVelocity;
			float num = Vector3.Dot(base.transform.forward, estimatedVelocity.normalized);
			Vector3 vector = Vector3.Lerp(Vector3.zero, estimatedVelocity, Mathf.Clamp(num, 0f, 1f));
			vector *= this.inheritVelocityMultiplier;
			this.rigidBody.AddForce(vector, ForceMode.VelocityChange);
		}
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x000A5D80 File Offset: 0x000A3F80
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		bool flag = this.rigidBody.IsSleeping();
		if (this.prevSleeping && !flag && this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
		this.prevSleeping = flag;
		if (this.rigidBody.velocity.magnitude > this.maxSpeed)
		{
			this.rigidBody.velocity = Vector3.ClampMagnitude(this.rigidBody.velocity, this.maxSpeed);
		}
		if (this.AnyMounted() && this.headSpaceCheckPosition != null)
		{
			Vector3 position = base.transform.position;
			if (this.forceClippingCheck || Vector3.Distance(position, this.lastClipCheckPosition) > this.headSpaceCheckRadius * 0.5f)
			{
				this.forceClippingCheck = false;
				this.lastClipCheckPosition = position;
				if (GamePhysics.CheckSphere(this.headSpaceCheckPosition.position, this.headSpaceCheckRadius, 1218511105, QueryTriggerInteraction.UseGlobal))
				{
					this.DismountAllPlayers();
				}
			}
		}
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x000A5E7C File Offset: 0x000A407C
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		this.lastPos = base.transform.position;
		this.forceClippingCheck = true;
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x000A5E9C File Offset: 0x000A409C
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.1f)
		{
			this.DismountAllPlayers();
			return;
		}
		if (this.lastPaddle < this.maxPaddleFrequency)
		{
			return;
		}
		if (this.buoyancy != null && this.IsOutOfWaterServer())
		{
			return;
		}
		if (player.GetHeldEntity() == null)
		{
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				if (this.rigidBody.velocity.magnitude < this.maxSpeed)
				{
					this.rigidBody.AddForce(base.transform.forward * this.forwardPushForce, ForceMode.Impulse);
				}
				this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, base.transform.forward, 0.5f);
				this.lastPaddle = 0f;
				base.ClientRPC<int>(null, "OnPaddled", 0);
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				this.rigidBody.AddForce(-base.transform.forward * this.rearPushForce, ForceMode.Impulse);
				this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, -base.transform.forward, 0.5f);
				this.lastPaddle = 0f;
				base.ClientRPC<int>(null, "OnPaddled", 3);
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				this.PaddleTurn(WaterInflatable.PaddleDirection.Left);
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				this.PaddleTurn(WaterInflatable.PaddleDirection.Right);
			}
		}
		if (this.inPoolCheck > 2f)
		{
			this.isInPool = base.IsInWaterVolume(base.transform.position);
			this.inPoolCheck = 0f;
		}
		if (this.additiveDownhillVelocity > 0f && !this.isInPool)
		{
			Vector3 vector = base.transform.TransformPoint(Vector3.forward);
			Vector3 position = base.transform.position;
			if (vector.y < position.y)
			{
				float num = this.additiveDownhillVelocity * (position.y - vector.y);
				this.rigidBody.AddForce(num * Time.fixedDeltaTime * base.transform.forward, ForceMode.Acceleration);
			}
			Vector3 velocity = this.rigidBody.velocity;
			this.rigidBody.velocity = Vector3.Lerp(velocity, base.transform.forward * velocity.magnitude, 0.4f);
		}
		if (this.driftTowardsIsland && this.landFacingCheck > 2f && !this.isInPool)
		{
			this.isFacingLand = false;
			this.landFacingCheck = 0f;
			Vector3 position2 = base.transform.position;
			if (!WaterResource.IsFreshWater(position2))
			{
				int num2 = 5;
				Vector3 forward = base.transform.forward;
				forward.y = 0f;
				for (int i = 1; i <= num2; i++)
				{
					int num3 = 128;
					if (!TerrainMeta.TopologyMap.GetTopology(position2 + (float)i * 15f * forward, num3))
					{
						this.isFacingLand = true;
						break;
					}
				}
			}
		}
		if (this.driftTowardsIsland && this.isFacingLand && !this.isInPool)
		{
			this.landPushAcceleration = Mathf.Clamp(this.landPushAcceleration + Time.deltaTime, 0f, 3f);
			this.rigidBody.AddForce(base.transform.forward * (Time.deltaTime * this.landPushAcceleration), ForceMode.VelocityChange);
		}
		else
		{
			this.landPushAcceleration = 0f;
		}
		this.lastPos = base.transform.position;
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x000A626C File Offset: 0x000A446C
	private void PaddleTurn(WaterInflatable.PaddleDirection direction)
	{
		if (direction == WaterInflatable.PaddleDirection.Forward || direction == WaterInflatable.PaddleDirection.Back)
		{
			return;
		}
		this.rigidBody.AddRelativeTorque(this.rotationForce * ((direction == WaterInflatable.PaddleDirection.Left) ? (-Vector3.up) : Vector3.up), ForceMode.Impulse);
		this.lastPaddle = 0f;
		base.ClientRPC<int>(null, "OnPaddled", (int)direction);
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x00029EBC File Offset: 0x000280BC
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		return 0f;
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x000A62CC File Offset: 0x000A44CC
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		BaseVehicle baseVehicle;
		if ((baseVehicle = hitEntity as BaseVehicle) != null && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x000A6303 File Offset: 0x000A4503
	private bool IsOutOfWaterServer()
	{
		return this.buoyancy.timeOutOfWater > 0.2f;
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x00029C50 File Offset: 0x00027E50
	public void OnPoolDestroyed()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x000A6318 File Offset: 0x000A4518
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x06001515 RID: 5397 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsSummerDlcVehicle
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x000A6374 File Offset: 0x000A4574
	public void OnObjects(TriggerNotify trigger)
	{
		if (base.isClient)
		{
			return;
		}
		using (HashSet<BaseEntity>.Enumerator enumerator = trigger.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseVehicle baseVehicle;
				if ((baseVehicle = enumerator.Current as BaseVehicle) != null && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
			}
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEmpty()
	{
	}

	// Token: 0x04000D23 RID: 3363
	public Rigidbody rigidBody;

	// Token: 0x04000D24 RID: 3364
	public Transform centerOfMass;

	// Token: 0x04000D25 RID: 3365
	public float forwardPushForce = 5f;

	// Token: 0x04000D26 RID: 3366
	public float rearPushForce = 5f;

	// Token: 0x04000D27 RID: 3367
	public float rotationForce = 5f;

	// Token: 0x04000D28 RID: 3368
	public float maxSpeed = 3f;

	// Token: 0x04000D29 RID: 3369
	public float maxPaddleFrequency = 0.5f;

	// Token: 0x04000D2A RID: 3370
	public SoundDefinition paddleSfx;

	// Token: 0x04000D2B RID: 3371
	public SoundDefinition smallPlayerMovementSound;

	// Token: 0x04000D2C RID: 3372
	public SoundDefinition largePlayerMovementSound;

	// Token: 0x04000D2D RID: 3373
	public BlendedSoundLoops waterLoops;

	// Token: 0x04000D2E RID: 3374
	public float waterSoundSpeedDivisor = 1f;

	// Token: 0x04000D2F RID: 3375
	public float additiveDownhillVelocity;

	// Token: 0x04000D30 RID: 3376
	public GameObjectRef handSplashForwardEffect;

	// Token: 0x04000D31 RID: 3377
	public GameObjectRef handSplashBackEffect;

	// Token: 0x04000D32 RID: 3378
	public GameObjectRef footSplashEffect;

	// Token: 0x04000D33 RID: 3379
	public float animationLerpSpeed = 1f;

	// Token: 0x04000D34 RID: 3380
	public Transform smoothedEyePosition;

	// Token: 0x04000D35 RID: 3381
	public float smoothedEyeSpeed = 1f;

	// Token: 0x04000D36 RID: 3382
	public Buoyancy buoyancy;

	// Token: 0x04000D37 RID: 3383
	public bool driftTowardsIsland;

	// Token: 0x04000D38 RID: 3384
	public GameObjectRef mountEffect;

	// Token: 0x04000D39 RID: 3385
	[Range(0f, 1f)]
	public float handSplashOffset = 1f;

	// Token: 0x04000D3A RID: 3386
	public float velocitySplashMultiplier = 4f;

	// Token: 0x04000D3B RID: 3387
	public Vector3 modifyEyeOffset = Vector3.zero;

	// Token: 0x04000D3C RID: 3388
	[Range(0f, 1f)]
	public float inheritVelocityMultiplier;

	// Token: 0x04000D3D RID: 3389
	private TimeSince lastPaddle;

	// Token: 0x04000D3E RID: 3390
	public ParticleSystem[] movingParticleSystems;

	// Token: 0x04000D3F RID: 3391
	public float movingParticlesThreshold = 0.0005f;

	// Token: 0x04000D40 RID: 3392
	public Transform headSpaceCheckPosition;

	// Token: 0x04000D41 RID: 3393
	public float headSpaceCheckRadius = 0.4f;

	// Token: 0x04000D42 RID: 3394
	private TimeSince landFacingCheck;

	// Token: 0x04000D43 RID: 3395
	private bool isFacingLand;

	// Token: 0x04000D44 RID: 3396
	private float landPushAcceleration;

	// Token: 0x04000D45 RID: 3397
	private TimeSince inPoolCheck;

	// Token: 0x04000D46 RID: 3398
	private bool isInPool;

	// Token: 0x04000D47 RID: 3399
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x04000D48 RID: 3400
	private Vector3 lastClipCheckPosition;

	// Token: 0x04000D49 RID: 3401
	private bool forceClippingCheck;

	// Token: 0x04000D4A RID: 3402
	private bool prevSleeping;

	// Token: 0x02000C2B RID: 3115
	private enum PaddleDirection
	{
		// Token: 0x040042B9 RID: 17081
		Forward,
		// Token: 0x040042BA RID: 17082
		Left,
		// Token: 0x040042BB RID: 17083
		Right,
		// Token: 0x040042BC RID: 17084
		Back
	}
}
