using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200054F RID: 1359
public class TreeLOD : LODComponent
{
	// Token: 0x04002286 RID: 8838
	[Horizontal(1, 0)]
	public TreeLOD.State[] States;

	// Token: 0x02000D5E RID: 3422
	[Serializable]
	public class State
	{
		// Token: 0x040047AE RID: 18350
		public float distance;

		// Token: 0x040047AF RID: 18351
		public Renderer renderer;

		// Token: 0x040047B0 RID: 18352
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x040047B1 RID: 18353
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x040047B2 RID: 18354
		[NonSerialized]
		public bool isImpostor;
	}
}
