using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class SprayCanSpray_Decal : SprayCanSpray, ICustomMaterialReplacer, IPropRenderNotify, INotifyLOD
{
	// Token: 0x040011D8 RID: 4568
	public DeferredDecal DecalComponent;

	// Token: 0x040011D9 RID: 4569
	public GameObject IconPreviewRoot;

	// Token: 0x040011DA RID: 4570
	public Material DefaultMaterial;
}
