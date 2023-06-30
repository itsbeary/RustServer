using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BF RID: 191
public class ResearchTable : StorageContainer
{
	// Token: 0x06001132 RID: 4402 RVA: 0x0008D760 File Offset: 0x0008B960
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResearchTable.OnRpcMessage", 0))
		{
			if (rpc == 3177710095U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoResearch ");
				}
				using (TimeWarning.New("DoResearch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3177710095U, "DoResearch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoResearch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoResearch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0008D8C8 File Offset: 0x0008BAC8
	public override void ResetState()
	{
		base.ResetState();
		this.researchFinishedTime = 0f;
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0008D8DC File Offset: 0x0008BADC
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		if (item.info.shortname == "scrap")
		{
			global::Item slot = base.inventory.GetSlot(1);
			if (slot == null)
			{
				return 1;
			}
			if (slot.amount < item.MaxStackable())
			{
				return 1;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0002A700 File Offset: 0x00028900
	public bool IsResearching()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0008D92A File Offset: 0x0008BB2A
	public int RarityMultiplier(Rarity rarity)
	{
		if (rarity == Rarity.Common)
		{
			return 20;
		}
		if (rarity == Rarity.Uncommon)
		{
			return 15;
		}
		if (rarity == Rarity.Rare)
		{
			return 10;
		}
		return 5;
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0008D944 File Offset: 0x0008BB44
	public int GetBlueprintStacksize(global::Item sourceItem)
	{
		int num = this.RarityMultiplier(sourceItem.info.rarity);
		if (sourceItem.info.category == ItemCategory.Ammunition)
		{
			num = Mathf.FloorToInt((float)sourceItem.MaxStackable() / (float)sourceItem.info.Blueprint.amountToCreate) * 2;
		}
		return num;
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0008D993 File Offset: 0x0008BB93
	public int ScrapForResearch(global::Item item)
	{
		return this.ScrapForResearch(item.info);
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0008D9A4 File Offset: 0x0008BBA4
	public int ScrapForResearch(ItemDefinition info)
	{
		if (info.isRedirectOf != null)
		{
			return this.ScrapForResearch(info.isRedirectOf);
		}
		int num = 0;
		if (info.rarity == Rarity.Common)
		{
			num = 20;
		}
		if (info.rarity == Rarity.Uncommon)
		{
			num = 75;
		}
		if (info.rarity == Rarity.Rare)
		{
			num = 125;
		}
		if (info.rarity == Rarity.VeryRare || info.rarity == Rarity.None)
		{
			num = 500;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(info);
		if (itemBlueprint != null && itemBlueprint.defaultBlueprint)
		{
			return ConVar.Server.defaultBlueprintResearchCost;
		}
		return num;
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0008DA28 File Offset: 0x0008BC28
	public static int ScrapForResearch(ItemDefinition info, global::ResearchTable.ResearchType type)
	{
		int num = 0;
		if (info.rarity == Rarity.Common)
		{
			num = 20;
		}
		if (info.rarity == Rarity.Uncommon)
		{
			num = 75;
		}
		if (info.rarity == Rarity.Rare)
		{
			num = 125;
		}
		if (info.rarity == Rarity.VeryRare || info.rarity == Rarity.None)
		{
			num = 500;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			BaseGameMode.ResearchCostResult scrapCostForResearch = activeGameMode.GetScrapCostForResearch(info, type);
			if (scrapCostForResearch.Scale != null)
			{
				num = Mathf.RoundToInt((float)num * scrapCostForResearch.Scale.Value);
			}
			else if (scrapCostForResearch.Amount != null)
			{
				num = scrapCostForResearch.Amount.Value;
			}
		}
		return num;
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0008DACC File Offset: 0x0008BCCC
	public bool IsItemResearchable(global::Item item)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint((item.info.isRedirectOf != null) ? item.info.isRedirectOf : item.info);
		return (itemBlueprint != null && itemBlueprint.defaultBlueprint) || (!(itemBlueprint == null) && itemBlueprint.isResearchable);
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0008DB2C File Offset: 0x0008BD2C
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x0008DB4C File Offset: 0x0008BD4C
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return (targetSlot != 1 || !(item.info != this.researchResource)) && base.ItemFilter(item, targetSlot);
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x0008DB6F File Offset: 0x0008BD6F
	public global::Item GetTargetItem()
	{
		return base.inventory.GetSlot(0);
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x0008DB80 File Offset: 0x0008BD80
	public global::Item GetScrapItem()
	{
		global::Item slot = base.inventory.GetSlot(1);
		if (slot == null || slot.info != this.researchResource)
		{
			return null;
		}
		return slot;
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x0008DBB3 File Offset: 0x0008BDB3
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		}
		base.inventory.SetLocked(false);
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x0008DBE8 File Offset: 0x0008BDE8
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		this.user = player;
		return base.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x0008DBFE File Offset: 0x0008BDFE
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		this.user = null;
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x0008DC10 File Offset: 0x0008BE10
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void DoResearch(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsResearching())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		global::Item targetItem = this.GetTargetItem();
		if (targetItem == null)
		{
			return;
		}
		if (targetItem.amount > 1)
		{
			return;
		}
		if (!this.IsItemResearchable(targetItem))
		{
			return;
		}
		targetItem.CollectedForCrafting(player);
		this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + this.researchDuration;
		base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		base.inventory.SetLocked(true);
		int num = this.ScrapForResearch(targetItem);
		Analytics.Azure.OnResearchStarted(player, this, targetItem, num);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		player.inventory.loot.SendImmediate();
		if (this.researchStartEffect.isValid)
		{
			Effect.server.Run(this.researchStartEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		msg.player.GiveAchievement("RESEARCH_ITEM");
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x0008DCF8 File Offset: 0x0008BEF8
	public void ResearchAttemptFinished()
	{
		global::Item targetItem = this.GetTargetItem();
		global::Item scrapItem = this.GetScrapItem();
		if (targetItem != null && scrapItem != null)
		{
			int num = this.ScrapForResearch(targetItem);
			if (scrapItem.amount >= num)
			{
				if (scrapItem.amount == num)
				{
					base.inventory.Remove(scrapItem);
					scrapItem.RemoveFromContainer();
					scrapItem.Remove(0f);
				}
				else
				{
					scrapItem.UseItem(num);
				}
				base.inventory.Remove(targetItem);
				targetItem.Remove(0f);
				global::Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
				item.blueprintTarget = ((targetItem.info.isRedirectOf != null) ? targetItem.info.isRedirectOf.itemid : targetItem.info.itemid);
				if (!item.MoveToContainer(base.inventory, 0, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
				if (this.researchSuccessEffect.isValid)
				{
					Effect.server.Run(this.researchSuccessEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		base.SendNetworkUpdateImmediate(false);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
		this.EndResearch();
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x000063A5 File Offset: 0x000045A5
	public void CancelResearch()
	{
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x0008DE4C File Offset: 0x0008C04C
	public void EndResearch()
	{
		base.inventory.SetLocked(false);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.researchFinishedTime = 0f;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0008DEA4 File Offset: 0x0008C0A4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.researchTable = Facepunch.Pool.Get<ProtoBuf.ResearchTable>();
		info.msg.researchTable.researchTimeLeft = this.researchFinishedTime - UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x0008DED9 File Offset: 0x0008C0D9
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.researchTable != null)
		{
			this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + info.msg.researchTable.researchTimeLeft;
		}
	}

	// Token: 0x04000AC6 RID: 2758
	[NonSerialized]
	public float researchFinishedTime;

	// Token: 0x04000AC7 RID: 2759
	public float researchCostFraction = 1f;

	// Token: 0x04000AC8 RID: 2760
	public float researchDuration = 10f;

	// Token: 0x04000AC9 RID: 2761
	public int requiredPaper = 10;

	// Token: 0x04000ACA RID: 2762
	public GameObjectRef researchStartEffect;

	// Token: 0x04000ACB RID: 2763
	public GameObjectRef researchFailEffect;

	// Token: 0x04000ACC RID: 2764
	public GameObjectRef researchSuccessEffect;

	// Token: 0x04000ACD RID: 2765
	public ItemDefinition researchResource;

	// Token: 0x04000ACE RID: 2766
	private global::BasePlayer user;

	// Token: 0x02000C03 RID: 3075
	public enum ResearchType
	{
		// Token: 0x04004223 RID: 16931
		ResearchTable,
		// Token: 0x04004224 RID: 16932
		TechTree
	}
}
