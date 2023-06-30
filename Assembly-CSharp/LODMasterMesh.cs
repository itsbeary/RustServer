using System;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class LODMasterMesh : LODComponent
{
	// Token: 0x0400226C RID: 8812
	public MeshRenderer ReplacementMesh;

	// Token: 0x0400226D RID: 8813
	public float Distance = 100f;

	// Token: 0x0400226E RID: 8814
	public LODComponent[] ChildComponents;

	// Token: 0x0400226F RID: 8815
	public bool Block;

	// Token: 0x04002270 RID: 8816
	public Bounds MeshBounds;
}
