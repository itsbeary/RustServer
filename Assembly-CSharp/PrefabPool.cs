using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000965 RID: 2405
public class PrefabPool
{
	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x060039A5 RID: 14757 RVA: 0x00155B32 File Offset: 0x00153D32
	public int Count
	{
		get
		{
			return this.stack.Count;
		}
	}

	// Token: 0x060039A6 RID: 14758 RVA: 0x00155B3F File Offset: 0x00153D3F
	public void Push(Poolable info)
	{
		this.stack.Push(info);
		info.EnterPool();
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x00155B54 File Offset: 0x00153D54
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		this.Push(component);
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x00155B70 File Offset: 0x00153D70
	public GameObject Pop(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		while (this.stack.Count > 0)
		{
			Poolable poolable = this.stack.Pop();
			if (poolable)
			{
				poolable.transform.position = pos;
				poolable.transform.rotation = rot;
				poolable.LeavePool();
				return poolable.gameObject;
			}
		}
		return null;
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x00155BC8 File Offset: 0x00153DC8
	public void Clear()
	{
		foreach (Poolable poolable in this.stack)
		{
			if (poolable)
			{
				UnityEngine.Object.Destroy(poolable.gameObject);
			}
		}
		this.stack.Clear();
	}

	// Token: 0x04003424 RID: 13348
	public Stack<Poolable> stack = new Stack<Poolable>();
}
