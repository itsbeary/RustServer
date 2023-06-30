using System;
using UnityEngine;

// Token: 0x02000718 RID: 1816
[ExecuteInEditMode]
public class DeferredDecal : MonoBehaviour
{
	// Token: 0x040029C0 RID: 10688
	public Mesh mesh;

	// Token: 0x040029C1 RID: 10689
	public Material material;

	// Token: 0x040029C2 RID: 10690
	public DeferredDecalQueue queue;

	// Token: 0x040029C3 RID: 10691
	public bool applyImmediately = true;
}
