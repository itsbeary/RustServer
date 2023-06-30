using System;
using UnityEngine;

// Token: 0x0200059A RID: 1434
public class TriggerPlayerForce : TriggerBase, IServerComponent
{
	// Token: 0x06002BD3 RID: 11219 RVA: 0x00109C3C File Offset: 0x00107E3C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity != null)
		{
			return baseEntity.gameObject;
		}
		return null;
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x00109C75 File Offset: 0x00107E75
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.HackDisableTick), 0f, 3.75f);
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x00109C93 File Offset: 0x00107E93
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.HackDisableTick));
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x00109CAD File Offset: 0x00107EAD
	protected override void OnDisable()
	{
		base.CancelInvoke(new Action(this.HackDisableTick));
		base.OnDisable();
	}

	// Token: 0x06002BD7 RID: 11223 RVA: 0x00108BA9 File Offset: 0x00106DA9
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	// Token: 0x06002BD8 RID: 11224 RVA: 0x00109CC8 File Offset: 0x00107EC8
	private void HackDisableTick()
	{
		if (this.entityContents == null || !base.enabled)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (this.IsInterested(baseEntity))
			{
				BasePlayer basePlayer = baseEntity.ToPlayer();
				if (basePlayer != null && !basePlayer.IsNpc)
				{
					basePlayer.PauseVehicleNoClipDetection(4f);
					basePlayer.PauseSpeedHackDetection(4f);
				}
			}
		}
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x00109D5C File Offset: 0x00107F5C
	protected void FixedUpdate()
	{
		if (this.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.entityContents)
			{
				if ((!this.requireUpAxis || Vector3.Dot(baseEntity.transform.up, base.transform.up) >= 0f) && this.IsInterested(baseEntity))
				{
					Vector3 vector = this.GetPushVelocity(baseEntity.gameObject);
					baseEntity.ApplyInheritedVelocity(vector);
				}
			}
		}
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x00109DF8 File Offset: 0x00107FF8
	private Vector3 GetPushVelocity(GameObject obj)
	{
		Vector3 vector = -(this.triggerCollider.bounds.center - obj.transform.position);
		vector.Normalize();
		vector.y = 0.2f;
		vector.Normalize();
		return vector * this.pushVelocity;
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x00109E54 File Offset: 0x00108054
	private bool IsInterested(BaseEntity entity)
	{
		if (entity == null || entity.isClient)
		{
			return false;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		return !(basePlayer != null) || (((!basePlayer.IsAdmin && !basePlayer.IsDeveloper) || !basePlayer.IsFlying) && (basePlayer != null && basePlayer.IsAlive()) && !basePlayer.isMounted);
	}

	// Token: 0x040023AC RID: 9132
	public BoxCollider triggerCollider;

	// Token: 0x040023AD RID: 9133
	public float pushVelocity = 5f;

	// Token: 0x040023AE RID: 9134
	public bool requireUpAxis;

	// Token: 0x040023AF RID: 9135
	private const float HACK_DISABLE_TIME = 4f;
}
