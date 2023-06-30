using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020005DE RID: 1502
public abstract class ItemModAssociatedEntity<T> : ItemMod where T : global::BaseEntity
{
	// Token: 0x170003BB RID: 955
	// (get) Token: 0x06002D5B RID: 11611 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool AllowNullParenting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x06002D5C RID: 11612 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool AllowHeldEntityParenting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x06002D5D RID: 11613 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool ShouldAutoCreateEntity
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x06002D5E RID: 11614 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool OwnedByParentPlayer
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x001121BE File Offset: 0x001103BE
	public override void OnItemCreated(global::Item item)
	{
		base.OnItemCreated(item);
		if (this.ShouldAutoCreateEntity)
		{
			this.CreateAssociatedEntity(item);
		}
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x001121D8 File Offset: 0x001103D8
	public T CreateAssociatedEntity(global::Item item)
	{
		if (item.instanceData != null)
		{
			return default(T);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, Vector3.zero, default(Quaternion), true);
		T component = baseEntity.GetComponent<T>();
		this.OnAssociatedItemCreated(component);
		baseEntity.Spawn();
		item.instanceData = new ProtoBuf.Item.InstanceData();
		item.instanceData.ShouldPool = false;
		item.instanceData.subEntity = baseEntity.net.ID;
		item.MarkDirty();
		return component;
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnAssociatedItemCreated(T ent)
	{
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x00112264 File Offset: 0x00110464
	public override void OnRemove(global::Item item)
	{
		base.OnRemove(item);
		T associatedEntity = ItemModAssociatedEntity<T>.GetAssociatedEntity(item, true);
		if (associatedEntity)
		{
			associatedEntity.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x00112299 File Offset: 0x00110499
	public override void OnMovedToWorld(global::Item item)
	{
		this.UpdateParent(item);
		base.OnMovedToWorld(item);
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x001122A9 File Offset: 0x001104A9
	public override void OnRemovedFromWorld(global::Item item)
	{
		this.UpdateParent(item);
		base.OnRemovedFromWorld(item);
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x001122BC File Offset: 0x001104BC
	public void UpdateParent(global::Item item)
	{
		T associatedEntity = ItemModAssociatedEntity<T>.GetAssociatedEntity(item, true);
		if (associatedEntity == null)
		{
			return;
		}
		global::BaseEntity entityForParenting = this.GetEntityForParenting(item);
		if (entityForParenting == null)
		{
			if (this.AllowNullParenting)
			{
				associatedEntity.SetParent(null, false, true);
			}
			if (this.OwnedByParentPlayer)
			{
				associatedEntity.OwnerID = 0UL;
			}
			return;
		}
		if (!entityForParenting.isServer)
		{
			return;
		}
		if (!entityForParenting.IsFullySpawned())
		{
			return;
		}
		associatedEntity.SetParent(entityForParenting, false, true);
		global::BasePlayer basePlayer;
		if (this.OwnedByParentPlayer && (basePlayer = entityForParenting as global::BasePlayer) != null)
		{
			associatedEntity.OwnerID = basePlayer.userID;
		}
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x00112360 File Offset: 0x00110560
	public override void OnParentChanged(global::Item item)
	{
		base.OnParentChanged(item);
		this.UpdateParent(item);
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x00112370 File Offset: 0x00110570
	public global::BaseEntity GetEntityForParenting(global::Item item = null)
	{
		if (item == null)
		{
			return null;
		}
		global::BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			return ownerPlayer;
		}
		global::BaseEntity baseEntity = ((item.parent == null) ? null : item.parent.entityOwner);
		if (baseEntity != null)
		{
			return baseEntity;
		}
		global::BaseEntity worldEntity = item.GetWorldEntity();
		if (worldEntity)
		{
			return worldEntity;
		}
		if (this.AllowHeldEntityParenting && item.parentItem != null && item.parentItem.GetHeldEntity() != null)
		{
			return item.parentItem.GetHeldEntity();
		}
		return null;
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x001123F8 File Offset: 0x001105F8
	public static T GetAssociatedEntity(global::Item item, bool isServer = true)
	{
		if (((item != null) ? item.instanceData : null) == null)
		{
			return default(T);
		}
		global::BaseNetworkable baseNetworkable = null;
		if (isServer)
		{
			baseNetworkable = global::BaseNetworkable.serverEntities.Find(item.instanceData.subEntity);
		}
		if (baseNetworkable)
		{
			return baseNetworkable.GetComponent<T>();
		}
		return default(T);
	}

	// Token: 0x04002517 RID: 9495
	public GameObjectRef entityPrefab;
}
