using System;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class AITraversalArea : TriggerBase
{
	// Token: 0x060019B9 RID: 6585 RVA: 0x000BC4C7 File Offset: 0x000BA6C7
	public void OnValidate()
	{
		this.movementArea.center = base.transform.position;
	}

	// Token: 0x060019BA RID: 6586 RVA: 0x000BC4E0 File Offset: 0x000BA6E0
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
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x000BC52D File Offset: 0x000BA72D
	public bool CanTraverse(BaseEntity ent)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x000BC53C File Offset: 0x000BA73C
	public Transform GetClosestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num < num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x000BC57C File Offset: 0x000BA77C
	public Transform GetFarthestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num > num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000BC5BC File Offset: 0x000BA7BC
	public void SetBusyFor(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x000BC52D File Offset: 0x000BA72D
	public bool CanUse(Vector3 dirFrom)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x000BC5CB File Offset: 0x000BA7CB
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x000BC5D4 File Offset: 0x000BA7D4
	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		Vector3 position = this.GetClosestEntry(pos).position;
		Vector3 position2 = this.GetFarthestEntry(pos).position;
		new BaseEntity[1];
		AITraversalWaitPoint aitraversalWaitPoint = null;
		float num = 0f;
		foreach (AITraversalWaitPoint aitraversalWaitPoint2 in this.waitPoints)
		{
			if (!aitraversalWaitPoint2.Occupied())
			{
				Vector3 position3 = aitraversalWaitPoint2.transform.position;
				float num2 = Vector3.Distance(position, position3);
				if (Vector3.Distance(position2, position3) >= num2)
				{
					float num3 = Vector3.Distance(position3, pos);
					float num4 = (1f - Mathf.InverseLerp(0f, 20f, num3)) * 100f;
					if (num4 > num)
					{
						num = num4;
						aitraversalWaitPoint = aitraversalWaitPoint2;
					}
				}
			}
		}
		return aitraversalWaitPoint;
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x000BC68E File Offset: 0x000BA88E
	public bool EntityFilter(BaseEntity ent)
	{
		return ent.IsNpc && ent.isServer;
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x000BC6A0 File Offset: 0x000BA8A0
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x000BC6AC File Offset: 0x000BA8AC
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(this.entryPoint1.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(this.entryPoint2.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
		Gizmos.DrawCube(this.movementArea.center, this.movementArea.size);
		Gizmos.color = Color.magenta;
		AITraversalWaitPoint[] array = this.waitPoints;
		for (int i = 0; i < array.Length; i++)
		{
			GizmosUtil.DrawCircleY(array[i].transform.position, 0.5f);
		}
	}

	// Token: 0x04001268 RID: 4712
	public Transform entryPoint1;

	// Token: 0x04001269 RID: 4713
	public Transform entryPoint2;

	// Token: 0x0400126A RID: 4714
	public AITraversalWaitPoint[] waitPoints;

	// Token: 0x0400126B RID: 4715
	public Bounds movementArea;

	// Token: 0x0400126C RID: 4716
	public Transform activeEntryPoint;

	// Token: 0x0400126D RID: 4717
	public float nextFreeTime;
}
