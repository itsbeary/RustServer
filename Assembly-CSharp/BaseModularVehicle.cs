using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust.Modular;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public abstract class BaseModularVehicle : GroundVehicle, global::PlayerInventory.ICanMoveFrom, IPrefabPreProcess
{
	// Token: 0x17000325 RID: 805
	// (get) Token: 0x060026A8 RID: 9896 RVA: 0x000F346C File Offset: 0x000F166C
	// (set) Token: 0x060026A9 RID: 9897 RVA: 0x000F3474 File Offset: 0x000F1674
	public ModularVehicleInventory Inventory { get; private set; }

	// Token: 0x060026AA RID: 9898 RVA: 0x000F3480 File Offset: 0x000F1680
	public override void ServerInit()
	{
		base.ServerInit();
		if (!this.disablePhysics)
		{
			this.rigidBody.isKinematic = false;
		}
		this.prevEditable = this.IsEditableNow;
		if (this.Inventory == null)
		{
			this.Inventory = new ModularVehicleInventory(this, this.AssociatedItemDef, true);
		}
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000F34CE File Offset: 0x000F16CE
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		if (this.Inventory == null)
		{
			this.Inventory = new ModularVehicleInventory(this, this.AssociatedItemDef, false);
		}
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000F34F4 File Offset: 0x000F16F4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.Inventory != null && !this.Inventory.UID.IsValid)
		{
			this.Inventory.GiveUIDs();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000F3539 File Offset: 0x000F1739
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.Inventory != null)
		{
			this.Inventory.Dispose();
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x00074E15 File Offset: 0x00073015
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x060026AF RID: 9903
	public abstract bool IsComplete();

	// Token: 0x060026B0 RID: 9904 RVA: 0x000F3554 File Offset: 0x000F1754
	public bool CouldBeEdited()
	{
		return !this.AnyMounted() && !this.IsDead();
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000F3569 File Offset: 0x000F1769
	public void DisablePhysics()
	{
		this.disablePhysics = true;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000F357E File Offset: 0x000F177E
	public void EnablePhysics()
	{
		this.disablePhysics = false;
		this.rigidBody.isKinematic = false;
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000F3594 File Offset: 0x000F1794
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.IsEditableNow != this.prevEditable)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.prevEditable = this.IsEditableNow;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, this.rigidBody.isKinematic, false, true);
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x000F35E0 File Offset: 0x000F17E0
	public override bool MountEligable(global::BasePlayer player)
	{
		return base.MountEligable(player) && !this.IsDead() && (!base.HasDriver() || base.Velocity.magnitude < 2f);
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000F3622 File Offset: 0x000F1822
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.modularVehicle = Pool.Get<ModularVehicle>();
		info.msg.modularVehicle.editable = this.IsEditableNow;
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000F3654 File Offset: 0x000F1854
	public bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		BaseVehicleModule moduleForItem = this.GetModuleForItem(item);
		return !(moduleForItem != null) || moduleForItem.CanBeMovedNow();
	}

	// Token: 0x060026B7 RID: 9911
	protected abstract Vector3 GetCOMMultiplier();

	// Token: 0x060026B8 RID: 9912
	public abstract void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info);

	// Token: 0x060026B9 RID: 9913
	public abstract void ModuleReachedZeroHealth();

	// Token: 0x060026BA RID: 9914 RVA: 0x000F367C File Offset: 0x000F187C
	public bool TryAddModule(global::Item moduleItem, int socketIndex)
	{
		string text;
		if (!this.ModuleCanBeAdded(moduleItem, socketIndex, out text))
		{
			Debug.LogError(base.GetType().Name + ": Can't add module: " + text);
			return false;
		}
		bool flag = this.Inventory.TryAddModuleItem(moduleItem, socketIndex);
		if (!flag)
		{
			Debug.LogError(base.GetType().Name + ": Couldn't add new item!");
		}
		return flag;
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x000F36DC File Offset: 0x000F18DC
	public bool TryAddModule(global::Item moduleItem)
	{
		ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
		if (component == null)
		{
			return false;
		}
		int socketsTaken = component.socketsTaken;
		int num = this.Inventory.TryGetFreeSocket(socketsTaken);
		return num >= 0 && this.TryAddModule(moduleItem, num);
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x000F3724 File Offset: 0x000F1924
	public bool ModuleCanBeAdded(global::Item moduleItem, int socketIndex, out string failureReason)
	{
		if (!base.isServer)
		{
			failureReason = "Can only add modules on server";
			return false;
		}
		if (moduleItem == null)
		{
			failureReason = "Module item is null";
			return false;
		}
		if (moduleItem.info.category != ItemCategory.Component)
		{
			failureReason = "Not a component type item";
			return false;
		}
		ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
		if (component == null)
		{
			failureReason = "Not the right item module type";
			return false;
		}
		int socketsTaken = component.socketsTaken;
		if (socketIndex < 0)
		{
			socketIndex = this.Inventory.TryGetFreeSocket(socketsTaken);
		}
		if (!this.Inventory.SocketsAreFree(socketIndex, socketsTaken, moduleItem))
		{
			failureReason = "One or more desired sockets already in use";
			return false;
		}
		failureReason = string.Empty;
		return true;
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000F37C0 File Offset: 0x000F19C0
	public BaseVehicleModule CreatePhysicalModuleEntity(global::Item moduleItem, ItemModVehicleModule itemModModule, int socketIndex)
	{
		Vector3 worldPosition = this.moduleSockets[socketIndex].WorldPosition;
		Quaternion worldRotation = this.moduleSockets[socketIndex].WorldRotation;
		BaseVehicleModule baseVehicleModule = itemModModule.CreateModuleEntity(this, worldPosition, worldRotation);
		baseVehicleModule.AssociatedItemInstance = moduleItem;
		this.SetUpModule(baseVehicleModule, moduleItem);
		return baseVehicleModule;
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000F380B File Offset: 0x000F1A0B
	public void SetUpModule(BaseVehicleModule moduleEntity, global::Item moduleItem)
	{
		moduleEntity.InitializeHealth(moduleItem.condition, moduleItem.maxCondition);
		if (moduleItem.condition < moduleItem.maxCondition)
		{
			moduleEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000F3834 File Offset: 0x000F1A34
	public global::Item GetVehicleItem(ItemId itemUID)
	{
		global::Item item = this.Inventory.ChassisContainer.FindItemByUID(itemUID);
		if (item == null)
		{
			item = this.Inventory.ModuleContainer.FindItemByUID(itemUID);
		}
		return item;
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000F386C File Offset: 0x000F1A6C
	public BaseVehicleModule GetModuleForItem(global::Item item)
	{
		if (item == null)
		{
			return null;
		}
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			if (baseVehicleModule.AssociatedItemInstance == item)
			{
				return baseVehicleModule;
			}
		}
		return null;
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000F38D0 File Offset: 0x000F1AD0
	private void SetMass(float mass)
	{
		this.TotalMass = mass;
		this.rigidBody.mass = this.TotalMass;
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000F38EA File Offset: 0x000F1AEA
	private void SetCOM(Vector3 com)
	{
		this.realLocalCOM = com;
		this.rigidBody.centerOfMass = Vector3.Scale(this.realLocalCOM, this.GetCOMMultiplier());
	}

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x060026C3 RID: 9923 RVA: 0x000F390F File Offset: 0x000F1B0F
	public Vector3 CentreOfMass
	{
		get
		{
			return this.centreOfMassTransform.localPosition;
		}
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x060026C4 RID: 9924 RVA: 0x000F391C File Offset: 0x000F1B1C
	public int NumAttachedModules
	{
		get
		{
			return this.AttachedModuleEntities.Count;
		}
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x060026C5 RID: 9925 RVA: 0x000F3929 File Offset: 0x000F1B29
	public bool HasAnyModules
	{
		get
		{
			return this.AttachedModuleEntities.Count > 0;
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x060026C6 RID: 9926 RVA: 0x000F3939 File Offset: 0x000F1B39
	public List<BaseVehicleModule> AttachedModuleEntities { get; } = new List<BaseVehicleModule>();

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x060026C7 RID: 9927 RVA: 0x000F3941 File Offset: 0x000F1B41
	public int TotalSockets
	{
		get
		{
			return this.moduleSockets.Count;
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x060026C8 RID: 9928 RVA: 0x000F3950 File Offset: 0x000F1B50
	public int NumFreeSockets
	{
		get
		{
			int num = 0;
			for (int i = 0; i < this.NumAttachedModules; i++)
			{
				num += this.AttachedModuleEntities[i].GetNumSocketsTaken();
			}
			return this.TotalSockets - num;
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x060026C9 RID: 9929 RVA: 0x000F398C File Offset: 0x000F1B8C
	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return this.rigidBody.mass;
			}
			return this._mass;
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x060026CA RID: 9930 RVA: 0x000F39A8 File Offset: 0x000F1BA8
	// (set) Token: 0x060026CB RID: 9931 RVA: 0x000F39B0 File Offset: 0x000F1BB0
	public float TotalMass { get; private set; }

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x060026CC RID: 9932 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsKinematic
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x060026CD RID: 9933 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsLockable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x060026CE RID: 9934 RVA: 0x000F39B9 File Offset: 0x000F1BB9
	// (set) Token: 0x060026CF RID: 9935 RVA: 0x000F39C1 File Offset: 0x000F1BC1
	public bool HasInited { get; private set; }

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x060026D0 RID: 9936 RVA: 0x000512B3 File Offset: 0x0004F4B3
	private ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x060026D1 RID: 9937 RVA: 0x000F39CA File Offset: 0x000F1BCA
	public bool IsEditableNow
	{
		get
		{
			return base.isServer && this.inEditableLocation && this.CouldBeEdited();
		}
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x000F39E8 File Offset: 0x000F1BE8
	public override void InitShared()
	{
		base.InitShared();
		this.AddMass(this.Mass, this.CentreOfMass, base.transform.position);
		this.HasInited = true;
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			baseVehicleModule.RefreshConditionals(false);
		}
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool PlayerCanUseThis(global::BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return true;
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x000F3A64 File Offset: 0x000F1C64
	public bool TryDeduceSocketIndex(BaseVehicleModule addedModule, out int index)
	{
		if (addedModule.FirstSocketIndex >= 0)
		{
			index = addedModule.FirstSocketIndex;
			return index >= 0;
		}
		index = -1;
		for (int i = 0; i < this.moduleSockets.Count; i++)
		{
			if (Vector3.SqrMagnitude(this.moduleSockets[i].WorldPosition - addedModule.transform.position) < 0.1f)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000F3AD8 File Offset: 0x000F1CD8
	public void AddMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		if (base.isServer)
		{
			Vector3 vector = base.transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			if (this.TotalMass == 0f)
			{
				this.SetMass(moduleMass);
				this.SetCOM(vector);
				return;
			}
			float num = this.TotalMass + moduleMass;
			Vector3 vector2 = this.realLocalCOM * (this.TotalMass / num) + vector * (moduleMass / num);
			this.SetMass(num);
			this.SetCOM(vector2);
		}
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000F3B58 File Offset: 0x000F1D58
	public void RemoveMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		if (base.isServer)
		{
			float num = this.TotalMass - moduleMass;
			Vector3 vector = base.transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			Vector3 vector2 = (this.realLocalCOM - vector * (moduleMass / this.TotalMass)) / (num / this.TotalMass);
			this.SetMass(num);
			this.SetCOM(vector2);
		}
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x000F3BC0 File Offset: 0x000F1DC0
	public bool TryGetModuleAt(int socketIndex, out BaseVehicleModule result)
	{
		if (socketIndex < 0 || socketIndex >= this.moduleSockets.Count)
		{
			result = null;
			return false;
		}
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			int firstSocketIndex = baseVehicleModule.FirstSocketIndex;
			int num = firstSocketIndex + baseVehicleModule.GetNumSocketsTaken() - 1;
			if (firstSocketIndex <= socketIndex && num >= socketIndex)
			{
				result = baseVehicleModule;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x000F3C48 File Offset: 0x000F1E48
	public ModularVehicleSocket GetSocket(int index)
	{
		if (index < 0 || index >= this.moduleSockets.Count)
		{
			return null;
		}
		return this.moduleSockets[index];
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000F3C6A File Offset: 0x000F1E6A
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ModularVehicle modularVehicle = info.msg.modularVehicle;
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000F3C7F File Offset: 0x000F1E7F
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !this.IsKinematic && !this.IsEditableNow;
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000F3CA0 File Offset: 0x000F1EA0
	protected override void OnChildAdded(global::BaseEntity childEntity)
	{
		base.OnChildAdded(childEntity);
		BaseVehicleModule module;
		if ((module = childEntity as BaseVehicleModule) != null)
		{
			Action action = delegate
			{
				this.ModuleEntityAdded(module);
			};
			this.moduleAddActions[module] = action;
			module.Invoke(action, 0f);
		}
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000F3D04 File Offset: 0x000F1F04
	protected override void OnChildRemoved(global::BaseEntity childEntity)
	{
		base.OnChildRemoved(childEntity);
		BaseVehicleModule baseVehicleModule;
		if ((baseVehicleModule = childEntity as BaseVehicleModule) != null)
		{
			this.ModuleEntityRemoved(baseVehicleModule);
		}
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000F3D2C File Offset: 0x000F1F2C
	protected virtual void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		if (this.AttachedModuleEntities.Contains(addedModule))
		{
			return;
		}
		if (base.isServer && (this == null || this.IsDead() || base.IsDestroyed))
		{
			if (addedModule != null && !addedModule.IsDestroyed)
			{
				addedModule.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			return;
		}
		int num = -1;
		if (base.isServer && addedModule.AssociatedItemInstance != null)
		{
			num = addedModule.AssociatedItemInstance.position;
		}
		if (num == -1 && !this.TryDeduceSocketIndex(addedModule, out num))
		{
			string text = string.Format("{0}: Couldn't get socket index from position ({1}).", base.GetType().Name, addedModule.transform.position);
			for (int i = 0; i < this.moduleSockets.Count; i++)
			{
				text += string.Format(" Sqr dist to socket {0} at {1} is {2}.", i, this.moduleSockets[i].WorldPosition, Vector3.SqrMagnitude(this.moduleSockets[i].WorldPosition - addedModule.transform.position));
			}
			Debug.LogError(text, addedModule.gameObject);
			return;
		}
		if (this.moduleAddActions.ContainsKey(addedModule))
		{
			this.moduleAddActions.Remove(addedModule);
		}
		this.AttachedModuleEntities.Add(addedModule);
		addedModule.ModuleAdded(this, num);
		this.AddMass(addedModule.Mass, addedModule.CentreOfMass, addedModule.transform.position);
		if (base.isServer && !this.Inventory.TrySyncModuleInventory(addedModule, num))
		{
			Debug.LogError(string.Format("{0}: Unable to add module {1} to socket ({2}). Destroying it.", base.GetType().Name, addedModule.name, num), base.gameObject);
			addedModule.Kill(global::BaseNetworkable.DestroyMode.None);
			this.AttachedModuleEntities.Remove(addedModule);
			return;
		}
		this.RefreshModulesExcept(addedModule);
		if (base.isServer)
		{
			this.UpdateMountFlags();
		}
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000F3F10 File Offset: 0x000F2110
	protected virtual void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.moduleAddActions.ContainsKey(removedModule))
		{
			removedModule.CancelInvoke(this.moduleAddActions[removedModule]);
			this.moduleAddActions.Remove(removedModule);
		}
		if (!this.AttachedModuleEntities.Contains(removedModule))
		{
			return;
		}
		this.RemoveMass(removedModule.Mass, removedModule.CentreOfMass, removedModule.transform.position);
		this.AttachedModuleEntities.Remove(removedModule);
		removedModule.ModuleRemoved();
		this.RefreshModulesExcept(removedModule);
		if (base.isServer)
		{
			this.UpdateMountFlags();
		}
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000F3FA8 File Offset: 0x000F21A8
	private void RefreshModulesExcept(BaseVehicleModule ignoredModule)
	{
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			if (baseVehicleModule != ignoredModule)
			{
				baseVehicleModule.OtherVehicleModulesChanged();
			}
		}
	}

	// Token: 0x04001F25 RID: 7973
	internal bool inEditableLocation;

	// Token: 0x04001F26 RID: 7974
	private bool prevEditable;

	// Token: 0x04001F27 RID: 7975
	internal bool immuneToDecay;

	// Token: 0x04001F29 RID: 7977
	protected Vector3 realLocalCOM;

	// Token: 0x04001F2A RID: 7978
	public global::Item AssociatedItemInstance;

	// Token: 0x04001F2B RID: 7979
	private bool disablePhysics;

	// Token: 0x04001F2C RID: 7980
	[Header("Modular Vehicle")]
	[SerializeField]
	private List<ModularVehicleSocket> moduleSockets;

	// Token: 0x04001F2D RID: 7981
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04001F2E RID: 7982
	[SerializeField]
	protected Transform waterSample;

	// Token: 0x04001F2F RID: 7983
	[SerializeField]
	private LODGroup lodGroup;

	// Token: 0x04001F30 RID: 7984
	public GameObjectRef keyEnterDialog;

	// Token: 0x04001F32 RID: 7986
	private float _mass = -1f;

	// Token: 0x04001F35 RID: 7989
	public const global::BaseEntity.Flags FLAG_KINEMATIC = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04001F36 RID: 7990
	private Dictionary<BaseVehicleModule, Action> moduleAddActions = new Dictionary<BaseVehicleModule, Action>();
}
