using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200047F RID: 1151
public class DamageRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x04001E6D RID: 7789
	[SerializeField]
	private List<Material> damageShowingMats;

	// Token: 0x04001E6E RID: 7790
	[SerializeField]
	private float maxDamageOpacity = 0.9f;

	// Token: 0x04001E6F RID: 7791
	[SerializeField]
	[HideInInspector]
	private List<DamageRenderer.DamageShowingRenderer> damageShowingRenderers;

	// Token: 0x04001E70 RID: 7792
	[SerializeField]
	[HideInInspector]
	private List<GlassPane> damageShowingGlassRenderers;

	// Token: 0x02000D15 RID: 3349
	[Serializable]
	private struct DamageShowingRenderer
	{
		// Token: 0x06005048 RID: 20552 RVA: 0x001A85C8 File Offset: 0x001A67C8
		public DamageShowingRenderer(Renderer renderer, int[] indices)
		{
			this.renderer = renderer;
			this.indices = indices;
		}

		// Token: 0x040046B8 RID: 18104
		public Renderer renderer;

		// Token: 0x040046B9 RID: 18105
		public int[] indices;
	}
}
