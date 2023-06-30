using System;
using UnityEngine;

// Token: 0x0200073B RID: 1851
[ExecuteInEditMode]
[RequireComponent(typeof(WindZone))]
public class WindZoneExManager : MonoBehaviour
{
	// Token: 0x04002A38 RID: 10808
	public float maxAccumMain = 4f;

	// Token: 0x04002A39 RID: 10809
	public float maxAccumTurbulence = 4f;

	// Token: 0x04002A3A RID: 10810
	public float globalMainScale = 1f;

	// Token: 0x04002A3B RID: 10811
	public float globalTurbulenceScale = 1f;

	// Token: 0x04002A3C RID: 10812
	public Transform testPosition;

	// Token: 0x02000E55 RID: 3669
	private enum TestMode
	{
		// Token: 0x04004B7F RID: 19327
		Disabled,
		// Token: 0x04004B80 RID: 19328
		Low
	}
}
