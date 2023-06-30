using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class MeshColliderLookup
{
	// Token: 0x06001D8C RID: 7564 RVA: 0x000CB26C File Offset: 0x000C946C
	public void Apply()
	{
		MeshColliderLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x000CB29E File Offset: 0x000C949E
	public void Add(MeshColliderInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x000CB2AC File Offset: 0x000C94AC
	public MeshColliderLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x04001653 RID: 5715
	public MeshColliderLookup.LookupGroup src = new MeshColliderLookup.LookupGroup();

	// Token: 0x04001654 RID: 5716
	public MeshColliderLookup.LookupGroup dst = new MeshColliderLookup.LookupGroup();

	// Token: 0x02000CAB RID: 3243
	public class LookupGroup
	{
		// Token: 0x06004F72 RID: 20338 RVA: 0x001A6C03 File Offset: 0x001A4E03
		public void Clear()
		{
			this.data.Clear();
			this.indices.Clear();
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x001A6C1C File Offset: 0x001A4E1C
		public void Add(MeshColliderInstance instance)
		{
			this.data.Add(new MeshColliderLookup.LookupEntry(instance));
			int num = this.data.Count - 1;
			int num2 = instance.data.triangles.Length / 3;
			for (int i = 0; i < num2; i++)
			{
				this.indices.Add(num);
			}
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x001A6C70 File Offset: 0x001A4E70
		public MeshColliderLookup.LookupEntry Get(int index)
		{
			return this.data[this.indices[index]];
		}

		// Token: 0x040044D7 RID: 17623
		public List<MeshColliderLookup.LookupEntry> data = new List<MeshColliderLookup.LookupEntry>();

		// Token: 0x040044D8 RID: 17624
		public List<int> indices = new List<int>();
	}

	// Token: 0x02000CAC RID: 3244
	public struct LookupEntry
	{
		// Token: 0x06004F76 RID: 20342 RVA: 0x001A6CA7 File Offset: 0x001A4EA7
		public LookupEntry(MeshColliderInstance instance)
		{
			this.transform = instance.transform;
			this.rigidbody = instance.rigidbody;
			this.collider = instance.collider;
			this.bounds = instance.bounds;
		}

		// Token: 0x040044D9 RID: 17625
		public Transform transform;

		// Token: 0x040044DA RID: 17626
		public Rigidbody rigidbody;

		// Token: 0x040044DB RID: 17627
		public Collider collider;

		// Token: 0x040044DC RID: 17628
		public OBB bounds;
	}
}
