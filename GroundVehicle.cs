using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public abstract class GroundVehicle : BaseVehicle, IEngineControllerUser, IEntity, TriggerHurtNotChild.IHurtTriggerUser
{
	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06002603 RID: 9731 RVA: 0x000F0947 File Offset: 0x000EEB47
	// (set) Token: 0x06002604 RID: 9732 RVA: 0x000F094F File Offset: 0x000EEB4F
	public Vector3 Velocity { get; private set; }

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06002605 RID: 9733
	public abstract float DriveWheelVelocity { get; }

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06002606 RID: 9734 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06002607 RID: 9735 RVA: 0x000F0958 File Offset: 0x000EEB58
	public VehicleEngineController<GroundVehicle>.EngineState CurEngineState
	{
		get
		{
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x000F0965 File Offset: 0x000EEB65
	public override void InitShared()
	{
		base.InitShared();
		this.engineController = new VehicleEngineController<GroundVehicle>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, this.waterloggedPoint, BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x000F0996 File Offset: 0x000EEB96
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x000F09B5 File Offset: 0x000EEBB5
	public float GetSpeed()
	{
		if (base.IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(this.Velocity, base.transform.forward);
	}

	// Token: 0x0600260B RID: 9739
	public abstract float GetMaxForwardSpeed();

	// Token: 0x0600260C RID: 9740
	public abstract float GetThrottleInput();

	// Token: 0x0600260D RID: 9741
	public abstract float GetBrakeInput();

	// Token: 0x0600260E RID: 9742 RVA: 0x000F09DB File Offset: 0x000EEBDB
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !pusher.isMounted && !pusher.IsSwimming() && !pusher.IsStandingOnEntity(this, 8192);
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000F0A09 File Offset: 0x000EEC09
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceDragModSet = default(TimeSince);
		this.timeSinceDragModSet = float.MaxValue;
	}

	// Token: 0x06002610 RID: 9744
	public abstract void OnEngineStartFailed();

	// Token: 0x06002611 RID: 9745
	public abstract bool MeetsEngineRequirements();

	// Token: 0x06002612 RID: 9746 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void ServerFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
	}

	// Token: 0x06002613 RID: 9747 RVA: 0x000F0A2D File Offset: 0x000EEC2D
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			this.ProcessCollision(collision);
		}
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000F0A40 File Offset: 0x000EEC40
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (base.IsMovingOrOn)
		{
			this.Velocity = base.GetLocalVelocity();
		}
		else
		{
			this.Velocity = Vector3.zero;
		}
		if (this.LightsAreOn && !this.AnyMounted())
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
		}
		if (Time.time >= this.nextCollisionDamageTime)
		{
			this.nextCollisionDamageTime = Time.time + 0.33f;
			foreach (KeyValuePair<BaseEntity, float> keyValuePair in this.damageSinceLastTick)
			{
				this.DoCollisionDamage(keyValuePair.Key, keyValuePair.Value);
			}
			this.damageSinceLastTick.Clear();
		}
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000F0B10 File Offset: 0x000EED10
	public override void LightToggle(BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000F0B32 File Offset: 0x000EED32
	public float GetDamageMultiplier(BaseEntity ent)
	{
		return Mathf.Abs(this.GetSpeed()) * 1f;
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000F0B48 File Offset: 0x000EED48
	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
		if (base.isClient)
		{
			return;
		}
		if (hurtEntity.IsDestroyed)
		{
			return;
		}
		Vector3 vector = hurtEntity.GetLocalVelocity() - this.Velocity;
		Vector3 vector2 = base.ClosestPoint(hurtEntity.transform.position);
		Vector3 vector3 = hurtEntity.RealisticMass * vector;
		this.rigidBody.AddForceAtPosition(vector3 * 1.25f, vector2, ForceMode.Impulse);
		this.QueueCollisionDamage(this, vector3.magnitude * 0.75f / Time.deltaTime);
		this.SetTempDrag(2.25f, 1f);
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000F0BDC File Offset: 0x000EEDDC
	private float QueueCollisionDamage(BaseEntity hitEntity, float forceMagnitude)
	{
		float num = Mathf.InverseLerp(this.minCollisionDamageForce, this.maxCollisionDamageForce, forceMagnitude);
		if (num > 0f)
		{
			float num2 = Mathf.Lerp(1f, 200f, num) * this.collisionDamageMultiplier;
			float num3;
			if (this.damageSinceLastTick.TryGetValue(hitEntity, out num3))
			{
				if (num3 < num2)
				{
					this.damageSinceLastTick[hitEntity] = num2;
				}
			}
			else
			{
				this.damageSinceLastTick[hitEntity] = num2;
			}
		}
		return num;
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000F0C4D File Offset: 0x000EEE4D
	protected virtual void DoCollisionDamage(BaseEntity hitEntity, float damage)
	{
		base.Hurt(damage, DamageType.Collision, this, false);
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x000F0C5C File Offset: 0x000EEE5C
	private void ProcessCollision(Collision collision)
	{
		if (base.isClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		BaseEntity baseEntity = null;
		if (contact.otherCollider.attachedRigidbody == this.rigidBody)
		{
			baseEntity = contact.otherCollider.ToBaseEntity();
		}
		else if (contact.thisCollider.attachedRigidbody == this.rigidBody)
		{
			baseEntity = contact.thisCollider.ToBaseEntity();
		}
		if (baseEntity != null)
		{
			float num = collision.impulse.magnitude / Time.fixedDeltaTime;
			if (this.QueueCollisionDamage(baseEntity, num) > 0f)
			{
				base.TryShowCollisionFX(collision, this.collisionEffect);
			}
		}
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x000F0D23 File Offset: 0x000EEF23
	public virtual float GetModifiedDrag()
	{
		return (1f - Mathf.InverseLerp(0f, this.dragModDuration, this.timeSinceDragModSet)) * this.dragMod;
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x000F0D4D File Offset: 0x000EEF4D
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x0600261D RID: 9757 RVA: 0x000070BD File Offset: 0x000052BD
	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x000F0D5A File Offset: 0x000EEF5A
	private void SetTempDrag(float drag, float duration)
	{
		this.dragMod = Mathf.Clamp(drag, 0f, 1000f);
		this.timeSinceDragModSet = 0f;
		this.dragModDuration = duration;
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x00007C30 File Offset: 0x00005E30
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x00007C3A File Offset: 0x00005E3A
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x04001E71 RID: 7793
	[Header("GroundVehicle")]
	[SerializeField]
	protected GroundVehicleAudio gvAudio;

	// Token: 0x04001E72 RID: 7794
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04001E73 RID: 7795
	[SerializeField]
	private Transform waterloggedPoint;

	// Token: 0x04001E74 RID: 7796
	[SerializeField]
	private GameObjectRef collisionEffect;

	// Token: 0x04001E75 RID: 7797
	[SerializeField]
	private float engineStartupTime = 0.5f;

	// Token: 0x04001E76 RID: 7798
	[SerializeField]
	private float minCollisionDamageForce = 20000f;

	// Token: 0x04001E77 RID: 7799
	[SerializeField]
	private float maxCollisionDamageForce = 2500000f;

	// Token: 0x04001E78 RID: 7800
	[SerializeField]
	private float collisionDamageMultiplier = 1f;

	// Token: 0x04001E7A RID: 7802
	protected VehicleEngineController<GroundVehicle> engineController;

	// Token: 0x04001E7B RID: 7803
	private Dictionary<BaseEntity, float> damageSinceLastTick = new Dictionary<BaseEntity, float>();

	// Token: 0x04001E7C RID: 7804
	private float nextCollisionDamageTime;

	// Token: 0x04001E7D RID: 7805
	private float dragMod;

	// Token: 0x04001E7E RID: 7806
	private float dragModDuration;

	// Token: 0x04001E7F RID: 7807
	private TimeSince timeSinceDragModSet;
}
