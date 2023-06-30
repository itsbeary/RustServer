using System;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class LodLevelDisplay : MonoBehaviour, IEditorComponent
{
	// Token: 0x04001819 RID: 6169
	public Color TextColor = Color.green;

	// Token: 0x0400181A RID: 6170
	[Range(1f, 6f)]
	public float TextScaleMultiplier = 1f;
}
