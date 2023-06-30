using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Facepunch.CardGames
{
	// Token: 0x02000AFE RID: 2814
	public class CardPlayerDataBlackjack : CardPlayerData
	{
		// Token: 0x06004479 RID: 17529 RVA: 0x0019193A File Offset: 0x0018FB3A
		public CardPlayerDataBlackjack(int mountIndex, bool isServer)
			: base(mountIndex, isServer)
		{
			this.SplitCards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x0600447A RID: 17530 RVA: 0x0019194F File Offset: 0x0018FB4F
		public CardPlayerDataBlackjack(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer)
			: base(scrapItemID, getStorage, mountIndex, isServer)
		{
			this.SplitCards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x0600447B RID: 17531 RVA: 0x00191967 File Offset: 0x0018FB67
		public override void Dispose()
		{
			base.Dispose();
			Pool.FreeList<PlayingCard>(ref this.SplitCards);
		}

		// Token: 0x0600447C RID: 17532 RVA: 0x0019197A File Offset: 0x0018FB7A
		public override int GetTotalBetThisRound()
		{
			return this.betThisRound + this.splitBetThisRound + this.insuranceBetThisRound;
		}

		// Token: 0x0600447D RID: 17533 RVA: 0x00191990 File Offset: 0x0018FB90
		public override List<PlayingCard> GetSecondaryCards()
		{
			return this.SplitCards;
		}

		// Token: 0x0600447E RID: 17534 RVA: 0x00191998 File Offset: 0x0018FB98
		protected override void ClearPerRoundData()
		{
			base.ClearPerRoundData();
			this.SplitCards.Clear();
			this.splitBetThisRound = 0;
			this.insuranceBetThisRound = 0;
			this.playingSplitCards = false;
		}

		// Token: 0x0600447F RID: 17535 RVA: 0x001919C0 File Offset: 0x0018FBC0
		public override void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
		{
			if (!base.HasUserInCurrentRound)
			{
				return;
			}
			if (clearBets)
			{
				this.splitBetThisRound = 0;
				this.insuranceBetThisRound = 0;
			}
			base.LeaveCurrentRound(clearBets, leftRoundEarly);
		}

		// Token: 0x06004480 RID: 17536 RVA: 0x001919E4 File Offset: 0x0018FBE4
		public override void LeaveGame()
		{
			base.LeaveGame();
			if (!base.HasUserInGame)
			{
				return;
			}
			this.SplitCards.Clear();
		}

		// Token: 0x06004481 RID: 17537 RVA: 0x00191A00 File Offset: 0x0018FC00
		public override void Save(CardGame syncData)
		{
			base.Save(syncData);
			CardGame.BlackjackCardPlayer blackjackCardPlayer = Pool.Get<CardGame.BlackjackCardPlayer>();
			blackjackCardPlayer.splitCards = Pool.GetList<int>();
			foreach (PlayingCard playingCard in this.SplitCards)
			{
				blackjackCardPlayer.splitCards.Add(base.SendCardDetails ? playingCard.GetIndex() : (-1));
			}
			blackjackCardPlayer.splitBetThisRound = this.splitBetThisRound;
			blackjackCardPlayer.insuranceBetThisRound = this.insuranceBetThisRound;
			blackjackCardPlayer.playingSplitCards = this.playingSplitCards;
			if (syncData.blackjack.players == null)
			{
				syncData.blackjack.players = Pool.GetList<CardGame.BlackjackCardPlayer>();
			}
			syncData.blackjack.players.Add(blackjackCardPlayer);
		}

		// Token: 0x06004482 RID: 17538 RVA: 0x00191AD4 File Offset: 0x0018FCD4
		public bool TrySwitchToSplitHand()
		{
			if (this.SplitCards.Count > 0 && !this.playingSplitCards)
			{
				this.SwapSplitCardsWithMain();
				this.playingSplitCards = true;
				return true;
			}
			return false;
		}

		// Token: 0x06004483 RID: 17539 RVA: 0x00191AFC File Offset: 0x0018FCFC
		private void SwapSplitCardsWithMain()
		{
			List<PlayingCard> list = Pool.GetList<PlayingCard>();
			list.AddRange(this.Cards);
			this.Cards.Clear();
			this.Cards.AddRange(this.SplitCards);
			this.SplitCards.Clear();
			this.SplitCards.AddRange(list);
			Pool.FreeList<PlayingCard>(ref list);
			int betThisRound = this.betThisRound;
			int num = this.splitBetThisRound;
			this.splitBetThisRound = betThisRound;
			this.betThisRound = num;
		}

		// Token: 0x04003D18 RID: 15640
		public List<PlayingCard> SplitCards;

		// Token: 0x04003D19 RID: 15641
		public int splitBetThisRound;

		// Token: 0x04003D1A RID: 15642
		public int insuranceBetThisRound;

		// Token: 0x04003D1B RID: 15643
		public bool playingSplitCards;
	}
}
