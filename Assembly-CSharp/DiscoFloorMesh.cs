using System;
using UnityEngine;

// Token: 0x020003A0 RID: 928
public class DiscoFloorMesh : MonoBehaviour, IClientComponent
{
	// Token: 0x040019A8 RID: 6568
	public int GridRows = 5;

	// Token: 0x040019A9 RID: 6569
	public int GridColumns = 5;

	// Token: 0x040019AA RID: 6570
	public float GridSize = 1f;

	// Token: 0x040019AB RID: 6571
	[Range(0f, 10f)]
	public float TestOffset;

	// Token: 0x040019AC RID: 6572
	public Color OffColor = Color.grey;

	// Token: 0x040019AD RID: 6573
	public MeshRenderer Renderer;

	// Token: 0x040019AE RID: 6574
	public bool DrawInEditor;

	// Token: 0x040019AF RID: 6575
	public MeshFilter Filter;

	// Token: 0x040019B0 RID: 6576
	public AnimationCurve customCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040019B1 RID: 6577
	public AnimationCurve customCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
