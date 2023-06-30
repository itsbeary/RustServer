using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200054B RID: 1355
public class RendererLOD : LODComponent, IBatchingHandler
{
	// Token: 0x04002282 RID: 8834
	public RendererLOD.State[] States;

	// Token: 0x02000D5D RID: 3421
	[Serializable]
	public class State
	{
		// Token: 0x040047A9 RID: 18345
		public float distance;

		// Token: 0x040047AA RID: 18346
		public Renderer renderer;

		// Token: 0x040047AB RID: 18347
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x040047AC RID: 18348
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x040047AD RID: 18349
		[NonSerialized]
		public bool isImpostor;
	}
}
