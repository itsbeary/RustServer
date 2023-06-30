using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000094 RID: 148
public class Locker : StorageContainer
{
	// Token: 0x06000D9E RID: 3486 RVA: 0x00073D20 File Offset: 0x00071F20
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Locker.OnRpcMessage", 0))
		{
			if (rpc == 1799659668U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Equip ");
				}
				using (TimeWarning.New("RPC_Equip", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1799659668U, "RPC_Equip", this, player, 3f))
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
							this.RPC_Equip(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Equip");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool IsEquipping()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00073E88 File Offset: 0x00072088
	private Locker.RowType GetRowType(int slot)
	{
		if (slot == -1)
		{
			return Locker.RowType.Clothing;
		}
		if (slot % 13 >= 7)
		{
			return Locker.RowType.Belt;
		}
		return Locker.RowType.Clothing;
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x00073E9A File Offset: 0x0007209A
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00073EB0 File Offset: 0x000720B0
	public void ClearEquipping()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00073EC0 File Offset: 0x000720C0
	public override bool ItemFilter(Item item, int targetSlot)
	{
		return base.ItemFilter(item, targetSlot) && (item.info.category == ItemCategory.Attire || this.GetRowType(targetSlot) == Locker.RowType.Belt);
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00073EE8 File Offset: 0x000720E8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Equip(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num < 0 || num >= 3)
		{
			return;
		}
		if (this.IsEquipping())
		{
			return;
		}
		BasePlayer player = msg.player;
		int num2 = num * 13;
		bool flag = false;
		for (int i = 0; i < player.inventory.containerWear.capacity; i++)
		{
			Item slot = player.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				this.clothingBuffer[i] = slot;
			}
		}
		for (int j = 0; j < 7; j++)
		{
			int num3 = num2 + j;
			Item slot2 = base.inventory.GetSlot(num3);
			Item item = this.clothingBuffer[j];
			if (slot2 != null)
			{
				flag = true;
				if (slot2.info.category != ItemCategory.Attire || !slot2.MoveToContainer(player.inventory.containerWear, j, true, false, null, true))
				{
					slot2.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			if (item != null)
			{
				flag = true;
				if (item.info.category != ItemCategory.Attire || !item.MoveToContainer(base.inventory, num3, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			this.clothingBuffer[j] = null;
		}
		for (int k = 0; k < 6; k++)
		{
			int num4 = num2 + k + 7;
			int num5 = k;
			Item slot3 = base.inventory.GetSlot(num4);
			Item slot4 = player.inventory.containerBelt.GetSlot(k);
			if (slot4 != null)
			{
				slot4.RemoveFromContainer();
			}
			if (slot3 != null)
			{
				flag = true;
				if (!slot3.MoveToContainer(player.inventory.containerBelt, num5, true, false, null, true))
				{
					slot3.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			if (slot4 != null)
			{
				flag = true;
				if (!slot4.MoveToContainer(base.inventory, num4, true, false, null, true))
				{
					slot4.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
		}
		if (flag)
		{
			Effect.server.Run(this.equipSound.resourcePath, player, StringPool.Get("spine3"), Vector3.zero, Vector3.zero, null, false);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			base.Invoke(new Action(this.ClearEquipping), 1.5f);
		}
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x00074158 File Offset: 0x00072358
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		int i = 0;
		while (i < this.inventorySlots)
		{
			Locker.RowType rowType = this.GetRowType(i);
			if (item.info.category == ItemCategory.Attire)
			{
				if (rowType == Locker.RowType.Clothing)
				{
					goto IL_23;
				}
			}
			else if (rowType == Locker.RowType.Belt)
			{
				goto IL_23;
			}
			IL_41:
			i++;
			continue;
			IL_23:
			if (!base.inventory.SlotTaken(item, i) && (rowType != Locker.RowType.Clothing || !this.DoesWearableConflictWithRow(item, i)))
			{
				return i;
			}
			goto IL_41;
		}
		return int.MinValue;
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x000741B8 File Offset: 0x000723B8
	private bool DoesWearableConflictWithRow(Item item, int pos)
	{
		int num = pos / 13 * 13;
		ItemModWearable itemModWearable = item.info.ItemModWearable;
		if (itemModWearable == null)
		{
			return false;
		}
		for (int i = num; i < num + 7; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				ItemModWearable itemModWearable2 = slot.info.ItemModWearable;
				if (!(itemModWearable2 == null) && !itemModWearable2.CanExistWith(itemModWearable))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00074226 File Offset: 0x00072426
	public Vector2i GetIndustrialSlotRange(Vector3 localPosition)
	{
		if (localPosition.x < -0.3f)
		{
			return new Vector2i(26, 38);
		}
		if (localPosition.x > 0.3f)
		{
			return new Vector2i(0, 12);
		}
		return new Vector2i(13, 25);
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x0007425E File Offset: 0x0007245E
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !base.HasAttachedStorageAdaptor();
	}

	// Token: 0x040008B6 RID: 2230
	public GameObjectRef equipSound;

	// Token: 0x040008B7 RID: 2231
	private const int maxGearSets = 3;

	// Token: 0x040008B8 RID: 2232
	private const int attireSize = 7;

	// Token: 0x040008B9 RID: 2233
	private const int beltSize = 6;

	// Token: 0x040008BA RID: 2234
	private const int columnSize = 2;

	// Token: 0x040008BB RID: 2235
	private Item[] clothingBuffer = new Item[7];

	// Token: 0x040008BC RID: 2236
	private const int setSize = 13;

	// Token: 0x02000BEC RID: 3052
	private enum RowType
	{
		// Token: 0x040041D9 RID: 16857
		Clothing,
		// Token: 0x040041DA RID: 16858
		Belt
	}

	// Token: 0x02000BED RID: 3053
	public static class LockerFlags
	{
		// Token: 0x040041DB RID: 16859
		public const BaseEntity.Flags IsEquipping = BaseEntity.Flags.Reserved1;
	}
}
