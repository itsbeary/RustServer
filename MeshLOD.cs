using System;
using UnityEngine;

// Token: 0x02000545 RID: 1349
public class MeshLOD : LODComponent, IBatchingHandler
{
	// Token: 0x0400227B RID: 8827
	[Horizontal(1, 0)]
	public MeshLOD.State[] States;

	// Token: 0x02000D59 RID: 3417
	[Serializable]
	public class State
	{
		// Token: 0x0400479E RID: 18334
		public float distance;

		// Token: 0x0400479F RID: 18335
		public Mesh mesh;
	}
}
