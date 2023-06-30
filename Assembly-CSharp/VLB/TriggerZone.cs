using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009BE RID: 2494
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-triggerzone/")]
	public class TriggerZone : MonoBehaviour
	{
		// Token: 0x06003B4F RID: 15183 RVA: 0x0015ED80 File Offset: 0x0015CF80
		private void Update()
		{
			VolumetricLightBeam component = base.GetComponent<VolumetricLightBeam>();
			if (component)
			{
				MeshCollider orAddComponent = base.gameObject.GetOrAddComponent<MeshCollider>();
				Debug.Assert(orAddComponent);
				float num = component.fadeEnd * this.rangeMultiplier;
				float num2 = Mathf.LerpUnclamped(component.coneRadiusStart, component.coneRadiusEnd, this.rangeMultiplier);
				this.m_Mesh = MeshGenerator.GenerateConeZ_Radius(num, component.coneRadiusStart, num2, 8, 0, false);
				this.m_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				orAddComponent.sharedMesh = this.m_Mesh;
				if (this.setIsTrigger)
				{
					orAddComponent.convex = true;
					orAddComponent.isTrigger = true;
				}
				UnityEngine.Object.Destroy(this);
			}
		}

		// Token: 0x0400364E RID: 13902
		public bool setIsTrigger = true;

		// Token: 0x0400364F RID: 13903
		public float rangeMultiplier = 1f;

		// Token: 0x04003650 RID: 13904
		private const int kMeshColliderNumSides = 8;

		// Token: 0x04003651 RID: 13905
		private Mesh m_Mesh;
	}
}
