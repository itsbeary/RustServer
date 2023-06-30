using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008AB RID: 2219
public class PingWidget : MonoBehaviour
{
	// Token: 0x0400320D RID: 12813
	public RectTransform MoveTransform;

	// Token: 0x0400320E RID: 12814
	public RectTransform ScaleTransform;

	// Token: 0x0400320F RID: 12815
	public Image InnerImage;

	// Token: 0x04003210 RID: 12816
	public Image OuterImage;

	// Token: 0x04003211 RID: 12817
	public GameObject TeamLeaderRoot;

	// Token: 0x04003212 RID: 12818
	public GameObject CancelHoverRoot;

	// Token: 0x04003213 RID: 12819
	public SoundDefinition PingDeploySoundHostile;

	// Token: 0x04003214 RID: 12820
	public SoundDefinition PingDeploySoundGoTo;

	// Token: 0x04003215 RID: 12821
	public SoundDefinition PingDeploySoundDollar;

	// Token: 0x04003216 RID: 12822
	public SoundDefinition PingDeploySoundLoot;

	// Token: 0x04003217 RID: 12823
	public SoundDefinition PingDeploySoundNode;

	// Token: 0x04003218 RID: 12824
	public SoundDefinition PingDeploySoundGun;

	// Token: 0x04003219 RID: 12825
	public CanvasGroup FadeCanvas;
}
