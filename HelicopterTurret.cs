using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

// Token: 0x02000421 RID: 1057
public class HelicopterTurret : MonoBehaviour
{
	// Token: 0x060023BB RID: 9147 RVA: 0x000E4347 File Offset: 0x000E2547
	public void SetTarget(BaseCombatEntity newTarget)
	{
		this._target = newTarget;
		this.UpdateTargetVisibility();
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000E4356 File Offset: 0x000E2556
	public bool NeedsNewTarget()
	{
		return !this.HasTarget() || (!this.targetVisible && this.TimeSinceTargetLastSeen() > this.loseTargetAfter);
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000E437C File Offset: 0x000E257C
	public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
	{
		int num = UnityEngine.Random.Range(0, newTargetList.Count);
		int i = newTargetList.Count;
		while (i >= 0)
		{
			i--;
			PatrolHelicopterAI.targetinfo targetinfo = newTargetList[num];
			if (targetinfo != null && targetinfo.ent != null && targetinfo.IsVisible() && this.InFiringArc(targetinfo.ply))
			{
				this.SetTarget(targetinfo.ply);
				return true;
			}
			num++;
			if (num >= newTargetList.Count)
			{
				num = 0;
			}
		}
		return false;
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000E43F4 File Offset: 0x000E25F4
	public bool TargetVisible()
	{
		this.UpdateTargetVisibility();
		return this.targetVisible;
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000E4402 File Offset: 0x000E2602
	public float TimeSinceTargetLastSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTargetTime;
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000E4410 File Offset: 0x000E2610
	public bool HasTarget()
	{
		return this._target != null;
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000E441E File Offset: 0x000E261E
	public void ClearTarget()
	{
		this._target = null;
		this.targetVisible = false;
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000E4430 File Offset: 0x000E2630
	public void TurretThink()
	{
		if (this.HasTarget() && this.TimeSinceTargetLastSeen() > this.loseTargetAfter * 2f)
		{
			this.ClearTarget();
		}
		if (!this.HasTarget())
		{
			return;
		}
		if (UnityEngine.Time.time - this.lastBurstTime > this.burstLength + this.timeBetweenBursts && this.TargetVisible())
		{
			this.lastBurstTime = UnityEngine.Time.time;
		}
		if (UnityEngine.Time.time < this.lastBurstTime + this.burstLength && UnityEngine.Time.time - this.lastFireTime >= this.fireRate && this.InFiringArc(this._target))
		{
			this.lastFireTime = UnityEngine.Time.time;
			this.FireGun();
		}
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000E44E0 File Offset: 0x000E26E0
	public void FireGun()
	{
		this._heliAI.FireGun(this._target.transform.position + new Vector3(0f, 0.25f, 0f), PatrolHelicopter.bulletAccuracy, this.left);
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000E452C File Offset: 0x000E272C
	public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
	{
		return potentialtarget.transform.position;
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000E453C File Offset: 0x000E273C
	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		Vector3 positionForEntity = this.GetPositionForEntity(potentialtarget);
		Vector3 position = this.muzzleTransform.position;
		Vector3 normalized = (positionForEntity - position).normalized;
		return Vector3.Angle(this.left ? (-this._heliAI.transform.right) : this._heliAI.transform.right, normalized);
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000E45A0 File Offset: 0x000E27A0
	public bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return this.AngleToTarget(potentialtarget) < 80f;
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000E45B0 File Offset: 0x000E27B0
	public void UpdateTargetVisibility()
	{
		if (!this.HasTarget())
		{
			return;
		}
		Vector3 vector = this._target.transform.position;
		BasePlayer basePlayer = this._target as BasePlayer;
		if (basePlayer)
		{
			vector = basePlayer.eyes.position;
		}
		bool flag = false;
		float num = Vector3.Distance(vector, this.muzzleTransform.position);
		Vector3 normalized = (vector - this.muzzleTransform.position).normalized;
		RaycastHit raycastHit;
		if (num < this.maxTargetRange && this.InFiringArc(this._target) && GamePhysics.Trace(new Ray(this.muzzleTransform.position + normalized * 6f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null) && raycastHit.collider.gameObject.ToBaseEntity() == this._target)
		{
			flag = true;
		}
		if (flag)
		{
			this.lastSeenTargetTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.targetVisible = flag;
	}

	// Token: 0x04001BCA RID: 7114
	public PatrolHelicopterAI _heliAI;

	// Token: 0x04001BCB RID: 7115
	public float fireRate = 0.125f;

	// Token: 0x04001BCC RID: 7116
	public float burstLength = 3f;

	// Token: 0x04001BCD RID: 7117
	public float timeBetweenBursts = 3f;

	// Token: 0x04001BCE RID: 7118
	public float maxTargetRange = 300f;

	// Token: 0x04001BCF RID: 7119
	public float loseTargetAfter = 5f;

	// Token: 0x04001BD0 RID: 7120
	public Transform gun_yaw;

	// Token: 0x04001BD1 RID: 7121
	public Transform gun_pitch;

	// Token: 0x04001BD2 RID: 7122
	public Transform muzzleTransform;

	// Token: 0x04001BD3 RID: 7123
	public bool left;

	// Token: 0x04001BD4 RID: 7124
	public BaseCombatEntity _target;

	// Token: 0x04001BD5 RID: 7125
	private float lastBurstTime = float.NegativeInfinity;

	// Token: 0x04001BD6 RID: 7126
	private float lastFireTime = float.NegativeInfinity;

	// Token: 0x04001BD7 RID: 7127
	private float lastSeenTargetTime = float.NegativeInfinity;

	// Token: 0x04001BD8 RID: 7128
	private bool targetVisible;
}
