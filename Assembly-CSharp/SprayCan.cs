using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x020000D5 RID: 213
public class SprayCan : HeldEntity
{
	// Token: 0x060012ED RID: 4845 RVA: 0x00097B1C File Offset: 0x00095D1C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCan.OnRpcMessage", 0))
		{
			if (rpc == 3490735573U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginFreehandSpray ");
				}
				using (TimeWarning.New("BeginFreehandSpray", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3490735573U, "BeginFreehandSpray", this, player))
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
							this.BeginFreehandSpray(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BeginFreehandSpray");
					}
				}
				return true;
			}
			if (rpc == 151738090U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ChangeItemSkin ");
				}
				using (TimeWarning.New("ChangeItemSkin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(151738090U, "ChangeItemSkin", this, player, 2UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(151738090U, "ChangeItemSkin", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ChangeItemSkin(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ChangeItemSkin");
					}
				}
				return true;
			}
			if (rpc == 396000799U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CreateSpray ");
				}
				using (TimeWarning.New("CreateSpray", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(396000799U, "CreateSpray", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CreateSpray(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in CreateSpray");
					}
				}
				return true;
			}
			if (rpc == 14517645U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetBlockColourId ");
				}
				using (TimeWarning.New("Server_SetBlockColourId", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(14517645U, "Server_SetBlockColourId", this, player, 3UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(14517645U, "Server_SetBlockColourId", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_SetBlockColourId(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_SetBlockColourId");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000980F8 File Offset: 0x000962F8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void BeginFreehandSpray(BaseEntity.RPCMessage msg)
	{
		if (base.IsBusy())
		{
			return;
		}
		if (!this.CanSprayFreehand(msg.player))
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3();
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		if (num < 0 || num >= this.SprayColours.Length || num2 < 0 || num2 >= this.SprayWidths.Length)
		{
			return;
		}
		if (Vector3.Distance(vector, base.GetOwnerPlayer().transform.position) > 3f)
		{
			return;
		}
		SprayCanSpray_Freehand sprayCanSpray_Freehand = GameManager.server.CreateEntity(this.LinePrefab.resourcePath, vector, Quaternion.identity, true) as SprayCanSpray_Freehand;
		sprayCanSpray_Freehand.AddInitialPoint(vector2);
		sprayCanSpray_Freehand.SetColour(this.SprayColours[num]);
		sprayCanSpray_Freehand.SetWidth(this.SprayWidths[num2]);
		sprayCanSpray_Freehand.EnableChanges(msg.player);
		sprayCanSpray_Freehand.Spawn();
		this.paintingLine = sprayCanSpray_Freehand;
		base.ClientRPC<int>(null, "Client_ChangeSprayColour", num);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		this.CheckAchievementPosition(vector);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00098223 File Offset: 0x00096423
	public void ClearPaintingLine(bool allowNewSprayImmediately)
	{
		this.paintingLine = null;
		this.LoseCondition(this.ConditionLossPerSpray);
		if (allowNewSprayImmediately)
		{
			this.ClearBusy();
			return;
		}
		base.Invoke(new Action(this.ClearBusy), 0.1f);
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x0009825C File Offset: 0x0009645C
	public bool CanSprayFreehand(BasePlayer player)
	{
		return player.UnlockAllSkins || (this.FreeSprayUnlockItem != null && (player.blueprints.steamInventory.HasItem(this.FreeSprayUnlockItem.id) || this.FreeSprayUnlockItem.HasUnlocked(player.userID)));
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x000982B4 File Offset: 0x000964B4
	private bool IsSprayBlockedByTrigger(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return true;
		}
		TriggerNoSpray triggerNoSpray = ownerPlayer.FindTrigger<TriggerNoSpray>();
		return !(triggerNoSpray == null) && !triggerNoSpray.IsPositionValid(pos);
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x000982F0 File Offset: 0x000964F0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void ChangeItemSkin(BaseEntity.RPCMessage msg)
	{
		SprayCan.<>c__DisplayClass34_0 CS$<>8__locals1 = new SprayCan.<>c__DisplayClass34_0();
		CS$<>8__locals1.<>4__this = this;
		if (base.IsBusy())
		{
			return;
		}
		NetworkableId networkableId = msg.read.EntityID();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(networkableId);
		CS$<>8__locals1.targetSkin = msg.read.Int32();
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		bool flag = false;
		if (msg.player.UnlockAllSkins)
		{
			flag = true;
		}
		if (CS$<>8__locals1.targetSkin != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(CS$<>8__locals1.targetSkin, msg.player.userID))
		{
			CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.SkinNotOwned);
			return;
		}
		BaseEntity baseEntity;
		if (baseNetworkable != null && (baseEntity = baseNetworkable as BaseEntity) != null)
		{
			Vector3 vector = baseEntity.WorldSpaceBounds().ClosestPoint(msg.player.eyes.position);
			if (!msg.player.IsVisible(vector, 3f))
			{
				CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.LineOfSight);
				return;
			}
			Door door;
			if ((door = baseNetworkable as Door) != null)
			{
				if (!door.GetPlayerLockPermission(msg.player))
				{
					msg.player.ChatMessage("Door must be openable");
					return;
				}
				if (door.IsOpen())
				{
					msg.player.ChatMessage("Door must be closed");
					return;
				}
			}
			ItemDefinition itemDefinition;
			if (!SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition, true))
			{
				CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.InvalidItem);
				return;
			}
			ItemDefinition itemDefinition2 = null;
			ulong num = ItemDefinition.FindSkin(itemDefinition.itemid, CS$<>8__locals1.targetSkin);
			ItemSkinDirectory.Skin skin = itemDefinition.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => x.id == CS$<>8__locals1.targetSkin);
			ItemSkin itemSkin;
			if (skin.invItem != null && (itemSkin = skin.invItem as ItemSkin) != null)
			{
				if (itemSkin.Redirect != null)
				{
					itemDefinition2 = itemSkin.Redirect;
				}
				else if (SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition, false) && itemDefinition.isRedirectOf != null)
				{
					itemDefinition2 = itemDefinition.isRedirectOf;
				}
			}
			else if (itemDefinition.isRedirectOf != null || (SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition, false) && itemDefinition.isRedirectOf != null))
			{
				itemDefinition2 = itemDefinition.isRedirectOf;
			}
			if (itemDefinition2 == null)
			{
				baseEntity.skinID = num;
				baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				Analytics.Server.SkinUsed(itemDefinition.shortname, CS$<>8__locals1.targetSkin);
			}
			else
			{
				SprayCan.SprayFailReason sprayFailReason;
				if (!this.CanEntityBeRespawned(baseEntity, out sprayFailReason))
				{
					CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(sprayFailReason);
					return;
				}
				string text;
				if (!this.GetEntityPrefabPath(itemDefinition2, out text))
				{
					Debug.LogWarning("Cannot find resource path of redirect entity to spawn! " + itemDefinition2.gameObject.name);
					CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.InvalidItem);
					return;
				}
				Vector3 localPosition = baseEntity.transform.localPosition;
				Quaternion localRotation = baseEntity.transform.localRotation;
				BaseEntity parentEntity = baseEntity.GetParentEntity();
				float num2 = baseEntity.Health();
				EntityRef[] slots = baseEntity.GetSlots();
				BaseCombatEntity baseCombatEntity;
				float num3 = (((baseCombatEntity = baseEntity as BaseCombatEntity) != null) ? baseCombatEntity.lastAttackedTime : 0f);
				bool flag2 = baseEntity is Door;
				Dictionary<SprayCan.ContainerSet, List<Item>> dictionary = new Dictionary<SprayCan.ContainerSet, List<Item>>();
				SprayCan.<ChangeItemSkin>g__SaveEntityStorage|34_0(baseEntity, dictionary, 0);
				List<SprayCan.ChildPreserveInfo> list = Facepunch.Pool.GetList<SprayCan.ChildPreserveInfo>();
				if (flag2)
				{
					foreach (BaseEntity baseEntity2 in baseEntity.children)
					{
						list.Add(new SprayCan.ChildPreserveInfo
						{
							TargetEntity = baseEntity2,
							TargetBone = baseEntity2.parentBone,
							LocalPosition = baseEntity2.transform.localPosition,
							LocalRotation = baseEntity2.transform.localRotation
						});
					}
					using (List<SprayCan.ChildPreserveInfo>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SprayCan.ChildPreserveInfo childPreserveInfo = enumerator2.Current;
							childPreserveInfo.TargetEntity.SetParent(null, true, false);
						}
						goto IL_3F4;
					}
				}
				for (int i = 0; i < baseEntity.children.Count; i++)
				{
					SprayCan.<ChangeItemSkin>g__SaveEntityStorage|34_0(baseEntity.children[i], dictionary, -1);
				}
				IL_3F4:
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
				baseEntity = GameManager.server.CreateEntity(text, (parentEntity != null) ? parentEntity.transform.TransformPoint(localPosition) : localPosition, (parentEntity != null) ? (parentEntity.transform.rotation * localRotation) : localRotation, true);
				baseEntity.SetParent(parentEntity, false, false);
				baseEntity.transform.localPosition = localPosition;
				baseEntity.transform.localRotation = localRotation;
				ItemDefinition itemDefinition3;
				if (SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition3, false) && itemDefinition3.isRedirectOf != null)
				{
					baseEntity.skinID = 0UL;
				}
				else
				{
					baseEntity.skinID = num;
				}
				DecayEntity decayEntity;
				if ((decayEntity = baseEntity as DecayEntity) != null)
				{
					decayEntity.AttachToBuilding(null);
				}
				baseEntity.Spawn();
				BaseCombatEntity baseCombatEntity2;
				if ((baseCombatEntity2 = baseEntity as BaseCombatEntity) != null)
				{
					baseCombatEntity2.SetHealth(num2);
					baseCombatEntity2.lastAttackedTime = num3;
				}
				if (dictionary.Count > 0)
				{
					SprayCan.<ChangeItemSkin>g__RestoreEntityStorage|34_1(baseEntity, 0, dictionary);
					if (!flag2)
					{
						for (int j = 0; j < baseEntity.children.Count; j++)
						{
							SprayCan.<ChangeItemSkin>g__RestoreEntityStorage|34_1(baseEntity.children[j], -1, dictionary);
						}
					}
					foreach (KeyValuePair<SprayCan.ContainerSet, List<Item>> keyValuePair in dictionary)
					{
						foreach (Item item in keyValuePair.Value)
						{
							Debug.Log(string.Format("Deleting {0} as it has no new container", item));
							item.Remove(0f);
						}
					}
					Analytics.Server.SkinUsed(itemDefinition.shortname, CS$<>8__locals1.targetSkin);
				}
				if (flag2)
				{
					foreach (SprayCan.ChildPreserveInfo childPreserveInfo2 in list)
					{
						childPreserveInfo2.TargetEntity.SetParent(baseEntity, childPreserveInfo2.TargetBone, true, false);
						childPreserveInfo2.TargetEntity.transform.localPosition = childPreserveInfo2.LocalPosition;
						childPreserveInfo2.TargetEntity.transform.localRotation = childPreserveInfo2.LocalRotation;
						childPreserveInfo2.TargetEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
					}
					baseEntity.SetSlots(slots);
				}
				Facepunch.Pool.FreeList<SprayCan.ChildPreserveInfo>(ref list);
			}
			base.ClientRPC<int, NetworkableId>(null, "Client_ReskinResult", 1, baseEntity.net.ID);
		}
		this.LoseCondition(this.ConditionLossPerReskin);
		base.ClientRPC<int>(null, "Client_ChangeSprayColour", -1);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.ClearBusy), this.SprayCooldown);
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x000989E0 File Offset: 0x00096BE0
	private bool GetEntityPrefabPath(ItemDefinition def, out string resourcePath)
	{
		resourcePath = string.Empty;
		ItemModDeployable itemModDeployable;
		if (def.TryGetComponent<ItemModDeployable>(out itemModDeployable))
		{
			resourcePath = itemModDeployable.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntity itemModEntity;
		if (def.TryGetComponent<ItemModEntity>(out itemModEntity))
		{
			resourcePath = itemModEntity.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntityReference itemModEntityReference;
		if (def.TryGetComponent<ItemModEntityReference>(out itemModEntityReference))
		{
			resourcePath = itemModEntityReference.entityPrefab.resourcePath;
			return true;
		}
		return false;
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00098A40 File Offset: 0x00096C40
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void CreateSpray(BaseEntity.RPCMessage msg)
	{
		if (base.IsBusy())
		{
			return;
		}
		base.ClientRPC<int>(null, "Client_ChangeSprayColour", -1);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.ClearBusy), this.SprayCooldown);
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3();
		Vector3 vector3 = msg.read.Vector3();
		int num = msg.read.Int32();
		if (Vector3.Distance(vector, base.transform.position) > 4.5f)
		{
			return;
		}
		Plane plane = new Plane(vector2, vector);
		Quaternion quaternion = Quaternion.LookRotation((plane.ClosestPointOnPlane(vector3) - vector).normalized, vector2);
		quaternion *= Quaternion.Euler(0f, 0f, 90f);
		bool flag = false;
		if (msg.player.IsDeveloper)
		{
			flag = true;
		}
		if (num != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(num, msg.player.userID))
		{
			Debug.Log(string.Format("SprayCan.ChangeItemSkin player does not have item :{0}:", num));
			return;
		}
		ulong num2 = ItemDefinition.FindSkin(this.SprayDecalItem.itemid, num);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.SprayDecalEntityRef.resourcePath, vector, quaternion, true);
		baseEntity.skinID = num2;
		baseEntity.OnDeployed(null, base.GetOwnerPlayer(), this.GetItem());
		baseEntity.Spawn();
		this.CheckAchievementPosition(vector);
		this.LoseCondition(this.ConditionLossPerSpray);
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x000063A5 File Offset: 0x000045A5
	private void CheckAchievementPosition(Vector3 pos)
	{
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00098BC4 File Offset: 0x00096DC4
	private void LoseCondition(float amount)
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(amount);
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x00098BE3 File Offset: 0x00096DE3
	public void ClearBusy()
	{
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x00098C01 File Offset: 0x00096E01
	public override void OnHeldChanged()
	{
		if (base.IsDisabled())
		{
			this.ClearBusy();
			if (this.paintingLine != null)
			{
				this.paintingLine.Kill(BaseNetworkable.DestroyMode.None);
			}
			this.paintingLine = null;
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x00098C34 File Offset: 0x00096E34
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void Server_SetBlockColourId(BaseEntity.RPCMessage msg)
	{
		NetworkableId networkableId = msg.read.EntityID();
		uint num = msg.read.UInt32();
		BasePlayer player = msg.player;
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.ClearBusy), 0.1f);
		if (player == null || !player.CanBuild())
		{
			return;
		}
		BuildingBlock buildingBlock = BaseNetworkable.serverEntities.Find(networkableId) as BuildingBlock;
		if (buildingBlock != null)
		{
			if (player.Distance(buildingBlock) > 4f)
			{
				return;
			}
			buildingBlock.SetCustomColour(num);
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null)
		{
			ownerPlayer.LastBlockColourChangeId = num;
		}
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x00098CE4 File Offset: 0x00096EE4
	private bool CanEntityBeRespawned(BaseEntity targetEntity, out SprayCan.SprayFailReason reason)
	{
		BaseMountable baseMountable;
		if ((baseMountable = targetEntity as BaseMountable) != null && baseMountable.AnyMounted())
		{
			reason = SprayCan.SprayFailReason.MountedBlocked;
			return false;
		}
		BaseVehicle baseVehicle;
		if (targetEntity.isServer && (baseVehicle = targetEntity as BaseVehicle) != null && (baseVehicle.HasDriver() || baseVehicle.AnyMounted()))
		{
			reason = SprayCan.SprayFailReason.MountedBlocked;
			return false;
		}
		IOEntity ioentity;
		if ((ioentity = targetEntity as IOEntity) != null && (ioentity.GetConnectedInputCount() != 0 || ioentity.GetConnectedOutputCount() != 0))
		{
			reason = SprayCan.SprayFailReason.IOConnection;
			return false;
		}
		reason = SprayCan.SprayFailReason.None;
		return true;
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x00098D54 File Offset: 0x00096F54
	public static bool GetItemDefinitionForEntity(BaseEntity be, out ItemDefinition def, bool useRedirect = true)
	{
		def = null;
		BaseCombatEntity baseCombatEntity;
		if ((baseCombatEntity = be as BaseCombatEntity) != null)
		{
			if (baseCombatEntity.pickup.enabled && baseCombatEntity.pickup.itemTarget != null)
			{
				def = baseCombatEntity.pickup.itemTarget;
			}
			else if (baseCombatEntity.repair.enabled && baseCombatEntity.repair.itemTarget != null)
			{
				def = baseCombatEntity.repair.itemTarget;
			}
		}
		if (useRedirect && def != null && def.isRedirectOf != null)
		{
			def = def.isRedirectOf;
		}
		return def != null;
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x00098ED8 File Offset: 0x000970D8
	[CompilerGenerated]
	internal static void <ChangeItemSkin>g__SaveEntityStorage|34_0(BaseEntity baseEntity, Dictionary<SprayCan.ContainerSet, List<Item>> dictionary, int index)
	{
		IItemContainerEntity itemContainerEntity;
		if ((itemContainerEntity = baseEntity as IItemContainerEntity) != null)
		{
			SprayCan.ContainerSet containerSet = new SprayCan.ContainerSet
			{
				ContainerIndex = index,
				PrefabId = ((index == 0) ? 0U : baseEntity.prefabID)
			};
			if (dictionary.ContainsKey(containerSet))
			{
				Debug.Log("Multiple containers with the same prefab id being added during vehicle reskin");
				return;
			}
			dictionary.Add(containerSet, new List<Item>());
			foreach (Item item in itemContainerEntity.inventory.itemList)
			{
				dictionary[containerSet].Add(item);
			}
			foreach (Item item2 in dictionary[containerSet])
			{
				item2.RemoveFromContainer();
			}
		}
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x00098FCC File Offset: 0x000971CC
	[CompilerGenerated]
	internal static void <ChangeItemSkin>g__RestoreEntityStorage|34_1(BaseEntity baseEntity, int index, Dictionary<SprayCan.ContainerSet, List<Item>> copy)
	{
		IItemContainerEntity itemContainerEntity;
		if ((itemContainerEntity = baseEntity as IItemContainerEntity) != null)
		{
			SprayCan.ContainerSet containerSet = new SprayCan.ContainerSet
			{
				ContainerIndex = index,
				PrefabId = ((index == 0) ? 0U : baseEntity.prefabID)
			};
			if (copy.ContainsKey(containerSet))
			{
				foreach (Item item in copy[containerSet])
				{
					item.MoveToContainer(itemContainerEntity.inventory, -1, true, false, null, true);
				}
				copy.Remove(containerSet);
			}
		}
	}

	// Token: 0x04000BD5 RID: 3029
	public const float MaxFreeSprayDistanceFromStart = 10f;

	// Token: 0x04000BD6 RID: 3030
	public const float MaxFreeSprayStartingDistance = 3f;

	// Token: 0x04000BD7 RID: 3031
	private SprayCanSpray_Freehand paintingLine;

	// Token: 0x04000BD8 RID: 3032
	public const BaseEntity.Flags IsFreeSpraying = BaseEntity.Flags.Reserved1;

	// Token: 0x04000BD9 RID: 3033
	public SoundDefinition SpraySound;

	// Token: 0x04000BDA RID: 3034
	public GameObjectRef SkinSelectPanel;

	// Token: 0x04000BDB RID: 3035
	public float SprayCooldown = 2f;

	// Token: 0x04000BDC RID: 3036
	public float ConditionLossPerSpray = 10f;

	// Token: 0x04000BDD RID: 3037
	public float ConditionLossPerReskin = 10f;

	// Token: 0x04000BDE RID: 3038
	public GameObjectRef LinePrefab;

	// Token: 0x04000BDF RID: 3039
	public Color[] SprayColours = new Color[0];

	// Token: 0x04000BE0 RID: 3040
	public float[] SprayWidths = new float[] { 0.1f, 0.2f, 0.3f };

	// Token: 0x04000BE1 RID: 3041
	public ParticleSystem worldSpaceSprayFx;

	// Token: 0x04000BE2 RID: 3042
	public GameObjectRef ReskinEffect;

	// Token: 0x04000BE3 RID: 3043
	public ItemDefinition SprayDecalItem;

	// Token: 0x04000BE4 RID: 3044
	public GameObjectRef SprayDecalEntityRef;

	// Token: 0x04000BE5 RID: 3045
	public SteamInventoryItem FreeSprayUnlockItem;

	// Token: 0x04000BE6 RID: 3046
	public ParticleSystem.MinMaxGradient DecalSprayGradient;

	// Token: 0x04000BE7 RID: 3047
	public SoundDefinition SprayLoopDef;

	// Token: 0x04000BE8 RID: 3048
	public static Translate.Phrase FreeSprayNamePhrase = new Translate.Phrase("freespray_radial", "Free Spray");

	// Token: 0x04000BE9 RID: 3049
	public static Translate.Phrase FreeSprayDescPhrase = new Translate.Phrase("freespray_radial_desc", "Spray shapes freely with various colors");

	// Token: 0x04000BEA RID: 3050
	public static Translate.Phrase BuildingSkinDefaultPhrase = new Translate.Phrase("buildingskin_default", "Automatic colour");

	// Token: 0x04000BEB RID: 3051
	public static Translate.Phrase BuildingSkinDefaultDescPhrase = new Translate.Phrase("buildingskin_default_desc", "Reset the block to random colouring");

	// Token: 0x04000BEC RID: 3052
	public static Translate.Phrase BuildingSkinColourPhrase = new Translate.Phrase("buildingskin_colour", "Set colour");

	// Token: 0x04000BED RID: 3053
	public static Translate.Phrase BuildingSkinColourDescPhrase = new Translate.Phrase("buildingskin_colour_desc", "Set the block to the highlighted colour");

	// Token: 0x04000BEE RID: 3054
	[FormerlySerializedAs("ShippingCOntainerColourLookup")]
	public ConstructionSkin_ColourLookup ShippingContainerColourLookup;

	// Token: 0x04000BEF RID: 3055
	public const string ENEMY_BASE_STAT = "sprayed_enemy_base";

	// Token: 0x02000C12 RID: 3090
	private enum SprayFailReason
	{
		// Token: 0x04004257 RID: 16983
		None,
		// Token: 0x04004258 RID: 16984
		MountedBlocked,
		// Token: 0x04004259 RID: 16985
		IOConnection,
		// Token: 0x0400425A RID: 16986
		LineOfSight,
		// Token: 0x0400425B RID: 16987
		SkinNotOwned,
		// Token: 0x0400425C RID: 16988
		InvalidItem
	}

	// Token: 0x02000C13 RID: 3091
	private struct ContainerSet
	{
		// Token: 0x0400425D RID: 16989
		public int ContainerIndex;

		// Token: 0x0400425E RID: 16990
		public uint PrefabId;
	}

	// Token: 0x02000C14 RID: 3092
	private struct ChildPreserveInfo
	{
		// Token: 0x0400425F RID: 16991
		public BaseEntity TargetEntity;

		// Token: 0x04004260 RID: 16992
		public uint TargetBone;

		// Token: 0x04004261 RID: 16993
		public Vector3 LocalPosition;

		// Token: 0x04004262 RID: 16994
		public Quaternion LocalRotation;
	}
}
