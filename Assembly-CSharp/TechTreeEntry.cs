using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007DC RID: 2012
public class TechTreeEntry : TechTreeWidget
{
	// Token: 0x04002D3D RID: 11581
	public RawImage icon;

	// Token: 0x04002D3E RID: 11582
	public GameObject ableToUnlockBackground;

	// Token: 0x04002D3F RID: 11583
	public GameObject unlockedBackground;

	// Token: 0x04002D40 RID: 11584
	public GameObject lockedBackground;

	// Token: 0x04002D41 RID: 11585
	public GameObject lockOverlay;

	// Token: 0x04002D42 RID: 11586
	public GameObject selectedBackground;

	// Token: 0x04002D43 RID: 11587
	public Image radialUnlock;

	// Token: 0x04002D44 RID: 11588
	public float holdTime = 1f;
}
