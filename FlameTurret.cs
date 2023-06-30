using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class FlameTurret : StorageContainer
{
	// Token: 0x06000B34 RID: 2868 RVA: 0x00064B7C File Offset: 0x00062D7C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameTurret.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00023AF4 File Offset: 0x00021CF4
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00064BBC File Offset: 0x00062DBC
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x00064BC9 File Offset: 0x00062DC9
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.IsTriggered();
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00064BDF File Offset: 0x00062DDF
	public void SetTriggered(bool triggered)
	{
		if (triggered && this.HasFuel())
		{
			this.triggeredTime = Time.realtimeSinceStartup;
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, triggered && this.HasFuel(), false, true);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00064C10 File Offset: 0x00062E10
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.SendAimDir), 0f, 0.1f);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00064C34 File Offset: 0x00062E34
	public void SendAimDir()
	{
		float num = Time.realtimeSinceStartup - this.lastMovementUpdate;
		this.lastMovementUpdate = Time.realtimeSinceStartup;
		this.MovementUpdate(num);
		base.ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", this.aimDir);
		FlameTurret.updateFlameTurretQueueServer.Add(this);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x00064C7D File Offset: 0x00062E7D
	public float GetSpinSpeed()
	{
		return (float)(this.IsTriggered() ? 180 : 45);
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00064C91 File Offset: 0x00062E91
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		if (info.damageTypes.IsMeleeType())
		{
			this.SetTriggered(true);
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00064CB8 File Offset: 0x00062EB8
	public void MovementUpdate(float delta)
	{
		this.aimDir += new Vector3(0f, delta * this.GetSpinSpeed(), 0f) * (float)this.turnDir;
		if (this.aimDir.y >= this.arc || this.aimDir.y <= -this.arc)
		{
			this.turnDir *= -1;
			this.aimDir.y = Mathf.Clamp(this.aimDir.y, -this.arc, this.arc);
		}
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x00064D58 File Offset: 0x00062F58
	public void ServerThink()
	{
		bool flag = this.IsTriggered();
		float num = Time.realtimeSinceStartup - this.lastServerThink;
		this.lastServerThink = Time.realtimeSinceStartup;
		if (this.IsTriggered() && (Time.realtimeSinceStartup - this.triggeredTime > this.triggeredDuration || !this.HasFuel()))
		{
			this.SetTriggered(false);
		}
		if (!this.IsTriggered() && this.HasFuel() && this.CheckTrigger())
		{
			this.SetTriggered(true);
			Effect.server.Run(this.triggeredEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (flag != this.IsTriggered())
		{
			base.SendNetworkUpdateImmediate(false);
		}
		if (this.IsTriggered())
		{
			this.DoFlame(num);
		}
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00064E10 File Offset: 0x00063010
	public bool CheckTrigger()
	{
		if (Time.realtimeSinceStartup < this.nextTriggerCheckTime)
		{
			return false;
		}
		this.nextTriggerCheckTime = Time.realtimeSinceStartup + 1f / this.triggerCheckRate;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = this.trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity baseEntity in entityContents)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (!component.IsSleeping() && component.IsAlive() && component.transform.position.y <= this.GetEyePosition().y + 0.5f && !component.IsBuildingAuthed())
				{
					list.Clear();
					GamePhysics.TraceAll(new Ray(component.eyes.position, (this.GetEyePosition() - component.eyes.position).normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal, null);
					for (int i = 0; i < list.Count; i++)
					{
						BaseEntity entity = list[i].GetEntity();
						if (entity != null && (entity == this || entity.EqualNetID(this)))
						{
							flag = true;
							break;
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00064FB0 File Offset: 0x000631B0
	public override void OnKilled(HitInfo info)
	{
		float num = (float)this.GetFuelAmount() / 500f;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), this.GetEyePosition(), 2f, 6f, this.damagePerSec, 133120, true);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		int num2 = Mathf.CeilToInt(Mathf.Clamp(num * 8f, 1f, 8f));
		for (int i = 0; i < num2; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				baseEntity.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * UnityEngine.Random.Range(-1f, 1f);
				baseEntity.Spawn();
				baseEntity.SetVelocity(onUnitSphere * (float)UnityEngine.Random.Range(3, 10));
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x000650E8 File Offset: 0x000632E8
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00065116 File Offset: 0x00063316
	public bool HasFuel()
	{
		return this.GetFuelAmount() > 0;
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00065124 File Offset: 0x00063324
	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel += seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "flame_turret", true, false);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x000651AC File Offset: 0x000633AC
	public void DoFlame(float delta)
	{
		if (!this.UseFuel(delta))
		{
			return;
		}
		Ray ray = new Ray(this.GetEyePosition(), base.transform.TransformDirection(Quaternion.Euler(this.aimDir) * Vector3.forward));
		Vector3 origin = ray.origin;
		RaycastHit raycastHit;
		bool flag = Physics.SphereCast(ray, 0.4f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = origin + ray.direction * this.flameRange;
		}
		float amount = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = amount * delta;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), raycastHit.point - ray.direction * 0.1f, this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2230272, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.transform.position + new Vector3(0f, 1.25f, 0f), 0.25f, 0.25f, this.damagePerSec, 133120, false);
		this.damagePerSec[0].amount = amount;
		if (Time.realtimeSinceStartup >= this.nextFireballTime)
		{
			this.nextFireballTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(1f, 2f);
			Vector3 vector = ((UnityEngine.Random.Range(0, 10) <= 7 && flag) ? raycastHit.point : (ray.origin + ray.direction * (flag ? raycastHit.distance : this.flameRange) * UnityEngine.Random.Range(0.4f, 1f)));
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, vector - ray.direction * 0.25f, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = this;
				baseEntity.Spawn();
			}
		}
	}

	// Token: 0x04000753 RID: 1875
	public static FlameTurret.UpdateFlameTurretWorkQueue updateFlameTurretQueueServer = new FlameTurret.UpdateFlameTurretWorkQueue();

	// Token: 0x04000754 RID: 1876
	public Transform upper;

	// Token: 0x04000755 RID: 1877
	public Vector3 aimDir;

	// Token: 0x04000756 RID: 1878
	public float arc = 45f;

	// Token: 0x04000757 RID: 1879
	public float triggeredDuration = 5f;

	// Token: 0x04000758 RID: 1880
	public float flameRange = 7f;

	// Token: 0x04000759 RID: 1881
	public float flameRadius = 4f;

	// Token: 0x0400075A RID: 1882
	public float fuelPerSec = 1f;

	// Token: 0x0400075B RID: 1883
	public Transform eyeTransform;

	// Token: 0x0400075C RID: 1884
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x0400075D RID: 1885
	public GameObjectRef triggeredEffect;

	// Token: 0x0400075E RID: 1886
	public GameObjectRef fireballPrefab;

	// Token: 0x0400075F RID: 1887
	public GameObjectRef explosionEffect;

	// Token: 0x04000760 RID: 1888
	public TargetTrigger trigger;

	// Token: 0x04000761 RID: 1889
	private float nextFireballTime;

	// Token: 0x04000762 RID: 1890
	private int turnDir = 1;

	// Token: 0x04000763 RID: 1891
	private float lastMovementUpdate;

	// Token: 0x04000764 RID: 1892
	private float triggeredTime;

	// Token: 0x04000765 RID: 1893
	private float lastServerThink;

	// Token: 0x04000766 RID: 1894
	private float triggerCheckRate = 2f;

	// Token: 0x04000767 RID: 1895
	private float nextTriggerCheckTime;

	// Token: 0x04000768 RID: 1896
	private float pendingFuel;

	// Token: 0x02000BD7 RID: 3031
	public class UpdateFlameTurretWorkQueue : ObjectWorkQueue<FlameTurret>
	{
		// Token: 0x06004DCD RID: 19917 RVA: 0x001A18B3 File Offset: 0x0019FAB3
		protected override void RunJob(FlameTurret entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.ServerThink();
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x001A18C5 File Offset: 0x0019FAC5
		protected override bool ShouldAdd(FlameTurret entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
