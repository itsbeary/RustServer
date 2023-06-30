using System;
using UnityEngine;

// Token: 0x02000712 RID: 1810
[ExecuteInEditMode]
public class AdaptMeshToTerrain : MonoBehaviour
{
	// Token: 0x040029AB RID: 10667
	public LayerMask LayerMask = -1;

	// Token: 0x040029AC RID: 10668
	public float RayHeight = 10f;

	// Token: 0x040029AD RID: 10669
	public float RayMaxDistance = 20f;

	// Token: 0x040029AE RID: 10670
	public float MinDisplacement = 0.01f;

	// Token: 0x040029AF RID: 10671
	public float MaxDisplacement = 0.33f;

	// Token: 0x040029B0 RID: 10672
	[Range(8f, 64f)]
	public int PlaneResolution = 24;
}
