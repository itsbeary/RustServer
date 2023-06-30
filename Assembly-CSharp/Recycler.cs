using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BB RID: 187
public class Recycler : StorageContainer
{
	// Token: 0x060010BB RID: 4283 RVA: 0x00089CB0 File Offset: 0x00087EB0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Recycler.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x00089E18 File Offset: 0x00088018
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x00089E20 File Offset: 0x00088020
	private bool CanBeRecycled(Item item)
	{
		return item != null && item.info.Blueprint != null;
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x00089E38 File Offset: 0x00088038
	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<Item, int, bool>(this.RecyclerItemFilter));
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00089E68 File Offset: 0x00088068
	public bool RecyclerItemFilter(Item item, int targetSlot)
	{
		int num = Mathf.CeilToInt((float)base.inventory.capacity * 0.5f);
		if (targetSlot == -1)
		{
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if (!base.inventory.SlotTaken(item, i))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return targetSlot >= num || this.CanBeRecycled(item);
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x00089EC4 File Offset: 0x000880C4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void SVSwitch(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (flag && !this.HasRecyclable())
		{
			return;
		}
		if (flag)
		{
			foreach (Item item in base.inventory.itemList)
			{
				item.CollectedForCrafting(msg.player);
			}
			this.StartRecycling();
			return;
		}
		this.StopRecycling();
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x00089F60 File Offset: 0x00088160
	public bool MoveItemToOutput(Item newItem)
	{
		int num = -1;
		for (int i = 6; i < 12; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot == null)
			{
				num = i;
				break;
			}
			if (slot.CanStack(newItem))
			{
				if (slot.amount + newItem.amount <= slot.MaxStackable())
				{
					num = i;
					break;
				}
				int num2 = Mathf.Min(slot.MaxStackable() - slot.amount, newItem.amount);
				newItem.UseItem(num2);
				slot.amount += num2;
				slot.MarkDirty();
				newItem.MarkDirty();
			}
			if (newItem.amount <= 0)
			{
				return true;
			}
		}
		if (num != -1 && newItem.MoveToContainer(base.inventory, num, true, false, null, true))
		{
			return true;
		}
		newItem.Drop(base.transform.position + new Vector3(0f, 2f, 0f), this.GetInheritedDropVelocity() + base.transform.forward * 2f, default(Quaternion));
		return false;
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0008A06C File Offset: 0x0008826C
	public bool HasRecyclable()
	{
		for (int i = 0; i < 6; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null && slot.info.Blueprint != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x0008A0AC File Offset: 0x000882AC
	public void RecycleThink()
	{
		bool flag = false;
		float num = this.recycleEfficiency;
		for (int i = 0; i < 6; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (this.CanBeRecycled(slot))
			{
				if (slot.hasCondition)
				{
					num = Mathf.Clamp01(num * Mathf.Clamp(slot.conditionNormalized * slot.maxConditionNormalized, 0.1f, 1f));
				}
				int num2 = 1;
				if (slot.amount > 1)
				{
					num2 = Mathf.CeilToInt(Mathf.Min((float)slot.amount, (float)slot.MaxStackable() * 0.1f));
				}
				if (slot.info.Blueprint.scrapFromRecycle > 0)
				{
					int num3 = slot.info.Blueprint.scrapFromRecycle * num2;
					if (slot.MaxStackable() == 1 && slot.hasCondition)
					{
						num3 = Mathf.CeilToInt((float)num3 * slot.conditionNormalized);
					}
					if (num3 >= 1)
					{
						Item item = ItemManager.CreateByName("scrap", num3, 0UL);
						Analytics.Azure.OnRecyclerItemProduced(item.info.shortname, item.amount, this, slot);
						this.MoveItemToOutput(item);
					}
				}
				if (!string.IsNullOrEmpty(slot.info.Blueprint.RecycleStat))
				{
					List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
					global::Vis.Entities<BasePlayer>(base.transform.position, 3f, list, 131072, QueryTriggerInteraction.Collide);
					foreach (BasePlayer basePlayer in list)
					{
						if (basePlayer.IsAlive() && !basePlayer.IsSleeping() && basePlayer.inventory.loot.entitySource == this)
						{
							basePlayer.stats.Add(slot.info.Blueprint.RecycleStat, num2, (global::Stats)5);
							basePlayer.stats.Save(false);
						}
					}
					Facepunch.Pool.FreeList<BasePlayer>(ref list);
				}
				Analytics.Azure.OnItemRecycled(slot.info.shortname, num2, this);
				slot.UseItem(num2);
				using (List<ItemAmount>.Enumerator enumerator2 = slot.info.Blueprint.ingredients.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemAmount itemAmount = enumerator2.Current;
						if (!(itemAmount.itemDef.shortname == "scrap"))
						{
							float num4 = itemAmount.amount / (float)slot.info.Blueprint.amountToCreate;
							int num5 = 0;
							if (num4 <= 1f)
							{
								for (int j = 0; j < num2; j++)
								{
									if (UnityEngine.Random.Range(0f, 1f) <= num4 * num)
									{
										num5++;
									}
								}
							}
							else
							{
								num5 = Mathf.CeilToInt(Mathf.Clamp(num4 * num * UnityEngine.Random.Range(1f, 1f), 0f, itemAmount.amount)) * num2;
							}
							if (num5 > 0)
							{
								int num6 = Mathf.CeilToInt((float)num5 / (float)itemAmount.itemDef.stackable);
								for (int k = 0; k < num6; k++)
								{
									int num7 = ((num5 > itemAmount.itemDef.stackable) ? itemAmount.itemDef.stackable : num5);
									Item item2 = ItemManager.Create(itemAmount.itemDef, num7, 0UL);
									Analytics.Azure.OnRecyclerItemProduced(item2.info.shortname, item2.amount, this, slot);
									if (!this.MoveItemToOutput(item2))
									{
										flag = true;
									}
									num5 -= num7;
									if (num5 <= 0)
									{
										break;
									}
								}
							}
						}
					}
					break;
				}
			}
		}
		if (flag || !this.HasRecyclable())
		{
			this.StopRecycling();
		}
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x0008A468 File Offset: 0x00088668
	public void StartRecycling()
	{
		if (base.IsOn())
		{
			return;
		}
		base.InvokeRepeating(new Action(this.RecycleThink), 5f, 5f);
		Effect.server.Run(this.startSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x0008A4CC File Offset: 0x000886CC
	public void StopRecycling()
	{
		base.CancelInvoke(new Action(this.RecycleThink));
		if (!base.IsOn())
		{
			return;
		}
		Effect.server.Run(this.stopSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlayAnim()
	{
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAnim()
	{
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ToggleAnim(bool toggle)
	{
	}

	// Token: 0x04000AA3 RID: 2723
	public Animator Animator;

	// Token: 0x04000AA4 RID: 2724
	public float recycleEfficiency = 0.5f;

	// Token: 0x04000AA5 RID: 2725
	public SoundDefinition grindingLoopDef;

	// Token: 0x04000AA6 RID: 2726
	public GameObjectRef startSound;

	// Token: 0x04000AA7 RID: 2727
	public GameObjectRef stopSound;
}
