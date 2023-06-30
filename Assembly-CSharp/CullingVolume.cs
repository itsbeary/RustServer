using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004F9 RID: 1273
public class CullingVolume : MonoBehaviour, IClientComponent
{
	// Token: 0x04002166 RID: 8550
	[Tooltip("Override occludee root from children of this object (default) to children of any other object.")]
	public GameObject OccludeeRoot;

	// Token: 0x04002167 RID: 8551
	[Tooltip("Invert visibility. False will show occludes. True will hide them.")]
	public bool Invert;

	// Token: 0x04002168 RID: 8552
	[Tooltip("A portal in the culling volume chain does not toggle objects visible, it merely signals the non-portal volumes to hide their occludees.")]
	public bool Portal;

	// Token: 0x04002169 RID: 8553
	[Tooltip("Secondary culling volumes, connected to this one, that will get signaled when this trigger is activated.")]
	public List<CullingVolume> Connections = new List<CullingVolume>();
}
