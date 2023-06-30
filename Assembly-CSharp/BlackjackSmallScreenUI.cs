using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C3 RID: 1987
public class BlackjackSmallScreenUI : FacepunchBehaviour
{
	// Token: 0x04002C31 RID: 11313
	[SerializeField]
	private Canvas notInGameDisplay;

	// Token: 0x04002C32 RID: 11314
	[SerializeField]
	private Canvas inGameDisplay;

	// Token: 0x04002C33 RID: 11315
	[SerializeField]
	private RustText cardCountText;

	// Token: 0x04002C34 RID: 11316
	[SerializeField]
	private RustText betText;

	// Token: 0x04002C35 RID: 11317
	[SerializeField]
	private RustText splitBetText;

	// Token: 0x04002C36 RID: 11318
	[SerializeField]
	private RustText insuranceText;

	// Token: 0x04002C37 RID: 11319
	[SerializeField]
	private RustText bankText;

	// Token: 0x04002C38 RID: 11320
	[SerializeField]
	private RustText splitText;

	// Token: 0x04002C39 RID: 11321
	[SerializeField]
	private Canvas infoTextCanvas;

	// Token: 0x04002C3A RID: 11322
	[SerializeField]
	private RustText inGameText;

	// Token: 0x04002C3B RID: 11323
	[SerializeField]
	private RustText notInGameText;

	// Token: 0x04002C3C RID: 11324
	[SerializeField]
	private HorizontalLayoutGroup cardsLayout;

	// Token: 0x04002C3D RID: 11325
	[SerializeField]
	private BlackjackScreenCardUI[] cards;

	// Token: 0x04002C3E RID: 11326
	[SerializeField]
	private BlackjackScreenInputUI[] inputs;

	// Token: 0x04002C3F RID: 11327
	[SerializeField]
	private Translate.Phrase phraseBust;

	// Token: 0x04002C40 RID: 11328
	[SerializeField]
	private Translate.Phrase phraseBet;

	// Token: 0x04002C41 RID: 11329
	[SerializeField]
	private Translate.Phrase phrasePlaceYourBet;

	// Token: 0x04002C42 RID: 11330
	[SerializeField]
	private Translate.Phrase phraseStandoff;

	// Token: 0x04002C43 RID: 11331
	[SerializeField]
	private Translate.Phrase phraseYouWin;

	// Token: 0x04002C44 RID: 11332
	[SerializeField]
	private Translate.Phrase phraseYouLose;

	// Token: 0x04002C45 RID: 11333
	[SerializeField]
	private Translate.Phrase phraseWaitingForOtherPlayers;

	// Token: 0x04002C46 RID: 11334
	[SerializeField]
	private Translate.Phrase phraseAddFunds;

	// Token: 0x04002C47 RID: 11335
	[SerializeField]
	private Translate.Phrase phraseWaitingForPlayer;

	// Token: 0x04002C48 RID: 11336
	[SerializeField]
	private Translate.Phrase phraseSplitStored;

	// Token: 0x04002C49 RID: 11337
	[SerializeField]
	private Translate.Phrase phraseSplitActive;

	// Token: 0x04002C4A RID: 11338
	[SerializeField]
	private Translate.Phrase phraseHand;
}
