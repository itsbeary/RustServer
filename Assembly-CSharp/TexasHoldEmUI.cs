using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020007C9 RID: 1993
public class TexasHoldEmUI : MonoBehaviour
{
	// Token: 0x04002CAA RID: 11434
	[SerializeField]
	private Image[] holeCardImages;

	// Token: 0x04002CAB RID: 11435
	[SerializeField]
	private Image[] holeCardBackings;

	// Token: 0x04002CAC RID: 11436
	[FormerlySerializedAs("flopCardImages")]
	[SerializeField]
	private Image[] communityCardImages;

	// Token: 0x04002CAD RID: 11437
	[SerializeField]
	private Image[] communityCardBackings;

	// Token: 0x04002CAE RID: 11438
	[SerializeField]
	private RustText potText;

	// Token: 0x04002CAF RID: 11439
	[SerializeField]
	private CardGamePlayerWidget[] playerWidgets;

	// Token: 0x04002CB0 RID: 11440
	[SerializeField]
	private Translate.Phrase phraseWinningHand;

	// Token: 0x04002CB1 RID: 11441
	[SerializeField]
	private Translate.Phrase foldPhrase;

	// Token: 0x04002CB2 RID: 11442
	[SerializeField]
	private Translate.Phrase raisePhrase;

	// Token: 0x04002CB3 RID: 11443
	[SerializeField]
	private Translate.Phrase checkPhrase;

	// Token: 0x04002CB4 RID: 11444
	[SerializeField]
	private Translate.Phrase callPhrase;

	// Token: 0x04002CB5 RID: 11445
	[SerializeField]
	private Translate.Phrase phraseRoyalFlush;

	// Token: 0x04002CB6 RID: 11446
	[SerializeField]
	private Translate.Phrase phraseStraightFlush;

	// Token: 0x04002CB7 RID: 11447
	[SerializeField]
	private Translate.Phrase phraseFourOfAKind;

	// Token: 0x04002CB8 RID: 11448
	[SerializeField]
	private Translate.Phrase phraseFullHouse;

	// Token: 0x04002CB9 RID: 11449
	[SerializeField]
	private Translate.Phrase phraseFlush;

	// Token: 0x04002CBA RID: 11450
	[SerializeField]
	private Translate.Phrase phraseStraight;

	// Token: 0x04002CBB RID: 11451
	[SerializeField]
	private Translate.Phrase phraseThreeOfAKind;

	// Token: 0x04002CBC RID: 11452
	[SerializeField]
	private Translate.Phrase phraseTwoPair;

	// Token: 0x04002CBD RID: 11453
	[SerializeField]
	private Translate.Phrase phrasePair;

	// Token: 0x04002CBE RID: 11454
	[SerializeField]
	private Translate.Phrase phraseHighCard;

	// Token: 0x04002CBF RID: 11455
	[SerializeField]
	private Translate.Phrase phraseRaiseAmount;

	// Token: 0x04002CC0 RID: 11456
	[SerializeField]
	private Sprite dealerChip;

	// Token: 0x04002CC1 RID: 11457
	[SerializeField]
	private Sprite smallBlindChip;

	// Token: 0x04002CC2 RID: 11458
	[SerializeField]
	private Sprite bigBlindChip;

	// Token: 0x04002CC3 RID: 11459
	[SerializeField]
	private Sprite noIcon;
}
