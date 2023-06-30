using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000052 RID: 82
public class BuildingPrivlidge : StorageContainer
{
	// Token: 0x06000903 RID: 2307 RVA: 0x00056828 File Offset: 0x00054A28
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0))
		{
			if (rpc == 1092560690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddSelfAuthorize ");
				}
				using (TimeWarning.New("AddSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1092560690U, "AddSelfAuthorize", this, player, 3f))
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
							this.AddSelfAuthorize(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 253307592U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearList ");
				}
				using (TimeWarning.New("ClearList", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(253307592U, "ClearList", this, player, 3f))
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
							this.ClearList(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClearList");
					}
				}
				return true;
			}
			if (rpc == 3617985969U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RemoveSelfAuthorize ");
				}
				using (TimeWarning.New("RemoveSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3617985969U, "RemoveSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RemoveSelfAuthorize(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 2051750736U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Rotate ");
				}
				using (TimeWarning.New("RPC_Rotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2051750736U, "RPC_Rotate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Rotate(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_Rotate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00056DE0 File Offset: 0x00054FE0
	public float CalculateUpkeepPeriodMinutes()
	{
		if (base.isServer)
		{
			return ConVar.Decay.upkeep_period_minutes;
		}
		return 0f;
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00056DF5 File Offset: 0x00054FF5
	public float CalculateUpkeepCostFraction()
	{
		if (base.isServer)
		{
			return this.CalculateBuildingTaxRate();
		}
		return 0f;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00056E0C File Offset: 0x0005500C
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (!building.HasDecayEntities())
		{
			return;
		}
		float num = this.CalculateUpkeepCostFraction();
		foreach (global::DecayEntity decayEntity in building.decayEntities)
		{
			decayEntity.CalculateUpkeepCostAmounts(itemAmounts, num);
		}
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00056E7C File Offset: 0x0005507C
	public float GetProtectedMinutes(bool force = false)
	{
		if (!base.isServer)
		{
			return 0f;
		}
		if (!force && UnityEngine.Time.realtimeSinceStartup < this.nextProtectedCalcTime)
		{
			return this.cachedProtectedMinutes;
		}
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + 60f;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		this.CalculateUpkeepCostAmounts(list);
		float num = this.CalculateUpkeepPeriodMinutes();
		float num2 = -1f;
		if (base.inventory != null)
		{
			foreach (ItemAmount itemAmount in list)
			{
				int num3 = base.inventory.FindItemsByItemID(itemAmount.itemid).Sum((global::Item x) => x.amount);
				if (num3 > 0 && itemAmount.amount > 0f)
				{
					float num4 = (float)num3 / itemAmount.amount * num;
					if (num2 == -1f || num4 < num2)
					{
						num2 = num4;
					}
				}
				else
				{
					num2 = 0f;
				}
			}
			if (num2 == -1f)
			{
				num2 = 0f;
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.cachedProtectedMinutes = num2;
		return this.cachedProtectedMinutes;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00056FBC File Offset: 0x000551BC
	public override void OnKilled(HitInfo info)
	{
		if (ConVar.Decay.upkeep_grief_protection > 0f)
		{
			this.PurchaseUpkeepTime(ConVar.Decay.upkeep_grief_protection * 60f);
		}
		base.OnKilled(info);
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00056FE2 File Offset: 0x000551E2
	public override void DecayTick()
	{
		if (this.EnsurePrimary())
		{
			base.DecayTick();
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00056FF4 File Offset: 0x000551F4
	private bool EnsurePrimary()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null)
		{
			BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null && dominatingBuildingPrivilege != this)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.Gib);
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x0005702E File Offset: 0x0005522E
	public void MarkProtectedMinutesDirty(float delay = 0f)
	{
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + delay;
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00057040 File Offset: 0x00055240
	private float CalculateBuildingTaxRate()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		if (!building.HasBuildingBlocks())
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		int count = building.buildingBlocks.Count;
		int num = count;
		for (int i = 0; i < BuildingPrivlidge.upkeepBrackets.Length; i++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket = BuildingPrivlidge.upkeepBrackets[i];
			upkeepBracket.blocksTaxPaid = 0f;
			if (num > 0)
			{
				int num2;
				if (i == BuildingPrivlidge.upkeepBrackets.Length - 1)
				{
					num2 = num;
				}
				else
				{
					num2 = Mathf.Min(num, BuildingPrivlidge.upkeepBrackets[i].objectsUpTo);
				}
				num -= num2;
				upkeepBracket.blocksTaxPaid = (float)num2 * upkeepBracket.fraction;
			}
		}
		float num3 = 0f;
		for (int j = 0; j < BuildingPrivlidge.upkeepBrackets.Length; j++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket2 = BuildingPrivlidge.upkeepBrackets[j];
			if (upkeepBracket2.blocksTaxPaid <= 0f)
			{
				break;
			}
			num3 += upkeepBracket2.blocksTaxPaid;
		}
		return num3 / (float)count;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00057130 File Offset: 0x00055330
	private void ApplyUpkeepPayment()
	{
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		for (int i = 0; i < this.upkeepBuffer.Count; i++)
		{
			ItemAmount itemAmount = this.upkeepBuffer[i];
			int num = (int)itemAmount.amount;
			if (num >= 1)
			{
				base.inventory.Take(list, itemAmount.itemid, num);
				Analytics.Azure.AddPendingItems(this, itemAmount.itemDef.shortname, num, "upkeep", true, true);
				foreach (global::Item item in list)
				{
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[]
						{
							this.ToString(),
							": Using ",
							item.amount,
							" of ",
							item.info.shortname
						}));
					}
					item.UseItem(item.amount);
				}
				list.Clear();
				itemAmount.amount -= (float)num;
				this.upkeepBuffer[i] = itemAmount;
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0005726C File Offset: 0x0005546C
	private void QueueUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			bool flag = false;
			foreach (ItemAmount itemAmount2 in this.upkeepBuffer)
			{
				if (itemAmount2.itemDef == itemAmount.itemDef)
				{
					itemAmount2.amount += itemAmount.amount;
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[]
						{
							this.ToString(),
							": Adding ",
							itemAmount.amount,
							" of ",
							itemAmount.itemDef.shortname,
							" to ",
							itemAmount2.amount
						}));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[]
					{
						this.ToString(),
						": Adding ",
						itemAmount.amount,
						" of ",
						itemAmount.itemDef.shortname
					}));
				}
				this.upkeepBuffer.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
			}
		}
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000573DC File Offset: 0x000555DC
	private bool CanAffordUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			if ((float)base.inventory.GetAmount(itemAmount.itemid, true) < itemAmount.amount)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[]
					{
						this.ToString(),
						": Can't afford ",
						itemAmount.amount,
						" of ",
						itemAmount.itemDef.shortname
					}));
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00057470 File Offset: 0x00055670
	public float PurchaseUpkeepTime(global::DecayEntity entity, float deltaTime)
	{
		float num = this.CalculateUpkeepCostFraction();
		float num2 = this.CalculateUpkeepPeriodMinutes() * 60f;
		float num3 = num * deltaTime / num2;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		entity.CalculateUpkeepCostAmounts(list, num3);
		bool flag = this.CanAffordUpkeepPayment(list);
		this.QueueUpkeepPayment(list);
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.ApplyUpkeepPayment();
		if (!flag)
		{
			return 0f;
		}
		return deltaTime;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x000574C8 File Offset: 0x000556C8
	public void PurchaseUpkeepTime(float deltaTime)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null && building.HasDecayEntities())
		{
			float num = Mathf.Min(this.GetProtectedMinutes(true) * 60f, deltaTime);
			if (num > 0f)
			{
				foreach (global::DecayEntity decayEntity in building.decayEntities)
				{
					float protectedSeconds = decayEntity.GetProtectedSeconds();
					if (num > protectedSeconds)
					{
						float num2 = this.PurchaseUpkeepTime(decayEntity, num - protectedSeconds);
						decayEntity.AddUpkeepTime(num2);
						if (this.IsDebugging())
						{
							Debug.Log(string.Concat(new object[]
							{
								this.ToString(),
								" purchased upkeep time for ",
								decayEntity.ToString(),
								": ",
								protectedSeconds,
								" + ",
								num2,
								" = ",
								decayEntity.GetProtectedSeconds()
							}));
						}
					}
				}
			}
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000575E4 File Offset: 0x000557E4
	public override void ResetState()
	{
		base.ResetState();
		this.authorizedPlayers.Clear();
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000575F8 File Offset: 0x000557F8
	public bool IsAuthed(global::BasePlayer player)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == player.userID);
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0005762C File Offset: 0x0005582C
	public bool IsAuthed(ulong userID)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == userID);
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x0005765D File Offset: 0x0005585D
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00057670 File Offset: 0x00055870
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		bool flag = this.allowedConstructionItems.Contains(item.info);
		if (!flag && targetSlot == -1)
		{
			int num = 0;
			foreach (global::Item item2 in base.inventory.itemList)
			{
				if (!this.allowedConstructionItems.Contains(item2.info) && (item2.info != item.info || item2.amount == item2.MaxStackable()))
				{
					num++;
				}
			}
			if (num >= 24)
			{
				return false;
			}
		}
		if (targetSlot >= 24 && targetSlot <= 27)
		{
			return flag;
		}
		return base.ItemFilter(item, targetSlot);
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00057730 File Offset: 0x00055930
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingPrivilege = Facepunch.Pool.Get<BuildingPrivilege>();
		info.msg.buildingPrivilege.users = this.authorizedPlayers;
		if (!info.forDisk)
		{
			info.msg.buildingPrivilege.upkeepPeriodMinutes = this.CalculateUpkeepPeriodMinutes();
			info.msg.buildingPrivilege.costFraction = this.CalculateUpkeepCostFraction();
			info.msg.buildingPrivilege.protectedMinutes = this.GetProtectedMinutes(false);
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000577B5 File Offset: 0x000559B5
	public override void PostSave(global::BaseNetworkable.SaveInfo info)
	{
		info.msg.buildingPrivilege.users = null;
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x000577C8 File Offset: 0x000559C8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			this.authorizedPlayers = info.msg.buildingPrivilege.users;
			if (!info.fromDisk)
			{
				this.cachedProtectedMinutes = info.msg.buildingPrivilege.protectedMinutes;
			}
			info.msg.buildingPrivilege.users = null;
		}
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0005784B File Offset: 0x00055A4B
	public void BuildingDirty()
	{
		if (base.isServer)
		{
			this.AddDelayedUpdate();
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool AtMaxAuthCapacity()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved5);
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0005785C File Offset: 0x00055A5C
	public void UpdateMaxAuthCapacity()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode && activeGameMode.limitTeamAuths)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, this.authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize(), false, true);
		}
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x000578A3 File Offset: 0x00055AA3
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		this.AddDelayedUpdate();
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x000578B1 File Offset: 0x00055AB1
	public override void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		this.AddDelayedUpdate();
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000578C1 File Offset: 0x00055AC1
	public void AddDelayedUpdate()
	{
		if (base.IsInvoking(new Action(this.DelayedUpdate)))
		{
			base.CancelInvoke(new Action(this.DelayedUpdate));
		}
		base.Invoke(new Action(this.DelayedUpdate), 1f);
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00057900 File Offset: 0x00055B00
	public void DelayedUpdate()
	{
		this.MarkProtectedMinutesDirty(0f);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00057914 File Offset: 0x00055B14
	public bool CanAdministrate(global::BasePlayer player)
	{
		BaseLock baseLock = base.GetSlot(global::BaseEntity.Slot.Lock) as BaseLock;
		return baseLock == null || baseLock.OnTryToOpen(player);
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x00057940 File Offset: 0x00055B40
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.AddPlayer(rpc.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x00057974 File Offset: 0x00055B74
	public void AddPlayer(global::BasePlayer player)
	{
		if (this.AtMaxAuthCapacity())
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
		PlayerNameID playerNameID = new PlayerNameID();
		playerNameID.userid = player.userID;
		playerNameID.username = player.displayName;
		this.authorizedPlayers.Add(playerNameID);
		Analytics.Azure.OnEntityAuthChanged(this, player, this.authorizedPlayers.Select((PlayerNameID x) => x.userid), "added", player.userID);
		this.UpdateMaxAuthCapacity();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x00057A30 File Offset: 0x00055C30
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		Analytics.Azure.OnEntityAuthChanged(this, rpc.player, this.authorizedPlayers.Select((PlayerNameID x) => x.userid), "removed", rpc.player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00057AE6 File Offset: 0x00055CE6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void ClearList(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.Clear();
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00057B20 File Offset: 0x00055D20
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Rotate(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player.CanBuild() && player.GetHeldEntity() && player.GetHeldEntity().GetComponent<Hammer>() != null && (base.GetSlot(global::BaseEntity.Slot.Lock) == null || !base.GetSlot(global::BaseEntity.Slot.Lock).IsLocked()) && !base.HasAttachedStorageAdaptor())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			Deployable component = base.GetComponent<Deployable>();
			if (component != null && component.placeEffect.isValid)
			{
				Effect.server.Run(component.placeEffect.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.Lock);
		if (slot != null)
		{
			slot.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00057C14 File Offset: 0x00055E14
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		if (item != null && item.info != null && this.allowedConstructionItems.Contains(item.info))
		{
			for (int i = 24; i <= 27; i++)
			{
				if (base.inventory.GetSlot(i) == null)
				{
					return i;
				}
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00057C6B File Offset: 0x00055E6B
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return slot == global::BaseEntity.Slot.Lock || base.HasSlot(slot);
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000614 RID: 1556
	private float cachedProtectedMinutes;

	// Token: 0x04000615 RID: 1557
	private float nextProtectedCalcTime;

	// Token: 0x04000616 RID: 1558
	private static BuildingPrivlidge.UpkeepBracket[] upkeepBrackets = new BuildingPrivlidge.UpkeepBracket[]
	{
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_0_blockcount, ConVar.Decay.bracket_0_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_1_blockcount, ConVar.Decay.bracket_1_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_2_blockcount, ConVar.Decay.bracket_2_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_3_blockcount, ConVar.Decay.bracket_3_costfraction)
	};

	// Token: 0x04000617 RID: 1559
	private List<ItemAmount> upkeepBuffer = new List<ItemAmount>();

	// Token: 0x04000618 RID: 1560
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x04000619 RID: 1561
	public const global::BaseEntity.Flags Flag_MaxAuths = global::BaseEntity.Flags.Reserved5;

	// Token: 0x0400061A RID: 1562
	public List<ItemDefinition> allowedConstructionItems = new List<ItemDefinition>();

	// Token: 0x02000BD1 RID: 3025
	public class UpkeepBracket
	{
		// Token: 0x06004DBF RID: 19903 RVA: 0x001A182A File Offset: 0x0019FA2A
		public UpkeepBracket(int numObjs, float frac)
		{
			this.objectsUpTo = numObjs;
			this.fraction = frac;
			this.blocksTaxPaid = 0f;
		}

		// Token: 0x0400417F RID: 16767
		public int objectsUpTo;

		// Token: 0x04004180 RID: 16768
		public float fraction;

		// Token: 0x04004181 RID: 16769
		public float blocksTaxPaid;
	}
}
