using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class AIInformationCellContents<T> where T : AIPoint
{
	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001966 RID: 6502 RVA: 0x000BA69C File Offset: 0x000B889C
	public int Count
	{
		get
		{
			return this.Items.Count;
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001967 RID: 6503 RVA: 0x000BA6A9 File Offset: 0x000B88A9
	public bool Empty
	{
		get
		{
			return this.Items.Count == 0;
		}
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000BA6BC File Offset: 0x000B88BC
	public void Init(Bounds cellBounds, GameObject root)
	{
		this.Clear();
		foreach (T t in root.GetComponentsInChildren<T>(true))
		{
			if (cellBounds.Contains(t.gameObject.transform.position))
			{
				this.Add(t);
			}
		}
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000BA712 File Offset: 0x000B8912
	public void Clear()
	{
		this.Items.Clear();
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000BA71F File Offset: 0x000B891F
	public void Add(T item)
	{
		this.Items.Add(item);
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000BA72E File Offset: 0x000B892E
	public void Remove(T item)
	{
		this.Items.Remove(item);
	}

	// Token: 0x04001237 RID: 4663
	public HashSet<T> Items = new HashSet<T>();
}
