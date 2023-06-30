using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B1 RID: 2225
public class UIInvertedMaskImage : Image
{
	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06003713 RID: 14099 RVA: 0x0014BA5E File Offset: 0x00149C5E
	public override Material materialForRendering
	{
		get
		{
			if (this.cachedMaterial == null)
			{
				this.cachedMaterial = UnityEngine.Object.Instantiate<Material>(base.materialForRendering);
				this.cachedMaterial.SetInt("_StencilComp", 6);
			}
			return this.cachedMaterial;
		}
	}

	// Token: 0x04003236 RID: 12854
	private Material cachedMaterial;
}
