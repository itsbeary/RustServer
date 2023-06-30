using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000466 RID: 1126
public class StabilityEntity : global::DecayEntity
{
	// Token: 0x0600254D RID: 9549 RVA: 0x000EBB54 File Offset: 0x000E9D54
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.stabilityEntity = Facepunch.Pool.Get<ProtoBuf.StabilityEntity>();
		info.msg.stabilityEntity.stability = this.cachedStability;
		info.msg.stabilityEntity.distanceFromGround = this.cachedDistanceFromGround;
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x000EBBA4 File Offset: 0x000E9DA4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.stabilityEntity != null)
		{
			this.cachedStability = info.msg.stabilityEntity.stability;
			this.cachedDistanceFromGround = info.msg.stabilityEntity.distanceFromGround;
			if (this.cachedStability <= 0f)
			{
				this.cachedStability = 0f;
			}
			if (this.cachedDistanceFromGround <= 0)
			{
				this.cachedDistanceFromGround = int.MaxValue;
			}
		}
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000EBC1D File Offset: 0x000E9E1D
	public override void ResetState()
	{
		base.ResetState();
		this.cachedStability = 0f;
		this.cachedDistanceFromGround = int.MaxValue;
		if (base.isServer)
		{
			this.supports = null;
			this.stabilityStrikes = 0;
			this.dirty = false;
		}
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000EBC58 File Offset: 0x000E9E58
	public void InitializeSupports()
	{
		this.supports = new List<global::StabilityEntity.Support>();
		if (this.grounded || base.HasParent())
		{
			return;
		}
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			if (entityLink.IsMale())
			{
				if (entityLink.socket is StabilitySocket)
				{
					this.supports.Add(new global::StabilityEntity.Support(this, entityLink, (entityLink.socket as StabilitySocket).support));
				}
				if (entityLink.socket is ConstructionSocket)
				{
					this.supports.Add(new global::StabilityEntity.Support(this, entityLink, (entityLink.socket as ConstructionSocket).support));
				}
			}
		}
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000EBD08 File Offset: 0x000E9F08
	public int DistanceFromGround(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded || base.HasParent())
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				int num2 = stabilityEntity.CachedDistanceFromGround(ignoreEntity);
				if (num2 != 2147483647)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x000EBD90 File Offset: 0x000E9F90
	public float SupportValue(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded || base.HasParent())
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity.Support support = this.supports[i];
			global::StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				float num2 = stabilityEntity.CachedSupportValue(ignoreEntity);
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000EBE2C File Offset: 0x000EA02C
	public int CachedDistanceFromGround(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded || base.HasParent())
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				int num2 = stabilityEntity.cachedDistanceFromGround;
				if (num2 != 2147483647)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000EBEB4 File Offset: 0x000EA0B4
	public float CachedSupportValue(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded || base.HasParent())
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity.Support support = this.supports[i];
			global::StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				float num2 = stabilityEntity.cachedStability;
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x000EBF50 File Offset: 0x000EA150
	public void StabilityCheck()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.supports == null)
		{
			this.InitializeSupports();
		}
		bool flag = false;
		int num = this.DistanceFromGround(null);
		if (num != this.cachedDistanceFromGround)
		{
			this.cachedDistanceFromGround = num;
			flag = true;
		}
		float num2 = this.SupportValue(null);
		if (Mathf.Abs(this.cachedStability - num2) > Stability.accuracy)
		{
			this.cachedStability = num2;
			flag = true;
		}
		if (flag)
		{
			this.dirty = true;
			this.UpdateConnectedEntities();
			this.UpdateStability();
		}
		else if (this.dirty)
		{
			this.dirty = false;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (num2 >= Stability.collapse)
		{
			this.stabilityStrikes = 0;
			return;
		}
		if (this.stabilityStrikes < Stability.strikes)
		{
			this.UpdateStability();
			this.stabilityStrikes++;
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x000EC01C File Offset: 0x000EA21C
	public void UpdateStability()
	{
		global::StabilityEntity.stabilityCheckQueue.Add(this);
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x000EC02C File Offset: 0x000EA22C
	public void UpdateSurroundingEntities()
	{
		global::StabilityEntity.updateSurroundingsQueue.Add(this.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x000EC054 File Offset: 0x000EA254
	public void UpdateConnectedEntities()
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			if (entityLink.IsFemale())
			{
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::StabilityEntity stabilityEntity = entityLink.connections[j].owner as global::StabilityEntity;
					if (!(stabilityEntity == null) && !stabilityEntity.isClient && !stabilityEntity.IsDestroyed)
					{
						stabilityEntity.UpdateStability();
					}
				}
			}
		}
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x000EC0DB File Offset: 0x000EA2DB
	protected void OnPhysicsNeighbourChanged()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this.StabilityCheck();
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000EC0EC File Offset: 0x000EA2EC
	protected void DebugNudge()
	{
		this.StabilityCheck();
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x000EC0F4 File Offset: 0x000EA2F4
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateStability();
		}
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000EC109 File Offset: 0x000EA309
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.UpdateSurroundingEntities();
	}

	// Token: 0x04001D86 RID: 7558
	public static global::StabilityEntity.StabilityCheckWorkQueue stabilityCheckQueue = new global::StabilityEntity.StabilityCheckWorkQueue();

	// Token: 0x04001D87 RID: 7559
	public static global::StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue = new global::StabilityEntity.UpdateSurroundingsQueue();

	// Token: 0x04001D88 RID: 7560
	public bool grounded;

	// Token: 0x04001D89 RID: 7561
	[NonSerialized]
	public float cachedStability;

	// Token: 0x04001D8A RID: 7562
	[NonSerialized]
	public int cachedDistanceFromGround = int.MaxValue;

	// Token: 0x04001D8B RID: 7563
	private List<global::StabilityEntity.Support> supports;

	// Token: 0x04001D8C RID: 7564
	private int stabilityStrikes;

	// Token: 0x04001D8D RID: 7565
	private bool dirty;

	// Token: 0x02000D09 RID: 3337
	public class StabilityCheckWorkQueue : ObjectWorkQueue<global::StabilityEntity>
	{
		// Token: 0x0600502F RID: 20527 RVA: 0x001A8399 File Offset: 0x001A6599
		protected override void RunJob(global::StabilityEntity entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.StabilityCheck();
		}

		// Token: 0x06005030 RID: 20528 RVA: 0x001A83AB File Offset: 0x001A65AB
		protected override bool ShouldAdd(global::StabilityEntity entity)
		{
			return ConVar.Server.stability && entity.IsValid() && entity.isServer;
		}
	}

	// Token: 0x02000D0A RID: 3338
	public class UpdateSurroundingsQueue : ObjectWorkQueue<Bounds>
	{
		// Token: 0x06005032 RID: 20530 RVA: 0x001A83D4 File Offset: 0x001A65D4
		protected override void RunJob(Bounds bounds)
		{
			if (!ConVar.Server.stability)
			{
				return;
			}
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(bounds.center, bounds.extents.magnitude + 1f, list, 69372162, QueryTriggerInteraction.Collide);
			foreach (global::BaseEntity baseEntity in list)
			{
				if (!baseEntity.IsDestroyed && !baseEntity.isClient)
				{
					if (baseEntity is global::StabilityEntity)
					{
						(baseEntity as global::StabilityEntity).OnPhysicsNeighbourChanged();
					}
					else
					{
						baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x02000D0B RID: 3339
	private class Support
	{
		// Token: 0x06005034 RID: 20532 RVA: 0x001A8494 File Offset: 0x001A6694
		public Support(global::StabilityEntity parent, EntityLink link, float factor)
		{
			this.parent = parent;
			this.link = link;
			this.factor = factor;
		}

		// Token: 0x06005035 RID: 20533 RVA: 0x001A84BC File Offset: 0x001A66BC
		public global::StabilityEntity SupportEntity(global::StabilityEntity ignoreEntity = null)
		{
			global::StabilityEntity stabilityEntity = null;
			for (int i = 0; i < this.link.connections.Count; i++)
			{
				global::StabilityEntity stabilityEntity2 = this.link.connections[i].owner as global::StabilityEntity;
				Socket_Base socket = this.link.connections[i].socket;
				ConstructionSocket constructionSocket;
				if (!(stabilityEntity2 == null) && !(stabilityEntity2 == this.parent) && !(stabilityEntity2 == ignoreEntity) && !stabilityEntity2.isClient && !stabilityEntity2.IsDestroyed && ((constructionSocket = socket as ConstructionSocket) == null || !constructionSocket.femaleNoStability) && (stabilityEntity == null || stabilityEntity2.cachedDistanceFromGround < stabilityEntity.cachedDistanceFromGround))
				{
					stabilityEntity = stabilityEntity2;
				}
			}
			return stabilityEntity;
		}

		// Token: 0x0400468D RID: 18061
		public global::StabilityEntity parent;

		// Token: 0x0400468E RID: 18062
		public EntityLink link;

		// Token: 0x0400468F RID: 18063
		public float factor = 1f;
	}
}
