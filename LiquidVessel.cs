using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000092 RID: 146
public class LiquidVessel : HeldEntity
{
	// Token: 0x06000D82 RID: 3458 RVA: 0x0007306C File Offset: 0x0007126C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidVessel.OnRpcMessage", 0))
		{
			if (rpc == 4034725537U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoEmpty ");
				}
				using (TimeWarning.New("DoEmpty", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4034725537U, "DoEmpty", this, player))
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
							this.DoEmpty(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoEmpty");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000731D0 File Offset: 0x000713D0
	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00073238 File Offset: 0x00071438
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoEmpty(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		using (List<Item>.Enumerator enumerator = item.contents.itemList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.UseItem(50);
			}
		}
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x000732C8 File Offset: 0x000714C8
	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item item2 = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (item2 == null)
		{
			Item item3 = ItemManager.Create(liquidType, amount, 0UL);
			if (item3 != null)
			{
				item3.MoveToContainer(item.contents, -1, true, false, null, true);
				return;
			}
		}
		else
		{
			int num = Mathf.Clamp(item2.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(item2.info, liquidType);
			if (itemDefinition != item2.info)
			{
				item2.Remove(0f);
				item2 = ItemManager.Create(itemDefinition, num, 0UL);
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
			else
			{
				item2.amount = num;
			}
			item2.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x00073394 File Offset: 0x00071594
	public bool CanFillHere(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && (double)ownerPlayer.WaterFactor() > 0.05;
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x000733C8 File Offset: 0x000715C8
	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x000733F2 File Offset: 0x000715F2
	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00073403 File Offset: 0x00071603
	public bool IsFull()
	{
		return this.HeldFraction() >= 1f;
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x00073415 File Offset: 0x00071615
	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}
}
