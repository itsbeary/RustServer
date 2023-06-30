using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A2 RID: 162
public class ModularCarGarage : ContainerIOEntity
{
	// Token: 0x06000EDC RID: 3804 RVA: 0x0007D220 File Offset: 0x0007B420
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCarGarage.OnRpcMessage", 0))
		{
			if (rpc == 554177909U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DeselectedLootItem ");
				}
				using (TimeWarning.New("RPC_DeselectedLootItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(554177909U, "RPC_DeselectedLootItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DeselectedLootItem(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_DeselectedLootItem");
					}
				}
				return true;
			}
			if (rpc == 3683966290U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DiedWithKeypadOpen ");
				}
				using (TimeWarning.New("RPC_DiedWithKeypadOpen", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3683966290U, "RPC_DiedWithKeypadOpen", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3683966290U, "RPC_DiedWithKeypadOpen", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DiedWithKeypadOpen(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_DiedWithKeypadOpen");
					}
				}
				return true;
			}
			if (rpc == 3659332720U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenEditing ");
				}
				using (TimeWarning.New("RPC_OpenEditing", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3659332720U, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3659332720U, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenEditing(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenEditing");
					}
				}
				return true;
			}
			if (rpc == 1582295101U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RepairItem ");
				}
				using (TimeWarning.New("RPC_RepairItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1582295101U, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1582295101U, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RepairItem(rpcmessage4);
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_RepairItem");
					}
				}
				return true;
			}
			if (rpc == 3710764312U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestAddLock ");
				}
				using (TimeWarning.New("RPC_RequestAddLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3710764312U, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3710764312U, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestAddLock(rpcmessage5);
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_RequestAddLock");
					}
				}
				return true;
			}
			if (rpc == 3305106830U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestNewCode ");
				}
				using (TimeWarning.New("RPC_RequestNewCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3305106830U, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3305106830U, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestNewCode(rpcmessage6);
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_RequestNewCode");
					}
				}
				return true;
			}
			if (rpc == 1046853419U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestRemoveLock ");
				}
				using (TimeWarning.New("RPC_RequestRemoveLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1046853419U, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1046853419U, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestRemoveLock(rpcmessage7);
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in RPC_RequestRemoveLock");
					}
				}
				return true;
			}
			if (rpc == 4033916654U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SelectedLootItem ");
				}
				using (TimeWarning.New("RPC_SelectedLootItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4033916654U, "RPC_SelectedLootItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_SelectedLootItem(rpcmessage8);
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in RPC_SelectedLootItem");
					}
				}
				return true;
			}
			if (rpc == 2974124904U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartDestroyingChassis ");
				}
				using (TimeWarning.New("RPC_StartDestroyingChassis", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartDestroyingChassis(rpcmessage9);
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in RPC_StartDestroyingChassis");
					}
				}
				return true;
			}
			if (rpc == 3872977075U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartKeycodeEntry ");
				}
				using (TimeWarning.New("RPC_StartKeycodeEntry", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3872977075U, "RPC_StartKeycodeEntry", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage10 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartKeycodeEntry(rpcmessage10);
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in RPC_StartKeycodeEntry");
					}
				}
				return true;
			}
			if (rpc == 3830531963U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StopDestroyingChassis ");
				}
				using (TimeWarning.New("RPC_StopDestroyingChassis", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StopDestroyingChassis(rpcmessage11);
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in RPC_StopDestroyingChassis");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0007E284 File Offset: 0x0007C484
	private global::ModularCar carOccupant
	{
		get
		{
			if (!(this.lockedOccupant != null))
			{
				return this.occupantTrigger.carOccupant;
			}
			return this.lockedOccupant;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0007E2A6 File Offset: 0x0007C4A6
	private bool HasOccupant
	{
		get
		{
			return this.carOccupant != null && this.carOccupant.IsFullySpawned();
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0007E2C4 File Offset: 0x0007C4C4
	protected void FixedUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.magnetSnap == null)
		{
			return;
		}
		if (this.playerTrigger != null)
		{
			bool hasAnyContents = this.playerTrigger.HasAnyContents;
			if (this.PlayerObstructingLift != hasAnyContents)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved8, hasAnyContents, false, true);
			}
		}
		this.UpdateCarOccupant();
		if (this.HasOccupant && this.carOccupant.CouldBeEdited() && this.carOccupant.GetSpeed() <= 1f)
		{
			if (base.IsOn() || !this.carOccupant.IsComplete())
			{
				if (this.lockedOccupant == null && !this.carOccupant.rigidBody.isKinematic)
				{
					this.GrabOccupant(this.occupantTrigger.carOccupant);
				}
				this.magnetSnap.FixedUpdate(this.carOccupant.transform);
			}
			if (this.carOccupant.CarLock.HasALock && !this.carOccupant.CarLock.CanHaveALock())
			{
				this.carOccupant.CarLock.RemoveLock();
			}
		}
		else if (this.HasOccupant && this.carOccupant.rigidBody.isKinematic)
		{
			this.ReleaseOccupant();
		}
		if (this.HasOccupant && this.IsDestroyingChassis && this.carOccupant.HasAnyModules)
		{
			this.StopChassisDestroy();
		}
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0007E41F File Offset: 0x0007C61F
	internal override void DoServerDestroy()
	{
		if (this.HasOccupant)
		{
			this.ReleaseOccupant();
			if (!this.HasDriveableOccupant)
			{
				this.carOccupant.Kill(global::BaseNetworkable.DestroyMode.Gib);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0007E449 File Offset: 0x0007C649
	public override void ServerInit()
	{
		base.ServerInit();
		this.magnetSnap = new MagnetSnap(this.vehicleLiftPos);
		this.RefreshOnOffState();
		this.SetOccupantState(false, false, false, ModularCarGarage.OccupantLock.CannotHaveLock, true);
		this.RefreshLiftState(true);
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0007E47C File Offset: 0x0007C67C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleLift = Facepunch.Pool.Get<VehicleLift>();
		info.msg.vehicleLift.platformIsOccupied = this.PlatformIsOccupied;
		info.msg.vehicleLift.editableOccupant = this.HasEditableOccupant;
		info.msg.vehicleLift.driveableOccupant = this.HasDriveableOccupant;
		info.msg.vehicleLift.occupantLockState = (int)this.OccupantLockState;
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0007E4F8 File Offset: 0x0007C6F8
	public override ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x0007E510 File Offset: 0x0007C710
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		if (player == null)
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (!flag)
		{
			return false;
		}
		if (this.HasEditableOccupant)
		{
			player.inventory.loot.AddContainer(this.carOccupant.Inventory.ModuleContainer);
			player.inventory.loot.AddContainer(this.carOccupant.Inventory.ChassisContainer);
			player.inventory.loot.SendImmediate();
		}
		this.lootingPlayers.Add(player);
		this.RefreshLiftState(false);
		return flag;
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0007E5B2 File Offset: 0x0007C7B2
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (!this.IsEnteringKeycode)
		{
			this.lootingPlayers.Remove(player);
			this.RefreshLiftState(false);
		}
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0007E5D7 File Offset: 0x0007C7D7
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.RefreshOnOffState();
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x0007E5E7 File Offset: 0x0007C7E7
	public bool TryGetModuleForItem(global::Item item, out BaseVehicleModule result)
	{
		if (!this.HasOccupant)
		{
			result = null;
			return false;
		}
		result = this.carOccupant.GetModuleForItem(item);
		return result != null;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0007E60C File Offset: 0x0007C80C
	private void RefreshOnOffState()
	{
		bool flag = !this.needsElectricity || this.currentEnergy >= this.ConsumptionAmount();
		if (flag != base.IsOn())
		{
			base.SetFlag(global::BaseEntity.Flags.On, flag, false, true);
		}
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0007E64C File Offset: 0x0007C84C
	private void UpdateCarOccupant()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.HasOccupant)
		{
			bool flag = Vector3.SqrMagnitude(this.carOccupant.transform.position - this.vehicleLiftPos.position) < 1f && this.carOccupant.CouldBeEdited() && !this.PlayerObstructingLift;
			bool flag2 = this.carOccupant.IsComplete();
			ModularCarGarage.OccupantLock occupantLock;
			if (this.carOccupant.CarLock.CanHaveALock())
			{
				if (this.carOccupant.CarLock.HasALock)
				{
					occupantLock = ModularCarGarage.OccupantLock.HasLock;
				}
				else
				{
					occupantLock = ModularCarGarage.OccupantLock.NoLock;
				}
			}
			else
			{
				occupantLock = ModularCarGarage.OccupantLock.CannotHaveLock;
			}
			this.SetOccupantState(this.HasOccupant, flag, flag2, occupantLock, false);
			return;
		}
		this.SetOccupantState(false, false, false, ModularCarGarage.OccupantLock.CannotHaveLock, false);
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0007E70B File Offset: 0x0007C90B
	private void UpdateOccupantMode()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		this.carOccupant.inEditableLocation = this.HasEditableOccupant && this.LiftIsUp;
		this.carOccupant.immuneToDecay = base.IsOn();
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0007E744 File Offset: 0x0007C944
	private void WakeNearbyRigidbodies()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(base.transform.position, 7f, list, 34816, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null && attachedRigidbody.IsSleeping())
			{
				attachedRigidbody.WakeUp();
			}
			global::BaseEntity baseEntity = collider.ToBaseEntity();
			BaseRidableAnimal baseRidableAnimal;
			if (baseEntity != null && (baseRidableAnimal = baseEntity as BaseRidableAnimal) != null && baseRidableAnimal.isServer)
			{
				baseRidableAnimal.UpdateDropToGroundForDuration(2f);
			}
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0007E800 File Offset: 0x0007CA00
	private void EditableOccupantEntered()
	{
		this.RefreshLoot();
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0007E800 File Offset: 0x0007CA00
	private void EditableOccupantLeft()
	{
		this.RefreshLoot();
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0007E808 File Offset: 0x0007CA08
	private void RefreshLoot()
	{
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		list.AddRange(this.lootingPlayers);
		foreach (global::BasePlayer basePlayer in list)
		{
			basePlayer.inventory.loot.Clear();
			this.PlayerOpenLoot(basePlayer, "", true);
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x0007E888 File Offset: 0x0007CA88
	private void GrabOccupant(global::ModularCar occupant)
	{
		if (occupant == null)
		{
			return;
		}
		this.lockedOccupant = occupant;
		this.lockedOccupant.DisablePhysics();
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x0007E8A8 File Offset: 0x0007CAA8
	private void ReleaseOccupant()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		this.carOccupant.inEditableLocation = false;
		this.carOccupant.immuneToDecay = false;
		if (this.lockedOccupant != null)
		{
			this.lockedOccupant.EnablePhysics();
			this.lockedOccupant = null;
		}
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0007E8F6 File Offset: 0x0007CAF6
	private void StopChassisDestroy()
	{
		if (base.IsInvoking(new Action(this.FinishDestroyingChassis)))
		{
			base.CancelInvoke(new Action(this.FinishDestroyingChassis));
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0007E92C File Offset: 0x0007CB2C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RepairItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		ItemId itemId = msg.read.ItemID();
		if (player == null || !this.HasOccupant)
		{
			return;
		}
		global::Item vehicleItem = this.carOccupant.GetVehicleItem(itemId);
		if (vehicleItem != null)
		{
			RepairBench.RepairAnItem(vehicleItem, player, this, 0f, false);
			Effect.server.Run(this.repairEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			return;
		}
		Debug.LogError(base.GetType().Name + ": Couldn't get item to repair, with ID: " + itemId);
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0007E9BC File Offset: 0x0007CBBC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenEditing(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || this.LiftIsMoving)
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0007E9F0 File Offset: 0x0007CBF0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_DiedWithKeypadOpen(global::BaseEntity.RPCMessage msg)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		this.lootingPlayers.Clear();
		this.RefreshLiftState(false);
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0007EA14 File Offset: 0x0007CC14
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SelectedLootItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		ItemId itemId = msg.read.ItemID();
		if (player == null || !player.inventory.loot.IsLooting() || player.inventory.loot.entitySource != this)
		{
			return;
		}
		if (!this.HasOccupant)
		{
			return;
		}
		global::Item vehicleItem = this.carOccupant.GetVehicleItem(itemId);
		if (vehicleItem != null)
		{
			bool flag = player.inventory.loot.RemoveContainerAt(3);
			BaseVehicleModule baseVehicleModule;
			if (this.TryGetModuleForItem(vehicleItem, out baseVehicleModule))
			{
				VehicleModuleStorage vehicleModuleStorage;
				VehicleModuleCamper vehicleModuleCamper;
				if ((vehicleModuleStorage = baseVehicleModule as VehicleModuleStorage) != null)
				{
					IItemContainerEntity container = vehicleModuleStorage.GetContainer();
					if (!container.IsUnityNull<IItemContainerEntity>())
					{
						player.inventory.loot.AddContainer(container.inventory);
						flag = true;
					}
				}
				else if ((vehicleModuleCamper = baseVehicleModule as VehicleModuleCamper) != null)
				{
					IItemContainerEntity container2 = vehicleModuleCamper.GetContainer();
					if (!container2.IsUnityNull<IItemContainerEntity>())
					{
						player.inventory.loot.AddContainer(container2.inventory);
						flag = true;
					}
				}
			}
			if (flag)
			{
				player.inventory.loot.SendImmediate();
			}
		}
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0007EB24 File Offset: 0x0007CD24
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_DeselectedLootItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!player.inventory.loot.IsLooting() || player.inventory.loot.entitySource != this)
		{
			return;
		}
		if (player.inventory.loot.RemoveContainerAt(3))
		{
			player.inventory.loot.SendImmediate();
		}
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0007EB86 File Offset: 0x0007CD86
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_StartKeycodeEntry(global::BaseEntity.RPCMessage msg)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, true, false, true);
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0007EB98 File Offset: 0x0007CD98
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestAddLock(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string text = msg.read.String(256);
		ItemAmount itemAmount = this.lockResourceCost;
		if ((float)player.inventory.GetAmount(itemAmount.itemDef.itemid) >= itemAmount.amount && this.carOccupant.CarLock.TryAddALock(text, player.userID))
		{
			player.inventory.Take(null, itemAmount.itemDef.itemid, Mathf.CeilToInt(itemAmount.amount));
			Effect.server.Run(this.addRemoveLockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0007EC64 File Offset: 0x0007CE64
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestRemoveLock(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (!this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		this.carOccupant.CarLock.RemoveLock();
		Effect.server.Run(this.addRemoveLockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0007ECBC File Offset: 0x0007CEBC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestNewCode(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (!this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string text = msg.read.String(256);
		if (this.carOccupant.CarLock.TrySetNewCode(text, player.userID))
		{
			Effect.server.Run(this.changeLockCodeEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x0007ED3E File Offset: 0x0007CF3E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_StartDestroyingChassis(global::BaseEntity.RPCMessage msg)
	{
		if (this.carOccupant.HasAnyModules)
		{
			return;
		}
		base.Invoke(new Action(this.FinishDestroyingChassis), 10f);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0007ED73 File Offset: 0x0007CF73
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_StopDestroyingChassis(global::BaseEntity.RPCMessage msg)
	{
		this.StopChassisDestroy();
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0007ED7B File Offset: 0x0007CF7B
	private void FinishDestroyingChassis()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (this.carOccupant.HasAnyModules)
		{
			return;
		}
		this.carOccupant.Kill(global::BaseNetworkable.DestroyMode.Gib);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000EFE RID: 3838 RVA: 0x0007EDAE File Offset: 0x0007CFAE
	// (set) Token: 0x06000EFF RID: 3839 RVA: 0x0007EDB6 File Offset: 0x0007CFB6
	public bool PlatformIsOccupied { get; private set; }

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000F00 RID: 3840 RVA: 0x0007EDBF File Offset: 0x0007CFBF
	// (set) Token: 0x06000F01 RID: 3841 RVA: 0x0007EDC7 File Offset: 0x0007CFC7
	public bool HasEditableOccupant { get; private set; }

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000F02 RID: 3842 RVA: 0x0007EDD0 File Offset: 0x0007CFD0
	// (set) Token: 0x06000F03 RID: 3843 RVA: 0x0007EDD8 File Offset: 0x0007CFD8
	public bool HasDriveableOccupant { get; private set; }

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000F04 RID: 3844 RVA: 0x0007EDE1 File Offset: 0x0007CFE1
	// (set) Token: 0x06000F05 RID: 3845 RVA: 0x0007EDE9 File Offset: 0x0007CFE9
	public ModularCarGarage.OccupantLock OccupantLockState { get; private set; }

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000F06 RID: 3846 RVA: 0x0007EDF2 File Offset: 0x0007CFF2
	private bool LiftIsUp
	{
		get
		{
			return this.vehicleLiftState == ModularCarGarage.VehicleLiftState.Up;
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000F07 RID: 3847 RVA: 0x0007EDFD File Offset: 0x0007CFFD
	private bool LiftIsMoving
	{
		get
		{
			return this.vehicleLiftAnim.isPlaying;
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000F08 RID: 3848 RVA: 0x0007EE0A File Offset: 0x0007D00A
	private bool LiftIsDown
	{
		get
		{
			return this.vehicleLiftState == ModularCarGarage.VehicleLiftState.Down;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000F09 RID: 3849 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsDestroyingChassis
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000F0A RID: 3850 RVA: 0x0004B603 File Offset: 0x00049803
	private bool IsEnteringKeycode
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved7);
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000F0B RID: 3851 RVA: 0x00003278 File Offset: 0x00001478
	public bool PlayerObstructingLift
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0007EE15 File Offset: 0x0007D015
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.downPos = this.vehicleLift.transform.position;
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x0007EE2D File Offset: 0x0007D02D
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			this.UpdateOccupantMode();
		}
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x00007649 File Offset: 0x00005849
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.IsOn();
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x000219AE File Offset: 0x0001FBAE
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x0007EE48 File Offset: 0x0007D048
	private void SetOccupantState(bool hasOccupant, bool editableOccupant, bool driveableOccupant, ModularCarGarage.OccupantLock occupantLockState, bool forced = false)
	{
		if (this.PlatformIsOccupied == hasOccupant && this.HasEditableOccupant == editableOccupant && this.HasDriveableOccupant == driveableOccupant && this.OccupantLockState == occupantLockState && !forced)
		{
			return;
		}
		bool hasEditableOccupant = this.HasEditableOccupant;
		this.PlatformIsOccupied = hasOccupant;
		this.HasEditableOccupant = editableOccupant;
		this.HasDriveableOccupant = driveableOccupant;
		this.OccupantLockState = occupantLockState;
		if (base.isServer)
		{
			this.UpdateOccupantMode();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			if (hasEditableOccupant && !editableOccupant)
			{
				this.EditableOccupantLeft();
			}
			else if (editableOccupant && !hasEditableOccupant)
			{
				this.EditableOccupantEntered();
			}
		}
		this.RefreshLiftState(false);
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0007EEDC File Offset: 0x0007D0DC
	private void RefreshLiftState(bool forced = false)
	{
		ModularCarGarage.VehicleLiftState vehicleLiftState = ((base.IsOpen() || this.IsEnteringKeycode || (this.HasEditableOccupant && !this.HasDriveableOccupant)) ? ModularCarGarage.VehicleLiftState.Up : ModularCarGarage.VehicleLiftState.Down);
		this.MoveLift(vehicleLiftState, 0f, forced);
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0007EF24 File Offset: 0x0007D124
	private void MoveLift(ModularCarGarage.VehicleLiftState desiredLiftState, float startDelay = 0f, bool forced = false)
	{
		if (this.vehicleLiftState == desiredLiftState && !forced)
		{
			return;
		}
		ModularCarGarage.VehicleLiftState vehicleLiftState = this.vehicleLiftState;
		this.vehicleLiftState = desiredLiftState;
		if (base.isServer)
		{
			this.UpdateOccupantMode();
			this.WakeNearbyRigidbodies();
		}
		if (!base.gameObject.activeSelf)
		{
			this.vehicleLiftAnim[this.animName].time = ((desiredLiftState == ModularCarGarage.VehicleLiftState.Up) ? 1f : 0f);
			this.vehicleLiftAnim.Play();
			return;
		}
		if (desiredLiftState == ModularCarGarage.VehicleLiftState.Up)
		{
			base.Invoke(new Action(this.MoveLiftUp), startDelay);
			return;
		}
		base.Invoke(new Action(this.MoveLiftDown), startDelay);
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x0007EFCB File Offset: 0x0007D1CB
	private void MoveLiftUp()
	{
		this.vehicleLiftAnim[this.animName].length /= this.liftMoveTime;
		this.vehicleLiftAnim.Play();
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0007EFFC File Offset: 0x0007D1FC
	private void MoveLiftDown()
	{
		AnimationState animationState = this.vehicleLiftAnim[this.animName];
		animationState.speed = animationState.length / this.liftMoveTime;
		if (!this.vehicleLiftAnim.isPlaying && Vector3.Distance(this.vehicleLift.transform.position, this.downPos) > 0.01f)
		{
			animationState.time = 1f;
		}
		animationState.speed *= -1f;
		this.vehicleLiftAnim.Play();
	}

	// Token: 0x040009A6 RID: 2470
	private global::ModularCar lockedOccupant;

	// Token: 0x040009A7 RID: 2471
	private readonly HashSet<global::BasePlayer> lootingPlayers = new HashSet<global::BasePlayer>();

	// Token: 0x040009A8 RID: 2472
	private MagnetSnap magnetSnap;

	// Token: 0x040009A9 RID: 2473
	[SerializeField]
	private Transform vehicleLift;

	// Token: 0x040009AA RID: 2474
	[SerializeField]
	private Animation vehicleLiftAnim;

	// Token: 0x040009AB RID: 2475
	[SerializeField]
	private string animName = "LiftUp";

	// Token: 0x040009AC RID: 2476
	[SerializeField]
	private VehicleLiftOccupantTrigger occupantTrigger;

	// Token: 0x040009AD RID: 2477
	[SerializeField]
	private float liftMoveTime = 1f;

	// Token: 0x040009AE RID: 2478
	[SerializeField]
	private EmissionToggle poweredLight;

	// Token: 0x040009AF RID: 2479
	[SerializeField]
	private EmissionToggle inUseLight;

	// Token: 0x040009B0 RID: 2480
	[SerializeField]
	private Transform vehicleLiftPos;

	// Token: 0x040009B1 RID: 2481
	[SerializeField]
	[Range(0f, 1f)]
	private float recycleEfficiency = 0.5f;

	// Token: 0x040009B2 RID: 2482
	[SerializeField]
	private Transform recycleDropPos;

	// Token: 0x040009B3 RID: 2483
	[SerializeField]
	private bool needsElectricity;

	// Token: 0x040009B4 RID: 2484
	[SerializeField]
	private SoundDefinition liftStartSoundDef;

	// Token: 0x040009B5 RID: 2485
	[SerializeField]
	private SoundDefinition liftStopSoundDef;

	// Token: 0x040009B6 RID: 2486
	[SerializeField]
	private SoundDefinition liftStopDownSoundDef;

	// Token: 0x040009B7 RID: 2487
	[SerializeField]
	private SoundDefinition liftLoopSoundDef;

	// Token: 0x040009B8 RID: 2488
	[SerializeField]
	private GameObjectRef addRemoveLockEffect;

	// Token: 0x040009B9 RID: 2489
	[SerializeField]
	private GameObjectRef changeLockCodeEffect;

	// Token: 0x040009BA RID: 2490
	[SerializeField]
	private GameObjectRef repairEffect;

	// Token: 0x040009BB RID: 2491
	[SerializeField]
	private TriggerBase playerTrigger;

	// Token: 0x040009BC RID: 2492
	public ModularCarGarage.ChassisBuildOption[] chassisBuildOptions;

	// Token: 0x040009BD RID: 2493
	public ItemAmount lockResourceCost;

	// Token: 0x040009C2 RID: 2498
	private ModularCarGarage.VehicleLiftState vehicleLiftState;

	// Token: 0x040009C3 RID: 2499
	private Sound liftLoopSound;

	// Token: 0x040009C4 RID: 2500
	private Vector3 downPos;

	// Token: 0x040009C5 RID: 2501
	public const global::BaseEntity.Flags Flag_DestroyingChassis = global::BaseEntity.Flags.Reserved6;

	// Token: 0x040009C6 RID: 2502
	public const float TimeToDestroyChassis = 10f;

	// Token: 0x040009C7 RID: 2503
	public const global::BaseEntity.Flags Flag_EnteringKeycode = global::BaseEntity.Flags.Reserved7;

	// Token: 0x040009C8 RID: 2504
	public const global::BaseEntity.Flags Flag_PlayerObstructing = global::BaseEntity.Flags.Reserved8;

	// Token: 0x02000BF5 RID: 3061
	[Serializable]
	public class ChassisBuildOption
	{
		// Token: 0x040041F5 RID: 16885
		public GameObjectRef prefab;

		// Token: 0x040041F6 RID: 16886
		public ItemDefinition itemDef;
	}

	// Token: 0x02000BF6 RID: 3062
	public enum OccupantLock
	{
		// Token: 0x040041F8 RID: 16888
		CannotHaveLock,
		// Token: 0x040041F9 RID: 16889
		NoLock,
		// Token: 0x040041FA RID: 16890
		HasLock
	}

	// Token: 0x02000BF7 RID: 3063
	private enum VehicleLiftState
	{
		// Token: 0x040041FC RID: 16892
		Down,
		// Token: 0x040041FD RID: 16893
		Up
	}
}
