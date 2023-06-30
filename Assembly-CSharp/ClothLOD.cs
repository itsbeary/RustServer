using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class ClothLOD : FacepunchBehaviour
{
	// Token: 0x04002251 RID: 8785
	[ServerVar(Help = "distance cloth will simulate until")]
	public static float clothLODDist = 20f;

	// Token: 0x04002252 RID: 8786
	public Cloth cloth;
}
