using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class BasePathNode : MonoBehaviour, IAIPathNode
{
	// Token: 0x1700020A RID: 522
	// (get) Token: 0x060018B6 RID: 6326 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x060018B7 RID: 6327 RVA: 0x000B835B File Offset: 0x000B655B
	public bool Straightaway
	{
		get
		{
			return this.straightaway;
		}
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x060018B8 RID: 6328 RVA: 0x000B8363 File Offset: 0x000B6563
	public IEnumerable<IAIPathNode> Linked
	{
		get
		{
			return this.linked;
		}
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x000B836B File Offset: 0x000B656B
	public bool IsValid()
	{
		return base.transform != null;
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x0002A507 File Offset: 0x00028707
	public void AddLink(IAIPathNode link)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnDrawGizmosSelected()
	{
	}

	// Token: 0x0400115C RID: 4444
	public BasePath Path;

	// Token: 0x0400115D RID: 4445
	public List<BasePathNode> linked;

	// Token: 0x0400115E RID: 4446
	public float maxVelocityOnApproach = -1f;

	// Token: 0x0400115F RID: 4447
	public bool straightaway;
}
