using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000033 RID: 51
public class AdventCalendar : BaseCombatEntity
{
	// Token: 0x0600014C RID: 332 RVA: 0x00021C4C File Offset: 0x0001FE4C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AdventCalendar.OnRpcMessage", 0))
		{
			if (rpc == 1911254136U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestGift ");
				}
				using (TimeWarning.New("RPC_RequestGift", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1911254136U, "RPC_RequestGift", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsVisible.Test(1911254136U, "RPC_RequestGift", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestGift(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_RequestGift");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00021E0C File Offset: 0x0002000C
	public override void ServerInit()
	{
		base.ServerInit();
		AdventCalendar.all.Add(this);
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00021E1F File Offset: 0x0002001F
	public override void DestroyShared()
	{
		AdventCalendar.all.Remove(this);
		base.DestroyShared();
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00021E34 File Offset: 0x00020034
	public void AwardGift(BasePlayer player)
	{
		DateTime now = DateTime.Now;
		int num = now.Day - this.startDay;
		if (now.Month != this.startMonth)
		{
			return;
		}
		if (num < 0 || num >= this.days.Length)
		{
			return;
		}
		if (!AdventCalendar.playerRewardHistory.ContainsKey(player.userID))
		{
			AdventCalendar.playerRewardHistory.Add(player.userID, new List<int>());
		}
		AdventCalendar.playerRewardHistory[player.userID].Add(num);
		Effect.server.Run(this.giftEffect.resourcePath, player.transform.position, default(Vector3), null, false);
		if (num >= 0 && num < this.crosses.Length)
		{
			Effect.server.Run(this.boxCloseEffect.resourcePath, base.transform.position + Vector3.up * 1.5f, default(Vector3), null, false);
		}
		AdventCalendar.DayReward dayReward = this.days[num];
		for (int i = 0; i < dayReward.rewards.Length; i++)
		{
			ItemAmount itemAmount = dayReward.rewards[i];
			player.GiveItem(ItemManager.CreateByItemID(itemAmount.itemid, Mathf.CeilToInt(itemAmount.amount), 0UL), BaseEntity.GiveItemReason.PickedUp);
		}
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00021F70 File Offset: 0x00020170
	public bool WasAwardedTodaysGift(BasePlayer player)
	{
		if (!AdventCalendar.playerRewardHistory.ContainsKey(player.userID))
		{
			return false;
		}
		DateTime now = DateTime.Now;
		if (now.Month != this.startMonth)
		{
			return true;
		}
		int num = now.Day - this.startDay;
		return num < 0 || num >= this.days.Length || AdventCalendar.playerRewardHistory[player.userID].Contains(num);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x00021FE4 File Offset: 0x000201E4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_RequestGift(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (this.WasAwardedTodaysGift(player))
		{
			player.ShowToast(GameTip.Styles.Red_Normal, AdventCalendar.CheckLater, Array.Empty<string>());
			return;
		}
		this.AwardGift(player);
	}

	// Token: 0x04000194 RID: 404
	public int startMonth;

	// Token: 0x04000195 RID: 405
	public int startDay;

	// Token: 0x04000196 RID: 406
	public AdventCalendar.DayReward[] days;

	// Token: 0x04000197 RID: 407
	public GameObject[] crosses;

	// Token: 0x04000198 RID: 408
	public static List<AdventCalendar> all = new List<AdventCalendar>();

	// Token: 0x04000199 RID: 409
	public static Dictionary<ulong, List<int>> playerRewardHistory = new Dictionary<ulong, List<int>>();

	// Token: 0x0400019A RID: 410
	public static readonly Translate.Phrase CheckLater = new Translate.Phrase("adventcalendar.checklater", "You've already claimed today's gift. Come back tomorrow.");

	// Token: 0x0400019B RID: 411
	public static readonly Translate.Phrase EventOver = new Translate.Phrase("adventcalendar.eventover", "The Advent Calendar event is over. See you next year.");

	// Token: 0x0400019C RID: 412
	public GameObjectRef giftEffect;

	// Token: 0x0400019D RID: 413
	public GameObjectRef boxCloseEffect;

	// Token: 0x02000B60 RID: 2912
	[Serializable]
	public class DayReward
	{
		// Token: 0x04003F5F RID: 16223
		public ItemAmount[] rewards;
	}
}
