using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200097D RID: 2429
public class ViewModelRenderer : MonoBehaviour
{
	// Token: 0x04003472 RID: 13426
	public List<Texture2D> cachedTextureRefs = new List<Texture2D>();

	// Token: 0x04003473 RID: 13427
	public List<ViewModelDrawEvent> opaqueEvents = new List<ViewModelDrawEvent>();

	// Token: 0x04003474 RID: 13428
	public List<ViewModelDrawEvent> transparentEvents = new List<ViewModelDrawEvent>();

	// Token: 0x04003475 RID: 13429
	public Matrix4x4 prevModelMatrix;

	// Token: 0x04003476 RID: 13430
	private Renderer viewModelRenderer;
}
