using System;

namespace UnityEngine
{
	// Token: 0x02000A31 RID: 2609
	public static class SkinnedMeshRendererEx
	{
		// Token: 0x06003DC1 RID: 15809 RVA: 0x001699A4 File Offset: 0x00167BA4
		public static Transform FindRig(this SkinnedMeshRenderer renderer)
		{
			Transform parent = renderer.transform.parent;
			Transform transform = renderer.rootBone;
			while (transform != null && transform.parent != null && transform.parent != parent)
			{
				transform = transform.parent;
			}
			return transform;
		}
	}
}
