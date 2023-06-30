using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class TriggerSafeZone : TriggerBase
{
	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06002BEB RID: 11243 RVA: 0x0010A2DB File Offset: 0x001084DB
	// (set) Token: 0x06002BEC RID: 11244 RVA: 0x0010A2E3 File Offset: 0x001084E3
	public Collider triggerCollider { get; private set; }

	// Token: 0x06002BED RID: 11245 RVA: 0x0010A2EC File Offset: 0x001084EC
	protected void Awake()
	{
		this.triggerCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x0010A2FA File Offset: 0x001084FA
	protected void OnEnable()
	{
		TriggerSafeZone.allSafeZones.Add(this);
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x0010A307 File Offset: 0x00108507
	protected override void OnDisable()
	{
		base.OnDisable();
		TriggerSafeZone.allSafeZones.Remove(this);
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x0010A31C File Offset: 0x0010851C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x0010A360 File Offset: 0x00108560
	public bool PassesHeightChecks(Vector3 entPos)
	{
		Vector3 position = base.transform.position;
		float num = Mathf.Abs(position.y - entPos.y);
		return (this.maxDepth == -1f || entPos.y >= position.y || num <= this.maxDepth) && (this.maxAltitude == -1f || entPos.y <= position.y || num <= this.maxAltitude);
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x0010A3D9 File Offset: 0x001085D9
	public float GetSafeLevel(Vector3 pos)
	{
		if (!this.PassesHeightChecks(pos))
		{
			return 0f;
		}
		return 1f;
	}

	// Token: 0x040023B8 RID: 9144
	public static List<TriggerSafeZone> allSafeZones = new List<TriggerSafeZone>();

	// Token: 0x040023B9 RID: 9145
	public float maxDepth = 20f;

	// Token: 0x040023BA RID: 9146
	public float maxAltitude = -1f;
}
