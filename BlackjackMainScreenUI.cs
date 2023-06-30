using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C0 RID: 1984
public class BlackjackMainScreenUI : FacepunchBehaviour
{
	// Token: 0x04002C12 RID: 11282
	[SerializeField]
	private Canvas inGameDisplay;

	// Token: 0x04002C13 RID: 11283
	[SerializeField]
	private Canvas notInGameDisplay;

	// Token: 0x04002C14 RID: 11284
	[SerializeField]
	private Sprite faceNeutral;

	// Token: 0x04002C15 RID: 11285
	[SerializeField]
	private Sprite faceShocked;

	// Token: 0x04002C16 RID: 11286
	[SerializeField]
	private Sprite faceSad;

	// Token: 0x04002C17 RID: 11287
	[SerializeField]
	private Sprite faceCool;

	// Token: 0x04002C18 RID: 11288
	[SerializeField]
	private Sprite faceHappy;

	// Token: 0x04002C19 RID: 11289
	[SerializeField]
	private Sprite faceLove;

	// Token: 0x04002C1A RID: 11290
	[SerializeField]
	private Image faceInGame;

	// Token: 0x04002C1B RID: 11291
	[SerializeField]
	private Image faceNotInGame;

	// Token: 0x04002C1C RID: 11292
	[SerializeField]
	private Sprite[] faceNeutralVariants;

	// Token: 0x04002C1D RID: 11293
	[SerializeField]
	private Sprite[] faceHalloweenVariants;

	// Token: 0x04002C1E RID: 11294
	[SerializeField]
	private RustText cardCountText;

	// Token: 0x04002C1F RID: 11295
	[SerializeField]
	private RustText payoutText;

	// Token: 0x04002C20 RID: 11296
	[SerializeField]
	private RustText insuranceText;

	// Token: 0x04002C21 RID: 11297
	[SerializeField]
	private Canvas placeBetsCanvas;

	// Token: 0x04002C22 RID: 11298
	[SerializeField]
	private HorizontalLayoutGroup cardsLayout;

	// Token: 0x04002C23 RID: 11299
	[SerializeField]
	private BlackjackScreenCardUI[] cards;

	// Token: 0x04002C24 RID: 11300
	[SerializeField]
	private Translate.Phrase phraseBust;
}
