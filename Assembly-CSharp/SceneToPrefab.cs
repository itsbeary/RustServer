using System;
using UnityEngine;

// Token: 0x0200056A RID: 1386
public class SceneToPrefab : MonoBehaviour, IEditorComponent
{
	// Token: 0x040022E1 RID: 8929
	public bool flattenHierarchy;

	// Token: 0x040022E2 RID: 8930
	public GameObject outputPrefab;

	// Token: 0x040022E3 RID: 8931
	[Tooltip("If true the HLOD generation will be skipped and the previous results will be used, good to use if non-visual changes were made (eg.triggers)")]
	public bool skipAllHlod;
}
