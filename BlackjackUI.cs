using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C4 RID: 1988
public class BlackjackUI : MonoBehaviour
{
	// Token: 0x04002C4B RID: 11339
	[SerializeField]
	private Image[] playerCardImages;

	// Token: 0x04002C4C RID: 11340
	[SerializeField]
	private Image[] dealerCardImages;

	// Token: 0x04002C4D RID: 11341
	[SerializeField]
	private Image[] splitCardImages;

	// Token: 0x04002C4E RID: 11342
	[SerializeField]
	private Image[] playerCardBackings;

	// Token: 0x04002C4F RID: 11343
	[SerializeField]
	private Image[] dealerCardBackings;

	// Token: 0x04002C50 RID: 11344
	[SerializeField]
	private Image[] splitCardBackings;

	// Token: 0x04002C51 RID: 11345
	[SerializeField]
	private CardGamePlayerWidget[] playerWidgets;

	// Token: 0x04002C52 RID: 11346
	[SerializeField]
	private GameObject dealerValueObj;

	// Token: 0x04002C53 RID: 11347
	[SerializeField]
	private RustText dealerValueText;

	// Token: 0x04002C54 RID: 11348
	[SerializeField]
	private GameObject yourValueObj;

	// Token: 0x04002C55 RID: 11349
	[SerializeField]
	private RustText yourValueText;

	// Token: 0x04002C56 RID: 11350
	[SerializeField]
	private Translate.Phrase phrasePlaceYourBet;

	// Token: 0x04002C57 RID: 11351
	[SerializeField]
	private Translate.Phrase phraseHit;

	// Token: 0x04002C58 RID: 11352
	[SerializeField]
	private Translate.Phrase phraseStand;

	// Token: 0x04002C59 RID: 11353
	[SerializeField]
	private Translate.Phrase phraseSplit;

	// Token: 0x04002C5A RID: 11354
	[SerializeField]
	private Translate.Phrase phraseDouble;

	// Token: 0x04002C5B RID: 11355
	[SerializeField]
	private Translate.Phrase phraseInsurance;

	// Token: 0x04002C5C RID: 11356
	[SerializeField]
	private Translate.Phrase phraseBust;

	// Token: 0x04002C5D RID: 11357
	[SerializeField]
	private Translate.Phrase phraseBlackjack;

	// Token: 0x04002C5E RID: 11358
	[SerializeField]
	private Translate.Phrase phraseStandoff;

	// Token: 0x04002C5F RID: 11359
	[SerializeField]
	private Translate.Phrase phraseYouWin;

	// Token: 0x04002C60 RID: 11360
	[SerializeField]
	private Translate.Phrase phraseYouLose;

	// Token: 0x04002C61 RID: 11361
	[SerializeField]
	private Translate.Phrase phraseWaitingForOtherPlayers;

	// Token: 0x04002C62 RID: 11362
	[SerializeField]
	private Translate.Phrase phraseHand;

	// Token: 0x04002C63 RID: 11363
	[SerializeField]
	private Translate.Phrase phraseInsurancePaidOut;

	// Token: 0x04002C64 RID: 11364
	[SerializeField]
	private Sprite insuranceIcon;

	// Token: 0x04002C65 RID: 11365
	[SerializeField]
	private Sprite noIcon;

	// Token: 0x04002C66 RID: 11366
	[SerializeField]
	private Color bustTextColour;
}
