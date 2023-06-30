using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class RuntimePathNode : IAIPathNode
{
	// Token: 0x1700021B RID: 539
	// (get) Token: 0x060018F7 RID: 6391 RVA: 0x000B8BBE File Offset: 0x000B6DBE
	// (set) Token: 0x060018F8 RID: 6392 RVA: 0x000B8BC6 File Offset: 0x000B6DC6
	public Vector3 Position { get; set; }

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x060018F9 RID: 6393 RVA: 0x000B8BCF File Offset: 0x000B6DCF
	// (set) Token: 0x060018FA RID: 6394 RVA: 0x000B8BD7 File Offset: 0x000B6DD7
	public bool Straightaway { get; set; }

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x060018FB RID: 6395 RVA: 0x000B8BE0 File Offset: 0x000B6DE0
	public IEnumerable<IAIPathNode> Linked
	{
		get
		{
			return this.linked;
		}
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x000B8BE8 File Offset: 0x000B6DE8
	public RuntimePathNode(Vector3 position)
	{
		this.Position = position;
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x0000441C File Offset: 0x0000261C
	public bool IsValid()
	{
		return true;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x000B8C02 File Offset: 0x000B6E02
	public void AddLink(IAIPathNode link)
	{
		this.linked.Add(link);
	}

	// Token: 0x04001182 RID: 4482
	private HashSet<IAIPathNode> linked = new HashSet<IAIPathNode>();
}
