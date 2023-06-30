using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class AlignedLineDrawer : MonoBehaviour, IClientComponent
{
	// Token: 0x040011CA RID: 4554
	public MeshFilter Filter;

	// Token: 0x040011CB RID: 4555
	public MeshRenderer Renderer;

	// Token: 0x040011CC RID: 4556
	public float LineWidth = 1f;

	// Token: 0x040011CD RID: 4557
	public float SurfaceOffset = 0.001f;

	// Token: 0x040011CE RID: 4558
	public float SprayThickness = 0.4f;

	// Token: 0x040011CF RID: 4559
	public float uvTilingFactor = 1f;

	// Token: 0x040011D0 RID: 4560
	public bool DrawEndCaps;

	// Token: 0x040011D1 RID: 4561
	public bool DrawSideMesh;

	// Token: 0x040011D2 RID: 4562
	public bool DrawBackMesh;

	// Token: 0x040011D3 RID: 4563
	public SprayCanSpray_Freehand Spray;

	// Token: 0x02000C4C RID: 3148
	[Serializable]
	public struct LinePoint
	{
		// Token: 0x04004348 RID: 17224
		public Vector3 LocalPosition;

		// Token: 0x04004349 RID: 17225
		public Vector3 WorldNormal;
	}
}
