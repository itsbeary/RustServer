using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	// Token: 0x02000A1F RID: 2591
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Other/Scope Overlay")]
	public class ScopeEffect : PostEffectsBase, IImageEffect
	{
		// Token: 0x06003D7D RID: 15741 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CheckResources()
		{
			return true;
		}

		// Token: 0x06003D7E RID: 15742 RVA: 0x00168E6E File Offset: 0x0016706E
		public bool IsActive()
		{
			return base.enabled && this.CheckResources();
		}

		// Token: 0x06003D7F RID: 15743 RVA: 0x00168E80 File Offset: 0x00167080
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.overlayMaterial.SetVector("_Screen", new Vector2((float)Screen.width, (float)Screen.height));
			Graphics.Blit(source, destination, this.overlayMaterial);
		}

		// Token: 0x040037B9 RID: 14265
		public Material overlayMaterial;
	}
}
