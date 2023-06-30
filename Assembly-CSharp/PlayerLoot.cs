using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B0 RID: 176
public class PlayerLoot : EntityComponent<global::BasePlayer>
{
	// Token: 0x06001017 RID: 4119 RVA: 0x000862D0 File Offset: 0x000844D0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerLoot.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x00086310 File Offset: 0x00084510
	public bool IsLooting()
	{
		return this.containers.Count > 0;
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x00086320 File Offset: 0x00084520
	public void Clear()
	{
		if (!this.IsLooting())
		{
			return;
		}
		this.MarkDirty();
		if (this.entitySource)
		{
			this.entitySource.SendMessage("PlayerStoppedLooting", base.baseEntity, SendMessageOptions.DontRequireReceiver);
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			if (itemContainer != null)
			{
				itemContainer.onDirty -= this.MarkDirty;
			}
		}
		this.containers.Clear();
		this.entitySource = null;
		this.itemSource = null;
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x000863D0 File Offset: 0x000845D0
	public global::ItemContainer FindContainer(ItemContainerId id)
	{
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			global::ItemContainer itemContainer2 = itemContainer.FindContainer(id);
			if (itemContainer2 != null)
			{
				return itemContainer2;
			}
		}
		return null;
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0008643C File Offset: 0x0008463C
	public global::Item FindItem(ItemId id)
	{
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			global::Item item = itemContainer.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x000864B0 File Offset: 0x000846B0
	public void Check()
	{
		if (!this.IsLooting())
		{
			return;
		}
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (this.entitySource == null)
		{
			base.baseEntity.ChatMessage("Stopping Looting because lootable doesn't exist!");
			this.Clear();
			return;
		}
		if (!this.entitySource.CanBeLooted(base.baseEntity))
		{
			this.Clear();
			return;
		}
		if (this.PositionChecks)
		{
			float num = this.entitySource.Distance(base.baseEntity.eyes.position);
			if (num > 3f)
			{
				LootDistanceOverride component = this.entitySource.GetComponent<LootDistanceOverride>();
				if (component == null || num > component.amount)
				{
					this.Clear();
					return;
				}
			}
		}
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x00086564 File Offset: 0x00084764
	private void MarkDirty()
	{
		if (!this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = true;
			base.Invoke(new Action(this.SendUpdate), 0.1f);
		}
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0008658C File Offset: 0x0008478C
	public void SendImmediate()
	{
		if (this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = false;
			base.CancelInvoke(new Action(this.SendUpdate));
		}
		this.SendUpdate();
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x000865B8 File Offset: 0x000847B8
	private void SendUpdate()
	{
		this.isInvokingSendUpdate = false;
		if (!base.baseEntity.IsValid())
		{
			return;
		}
		using (PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>())
		{
			if (this.entitySource && this.entitySource.net != null)
			{
				playerUpdateLoot.entityID = this.entitySource.net.ID;
			}
			if (this.itemSource != null)
			{
				playerUpdateLoot.itemID = this.itemSource.uid;
			}
			if (this.containers.Count > 0)
			{
				playerUpdateLoot.containers = Pool.Get<List<ProtoBuf.ItemContainer>>();
				foreach (global::ItemContainer itemContainer in this.containers)
				{
					playerUpdateLoot.containers.Add(itemContainer.Save());
				}
			}
			base.baseEntity.ClientRPCPlayer<PlayerUpdateLoot>(null, base.baseEntity, "UpdateLoot", playerUpdateLoot);
		}
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x000866C4 File Offset: 0x000848C4
	public bool StartLootingEntity(global::BaseEntity targetEntity, bool doPositionChecks = true)
	{
		this.Clear();
		if (!targetEntity)
		{
			return false;
		}
		if (!targetEntity.OnStartBeingLooted(base.baseEntity))
		{
			return false;
		}
		Assert.IsTrue(targetEntity.isServer, "Assure is server");
		this.PositionChecks = doPositionChecks;
		this.entitySource = targetEntity;
		this.itemSource = null;
		this.MarkDirty();
		ILootableEntity lootableEntity;
		if ((lootableEntity = targetEntity as ILootableEntity) != null)
		{
			lootableEntity.LastLootedBy = base.baseEntity.userID;
		}
		return true;
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00086738 File Offset: 0x00084938
	public void AddContainer(global::ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		this.containers.Add(container);
		container.onDirty += this.MarkDirty;
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0008675C File Offset: 0x0008495C
	public void RemoveContainer(global::ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		container.onDirty -= this.MarkDirty;
		this.containers.Remove(container);
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x00086784 File Offset: 0x00084984
	public bool RemoveContainerAt(int index)
	{
		if (index < 0 || index >= this.containers.Count)
		{
			return false;
		}
		if (this.containers[index] != null)
		{
			this.containers[index].onDirty -= this.MarkDirty;
		}
		this.containers.RemoveAt(index);
		return true;
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x000867E0 File Offset: 0x000849E0
	public void StartLootingItem(global::Item item)
	{
		this.Clear();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		this.PositionChecks = true;
		this.containers.Add(item.contents);
		item.contents.onDirty += this.MarkDirty;
		this.itemSource = item;
		this.entitySource = item.GetWorldEntity();
		this.MarkDirty();
	}

	// Token: 0x04000A54 RID: 2644
	public global::BaseEntity entitySource;

	// Token: 0x04000A55 RID: 2645
	public global::Item itemSource;

	// Token: 0x04000A56 RID: 2646
	public List<global::ItemContainer> containers = new List<global::ItemContainer>();

	// Token: 0x04000A57 RID: 2647
	internal bool PositionChecks = true;

	// Token: 0x04000A58 RID: 2648
	private bool isInvokingSendUpdate;
}
