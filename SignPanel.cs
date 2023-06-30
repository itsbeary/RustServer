using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017F RID: 383
public class SignPanel : MonoBehaviour, IImageReceiver
{
	// Token: 0x0400108F RID: 4239
	public RawImage Image;

	// Token: 0x04001090 RID: 4240
	public RectTransform ImageContainer;

	// Token: 0x04001091 RID: 4241
	public RustText DisabledSignsMessage;
}
