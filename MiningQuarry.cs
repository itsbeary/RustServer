using System;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class MiningQuarry : BaseResourceExtractor
{
	// Token: 0x0600233B RID: 9019 RVA: 0x0002A700 File Offset: 0x00028900
	public bool IsEngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000E1588 File Offset: 0x000DF788
	private void SetOn(bool isOn)
	{
		base.SetFlag(global::BaseEntity.Flags.On, isOn, false, true);
		this.engineSwitchPrefab.instance.SetFlag(global::BaseEntity.Flags.On, isOn, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.engineSwitchPrefab.instance.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (isOn)
		{
			base.InvokeRepeating(new Action(this.ProcessResources), this.processRate, this.processRate);
			return;
		}
		base.CancelInvoke(new Action(this.ProcessResources));
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000E15FF File Offset: 0x000DF7FF
	public void EngineSwitch(bool isOn)
	{
		if (isOn && this.FuelCheck())
		{
			this.SetOn(true);
			return;
		}
		this.SetOn(false);
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000E161C File Offset: 0x000DF81C
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.isStatic)
		{
			this.UpdateStaticDeposit();
		}
		else
		{
			ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(base.transform.position);
			this._linkedDeposit = orCreate;
		}
		this.SpawnChildEntities();
		this.engineSwitchPrefab.instance.SetFlag(global::BaseEntity.Flags.On, base.HasFlag(global::BaseEntity.Flags.On), false, true);
		if (base.isServer)
		{
			global::ItemContainer inventory = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory;
			inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		}
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000E16B5 File Offset: 0x000DF8B5
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		return item.info.shortname == "diesel_barrel";
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000E16CC File Offset: 0x000DF8CC
	public void UpdateStaticDeposit()
	{
		if (!this.isStatic)
		{
			return;
		}
		if (this._linkedDeposit == null)
		{
			this._linkedDeposit = new ResourceDepositManager.ResourceDeposit();
		}
		else
		{
			this._linkedDeposit._resources.Clear();
		}
		if (this.staticType == global::MiningQuarry.QuarryType.None)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 7.5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 75f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.Basic)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.Sulfur)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.HQM)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 20f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("crude.oil"), 1f, 1000, 16.666666f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("lowgradefuel"), 1f, 1000, 5.882353f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000E18B2 File Offset: 0x000DFAB2
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.EngineSwitch(base.HasFlag(global::BaseEntity.Flags.On));
		this.UpdateStaticDeposit();
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000E18CD File Offset: 0x000DFACD
	public void SpawnChildEntities()
	{
		this.engineSwitchPrefab.DoSpawn(this);
		this.hopperPrefab.DoSpawn(this);
		this.fuelStoragePrefab.DoSpawn(this);
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000E18F4 File Offset: 0x000DFAF4
	public void ProcessResources()
	{
		if (this._linkedDeposit == null)
		{
			return;
		}
		if (this.hopperPrefab.instance == null)
		{
			return;
		}
		if (!this.FuelCheck())
		{
			this.SetOn(false);
		}
		float num = Mathf.Min(this.workToAdd, this.pendingWork);
		this.pendingWork -= num;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry in this._linkedDeposit._resources)
		{
			if ((this.canExtractLiquid || !resourceDepositEntry.isLiquid) && (this.canExtractSolid || resourceDepositEntry.isLiquid))
			{
				float workNeeded = resourceDepositEntry.workNeeded;
				int num2 = Mathf.FloorToInt(resourceDepositEntry.workDone / workNeeded);
				resourceDepositEntry.workDone += num;
				int num3 = Mathf.FloorToInt(resourceDepositEntry.workDone / workNeeded);
				if (resourceDepositEntry.workDone > workNeeded)
				{
					resourceDepositEntry.workDone %= workNeeded;
				}
				if (num2 != num3)
				{
					int num4 = num3 - num2;
					global::Item item = ItemManager.Create(resourceDepositEntry.type, num4, 0UL);
					Analytics.Azure.OnQuarryItem(Analytics.Azure.ResourceMode.Produced, item.info.shortname, item.amount, this);
					if (!item.MoveToContainer(this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory, -1, true, false, null, true))
					{
						item.Remove(0f);
						this.SetOn(false);
					}
				}
			}
		}
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000E1A78 File Offset: 0x000DFC78
	public bool FuelCheck()
	{
		if (this.pendingWork > 0f)
		{
			return true;
		}
		global::Item item = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.FindItemsByItemName("diesel_barrel");
		if (item != null && item.amount >= 1)
		{
			this.pendingWork += this.workPerFuel;
			Analytics.Azure.OnQuarryItem(Analytics.Azure.ResourceMode.Consumed, item.info.shortname, 1, this);
			item.UseItem(1);
			return true;
		}
		return false;
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000E1AF0 File Offset: 0x000DFCF0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot save mining quary because children were null");
				return;
			}
			info.msg.miningQuarry = Pool.Get<ProtoBuf.MiningQuarry>();
			info.msg.miningQuarry.extractor = Pool.Get<ResourceExtractor>();
			info.msg.miningQuarry.extractor.fuelContents = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.extractor.outputContents = this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.staticType = (int)this.staticType;
		}
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000E1BDC File Offset: 0x000DFDDC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.miningQuarry != null)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot load mining quary because children were null");
				return;
			}
			this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.fuelContents);
			this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.outputContents);
			this.staticType = (global::MiningQuarry.QuarryType)info.msg.miningQuarry.staticType;
		}
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Update()
	{
	}

	// Token: 0x04001B1B RID: 6939
	public Animator beltAnimator;

	// Token: 0x04001B1C RID: 6940
	public Renderer beltScrollRenderer;

	// Token: 0x04001B1D RID: 6941
	public int scrollMatIndex = 3;

	// Token: 0x04001B1E RID: 6942
	public SoundPlayer[] onSounds;

	// Token: 0x04001B1F RID: 6943
	public float processRate = 5f;

	// Token: 0x04001B20 RID: 6944
	public float workToAdd = 15f;

	// Token: 0x04001B21 RID: 6945
	public float workPerFuel = 1000f;

	// Token: 0x04001B22 RID: 6946
	public float pendingWork;

	// Token: 0x04001B23 RID: 6947
	public GameObjectRef bucketDropEffect;

	// Token: 0x04001B24 RID: 6948
	public GameObject bucketDropTransform;

	// Token: 0x04001B25 RID: 6949
	public global::MiningQuarry.ChildPrefab engineSwitchPrefab;

	// Token: 0x04001B26 RID: 6950
	public global::MiningQuarry.ChildPrefab hopperPrefab;

	// Token: 0x04001B27 RID: 6951
	public global::MiningQuarry.ChildPrefab fuelStoragePrefab;

	// Token: 0x04001B28 RID: 6952
	public global::MiningQuarry.QuarryType staticType;

	// Token: 0x04001B29 RID: 6953
	public bool isStatic;

	// Token: 0x04001B2A RID: 6954
	private ResourceDepositManager.ResourceDeposit _linkedDeposit;

	// Token: 0x02000CEE RID: 3310
	[Serializable]
	public enum QuarryType
	{
		// Token: 0x040045EA RID: 17898
		None,
		// Token: 0x040045EB RID: 17899
		Basic,
		// Token: 0x040045EC RID: 17900
		Sulfur,
		// Token: 0x040045ED RID: 17901
		HQM
	}

	// Token: 0x02000CEF RID: 3311
	[Serializable]
	public class ChildPrefab
	{
		// Token: 0x06005018 RID: 20504 RVA: 0x001A8164 File Offset: 0x001A6364
		public void DoSpawn(global::MiningQuarry owner)
		{
			if (!this.prefabToSpawn.isValid)
			{
				return;
			}
			this.instance = GameManager.server.CreateEntity(this.prefabToSpawn.resourcePath, this.origin.transform.localPosition, this.origin.transform.localRotation, true);
			this.instance.SetParent(owner, false, false);
			this.instance.Spawn();
		}

		// Token: 0x040045EE RID: 17902
		public GameObjectRef prefabToSpawn;

		// Token: 0x040045EF RID: 17903
		public GameObject origin;

		// Token: 0x040045F0 RID: 17904
		public global::BaseEntity instance;
	}
}
