using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class Marketplace : global::BaseEntity
{
	// Token: 0x060017A0 RID: 6048 RVA: 0x000B3454 File Offset: 0x000B1654
	public NetworkableId SendDrone(global::BasePlayer player, global::MarketTerminal sourceTerminal, global::VendingMachine vendingMachine)
	{
		if (sourceTerminal == null || vendingMachine == null)
		{
			return default(NetworkableId);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.deliveryDronePrefab;
		global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, this.droneLaunchPoint.position, this.droneLaunchPoint.rotation, true);
		global::DeliveryDrone deliveryDrone;
		if ((deliveryDrone = baseEntity as global::DeliveryDrone) == null)
		{
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			return default(NetworkableId);
		}
		deliveryDrone.OwnerID = player.userID;
		deliveryDrone.Spawn();
		deliveryDrone.Setup(this, sourceTerminal, vendingMachine);
		return deliveryDrone.net.ID;
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x000B34F4 File Offset: 0x000B16F4
	public void ReturnDrone(global::DeliveryDrone deliveryDrone)
	{
		global::MarketTerminal marketTerminal;
		if (deliveryDrone.sourceTerminal.TryGet(true, out marketTerminal))
		{
			marketTerminal.CompleteOrder(deliveryDrone.targetVendingMachine.uid);
		}
		deliveryDrone.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x000B3529 File Offset: 0x000B1729
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnSubEntities();
		}
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x000B3540 File Offset: 0x000B1740
	private void SpawnSubEntities()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.terminalEntities != null && this.terminalEntities.Length > this.terminalPoints.Length)
		{
			for (int i = this.terminalPoints.Length; i < this.terminalEntities.Length; i++)
			{
				global::MarketTerminal marketTerminal;
				if (this.terminalEntities[i].TryGet(true, out marketTerminal))
				{
					marketTerminal.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
		}
		Array.Resize<EntityRef<global::MarketTerminal>>(ref this.terminalEntities, this.terminalPoints.Length);
		for (int j = 0; j < this.terminalPoints.Length; j++)
		{
			Transform transform = this.terminalPoints[j];
			global::MarketTerminal marketTerminal2;
			if (!this.terminalEntities[j].TryGet(true, out marketTerminal2))
			{
				GameManager server = GameManager.server;
				GameObjectRef gameObjectRef = this.terminalPrefab;
				global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, transform.position, transform.rotation, true);
				baseEntity.SetParent(this, true, false);
				baseEntity.Spawn();
				global::MarketTerminal marketTerminal3;
				if ((marketTerminal3 = baseEntity as global::MarketTerminal) == null)
				{
					Debug.LogError("Marketplace.terminalPrefab did not spawn a MarketTerminal (it spawned " + baseEntity.GetType().FullName + ")");
					baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
				else
				{
					marketTerminal3.Setup(this);
					this.terminalEntities[j].Set(marketTerminal3);
				}
			}
		}
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x000B3684 File Offset: 0x000B1884
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.subEntityList != null)
		{
			List<NetworkableId> subEntityIds = info.msg.subEntityList.subEntityIds;
			Array.Resize<EntityRef<global::MarketTerminal>>(ref this.terminalEntities, subEntityIds.Count);
			for (int i = 0; i < subEntityIds.Count; i++)
			{
				this.terminalEntities[i] = new EntityRef<global::MarketTerminal>(subEntityIds[i]);
			}
		}
		this.SpawnSubEntities();
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x000B36F8 File Offset: 0x000B18F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.subEntityList = Pool.Get<SubEntityList>();
		info.msg.subEntityList.subEntityIds = Pool.GetList<NetworkableId>();
		if (this.terminalEntities != null)
		{
			for (int i = 0; i < this.terminalEntities.Length; i++)
			{
				info.msg.subEntityList.subEntityIds.Add(this.terminalEntities[i].uid);
			}
		}
	}

	// Token: 0x04001045 RID: 4165
	[Header("Marketplace")]
	public GameObjectRef terminalPrefab;

	// Token: 0x04001046 RID: 4166
	public Transform[] terminalPoints;

	// Token: 0x04001047 RID: 4167
	public Transform droneLaunchPoint;

	// Token: 0x04001048 RID: 4168
	public GameObjectRef deliveryDronePrefab;

	// Token: 0x04001049 RID: 4169
	public EntityRef<global::MarketTerminal>[] terminalEntities;
}
