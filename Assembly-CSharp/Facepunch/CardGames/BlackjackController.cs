using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AFB RID: 2811
	public class BlackjackController : CardGameController
	{
		// Token: 0x060043C4 RID: 17348 RVA: 0x0018F0BE File Offset: 0x0018D2BE
		public BlackjackController(BaseCardGameEntity owner)
			: base(owner)
		{
		}

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x060043C5 RID: 17349 RVA: 0x0000441C File Offset: 0x0000261C
		public override int MinPlayers
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x060043C6 RID: 17350 RVA: 0x000219AE File Offset: 0x0001FBAE
		public override int MinBuyIn
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x060043C7 RID: 17351 RVA: 0x0018F0DE File Offset: 0x0018D2DE
		public override int MaxBuyIn
		{
			get
			{
				return int.MaxValue;
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x060043C8 RID: 17352 RVA: 0x0018F0E5 File Offset: 0x0018D2E5
		public override int MinToPlay
		{
			get
			{
				return this.MinBuyIn;
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x060043C9 RID: 17353 RVA: 0x0000441C File Offset: 0x0000261C
		public override int EndRoundDelay
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x060043CA RID: 17354 RVA: 0x0018F0ED File Offset: 0x0018D2ED
		public override int TimeBetweenRounds
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x060043CB RID: 17355 RVA: 0x0018F0F0 File Offset: 0x0018D2F0
		// (set) Token: 0x060043CC RID: 17356 RVA: 0x0018F0F8 File Offset: 0x0018D2F8
		public BlackjackController.BlackjackInputOption LastAction { get; private set; }

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x060043CD RID: 17357 RVA: 0x0018F101 File Offset: 0x0018D301
		// (set) Token: 0x060043CE RID: 17358 RVA: 0x0018F109 File Offset: 0x0018D309
		public ulong LastActionTarget { get; private set; }

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x060043CF RID: 17359 RVA: 0x0018F112 File Offset: 0x0018D312
		// (set) Token: 0x060043D0 RID: 17360 RVA: 0x0018F11A File Offset: 0x0018D31A
		public int LastActionValue { get; private set; }

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x060043D1 RID: 17361 RVA: 0x0018F124 File Offset: 0x0018D324
		public bool AllBetsPlaced
		{
			get
			{
				if (!base.HasRoundInProgressOrEnding)
				{
					return false;
				}
				using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.betThisRound == 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x060043D2 RID: 17362 RVA: 0x00007A44 File Offset: 0x00005C44
		protected override int GetFirstPlayerRelIndex(bool startOfRound)
		{
			return 0;
		}

		// Token: 0x060043D3 RID: 17363 RVA: 0x0018F184 File Offset: 0x0018D384
		public override List<PlayingCard> GetTableCards()
		{
			return this.dealerCards;
		}

		// Token: 0x060043D4 RID: 17364 RVA: 0x0018F18C File Offset: 0x0018D38C
		public void InputsToList(int availableInputs, List<BlackjackController.BlackjackInputOption> result)
		{
			foreach (BlackjackController.BlackjackInputOption blackjackInputOption in (BlackjackController.BlackjackInputOption[])Enum.GetValues(typeof(BlackjackController.BlackjackInputOption)))
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.None && (availableInputs & (int)blackjackInputOption) == (int)blackjackInputOption)
				{
					result.Add(blackjackInputOption);
				}
			}
		}

		// Token: 0x060043D5 RID: 17365 RVA: 0x0018F1D0 File Offset: 0x0018D3D0
		public bool WaitingForOtherPlayers(CardPlayerData pData)
		{
			if (!pData.HasUserInCurrentRound)
			{
				return false;
			}
			if (base.State == CardGameController.CardGameState.InGameRound && !pData.HasAvailableInputs)
			{
				foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
				{
					if (cardPlayerData != pData && cardPlayerData.HasAvailableInputs)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060043D6 RID: 17366 RVA: 0x0018F244 File Offset: 0x0018D444
		public int GetCardsValue(List<PlayingCard> cards, BlackjackController.CardsValueMode mode)
		{
			int num = 0;
			foreach (PlayingCard playingCard in cards)
			{
				if (!playingCard.IsUnknownCard)
				{
					num += this.GetCardValue(playingCard, mode);
					if (playingCard.Rank == Rank.Ace)
					{
						mode = BlackjackController.CardsValueMode.Low;
					}
				}
			}
			return num;
		}

		// Token: 0x060043D7 RID: 17367 RVA: 0x0018F2B0 File Offset: 0x0018D4B0
		public int GetOptimalCardsValue(List<PlayingCard> cards)
		{
			int cardsValue = this.GetCardsValue(cards, BlackjackController.CardsValueMode.Low);
			int cardsValue2 = this.GetCardsValue(cards, BlackjackController.CardsValueMode.High);
			if (cardsValue2 <= 21)
			{
				return cardsValue2;
			}
			return cardsValue;
		}

		// Token: 0x060043D8 RID: 17368 RVA: 0x0018F2D8 File Offset: 0x0018D4D8
		public int GetCardValue(PlayingCard card, BlackjackController.CardsValueMode mode)
		{
			int rank = (int)card.Rank;
			if (rank <= 8)
			{
				return rank + 2;
			}
			if (rank <= 11)
			{
				return 10;
			}
			if (mode != BlackjackController.CardsValueMode.Low)
			{
				return 11;
			}
			return 1;
		}

		// Token: 0x060043D9 RID: 17369 RVA: 0x0018F303 File Offset: 0x0018D503
		public bool Has21(List<PlayingCard> cards)
		{
			return this.GetOptimalCardsValue(cards) == 21;
		}

		// Token: 0x060043DA RID: 17370 RVA: 0x0018F310 File Offset: 0x0018D510
		public bool HasBlackjack(List<PlayingCard> cards)
		{
			return this.GetCardsValue(cards, BlackjackController.CardsValueMode.High) == 21 && cards.Count == 2;
		}

		// Token: 0x060043DB RID: 17371 RVA: 0x0018F329 File Offset: 0x0018D529
		public bool HasBusted(List<PlayingCard> cards)
		{
			return this.GetCardsValue(cards, BlackjackController.CardsValueMode.Low) > 21;
		}

		// Token: 0x060043DC RID: 17372 RVA: 0x0018F338 File Offset: 0x0018D538
		private bool CanSplit(CardPlayerDataBlackjack pData)
		{
			if (pData.Cards.Count != 2)
			{
				return false;
			}
			if (this.HasSplit(pData))
			{
				return false;
			}
			int betThisRound = pData.betThisRound;
			return pData.GetScrapAmount() >= betThisRound && this.GetCardValue(pData.Cards[0], BlackjackController.CardsValueMode.Low) == this.GetCardValue(pData.Cards[1], BlackjackController.CardsValueMode.Low);
		}

		// Token: 0x060043DD RID: 17373 RVA: 0x0018F39C File Offset: 0x0018D59C
		private bool HasAnyAces(List<PlayingCard> cards)
		{
			using (List<PlayingCard>.Enumerator enumerator = cards.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Rank == Rank.Ace)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060043DE RID: 17374 RVA: 0x0018F3F4 File Offset: 0x0018D5F4
		private bool CanDoubleDown(CardPlayerDataBlackjack pData)
		{
			if (pData.Cards.Count != 2)
			{
				return false;
			}
			if (this.HasAnyAces(pData.Cards))
			{
				return false;
			}
			int betThisRound = pData.betThisRound;
			return pData.GetScrapAmount() >= betThisRound;
		}

		// Token: 0x060043DF RID: 17375 RVA: 0x0018F434 File Offset: 0x0018D634
		private bool CanTakeInsurance(CardPlayerDataBlackjack pData)
		{
			if (this.dealerCards.Count != 2)
			{
				return false;
			}
			if (this.dealerCards[1].Rank != Rank.Ace)
			{
				return false;
			}
			if (pData.insuranceBetThisRound > 0)
			{
				return false;
			}
			int num = Mathf.FloorToInt((float)pData.betThisRound / 2f);
			return pData.GetScrapAmount() >= num;
		}

		// Token: 0x060043E0 RID: 17376 RVA: 0x0018F492 File Offset: 0x0018D692
		private bool HasSplit(CardPlayerDataBlackjack pData)
		{
			return pData.SplitCards.Count > 0;
		}

		// Token: 0x060043E1 RID: 17377 RVA: 0x0018F4A2 File Offset: 0x0018D6A2
		protected override CardPlayerData GetNewCardPlayerData(int mountIndex)
		{
			if (base.IsServer)
			{
				return new CardPlayerDataBlackjack(base.ScrapItemID, new Func<int, StorageContainer>(base.Owner.GetPlayerStorage), mountIndex, base.IsServer);
			}
			return new CardPlayerDataBlackjack(mountIndex, base.IsServer);
		}

		// Token: 0x060043E2 RID: 17378 RVA: 0x0018F4DC File Offset: 0x0018D6DC
		public bool TryGetCardPlayerDataBlackjack(int index, out CardPlayerDataBlackjack cpBlackjack)
		{
			CardPlayerData cardPlayerData;
			bool flag = base.TryGetCardPlayerData(index, out cardPlayerData);
			cpBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
			return flag;
		}

		// Token: 0x060043E3 RID: 17379 RVA: 0x0018F4FA File Offset: 0x0018D6FA
		public int ResultsToInt(BlackjackController.BlackjackRoundResult mainResult, BlackjackController.BlackjackRoundResult splitResult, int insurancePayout)
		{
			return (int)(mainResult + (int)((BlackjackController.BlackjackRoundResult)10 * splitResult) + 100 * insurancePayout);
		}

		// Token: 0x060043E4 RID: 17380 RVA: 0x0018F507 File Offset: 0x0018D707
		public void ResultsFromInt(int result, out BlackjackController.BlackjackRoundResult mainResult, out BlackjackController.BlackjackRoundResult splitResult, out int insurancePayout)
		{
			mainResult = (BlackjackController.BlackjackRoundResult)(result % 10);
			splitResult = (BlackjackController.BlackjackRoundResult)(result / 10 % 10);
			insurancePayout = (result - (int)mainResult - (int)splitResult) / 100;
		}

		// Token: 0x060043E5 RID: 17381 RVA: 0x0018F528 File Offset: 0x0018D728
		public override void Save(CardGame syncData)
		{
			syncData.blackjack = Pool.Get<CardGame.Blackjack>();
			syncData.blackjack.dealerCards = Pool.GetList<int>();
			syncData.lastActionId = (int)this.LastAction;
			syncData.lastActionTarget = this.LastActionTarget;
			syncData.lastActionValue = this.LastActionValue;
			for (int i = 0; i < this.dealerCards.Count; i++)
			{
				PlayingCard playingCard = this.dealerCards[i];
				if (base.HasActiveRound && i == 0)
				{
					syncData.blackjack.dealerCards.Add(-1);
				}
				else
				{
					syncData.blackjack.dealerCards.Add(playingCard.GetIndex());
				}
			}
			base.Save(syncData);
			this.ClearLastAction();
		}

		// Token: 0x060043E6 RID: 17382 RVA: 0x0018F5D8 File Offset: 0x0018D7D8
		private void EditorMakeRandomMove(CardPlayerDataBlackjack pdBlackjack)
		{
			List<BlackjackController.BlackjackInputOption> list = Pool.GetList<BlackjackController.BlackjackInputOption>();
			this.InputsToList(pdBlackjack.availableInputs, list);
			if (list.Count == 0)
			{
				Debug.Log("No moves currently available.");
				Pool.FreeList<BlackjackController.BlackjackInputOption>(ref list);
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = list[UnityEngine.Random.Range(0, list.Count)];
			if (this.AllBetsPlaced)
			{
				if (this.GetOptimalCardsValue(pdBlackjack.Cards) < 17 && list.Contains(BlackjackController.BlackjackInputOption.Hit))
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.Hit;
				}
				else if (list.Contains(BlackjackController.BlackjackInputOption.Stand))
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.Stand;
				}
			}
			else if (list.Contains(BlackjackController.BlackjackInputOption.SubmitBet))
			{
				blackjackInputOption = BlackjackController.BlackjackInputOption.SubmitBet;
			}
			if (list.Count > 0)
			{
				int num = 0;
				if (blackjackInputOption == BlackjackController.BlackjackInputOption.SubmitBet)
				{
					num = this.MinBuyIn;
				}
				Debug.Log(string.Concat(new object[] { pdBlackjack.UserID, " Taking random action: ", blackjackInputOption, " with value ", num }));
				base.ReceivedInputFromPlayer(pdBlackjack, (int)blackjackInputOption, true, num, true);
			}
			else
			{
				Debug.LogWarning(base.GetType().Name + ": No input options are available for the current player.");
			}
			Pool.FreeList<BlackjackController.BlackjackInputOption>(ref list);
		}

		// Token: 0x060043E7 RID: 17383 RVA: 0x0018F6E8 File Offset: 0x0018D8E8
		protected override int GetAvailableInputsForPlayer(CardPlayerData pData)
		{
			BlackjackController.BlackjackInputOption blackjackInputOption = BlackjackController.BlackjackInputOption.None;
			CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)pData;
			if (cardPlayerDataBlackjack == null || this.isWaitingBetweenTurns || cardPlayerDataBlackjack.hasCompletedTurn || !cardPlayerDataBlackjack.HasUserInCurrentRound)
			{
				return (int)blackjackInputOption;
			}
			if (!base.HasActiveRound)
			{
				return (int)blackjackInputOption;
			}
			if (this.AllBetsPlaced)
			{
				blackjackInputOption |= BlackjackController.BlackjackInputOption.Stand;
				if (!this.Has21(cardPlayerDataBlackjack.Cards))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Hit;
				}
				if (this.CanSplit(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Split;
				}
				if (this.CanDoubleDown(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.DoubleDown;
				}
				if (this.CanTakeInsurance(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Insurance;
				}
			}
			else
			{
				blackjackInputOption |= BlackjackController.BlackjackInputOption.SubmitBet;
				blackjackInputOption |= BlackjackController.BlackjackInputOption.MaxBet;
			}
			return (int)blackjackInputOption;
		}

		// Token: 0x060043E8 RID: 17384 RVA: 0x0018F778 File Offset: 0x0018D978
		protected override void SubEndGameplay()
		{
			this.dealerCards.Clear();
		}

		// Token: 0x060043E9 RID: 17385 RVA: 0x0018F788 File Offset: 0x0018D988
		protected override void SubEndRound()
		{
			BlackjackController.<>c__DisplayClass59_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dealerCardsVal = this.GetOptimalCardsValue(this.dealerCards);
			if (CS$<>8__locals1.dealerCardsVal > 21)
			{
				CS$<>8__locals1.dealerCardsVal = 0;
			}
			base.resultInfo.winningScore = CS$<>8__locals1.dealerCardsVal;
			if (base.NumPlayersInCurrentRound() == 0)
			{
				base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
				return;
			}
			CS$<>8__locals1.dealerHasBlackjack = this.HasBlackjack(this.dealerCards);
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				int num = 0;
				int num2;
				BlackjackController.BlackjackRoundResult blackjackRoundResult = this.<SubEndRound>g__CheckResult|59_0(cardPlayerDataBlackjack.Cards, cardPlayerDataBlackjack.betThisRound, out num2, ref CS$<>8__locals1);
				num += num2;
				BlackjackController.BlackjackRoundResult blackjackRoundResult2 = this.<SubEndRound>g__CheckResult|59_0(cardPlayerDataBlackjack.SplitCards, cardPlayerDataBlackjack.splitBetThisRound, out num2, ref CS$<>8__locals1);
				num += num2;
				int num3 = cardPlayerDataBlackjack.betThisRound + cardPlayerDataBlackjack.splitBetThisRound + cardPlayerDataBlackjack.insuranceBetThisRound;
				int num4 = 0;
				if (CS$<>8__locals1.dealerHasBlackjack && cardPlayerDataBlackjack.insuranceBetThisRound > 0)
				{
					int num5 = Mathf.FloorToInt((float)cardPlayerDataBlackjack.insuranceBetThisRound * 3f);
					num += num5;
					num4 = num5;
				}
				int num6 = this.ResultsToInt(blackjackRoundResult, blackjackRoundResult2, num4);
				this.AddRoundResult(cardPlayerDataBlackjack, num - num3, num6);
				this.PayOut(cardPlayerDataBlackjack, num);
			}
			base.ClearPot();
			base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
		}

		// Token: 0x060043EA RID: 17386 RVA: 0x0018F918 File Offset: 0x0018DB18
		private int PayOut(CardPlayerData pData, int winnings)
		{
			if (winnings == 0)
			{
				return 0;
			}
			StorageContainer storage = pData.GetStorage();
			if (storage == null)
			{
				return 0;
			}
			storage.inventory.AddItem(base.Owner.scrapItemDef, winnings, 0UL, global::ItemContainer.LimitStack.None);
			return winnings;
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x0018F957 File Offset: 0x0018DB57
		protected override void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData)
		{
			base.ReceivedInputFromPlayer(pData, 128, true, 0, false);
		}

		// Token: 0x060043EC RID: 17388 RVA: 0x0018F968 File Offset: 0x0018DB68
		protected override void SubReceivedInputFromPlayer(CardPlayerData pData, int input, int value, bool countAsAction)
		{
			if (!Enum.IsDefined(typeof(BlackjackController.BlackjackInputOption), input))
			{
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = (BlackjackController.BlackjackInputOption)input;
			CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)pData;
			if (!base.HasActiveRound)
			{
				this.LastActionTarget = pData.UserID;
				this.LastAction = blackjackInputOption;
				this.LastActionValue = 0;
				return;
			}
			int num = 0;
			if (this.AllBetsPlaced)
			{
				this.DoInRoundPlayerInput(cardPlayerDataBlackjack, ref blackjackInputOption, ref num);
			}
			else
			{
				this.DoBettingPhasePlayerInput(cardPlayerDataBlackjack, value, countAsAction, ref blackjackInputOption, ref num);
			}
			this.LastActionTarget = pData.UserID;
			this.LastAction = blackjackInputOption;
			this.LastActionValue = num;
			if (base.NumPlayersInCurrentRound() == 0)
			{
				base.EndGameplay();
				return;
			}
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			base.StartTurnTimer(pData, this.MaxTurnTime);
			base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060043ED RID: 17389 RVA: 0x0018FA30 File Offset: 0x0018DC30
		private void DoInRoundPlayerInput(CardPlayerDataBlackjack pdBlackjack, ref BlackjackController.BlackjackInputOption selectedMove, ref int selectedMoveValue)
		{
			if (selectedMove != BlackjackController.BlackjackInputOption.Abandon && (pdBlackjack.availableInputs & (int)selectedMove) != (int)selectedMove)
			{
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = selectedMove;
			if (blackjackInputOption <= BlackjackController.BlackjackInputOption.Split)
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Hit)
				{
					if (blackjackInputOption != BlackjackController.BlackjackInputOption.Stand)
					{
						if (blackjackInputOption == BlackjackController.BlackjackInputOption.Split)
						{
							PlayingCard playingCard = pdBlackjack.Cards[1];
							bool flag = playingCard.Rank == Rank.Ace;
							pdBlackjack.SplitCards.Add(playingCard);
							pdBlackjack.Cards.Remove(playingCard);
							PlayingCard playingCard2;
							this.cardStack.TryTakeCard(out playingCard2);
							pdBlackjack.Cards.Add(playingCard2);
							this.cardStack.TryTakeCard(out playingCard2);
							pdBlackjack.SplitCards.Add(playingCard2);
							selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.betThisRound, BlackjackController.BetType.Split);
							if (flag)
							{
								pdBlackjack.SetHasCompletedTurn(true);
							}
						}
					}
					else if (!pdBlackjack.TrySwitchToSplitHand())
					{
						pdBlackjack.SetHasCompletedTurn(true);
					}
				}
				else
				{
					PlayingCard playingCard3;
					this.cardStack.TryTakeCard(out playingCard3);
					pdBlackjack.Cards.Add(playingCard3);
				}
			}
			else if (blackjackInputOption != BlackjackController.BlackjackInputOption.DoubleDown)
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Insurance)
				{
					if (blackjackInputOption == BlackjackController.BlackjackInputOption.Abandon)
					{
						pdBlackjack.LeaveCurrentRound(false, true);
					}
				}
				else
				{
					int num = Mathf.FloorToInt((float)pdBlackjack.betThisRound / 2f);
					selectedMoveValue = this.TryMakeBet(pdBlackjack, num, BlackjackController.BetType.Insurance);
				}
			}
			else
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.betThisRound, BlackjackController.BetType.Main);
				PlayingCard playingCard4;
				this.cardStack.TryTakeCard(out playingCard4);
				pdBlackjack.Cards.Add(playingCard4);
				if (!pdBlackjack.TrySwitchToSplitHand())
				{
					pdBlackjack.SetHasCompletedTurn(true);
				}
			}
			if (this.HasBusted(pdBlackjack.Cards) && !pdBlackjack.TrySwitchToSplitHand())
			{
				pdBlackjack.SetHasCompletedTurn(true);
			}
			if (this.Has21(pdBlackjack.Cards) && !this.CanTakeInsurance(pdBlackjack) && !this.CanDoubleDown(pdBlackjack) && !this.CanSplit(pdBlackjack) && !pdBlackjack.TrySwitchToSplitHand())
			{
				pdBlackjack.SetHasCompletedTurn(true);
			}
		}

		// Token: 0x060043EE RID: 17390 RVA: 0x0018FC00 File Offset: 0x0018DE00
		private void DoBettingPhasePlayerInput(CardPlayerDataBlackjack pdBlackjack, int value, bool countAsAction, ref BlackjackController.BlackjackInputOption selectedMove, ref int selectedMoveValue)
		{
			if (selectedMove != BlackjackController.BlackjackInputOption.Abandon && (pdBlackjack.availableInputs & (int)selectedMove) != (int)selectedMove)
			{
				return;
			}
			if (selectedMove == BlackjackController.BlackjackInputOption.SubmitBet)
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, value, BlackjackController.BetType.Main);
				if (countAsAction)
				{
					pdBlackjack.SetHasCompletedTurn(true);
					return;
				}
			}
			else if (selectedMove == BlackjackController.BlackjackInputOption.MaxBet)
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, BlackjackMachine.maxbet, BlackjackController.BetType.Main);
				if (countAsAction)
				{
					pdBlackjack.SetHasCompletedTurn(true);
					return;
				}
			}
			else if (selectedMove == BlackjackController.BlackjackInputOption.Abandon)
			{
				pdBlackjack.LeaveCurrentRound(false, true);
			}
		}

		// Token: 0x060043EF RID: 17391 RVA: 0x0018FC7C File Offset: 0x0018DE7C
		private int TryMakeBet(CardPlayerDataBlackjack pdBlackjack, int maxAmount, BlackjackController.BetType betType)
		{
			int num = base.TryMoveToPotStorage(pdBlackjack, maxAmount);
			switch (betType)
			{
			case BlackjackController.BetType.Main:
				pdBlackjack.betThisTurn += num;
				pdBlackjack.betThisRound += num;
				break;
			case BlackjackController.BetType.Split:
				pdBlackjack.splitBetThisRound += num;
				break;
			case BlackjackController.BetType.Insurance:
				pdBlackjack.insuranceBetThisRound += num;
				break;
			}
			return num;
		}

		// Token: 0x060043F0 RID: 17392 RVA: 0x0018FCE4 File Offset: 0x0018DEE4
		protected override void SubStartRound()
		{
			this.dealerCards.Clear();
			this.cardStack = new StackOfCards(6);
			this.ClearLastAction();
			base.ServerPlaySound(CardGameSounds.SoundType.Shuffle);
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				cardPlayerDataBlackjack.EnableSendingCards();
				cardPlayerDataBlackjack.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerDataBlackjack);
				base.StartTurnTimer(cardPlayerDataBlackjack, this.MaxTurnTime);
			}
		}

		// Token: 0x060043F1 RID: 17393 RVA: 0x0018FD74 File Offset: 0x0018DF74
		protected override void OnTurnTimeout(CardPlayerData pData)
		{
			if (pData.HasUserInCurrentRound && !pData.hasCompletedTurn)
			{
				BlackjackController.BlackjackInputOption blackjackInputOption = BlackjackController.BlackjackInputOption.Abandon;
				int num = 0;
				if (this.AllBetsPlaced)
				{
					if ((pData.availableInputs & 4) == 4)
					{
						blackjackInputOption = BlackjackController.BlackjackInputOption.Stand;
						base.ReceivedInputFromPlayer(pData, 4, true, 0, false);
					}
				}
				else if ((pData.availableInputs & 1) == 1 && pData.GetScrapAmount() >= this.MinBuyIn)
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.SubmitBet;
					num = this.MinBuyIn;
				}
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Abandon)
				{
					base.ReceivedInputFromPlayer(pData, (int)blackjackInputOption, true, num, false);
					return;
				}
				blackjackInputOption = BlackjackController.BlackjackInputOption.Abandon;
				base.ReceivedInputFromPlayer(pData, (int)blackjackInputOption, true, 0, false);
				pData.ClearAllData();
				if (base.HasActiveRound && base.NumPlayersInCurrentRound() < this.MinPlayers)
				{
					base.BeginRoundEnd();
				}
				if (pData.HasUserInGame)
				{
					base.Owner.ClientRPC<ulong>(null, "ClientOnPlayerLeft", pData.UserID);
				}
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x060043F2 RID: 17394 RVA: 0x0018FE5C File Offset: 0x0018E05C
		protected override void StartNextCycle()
		{
			base.StartNextCycle();
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				base.StartTurnTimer(cardPlayerDataBlackjack, this.MaxTurnTime);
			}
			base.UpdateAllAvailableInputs();
			base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060043F3 RID: 17395 RVA: 0x0018FEDC File Offset: 0x0018E0DC
		protected override bool ShouldEndCycle()
		{
			using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.hasCompletedTurn)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060043F4 RID: 17396 RVA: 0x0018FF30 File Offset: 0x0018E130
		protected override void EndCycle()
		{
			CardPlayerData[] playerData = base.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].SetHasCompletedTurn(false);
			}
			if (this.dealerCards.Count == 0)
			{
				this.DealInitialCards();
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				base.QueueNextCycleInvoke();
				return;
			}
			bool flag = true;
			bool flag2 = true;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				if (!this.HasBusted(cardPlayerDataBlackjack.Cards))
				{
					flag = false;
				}
				if (!this.HasBlackjack(cardPlayerDataBlackjack.Cards))
				{
					flag2 = false;
				}
				if (cardPlayerDataBlackjack.SplitCards.Count > 0)
				{
					if (!this.HasBusted(cardPlayerDataBlackjack.SplitCards))
					{
						flag = false;
					}
					if (!this.HasBlackjack(cardPlayerDataBlackjack.SplitCards))
					{
						flag2 = false;
					}
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
			base.ServerPlaySound(CardGameSounds.SoundType.Draw);
			if (base.NumPlayersInCurrentRound() > 0 && !flag && !flag2)
			{
				base.Owner.Invoke(new Action(this.DealerPlayInvoke), 1f);
				base.BeginRoundEnd();
				return;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060043F5 RID: 17397 RVA: 0x00190064 File Offset: 0x0018E264
		private void DealerPlayInvoke()
		{
			int cardsValue = this.GetCardsValue(this.dealerCards, BlackjackController.CardsValueMode.High);
			if (this.GetCardsValue(this.dealerCards, BlackjackController.CardsValueMode.Low) < 17 && (cardsValue < 17 || cardsValue > 21))
			{
				PlayingCard playingCard;
				this.cardStack.TryTakeCard(out playingCard);
				this.dealerCards.Add(playingCard);
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				base.Owner.Invoke(new Action(this.DealerPlayInvoke), 1f);
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060043F6 RID: 17398 RVA: 0x001900EC File Offset: 0x0018E2EC
		private void DealInitialCards()
		{
			if (!base.HasActiveRound)
			{
				return;
			}
			PlayingCard playingCard;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				this.cardStack.TryTakeCard(out playingCard);
				cardPlayerData.Cards.Add(playingCard);
			}
			this.cardStack.TryTakeCard(out playingCard);
			this.dealerCards.Add(playingCard);
			foreach (CardPlayerData cardPlayerData2 in base.PlayersInRound())
			{
				this.cardStack.TryTakeCard(out playingCard);
				cardPlayerData2.Cards.Add(playingCard);
				if (this.HasBlackjack(cardPlayerData2.Cards))
				{
					cardPlayerData2.SetHasCompletedTurn(true);
				}
			}
			this.cardStack.TryTakeCard(out playingCard);
			this.dealerCards.Add(playingCard);
		}

		// Token: 0x060043F7 RID: 17399 RVA: 0x001901EC File Offset: 0x0018E3EC
		private void ClearLastAction()
		{
			this.LastAction = BlackjackController.BlackjackInputOption.None;
			this.LastActionTarget = 0UL;
			this.LastActionValue = 0;
		}

		// Token: 0x060043F8 RID: 17400 RVA: 0x00190204 File Offset: 0x0018E404
		[CompilerGenerated]
		private BlackjackController.BlackjackRoundResult <SubEndRound>g__CheckResult|59_0(List<PlayingCard> cards, int betAmount, out int winnings, ref BlackjackController.<>c__DisplayClass59_0 A_4)
		{
			if (cards.Count == 0)
			{
				winnings = 0;
				return BlackjackController.BlackjackRoundResult.None;
			}
			int optimalCardsValue = this.GetOptimalCardsValue(cards);
			if (optimalCardsValue > 21)
			{
				winnings = 0;
				return BlackjackController.BlackjackRoundResult.Bust;
			}
			if (optimalCardsValue > base.resultInfo.winningScore)
			{
				base.resultInfo.winningScore = optimalCardsValue;
			}
			BlackjackController.BlackjackRoundResult blackjackRoundResult = BlackjackController.BlackjackRoundResult.Loss;
			bool flag = this.HasBlackjack(cards);
			if (A_4.dealerHasBlackjack)
			{
				if (flag)
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.Standoff;
				}
			}
			else if (optimalCardsValue > A_4.dealerCardsVal)
			{
				blackjackRoundResult = (flag ? BlackjackController.BlackjackRoundResult.BlackjackWin : BlackjackController.BlackjackRoundResult.Win);
			}
			else if (optimalCardsValue == A_4.dealerCardsVal)
			{
				if (flag)
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.BlackjackWin;
				}
				else
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.Standoff;
				}
			}
			if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.BlackjackWin)
			{
				winnings = Mathf.FloorToInt((float)betAmount * 2.5f);
			}
			else if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.Win)
			{
				winnings = Mathf.FloorToInt((float)betAmount * 2f);
			}
			else if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.Standoff)
			{
				winnings = betAmount;
			}
			else
			{
				winnings = 0;
			}
			return blackjackRoundResult;
		}

		// Token: 0x04003CF5 RID: 15605
		public List<PlayingCard> dealerCards = new List<PlayingCard>();

		// Token: 0x04003CF6 RID: 15606
		public const float BLACKJACK_PAYOUT_RATIO = 1.5f;

		// Token: 0x04003CF7 RID: 15607
		public const float INSURANCE_PAYOUT_RATIO = 2f;

		// Token: 0x04003CF8 RID: 15608
		private const float DEALER_MOVE_TIME = 1f;

		// Token: 0x04003CFC RID: 15612
		private const int NUM_DECKS = 6;

		// Token: 0x04003CFD RID: 15613
		private StackOfCards cardStack = new StackOfCards(6);

		// Token: 0x02000F88 RID: 3976
		[Flags]
		public enum BlackjackInputOption
		{
			// Token: 0x0400505D RID: 20573
			None = 0,
			// Token: 0x0400505E RID: 20574
			SubmitBet = 1,
			// Token: 0x0400505F RID: 20575
			Hit = 2,
			// Token: 0x04005060 RID: 20576
			Stand = 4,
			// Token: 0x04005061 RID: 20577
			Split = 8,
			// Token: 0x04005062 RID: 20578
			DoubleDown = 16,
			// Token: 0x04005063 RID: 20579
			Insurance = 32,
			// Token: 0x04005064 RID: 20580
			MaxBet = 64,
			// Token: 0x04005065 RID: 20581
			Abandon = 128
		}

		// Token: 0x02000F89 RID: 3977
		public enum BlackjackRoundResult
		{
			// Token: 0x04005067 RID: 20583
			None,
			// Token: 0x04005068 RID: 20584
			Bust,
			// Token: 0x04005069 RID: 20585
			Loss,
			// Token: 0x0400506A RID: 20586
			Standoff,
			// Token: 0x0400506B RID: 20587
			Win,
			// Token: 0x0400506C RID: 20588
			BlackjackWin
		}

		// Token: 0x02000F8A RID: 3978
		public enum CardsValueMode
		{
			// Token: 0x0400506E RID: 20590
			Low,
			// Token: 0x0400506F RID: 20591
			High
		}

		// Token: 0x02000F8B RID: 3979
		private enum BetType
		{
			// Token: 0x04005071 RID: 20593
			Main,
			// Token: 0x04005072 RID: 20594
			Split,
			// Token: 0x04005073 RID: 20595
			Insurance
		}
	}
}
