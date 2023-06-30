using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C1 RID: 1985
public class BlackjackScreenCardUI : FacepunchBehaviour
{
	// Token: 0x04002C25 RID: 11301
	[SerializeField]
	private Canvas baseCanvas;

	// Token: 0x04002C26 RID: 11302
	[SerializeField]
	private Canvas cardFront;

	// Token: 0x04002C27 RID: 11303
	[SerializeField]
	private Canvas cardBack;

	// Token: 0x04002C28 RID: 11304
	[SerializeField]
	private Image image;

	// Token: 0x04002C29 RID: 11305
	[SerializeField]
	private RustText text;

	// Token: 0x04002C2A RID: 11306
	[SerializeField]
	private Sprite heartSprite;

	// Token: 0x04002C2B RID: 11307
	[SerializeField]
	private Sprite diamondSprite;

	// Token: 0x04002C2C RID: 11308
	[SerializeField]
	private Sprite spadeSprite;

	// Token: 0x04002C2D RID: 11309
	[SerializeField]
	private Sprite clubSprite;
}
