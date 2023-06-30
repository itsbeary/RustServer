using System;
using UnityEngine;

// Token: 0x0200030F RID: 783
[ExecuteInEditMode]
public class MeshTrimTester : MonoBehaviour
{
	// Token: 0x040017CE RID: 6094
	public MeshTrimSettings Settings = MeshTrimSettings.Default;

	// Token: 0x040017CF RID: 6095
	public Mesh SourceMesh;

	// Token: 0x040017D0 RID: 6096
	public MeshFilter TargetMeshFilter;

	// Token: 0x040017D1 RID: 6097
	public int SubtractIndex;
}
