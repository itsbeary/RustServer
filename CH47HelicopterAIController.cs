using System;
using Rust;
using UnityEngine;

// Token: 0x02000486 RID: 1158
public class CH47HelicopterAIController : CH47Helicopter
{
	// Token: 0x0600264A RID: 9802 RVA: 0x000F1A8C File Offset: 0x000EFC8C
	public void DropCrate()
	{
		if (this.numCrates <= 0)
		{
			return;
		}
		Vector3 vector = base.transform.position + Vector3.down * 5f;
		Quaternion quaternion = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.lockedCratePrefab.resourcePath, vector, quaternion, true);
		if (baseEntity)
		{
			baseEntity.SendMessage("SetWasDropped");
			baseEntity.Spawn();
		}
		this.numCrates--;
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x000F1B22 File Offset: 0x000EFD22
	public bool OutOfCrates()
	{
		return this.numCrates <= 0;
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000F1B30 File Offset: 0x000EFD30
	public bool CanDropCrate()
	{
		return this.numCrates > 0;
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsDropDoorOpen()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x00070BE7 File Offset: 0x0006EDE7
	public void SetDropDoorOpen(bool open)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, open, false, true);
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x000F1B3B File Offset: 0x000EFD3B
	public bool ShouldLand()
	{
		return this.shouldLand;
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000F1B43 File Offset: 0x000EFD43
	public void SetLandingTarget(Vector3 target)
	{
		this.shouldLand = true;
		this.landingTarget = target;
		this.numCrates = 0;
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000F1B5A File Offset: 0x000EFD5A
	public void ClearLandingTarget()
	{
		this.shouldLand = false;
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000F1B64 File Offset: 0x000EFD64
	public void TriggeredEventSpawn()
	{
		float x = TerrainMeta.Size.x;
		float num = 30f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * 1f;
		vector.y = num;
		base.transform.position = vector;
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000F1BC7 File Offset: 0x000EFDC7
	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (!player.IsNpc && !player.IsAdmin)
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000F1BE2 File Offset: 0x000EFDE2
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.SpawnScientists), 0.25f);
		this.SetMoveTarget(base.transform.position);
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000F1C14 File Offset: 0x000EFE14
	public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(prefabPath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000F1C4C File Offset: 0x000EFE4C
	public void SpawnPassenger(Vector3 spawnPos)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.dismountablePrefab.resourcePath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000F1C8C File Offset: 0x000EFE8C
	public void SpawnScientist(Vector3 spawnPos)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.scientistPrefab.resourcePath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
		component.Brain.SetEnabled(false);
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000F1CD8 File Offset: 0x000EFED8
	public void SpawnScientists()
	{
		if (this.shouldLand)
		{
			float dropoffScale = CH47LandingZone.GetClosest(this.landingTarget).dropoffScale;
			int num = Mathf.FloorToInt((float)(this.mountPoints.Count - 2) * dropoffScale);
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = base.transform.position + base.transform.forward * 10f;
				this.SpawnPassenger(vector, this.dismountablePrefab.resourcePath);
			}
			for (int j = 0; j < 1; j++)
			{
				Vector3 vector2 = base.transform.position - base.transform.forward * 15f;
				this.SpawnPassenger(vector2);
			}
			return;
		}
		for (int k = 0; k < 4; k++)
		{
			Vector3 vector3 = base.transform.position + base.transform.forward * 10f;
			this.SpawnScientist(vector3);
		}
		for (int l = 0; l < 1; l++)
		{
			Vector3 vector4 = base.transform.position - base.transform.forward * 15f;
			this.SpawnScientist(vector4);
		}
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000F1E19 File Offset: 0x000F0019
	public void EnableFacingOverride(bool enabled)
	{
		this.aimDirOverride = enabled;
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000F1E22 File Offset: 0x000F0022
	public void SetMoveTarget(Vector3 position)
	{
		this._moveTarget = position;
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000F1E2B File Offset: 0x000F002B
	public Vector3 GetMoveTarget()
	{
		return this._moveTarget;
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000F1E33 File Offset: 0x000F0033
	public void SetAimDirection(Vector3 dir)
	{
		this._aimDirection = dir;
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000F1E3C File Offset: 0x000F003C
	public Vector3 GetAimDirectionOverride()
	{
		return this._aimDirection;
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000F1E44 File Offset: 0x000F0044
	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		this.InitiateAnger();
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000F1E4C File Offset: 0x000F004C
	public void CancelAnger()
	{
		if (base.SecondsSinceAttacked > 120f)
		{
			this.UnHostile();
			base.CancelInvoke(new Action(this.UnHostile));
		}
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000F1E74 File Offset: 0x000F0074
	public void InitiateAnger()
	{
		base.CancelInvoke(new Action(this.UnHostile));
		base.Invoke(new Action(this.UnHostile), 120f);
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					ScientistNPC scientistNPC = mounted as ScientistNPC;
					if (scientistNPC != null)
					{
						scientistNPC.Brain.SetEnabled(true);
					}
				}
			}
		}
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000F1F28 File Offset: 0x000F0128
	public void UnHostile()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					ScientistNPC scientistNPC = mounted as ScientistNPC;
					if (scientistNPC != null)
					{
						scientistNPC.Brain.SetEnabled(false);
					}
				}
			}
		}
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000F1FB4 File Offset: 0x000F01B4
	public override void OnKilled(HitInfo info)
	{
		if (!this.OutOfCrates())
		{
			this.DropCrate();
		}
		base.OnKilled(info);
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000F1FCC File Offset: 0x000F01CC
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.InitiateAnger();
		base.SetFlag(BaseEntity.Flags.Reserved7, base.healthFraction <= 0.8f, false, true);
		base.SetFlag(BaseEntity.Flags.OnFire, base.healthFraction <= 0.33f, false, true);
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000F201C File Offset: 0x000F021C
	public void DelayedKill()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
				{
					mounted.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000F20C0 File Offset: 0x000F02C0
	public override void DismountAllPlayers()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, false);
				}
			}
		}
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000F2140 File Offset: 0x000F0340
	public void SetAltitudeProtection(bool on)
	{
		this.altitudeProtection = on;
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000F214C File Offset: 0x000F034C
	public void CalculateDesiredAltitude()
	{
		this.CalculateOverrideAltitude();
		if (this.altOverride > this.currentDesiredAltitude)
		{
			this.currentDesiredAltitude = this.altOverride;
			return;
		}
		this.currentDesiredAltitude = Mathf.MoveTowards(this.currentDesiredAltitude, this.altOverride, Time.fixedDeltaTime * 5f);
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000F219D File Offset: 0x000F039D
	public void SetMinHoverHeight(float newHeight)
	{
		this.hoverHeight = newHeight;
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000F21A8 File Offset: 0x000F03A8
	public float CalculateOverrideAltitude()
	{
		if (Time.frameCount == this.lastAltitudeCheckFrame)
		{
			return this.altOverride;
		}
		this.lastAltitudeCheckFrame = Time.frameCount;
		float y = this.GetMoveTarget().y;
		float num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(this.GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(this.GetMoveTarget()));
		float num2 = Mathf.Max(y, num + this.hoverHeight);
		if (this.altitudeProtection)
		{
			Vector3 vector = ((this.rigidBody.velocity.magnitude < 0.1f) ? base.transform.forward : this.rigidBody.velocity.normalized);
			Vector3 normalized = (Vector3.Cross(Vector3.Cross(base.transform.up, vector), Vector3.up) + Vector3.down * 0.3f).normalized;
			RaycastHit raycastHit;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(base.transform.position - normalized * 20f, 20f, normalized, out raycastHit, 75f, 1218511105) && Physics.SphereCast(raycastHit.point + Vector3.up * 200f, 20f, Vector3.down, out raycastHit2, 200f, 1218511105))
			{
				num2 = raycastHit2.point.y + this.hoverHeight;
			}
		}
		this.altOverride = num2;
		return this.altOverride;
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000F2324 File Offset: 0x000F0524
	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		Vector3 moveTarget = this.GetMoveTarget();
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
		float num = -Vector3.Dot(Vector3.up, base.transform.right);
		float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
		float num3 = Vector3Ex.Distance2D(base.transform.position, moveTarget);
		float y = base.transform.position.y;
		float num4 = this.currentDesiredAltitude;
		(base.transform.position + base.transform.forward * 10f).y = num4;
		Vector3 vector3 = Vector3Ex.Direction2D(moveTarget, base.transform.position);
		float num5 = -Vector3.Dot(vector3, vector2);
		float num6 = Vector3.Dot(vector3, vector);
		float num7 = Mathf.InverseLerp(0f, 25f, num3);
		if (num6 > 0f)
		{
			float num8 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, num2);
			this.currentInputState.pitch = 1f * num6 * num8 * num7;
		}
		else
		{
			float num9 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, num2);
			this.currentInputState.pitch = 1f * num6 * num9 * num7;
		}
		if (num5 > 0f)
		{
			float num10 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, num);
			this.currentInputState.roll = 1f * num5 * num10 * num7;
		}
		else
		{
			float num11 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, num);
			this.currentInputState.roll = 1f * num5 * num11 * num7;
		}
		float num12 = Mathf.Abs(num4 - y);
		float num13 = 1f - Mathf.InverseLerp(10f, 30f, num12);
		this.currentInputState.pitch *= num13;
		this.currentInputState.roll *= num13;
		float num14 = this.maxTiltAngle;
		float num15 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.pitch) * num14, num14 + Mathf.Abs(this.currentInputState.pitch) * num14, Mathf.Abs(num2));
		this.currentInputState.pitch += num15 * ((num2 < 0f) ? (-1f) : 1f);
		float num16 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.roll) * num14, num14 + Mathf.Abs(this.currentInputState.roll) * num14, Mathf.Abs(num));
		this.currentInputState.roll += num16 * ((num < 0f) ? (-1f) : 1f);
		if (this.aimDirOverride || num3 > 30f)
		{
			Vector3 vector4 = (this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position));
			Vector3 vector5 = (this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position));
			float num17 = Vector3.Dot(vector2, vector4);
			float num18 = Vector3.Angle(vector, vector5);
			float num19 = Mathf.InverseLerp(0f, 70f, Mathf.Abs(num18));
			this.currentInputState.yaw = ((num17 > 0f) ? 1f : 0f);
			this.currentInputState.yaw -= ((num17 < 0f) ? 1f : 0f);
			this.currentInputState.yaw *= num19;
		}
		float num20 = Mathf.InverseLerp(5f, 30f, num3);
		this.currentInputState.throttle = num20;
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000F2728 File Offset: 0x000F0928
	public void MaintainAIAltutide()
	{
		ref Vector3 ptr = base.transform.position + this.rigidBody.velocity;
		float num = this.currentDesiredAltitude;
		float y = ptr.y;
		float num2 = Mathf.Abs(num - y);
		bool flag = num > y;
		float num3 = Mathf.InverseLerp(0f, 10f, num2) * this.AiAltitudeForce * (flag ? 1f : (-1f));
		this.rigidBody.AddForce(Vector3.up * num3, ForceMode.Force);
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000F27AC File Offset: 0x000F09AC
	public override void VehicleFixedUpdate()
	{
		this.hoverForceScale = 1f;
		base.VehicleFixedUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight, false, true);
		this.CalculateDesiredAltitude();
		this.MaintainAIAltutide();
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000F27E4 File Offset: 0x000F09E4
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable != null)
				{
					BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
					{
						mounted.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
		}
		base.DestroyShared();
	}

	// Token: 0x04001EB0 RID: 7856
	public GameObjectRef scientistPrefab;

	// Token: 0x04001EB1 RID: 7857
	public GameObjectRef dismountablePrefab;

	// Token: 0x04001EB2 RID: 7858
	public GameObjectRef weakDismountablePrefab;

	// Token: 0x04001EB3 RID: 7859
	public float maxTiltAngle = 0.3f;

	// Token: 0x04001EB4 RID: 7860
	public float AiAltitudeForce = 10000f;

	// Token: 0x04001EB5 RID: 7861
	public GameObjectRef lockedCratePrefab;

	// Token: 0x04001EB6 RID: 7862
	public const BaseEntity.Flags Flag_Damaged = BaseEntity.Flags.Reserved7;

	// Token: 0x04001EB7 RID: 7863
	public const BaseEntity.Flags Flag_NearDeath = BaseEntity.Flags.OnFire;

	// Token: 0x04001EB8 RID: 7864
	public const BaseEntity.Flags Flag_DropDoorOpen = BaseEntity.Flags.Reserved8;

	// Token: 0x04001EB9 RID: 7865
	public GameObject triggerHurt;

	// Token: 0x04001EBA RID: 7866
	public Vector3 landingTarget;

	// Token: 0x04001EBB RID: 7867
	private int numCrates = 1;

	// Token: 0x04001EBC RID: 7868
	private bool shouldLand;

	// Token: 0x04001EBD RID: 7869
	private bool aimDirOverride;

	// Token: 0x04001EBE RID: 7870
	private Vector3 _aimDirection = Vector3.forward;

	// Token: 0x04001EBF RID: 7871
	private Vector3 _moveTarget = Vector3.zero;

	// Token: 0x04001EC0 RID: 7872
	private int lastAltitudeCheckFrame;

	// Token: 0x04001EC1 RID: 7873
	private float altOverride;

	// Token: 0x04001EC2 RID: 7874
	private float currentDesiredAltitude;

	// Token: 0x04001EC3 RID: 7875
	private bool altitudeProtection = true;

	// Token: 0x04001EC4 RID: 7876
	private float hoverHeight = 30f;
}
