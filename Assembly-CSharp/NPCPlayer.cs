using System;
using System.Collections;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001FC RID: 508
public class NPCPlayer : BasePlayer
{
	// Token: 0x17000234 RID: 564
	// (get) Token: 0x06001A8C RID: 6796 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06001A8D RID: 6797 RVA: 0x000BF336 File Offset: 0x000BD536
	// (set) Token: 0x06001A8E RID: 6798 RVA: 0x000BF33E File Offset: 0x000BD53E
	public virtual bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			bool isDormant = this._isDormant;
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x06001A8F RID: 6799 RVA: 0x0002C198 File Offset: 0x0002A398
	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsLoadBalanced()
	{
		return false;
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x000BF350 File Offset: 0x000BD550
	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		this.spawnPos = this.GetPosition();
		this.randomOffset = UnityEngine.Random.Range(0f, 1f);
		base.ServerInit();
		this.UpdateNetworkGroup();
		this.EquipLoadout(this.loadouts);
		if (!this.IsLoadBalanced())
		{
			base.InvokeRepeating(new Action(this.ServerThink_Internal), 0f, 0.1f);
			this.lastThinkTime = UnityEngine.Time.time;
		}
		base.Invoke(new Action(this.EquipTest), 0.25f);
		this.finalDestination = base.transform.position;
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
			if (!this.LegacyNavigation)
			{
				base.transform.gameObject.GetComponent<BaseNavigator>().Init(this, this.NavAgent);
			}
		}
		base.InvokeRandomized(new Action(this.TickMovement), 1f, this.PositionTickRate, this.PositionTickRate * 0.1f);
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x000BF482 File Offset: 0x000BD682
	public void EquipLoadout(PlayerInventoryProperties[] loads)
	{
		if (loads == null)
		{
			return;
		}
		if (loads.Length == 0)
		{
			return;
		}
		loads[UnityEngine.Random.Range(0, loads.Length)].GiveToPlayer(this);
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x00035610 File Offset: 0x00033810
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		this.ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x000BF4A0 File Offset: 0x000BD6A0
	public void RandomMove()
	{
		float num = 8f;
		Vector2 vector = UnityEngine.Random.insideUnitCircle * num;
		this.SetDestination(this.spawnPos + new Vector3(vector.x, 0f, vector.y));
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x000BF4E6 File Offset: 0x000BD6E6
	public virtual void SetDestination(Vector3 newDestination)
	{
		this.finalDestination = newDestination;
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x000BF4EF File Offset: 0x000BD6EF
	public AttackEntity GetAttackEntity()
	{
		return base.GetHeldEntity() as AttackEntity;
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x000BF4FC File Offset: 0x000BD6FC
	public BaseProjectile GetGun()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return null;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (baseProjectile)
		{
			return baseProjectile;
		}
		return null;
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x000BF534 File Offset: 0x000BD734
	public virtual float AmmoFractionRemaining()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.AmmoFraction();
		}
		return 0f;
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x000BF55C File Offset: 0x000BD75C
	public virtual bool IsReloading()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		return attackEntity && attackEntity.ServerIsReloading();
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x000BF580 File Offset: 0x000BD780
	public virtual void AttemptReload()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity == null)
		{
			return;
		}
		if (attackEntity.CanReload())
		{
			attackEntity.ServerReload();
		}
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x000BF5AC File Offset: 0x000BD7AC
	public virtual bool ShotTest(float targetDist)
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (baseProjectile)
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerReload();
				return false;
			}
			if (baseProjectile.NextAttackTime > UnityEngine.Time.time)
			{
				return false;
			}
		}
		if (Mathf.Approximately(attackEntity.attackLengthMin, -1f))
		{
			attackEntity.ServerUse(this.damageScale, null);
			this.lastGunShotTime = UnityEngine.Time.time;
			return true;
		}
		if (base.IsInvoking(new Action(this.TriggerDown)))
		{
			return true;
		}
		if (UnityEngine.Time.time < this.nextTriggerTime)
		{
			return true;
		}
		base.InvokeRepeating(new Action(this.TriggerDown), 0f, 0.01f);
		if (targetDist <= this.shortRange)
		{
			this.triggerEndTime = UnityEngine.Time.time + UnityEngine.Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax * this.attackLengthMaxShortRangeScale);
		}
		else
		{
			this.triggerEndTime = UnityEngine.Time.time + UnityEngine.Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax);
		}
		this.TriggerDown();
		return true;
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float GetAimConeScale()
	{
		return 1f;
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x000BF6C9 File Offset: 0x000BD8C9
	public void CancelBurst(float delay = 0.2f)
	{
		if (this.triggerEndTime > UnityEngine.Time.time + delay)
		{
			this.triggerEndTime = UnityEngine.Time.time + delay;
		}
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x000BF6E8 File Offset: 0x000BD8E8
	public bool MeleeAttack()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		BaseMelee baseMelee = attackEntity as BaseMelee;
		if (baseMelee == null)
		{
			return false;
		}
		baseMelee.ServerUse(this.damageScale, null);
		return true;
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000BF72C File Offset: 0x000BD92C
	public virtual void TriggerDown()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity != null)
		{
			attackEntity.ServerUse(this.damageScale, null);
		}
		this.lastGunShotTime = UnityEngine.Time.time;
		if (UnityEngine.Time.time > this.triggerEndTime)
		{
			base.CancelInvoke(new Action(this.TriggerDown));
			this.nextTriggerTime = UnityEngine.Time.time + ((attackEntity != null) ? attackEntity.attackSpacing : 1f);
		}
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000BF7A8 File Offset: 0x000BD9A8
	public virtual void EquipWeapon(bool skipDeployDelay = false)
	{
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return;
		}
		Item slot = this.inventory.containerBelt.GetSlot(0);
		if (slot != null)
		{
			base.UpdateActiveItem(this.inventory.containerBelt.GetSlot(0).uid);
			BaseEntity heldEntity = slot.GetHeldEntity();
			if (heldEntity != null)
			{
				AttackEntity component = heldEntity.GetComponent<AttackEntity>();
				if (component != null)
				{
					if (skipDeployDelay)
					{
						component.ResetAttackCooldown();
					}
					component.TopUpAmmo();
				}
			}
		}
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x000BF833 File Offset: 0x000BDA33
	public void EquipTest()
	{
		this.EquipWeapon(true);
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x000BF83C File Offset: 0x000BDA3C
	internal void ServerThink_Internal()
	{
		float num = UnityEngine.Time.time - this.lastThinkTime;
		this.ServerThink(num);
		this.lastThinkTime = UnityEngine.Time.time;
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x000BF868 File Offset: 0x000BDA68
	public virtual void ServerThink(float delta)
	{
		this.TickAi(delta);
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void Resume()
	{
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsNavRunning()
	{
		return false;
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06001AA6 RID: 6822 RVA: 0x000BF871 File Offset: 0x000BDA71
	public virtual bool IsOnNavMeshLink
	{
		get
		{
			return this.IsNavRunning() && this.NavAgent.isOnOffMeshLink;
		}
	}

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06001AA7 RID: 6823 RVA: 0x000BF888 File Offset: 0x000BDA88
	public virtual bool HasPath
	{
		get
		{
			return this.IsNavRunning() && this.NavAgent.hasPath;
		}
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void TickAi(float delta)
	{
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x000BF8A0 File Offset: 0x000BDAA0
	public void TickMovement()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastMovementTickTime;
		this.lastMovementTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.MovementUpdate(num);
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x000BF8CC File Offset: 0x000BDACC
	public override float GetNetworkTime()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastPositionUpdateTime > this.PositionTickRate * 2f)
		{
			return UnityEngine.Time.time;
		}
		return this.lastPositionUpdateTime;
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x000BF8F4 File Offset: 0x000BDAF4
	public virtual void MovementUpdate(float delta)
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		if (!this.IsAlive() || base.IsWounded() || (!base.isMounted && !this.IsNavRunning()))
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			if (this.IsNavRunning())
			{
				this.NavAgent.destination = this.ServerPosition;
			}
			return;
		}
		Vector3 vector = base.transform.position;
		if (this.HasPath)
		{
			vector = this.NavAgent.nextPosition;
		}
		if (!this.ValidateNextPosition(ref vector))
		{
			return;
		}
		this.UpdateSpeed(delta);
		this.UpdatePositionAndRotation(vector);
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x000BF998 File Offset: 0x000BDB98
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (!ValidBounds.Test(moveToPosition) && base.transform != null && !base.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition.ToString(),
				" (destroying)"
			}));
			base.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x000BFA10 File Offset: 0x000BDC10
	private void UpdateSpeed(float delta)
	{
		float num = this.DesiredMoveSpeed();
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, delta * 8f);
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x000BFA47 File Offset: 0x000BDC47
	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.lastPositionUpdateTime = UnityEngine.Time.time;
		this.ServerPosition = moveToPosition;
		this.SetAimDirection(this.GetAimDirection());
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x000BFA68 File Offset: 0x000BDC68
	public virtual float DesiredMoveSpeed()
	{
		float num = Mathf.Sin(UnityEngine.Time.time + this.randomOffset);
		return base.GetSpeed(num, 0f, 0f);
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x000BFA98 File Offset: 0x000BDC98
	public virtual Vector3 GetAimDirection()
	{
		if (Vector3Ex.Distance2D(this.finalDestination, this.GetPosition()) >= 1f)
		{
			Vector3 normalized = (this.finalDestination - this.GetPosition()).normalized;
			return new Vector3(normalized.x, 0f, normalized.z);
		}
		return this.eyes.BodyForward();
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x000BFAFC File Offset: 0x000BDCFC
	public virtual void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, 1f);
		}
		this.eyes.rotation = Quaternion.LookRotation(newAim, Vector3.up);
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
		this.lastPositionUpdateTime = UnityEngine.Time.time;
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x000BFB80 File Offset: 0x000BDD80
	public bool TryUseThrownWeapon(BaseEntity target, float attackRate)
	{
		if (this.HasThrownItemCooldown())
		{
			return false;
		}
		Item item = this.FindThrownWeapon();
		if (item == null)
		{
			this.lastThrowTime = UnityEngine.Time.time;
			return false;
		}
		return this.TryUseThrownWeapon(item, target, attackRate);
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x000BFBB8 File Offset: 0x000BDDB8
	public bool TryUseThrownWeapon(Item item, BaseEntity target, float attackRate)
	{
		if (this.HasThrownItemCooldown())
		{
			return false;
		}
		float num = Vector3.Distance(target.transform.position, base.transform.position);
		if (num <= 2f || num >= 20f)
		{
			return false;
		}
		Vector3 position = target.transform.position;
		if (!base.IsVisible(base.CenterPoint(), position, float.PositiveInfinity))
		{
			return false;
		}
		if (this.UseThrownWeapon(item, target))
		{
			if (this is ScarecrowNPC)
			{
				ScarecrowNPC.NextBeanCanAllowedTime = UnityEngine.Time.time + Halloween.scarecrow_throw_beancan_global_delay;
			}
			this.lastThrowTime = UnityEngine.Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x000BFC51 File Offset: 0x000BDE51
	public bool HasThrownItemCooldown()
	{
		return UnityEngine.Time.time - this.lastThrowTime < 10f;
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x000BFC68 File Offset: 0x000BDE68
	protected bool UseThrownWeapon(Item item, BaseEntity target)
	{
		base.UpdateActiveItem(item.uid);
		ThrownWeapon thrownWeapon = base.GetActiveItem().GetHeldEntity() as ThrownWeapon;
		if (thrownWeapon == null)
		{
			return false;
		}
		base.StartCoroutine(this.DoThrow(thrownWeapon, target));
		return true;
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x000BFCAD File Offset: 0x000BDEAD
	private IEnumerator DoThrow(ThrownWeapon thrownWeapon, BaseEntity target)
	{
		this.modelState.aiming = true;
		yield return new WaitForSeconds(1.5f);
		this.SetAimDirection(Vector3Ex.Direction(target.transform.position, base.transform.position));
		thrownWeapon.ResetAttackCooldown();
		thrownWeapon.ServerThrow(target.transform.position);
		this.modelState.aiming = false;
		base.Invoke(new Action(this.EquipTest), 0.5f);
		yield break;
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x000BFCCC File Offset: 0x000BDECC
	public Item FindThrownWeapon()
	{
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < this.inventory.containerBelt.capacity; i++)
		{
			Item slot = this.inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.GetHeldEntity() as ThrownWeapon != null)
			{
				return slot;
			}
		}
		return null;
	}

	// Token: 0x040012D5 RID: 4821
	public AIInformationZone VirtualInfoZone;

	// Token: 0x040012D6 RID: 4822
	public Vector3 finalDestination;

	// Token: 0x040012D7 RID: 4823
	[NonSerialized]
	private float randomOffset;

	// Token: 0x040012D8 RID: 4824
	[NonSerialized]
	private Vector3 spawnPos;

	// Token: 0x040012D9 RID: 4825
	public PlayerInventoryProperties[] loadouts;

	// Token: 0x040012DA RID: 4826
	public LayerMask movementMask = 429990145;

	// Token: 0x040012DB RID: 4827
	public bool LegacyNavigation = true;

	// Token: 0x040012DC RID: 4828
	public NavMeshAgent NavAgent;

	// Token: 0x040012DD RID: 4829
	public float damageScale = 1f;

	// Token: 0x040012DE RID: 4830
	public float shortRange = 10f;

	// Token: 0x040012DF RID: 4831
	public float attackLengthMaxShortRangeScale = 1f;

	// Token: 0x040012E0 RID: 4832
	private bool _isDormant;

	// Token: 0x040012E1 RID: 4833
	protected float lastGunShotTime;

	// Token: 0x040012E2 RID: 4834
	private float triggerEndTime;

	// Token: 0x040012E3 RID: 4835
	protected float nextTriggerTime;

	// Token: 0x040012E4 RID: 4836
	private float lastThinkTime;

	// Token: 0x040012E5 RID: 4837
	private float lastPositionUpdateTime;

	// Token: 0x040012E6 RID: 4838
	private float lastMovementTickTime;

	// Token: 0x040012E7 RID: 4839
	private Vector3 lastPos;

	// Token: 0x040012E8 RID: 4840
	private float lastThrowTime;
}
