using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AFF RID: 2815
	public class StackOfCards
	{
		// Token: 0x06004484 RID: 17540 RVA: 0x00191B74 File Offset: 0x0018FD74
		public StackOfCards(int numDecks)
		{
			this.cards = new List<PlayingCard>(52 * numDecks);
			for (int i = 0; i < numDecks; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 13; k++)
					{
						this.cards.Add(PlayingCard.GetCard(j, k));
					}
				}
			}
			this.ShuffleDeck();
		}

		// Token: 0x06004485 RID: 17541 RVA: 0x00191BD4 File Offset: 0x0018FDD4
		public bool TryTakeCard(out PlayingCard card)
		{
			if (this.cards.Count == 0)
			{
				card = null;
				return false;
			}
			card = this.cards[this.cards.Count - 1];
			this.cards.RemoveAt(this.cards.Count - 1);
			return true;
		}

		// Token: 0x06004486 RID: 17542 RVA: 0x00191C26 File Offset: 0x0018FE26
		public void AddCard(PlayingCard card)
		{
			this.cards.Insert(0, card);
		}

		// Token: 0x06004487 RID: 17543 RVA: 0x00191C38 File Offset: 0x0018FE38
		public void ShuffleDeck()
		{
			int i = this.cards.Count;
			while (i > 1)
			{
				i--;
				int num = UnityEngine.Random.Range(0, i);
				PlayingCard playingCard = this.cards[num];
				this.cards[num] = this.cards[i];
				this.cards[i] = playingCard;
			}
		}

		// Token: 0x06004488 RID: 17544 RVA: 0x00191C98 File Offset: 0x0018FE98
		public void Print()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Cards in the deck: ");
			foreach (PlayingCard playingCard in this.cards)
			{
				stringBuilder.AppendLine(playingCard.Rank + " of " + playingCard.Suit);
			}
			Debug.Log(stringBuilder.ToString());
		}

		// Token: 0x04003D1C RID: 15644
		private List<PlayingCard> cards;
	}
}
