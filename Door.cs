using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Token: 0x0200006A RID: 106
public class Door : AnimatedBuildingBlock, INotifyTrigger
{
	// Token: 0x06000A7F RID: 2687 RVA: 0x0005FEF4 File Offset: 0x0005E0F4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Door.OnRpcMessage", 0))
		{
			if (rpc == 3999508679U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_CloseDoor ");
				}
				using (TimeWarning.New("RPC_CloseDoor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3999508679U, "RPC_CloseDoor", this, player, 3f))
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
							this.RPC_CloseDoor(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_CloseDoor");
					}
				}
				return true;
			}
			if (rpc == 1487779344U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_KnockDoor ");
				}
				using (TimeWarning.New("RPC_KnockDoor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1487779344U, "RPC_KnockDoor", this, player, 3f))
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
							this.RPC_KnockDoor(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_KnockDoor");
					}
				}
				return true;
			}
			if (rpc == 3314360565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenDoor ");
				}
				using (TimeWarning.New("RPC_OpenDoor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3314360565U, "RPC_OpenDoor", this, player, 3f))
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
							this.RPC_OpenDoor(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenDoor");
					}
				}
				return true;
			}
			if (rpc == 3000490601U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ToggleHatch ");
				}
				using (TimeWarning.New("RPC_ToggleHatch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3000490601U, "RPC_ToggleHatch", this, player, 3f))
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
							this.RPC_ToggleHatch(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_ToggleHatch");
					}
				}
				return true;
			}
			if (rpc == 3672787865U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_NotifyWoundedClose ");
				}
				using (TimeWarning.New("Server_NotifyWoundedClose", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3672787865U, "Server_NotifyWoundedClose", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_NotifyWoundedClose(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in Server_NotifyWoundedClose");
					}
				}
				return true;
			}
			if (rpc == 3730851545U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_NotifyWoundedOpen ");
				}
				using (TimeWarning.New("Server_NotifyWoundedOpen", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3730851545U, "Server_NotifyWoundedOpen", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpcmessage6 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_NotifyWoundedOpen(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in Server_NotifyWoundedOpen");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00060760 File Offset: 0x0005E960
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.decayResetTimeLast = float.NegativeInfinity;
			if (this.isSecurityDoor && this.NavMeshLink != null)
			{
				this.SetNavMeshLinkEnabled(false);
			}
			this.woundedCloses.Clear();
			this.woundedOpens.Clear();
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x000607BC File Offset: 0x0005E9BC
	public override void ServerInit()
	{
		base.ServerInit();
		if (Door.nonWalkableArea < 0)
		{
			Door.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Door.animalAgentTypeId < 0)
		{
			Door.animalAgentTypeId = NavMesh.GetSettingsByIndex(1).agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Door.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Door.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (this.HasSlot(BaseEntity.Slot.Lock))
		{
			this.canNpcOpen = false;
		}
		if (!this.canNpcOpen)
		{
			if (Door.humanoidAgentTypeId < 0)
			{
				Door.humanoidAgentTypeId = NavMesh.GetSettingsByIndex(0).agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Door.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Door.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = Vector3.one + Vector3.up + Vector3.forward;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			if (this.isSecurityDoor)
			{
				NavMeshObstacle navMeshObstacle = base.gameObject.AddComponent<NavMeshObstacle>();
				navMeshObstacle.carving = true;
				navMeshObstacle.center = Vector3.zero;
				navMeshObstacle.size = Vector3.one;
				navMeshObstacle.shape = NavMeshObstacleShape.Box;
			}
			this.NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCDoorTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
		AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, true);
		if (forPoint != null && this.NavMeshLink == null)
		{
			this.NavMeshLink = forPoint.GetClosestNavMeshLink(base.transform.position);
		}
		this.DisableVehiclePhysBox();
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x000609B9 File Offset: 0x0005EBB9
	public override bool HasSlot(BaseEntity.Slot slot)
	{
		return (slot == BaseEntity.Slot.Lock && this.canTakeLock) || slot == BaseEntity.Slot.UpperModifier || (slot == BaseEntity.Slot.CenterDecoration && this.canTakeCloser) || (slot == BaseEntity.Slot.LowerCenterDecoration && this.canTakeKnocker) || base.HasSlot(slot);
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x000609F4 File Offset: 0x0005EBF4
	public override bool CanPickup(BasePlayer player)
	{
		return base.IsOpen() && !base.GetSlot(BaseEntity.Slot.Lock) && !base.GetSlot(BaseEntity.Slot.UpperModifier) && !base.GetSlot(BaseEntity.Slot.CenterDecoration) && !base.GetSlot(BaseEntity.Slot.LowerCenterDecoration) && base.CanPickup(player);
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00060A52 File Offset: 0x0005EC52
	public void CloseRequest()
	{
		this.SetOpen(false, false);
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00060A5C File Offset: 0x0005EC5C
	public void SetOpen(bool open, bool suppressBlockageChecks = false)
	{
		base.SetFlag(BaseEntity.Flags.Open, open, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(open);
		}
		if (!suppressBlockageChecks && (!open || this.checkPhysBoxesOnOpen))
		{
			this.StartCheckingForBlockages(open);
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00060AB2 File Offset: 0x0005ECB2
	public void SetLocked(bool locked)
	{
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00060AC8 File Offset: 0x0005ECC8
	public bool GetPlayerLockPermission(BasePlayer player)
	{
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		return baseLock == null || baseLock.GetPlayerLockPermission(player);
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x00060AF4 File Offset: 0x0005ECF4
	public void SetNavMeshLinkEnabled(bool wantsOn)
	{
		if (this.NavMeshLink != null)
		{
			if (wantsOn)
			{
				this.NavMeshLink.gameObject.SetActive(true);
				this.NavMeshLink.enabled = true;
				return;
			}
			this.NavMeshLink.enabled = false;
			this.NavMeshLink.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00060B50 File Offset: 0x0005ED50
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_OpenDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.canHandOpen)
		{
			return;
		}
		if (base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (rpc.player.IsWounded())
		{
			if (!this.woundedOpens.ContainsKey(rpc.player) || this.woundedOpens[rpc.player] <= 2.5f)
			{
				return;
			}
			this.woundedOpens.Remove(rpc.player);
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null)
		{
			if (!baseLock.OnTryToOpen(rpc.player))
			{
				return;
			}
			if (baseLock.IsLocked() && UnityEngine.Time.realtimeSinceStartup - this.decayResetTimeLast > 60f)
			{
				BuildingBlock buildingBlock = base.FindLinkedEntity<BuildingBlock>();
				if (buildingBlock)
				{
					global::Decay.BuildingDecayTouch(buildingBlock);
				}
				else
				{
					global::Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
				}
				this.decayResetTimeLast = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(true);
		}
		if (this.checkPhysBoxesOnOpen)
		{
			this.StartCheckingForBlockages(true);
		}
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00060C9C File Offset: 0x0005EE9C
	private void StartCheckingForBlockages(bool isOpening)
	{
		if (this.HasVehiclePushBoxes)
		{
			base.Invoke(new Action(this.EnableVehiclePhysBoxes), 0.2f);
			float num = (isOpening ? this.openAnimLength : this.closeAnimLength);
			base.Invoke(new Action(this.DisableVehiclePhysBox), num);
		}
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00060CED File Offset: 0x0005EEED
	private void StopCheckingForBlockages()
	{
		if (this.HasVehiclePushBoxes)
		{
			this.ToggleVehiclePushBoxes(false);
			base.CancelInvoke(new Action(this.DisableVehiclePhysBox));
		}
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00060D10 File Offset: 0x0005EF10
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_CloseDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.canHandOpen)
		{
			return;
		}
		if (!base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (rpc.player.IsWounded())
		{
			if (!this.woundedCloses.ContainsKey(rpc.player) || this.woundedCloses[rpc.player] <= 2.5f)
			{
				return;
			}
			this.woundedCloses.Remove(rpc.player);
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null && !baseLock.OnTryToClose(rpc.player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(false);
		}
		this.StartCheckingForBlockages(false);
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00060DFC File Offset: 0x0005EFFC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_KnockDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.knockEffect.isValid)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextKnockTime)
		{
			return;
		}
		this.nextKnockTime = UnityEngine.Time.realtimeSinceStartup + 0.5f;
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.LowerCenterDecoration);
		if (slot != null)
		{
			DoorKnocker component = slot.GetComponent<DoorKnocker>();
			if (component)
			{
				component.Knock(rpc.player);
				return;
			}
		}
		Effect.server.Run(this.knockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00060E90 File Offset: 0x0005F090
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_ToggleHatch(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.hasHatch)
		{
			return;
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (!baseLock || baseLock.OnTryToOpen(rpc.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, !base.HasFlag(BaseEntity.Flags.Reserved3), false, true);
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00060EF3 File Offset: 0x0005F0F3
	private void EnableVehiclePhysBoxes()
	{
		this.ToggleVehiclePushBoxes(true);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00060EFC File Offset: 0x0005F0FC
	private void DisableVehiclePhysBox()
	{
		this.ToggleVehiclePushBoxes(false);
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000A91 RID: 2705 RVA: 0x00060F05 File Offset: 0x0005F105
	private bool HasVehiclePushBoxes
	{
		get
		{
			return this.vehiclePhysBoxes != null && this.vehiclePhysBoxes.Length != 0;
		}
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00060F1C File Offset: 0x0005F11C
	private void ToggleVehiclePushBoxes(bool state)
	{
		if (this.vehiclePhysBoxes == null)
		{
			return;
		}
		foreach (TriggerNotify triggerNotify in this.vehiclePhysBoxes)
		{
			if (triggerNotify != null)
			{
				triggerNotify.gameObject.SetActive(state);
			}
		}
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00060F60 File Offset: 0x0005F160
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_NotifyWoundedOpen(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player.IsWounded())
		{
			return;
		}
		if (!this.woundedOpens.ContainsKey(player))
		{
			this.woundedOpens.Add(player, default(TimeSince));
		}
		else
		{
			this.woundedOpens[player] = 0f;
		}
		base.Invoke(delegate
		{
			this.CheckTimedOutPlayers(this.woundedOpens);
		}, 5f);
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00060FD0 File Offset: 0x0005F1D0
	private void CheckTimedOutPlayers(Dictionary<BasePlayer, TimeSince> dictionary)
	{
		List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
		foreach (KeyValuePair<BasePlayer, TimeSince> keyValuePair in dictionary)
		{
			if (keyValuePair.Value > 5f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (BasePlayer basePlayer in list)
		{
			if (dictionary.ContainsKey(basePlayer))
			{
				dictionary.Remove(basePlayer);
			}
		}
		Facepunch.Pool.FreeList<BasePlayer>(ref list);
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x00061090 File Offset: 0x0005F290
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_NotifyWoundedClose(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player.IsWounded())
		{
			return;
		}
		if (!this.woundedCloses.ContainsKey(player))
		{
			this.woundedCloses.Add(player, default(TimeSince));
		}
		else
		{
			this.woundedCloses[player] = 0f;
		}
		base.Invoke(delegate
		{
			this.CheckTimedOutPlayers(this.woundedCloses);
		}, 5f);
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00061100 File Offset: 0x0005F300
	private void ReverseDoorAnimation(bool wasOpening)
	{
		if (this.model == null || this.model.animator == null)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.model.animator.GetCurrentAnimatorStateInfo(0);
		this.model.animator.Play(wasOpening ? Door.closeHash : Door.openHash, 0, 1f - currentAnimatorStateInfo.normalizedTime);
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00032D46 File Offset: 0x00030F46
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00061170 File Offset: 0x0005F370
	public void OnObjects(TriggerNotify trigger)
	{
		if (base.isServer)
		{
			bool flag = false;
			foreach (BaseEntity baseEntity in trigger.entityContents)
			{
				BaseMountable baseMountable;
				if ((baseMountable = baseEntity as BaseMountable) != null && baseMountable.BlocksDoors)
				{
					flag = true;
					break;
				}
				BaseVehicleModule baseVehicleModule;
				if ((baseVehicleModule = baseEntity as BaseVehicleModule) != null && baseVehicleModule.Vehicle != null && baseVehicleModule.Vehicle.BlocksDoors)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = base.HasFlag(BaseEntity.Flags.Open);
			this.SetOpen(!flag2, true);
			this.ReverseDoorAnimation(flag2);
			this.StopCheckingForBlockages();
			base.ClientRPC<int>(null, "OnDoorInterrupted", flag2 ? 1 : 0);
		}
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEmpty()
	{
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x00061244 File Offset: 0x0005F444
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			BaseEntity slot = base.GetSlot(BaseEntity.Slot.UpperModifier);
			if (slot)
			{
				slot.SendMessage("Think");
			}
		}
		if (this.ClosedColliderRoots != null)
		{
			bool flag = !base.HasFlag(BaseEntity.Flags.Open) || base.HasFlag(BaseEntity.Flags.Busy);
			foreach (GameObject gameObject in this.ClosedColliderRoots)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(flag);
				}
			}
		}
	}

	// Token: 0x040006D2 RID: 1746
	public GameObjectRef knockEffect;

	// Token: 0x040006D3 RID: 1747
	public bool canTakeLock = true;

	// Token: 0x040006D4 RID: 1748
	public bool hasHatch;

	// Token: 0x040006D5 RID: 1749
	public bool canTakeCloser;

	// Token: 0x040006D6 RID: 1750
	public bool canTakeKnocker;

	// Token: 0x040006D7 RID: 1751
	public bool canNpcOpen = true;

	// Token: 0x040006D8 RID: 1752
	public bool canHandOpen = true;

	// Token: 0x040006D9 RID: 1753
	public bool isSecurityDoor;

	// Token: 0x040006DA RID: 1754
	public TriggerNotify[] vehiclePhysBoxes;

	// Token: 0x040006DB RID: 1755
	public bool checkPhysBoxesOnOpen;

	// Token: 0x040006DC RID: 1756
	public SoundDefinition vehicleCollisionSfx;

	// Token: 0x040006DD RID: 1757
	public GameObject[] ClosedColliderRoots;

	// Token: 0x040006DE RID: 1758
	[SerializeField]
	[ReadOnly]
	private float openAnimLength = 4f;

	// Token: 0x040006DF RID: 1759
	[SerializeField]
	[ReadOnly]
	private float closeAnimLength = 4f;

	// Token: 0x040006E0 RID: 1760
	private float decayResetTimeLast = float.NegativeInfinity;

	// Token: 0x040006E1 RID: 1761
	public NavMeshModifierVolume NavMeshVolumeAnimals;

	// Token: 0x040006E2 RID: 1762
	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	// Token: 0x040006E3 RID: 1763
	public NavMeshLink NavMeshLink;

	// Token: 0x040006E4 RID: 1764
	public NPCDoorTriggerBox NpcTriggerBox;

	// Token: 0x040006E5 RID: 1765
	private static int nonWalkableArea = -1;

	// Token: 0x040006E6 RID: 1766
	private static int animalAgentTypeId = -1;

	// Token: 0x040006E7 RID: 1767
	private static int humanoidAgentTypeId = -1;

	// Token: 0x040006E8 RID: 1768
	private Dictionary<BasePlayer, TimeSince> woundedOpens = new Dictionary<BasePlayer, TimeSince>();

	// Token: 0x040006E9 RID: 1769
	private Dictionary<BasePlayer, TimeSince> woundedCloses = new Dictionary<BasePlayer, TimeSince>();

	// Token: 0x040006EA RID: 1770
	private float nextKnockTime = float.NegativeInfinity;

	// Token: 0x040006EB RID: 1771
	private static int openHash = Animator.StringToHash("open");

	// Token: 0x040006EC RID: 1772
	private static int closeHash = Animator.StringToHash("close");
}
