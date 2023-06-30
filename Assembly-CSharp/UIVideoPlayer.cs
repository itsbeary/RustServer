using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x020007E3 RID: 2019
public class UIVideoPlayer : UIDialog
{
	// Token: 0x04002D6A RID: 11626
	public AspectRatioFitter aspectRatioFitter;

	// Token: 0x04002D6B RID: 11627
	public GameObject closeButton;

	// Token: 0x04002D6C RID: 11628
	public VideoPlayer videoPlayer;

	// Token: 0x04002D6D RID: 11629
	public RawImage videoCanvas;

	// Token: 0x04002D6E RID: 11630
	public RectTransform videoProgressBar;

	// Token: 0x04002D6F RID: 11631
	public GameObject loadingIndicator;

	// Token: 0x04002D70 RID: 11632
	public float audioDuckingAmount = 0.333f;

	// Token: 0x04002D71 RID: 11633
	public float timeoutAfter = 5f;
}
