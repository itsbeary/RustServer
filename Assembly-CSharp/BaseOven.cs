using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000045 RID: 69
public class BaseOven : StorageContainer, ISplashable, IIndustrialStorage
{
	// Token: 0x06000503 RID: 1283 RVA: 0x0003785C File Offset: 0x00035A5C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseOven.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x000379C4 File Offset: 0x00035BC4
	public override void PreInitShared()
	{
		base.PreInitShared();
		this._inputSlotIndex = this.fuelSlots;
		this._outputSlotIndex = this._inputSlotIndex + this.inputSlots;
		this._activeCookingSlot = this._inputSlotIndex;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x000379F7 File Offset: 0x00035BF7
	public override void ServerInit()
	{
		this.inventorySlots = this.fuelSlots + this.inputSlots + this.outputSlots;
		base.ServerInit();
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00037A19 File Offset: 0x00035C19
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.IsOn())
		{
			this.StartCooking();
		}
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00037A2F File Offset: 0x00035C2F
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.baseOven = Facepunch.Pool.Get<ProtoBuf.BaseOven>();
			info.msg.baseOven.cookSpeed = this.GetSmeltingSpeed();
		}
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00037A68 File Offset: 0x00035C68
	public override void OnInventoryFirstCreated(global::ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		if (this.startupContents == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in this.startupContents)
		{
			ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL).MoveToContainer(container, -1, true, false, null, true);
		}
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00037AC0 File Offset: 0x00035CC0
	public override void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		if (item != null)
		{
			ItemModCookable component = item.info.GetComponent<ItemModCookable>();
			if (component != null)
			{
				item.cookTimeLeft = component.cookTime;
			}
			if (item.HasFlag(global::Item.Flag.OnFire))
			{
				item.SetFlag(global::Item.Flag.OnFire, false);
				item.MarkDirty();
			}
			if (item.HasFlag(global::Item.Flag.Cooking))
			{
				item.SetFlag(global::Item.Flag.Cooking, false);
				item.MarkDirty();
			}
		}
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00037B2C File Offset: 0x00035D2C
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		if (!base.ItemFilter(item, targetSlot))
		{
			return false;
		}
		if (targetSlot == -1)
		{
			return false;
		}
		if (this.IsOutputItem(item) && item.GetEntityOwner() != this)
		{
			global::BaseEntity entityOwner = item.GetEntityOwner();
			if (entityOwner != this && entityOwner != null)
			{
				return false;
			}
		}
		global::BaseOven.MinMax? allowedSlots = this.GetAllowedSlots(item);
		return allowedSlots != null && targetSlot >= allowedSlots.Value.Min && targetSlot <= allowedSlots.Value.Max;
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00037BB4 File Offset: 0x00035DB4
	private global::BaseOven.MinMax? GetAllowedSlots(global::Item item)
	{
		int num = 0;
		int num2;
		if (this.IsBurnableItem(item))
		{
			num2 = this.fuelSlots;
		}
		else if (this.IsOutputItem(item))
		{
			num = this._outputSlotIndex;
			num2 = num + this.outputSlots;
		}
		else
		{
			if (!this.IsMaterialInput(item))
			{
				return null;
			}
			num = this._inputSlotIndex;
			num2 = num + this.inputSlots;
		}
		return new global::BaseOven.MinMax?(new global::BaseOven.MinMax(num, num2 - 1));
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00037C25 File Offset: 0x00035E25
	public global::BaseOven.MinMax GetOutputSlotRange()
	{
		return new global::BaseOven.MinMax(this._outputSlotIndex, this._outputSlotIndex + this.outputSlots - 1);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00037C44 File Offset: 0x00035E44
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		global::BaseOven.MinMax? allowedSlots = this.GetAllowedSlots(item);
		if (allowedSlots == null)
		{
			return -1;
		}
		for (int i = allowedSlots.Value.Min; i <= allowedSlots.Value.Max; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot == null || (slot.CanStack(item) && slot.amount < slot.MaxStackable()))
			{
				return i;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00037CB7 File Offset: 0x00035EB7
	public void OvenFull()
	{
		this.StopCooking();
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0000441C File Offset: 0x0000261C
	private int GetFuelRate()
	{
		return 1;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0000441C File Offset: 0x0000261C
	private int GetCharcoalRate()
	{
		return 1;
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000511 RID: 1297 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool CanRunWithNoFuel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00037CC0 File Offset: 0x00035EC0
	public void Cook()
	{
		global::Item item = this.FindBurnable();
		if (item == null && !this.CanRunWithNoFuel)
		{
			this.StopCooking();
			return;
		}
		foreach (global::Item item2 in base.inventory.itemList)
		{
			if (item2.position >= this._inputSlotIndex && item2.position < this._inputSlotIndex + this.inputSlots && !item2.HasFlag(global::Item.Flag.Cooking))
			{
				item2.SetFlag(global::Item.Flag.Cooking, true);
				item2.MarkDirty();
			}
		}
		this.IncreaseCookTime(0.5f * this.GetSmeltingSpeed());
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.FireMod);
		if (slot)
		{
			slot.SendMessage("Cook", 0.5f, SendMessageOptions.DontRequireReceiver);
		}
		if (item != null)
		{
			ItemModBurnable component = item.info.GetComponent<ItemModBurnable>();
			item.fuel -= 0.5f * (this.cookingTemperature / 200f);
			if (!item.HasFlag(global::Item.Flag.OnFire))
			{
				item.SetFlag(global::Item.Flag.OnFire, true);
				item.MarkDirty();
			}
			if (item.fuel <= 0f)
			{
				this.ConsumeFuel(item, component);
			}
		}
		this.OnCooked();
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnCooked()
	{
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x00037E00 File Offset: 0x00036000
	private void ConsumeFuel(global::Item fuel, ItemModBurnable burnable)
	{
		if (this.allowByproductCreation && burnable.byproductItem != null && UnityEngine.Random.Range(0f, 1f) > burnable.byproductChance)
		{
			global::Item item = ItemManager.Create(burnable.byproductItem, burnable.byproductAmount * this.GetCharcoalRate(), 0UL);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				this.OvenFull();
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
			}
		}
		if (fuel.amount <= this.GetFuelRate())
		{
			fuel.Remove(0f);
			return;
		}
		int fuelRate = this.GetFuelRate();
		fuel.UseItem(fuelRate);
		Analytics.Azure.AddPendingItems(this, fuel.info.shortname, fuelRate, "smelt", true, false);
		fuel.fuel = burnable.fuelAmount;
		fuel.MarkDirty();
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x00037EE8 File Offset: 0x000360E8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	protected virtual void SVSwitch(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			this.StartCooking();
			return;
		}
		this.StopCooking();
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x00037F31 File Offset: 0x00036131
	public float GetTemperature(int slot)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.On))
		{
			return 15f;
		}
		return this.cookingTemperature;
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00037F48 File Offset: 0x00036148
	public void UpdateAttachmentTemperature()
	{
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.FireMod);
		if (slot)
		{
			slot.SendMessage("ParentTemperatureUpdate", base.inventory.temperature, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x00037F84 File Offset: 0x00036184
	public virtual void StartCooking()
	{
		if (this.FindBurnable() == null && !this.CanRunWithNoFuel)
		{
			return;
		}
		base.inventory.temperature = this.cookingTemperature;
		this.UpdateAttachmentTemperature();
		base.InvokeRepeating(new Action(this.Cook), 0.5f, 0.5f);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x00037FE0 File Offset: 0x000361E0
	public virtual void StopCooking()
	{
		this.UpdateAttachmentTemperature();
		if (base.inventory != null)
		{
			base.inventory.temperature = 15f;
			foreach (global::Item item in base.inventory.itemList)
			{
				if (item.HasFlag(global::Item.Flag.OnFire))
				{
					item.SetFlag(global::Item.Flag.OnFire, false);
					item.MarkDirty();
				}
				else if (item.HasFlag(global::Item.Flag.Cooking))
				{
					item.SetFlag(global::Item.Flag.Cooking, false);
					item.MarkDirty();
				}
			}
		}
		base.CancelInvoke(new Action(this.Cook));
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x000380A0 File Offset: 0x000362A0
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && base.IsOn() && this.disabledBySplash;
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x000380BA File Offset: 0x000362BA
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.StopCooking();
		return Mathf.Min(200, amount);
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x000380D0 File Offset: 0x000362D0
	public global::Item FindBurnable()
	{
		if (base.inventory == null)
		{
			return null;
		}
		foreach (global::Item item in base.inventory.itemList)
		{
			if (this.IsBurnableItem(item))
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0003813C File Offset: 0x0003633C
	private void IncreaseCookTime(float amount)
	{
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		foreach (global::Item item in base.inventory.itemList)
		{
			if (item.HasFlag(global::Item.Flag.Cooking))
			{
				list.Add(item);
			}
		}
		float num = amount / (float)list.Count;
		foreach (global::Item item2 in list)
		{
			item2.OnCycle(num);
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600051E RID: 1310 RVA: 0x000381F0 File Offset: 0x000363F0
	public global::ItemContainer Container
	{
		get
		{
			return base.inventory;
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x000381F8 File Offset: 0x000363F8
	public Vector2i InputSlotRange(int slotIndex)
	{
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.LargeFurnace)
		{
			return new Vector2i(0, 6);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.OilRefinery)
		{
			return new Vector2i(0, 1);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.ElectricFurnace)
		{
			return new Vector2i(0, 1);
		}
		return new Vector2i(0, 2);
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x00038234 File Offset: 0x00036434
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.LargeFurnace)
		{
			return new Vector2i(7, 16);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.OilRefinery)
		{
			return new Vector2i(2, 4);
		}
		if (this.IndustrialMode == global::BaseOven.IndustrialSlotMode.ElectricFurnace)
		{
			return new Vector2i(2, 4);
		}
		return new Vector2i(3, 5);
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000523 RID: 1315 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00038271 File Offset: 0x00036471
	public float GetSmeltingSpeed()
	{
		if (base.isServer)
		{
			return (float)this.smeltSpeed;
		}
		throw new Exception("No way it should be able to get here?");
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000525 RID: 1317 RVA: 0x00038290 File Offset: 0x00036490
	private float cookingTemperature
	{
		get
		{
			switch (this.temperature)
			{
			case global::BaseOven.TemperatureType.Warming:
				return 50f;
			case global::BaseOven.TemperatureType.Cooking:
				return 200f;
			case global::BaseOven.TemperatureType.Smelting:
				return 1000f;
			case global::BaseOven.TemperatureType.Fractioning:
				return 1500f;
			default:
				return 15f;
			}
		}
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x000382DB File Offset: 0x000364DB
	private bool IsBurnableItem(global::Item item)
	{
		return item.info.GetComponent<ItemModBurnable>() && (this.fuelType == null || item.info == this.fuelType);
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00038314 File Offset: 0x00036514
	private bool IsBurnableByproduct(global::Item item)
	{
		ItemDefinition itemDefinition = this.fuelType;
		ItemModBurnable itemModBurnable = ((itemDefinition != null) ? itemDefinition.GetComponent<ItemModBurnable>() : null);
		return !(itemModBurnable == null) && item.info == itemModBurnable.byproductItem;
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00038350 File Offset: 0x00036550
	private bool IsMaterialInput(global::Item item)
	{
		ItemModCookable component = item.info.GetComponent<ItemModCookable>();
		return !(component == null) && (float)component.lowTemp <= this.cookingTemperature && (float)component.highTemp >= this.cookingTemperature;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00038394 File Offset: 0x00036594
	private bool IsMaterialOutput(global::Item item)
	{
		if (global::BaseOven._materialOutputCache == null)
		{
			this.BuildMaterialOutputCache();
		}
		HashSet<ItemDefinition> hashSet;
		if (!global::BaseOven._materialOutputCache.TryGetValue(this.cookingTemperature, out hashSet))
		{
			Debug.LogError("Can't find smeltable item list for oven");
			return true;
		}
		return hashSet.Contains(item.info);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x000383DA File Offset: 0x000365DA
	private bool IsOutputItem(global::Item item)
	{
		return this.IsMaterialOutput(item) || this.IsBurnableByproduct(item);
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x000383F0 File Offset: 0x000365F0
	private void BuildMaterialOutputCache()
	{
		global::BaseOven._materialOutputCache = new Dictionary<float, HashSet<ItemDefinition>>();
		foreach (float num in (from x in GameManager.server.preProcessed.prefabList.Values
			select x.GetComponent<global::BaseOven>() into x
			where x != null
			select x.cookingTemperature).Distinct<float>().ToArray<float>())
		{
			HashSet<ItemDefinition> hashSet = new HashSet<ItemDefinition>();
			global::BaseOven._materialOutputCache[num] = hashSet;
			foreach (ItemDefinition itemDefinition in ItemManager.itemList)
			{
				ItemModCookable component = itemDefinition.GetComponent<ItemModCookable>();
				if (!(component == null) && component.CanBeCookedByAtTemperature(num))
				{
					hashSet.Add(component.becomeOnCooked);
				}
			}
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00038520 File Offset: 0x00036720
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return (this.canModFire && slot == global::BaseEntity.Slot.FireMod) || base.HasSlot(slot);
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00038537 File Offset: 0x00036737
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && this.CanPickupOven();
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0003854A File Offset: 0x0003674A
	protected virtual bool CanPickupOven()
	{
		return this.children.Count == 0;
	}

	// Token: 0x0400039A RID: 922
	private static Dictionary<float, HashSet<ItemDefinition>> _materialOutputCache;

	// Token: 0x0400039B RID: 923
	public global::BaseOven.TemperatureType temperature;

	// Token: 0x0400039C RID: 924
	public global::BaseEntity.Menu.Option switchOnMenu;

	// Token: 0x0400039D RID: 925
	public global::BaseEntity.Menu.Option switchOffMenu;

	// Token: 0x0400039E RID: 926
	public ItemAmount[] startupContents;

	// Token: 0x0400039F RID: 927
	public bool allowByproductCreation = true;

	// Token: 0x040003A0 RID: 928
	public ItemDefinition fuelType;

	// Token: 0x040003A1 RID: 929
	public bool canModFire;

	// Token: 0x040003A2 RID: 930
	public bool disabledBySplash = true;

	// Token: 0x040003A3 RID: 931
	public int smeltSpeed = 1;

	// Token: 0x040003A4 RID: 932
	public int fuelSlots = 1;

	// Token: 0x040003A5 RID: 933
	public int inputSlots = 1;

	// Token: 0x040003A6 RID: 934
	public int outputSlots = 1;

	// Token: 0x040003A7 RID: 935
	public global::BaseOven.IndustrialSlotMode IndustrialMode;

	// Token: 0x040003A8 RID: 936
	private int _activeCookingSlot = -1;

	// Token: 0x040003A9 RID: 937
	private int _inputSlotIndex;

	// Token: 0x040003AA RID: 938
	private int _outputSlotIndex;

	// Token: 0x040003AB RID: 939
	private const float UpdateRate = 0.5f;

	// Token: 0x02000BA7 RID: 2983
	public enum TemperatureType
	{
		// Token: 0x040040A9 RID: 16553
		Normal,
		// Token: 0x040040AA RID: 16554
		Warming,
		// Token: 0x040040AB RID: 16555
		Cooking,
		// Token: 0x040040AC RID: 16556
		Smelting,
		// Token: 0x040040AD RID: 16557
		Fractioning
	}

	// Token: 0x02000BA8 RID: 2984
	public enum IndustrialSlotMode
	{
		// Token: 0x040040AF RID: 16559
		Furnace,
		// Token: 0x040040B0 RID: 16560
		LargeFurnace,
		// Token: 0x040040B1 RID: 16561
		OilRefinery,
		// Token: 0x040040B2 RID: 16562
		ElectricFurnace
	}

	// Token: 0x02000BA9 RID: 2985
	public struct MinMax
	{
		// Token: 0x06004D55 RID: 19797 RVA: 0x001A0A59 File Offset: 0x0019EC59
		public MinMax(int min, int max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x040040B3 RID: 16563
		public int Min;

		// Token: 0x040040B4 RID: 16564
		public int Max;
	}

	// Token: 0x02000BAA RID: 2986
	private enum OvenItemType
	{
		// Token: 0x040040B6 RID: 16566
		Burnable,
		// Token: 0x040040B7 RID: 16567
		Byproduct,
		// Token: 0x040040B8 RID: 16568
		MaterialInput,
		// Token: 0x040040B9 RID: 16569
		MaterialOutput
	}
}
