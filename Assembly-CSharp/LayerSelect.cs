using System;
using UnityEngine;

// Token: 0x02000968 RID: 2408
[Serializable]
public struct LayerSelect
{
	// Token: 0x060039B4 RID: 14772 RVA: 0x00155F02 File Offset: 0x00154102
	public LayerSelect(int layer)
	{
		this.layer = layer;
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x00155F0B File Offset: 0x0015410B
	public static implicit operator int(LayerSelect layer)
	{
		return layer.layer;
	}

	// Token: 0x060039B6 RID: 14774 RVA: 0x00155F13 File Offset: 0x00154113
	public static implicit operator LayerSelect(int layer)
	{
		return new LayerSelect(layer);
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x060039B7 RID: 14775 RVA: 0x00155F1B File Offset: 0x0015411B
	public int Mask
	{
		get
		{
			return 1 << this.layer;
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x060039B8 RID: 14776 RVA: 0x00155F28 File Offset: 0x00154128
	public string Name
	{
		get
		{
			return LayerMask.LayerToName(this.layer);
		}
	}

	// Token: 0x04003426 RID: 13350
	[SerializeField]
	private int layer;
}
