using System;
using PokerEvaluator;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000B02 RID: 2818
	public class PlayingCard
	{
		// Token: 0x06004489 RID: 17545 RVA: 0x00191D28 File Offset: 0x0018FF28
		private PlayingCard(Suit suit, Rank rank)
		{
			this.IsUnknownCard = false;
			this.Suit = suit;
			this.Rank = rank;
		}

		// Token: 0x0600448A RID: 17546 RVA: 0x00191D45 File Offset: 0x0018FF45
		private PlayingCard()
		{
			this.IsUnknownCard = true;
			this.Suit = Suit.Spades;
			this.Rank = Rank.Two;
		}

		// Token: 0x0600448B RID: 17547 RVA: 0x00191D62 File Offset: 0x0018FF62
		public static PlayingCard GetCard(Suit suit, Rank rank)
		{
			return PlayingCard.GetCard((int)suit, (int)rank);
		}

		// Token: 0x0600448C RID: 17548 RVA: 0x00191D6B File Offset: 0x0018FF6B
		public static PlayingCard GetCard(int suit, int rank)
		{
			return PlayingCard.cards[suit * 13 + rank];
		}

		// Token: 0x0600448D RID: 17549 RVA: 0x00191D79 File Offset: 0x0018FF79
		public static PlayingCard GetCard(int index)
		{
			if (index == -1)
			{
				return PlayingCard.unknownCard;
			}
			return PlayingCard.cards[index];
		}

		// Token: 0x0600448E RID: 17550 RVA: 0x00191D8C File Offset: 0x0018FF8C
		public int GetIndex()
		{
			if (this.IsUnknownCard)
			{
				return -1;
			}
			return PlayingCard.GetIndex(this.Suit, this.Rank);
		}

		// Token: 0x0600448F RID: 17551 RVA: 0x00191DA9 File Offset: 0x0018FFA9
		public static int GetIndex(Suit suit, Rank rank)
		{
			return (int)(suit * (Suit)13 + (int)rank);
		}

		// Token: 0x06004490 RID: 17552 RVA: 0x00191DB4 File Offset: 0x0018FFB4
		public int GetPokerEvaluationValue()
		{
			if (this.IsUnknownCard)
			{
				Debug.LogWarning(base.GetType().Name + ": Called GetPokerEvaluationValue on unknown card.");
			}
			return Arrays.primes[(int)this.Rank] | (int)((int)this.Rank << 8) | this.GetPokerSuitCode() | (1 << (int)(16 + this.Rank));
		}

		// Token: 0x06004491 RID: 17553 RVA: 0x00191E10 File Offset: 0x00190010
		private int GetPokerSuitCode()
		{
			if (this.IsUnknownCard)
			{
				Debug.LogWarning(base.GetType().Name + ": Called GetPokerSuitCode on unknown card.");
			}
			switch (this.Suit)
			{
			case Suit.Spades:
				return 4096;
			case Suit.Hearts:
				return 8192;
			case Suit.Diamonds:
				return 16384;
			case Suit.Clubs:
				return 32768;
			default:
				return 4096;
			}
		}

		// Token: 0x06004492 RID: 17554 RVA: 0x00191E7C File Offset: 0x0019007C
		private static PlayingCard[] GenerateAllCards()
		{
			PlayingCard[] array = new PlayingCard[52];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					array[i * 13 + j] = new PlayingCard((Suit)i, (Rank)j);
				}
			}
			return array;
		}

		// Token: 0x04003D30 RID: 15664
		public readonly bool IsUnknownCard;

		// Token: 0x04003D31 RID: 15665
		public readonly Suit Suit;

		// Token: 0x04003D32 RID: 15666
		public readonly Rank Rank;

		// Token: 0x04003D33 RID: 15667
		public static PlayingCard[] cards = PlayingCard.GenerateAllCards();

		// Token: 0x04003D34 RID: 15668
		public static PlayingCard unknownCard = new PlayingCard();
	}
}
