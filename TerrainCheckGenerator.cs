using System;
using UnityEngine;

// Token: 0x0200069A RID: 1690
public class TerrainCheckGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x040027D3 RID: 10195
	public float PlacementRadius = 32f;

	// Token: 0x040027D4 RID: 10196
	public float PlacementPadding;

	// Token: 0x040027D5 RID: 10197
	public float PlacementFade = 16f;

	// Token: 0x040027D6 RID: 10198
	public float PlacementDistance = 8f;

	// Token: 0x040027D7 RID: 10199
	public float CheckExtentsMin = 8f;

	// Token: 0x040027D8 RID: 10200
	public float CheckExtentsMax = 16f;

	// Token: 0x040027D9 RID: 10201
	public bool CheckRotate = true;
}
