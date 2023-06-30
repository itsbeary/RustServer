using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007A RID: 122
public class FrankensteinTable : StorageContainer
{
	// Token: 0x06000B65 RID: 2917 RVA: 0x00065924 File Offset: 0x00063B24
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FrankensteinTable.OnRpcMessage", 0))
		{
			if (rpc == 629197370U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CreateFrankenstein ");
				}
				using (TimeWarning.New("CreateFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(629197370U, "CreateFrankenstein", this, player, 3f))
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
							this.CreateFrankenstein(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in CreateFrankenstein");
					}
				}
				return true;
			}
			if (rpc == 4797457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestSleepFrankenstein ");
				}
				using (TimeWarning.New("RequestSleepFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4797457U, "RequestSleepFrankenstein", this, player, 3f))
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
							this.RequestSleepFrankenstein(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RequestSleepFrankenstein");
					}
				}
				return true;
			}
			if (rpc == 3804893505U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestWakeFrankenstein ");
				}
				using (TimeWarning.New("RequestWakeFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3804893505U, "RequestWakeFrankenstein", this, player, 3f))
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
							this.RequestWakeFrankenstein(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RequestWakeFrankenstein");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x00065D80 File Offset: 0x00063F80
	public bool IsHeadItem(ItemDefinition itemDef)
	{
		return this.HeadItems.Contains(itemDef);
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00065D8E File Offset: 0x00063F8E
	public bool IsTorsoItem(ItemDefinition itemDef)
	{
		return this.TorsoItems.Contains(itemDef);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00065D9C File Offset: 0x00063F9C
	public bool IsLegsItem(ItemDefinition itemDef)
	{
		return this.LegItems.Contains(itemDef);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00065DAA File Offset: 0x00063FAA
	public bool HasValidItems(global::ItemContainer container)
	{
		return this.GetValidItems(container) != null;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00065DB8 File Offset: 0x00063FB8
	public List<ItemDefinition> GetValidItems(global::ItemContainer container)
	{
		if (container == null)
		{
			return null;
		}
		if (container.itemList == null)
		{
			return null;
		}
		if (container.itemList.Count == 0)
		{
			return null;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		List<ItemDefinition> list = new List<ItemDefinition>();
		for (int i = 0; i < container.capacity; i++)
		{
			global::Item slot = container.GetSlot(i);
			if (slot != null)
			{
				this.CheckItem(slot.info, list, this.HeadItems, ref flag);
				this.CheckItem(slot.info, list, this.TorsoItems, ref flag2);
				this.CheckItem(slot.info, list, this.LegItems, ref flag3);
				if (flag && flag2 && flag3)
				{
					return list;
				}
			}
		}
		return null;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00065E60 File Offset: 0x00064060
	public bool HasAllValidItems(List<ItemDefinition> items)
	{
		if (items == null)
		{
			return false;
		}
		if (items.Count < 3)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (ItemDefinition itemDefinition in items)
		{
			if (itemDefinition == null)
			{
				return false;
			}
			this.CheckItem(itemDefinition, null, this.HeadItems, ref flag);
			this.CheckItem(itemDefinition, null, this.TorsoItems, ref flag2);
			this.CheckItem(itemDefinition, null, this.LegItems, ref flag3);
		}
		return flag && flag2 && flag3;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x00065F08 File Offset: 0x00064108
	private void CheckItem(ItemDefinition item, List<ItemDefinition> itemList, List<ItemDefinition> validItems, ref bool set)
	{
		if (set)
		{
			return;
		}
		if (validItems.Contains(item))
		{
			set = true;
			if (itemList != null)
			{
				itemList.Add(item);
			}
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00065F28 File Offset: 0x00064128
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00065F7A File Offset: 0x0006417A
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00065F8C File Offset: 0x0006418C
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		return item != null && ((this.HeadItems != null && this.IsHeadItem(item.info)) || (this.TorsoItems != null && this.IsTorsoItem(item.info)) || (this.LegItems != null && this.IsLegsItem(item.info)));
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void CreateFrankenstein(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00065FE7 File Offset: 0x000641E7
	private bool CanStartCreating(global::BasePlayer player)
	{
		return !this.waking && !(player == null) && !(player.PetEntity != null) && this.HasValidItems(base.inventory);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00066020 File Offset: 0x00064220
	private bool IsInventoryEmpty()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00066054 File Offset: 0x00064254
	private void ConsumeInventory()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.UseItem(slot.amount);
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00066098 File Offset: 0x00064298
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RequestWakeFrankenstein(global::BaseEntity.RPCMessage msg)
	{
		this.WakeFrankenstein(msg.player);
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x000660A8 File Offset: 0x000642A8
	private void WakeFrankenstein(global::BasePlayer owner)
	{
		if (owner == null)
		{
			return;
		}
		if (!this.CanStartCreating(owner))
		{
			return;
		}
		this.waking = true;
		base.inventory.SetLocked(true);
		base.SendNetworkUpdateImmediate(false);
		base.StartCoroutine(this.DelayWakeFrankenstein(owner));
		base.ClientRPC(null, "CL_WakeFrankenstein");
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x000660FD File Offset: 0x000642FD
	private IEnumerator DelayWakeFrankenstein(global::BasePlayer owner)
	{
		yield return new WaitForSeconds(1.5f);
		yield return new WaitForSeconds(this.TableDownDuration);
		if (owner != null && owner.PetEntity != null)
		{
			base.inventory.SetLocked(false);
			base.SendNetworkUpdateImmediate(false);
			this.waking = false;
			yield break;
		}
		this.ItemsToUse = this.GetValidItems(base.inventory);
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.FrankensteinPrefab.resourcePath, this.SpawnLocation.position, this.SpawnLocation.rotation, false);
		baseEntity.enableSaving = false;
		baseEntity.gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		this.EquipFrankenstein(baseEntity as FrankensteinPet);
		this.ConsumeInventory();
		base.inventory.SetLocked(false);
		base.SendNetworkUpdateImmediate(false);
		base.StartCoroutine(this.WaitForFrankensteinBrainInit(baseEntity as BasePet, owner));
		this.waking = false;
		yield return null;
		yield break;
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00066114 File Offset: 0x00064314
	private void EquipFrankenstein(FrankensteinPet frank)
	{
		if (this.ItemsToUse == null)
		{
			return;
		}
		if (frank == null)
		{
			return;
		}
		if (frank.inventory == null)
		{
			return;
		}
		foreach (ItemDefinition itemDefinition in this.ItemsToUse)
		{
			frank.inventory.GiveItem(ItemManager.Create(itemDefinition, 1, 0UL), frank.inventory.containerWear, false);
		}
		if (this.WeaponItem != null)
		{
			base.StartCoroutine(frank.DelayEquipWeapon(this.WeaponItem, 1.5f));
		}
		this.ItemsToUse.Clear();
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x000661D4 File Offset: 0x000643D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RequestSleepFrankenstein(global::BaseEntity.RPCMessage msg)
	{
		this.SleepFrankenstein(msg.player);
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x000661E4 File Offset: 0x000643E4
	private void SleepFrankenstein(global::BasePlayer owner)
	{
		if (!this.IsInventoryEmpty())
		{
			return;
		}
		if (owner == null)
		{
			return;
		}
		if (owner.PetEntity == null)
		{
			return;
		}
		FrankensteinPet frankensteinPet = owner.PetEntity as FrankensteinPet;
		if (frankensteinPet == null)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, frankensteinPet.transform.position) >= 5f)
		{
			return;
		}
		this.ReturnFrankensteinItems(frankensteinPet);
		ItemManager.DoRemoves();
		base.SendNetworkUpdateImmediate(false);
		frankensteinPet.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x00066268 File Offset: 0x00064468
	private void ReturnFrankensteinItems(FrankensteinPet frank)
	{
		if (frank == null)
		{
			return;
		}
		if (frank.inventory == null)
		{
			return;
		}
		if (frank.inventory.containerWear == null)
		{
			return;
		}
		for (int i = 0; i < frank.inventory.containerWear.capacity; i++)
		{
			global::Item slot = frank.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.MoveToContainer(base.inventory, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x000662DE File Offset: 0x000644DE
	private IEnumerator WaitForFrankensteinBrainInit(BasePet frankenstein, global::BasePlayer player)
	{
		yield return new WaitForEndOfFrame();
		frankenstein.ApplyPetStatModifiers();
		frankenstein.Brain.SetOwningPlayer(player);
		frankenstein.CreateMapMarker();
		player.SendClientPetLink();
		yield break;
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x000662F4 File Offset: 0x000644F4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			return;
		}
		info.msg.FrankensteinTable = Facepunch.Pool.Get<ProtoBuf.FrankensteinTable>();
		info.msg.FrankensteinTable.itemIds = new List<int>();
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				info.msg.FrankensteinTable.itemIds.Add(slot.info.itemid);
			}
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0006637C File Offset: 0x0006457C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x04000771 RID: 1905
	public GameObjectRef FrankensteinPrefab;

	// Token: 0x04000772 RID: 1906
	public Transform SpawnLocation;

	// Token: 0x04000773 RID: 1907
	public ItemDefinition WeaponItem;

	// Token: 0x04000774 RID: 1908
	public List<ItemDefinition> HeadItems;

	// Token: 0x04000775 RID: 1909
	public List<ItemDefinition> TorsoItems;

	// Token: 0x04000776 RID: 1910
	public List<ItemDefinition> LegItems;

	// Token: 0x04000777 RID: 1911
	[HideInInspector]
	public List<ItemDefinition> ItemsToUse;

	// Token: 0x04000778 RID: 1912
	public FrankensteinTableVisuals TableVisuals;

	// Token: 0x04000779 RID: 1913
	[Header("Timings")]
	public float TableDownDuration = 0.9f;

	// Token: 0x0400077A RID: 1914
	private bool waking;
}
