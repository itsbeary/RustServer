using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004C RID: 76
public class BaseVehicleModule : global::BaseVehicle, IPrefabPreProcess
{
	// Token: 0x06000837 RID: 2103 RVA: 0x00050E98 File Offset: 0x0004F098
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseVehicleModule.OnRpcMessage", 0))
		{
			if (rpc == 2683376664U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Use ");
				}
				using (TimeWarning.New("RPC_Use", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2683376664U, "RPC_Use", this, player, 3f))
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
							this.RPC_Use(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Use");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000838 RID: 2104 RVA: 0x00051000 File Offset: 0x0004F200
	// (set) Token: 0x06000839 RID: 2105 RVA: 0x00051008 File Offset: 0x0004F208
	public bool PropagateDamage { get; private set; } = true;

	// Token: 0x0600083A RID: 2106 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void NonUserSpawn()
	{
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00051014 File Offset: 0x0004F214
	public override void VehicleFixedUpdate()
	{
		if (!this.isSpawned || !this.IsOnAVehicle)
		{
			return;
		}
		base.VehicleFixedUpdate();
		if (this.Vehicle.IsEditableNow && this.AssociatedItemInstance != null && this.timeSinceItemLockRefresh > 1f)
		{
			this.AssociatedItemInstance.LockUnlock(!this.CanBeMovedNow());
			this.timeSinceItemLockRefresh = 0f;
		}
		for (int i = 0; i < this.slidingComponents.Length; i++)
		{
			this.slidingComponents[i].ServerUpdateTick(this);
		}
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x000510A4 File Offset: 0x0004F2A4
	public override void Hurt(HitInfo info)
	{
		if (this.IsOnAVehicle)
		{
			this.Vehicle.ModuleHurt(this, info);
		}
		base.Hurt(info);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x000510C4 File Offset: 0x0004F2C4
	public override void OnHealthChanged(float oldValue, float newValue)
	{
		base.OnHealthChanged(oldValue, newValue);
		if (!base.isServer)
		{
			return;
		}
		if (this.IsOnAVehicle)
		{
			if (this.Vehicle.IsDead())
			{
				return;
			}
			if (this.AssociatedItemInstance != null)
			{
				this.AssociatedItemInstance.condition = this.Health();
			}
			if (newValue <= 0f)
			{
				this.Vehicle.ModuleReachedZeroHealth();
			}
		}
		this.RefreshConditionals(true);
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0005112B File Offset: 0x0004F32B
	public bool CanBeMovedNow()
	{
		return !this.IsOnAVehicle || this.CanBeMovedNowOnVehicle();
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanBeMovedNowOnVehicle()
	{
		return true;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		return 0f;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0005113D File Offset: 0x0004F33D
	public void AcceptPropagatedDamage(float amount, DamageType type, global::BaseEntity attacker = null, bool useProtection = true)
	{
		this.PropagateDamage = false;
		base.Hurt(amount, type, attacker, useProtection);
		this.PropagateDamage = true;
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Die(HitInfo info = null)
	{
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00051158 File Offset: 0x0004F358
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Use(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeUsedNowBy(player))
		{
			return;
		}
		string text = msg.read.String(256);
		foreach (VehicleModuleSlidingComponent vehicleModuleSlidingComponent in this.slidingComponents)
		{
			if (this.PlayerIsLookingAtUsable(text, vehicleModuleSlidingComponent.interactionColliderName))
			{
				vehicleModuleSlidingComponent.Use(this);
				break;
			}
		}
		foreach (VehicleModuleButtonComponent vehicleModuleButtonComponent in this.buttonComponents)
		{
			if (vehicleModuleButtonComponent == null)
			{
				return;
			}
			if (this.PlayerIsLookingAtUsable(text, vehicleModuleButtonComponent.interactionColliderName))
			{
				vehicleModuleButtonComponent.ServerUse(player, this);
				return;
			}
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x000511FD File Offset: 0x0004F3FD
	public override void AdminKill()
	{
		if (this.IsOnAVehicle)
		{
			this.Vehicle.AdminKill();
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00051212 File Offset: 0x0004F412
	public override bool AdminFixUp(int tier)
	{
		return (!this.IsOnAVehicle || !this.Vehicle.IsDead()) && base.AdminFixUp(tier);
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPlayerDismountedVehicle(global::BasePlayer player)
	{
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x00051232 File Offset: 0x0004F432
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleModule = Facepunch.Pool.Get<VehicleModule>();
		info.msg.vehicleModule.socketIndex = this.FirstSocketIndex;
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000848 RID: 2120 RVA: 0x00051261 File Offset: 0x0004F461
	// (set) Token: 0x06000849 RID: 2121 RVA: 0x00051269 File Offset: 0x0004F469
	public BaseModularVehicle Vehicle { get; private set; }

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x0600084A RID: 2122 RVA: 0x00051272 File Offset: 0x0004F472
	// (set) Token: 0x0600084B RID: 2123 RVA: 0x0005127A File Offset: 0x0004F47A
	public int FirstSocketIndex { get; private set; } = -1;

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x0600084C RID: 2124 RVA: 0x00051283 File Offset: 0x0004F483
	public Vector3 CentreOfMass
	{
		get
		{
			return this.centreOfMassTransform.localPosition;
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x0600084D RID: 2125 RVA: 0x00051290 File Offset: 0x0004F490
	public float Mass
	{
		get
		{
			return this.mass;
		}
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x0600084E RID: 2126 RVA: 0x00051298 File Offset: 0x0004F498
	public NetworkableId ID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x0600084F RID: 2127 RVA: 0x000512A5 File Offset: 0x0004F4A5
	public bool IsOnAVehicle
	{
		get
		{
			return this.Vehicle != null;
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000850 RID: 2128 RVA: 0x000512B3 File Offset: 0x0004F4B3
	public ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000851 RID: 2129 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool HasSeating
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000852 RID: 2130 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool HasAnEngine
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x000512C0 File Offset: 0x0004F4C0
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		this.damageRenderer = base.GetComponent<DamageRenderer>();
		this.RefreshParameters();
		this.lights = base.GetComponentsInChildren<VehicleLight>();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x000512F0 File Offset: 0x0004F4F0
	public void RefreshParameters()
	{
		for (int i = this.conditionals.Count - 1; i >= 0; i--)
		{
			ConditionalObject conditionalObject = this.conditionals[i];
			if (conditionalObject.gameObject == null)
			{
				this.conditionals.RemoveAt(i);
			}
			else if (conditionalObject.restrictOnHealth)
			{
				conditionalObject.healthRestrictionMin = Mathf.Clamp01(conditionalObject.healthRestrictionMin);
				conditionalObject.healthRestrictionMax = Mathf.Clamp01(conditionalObject.healthRestrictionMax);
			}
			if (conditionalObject.gameObject != null)
			{
				Gibbable component = conditionalObject.gameObject.GetComponent<Gibbable>();
				if (component != null)
				{
					component.isConditional = true;
				}
			}
		}
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x00051398 File Offset: 0x0004F598
	public override global::BaseVehicle VehicleParent()
	{
		return base.GetParentEntity() as global::BaseVehicle;
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x000513A8 File Offset: 0x0004F5A8
	public virtual void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		this.Vehicle = vehicle;
		this.FirstSocketIndex = firstSocketIndex;
		this.TimeSinceAddedToVehicle = 0f;
		if (base.isServer)
		{
			TriggerParent[] array = this.triggerParents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].associatedMountable = vehicle;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		this.RefreshConditionals(false);
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00051408 File Offset: 0x0004F608
	public virtual void ModuleRemoved()
	{
		this.Vehicle = null;
		this.FirstSocketIndex = -1;
		if (base.isServer)
		{
			TriggerParent[] array = this.triggerParents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].associatedMountable = null;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00051450 File Offset: 0x0004F650
	public void OtherVehicleModulesChanged()
	{
		this.RefreshConditionals(false);
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x00051459 File Offset: 0x0004F659
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return this.IsOnAVehicle && this.Vehicle.CanBeLooted(player);
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x00051474 File Offset: 0x0004F674
	public bool KeycodeEntryBlocked(global::BasePlayer player)
	{
		global::ModularCar modularCar;
		return this.IsOnAVehicle && (modularCar = this.Vehicle as global::ModularCar) != null && modularCar.KeycodeEntryBlocked(player);
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x000514A1 File Offset: 0x0004F6A1
	public override float MaxHealth()
	{
		if (this.AssociatedItemDef != null)
		{
			return this.AssociatedItemDef.condition.max;
		}
		return base.MaxHealth();
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000514C8 File Offset: 0x0004F6C8
	public override float StartHealth()
	{
		return this.MaxHealth();
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x000514D0 File Offset: 0x0004F6D0
	public int GetNumSocketsTaken()
	{
		if (this.AssociatedItemDef == null)
		{
			return 1;
		}
		return this.AssociatedItemDef.GetComponent<ItemModVehicleModule>().socketsTaken;
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x000514F4 File Offset: 0x0004F6F4
	public List<ConditionalObject> GetConditionals()
	{
		List<ConditionalObject> list = new List<ConditionalObject>();
		foreach (ConditionalObject conditionalObject in this.conditionals)
		{
			if (conditionalObject.gameObject != null)
			{
				list.Add(conditionalObject);
			}
		}
		return list;
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x00029EBC File Offset: 0x000280BC
	public virtual float GetMaxDriveForce()
	{
		return 0f;
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0005155C File Offset: 0x0004F75C
	public void RefreshConditionals(bool canGib)
	{
		if (base.IsDestroyed || !this.IsOnAVehicle || !this.Vehicle.HasInited)
		{
			return;
		}
		foreach (ConditionalObject conditionalObject in this.conditionals)
		{
			this.RefreshConditional(conditionalObject, canGib);
		}
		this.prevRefreshHealth = this.Health();
		this.prevRefreshVehicleIsDead = this.Vehicle.IsDead();
		this.prevRefreshVehicleIsLockable = this.Vehicle.IsLockable;
		this.PostConditionalRefresh();
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void PostConditionalRefresh()
	{
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00051604 File Offset: 0x0004F804
	private void RefreshConditional(ConditionalObject conditional, bool canGib)
	{
		if (conditional == null || conditional.gameObject == null)
		{
			return;
		}
		bool flag = true;
		if (conditional.restrictOnHealth)
		{
			if (Mathf.Approximately(conditional.healthRestrictionMin, conditional.healthRestrictionMax))
			{
				flag = Mathf.Approximately(base.healthFraction, conditional.healthRestrictionMin);
			}
			else
			{
				flag = base.healthFraction > conditional.healthRestrictionMin && base.healthFraction <= conditional.healthRestrictionMax;
			}
			if (canGib)
			{
			}
		}
		if (flag && this.IsOnAVehicle && conditional.restrictOnLockable)
		{
			flag = this.Vehicle.IsLockable == conditional.lockableRestriction;
		}
		if (flag && conditional.restrictOnAdjacent)
		{
			bool flag2 = false;
			bool flag3 = false;
			BaseVehicleModule baseVehicleModule;
			if (this.TryGetAdjacentModuleInFront(out baseVehicleModule))
			{
				flag2 = this.InSameVisualGroupAs(baseVehicleModule, conditional.adjacentMatch);
			}
			if (this.TryGetAdjacentModuleBehind(out baseVehicleModule))
			{
				flag3 = this.InSameVisualGroupAs(baseVehicleModule, conditional.adjacentMatch);
			}
			switch (conditional.adjacentRestriction)
			{
			case ConditionalObject.AdjacentCondition.SameInFront:
				flag = flag2;
				break;
			case ConditionalObject.AdjacentCondition.SameBehind:
				flag = flag3;
				break;
			case ConditionalObject.AdjacentCondition.DifferentInFront:
				flag = !flag2;
				break;
			case ConditionalObject.AdjacentCondition.DifferentBehind:
				flag = !flag3;
				break;
			case ConditionalObject.AdjacentCondition.BothDifferent:
				flag = !flag2 && !flag3;
				break;
			case ConditionalObject.AdjacentCondition.BothSame:
				flag = flag2 && flag3;
				break;
			}
		}
		if (flag)
		{
			if (!this.IsOnAVehicle)
			{
				for (int i = 0; i < conditional.socketSettings.Length; i++)
				{
					flag = !conditional.socketSettings[i].HasSocketRestrictions;
					if (!flag)
					{
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < conditional.socketSettings.Length; j++)
				{
					ModularVehicleSocket socket = this.Vehicle.GetSocket(this.FirstSocketIndex + j);
					if (socket == null)
					{
						Debug.LogWarning(string.Format("{0} module got NULL socket at index {1}. Total vehicle sockets: {2} FirstSocketIndex: {3} Sockets taken: {4}", new object[]
						{
							this.AssociatedItemDef.displayName.translated,
							this.FirstSocketIndex + j,
							this.Vehicle.TotalSockets,
							this.FirstSocketIndex,
							conditional.socketSettings.Length
						}));
					}
					flag = socket != null && socket.ShouldBeActive(conditional.socketSettings[j]);
					if (!flag)
					{
						break;
					}
				}
			}
		}
		bool activeInHierarchy = conditional.gameObject.activeInHierarchy;
		conditional.SetActive(flag);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0005184C File Offset: 0x0004FA4C
	private bool TryGetAdjacentModuleInFront(out BaseVehicleModule result)
	{
		if (!this.IsOnAVehicle)
		{
			result = null;
			return false;
		}
		int num = this.FirstSocketIndex - 1;
		return this.Vehicle.TryGetModuleAt(num, out result);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0005187C File Offset: 0x0004FA7C
	private bool TryGetAdjacentModuleBehind(out BaseVehicleModule result)
	{
		if (!this.IsOnAVehicle)
		{
			result = null;
			return false;
		}
		int num = this.FirstSocketIndex + this.GetNumSocketsTaken() - 1;
		return this.Vehicle.TryGetModuleAt(num + 1, out result);
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x000518B8 File Offset: 0x0004FAB8
	private bool InSameVisualGroupAs(BaseVehicleModule moduleEntity, ConditionalObject.AdjacentMatchType matchType)
	{
		if (moduleEntity == null)
		{
			return false;
		}
		if (this.visualGroup == BaseVehicleModule.VisualGroup.None)
		{
			return matchType != ConditionalObject.AdjacentMatchType.GroupNotExact && moduleEntity.prefabID == this.prefabID;
		}
		switch (matchType)
		{
		case ConditionalObject.AdjacentMatchType.GroupOrExact:
			return moduleEntity.prefabID == this.prefabID || moduleEntity.visualGroup == this.visualGroup;
		case ConditionalObject.AdjacentMatchType.ExactOnly:
			return moduleEntity.prefabID == this.prefabID;
		case ConditionalObject.AdjacentMatchType.GroupNotExact:
			return moduleEntity.prefabID != this.prefabID && moduleEntity.visualGroup == this.visualGroup;
		default:
			return false;
		}
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x00051950 File Offset: 0x0004FB50
	private bool CanBeUsedNowBy(global::BasePlayer player)
	{
		return this.IsOnAVehicle && !(player == null) && !this.Vehicle.IsEditableNow && !this.Vehicle.IsDead() && this.Vehicle.PlayerIsMounted(player) && this.Vehicle.PlayerCanUseThis(player, ModularCarCodeLock.LockType.General);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x000519A7 File Offset: 0x0004FBA7
	public bool PlayerIsLookingAtUsable(string lookingAtColldierName, string usableColliderName)
	{
		return string.Compare(lookingAtColldierName, usableColliderName, true) == 0;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x000519B4 File Offset: 0x0004FBB4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x00007A44 File Offset: 0x00005C44
	public override bool IsVehicleRoot()
	{
		return false;
	}

	// Token: 0x0400057E RID: 1406
	public global::Item AssociatedItemInstance;

	// Token: 0x04000580 RID: 1408
	private TimeSince timeSinceItemLockRefresh;

	// Token: 0x04000581 RID: 1409
	private const float TIME_BETWEEN_LOCK_REFRESH = 1f;

	// Token: 0x04000582 RID: 1410
	[Header("Vehicle Module")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04000583 RID: 1411
	[SerializeField]
	private float mass = 100f;

	// Token: 0x04000584 RID: 1412
	public BaseVehicleModule.VisualGroup visualGroup;

	// Token: 0x04000585 RID: 1413
	[SerializeField]
	[HideInInspector]
	private VehicleLight[] lights;

	// Token: 0x04000588 RID: 1416
	public BaseVehicleModule.LODLevel[] lodRenderers;

	// Token: 0x04000589 RID: 1417
	[SerializeField]
	[HideInInspector]
	private List<ConditionalObject> conditionals;

	// Token: 0x0400058A RID: 1418
	[Header("Trigger Parent")]
	[SerializeField]
	private TriggerParent[] triggerParents;

	// Token: 0x0400058B RID: 1419
	[Header("Sliding Components")]
	[SerializeField]
	private VehicleModuleSlidingComponent[] slidingComponents;

	// Token: 0x0400058C RID: 1420
	[SerializeField]
	private VehicleModuleButtonComponent[] buttonComponents;

	// Token: 0x0400058D RID: 1421
	private TimeSince TimeSinceAddedToVehicle;

	// Token: 0x0400058E RID: 1422
	private float prevRefreshHealth = -1f;

	// Token: 0x0400058F RID: 1423
	private bool prevRefreshVehicleIsDead;

	// Token: 0x04000590 RID: 1424
	private bool prevRefreshVehicleIsLockable;

	// Token: 0x02000BCC RID: 3020
	public enum VisualGroup
	{
		// Token: 0x04004174 RID: 16756
		None,
		// Token: 0x04004175 RID: 16757
		Engine,
		// Token: 0x04004176 RID: 16758
		Cabin,
		// Token: 0x04004177 RID: 16759
		Flatbed
	}

	// Token: 0x02000BCD RID: 3021
	[Serializable]
	public class LODLevel
	{
		// Token: 0x04004178 RID: 16760
		public Renderer[] renderers;
	}
}
