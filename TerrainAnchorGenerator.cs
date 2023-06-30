using System;
using UnityEngine;

// Token: 0x02000697 RID: 1687
public class TerrainAnchorGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x040027C9 RID: 10185
	public float PlacementRadius = 32f;

	// Token: 0x040027CA RID: 10186
	public float PlacementPadding;

	// Token: 0x040027CB RID: 10187
	public float PlacementFade = 16f;

	// Token: 0x040027CC RID: 10188
	public float PlacementDistance = 8f;

	// Token: 0x040027CD RID: 10189
	public float AnchorExtentsMin = 8f;

	// Token: 0x040027CE RID: 10190
	public float AnchorExtentsMax = 16f;

	// Token: 0x040027CF RID: 10191
	public float AnchorOffsetMin;

	// Token: 0x040027D0 RID: 10192
	public float AnchorOffsetMax;
}
