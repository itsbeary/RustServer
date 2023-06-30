using System;
using Facepunch.CardGames;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C6 RID: 1990
public class CardGameUI : UIDialog
{
	// Token: 0x04002C7B RID: 11387
	[Header("Card Game")]
	[SerializeField]
	private CardGameUI.InfoTextUI primaryInfo;

	// Token: 0x04002C7C RID: 11388
	[SerializeField]
	private CardGameUI.InfoTextUI secondaryInfo;

	// Token: 0x04002C7D RID: 11389
	[SerializeField]
	private CardGameUI.InfoTextUI playerLeaveInfo;

	// Token: 0x04002C7E RID: 11390
	[SerializeField]
	private GameObject playingUI;

	// Token: 0x04002C7F RID: 11391
	[SerializeField]
	private CardGameUI.PlayingCardImage[] cardImages;

	// Token: 0x04002C80 RID: 11392
	[SerializeField]
	private CardInputWidget[] inputWidgets;

	// Token: 0x04002C81 RID: 11393
	[SerializeField]
	private RustSlider dismountProgressSlider;

	// Token: 0x04002C82 RID: 11394
	[SerializeField]
	private Translate.Phrase phraseLoading;

	// Token: 0x04002C83 RID: 11395
	[SerializeField]
	private Translate.Phrase phraseWaitingForNextRound;

	// Token: 0x04002C84 RID: 11396
	[SerializeField]
	private Translate.Phrase phraseNotEnoughPlayers;

	// Token: 0x04002C85 RID: 11397
	[SerializeField]
	private Translate.Phrase phrasePlayerLeftGame;

	// Token: 0x04002C86 RID: 11398
	[SerializeField]
	private Translate.Phrase phraseNotEnoughBuyIn;

	// Token: 0x04002C87 RID: 11399
	[SerializeField]
	private Translate.Phrase phraseTooMuchBuyIn;

	// Token: 0x04002C88 RID: 11400
	public Translate.Phrase phraseYourTurn;

	// Token: 0x04002C89 RID: 11401
	public Translate.Phrase phraseYouWinTheRound;

	// Token: 0x04002C8A RID: 11402
	public Translate.Phrase phraseRoundWinner;

	// Token: 0x04002C8B RID: 11403
	public Translate.Phrase phraseRoundWinners;

	// Token: 0x04002C8C RID: 11404
	public Translate.Phrase phraseScrapWon;

	// Token: 0x04002C8D RID: 11405
	public Translate.Phrase phraseScrapReturned;

	// Token: 0x04002C8E RID: 11406
	public Translate.Phrase phraseChangeBetAmount;

	// Token: 0x04002C8F RID: 11407
	public Translate.Phrase phraseBet;

	// Token: 0x04002C90 RID: 11408
	public Translate.Phrase phraseBetAdd;

	// Token: 0x04002C91 RID: 11409
	public Translate.Phrase phraseAllIn;

	// Token: 0x04002C92 RID: 11410
	public GameObject amountChangeRoot;

	// Token: 0x04002C93 RID: 11411
	public RustText amountChangeText;

	// Token: 0x04002C94 RID: 11412
	public Color colourNeutralUI;

	// Token: 0x04002C95 RID: 11413
	public Color colourGoodUI;

	// Token: 0x04002C96 RID: 11414
	public Color colourBadUI;

	// Token: 0x04002C97 RID: 11415
	[SerializeField]
	private CanvasGroup timerCanvas;

	// Token: 0x04002C98 RID: 11416
	[SerializeField]
	private RustSlider timerSlider;

	// Token: 0x04002C99 RID: 11417
	[SerializeField]
	private UIChat chat;

	// Token: 0x04002C9A RID: 11418
	[SerializeField]
	private HudElement Hunger;

	// Token: 0x04002C9B RID: 11419
	[SerializeField]
	private HudElement Thirst;

	// Token: 0x04002C9C RID: 11420
	[SerializeField]
	private HudElement Health;

	// Token: 0x04002C9D RID: 11421
	[SerializeField]
	private HudElement PendingHealth;

	// Token: 0x04002C9E RID: 11422
	public Sprite cardNone;

	// Token: 0x04002C9F RID: 11423
	public Sprite cardBackLarge;

	// Token: 0x04002CA0 RID: 11424
	public Sprite cardBackSmall;

	// Token: 0x04002CA1 RID: 11425
	private static Sprite cardBackLargeStatic;

	// Token: 0x04002CA2 RID: 11426
	private static Sprite cardBackSmallStatic;

	// Token: 0x04002CA3 RID: 11427
	[SerializeField]
	private TexasHoldEmUI texasHoldEmUI;

	// Token: 0x04002CA4 RID: 11428
	[SerializeField]
	private BlackjackUI blackjackUI;

	// Token: 0x02000E8A RID: 3722
	[Serializable]
	public class PlayingCardImage
	{
		// Token: 0x04004C74 RID: 19572
		public Rank rank;

		// Token: 0x04004C75 RID: 19573
		public Suit suit;

		// Token: 0x04004C76 RID: 19574
		public Sprite image;

		// Token: 0x04004C77 RID: 19575
		public Sprite imageSmall;

		// Token: 0x04004C78 RID: 19576
		public Sprite imageTransparent;
	}

	// Token: 0x02000E8B RID: 3723
	[Serializable]
	public class InfoTextUI
	{
		// Token: 0x04004C79 RID: 19577
		public GameObject gameObj;

		// Token: 0x04004C7A RID: 19578
		public RustText rustText;

		// Token: 0x04004C7B RID: 19579
		public Image background;

		// Token: 0x02000FE6 RID: 4070
		public enum Attitude
		{
			// Token: 0x040051A0 RID: 20896
			Neutral,
			// Token: 0x040051A1 RID: 20897
			Good,
			// Token: 0x040051A2 RID: 20898
			Bad
		}
	}

	// Token: 0x02000E8C RID: 3724
	public interface ICardGameSubUI
	{
		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x060052F4 RID: 21236
		int DynamicBetAmount { get; }

		// Token: 0x060052F5 RID: 21237
		void UpdateInGameUI(CardGameUI ui, CardGameController game);

		// Token: 0x060052F6 RID: 21238
		string GetSecondaryInfo(CardGameUI ui, CardGameController game, out CardGameUI.InfoTextUI.Attitude attitude);

		// Token: 0x060052F7 RID: 21239
		void UpdateInGameUI_NoPlayer(CardGameUI ui);
	}
}
