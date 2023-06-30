using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A7 RID: 679
public class CameraMan : SingletonComponent<CameraMan>
{
	// Token: 0x04001630 RID: 5680
	public static string DefaultSaveName = string.Empty;

	// Token: 0x04001631 RID: 5681
	public const string SavePositionExtension = ".cam";

	// Token: 0x04001632 RID: 5682
	public const string SavePositionDirectory = "camsaves";

	// Token: 0x04001633 RID: 5683
	public bool OnlyControlWhenCursorHidden = true;

	// Token: 0x04001634 RID: 5684
	public bool NeedBothMouseButtonsToZoom;

	// Token: 0x04001635 RID: 5685
	public float LookSensitivity = 1f;

	// Token: 0x04001636 RID: 5686
	public float MoveSpeed = 1f;

	// Token: 0x04001637 RID: 5687
	public static float GuideAspect = 4f;

	// Token: 0x04001638 RID: 5688
	public static float GuideRatio = 3f;

	// Token: 0x04001639 RID: 5689
	public Canvas canvas;

	// Token: 0x0400163A RID: 5690
	public Graphic[] guides;
}
