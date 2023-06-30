using System;
using System.Collections.Generic;
using PokerEvaluator;
using ProtoBuf;
using Rust;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000B03 RID: 2819
	public class TexasHoldEmController : CardGameController
	{
		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06004494 RID: 17556 RVA: 0x0004E9D7 File Offset: 0x0004CBD7
		public override int MinPlayers
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06004495 RID: 17557 RVA: 0x000070B9 File Offset: 0x000052B9
		public override int MinBuyIn
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06004496 RID: 17558 RVA: 0x00191ED0 File Offset: 0x001900D0
		public override int MaxBuyIn
		{
			get
			{
				return 1000;
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06004497 RID: 17559 RVA: 0x00023862 File Offset: 0x00021A62
		public override int MinToPlay
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06004498 RID: 17560 RVA: 0x00191ED7 File Offset: 0x001900D7
		// (set) Token: 0x06004499 RID: 17561 RVA: 0x00191EDF File Offset: 0x001900DF
		public TexasHoldEmController.PokerInputOption LastAction { get; private set; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x0600449A RID: 17562 RVA: 0x00191EE8 File Offset: 0x001900E8
		// (set) Token: 0x0600449B RID: 17563 RVA: 0x00191EF0 File Offset: 0x001900F0
		public ulong LastActionTarget { get; private set; }

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x0600449C RID: 17564 RVA: 0x00191EF9 File Offset: 0x001900F9
		// (set) Token: 0x0600449D RID: 17565 RVA: 0x00191F01 File Offset: 0x00190101
		public int LastActionValue { get; private set; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x0600449E RID: 17566 RVA: 0x00191F0A File Offset: 0x0019010A
		// (set) Token: 0x0600449F RID: 17567 RVA: 0x00191F12 File Offset: 0x00190112
		public int BiggestRaiseThisTurn { get; private set; }

		// Token: 0x060044A0 RID: 17568 RVA: 0x00191F1B File Offset: 0x0019011B
		public TexasHoldEmController(BaseCardGameEntity owner)
			: base(owner)
		{
		}

		// Token: 0x060044A1 RID: 17569 RVA: 0x00191F3C File Offset: 0x0019013C
		public int GetCurrentBet()
		{
			int num = 0;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				num = Mathf.Max(num, cardPlayerData.betThisTurn);
			}
			return num;
		}

		// Token: 0x060044A2 RID: 17570 RVA: 0x00191F94 File Offset: 0x00190194
		public bool TryGetDealer(out CardPlayerData dealer)
		{
			return base.ToCardPlayerData(this.dealerIndex, true, out dealer);
		}

		// Token: 0x060044A3 RID: 17571 RVA: 0x00191FA4 File Offset: 0x001901A4
		public bool TryGetSmallBlind(out CardPlayerData smallBlind)
		{
			int num = ((base.NumPlayersInGame() < 3) ? this.dealerIndex : (this.dealerIndex + 1));
			return base.ToCardPlayerData(num, true, out smallBlind);
		}

		// Token: 0x060044A4 RID: 17572 RVA: 0x00191FD4 File Offset: 0x001901D4
		public bool TryGetBigBlind(out CardPlayerData bigBlind)
		{
			int num = ((base.NumPlayersInGame() < 3) ? (this.dealerIndex + 1) : (this.dealerIndex + 2));
			return base.ToCardPlayerData(num, true, out bigBlind);
		}

		// Token: 0x060044A5 RID: 17573 RVA: 0x00192008 File Offset: 0x00190208
		protected override int GetFirstPlayerRelIndex(bool startOfRound)
		{
			int num = base.NumPlayersInGame();
			if (startOfRound && num == 2)
			{
				return this.dealerIndex;
			}
			return (this.dealerIndex + 1) % num;
		}

		// Token: 0x060044A6 RID: 17574 RVA: 0x00192034 File Offset: 0x00190234
		public static ushort EvaluatePokerHand(List<PlayingCard> cards)
		{
			ushort num = 0;
			int[] array = new int[cards.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = cards[i].GetPokerEvaluationValue();
			}
			if (cards.Count == 5)
			{
				num = PokerLib.Eval5Hand(array);
			}
			else if (cards.Count == 7)
			{
				num = PokerLib.Eval7Hand(array);
			}
			else
			{
				Debug.LogError("Currently we can only evaluate five or seven card hands.");
			}
			return num;
		}

		// Token: 0x060044A7 RID: 17575 RVA: 0x0019209B File Offset: 0x0019029B
		public int GetCurrentMinRaise(CardPlayerData playerData)
		{
			return Mathf.Max(10, this.GetCurrentBet() - playerData.betThisTurn + this.BiggestRaiseThisTurn);
		}

		// Token: 0x060044A8 RID: 17576 RVA: 0x001920B8 File Offset: 0x001902B8
		public override List<PlayingCard> GetTableCards()
		{
			return this.communityCards;
		}

		// Token: 0x060044A9 RID: 17577 RVA: 0x001920C0 File Offset: 0x001902C0
		public void InputsToList(int availableInputs, List<TexasHoldEmController.PokerInputOption> result)
		{
			foreach (TexasHoldEmController.PokerInputOption pokerInputOption in (TexasHoldEmController.PokerInputOption[])Enum.GetValues(typeof(TexasHoldEmController.PokerInputOption)))
			{
				if (pokerInputOption != TexasHoldEmController.PokerInputOption.None && (availableInputs & (int)pokerInputOption) == (int)pokerInputOption)
				{
					result.Add(pokerInputOption);
				}
			}
		}

		// Token: 0x060044AA RID: 17578 RVA: 0x00192104 File Offset: 0x00190304
		protected override CardPlayerData GetNewCardPlayerData(int mountIndex)
		{
			if (base.IsServer)
			{
				return new CardPlayerData(base.ScrapItemID, new Func<int, StorageContainer>(base.Owner.GetPlayerStorage), mountIndex, base.IsServer);
			}
			return new CardPlayerData(mountIndex, base.IsServer);
		}

		// Token: 0x060044AB RID: 17579 RVA: 0x00192140 File Offset: 0x00190340
		public override void Save(CardGame syncData)
		{
			base.Save(syncData);
			syncData.texasHoldEm = Pool.Get<CardGame.TexasHoldEm>();
			syncData.texasHoldEm.dealerIndex = this.dealerIndex;
			syncData.texasHoldEm.communityCards = Pool.GetList<int>();
			syncData.texasHoldEm.biggestRaiseThisTurn = this.BiggestRaiseThisTurn;
			syncData.lastActionId = (int)this.LastAction;
			syncData.lastActionTarget = this.LastActionTarget;
			syncData.lastActionValue = this.LastActionValue;
			foreach (PlayingCard playingCard in this.communityCards)
			{
				syncData.texasHoldEm.communityCards.Add(playingCard.GetIndex());
			}
			this.ClearLastAction();
		}

		// Token: 0x060044AC RID: 17580 RVA: 0x00192210 File Offset: 0x00190410
		protected override void SubStartRound()
		{
			this.communityCards.Clear();
			this.deck = new StackOfCards(1);
			this.BiggestRaiseThisTurn = 0;
			this.ClearLastAction();
			this.IncrementDealer();
			this.DealHoleCards();
			this.activePlayerIndex = this.GetFirstPlayerRelIndex(true);
			base.ServerPlaySound(CardGameSounds.SoundType.Shuffle);
			CardPlayerData cardPlayerData;
			base.TryGetActivePlayer(out cardPlayerData);
			cardPlayerData.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerData);
			if ((cardPlayerData.availableInputs & 32) == 32)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 32, false, 5, false);
			}
			else
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 4, false, 5, false);
			}
			base.TryGetActivePlayer(out cardPlayerData);
			cardPlayerData.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerData);
			if ((cardPlayerData.availableInputs & 16) == 16)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 16, false, 10, false);
				return;
			}
			base.ReceivedInputFromPlayer(cardPlayerData, 4, false, 10, false);
		}

		// Token: 0x060044AD RID: 17581 RVA: 0x001922DC File Offset: 0x001904DC
		protected override void SubEndRound()
		{
			int num = 0;
			List<CardPlayerData> list = Pool.GetList<CardPlayerData>();
			foreach (CardPlayerData cardPlayerData in base.PlayerData)
			{
				if (cardPlayerData.betThisRound > 0)
				{
					list.Add(cardPlayerData);
				}
				if (cardPlayerData.HasUserInCurrentRound)
				{
					num++;
				}
			}
			if (list.Count == 0)
			{
				base.Owner.GetPot().inventory.Clear();
				return;
			}
			bool flag = num > 1;
			int num2 = base.GetScrapInPot();
			foreach (CardPlayerData cardPlayerData2 in base.PlayerData)
			{
				if (cardPlayerData2.HasUserInGame)
				{
					num2 -= cardPlayerData2.betThisRound;
				}
			}
			bool flag2 = true;
			foreach (CardPlayerData cardPlayerData3 in base.PlayerData)
			{
				cardPlayerData3.remainingToPayOut = cardPlayerData3.betThisRound;
			}
			while (list.Count > 1)
			{
				int num3 = int.MaxValue;
				int num4 = 0;
				foreach (CardPlayerData cardPlayerData4 in base.PlayerData)
				{
					if (cardPlayerData4.betThisRound > 0)
					{
						if (cardPlayerData4.betThisRound < num3)
						{
							num3 = cardPlayerData4.betThisRound;
						}
						num4++;
					}
				}
				int num5 = num3 * num4;
				foreach (CardPlayerData cardPlayerData5 in list)
				{
					cardPlayerData5.betThisRound -= num3;
				}
				int num6 = int.MaxValue;
				foreach (CardPlayerData cardPlayerData6 in base.PlayersInRound())
				{
					if (cardPlayerData6.finalScore < num6)
					{
						num6 = cardPlayerData6.finalScore;
					}
				}
				if (flag2)
				{
					base.resultInfo.winningScore = num6;
				}
				int num7 = 0;
				using (IEnumerator<CardPlayerData> enumerator2 = base.PlayersInRound().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.finalScore == num6)
						{
							num7++;
						}
					}
				}
				int num8 = Mathf.CeilToInt((float)(num5 + num2) / (float)num7);
				num2 = 0;
				foreach (CardPlayerData cardPlayerData7 in base.PlayersInRound())
				{
					if (cardPlayerData7.finalScore == num6)
					{
						if (flag)
						{
							cardPlayerData7.EnableSendingCards();
						}
						base.PayOutFromPot(cardPlayerData7, num8);
						TexasHoldEmController.PokerRoundResult pokerRoundResult = (flag2 ? TexasHoldEmController.PokerRoundResult.PrimaryWinner : TexasHoldEmController.PokerRoundResult.SecondaryWinner);
						this.AddRoundResult(cardPlayerData7, num8, (int)pokerRoundResult);
					}
				}
				for (int j = list.Count - 1; j >= 0; j--)
				{
					if (list[j].betThisRound == 0)
					{
						list.RemoveAt(j);
					}
				}
				flag2 = false;
			}
			if (list.Count == 1)
			{
				int num9 = list[0].betThisRound + num2;
				num2 = 0;
				base.PayOutFromPot(list[0], num9);
				TexasHoldEmController.PokerRoundResult pokerRoundResult2 = ((base.resultInfo.results.Count == 0) ? TexasHoldEmController.PokerRoundResult.PrimaryWinner : TexasHoldEmController.PokerRoundResult.SecondaryWinner);
				this.AddRoundResult(list[0], num9, (int)pokerRoundResult2);
			}
			base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
			StorageContainer pot = base.Owner.GetPot();
			int amount = pot.inventory.GetAmount(base.ScrapItemID, true);
			if (amount > 0)
			{
				Debug.LogError(string.Format("{0}: Something went wrong in the winner calculation. Pot still has {1} scrap left over after payouts. Expected 0. Clearing it.", base.GetType().Name, amount));
				pot.inventory.Clear();
			}
			Pool.FreeList<CardPlayerData>(ref list);
		}

		// Token: 0x060044AE RID: 17582 RVA: 0x0019269C File Offset: 0x0019089C
		protected override void AddRoundResult(CardPlayerData pData, int winnings, int winState)
		{
			base.AddRoundResult(pData, winnings, winState);
			if (GameInfo.HasAchievements)
			{
				global::BasePlayer basePlayer = base.Owner.IDToPlayer(pData.UserID);
				if (basePlayer != null)
				{
					basePlayer.stats.Add("won_hand_texas_holdem", 1, Stats.Steam);
					basePlayer.stats.Save(true);
				}
			}
		}

		// Token: 0x060044AF RID: 17583 RVA: 0x001926F2 File Offset: 0x001908F2
		protected override void SubEndGameplay()
		{
			this.communityCards.Clear();
		}

		// Token: 0x060044B0 RID: 17584 RVA: 0x00192700 File Offset: 0x00190900
		private void IncrementDealer()
		{
			int num = base.NumPlayersInGame();
			if (num == 0)
			{
				this.dealerIndex = 0;
				return;
			}
			this.dealerIndex = Mathf.Clamp(this.dealerIndex, 0, num - 1);
			int num2 = this.dealerIndex + 1;
			this.dealerIndex = num2;
			this.dealerIndex = num2 % num;
		}

		// Token: 0x060044B1 RID: 17585 RVA: 0x00192750 File Offset: 0x00190950
		private void DealHoleCards()
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
				{
					PlayingCard playingCard;
					if (this.deck.TryTakeCard(out playingCard))
					{
						cardPlayerData.Cards.Add(playingCard);
					}
					else
					{
						Debug.LogError(base.GetType().Name + ": No more cards in the deck to deal!");
					}
				}
			}
			base.SyncAllLocalPlayerCards();
		}

		// Token: 0x060044B2 RID: 17586 RVA: 0x001927E0 File Offset: 0x001909E0
		private bool DealCommunityCards()
		{
			if (!base.HasActiveRound)
			{
				return false;
			}
			if (this.communityCards.Count == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					PlayingCard playingCard;
					if (this.deck.TryTakeCard(out playingCard))
					{
						this.communityCards.Add(playingCard);
					}
				}
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				return true;
			}
			if (this.communityCards.Count == 3 || this.communityCards.Count == 4)
			{
				PlayingCard playingCard2;
				if (this.deck.TryTakeCard(out playingCard2))
				{
					this.communityCards.Add(playingCard2);
				}
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				return true;
			}
			return false;
		}

		// Token: 0x060044B3 RID: 17587 RVA: 0x00192875 File Offset: 0x00190A75
		private void ClearLastAction()
		{
			this.LastAction = TexasHoldEmController.PokerInputOption.None;
			this.LastActionTarget = 0UL;
			this.LastActionValue = 0;
		}

		// Token: 0x060044B4 RID: 17588 RVA: 0x00192890 File Offset: 0x00190A90
		protected override void OnTurnTimeout(CardPlayerData pData)
		{
			CardPlayerData cardPlayerData;
			if (base.TryGetActivePlayer(out cardPlayerData) && cardPlayerData == pData)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 1, true, 0, false);
			}
		}

		// Token: 0x060044B5 RID: 17589 RVA: 0x001928B8 File Offset: 0x00190AB8
		protected override void SubReceivedInputFromPlayer(CardPlayerData playerData, int input, int value, bool countAsAction)
		{
			if (!Enum.IsDefined(typeof(TexasHoldEmController.PokerInputOption), input))
			{
				return;
			}
			if (!base.HasActiveRound)
			{
				if (input == 64)
				{
					playerData.EnableSendingCards();
					playerData.availableInputs = this.GetAvailableInputsForPlayer(playerData);
					base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				}
				this.LastActionTarget = playerData.UserID;
				this.LastAction = (TexasHoldEmController.PokerInputOption)input;
				this.LastActionValue = 0;
				return;
			}
			CardPlayerData cardPlayerData;
			if (!base.TryGetActivePlayer(out cardPlayerData))
			{
				return;
			}
			if (cardPlayerData != playerData)
			{
				return;
			}
			bool flag = false;
			if ((playerData.availableInputs & input) != input)
			{
				return;
			}
			if (input <= 8)
			{
				switch (input)
				{
				case 1:
					playerData.LeaveCurrentRound(false, true);
					flag = true;
					this.LastActionValue = 0;
					break;
				case 2:
				{
					int num = this.GetCurrentBet();
					int num2 = base.TryAddBet(playerData, num - playerData.betThisTurn);
					this.LastActionValue = num2;
					break;
				}
				case 3:
					break;
				case 4:
				{
					int num = this.GetCurrentBet();
					int num2 = base.GoAllIn(playerData);
					this.BiggestRaiseThisTurn = Mathf.Max(this.BiggestRaiseThisTurn, num2 - num);
					this.LastActionValue = num2;
					break;
				}
				default:
					if (input == 8)
					{
						this.LastActionValue = 0;
					}
					break;
				}
			}
			else if (input == 16 || input == 32)
			{
				int num = this.GetCurrentBet();
				int biggestRaiseThisTurn = this.BiggestRaiseThisTurn;
				if (playerData.betThisTurn + value < num + biggestRaiseThisTurn)
				{
					value = num + biggestRaiseThisTurn - playerData.betThisTurn;
				}
				int num2 = base.TryAddBet(playerData, value);
				this.BiggestRaiseThisTurn = Mathf.Max(this.BiggestRaiseThisTurn, num2 - num);
				this.LastActionValue = num2;
			}
			if (countAsAction && input != 0)
			{
				playerData.SetHasCompletedTurn(true);
			}
			this.LastActionTarget = playerData.UserID;
			this.LastAction = (TexasHoldEmController.PokerInputOption)input;
			if (flag && base.NumPlayersInCurrentRound() == 1)
			{
				base.EndRoundWithDelay();
				return;
			}
			int num3 = this.activePlayerIndex;
			if (flag)
			{
				if (this.activePlayerIndex > base.NumPlayersInCurrentRound() - 1)
				{
					num3 = 0;
				}
			}
			else
			{
				num3 = (this.activePlayerIndex + 1) % base.NumPlayersInCurrentRound();
			}
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			CardPlayerData cardPlayerData2;
			if (base.TryMoveToNextPlayerWithInputs(num3, out cardPlayerData2))
			{
				base.StartTurnTimer(cardPlayerData2, this.MaxTurnTime);
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			this.EndCycle();
		}

		// Token: 0x060044B6 RID: 17590 RVA: 0x00192AE4 File Offset: 0x00190CE4
		protected override void StartNextCycle()
		{
			base.StartNextCycle();
			int num = this.GetFirstPlayerRelIndex(false);
			int num2 = base.NumPlayersInGame();
			int num3 = 0;
			CardPlayerData cardPlayerData;
			while (!base.ToCardPlayerData(num, true, out cardPlayerData) || !cardPlayerData.HasUserInCurrentRound)
			{
				num = (num + 1) % num2;
				num3++;
				if (num3 > num2)
				{
					Debug.LogError(base.GetType().Name + ": This should never happen. Ended turn with no players in game?.");
					base.EndRoundWithDelay();
					return;
				}
			}
			int num4 = base.GameToRoundIndex(num);
			if (num4 < 0 || num4 > base.NumPlayersInCurrentRound())
			{
				Debug.LogError(string.Format("StartNextCycle NewActiveIndex is out of range: {0}. Clamping it to between 0 and {1}.", num4, base.NumPlayersInCurrentRound()));
				num4 = Mathf.Clamp(num4, 0, base.NumPlayersInCurrentRound());
			}
			int num5 = num4;
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			CardPlayerData cardPlayerData2;
			if (base.TryMoveToNextPlayerWithInputs(num5, out cardPlayerData2))
			{
				base.StartTurnTimer(cardPlayerData2, this.MaxTurnTime);
				base.UpdateAllAvailableInputs();
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			this.EndCycle();
		}

		// Token: 0x060044B7 RID: 17591 RVA: 0x00192BDC File Offset: 0x00190DDC
		protected override bool ShouldEndCycle()
		{
			int num = 0;
			using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetScrapAmount() > 0)
					{
						num++;
					}
				}
			}
			if (num == 1)
			{
				return true;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				if (cardPlayerData.GetScrapAmount() > 0 && (cardPlayerData.betThisTurn != this.GetCurrentBet() || !cardPlayerData.hasCompletedTurn))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060044B8 RID: 17592 RVA: 0x00192C90 File Offset: 0x00190E90
		protected override void EndCycle()
		{
			CardPlayerData[] playerData = base.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].SetHasCompletedTurn(false);
			}
			this.BiggestRaiseThisTurn = 0;
			if (this.DealCommunityCards())
			{
				base.QueueNextCycleInvoke();
				return;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				List<PlayingCard> list = Pool.GetList<PlayingCard>();
				list.AddRange(cardPlayerData.Cards);
				list.AddRange(this.communityCards);
				ushort num = TexasHoldEmController.EvaluatePokerHand(list);
				Pool.FreeList<PlayingCard>(ref list);
				cardPlayerData.finalScore = (int)num;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060044B9 RID: 17593 RVA: 0x00192D4C File Offset: 0x00190F4C
		protected override int GetAvailableInputsForPlayer(CardPlayerData playerData)
		{
			TexasHoldEmController.PokerInputOption pokerInputOption = TexasHoldEmController.PokerInputOption.None;
			if (playerData == null || this.isWaitingBetweenTurns)
			{
				return (int)pokerInputOption;
			}
			if (!base.HasActiveRound)
			{
				if (!playerData.LeftRoundEarly && playerData.Cards.Count > 0 && !playerData.SendCardDetails)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.RevealHand;
				}
				return (int)pokerInputOption;
			}
			CardPlayerData cardPlayerData;
			if (!base.TryGetActivePlayer(out cardPlayerData) || playerData != cardPlayerData)
			{
				return (int)pokerInputOption;
			}
			int scrapAmount = playerData.GetScrapAmount();
			if (scrapAmount > 0)
			{
				pokerInputOption |= TexasHoldEmController.PokerInputOption.AllIn;
				pokerInputOption |= TexasHoldEmController.PokerInputOption.Fold;
				int currentBet = this.GetCurrentBet();
				if (playerData.betThisTurn >= currentBet)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.Check;
				}
				if (currentBet > playerData.betThisTurn && scrapAmount >= currentBet - playerData.betThisTurn)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.Call;
				}
				if (scrapAmount >= this.GetCurrentMinRaise(playerData))
				{
					if (this.BiggestRaiseThisTurn == 0)
					{
						pokerInputOption |= TexasHoldEmController.PokerInputOption.Bet;
					}
					else
					{
						pokerInputOption |= TexasHoldEmController.PokerInputOption.Raise;
					}
				}
			}
			return (int)pokerInputOption;
		}

		// Token: 0x060044BA RID: 17594 RVA: 0x00192E03 File Offset: 0x00191003
		protected override void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData)
		{
			base.ReceivedInputFromPlayer(pData, 1, true, 0, false);
		}

		// Token: 0x04003D35 RID: 15669
		public List<PlayingCard> communityCards = new List<PlayingCard>();

		// Token: 0x04003D36 RID: 15670
		public const int SMALL_BLIND = 5;

		// Token: 0x04003D37 RID: 15671
		public const int BIG_BLIND = 10;

		// Token: 0x04003D38 RID: 15672
		public const string WON_HAND_STAT = "won_hand_texas_holdem";

		// Token: 0x04003D3D RID: 15677
		private int dealerIndex;

		// Token: 0x04003D3E RID: 15678
		private StackOfCards deck = new StackOfCards(1);

		// Token: 0x02000F93 RID: 3987
		[Flags]
		public enum PokerInputOption
		{
			// Token: 0x04005091 RID: 20625
			None = 0,
			// Token: 0x04005092 RID: 20626
			Fold = 1,
			// Token: 0x04005093 RID: 20627
			Call = 2,
			// Token: 0x04005094 RID: 20628
			AllIn = 4,
			// Token: 0x04005095 RID: 20629
			Check = 8,
			// Token: 0x04005096 RID: 20630
			Raise = 16,
			// Token: 0x04005097 RID: 20631
			Bet = 32,
			// Token: 0x04005098 RID: 20632
			RevealHand = 64
		}

		// Token: 0x02000F94 RID: 3988
		public enum PokerRoundResult
		{
			// Token: 0x0400509A RID: 20634
			Loss,
			// Token: 0x0400509B RID: 20635
			PrimaryWinner,
			// Token: 0x0400509C RID: 20636
			SecondaryWinner
		}
	}
}
