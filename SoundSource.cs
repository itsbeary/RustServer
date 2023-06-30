using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD
{
	// Token: 0x06001C63 RID: 7267 RVA: 0x000C5B42 File Offset: 0x000C3D42
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool IsSyncedToParent()
	{
		return false;
	}

	// Token: 0x040014F1 RID: 5361
	[Header("Occlusion")]
	public bool handleOcclusionChecks;

	// Token: 0x040014F2 RID: 5362
	public LayerMask occlusionLayerMask;

	// Token: 0x040014F3 RID: 5363
	public List<SoundSource.OcclusionPoint> occlusionPoints = new List<SoundSource.OcclusionPoint>();

	// Token: 0x040014F4 RID: 5364
	public bool isOccluded;

	// Token: 0x040014F5 RID: 5365
	public float occlusionAmount;

	// Token: 0x040014F6 RID: 5366
	public float lodDistance = 100f;

	// Token: 0x040014F7 RID: 5367
	public bool inRange;

	// Token: 0x02000C9B RID: 3227
	[Serializable]
	public class OcclusionPoint
	{
		// Token: 0x04004470 RID: 17520
		public Vector3 offset = Vector3.zero;

		// Token: 0x04004471 RID: 17521
		public bool isOccluded;
	}
}
