using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002D1 RID: 721
public class MeshPaintController : MonoBehaviour, IClientComponent
{
	// Token: 0x040016B1 RID: 5809
	public Camera pickerCamera;

	// Token: 0x040016B2 RID: 5810
	public Texture2D brushTexture;

	// Token: 0x040016B3 RID: 5811
	public Vector2 brushScale = new Vector2(8f, 8f);

	// Token: 0x040016B4 RID: 5812
	public Color brushColor = Color.white;

	// Token: 0x040016B5 RID: 5813
	public float brushSpacing = 2f;

	// Token: 0x040016B6 RID: 5814
	public RawImage brushImage;

	// Token: 0x040016B7 RID: 5815
	public float brushPreviewScaleMultiplier = 1f;

	// Token: 0x040016B8 RID: 5816
	public bool applyDefaults;

	// Token: 0x040016B9 RID: 5817
	public Texture2D defaltBrushTexture;

	// Token: 0x040016BA RID: 5818
	public float defaultBrushSize = 16f;

	// Token: 0x040016BB RID: 5819
	public Color defaultBrushColor = Color.black;

	// Token: 0x040016BC RID: 5820
	public float defaultBrushAlpha = 0.5f;

	// Token: 0x040016BD RID: 5821
	public Toggle lastBrush;

	// Token: 0x040016BE RID: 5822
	public Button UndoButton;

	// Token: 0x040016BF RID: 5823
	public Button RedoButton;

	// Token: 0x040016C0 RID: 5824
	private Vector3 lastPosition;
}
