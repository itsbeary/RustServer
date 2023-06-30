using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class InvisibleVendingMachine : NPCVendingMachine
{
	// Token: 0x060016C4 RID: 5828 RVA: 0x000AF570 File Offset: 0x000AD770
	public NPCShopKeeper GetNPCShopKeeper()
	{
		List<NPCShopKeeper> list = Pool.GetList<NPCShopKeeper>();
		Vis.Entities<NPCShopKeeper>(base.transform.position, 2f, list, 131072, QueryTriggerInteraction.Collide);
		NPCShopKeeper npcshopKeeper = null;
		if (list.Count > 0)
		{
			npcshopKeeper = list[0];
		}
		Pool.FreeList<NPCShopKeeper>(ref list);
		return npcshopKeeper;
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x000AF5BC File Offset: 0x000AD7BC
	public void KeeperLookAt(Vector3 pos)
	{
		NPCShopKeeper npcshopKeeper = this.GetNPCShopKeeper();
		if (npcshopKeeper == null)
		{
			return;
		}
		npcshopKeeper.SetAimDirection(Vector3Ex.Direction2D(pos, npcshopKeeper.transform.position));
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool HasVendingSounds()
	{
		return false;
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x00029084 File Offset: 0x00027284
	public override float GetBuyDuration()
	{
		return 0.5f;
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x000AF5F4 File Offset: 0x000AD7F4
	public override void CompletePendingOrder()
	{
		Effect.server.Run(this.buyEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		NPCShopKeeper npcshopKeeper = this.GetNPCShopKeeper();
		if (npcshopKeeper)
		{
			npcshopKeeper.SignalBroadcast(BaseEntity.Signal.Gesture, "victory", null);
			if (this.vend_Player != null)
			{
				npcshopKeeper.SetAimDirection(Vector3Ex.Direction2D(this.vend_Player.transform.position, npcshopKeeper.transform.position));
			}
		}
		base.CompletePendingOrder();
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x000AF67A File Offset: 0x000AD87A
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		this.KeeperLookAt(player.transform.position);
		return base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x000AF698 File Offset: 0x000AD898
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			info.msg.vendingMachine.vmoIndex = this.vmoManifest.GetIndex(this.vendingOrders);
		}
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x000AF6E8 File Offset: 0x000AD8E8
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.vmoManifest.GetIndex(this.vendingOrders) == -1)
		{
			Debug.LogError("VENDING ORDERS NOT FOUND! Did you forget to add these orders to the VMOManifest?");
		}
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x000AF710 File Offset: 0x000AD910
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			if (info.msg.vendingMachine.vmoIndex == -1 && TerrainMeta.Path.Monuments != null)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo.displayPhrase.token.Contains("fish") && Vector3.Distance(monumentInfo.transform.position, base.transform.position) < 100f)
					{
						info.msg.vendingMachine.vmoIndex = 17;
					}
				}
			}
			NPCVendingOrder fromIndex = this.vmoManifest.GetFromIndex(info.msg.vendingMachine.vmoIndex);
			this.vendingOrders = fromIndex;
		}
	}

	// Token: 0x04000EDE RID: 3806
	public GameObjectRef buyEffect;

	// Token: 0x04000EDF RID: 3807
	public NPCVendingOrderManifest vmoManifest;
}
