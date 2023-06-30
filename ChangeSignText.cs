using System;
using UnityEngine;

// Token: 0x020007CA RID: 1994
public class ChangeSignText : UIDialog
{
	// Token: 0x04002CC4 RID: 11460
	public Action<int, Texture2D> onUpdateTexture;

	// Token: 0x04002CC5 RID: 11461
	public GameObject objectContainer;

	// Token: 0x04002CC6 RID: 11462
	public GameObject currentFrameSection;

	// Token: 0x04002CC7 RID: 11463
	public GameObject[] frameOptions;

	// Token: 0x04002CC8 RID: 11464
	public Camera cameraPreview;

	// Token: 0x04002CC9 RID: 11465
	public Camera camera3D;
}
