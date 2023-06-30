using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AFD RID: 2813
	public class CardPlayerData : IDisposable
	{
		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06004452 RID: 17490 RVA: 0x00191451 File Offset: 0x0018F651
		// (set) Token: 0x06004453 RID: 17491 RVA: 0x00191459 File Offset: 0x0018F659
		public ulong UserID { get; private set; }

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06004454 RID: 17492 RVA: 0x00191462 File Offset: 0x0018F662
		// (set) Token: 0x06004455 RID: 17493 RVA: 0x0019146A File Offset: 0x0018F66A
		public CardPlayerData.CardPlayerState State { get; private set; }

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06004456 RID: 17494 RVA: 0x00191473 File Offset: 0x0018F673
		public bool HasUser
		{
			get
			{
				return this.State >= CardPlayerData.CardPlayerState.WantsToPlay;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06004457 RID: 17495 RVA: 0x00191481 File Offset: 0x0018F681
		public bool HasUserInGame
		{
			get
			{
				return this.State >= CardPlayerData.CardPlayerState.InGame;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06004458 RID: 17496 RVA: 0x0019148F File Offset: 0x0018F68F
		public bool HasUserInCurrentRound
		{
			get
			{
				return this.State == CardPlayerData.CardPlayerState.InCurrentRound;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06004459 RID: 17497 RVA: 0x0019149A File Offset: 0x0018F69A
		public bool HasAvailableInputs
		{
			get
			{
				return this.availableInputs > 0;
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x0600445A RID: 17498 RVA: 0x001914A8 File Offset: 0x0018F6A8
		public bool AllCardsAreKnown
		{
			get
			{
				if (this.Cards.Count == 0)
				{
					return false;
				}
				using (List<PlayingCard>.Enumerator enumerator = this.Cards.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsUnknownCard)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x0600445B RID: 17499 RVA: 0x00191510 File Offset: 0x0018F710
		private bool IsClient
		{
			get
			{
				return !this.isServer;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x0600445C RID: 17500 RVA: 0x0019151B File Offset: 0x0018F71B
		// (set) Token: 0x0600445D RID: 17501 RVA: 0x00191523 File Offset: 0x0018F723
		public bool LeftRoundEarly { get; private set; }

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x0600445E RID: 17502 RVA: 0x0019152C File Offset: 0x0018F72C
		// (set) Token: 0x0600445F RID: 17503 RVA: 0x00191534 File Offset: 0x0018F734
		public bool SendCardDetails { get; private set; }

		// Token: 0x06004460 RID: 17504 RVA: 0x0019153D File Offset: 0x0018F73D
		public CardPlayerData(int mountIndex, bool isServer)
		{
			this.isServer = isServer;
			this.mountIndex = mountIndex;
			this.Cards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004461 RID: 17505 RVA: 0x0019155E File Offset: 0x0018F75E
		public CardPlayerData(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer)
			: this(mountIndex, isServer)
		{
			this.scrapItemID = scrapItemID;
			this.getStorage = getStorage;
		}

		// Token: 0x06004462 RID: 17506 RVA: 0x00191577 File Offset: 0x0018F777
		public virtual void Dispose()
		{
			Pool.FreeList<PlayingCard>(ref this.Cards);
			if (this.isServer)
			{
				this.CancelTurnTimer();
			}
		}

		// Token: 0x06004463 RID: 17507 RVA: 0x00191594 File Offset: 0x0018F794
		public int GetScrapAmount()
		{
			if (this.isServer)
			{
				StorageContainer storage = this.GetStorage();
				if (storage != null)
				{
					return storage.inventory.GetAmount(this.scrapItemID, true);
				}
				Debug.LogError(base.GetType().Name + ": Couldn't get player storage.");
			}
			return 0;
		}

		// Token: 0x06004464 RID: 17508 RVA: 0x001915E7 File Offset: 0x0018F7E7
		public virtual int GetTotalBetThisRound()
		{
			return this.betThisRound;
		}

		// Token: 0x06004465 RID: 17509 RVA: 0x001915EF File Offset: 0x0018F7EF
		public virtual List<PlayingCard> GetMainCards()
		{
			return this.Cards;
		}

		// Token: 0x06004466 RID: 17510 RVA: 0x0002CFBB File Offset: 0x0002B1BB
		public virtual List<PlayingCard> GetSecondaryCards()
		{
			return null;
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06004467 RID: 17511 RVA: 0x001915F7 File Offset: 0x0018F7F7
		// (set) Token: 0x06004468 RID: 17512 RVA: 0x001915FF File Offset: 0x0018F7FF
		public bool hasCompletedTurn { get; private set; }

		// Token: 0x06004469 RID: 17513 RVA: 0x00191608 File Offset: 0x0018F808
		public void SetHasCompletedTurn(bool hasActed)
		{
			this.hasCompletedTurn = hasActed;
			if (!hasActed)
			{
				this.betThisTurn = 0;
			}
		}

		// Token: 0x0600446A RID: 17514 RVA: 0x0019161B File Offset: 0x0018F81B
		public bool HasBeenIdleFor(int seconds)
		{
			return this.HasUserInGame && Time.unscaledTime > this.lastActionTime + (float)seconds;
		}

		// Token: 0x0600446B RID: 17515 RVA: 0x00191637 File Offset: 0x0018F837
		public StorageContainer GetStorage()
		{
			return this.getStorage(this.mountIndex);
		}

		// Token: 0x0600446C RID: 17516 RVA: 0x0019164A File Offset: 0x0018F84A
		public void AddUser(ulong userID)
		{
			this.ClearAllData();
			this.UserID = userID;
			this.State = CardPlayerData.CardPlayerState.WantsToPlay;
			this.lastActionTime = Time.unscaledTime;
		}

		// Token: 0x0600446D RID: 17517 RVA: 0x0019166B File Offset: 0x0018F86B
		public void ClearAllData()
		{
			this.UserID = 0UL;
			this.availableInputs = 0;
			this.State = CardPlayerData.CardPlayerState.None;
			this.ClearPerRoundData();
		}

		// Token: 0x0600446E RID: 17518 RVA: 0x00191689 File Offset: 0x0018F889
		public void JoinRound()
		{
			if (!this.HasUser)
			{
				return;
			}
			this.State = CardPlayerData.CardPlayerState.InCurrentRound;
			this.ClearPerRoundData();
		}

		// Token: 0x0600446F RID: 17519 RVA: 0x001916A1 File Offset: 0x0018F8A1
		protected virtual void ClearPerRoundData()
		{
			this.Cards.Clear();
			this.betThisRound = 0;
			this.betThisTurn = 0;
			this.finalScore = 0;
			this.LeftRoundEarly = false;
			this.hasCompletedTurn = false;
			this.SendCardDetails = false;
		}

		// Token: 0x06004470 RID: 17520 RVA: 0x001916D8 File Offset: 0x0018F8D8
		public virtual void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
		{
			if (!this.HasUserInCurrentRound)
			{
				return;
			}
			this.availableInputs = 0;
			this.finalScore = 0;
			this.hasCompletedTurn = false;
			if (clearBets)
			{
				this.betThisRound = 0;
				this.betThisTurn = 0;
			}
			this.State = CardPlayerData.CardPlayerState.InGame;
			this.LeftRoundEarly = leftRoundEarly;
			this.CancelTurnTimer();
		}

		// Token: 0x06004471 RID: 17521 RVA: 0x00191728 File Offset: 0x0018F928
		public virtual void LeaveGame()
		{
			if (!this.HasUserInGame)
			{
				return;
			}
			this.Cards.Clear();
			this.availableInputs = 0;
			this.finalScore = 0;
			this.SendCardDetails = false;
			this.LeftRoundEarly = false;
			this.State = CardPlayerData.CardPlayerState.WantsToPlay;
		}

		// Token: 0x06004472 RID: 17522 RVA: 0x00191761 File Offset: 0x0018F961
		public void EnableSendingCards()
		{
			this.SendCardDetails = true;
		}

		// Token: 0x06004473 RID: 17523 RVA: 0x0019176A File Offset: 0x0018F96A
		public string HandToString()
		{
			return CardPlayerData.HandToString(this.Cards);
		}

		// Token: 0x06004474 RID: 17524 RVA: 0x00191778 File Offset: 0x0018F978
		public static string HandToString(List<PlayingCard> cards)
		{
			string text = string.Empty;
			foreach (PlayingCard playingCard in cards)
			{
				text = text + "23456789TJQKA"[(int)playingCard.Rank].ToString() + "♠♥♦♣"[(int)playingCard.Suit].ToString() + " ";
			}
			return text;
		}

		// Token: 0x06004475 RID: 17525 RVA: 0x00191804 File Offset: 0x0018FA04
		public virtual void Save(CardGame syncData)
		{
			CardGame.CardPlayer cardPlayer = Pool.Get<CardGame.CardPlayer>();
			cardPlayer.userid = this.UserID;
			cardPlayer.cards = Pool.GetList<int>();
			foreach (PlayingCard playingCard in this.Cards)
			{
				cardPlayer.cards.Add(this.SendCardDetails ? playingCard.GetIndex() : (-1));
			}
			cardPlayer.scrap = this.GetScrapAmount();
			cardPlayer.state = (int)this.State;
			cardPlayer.availableInputs = this.availableInputs;
			cardPlayer.betThisRound = this.betThisRound;
			cardPlayer.betThisTurn = this.betThisTurn;
			cardPlayer.leftRoundEarly = this.LeftRoundEarly;
			cardPlayer.sendCardDetails = this.SendCardDetails;
			syncData.players.Add(cardPlayer);
		}

		// Token: 0x06004476 RID: 17526 RVA: 0x001918EC File Offset: 0x0018FAEC
		public void StartTurnTimer(Action<CardPlayerData> callback, float maxTurnTime)
		{
			this.turnTimerCallback = callback;
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.TimeoutTurn), maxTurnTime);
		}

		// Token: 0x06004477 RID: 17527 RVA: 0x0019190C File Offset: 0x0018FB0C
		public void CancelTurnTimer()
		{
			SingletonComponent<InvokeHandler>.Instance.CancelInvoke(new Action(this.TimeoutTurn));
		}

		// Token: 0x06004478 RID: 17528 RVA: 0x00191924 File Offset: 0x0018FB24
		public void TimeoutTurn()
		{
			if (this.turnTimerCallback != null)
			{
				this.turnTimerCallback(this);
			}
		}

		// Token: 0x04003D08 RID: 15624
		public List<PlayingCard> Cards;

		// Token: 0x04003D0A RID: 15626
		public readonly int mountIndex;

		// Token: 0x04003D0B RID: 15627
		private readonly bool isServer;

		// Token: 0x04003D0C RID: 15628
		public int availableInputs;

		// Token: 0x04003D0D RID: 15629
		public int betThisRound;

		// Token: 0x04003D0E RID: 15630
		public int betThisTurn;

		// Token: 0x04003D11 RID: 15633
		public int finalScore;

		// Token: 0x04003D13 RID: 15635
		public float lastActionTime;

		// Token: 0x04003D14 RID: 15636
		public int remainingToPayOut;

		// Token: 0x04003D15 RID: 15637
		private Func<int, StorageContainer> getStorage;

		// Token: 0x04003D16 RID: 15638
		private readonly int scrapItemID;

		// Token: 0x04003D17 RID: 15639
		private Action<CardPlayerData> turnTimerCallback;

		// Token: 0x02000F92 RID: 3986
		public enum CardPlayerState
		{
			// Token: 0x0400508C RID: 20620
			None,
			// Token: 0x0400508D RID: 20621
			WantsToPlay,
			// Token: 0x0400508E RID: 20622
			InGame,
			// Token: 0x0400508F RID: 20623
			InCurrentRound
		}
	}
}
