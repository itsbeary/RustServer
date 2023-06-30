using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000089 RID: 137
public class IOEntity : global::DecayEntity
{
	// Token: 0x06000CC6 RID: 3270 RVA: 0x0006E7E0 File Offset: 0x0006C9E0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IOEntity.OnRpcMessage", 0))
		{
			if (rpc == 4161541566U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestData ");
				}
				using (TimeWarning.New("Server_RequestData", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4161541566U, "Server_RequestData", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4161541566U, "Server_RequestData", this, player, 6f))
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
							this.Server_RequestData(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_RequestData");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0006E9A4 File Offset: 0x0006CBA4
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.lastResetIndex = 0;
			this.cachedOutputsUsed = 0;
			this.lastPassthroughEnergy = 0;
			this.lastEnergy = 0;
			this.currentEnergy = 0;
			this.lastUpdateTime = 0f;
			this.ensureOutputsUpdated = false;
		}
		this.ClearIndustrialPreventBuilding();
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0006E9FA File Offset: 0x0006CBFA
	public string GetDisplayName()
	{
		if (this.sourceItem != null)
		{
			return this.sourceItem.displayName.translated;
		}
		return base.ShortPrefabName;
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsRootEntity()
	{
		return false;
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000CCA RID: 3274 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool IsGravitySource
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0006EA24 File Offset: 0x0006CC24
	public global::IOEntity FindGravitySource(ref Vector3 worldHandlePosition, int depth, bool ignoreSelf)
	{
		if (depth <= 0)
		{
			return null;
		}
		if (!ignoreSelf && this.IsGravitySource)
		{
			worldHandlePosition = base.transform.TransformPoint(this.outputs[0].handlePosition);
			return this;
		}
		global::IOEntity.IOSlot[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			global::IOEntity ioentity = array[i].connectedTo.Get(base.isServer);
			if (ioentity != null)
			{
				if (ioentity.IsGravitySource)
				{
					worldHandlePosition = ioentity.transform.TransformPoint(ioentity.outputs[0].handlePosition);
					return ioentity;
				}
				ioentity = ioentity.FindGravitySource(ref worldHandlePosition, depth - 1, false);
				if (ioentity != null)
				{
					worldHandlePosition = ioentity.transform.TransformPoint(ioentity.outputs[0].handlePosition);
					return ioentity;
				}
			}
		}
		return null;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void SetFuelType(ItemDefinition def, global::IOEntity source)
	{
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool WantsPower()
	{
		return true;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0006EAF5 File Offset: 0x0006CCF5
	public virtual bool AllowWireConnections()
	{
		return !(base.GetComponentInParent<global::BaseVehicle>() != null);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0006EB08 File Offset: 0x0006CD08
	public virtual bool WantsPassthroughPower()
	{
		return this.WantsPower();
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x0006EB10 File Offset: 0x0006CD10
	public virtual bool ShouldDrainBattery(global::IOEntity battery)
	{
		return this.ioType == battery.ioType;
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual int MaximalPowerOutput()
	{
		return 0;
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool AllowDrainFrom(int outputSlot)
	{
		return true;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00003278 File Offset: 0x00001478
	public virtual bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x0006EB20 File Offset: 0x0006CD20
	public bool IsConnectedToAnySlot(global::IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			global::IOEntity ioentity = this.inputs[slot].connectedTo.Get(true);
			if (ioentity != null)
			{
				if (ioentity == entity)
				{
					return true;
				}
				if (this.ConsiderConnectedTo(entity))
				{
					return true;
				}
				if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x0006EB80 File Offset: 0x0006CD80
	public bool IsConnectedTo(global::IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < this.inputs.Length)
		{
			global::IOEntity.IOSlot ioslot = this.inputs[slot];
			if (ioslot.mainPowerSlot)
			{
				global::IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null)
				{
					if (ioentity == entity)
					{
						return true;
					}
					if (this.ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x0006EBEC File Offset: 0x0006CDEC
	public bool IsConnectedTo(global::IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth > 0)
		{
			for (int i = 0; i < this.inputs.Length; i++)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (ioentity == entity)
						{
							return true;
						}
						if (this.ConsiderConnectedTo(entity))
						{
							return true;
						}
						if (ioentity.IsConnectedTo(entity, depth - 1, defaultReturn))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return defaultReturn;
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool ConsiderConnectedTo(global::IOEntity entity)
	{
		return false;
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x0006EC64 File Offset: 0x0006CE64
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	private void Server_RequestData(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		int num = msg.read.Int32();
		bool flag = msg.read.Int32() == 1;
		this.SendAdditionalData(player, num, flag);
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x0006EC9C File Offset: 0x0006CE9C
	public virtual void SendAdditionalData(global::BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = this.GetPassthroughAmountForAnySlot(slot, input);
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, passthroughAmountForAnySlot, 0f, 0f);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x0006ECD0 File Offset: 0x0006CED0
	protected int GetPassthroughAmountForAnySlot(int slot, bool isInputSlot)
	{
		int num = 0;
		if (isInputSlot)
		{
			if (slot >= 0 && slot < this.inputs.Length)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[slot];
				global::IOEntity ioentity = ioslot.connectedTo.Get(true);
				if (ioentity != null && ioslot.connectedToSlot >= 0 && ioslot.connectedToSlot < ioentity.outputs.Length)
				{
					num = ioentity.GetPassthroughAmount(this.inputs[slot].connectedToSlot);
				}
			}
		}
		else if (slot >= 0 && slot < this.outputs.Length)
		{
			num = this.GetPassthroughAmount(slot);
		}
		return num;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x0006ED58 File Offset: 0x0006CF58
	public static void ProcessQueue()
	{
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = global::IOEntity.framebudgetms / 1000f;
		if (global::IOEntity.debugBudget)
		{
			global::IOEntity.timings.Clear();
		}
		while (global::IOEntity._processQueue.Count > 0 && UnityEngine.Time.realtimeSinceStartup < realtimeSinceStartup + num && !global::IOEntity._processQueue.Peek().HasBlockedUpdatedOutputsThisFrame)
		{
			float realtimeSinceStartup2 = UnityEngine.Time.realtimeSinceStartup;
			global::IOEntity ioentity = global::IOEntity._processQueue.Dequeue();
			if (ioentity.IsValid())
			{
				ioentity.UpdateOutputs();
			}
			if (global::IOEntity.debugBudget)
			{
				global::IOEntity.timings.Add(new global::IOEntity.FrameTiming
				{
					PrefabName = ioentity.ShortPrefabName,
					Time = (UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup2) * 1000f
				});
			}
		}
		if (global::IOEntity.debugBudget)
		{
			float num2 = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
			float num3 = global::IOEntity.debugBudgetThreshold / 1000f;
			if (num2 > num3)
			{
				TextTable textTable = new TextTable();
				textTable.AddColumns(new string[] { "Prefab Name", "Time (in ms)" });
				foreach (global::IOEntity.FrameTiming frameTiming in global::IOEntity.timings)
				{
					TextTable textTable2 = textTable;
					string[] array = new string[2];
					array[0] = frameTiming.PrefabName;
					int num4 = 1;
					float time = frameTiming.Time;
					array[num4] = time.ToString();
					textTable2.AddRow(array);
				}
				textTable.AddRow(new string[]
				{
					"Total time",
					(num2 * 1000f).ToString()
				});
				Debug.Log(textTable.ToString());
			}
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ResetIOState()
	{
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x0006EF00 File Offset: 0x0006D100
	public virtual void Init()
	{
		for (int i = 0; i < this.outputs.Length; i++)
		{
			global::IOEntity.IOSlot ioslot = this.outputs[i];
			ioslot.connectedTo.Init();
			if (ioslot.connectedTo.Get(true) != null)
			{
				int connectedToSlot = ioslot.connectedToSlot;
				if (connectedToSlot < 0 || connectedToSlot >= ioslot.connectedTo.Get(true).inputs.Length)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Slot IOR Error: ",
						base.name,
						" setting up inputs for ",
						ioslot.connectedTo.Get(true).name,
						" slot : ",
						ioslot.connectedToSlot
					}));
				}
				else
				{
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedTo.Set(this);
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedToSlot = i;
					ioslot.connectedTo.Get(true).inputs[ioslot.connectedToSlot].connectedTo.Init();
				}
			}
		}
		this.UpdateUsedOutputs();
		if (this.IsRootEntity())
		{
			base.Invoke(new Action(this.MarkDirtyForceUpdateOutputs), UnityEngine.Random.Range(1f, 1f));
		}
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x0006F059 File Offset: 0x0006D259
	internal override void DoServerDestroy()
	{
		if (base.isServer)
		{
			this.Shutdown();
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0006F070 File Offset: 0x0006D270
	public void ClearConnections()
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		List<global::IOEntity> list2 = Facepunch.Pool.GetList<global::IOEntity>();
		foreach (global::IOEntity.IOSlot ioslot in this.inputs)
		{
			global::IOEntity ioentity = null;
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioentity = ioslot.connectedTo.Get(true);
				if (ioslot.type == global::IOEntity.IOType.Industrial)
				{
					list2.Add(ioentity);
				}
				foreach (global::IOEntity.IOSlot ioslot2 in ioslot.connectedTo.Get(true).outputs)
				{
					if (ioslot2.connectedTo.Get(true) != null && ioslot2.connectedTo.Get(true).EqualNetID(this))
					{
						ioslot2.Clear();
					}
				}
			}
			ioslot.Clear();
			if (ioentity)
			{
				ioentity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		foreach (global::IOEntity.IOSlot ioslot3 in this.outputs)
		{
			if (ioslot3.connectedTo.Get(true) != null)
			{
				list.Add(ioslot3.connectedTo.Get(true));
				if (ioslot3.type == global::IOEntity.IOType.Industrial)
				{
					list2.Add(list[list.Count - 1]);
				}
				foreach (global::IOEntity.IOSlot ioslot4 in ioslot3.connectedTo.Get(true).inputs)
				{
					if (ioslot4.connectedTo.Get(true) != null && ioslot4.connectedTo.Get(true).EqualNetID(this))
					{
						ioslot4.Clear();
					}
				}
			}
			if (ioslot3.connectedTo.Get(true))
			{
				ioslot3.connectedTo.Get(true).UpdateFromInput(0, ioslot3.connectedToSlot);
			}
			ioslot3.Clear();
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		foreach (global::IOEntity ioentity2 in list)
		{
			if (ioentity2 != null)
			{
				ioentity2.MarkDirty();
				ioentity2.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		for (int k = 0; k < this.inputs.Length; k++)
		{
			this.UpdateFromInput(0, k);
		}
		foreach (global::IOEntity ioentity3 in list2)
		{
			if (ioentity3 != null)
			{
				ioentity3.NotifyIndustrialNetworkChanged();
			}
			ioentity3.RefreshIndustrialPreventBuilding();
		}
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list2);
		this.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x0006F33C File Offset: 0x0006D53C
	public void Shutdown()
	{
		this.SendChangedToRoot(true);
		this.ClearConnections();
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x0006F34B File Offset: 0x0006D54B
	public void MarkDirtyForceUpdateOutputs()
	{
		this.ensureOutputsUpdated = true;
		this.MarkDirty();
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0006F35C File Offset: 0x0006D55C
	public void UpdateUsedOutputs()
	{
		this.cachedOutputsUsed = 0;
		global::IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			global::IOEntity ioentity = array[i].connectedTo.Get(true);
			if (ioentity != null && !ioentity.IsDestroyed)
			{
				this.cachedOutputsUsed++;
			}
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0006F3B3 File Offset: 0x0006D5B3
	public virtual void MarkDirty()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateUsedOutputs();
		this.TouchIOState();
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0006F3CA File Offset: 0x0006D5CA
	public virtual int DesiredPower()
	{
		return this.ConsumptionAmount();
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00036ECC File Offset: 0x000350CC
	public virtual int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0006F3D2 File Offset: 0x0006D5D2
	public virtual int GetCurrentEnergy()
	{
		return Mathf.Clamp(this.currentEnergy - this.ConsumptionAmount(), 0, this.currentEnergy);
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0006F3F0 File Offset: 0x0006D5F0
	public virtual int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot < 0 || outputSlot >= this.outputs.Length)
		{
			return 0;
		}
		global::IOEntity ioentity = this.outputs[outputSlot].connectedTo.Get(true);
		if (ioentity == null || ioentity.IsDestroyed)
		{
			return 0;
		}
		int num = ((this.cachedOutputsUsed == 0) ? 1 : this.cachedOutputsUsed);
		return this.GetCurrentEnergy() / num;
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0006F44F File Offset: 0x0006D64F
	public virtual void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount() && inputAmount > 0, false, false);
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0006F470 File Offset: 0x0006D670
	public void TouchInternal()
	{
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (flag)
		{
			this.IOStateChanged(this.currentEnergy, 0);
			this.ensureOutputsUpdated = true;
		}
		global::IOEntity._processQueue.Enqueue(this);
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0006F4BC File Offset: 0x0006D6BC
	public virtual void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.inputs[inputSlot].type != this.ioType || this.inputs[inputSlot].type == global::IOEntity.IOType.Industrial)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			return;
		}
		this.UpdateHasPower(inputAmount, inputSlot);
		this.lastEnergy = this.currentEnergy;
		this.currentEnergy = this.CalculateCurrentEnergy(inputAmount, inputSlot);
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (this.currentEnergy != this.lastEnergy || flag)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			this.ensureOutputsUpdated = true;
		}
		global::IOEntity._processQueue.Enqueue(this);
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x0006F564 File Offset: 0x0006D764
	public virtual void TouchIOState()
	{
		if (base.isClient)
		{
			return;
		}
		this.TouchInternal();
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0006F575 File Offset: 0x0006D775
	public virtual void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate_Flags();
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void IOStateChanged(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0006F57D File Offset: 0x0006D77D
	public virtual void OnCircuitChanged(bool forceUpdate)
	{
		if (forceUpdate)
		{
			this.MarkDirtyForceUpdateOutputs();
		}
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x0006F588 File Offset: 0x0006D788
	public virtual void SendChangedToRoot(bool forceUpdate)
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		this.SendChangedToRootRecursive(forceUpdate, ref list);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0006F5AC File Offset: 0x0006D7AC
	public virtual void SendChangedToRootRecursive(bool forceUpdate, ref List<global::IOEntity> existing)
	{
		bool flag = this.IsRootEntity();
		if (!existing.Contains(this))
		{
			existing.Add(this);
			bool flag2 = false;
			for (int i = 0; i < this.inputs.Length; i++)
			{
				global::IOEntity.IOSlot ioslot = this.inputs[i];
				if (ioslot.mainPowerSlot)
				{
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (!(ioentity == null) && !existing.Contains(ioentity))
					{
						flag2 = true;
						if (forceUpdate)
						{
							ioentity.ensureOutputsUpdated = true;
						}
						ioentity.SendChangedToRootRecursive(forceUpdate, ref existing);
					}
				}
			}
			if (flag)
			{
				forceUpdate = forceUpdate && !flag2;
				this.OnCircuitChanged(forceUpdate);
			}
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x0006F648 File Offset: 0x0006D848
	public void NotifyIndustrialNetworkChanged()
	{
		List<global::IOEntity> list = Facepunch.Pool.GetList<global::IOEntity>();
		this.OnIndustrialNetworkChanged();
		this.NotifyIndustrialNetworkChanged(list, true, 128);
		list.Clear();
		this.NotifyIndustrialNetworkChanged(list, false, 128);
		Facepunch.Pool.FreeList<global::IOEntity>(ref list);
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x0006F688 File Offset: 0x0006D888
	private void NotifyIndustrialNetworkChanged(List<global::IOEntity> existing, bool input, int maxDepth)
	{
		if (maxDepth <= 0)
		{
			return;
		}
		if (!existing.Contains(this))
		{
			if (existing.Count != 0)
			{
				this.OnIndustrialNetworkChanged();
			}
			existing.Add(this);
			foreach (global::IOEntity.IOSlot ioslot in input ? this.inputs : this.outputs)
			{
				if (ioslot.type == global::IOEntity.IOType.Industrial && ioslot.connectedTo.Get(true) != null)
				{
					ioslot.connectedTo.Get(true).NotifyIndustrialNetworkChanged(existing, input, maxDepth - 1);
				}
			}
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnIndustrialNetworkChanged()
	{
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x0006F710 File Offset: 0x0006D910
	protected bool ShouldUpdateOutputs()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < global::IOEntity.responsetime)
		{
			this.lastUpdateBlockedFrame = UnityEngine.Time.frameCount;
			global::IOEntity._processQueue.Enqueue(this);
			return false;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.SendIONetworkUpdate();
		if (this.outputs.Length == 0)
		{
			this.ensureOutputsUpdated = false;
			return false;
		}
		return true;
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x0006F76C File Offset: 0x0006D96C
	private bool HasBlockedUpdatedOutputsThisFrame
	{
		get
		{
			return UnityEngine.Time.frameCount == this.lastUpdateBlockedFrame;
		}
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x0006F77C File Offset: 0x0006D97C
	public virtual void UpdateOutputs()
	{
		if (!this.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			this.ensureOutputsUpdated = false;
			using (TimeWarning.New("ProcessIOOutputs", 0))
			{
				for (int i = 0; i < this.outputs.Length; i++)
				{
					global::IOEntity.IOSlot ioslot = this.outputs[i];
					bool flag = true;
					global::IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						if (this.ioType == global::IOEntity.IOType.Fluidic && !this.DisregardGravityRestrictionsOnLiquid && !ioentity.DisregardGravityRestrictionsOnLiquid)
						{
							using (TimeWarning.New("FluidOutputProcessing", 0))
							{
								if (!ioentity.AllowLiquidPassthrough(this, base.transform.TransformPoint(ioslot.handlePosition), false))
								{
									flag = false;
								}
							}
						}
						int passthroughAmount = this.GetPassthroughAmount(i);
						ioentity.UpdateFromInput(flag ? passthroughAmount : 0, ioslot.connectedToSlot);
					}
				}
			}
		}
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x0006F888 File Offset: 0x0006DA88
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.Init();
		}
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0006F89D File Offset: 0x0006DA9D
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0006F8AB File Offset: 0x0006DAAB
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0006F8BC File Offset: 0x0006DABC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.inputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		info.msg.ioEntity.outputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		foreach (global::IOEntity.IOSlot ioslot in this.inputs)
		{
			ProtoBuf.IOEntity.IOConnection ioconnection = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			ioconnection.connectedID = ioslot.connectedTo.entityRef.uid;
			ioconnection.connectedToSlot = ioslot.connectedToSlot;
			ioconnection.niceName = ioslot.niceName;
			ioconnection.type = (int)ioslot.type;
			ioconnection.inUse = ioconnection.connectedID.IsValid;
			ioconnection.colour = (int)ioslot.wireColour;
			ioconnection.lineThickness = ioslot.lineThickness;
			info.msg.ioEntity.inputs.Add(ioconnection);
		}
		foreach (global::IOEntity.IOSlot ioslot2 in this.outputs)
		{
			ProtoBuf.IOEntity.IOConnection ioconnection2 = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			ioconnection2.connectedID = ioslot2.connectedTo.entityRef.uid;
			ioconnection2.connectedToSlot = ioslot2.connectedToSlot;
			ioconnection2.niceName = ioslot2.niceName;
			ioconnection2.type = (int)ioslot2.type;
			ioconnection2.inUse = ioconnection2.connectedID.IsValid;
			ioconnection2.colour = (int)ioslot2.wireColour;
			ioconnection2.worldSpaceRotation = ioslot2.worldSpaceLineEndRotation;
			ioconnection2.lineThickness = ioslot2.lineThickness;
			if (ioslot2.linePoints != null)
			{
				ioconnection2.linePointList = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection.LineVec>();
				ioconnection2.linePointList.Clear();
				for (int j = 0; j < ioslot2.linePoints.Length; j++)
				{
					Vector3 vector = ioslot2.linePoints[j];
					ProtoBuf.IOEntity.IOConnection.LineVec lineVec = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection.LineVec>();
					lineVec.vec = vector;
					if (ioslot2.slackLevels.Length > j)
					{
						lineVec.vec.w = ioslot2.slackLevels[j];
					}
					ioconnection2.linePointList.Add(lineVec);
				}
			}
			info.msg.ioEntity.outputs.Add(ioconnection2);
		}
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x0006FAF8 File Offset: 0x0006DCF8
	public virtual float IOInput(global::IOEntity from, global::IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				inputAmount = ioslot.connectedTo.Get(true).IOInput(this, ioslot.type, inputAmount, ioslot.connectedToSlot);
			}
		}
		return inputAmount;
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000CFD RID: 3325 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool BlockFluidDraining
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x0006FB54 File Offset: 0x0006DD54
	public void FindContainerSource(List<global::IOEntity.ContainerInputOutput> found, int depth, bool input, int parentId = -1, int stackSize = 0)
	{
		global::IOEntity.<>c__DisplayClass87_0 CS$<>8__locals1;
		CS$<>8__locals1.found = found;
		if (depth <= 0 || CS$<>8__locals1.found.Count >= 32)
		{
			return;
		}
		int num = 0;
		int num2 = 1;
		if (!input)
		{
			num2 = 0;
			global::IOEntity.IOSlot[] array = this.outputs;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].type == global::IOEntity.IOType.Industrial)
				{
					num2++;
				}
			}
		}
		List<int> list = Facepunch.Pool.GetList<int>();
		foreach (global::IOEntity.IOSlot ioslot in input ? this.inputs : this.outputs)
		{
			num++;
			if (ioslot.type == global::IOEntity.IOType.Industrial)
			{
				global::IOEntity ioentity = ioslot.connectedTo.Get(base.isServer);
				if (ioentity != null)
				{
					int num3 = -1;
					IIndustrialStorage industrialStorage;
					if ((industrialStorage = ioentity as IIndustrialStorage) != null)
					{
						num = ioslot.connectedToSlot;
						if (global::IOEntity.<FindContainerSource>g__GetExistingCount|87_0(industrialStorage, ref CS$<>8__locals1) < 2)
						{
							CS$<>8__locals1.found.Add(new global::IOEntity.ContainerInputOutput
							{
								SlotIndex = num,
								Storage = industrialStorage,
								ParentStorage = parentId,
								MaxStackSize = stackSize / num2
							});
							num3 = CS$<>8__locals1.found.Count - 1;
							list.Add(num3);
						}
					}
					if ((!(ioentity is IIndustrialStorage) || ioentity is IndustrialStorageAdaptor) && !(ioentity is global::IndustrialConveyor) && ioentity != null)
					{
						ioentity.FindContainerSource(CS$<>8__locals1.found, depth - 1, input, (num3 == -1) ? parentId : num3, stackSize / num2);
					}
				}
			}
		}
		int count = list.Count;
		foreach (int num4 in list)
		{
			global::IOEntity.ContainerInputOutput containerInputOutput = CS$<>8__locals1.found[num4];
			containerInputOutput.IndustrialSiblingCount = count;
			CS$<>8__locals1.found[num4] = containerInputOutput;
		}
		Facepunch.Pool.FreeList<int>(ref list);
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000CFF RID: 3327 RVA: 0x00006CA5 File Offset: 0x00004EA5
	protected virtual float LiquidPassthroughGravityThreshold
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000D00 RID: 3328 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x0006FD48 File Offset: 0x0006DF48
	public virtual bool AllowLiquidPassthrough(global::IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		if (fromSource.DisregardGravityRestrictionsOnLiquid || this.DisregardGravityRestrictionsOnLiquid)
		{
			return true;
		}
		if (this.inputs.Length == 0)
		{
			return false;
		}
		Vector3 vector = base.transform.TransformPoint(this.inputs[0].handlePosition);
		float num = sourceWorldPosition.y - vector.y;
		return num > 0f || Mathf.Abs(num) < this.LiquidPassthroughGravityThreshold;
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x0006FDB4 File Offset: 0x0006DFB4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if (this.inputs.Length != count)
			{
				this.inputs = new global::IOEntity.IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (this.inputs[i] == null)
				{
					this.inputs[i] = new global::IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection ioconnection = info.msg.ioEntity.inputs[i];
				this.inputs[i].connectedTo = new global::IOEntity.IORef();
				this.inputs[i].connectedTo.entityRef.uid = ioconnection.connectedID;
				if (base.isClient)
				{
					this.inputs[i].connectedTo.InitClient();
				}
				this.inputs[i].connectedToSlot = ioconnection.connectedToSlot;
				this.inputs[i].niceName = ioconnection.niceName;
				this.inputs[i].type = (global::IOEntity.IOType)ioconnection.type;
				this.inputs[i].wireColour = (WireTool.WireColour)ioconnection.colour;
				this.inputs[i].lineThickness = ioconnection.lineThickness;
			}
		}
		if (info.msg.ioEntity.outputs != null)
		{
			int count2 = info.msg.ioEntity.outputs.Count;
			if (this.outputs.Length != count2 && count2 > 0)
			{
				global::IOEntity.IOSlot[] array = this.outputs;
				this.outputs = new global::IOEntity.IOSlot[count2];
				for (int j = 0; j < array.Length; j++)
				{
					if (j < count2)
					{
						this.outputs[j] = array[j];
					}
				}
			}
			for (int k = 0; k < count2; k++)
			{
				if (this.outputs[k] == null)
				{
					this.outputs[k] = new global::IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection ioconnection2 = info.msg.ioEntity.outputs[k];
				if (ioconnection2.linePointList == null || ioconnection2.linePointList.Count == 0 || !ioconnection2.connectedID.IsValid)
				{
					this.outputs[k].Clear();
				}
				this.outputs[k].connectedTo = new global::IOEntity.IORef();
				this.outputs[k].connectedTo.entityRef.uid = ioconnection2.connectedID;
				if (base.isClient)
				{
					this.outputs[k].connectedTo.InitClient();
				}
				this.outputs[k].connectedToSlot = ioconnection2.connectedToSlot;
				this.outputs[k].niceName = ioconnection2.niceName;
				this.outputs[k].type = (global::IOEntity.IOType)ioconnection2.type;
				this.outputs[k].wireColour = (WireTool.WireColour)ioconnection2.colour;
				this.outputs[k].worldSpaceLineEndRotation = ioconnection2.worldSpaceRotation;
				this.outputs[k].lineThickness = ioconnection2.lineThickness;
				if (info.fromDisk || base.isClient)
				{
					List<ProtoBuf.IOEntity.IOConnection.LineVec> linePointList = ioconnection2.linePointList;
					if (this.outputs[k].linePoints == null || this.outputs[k].linePoints.Length != linePointList.Count)
					{
						this.outputs[k].linePoints = new Vector3[linePointList.Count];
					}
					if (this.outputs[k].slackLevels == null || this.outputs[k].slackLevels.Length != linePointList.Count)
					{
						this.outputs[k].slackLevels = new float[linePointList.Count];
					}
					for (int l = 0; l < linePointList.Count; l++)
					{
						this.outputs[k].linePoints[l] = linePointList[l].vec;
						this.outputs[k].slackLevels[l] = linePointList[l].vec.w;
					}
				}
			}
		}
		this.RefreshIndustrialPreventBuilding();
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x000701D4 File Offset: 0x0006E3D4
	public int GetConnectedInputCount()
	{
		int num = 0;
		global::IOEntity.IOSlot[] array = this.inputs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].connectedTo.Get(base.isServer) != null)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00070218 File Offset: 0x0006E418
	public int GetConnectedOutputCount()
	{
		int num = 0;
		global::IOEntity.IOSlot[] array = this.outputs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].connectedTo.Get(base.isServer) != null)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0007025C File Offset: 0x0006E45C
	public bool HasConnections()
	{
		return this.GetConnectedInputCount() > 0 || this.GetConnectedOutputCount() > 0;
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00070272 File Offset: 0x0006E472
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.ClearIndustrialPreventBuilding();
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x00070280 File Offset: 0x0006E480
	public void RefreshIndustrialPreventBuilding()
	{
		this.ClearIndustrialPreventBuilding();
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.outputs.Length; i++)
		{
			global::IOEntity.IOSlot ioslot = this.outputs[i];
			if (ioslot.type == global::IOEntity.IOType.Industrial && ioslot.linePoints != null && ioslot.linePoints.Length > 1)
			{
				Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(ioslot.linePoints[0]);
				for (int j = 1; j < ioslot.linePoints.Length; j++)
				{
					Vector3 vector2 = localToWorldMatrix.MultiplyPoint3x4(ioslot.linePoints[j]);
					Vector3 vector3 = Vector3.Lerp(vector2, vector, 0.5f);
					float num = Vector3.Distance(vector2, vector);
					Quaternion quaternion = Quaternion.LookRotation((vector2 - vector).normalized);
					GameObject gameObject = base.gameManager.CreatePrefab("assets/prefabs/misc/ioentitypreventbuilding.prefab", vector3, quaternion, true);
					gameObject.transform.SetParent(base.transform);
					BoxCollider boxCollider;
					if (gameObject.TryGetComponent<BoxCollider>(out boxCollider))
					{
						boxCollider.size = new Vector3(0.1f, 0.1f, num);
						this.spawnedColliders.Add(boxCollider);
					}
					ColliderInfo_Pipe colliderInfo_Pipe;
					if (gameObject.TryGetComponent<ColliderInfo_Pipe>(out colliderInfo_Pipe))
					{
						colliderInfo_Pipe.OutputSlotIndex = i;
						colliderInfo_Pipe.ParentEntity = this;
					}
					vector = vector2;
				}
			}
		}
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x000703C8 File Offset: 0x0006E5C8
	private void ClearIndustrialPreventBuilding()
	{
		foreach (BoxCollider boxCollider in this.spawnedColliders)
		{
			base.gameManager.Retire(boxCollider.gameObject);
		}
		this.spawnedColliders.Clear();
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00070484 File Offset: 0x0006E684
	[CompilerGenerated]
	internal static int <FindContainerSource>g__GetExistingCount|87_0(IIndustrialStorage storage, ref global::IOEntity.<>c__DisplayClass87_0 A_1)
	{
		int num = 0;
		using (List<global::IOEntity.ContainerInputOutput>.Enumerator enumerator = A_1.found.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Storage == storage)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x0400083F RID: 2111
	[Header("IOEntity")]
	public Transform debugOrigin;

	// Token: 0x04000840 RID: 2112
	public ItemDefinition sourceItem;

	// Token: 0x04000841 RID: 2113
	[NonSerialized]
	public int lastResetIndex;

	// Token: 0x04000842 RID: 2114
	[ServerVar]
	[Help("How many miliseconds to budget for processing io entities per server frame")]
	public static float framebudgetms = 1f;

	// Token: 0x04000843 RID: 2115
	[ServerVar]
	public static float responsetime = 0.1f;

	// Token: 0x04000844 RID: 2116
	[ServerVar]
	public static int backtracking = 8;

	// Token: 0x04000845 RID: 2117
	[ServerVar(Help = "Print out what is taking so long in the IO frame budget")]
	public static bool debugBudget = false;

	// Token: 0x04000846 RID: 2118
	[ServerVar(Help = "Ignore frames with a lower ms than this while debugBudget is active")]
	public static float debugBudgetThreshold = 2f;

	// Token: 0x04000847 RID: 2119
	public const global::BaseEntity.Flags Flag_ShortCircuit = global::BaseEntity.Flags.Reserved7;

	// Token: 0x04000848 RID: 2120
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000849 RID: 2121
	public global::IOEntity.IOSlot[] inputs;

	// Token: 0x0400084A RID: 2122
	public global::IOEntity.IOSlot[] outputs;

	// Token: 0x0400084B RID: 2123
	public global::IOEntity.IOType ioType;

	// Token: 0x0400084C RID: 2124
	public static Queue<global::IOEntity> _processQueue = new Queue<global::IOEntity>();

	// Token: 0x0400084D RID: 2125
	private static List<global::IOEntity.FrameTiming> timings = new List<global::IOEntity.FrameTiming>();

	// Token: 0x0400084E RID: 2126
	private int cachedOutputsUsed;

	// Token: 0x0400084F RID: 2127
	protected int lastPassthroughEnergy;

	// Token: 0x04000850 RID: 2128
	private int lastEnergy;

	// Token: 0x04000851 RID: 2129
	protected int currentEnergy;

	// Token: 0x04000852 RID: 2130
	protected float lastUpdateTime;

	// Token: 0x04000853 RID: 2131
	protected int lastUpdateBlockedFrame;

	// Token: 0x04000854 RID: 2132
	protected bool ensureOutputsUpdated;

	// Token: 0x04000855 RID: 2133
	public const int MaxContainerSourceCount = 32;

	// Token: 0x04000856 RID: 2134
	private List<BoxCollider> spawnedColliders = new List<BoxCollider>();

	// Token: 0x02000BE5 RID: 3045
	public enum IOType
	{
		// Token: 0x040041B6 RID: 16822
		Electric,
		// Token: 0x040041B7 RID: 16823
		Fluidic,
		// Token: 0x040041B8 RID: 16824
		Kinetic,
		// Token: 0x040041B9 RID: 16825
		Generic,
		// Token: 0x040041BA RID: 16826
		Industrial
	}

	// Token: 0x02000BE6 RID: 3046
	[Serializable]
	public class IORef
	{
		// Token: 0x06004DEA RID: 19946 RVA: 0x001A1D54 File Offset: 0x0019FF54
		public void Init()
		{
			if (this.ioEnt != null && !this.entityRef.IsValid(true))
			{
				this.entityRef.Set(this.ioEnt);
			}
			if (this.entityRef.IsValid(true))
			{
				this.ioEnt = this.entityRef.Get(true).GetComponent<global::IOEntity>();
			}
		}

		// Token: 0x06004DEB RID: 19947 RVA: 0x001A1DB3 File Offset: 0x0019FFB3
		public void InitClient()
		{
			if (this.entityRef.IsValid(false) && this.ioEnt == null)
			{
				this.ioEnt = this.entityRef.Get(false).GetComponent<global::IOEntity>();
			}
		}

		// Token: 0x06004DEC RID: 19948 RVA: 0x001A1DE8 File Offset: 0x0019FFE8
		public global::IOEntity Get(bool isServer = true)
		{
			if (this.ioEnt == null && this.entityRef.IsValid(isServer))
			{
				this.ioEnt = this.entityRef.Get(isServer) as global::IOEntity;
			}
			return this.ioEnt;
		}

		// Token: 0x06004DED RID: 19949 RVA: 0x001A1E23 File Offset: 0x001A0023
		public void Clear()
		{
			this.ioEnt = null;
			this.entityRef.Set(null);
		}

		// Token: 0x06004DEE RID: 19950 RVA: 0x001A1E38 File Offset: 0x001A0038
		public void Set(global::IOEntity newIOEnt)
		{
			this.entityRef.Set(newIOEnt);
		}

		// Token: 0x040041BB RID: 16827
		public EntityRef entityRef;

		// Token: 0x040041BC RID: 16828
		public global::IOEntity ioEnt;
	}

	// Token: 0x02000BE7 RID: 3047
	[Serializable]
	public class IOSlot
	{
		// Token: 0x06004DF0 RID: 19952 RVA: 0x001A1E46 File Offset: 0x001A0046
		public void Clear()
		{
			if (this.connectedTo == null)
			{
				this.connectedTo = new global::IOEntity.IORef();
			}
			else
			{
				this.connectedTo.Clear();
			}
			this.connectedToSlot = 0;
			this.linePoints = null;
		}

		// Token: 0x040041BD RID: 16829
		public string niceName;

		// Token: 0x040041BE RID: 16830
		public global::IOEntity.IOType type;

		// Token: 0x040041BF RID: 16831
		public global::IOEntity.IORef connectedTo;

		// Token: 0x040041C0 RID: 16832
		public int connectedToSlot;

		// Token: 0x040041C1 RID: 16833
		public Vector3[] linePoints;

		// Token: 0x040041C2 RID: 16834
		public float[] slackLevels;

		// Token: 0x040041C3 RID: 16835
		public Vector3 worldSpaceLineEndRotation;

		// Token: 0x040041C4 RID: 16836
		public ClientIOLine line;

		// Token: 0x040041C5 RID: 16837
		public Vector3 handlePosition;

		// Token: 0x040041C6 RID: 16838
		public Vector3 handleDirection;

		// Token: 0x040041C7 RID: 16839
		public bool rootConnectionsOnly;

		// Token: 0x040041C8 RID: 16840
		public bool mainPowerSlot;

		// Token: 0x040041C9 RID: 16841
		public WireTool.WireColour wireColour;

		// Token: 0x040041CA RID: 16842
		public float lineThickness;
	}

	// Token: 0x02000BE8 RID: 3048
	private struct FrameTiming
	{
		// Token: 0x040041CB RID: 16843
		public string PrefabName;

		// Token: 0x040041CC RID: 16844
		public float Time;
	}

	// Token: 0x02000BE9 RID: 3049
	public struct ContainerInputOutput
	{
		// Token: 0x040041CD RID: 16845
		public IIndustrialStorage Storage;

		// Token: 0x040041CE RID: 16846
		public int SlotIndex;

		// Token: 0x040041CF RID: 16847
		public int MaxStackSize;

		// Token: 0x040041D0 RID: 16848
		public int ParentStorage;

		// Token: 0x040041D1 RID: 16849
		public int IndustrialSiblingCount;
	}
}
