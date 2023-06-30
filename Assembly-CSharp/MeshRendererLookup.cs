using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public class MeshRendererLookup
{
	// Token: 0x06001DA4 RID: 7588 RVA: 0x000CC23C File Offset: 0x000CA43C
	public void Apply()
	{
		MeshRendererLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x000CC26E File Offset: 0x000CA46E
	public void Clear()
	{
		this.dst.Clear();
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x000CC27B File Offset: 0x000CA47B
	public void Add(MeshRendererInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x000CC289 File Offset: 0x000CA489
	public MeshRendererLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x0400166F RID: 5743
	public MeshRendererLookup.LookupGroup src = new MeshRendererLookup.LookupGroup();

	// Token: 0x04001670 RID: 5744
	public MeshRendererLookup.LookupGroup dst = new MeshRendererLookup.LookupGroup();

	// Token: 0x02000CAD RID: 3245
	public class LookupGroup
	{
		// Token: 0x06004F77 RID: 20343 RVA: 0x001A6CD9 File Offset: 0x001A4ED9
		public void Clear()
		{
			this.data.Clear();
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x001A6CE6 File Offset: 0x001A4EE6
		public void Add(MeshRendererInstance instance)
		{
			this.data.Add(new MeshRendererLookup.LookupEntry(instance));
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x001A6CF9 File Offset: 0x001A4EF9
		public MeshRendererLookup.LookupEntry Get(int index)
		{
			return this.data[index];
		}

		// Token: 0x040044DD RID: 17629
		public List<MeshRendererLookup.LookupEntry> data = new List<MeshRendererLookup.LookupEntry>();
	}

	// Token: 0x02000CAE RID: 3246
	public struct LookupEntry
	{
		// Token: 0x06004F7B RID: 20347 RVA: 0x001A6D1A File Offset: 0x001A4F1A
		public LookupEntry(MeshRendererInstance instance)
		{
			this.renderer = instance.renderer;
		}

		// Token: 0x040044DE RID: 17630
		public Renderer renderer;
	}
}
