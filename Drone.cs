using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class Drone : RemoteControlEntity, IRemoteControllableClientCallbacks, IRemoteControllable
{
	// Token: 0x170001EB RID: 491
	// (get) Token: 0x060015E5 RID: 5605 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RequiresMouse
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x060015E6 RID: 5606 RVA: 0x000ABE10 File Offset: 0x000AA010
	public override float MaxRange
	{
		get
		{
			return global::Drone.maxControlRange;
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x000ABE17 File Offset: 0x000AA017
	public override void Spawn()
	{
		base.Spawn();
		this.isGrounded = true;
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x060015E8 RID: 5608 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool CanAcceptInput
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x000ABE28 File Offset: 0x000AA028
	public override void StopControl(CameraViewerId viewerID)
	{
		if (viewerID == base.ControllingViewerId)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
			this.pitch = 0f;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		base.StopControl(viewerID);
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x000ABE90 File Offset: 0x000AA090
	public override void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		if (viewerID != base.ControllingViewerId)
		{
			return;
		}
		this.currentInput.Reset();
		int num = (inputState.IsDown(BUTTON.FORWARD) ? 1 : 0) + (inputState.IsDown(BUTTON.BACKWARD) ? (-1) : 0);
		int num2 = (inputState.IsDown(BUTTON.RIGHT) ? 1 : 0) + (inputState.IsDown(BUTTON.LEFT) ? (-1) : 0);
		this.currentInput.movement = new Vector3((float)num2, 0f, (float)num).normalized;
		this.currentInput.throttle = (float)((inputState.IsDown(BUTTON.SPRINT) ? 1 : 0) + (inputState.IsDown(BUTTON.DUCK) ? (-1) : 0));
		this.currentInput.yaw = inputState.current.mouseDelta.x;
		this.currentInput.pitch = inputState.current.mouseDelta.y;
		this.lastInputTime = Time.time;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = this.currentInput.throttle > 0f;
		if (flag3 != base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved1, flag3, false, false);
			flag = true;
		}
		float num3 = this.pitch;
		this.pitch += this.currentInput.pitch * this.pitchSensitivity;
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMin, this.pitchMax);
		if (!Mathf.Approximately(this.pitch, num3))
		{
			flag2 = true;
		}
		if (flag2)
		{
			base.SendNetworkUpdateImmediate(false);
			return;
		}
		if (flag)
		{
			base.SendNetworkUpdate_Flags();
		}
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x000AC03C File Offset: 0x000AA23C
	protected virtual void Update_Server()
	{
		if (!base.isServer || this.IsDead())
		{
			return;
		}
		if (base.IsBeingControlled || this.targetPosition == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float height = TerrainMeta.HeightMap.GetHeight(position);
		Vector3 vector = this.targetPosition.Value - this.body.velocity * 0.5f;
		if (this.keepAboveTerrain)
		{
			vector.y = Mathf.Max(vector.y, height + 1f);
		}
		Vector2 vector2 = vector.XZ2D();
		Vector2 vector3 = position.XZ2D();
		Vector3 vector4;
		float num;
		(vector2 - vector3).XZ3D().ToDirectionAndMagnitude(out vector4, out num);
		this.currentInput.Reset();
		this.lastInputTime = Time.time;
		if (position.y - height > 1f)
		{
			float num2 = Mathf.Clamp01(num);
			this.currentInput.movement = base.transform.InverseTransformVector(vector4) * num2;
			if (num > 0.5f)
			{
				float y = base.transform.rotation.eulerAngles.y;
				float y2 = Quaternion.FromToRotation(Vector3.forward, vector4).eulerAngles.y;
				this.currentInput.yaw = Mathf.Clamp(Mathf.LerpAngle(y, y2, Time.deltaTime) - y, -2f, 2f);
			}
		}
		this.currentInput.throttle = Mathf.Clamp(vector.y - position.y, -1f, 1f);
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x000AC1D4 File Offset: 0x000AA3D4
	public void FixedUpdate()
	{
		if (!base.isServer || this.IsDead())
		{
			return;
		}
		float num = this.WaterFactor();
		if (this.killInWater && num > 0f)
		{
			if (num > 0.99f)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			return;
		}
		if ((!base.IsBeingControlled && this.targetPosition == null) || (this.isGrounded && this.currentInput.throttle <= 0f))
		{
			if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
				base.SendNetworkUpdate_Flags();
			}
			return;
		}
		if (this.playerCheckRadius > 0f && this.lastPlayerCheck > (double)this.playerCheckInterval)
		{
			this.lastPlayerCheck = 0.0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(base.transform.position, this.playerCheckRadius, list, 131072, QueryTriggerInteraction.Collide);
			if (list.Count > 0)
			{
				this.lastCollision = TimeEx.currentTimestamp;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}
		double currentTimestamp = TimeEx.currentTimestamp;
		object obj = this.lastCollision > 0.0 && currentTimestamp - this.lastCollision < (double)this.collisionDisableTime;
		if (this.enableGrounding)
		{
			if (this.lastGroundCheck >= this.groundCheckInterval)
			{
				this.lastGroundCheck = 0f;
				RaycastHit raycastHit;
				bool flag = this.body.SweepTest(Vector3.down, out raycastHit, this.groundTraceDist, QueryTriggerInteraction.Ignore);
				if (!flag && this.isGrounded)
				{
					this.lastPlayerCheck = (double)this.playerCheckInterval;
				}
				this.isGrounded = flag;
			}
		}
		else
		{
			this.isGrounded = false;
		}
		Vector3 vector = base.transform.TransformDirection(this.currentInput.movement);
		Vector3 vector2;
		float num2;
		this.body.velocity.WithY(0f).ToDirectionAndMagnitude(out vector2, out num2);
		float num3 = Mathf.Clamp01(num2 / this.leanMaxVelocity);
		Vector3 vector3 = (Mathf.Approximately(vector.sqrMagnitude, 0f) ? (-num3 * vector2) : vector);
		Vector3 normalized = (Vector3.up + vector3 * this.leanWeight * num3).normalized;
		Vector3 up = base.transform.up;
		float num4 = Mathf.Max(Vector3.Dot(normalized, up), 0f);
		object obj2 = obj;
		if (obj2 == null || this.isGrounded)
		{
			Vector3 vector4 = ((this.isGrounded && this.currentInput.throttle <= 0f) ? Vector3.zero : (-1f * base.transform.up * Physics.gravity.y));
			Vector3 vector5 = (this.isGrounded ? Vector3.zero : (vector * ((global::Drone.movementSpeedOverride > 0f) ? global::Drone.movementSpeedOverride : this.movementAcceleration)));
			Vector3 vector6 = base.transform.up * this.currentInput.throttle * ((global::Drone.altitudeSpeedOverride > 0f) ? global::Drone.altitudeSpeedOverride : this.altitudeAcceleration);
			Vector3 vector7 = vector4 + vector5 + vector6;
			this.body.AddForce(vector7 * num4, ForceMode.Acceleration);
		}
		if (obj2 == null && !this.isGrounded)
		{
			Vector3 vector8 = base.transform.TransformVector(0f, this.currentInput.yaw * this.yawSpeed, 0f);
			Vector3 vector9 = Vector3.Cross(Quaternion.Euler(this.body.angularVelocity * this.uprightPrediction) * up, normalized) * this.uprightSpeed;
			float num5 = ((num4 < this.uprightDot) ? 0f : num4);
			Vector3 vector10 = vector8 * num4 + vector9 * num5;
			this.body.AddTorque(vector10 * num4, ForceMode.Acceleration);
		}
		bool flag2 = obj2 == 0;
		if (flag2 != base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, flag2, false, false);
			base.SendNetworkUpdate_Flags();
		}
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x000AC5EC File Offset: 0x000AA7EC
	public void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			this.lastCollision = TimeEx.currentTimestamp;
			float magnitude = collision.relativeVelocity.magnitude;
			if (magnitude > this.hurtVelocityThreshold)
			{
				base.Hurt(Mathf.Pow(magnitude, this.hurtDamagePower), DamageType.Fall, null, false);
			}
		}
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x000AC63A File Offset: 0x000AA83A
	public void OnCollisionStay()
	{
		if (base.isServer)
		{
			this.lastCollision = TimeEx.currentTimestamp;
		}
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x000AC64F File Offset: 0x000AA84F
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (base.isServer && this.disableWhenHurt && info.damageTypes.GetMajorityDamageType() != DamageType.Fall && UnityEngine.Random.value < this.disableWhenHurtChance)
		{
			this.lastCollision = TimeEx.currentTimestamp;
		}
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x060015F1 RID: 5617 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000AC68F File Offset: 0x000AA88F
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.body == null)
		{
			return Vector3.zero;
		}
		return this.body.velocity;
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x000AC6B0 File Offset: 0x000AA8B0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.drone = Pool.Get<ProtoBuf.Drone>();
			info.msg.drone.pitch = this.pitch;
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x000AC6E7 File Offset: 0x000AA8E7
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.drone != null)
		{
			this.pitch = info.msg.drone.pitch;
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x000AC714 File Offset: 0x000AA914
	public virtual void Update()
	{
		this.Update_Server();
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			Vector3 eulerAngles = this.viewEyes.localRotation.eulerAngles;
			eulerAngles.x = Mathf.LerpAngle(eulerAngles.x, this.pitch, 0.1f);
			this.viewEyes.localRotation = Quaternion.Euler(eulerAngles);
		}
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x000AC776 File Offset: 0x000AA976
	protected override bool CanChangeID(global::BasePlayer player)
	{
		return player != null && base.OwnerID == player.userID && !base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000AC79F File Offset: 0x000AA99F
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && !base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x000AC7BA File Offset: 0x000AA9BA
	public override void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		if (player != null && player.userID == base.OwnerID)
		{
			createdItem.text = base.GetIdentifier();
		}
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x000AC7E8 File Offset: 0x000AA9E8
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		base.transform.position += base.transform.up * this.deployYOffset;
		if (this.body != null)
		{
			this.body.velocity = Vector3.zero;
			this.body.angularVelocity = Vector3.zero;
		}
		if (fromItem != null && !string.IsNullOrEmpty(fromItem.text) && global::ComputerStation.IsValidIdentifier(fromItem.text))
		{
			base.UpdateIdentifier(fromItem.text, false);
		}
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x000AC882 File Offset: 0x000AAA82
	public override float MaxVelocity()
	{
		return 30f;
	}

	// Token: 0x04000E2F RID: 3631
	[ReplicatedVar(Help = "How far drones can be flown away from the controlling computer station", ShowInAdminUI = true, Default = "250")]
	public static float maxControlRange = 500f;

	// Token: 0x04000E30 RID: 3632
	[ServerVar(Help = "If greater than zero, overrides the drone's planar movement speed")]
	public static float movementSpeedOverride = 0f;

	// Token: 0x04000E31 RID: 3633
	[ServerVar(Help = "If greater than zero, overrides the drone's vertical movement speed")]
	public static float altitudeSpeedOverride = 0f;

	// Token: 0x04000E32 RID: 3634
	[ClientVar(ClientAdmin = true)]
	public static float windTimeDivisor = 10f;

	// Token: 0x04000E33 RID: 3635
	[ClientVar(ClientAdmin = true)]
	public static float windPositionDivisor = 100f;

	// Token: 0x04000E34 RID: 3636
	[ClientVar(ClientAdmin = true)]
	public static float windPositionScale = 1f;

	// Token: 0x04000E35 RID: 3637
	[ClientVar(ClientAdmin = true)]
	public static float windRotationMultiplier = 45f;

	// Token: 0x04000E36 RID: 3638
	[ClientVar(ClientAdmin = true)]
	public static float windLerpSpeed = 0.1f;

	// Token: 0x04000E37 RID: 3639
	private const global::BaseEntity.Flags Flag_ThrottleUp = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000E38 RID: 3640
	private const global::BaseEntity.Flags Flag_Flying = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000E39 RID: 3641
	[Header("Drone")]
	public Rigidbody body;

	// Token: 0x04000E3A RID: 3642
	public Transform modelRoot;

	// Token: 0x04000E3B RID: 3643
	public bool killInWater = true;

	// Token: 0x04000E3C RID: 3644
	public bool enableGrounding = true;

	// Token: 0x04000E3D RID: 3645
	public bool keepAboveTerrain = true;

	// Token: 0x04000E3E RID: 3646
	public float groundTraceDist = 0.1f;

	// Token: 0x04000E3F RID: 3647
	public float groundCheckInterval = 0.05f;

	// Token: 0x04000E40 RID: 3648
	public float altitudeAcceleration = 10f;

	// Token: 0x04000E41 RID: 3649
	public float movementAcceleration = 10f;

	// Token: 0x04000E42 RID: 3650
	public float yawSpeed = 2f;

	// Token: 0x04000E43 RID: 3651
	public float uprightSpeed = 2f;

	// Token: 0x04000E44 RID: 3652
	public float uprightPrediction = 0.15f;

	// Token: 0x04000E45 RID: 3653
	public float uprightDot = 0.5f;

	// Token: 0x04000E46 RID: 3654
	public float leanWeight = 0.1f;

	// Token: 0x04000E47 RID: 3655
	public float leanMaxVelocity = 5f;

	// Token: 0x04000E48 RID: 3656
	public float hurtVelocityThreshold = 3f;

	// Token: 0x04000E49 RID: 3657
	public float hurtDamagePower = 3f;

	// Token: 0x04000E4A RID: 3658
	public float collisionDisableTime = 0.25f;

	// Token: 0x04000E4B RID: 3659
	public float pitchMin = -60f;

	// Token: 0x04000E4C RID: 3660
	public float pitchMax = 60f;

	// Token: 0x04000E4D RID: 3661
	public float pitchSensitivity = -5f;

	// Token: 0x04000E4E RID: 3662
	public bool disableWhenHurt;

	// Token: 0x04000E4F RID: 3663
	[Range(0f, 1f)]
	public float disableWhenHurtChance = 0.25f;

	// Token: 0x04000E50 RID: 3664
	public float playerCheckInterval = 0.1f;

	// Token: 0x04000E51 RID: 3665
	public float playerCheckRadius;

	// Token: 0x04000E52 RID: 3666
	public float deployYOffset = 0.1f;

	// Token: 0x04000E53 RID: 3667
	[Header("Sound")]
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x04000E54 RID: 3668
	public SoundDefinition movementStartSoundDef;

	// Token: 0x04000E55 RID: 3669
	public SoundDefinition movementStopSoundDef;

	// Token: 0x04000E56 RID: 3670
	public AnimationCurve movementLoopPitchCurve;

	// Token: 0x04000E57 RID: 3671
	public float movementSpeedReference = 50f;

	// Token: 0x04000E58 RID: 3672
	[Header("Animation")]
	public float propellerMaxSpeed = 1000f;

	// Token: 0x04000E59 RID: 3673
	public float propellerAcceleration = 3f;

	// Token: 0x04000E5A RID: 3674
	public Transform propellerA;

	// Token: 0x04000E5B RID: 3675
	public Transform propellerB;

	// Token: 0x04000E5C RID: 3676
	public Transform propellerC;

	// Token: 0x04000E5D RID: 3677
	public Transform propellerD;

	// Token: 0x04000E5E RID: 3678
	private float pitch;

	// Token: 0x04000E5F RID: 3679
	protected Vector3? targetPosition;

	// Token: 0x04000E60 RID: 3680
	private global::Drone.DroneInputState currentInput;

	// Token: 0x04000E61 RID: 3681
	private float lastInputTime;

	// Token: 0x04000E62 RID: 3682
	private double lastCollision = -1000.0;

	// Token: 0x04000E63 RID: 3683
	private TimeSince lastGroundCheck;

	// Token: 0x04000E64 RID: 3684
	private bool isGrounded;

	// Token: 0x04000E65 RID: 3685
	private RealTimeSinceEx lastPlayerCheck;

	// Token: 0x02000C31 RID: 3121
	private struct DroneInputState
	{
		// Token: 0x06004E4F RID: 20047 RVA: 0x001A2792 File Offset: 0x001A0992
		public void Reset()
		{
			this.movement = Vector3.zero;
			this.pitch = 0f;
			this.yaw = 0f;
		}

		// Token: 0x040042DD RID: 17117
		public Vector3 movement;

		// Token: 0x040042DE RID: 17118
		public float throttle;

		// Token: 0x040042DF RID: 17119
		public float pitch;

		// Token: 0x040042E0 RID: 17120
		public float yaw;
	}
}
