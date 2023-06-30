using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000484 RID: 1156
public class CH47DropZone : MonoBehaviour
{
	// Token: 0x0600263D RID: 9789 RVA: 0x000F190C File Offset: 0x000EFB0C
	public void Awake()
	{
		if (!CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Add(this);
		}
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000F1928 File Offset: 0x000EFB28
	public static CH47DropZone GetClosest(Vector3 pos)
	{
		float num = float.PositiveInfinity;
		CH47DropZone ch47DropZone = null;
		foreach (CH47DropZone ch47DropZone2 in CH47DropZone.dropZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ch47DropZone2.transform.position);
			if (num2 < num)
			{
				num = num2;
				ch47DropZone = ch47DropZone2;
			}
		}
		return ch47DropZone;
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x000F199C File Offset: 0x000EFB9C
	public void OnDestroy()
	{
		if (CH47DropZone.dropZones.Contains(this))
		{
			CH47DropZone.dropZones.Remove(this);
		}
	}

	// Token: 0x06002640 RID: 9792 RVA: 0x000F19B7 File Offset: 0x000EFBB7
	public float TimeSinceLastDrop()
	{
		return Time.time - this.lastDropTime;
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x000F19C5 File Offset: 0x000EFBC5
	public void Used()
	{
		this.lastDropTime = Time.time;
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000F19D2 File Offset: 0x000EFBD2
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 5f);
	}

	// Token: 0x04001EAC RID: 7852
	public float lastDropTime;

	// Token: 0x04001EAD RID: 7853
	private static List<CH47DropZone> dropZones = new List<CH47DropZone>();
}
