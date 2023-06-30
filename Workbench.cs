using System;
using ConVar;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F3 RID: 243
public class Workbench : StorageContainer
{
	// Token: 0x0600154B RID: 5451 RVA: 0x000A8424 File Offset: 0x000A6624
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Workbench.OnRpcMessage", 0))
		{
			if (rpc == 2308794761U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_BeginExperiment ");
				}
				using (TimeWarning.New("RPC_BeginExperiment", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2308794761U, "RPC_BeginExperiment", this, player, 3f))
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
							this.RPC_BeginExperiment(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_BeginExperiment");
					}
				}
				return true;
			}
			if (rpc == 4127240744U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TechTreeUnlock ");
				}
				using (TimeWarning.New("RPC_TechTreeUnlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4127240744U, "RPC_TechTreeUnlock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TechTreeUnlock(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_TechTreeUnlock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x000A8724 File Offset: 0x000A6924
	public int GetScrapForExperiment()
	{
		if (this.Workbenchlevel == 1)
		{
			return 75;
		}
		if (this.Workbenchlevel == 2)
		{
			return 300;
		}
		if (this.Workbenchlevel == 3)
		{
			return 1000;
		}
		Debug.LogWarning("GetScrapForExperiment fucked up big time.");
		return 0;
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x0002A700 File Offset: 0x00028900
	public bool IsWorking()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x000A875B File Offset: 0x000A695B
	public override bool CanPickup(global::BasePlayer player)
	{
		return this.children.Count == 0 && base.CanPickup(player);
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x000A8774 File Offset: 0x000A6974
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_TechTreeUnlock(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		int num = msg.read.Int32();
		TechTreeData.NodeInstance byID = this.techTree.GetByID(num);
		if (byID == null)
		{
			Debug.Log("Node for unlock not found :" + num);
			return;
		}
		if (this.techTree.PlayerCanUnlock(player, byID))
		{
			if (byID.IsGroup())
			{
				foreach (int num2 in byID.outputs)
				{
					TechTreeData.NodeInstance byID2 = this.techTree.GetByID(num2);
					if (byID2 != null && byID2.itemDef != null)
					{
						player.blueprints.Unlock(byID2.itemDef);
						Analytics.Azure.OnBlueprintLearned(player, byID2.itemDef, "techtree", this);
					}
				}
				Debug.Log("Player unlocked group :" + byID.groupName);
				return;
			}
			if (byID.itemDef != null)
			{
				int num3 = global::ResearchTable.ScrapForResearch(byID.itemDef, global::ResearchTable.ResearchType.TechTree);
				int itemid = ItemManager.FindItemDefinition("scrap").itemid;
				if (player.inventory.GetAmount(itemid) >= num3)
				{
					player.inventory.Take(null, itemid, num3);
					player.blueprints.Unlock(byID.itemDef);
					Analytics.Azure.OnBlueprintLearned(player, byID.itemDef, "techtree", this);
				}
			}
		}
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000A88E8 File Offset: 0x000A6AE8
	public static ItemDefinition GetBlueprintTemplate()
	{
		if (Workbench.blueprintBaseDef == null)
		{
			Workbench.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
		}
		return Workbench.blueprintBaseDef;
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x000A890C File Offset: 0x000A6B0C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_BeginExperiment(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (this.IsWorking())
		{
			return;
		}
		PersistantPlayer persistantPlayerInfo = player.PersistantPlayerInfo;
		int num = UnityEngine.Random.Range(0, this.experimentalItems.subSpawn.Length);
		for (int i = 0; i < this.experimentalItems.subSpawn.Length; i++)
		{
			int num2 = i + num;
			if (num2 >= this.experimentalItems.subSpawn.Length)
			{
				num2 -= this.experimentalItems.subSpawn.Length;
			}
			ItemDefinition itemDef = this.experimentalItems.subSpawn[num2].category.items[0].itemDef;
			if (itemDef.Blueprint && !itemDef.Blueprint.defaultBlueprint && itemDef.Blueprint.userCraftable && itemDef.Blueprint.isResearchable && !itemDef.Blueprint.NeedsSteamItem && !itemDef.Blueprint.NeedsSteamDLC && !persistantPlayerInfo.unlockedItems.Contains(itemDef.itemid))
			{
				this.pendingBlueprint = itemDef;
				break;
			}
		}
		if (this.pendingBlueprint == null)
		{
			player.ChatMessage("You have already unlocked everything for this workbench tier.");
			return;
		}
		global::Item slot = base.inventory.GetSlot(0);
		if (slot != null)
		{
			if (!slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
			player.inventory.loot.SendImmediate();
		}
		if (this.experimentStartEffect.isValid)
		{
			Effect.server.Run(this.experimentStartEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.inventory.SetLocked(true);
		base.CancelInvoke(new Action(this.ExperimentComplete));
		base.Invoke(new Action(this.ExperimentComplete), 5f);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x000A8B10 File Offset: 0x000A6D10
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x000A8B19 File Offset: 0x000A6D19
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		base.CancelInvoke(new Action(this.ExperimentComplete));
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x000A8B34 File Offset: 0x000A6D34
	public int GetAvailableExperimentResources()
	{
		global::Item experimentResourceItem = this.GetExperimentResourceItem();
		if (experimentResourceItem == null || experimentResourceItem.info != this.experimentResource)
		{
			return 0;
		}
		return experimentResourceItem.amount;
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x000A8B66 File Offset: 0x000A6D66
	public global::Item GetExperimentResourceItem()
	{
		return base.inventory.GetSlot(1);
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x000A8B74 File Offset: 0x000A6D74
	public void ExperimentComplete()
	{
		global::Item experimentResourceItem = this.GetExperimentResourceItem();
		int scrapForExperiment = this.GetScrapForExperiment();
		if (this.pendingBlueprint == null)
		{
			Debug.LogWarning("Pending blueprint was null!");
		}
		if (experimentResourceItem != null && experimentResourceItem.amount >= scrapForExperiment && this.pendingBlueprint != null)
		{
			experimentResourceItem.UseItem(scrapForExperiment);
			global::Item item = ItemManager.Create(Workbench.GetBlueprintTemplate(), 1, 0UL);
			item.blueprintTarget = this.pendingBlueprint.itemid;
			this.creatingBlueprint = true;
			if (!item.MoveToContainer(base.inventory, 0, true, false, null, true))
			{
				item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
			this.creatingBlueprint = false;
			if (this.experimentSuccessEffect.isValid)
			{
				Effect.server.Run(this.experimentSuccessEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			}
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.pendingBlueprint = null;
		base.inventory.SetLocked(false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x000A8C7C File Offset: 0x000A6E7C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (base.inventory != null)
		{
			base.inventory.SetLocked(false);
		}
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x0008DB2C File Offset: 0x0008BD2C
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x000A8CA2 File Offset: 0x000A6EA2
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return (targetSlot == 1 && item.info == this.experimentResource) || (targetSlot == 0 && this.creatingBlueprint);
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000D81 RID: 3457
	public const int blueprintSlot = 0;

	// Token: 0x04000D82 RID: 3458
	public const int experimentSlot = 1;

	// Token: 0x04000D83 RID: 3459
	public bool Static;

	// Token: 0x04000D84 RID: 3460
	public int Workbenchlevel;

	// Token: 0x04000D85 RID: 3461
	public LootSpawn experimentalItems;

	// Token: 0x04000D86 RID: 3462
	public GameObjectRef experimentStartEffect;

	// Token: 0x04000D87 RID: 3463
	public GameObjectRef experimentSuccessEffect;

	// Token: 0x04000D88 RID: 3464
	public ItemDefinition experimentResource;

	// Token: 0x04000D89 RID: 3465
	public TechTreeData techTree;

	// Token: 0x04000D8A RID: 3466
	public bool supportsIndustrialCrafter;

	// Token: 0x04000D8B RID: 3467
	public static ItemDefinition blueprintBaseDef;

	// Token: 0x04000D8C RID: 3468
	private ItemDefinition pendingBlueprint;

	// Token: 0x04000D8D RID: 3469
	private bool creatingBlueprint;
}
