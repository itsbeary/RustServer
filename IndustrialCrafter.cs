using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000086 RID: 134
public class IndustrialCrafter : IndustrialEntity, IItemContainerEntity, IIdealSlotEntity, ILootableEntity, LootPanel.IHasLootPanel, IContainerSounds, IIndustrialStorage
{
	// Token: 0x06000C88 RID: 3208 RVA: 0x0006D264 File Offset: 0x0006B464
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IndustrialCrafter.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
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
							this.RPC_OpenLoot(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SvSwitch ");
				}
				using (TimeWarning.New("SvSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4167839872U, "SvSwitch", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SvSwitch", this, player, 3f))
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
							this.SvSwitch(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SvSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000C89 RID: 3209 RVA: 0x0006D580 File Offset: 0x0006B780
	// (set) Token: 0x06000C8A RID: 3210 RVA: 0x0006D588 File Offset: 0x0006B788
	public TimeUntilWithDuration jobFinishes { get; private set; }

	// Token: 0x06000C8B RID: 3211 RVA: 0x0006D594 File Offset: 0x0006B794
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(global::BaseEntity.Flags.On);
		if (old.HasFlag(global::BaseEntity.Flags.On) != flag && base.isServer)
		{
			float industrialCrafterFrequency = ConVar.Server.industrialCrafterFrequency;
			if (flag && industrialCrafterFrequency > 0f)
			{
				base.InvokeRandomized(new Action(this.CheckCraft), industrialCrafterFrequency, industrialCrafterFrequency, industrialCrafterFrequency * 0.5f);
				return;
			}
			base.CancelInvoke(new Action(this.CheckCraft));
		}
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000C8C RID: 3212 RVA: 0x0006D616 File Offset: 0x0006B816
	// (set) Token: 0x06000C8D RID: 3213 RVA: 0x0006D61E File Offset: 0x0006B81E
	public global::ItemContainer inventory { get; set; }

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0005DDD5 File Offset: 0x0005BFD5
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000C8F RID: 3215 RVA: 0x0000441C File Offset: 0x0000261C
	public bool DropsLoot
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000C90 RID: 3216 RVA: 0x00029EBC File Offset: 0x000280BC
	public float DestroyLootPercent
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000C91 RID: 3217 RVA: 0x0006D627 File Offset: 0x0006B827
	public bool DropFloats { get; }

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0006D62F File Offset: 0x0006B82F
	// (set) Token: 0x06000C93 RID: 3219 RVA: 0x0006D637 File Offset: 0x0006B837
	public ulong LastLootedBy { get; set; }

	// Token: 0x06000C94 RID: 3220 RVA: 0x0006D640 File Offset: 0x0006B840
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.DropItems((info != null) ? info.Initiator : null);
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0005DFC5 File Offset: 0x0005C1C5
	public void DropItems(global::BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x000063A5 File Offset: 0x000045A5
	public void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0006D65C File Offset: 0x0006B85C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0006D698 File Offset: 0x0006B898
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (this.NeedsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		if (this.OnlyOneUser && base.IsOpen())
		{
			player.ChatMessage("Already in use");
			return false;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.LootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0005E0C2 File Offset: 0x0005C2C2
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0006D735 File Offset: 0x0006B935
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.inventory == null)
		{
			this.CreateInventory(true);
		}
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x0006D74C File Offset: 0x0006B94C
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.CanAcceptItem);
		this.inventory.ServerInitialize(null, 12);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0006D7A3 File Offset: 0x0006B9A3
	private bool CanAcceptItem(global::Item item, int index)
	{
		return index < 0 || index > 3 || item.IsBlueprint();
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0006BD66 File Offset: 0x00069F66
	private void CheckCraft()
	{
		global::IndustrialEntity.Queue.Add(this);
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0006D7B8 File Offset: 0x0006B9B8
	private global::Item GetTargetBlueprint(int index)
	{
		if (this.inventory == null)
		{
			return null;
		}
		if (index < 0 || index > 3)
		{
			return null;
		}
		global::Item slot = this.inventory.GetSlot(index);
		if (slot == null || !slot.IsBlueprint())
		{
			return null;
		}
		return slot;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0006D7F4 File Offset: 0x0006B9F4
	protected override void RunJob()
	{
		base.RunJob();
		if (ConVar.Server.industrialCrafterFrequency <= 0f)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved1) || this.currentlyCrafting != null)
		{
			return;
		}
		for (int i = 0; i <= 3; i++)
		{
			global::Item targetBlueprint = this.GetTargetBlueprint(i);
			if (targetBlueprint != null && !(this.GetWorkbench() == null) && this.GetWorkbench().Workbenchlevel >= targetBlueprint.blueprintTargetDef.Blueprint.workbenchLevelRequired)
			{
				ItemBlueprint blueprint = targetBlueprint.blueprintTargetDef.Blueprint;
				bool flag = true;
				foreach (ItemAmount itemAmount in blueprint.ingredients)
				{
					if ((float)this.GetInputAmount(itemAmount.itemDef) < itemAmount.amount)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					flag = false;
					for (int j = 8; j <= 11; j++)
					{
						global::Item slot = this.inventory.GetSlot(j);
						if (slot == null || (slot.info == targetBlueprint.blueprintTargetDef && slot.amount + blueprint.amountToCreate <= slot.MaxStackable()))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
						foreach (ItemAmount itemAmount2 in blueprint.ingredients)
						{
							this.ConsumeInputIngredient(itemAmount2);
						}
						this.currentlyCrafting = targetBlueprint.blueprintTargetDef;
						this.currentlyCraftingAmount = blueprint.amountToCreate;
						float time = blueprint.time;
						base.Invoke(new Action(this.CompleteCraft), time);
						this.jobFinishes = time;
						base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
						base.ClientRPC<float, float>(null, "ClientUpdateCraftTimeRemaining", this.jobFinishes, this.jobFinishes.Duration);
						return;
					}
					base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				}
			}
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0006DA24 File Offset: 0x0006BC24
	private void CompleteCraft()
	{
		bool flag = false;
		for (int i = 8; i <= 11; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot == null)
			{
				global::Item item = ItemManager.Create(this.currentlyCrafting, this.currentlyCraftingAmount, 0UL);
				item.position = i;
				this.inventory.Insert(item);
				flag = true;
				break;
			}
			if (slot.info == this.currentlyCrafting && slot.amount + this.currentlyCraftingAmount <= slot.MaxStackable())
			{
				slot.amount += this.currentlyCraftingAmount;
				slot.MarkDirty();
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			ItemManager.Create(this.currentlyCrafting, this.currentlyCraftingAmount, 0UL).Drop(base.transform.position + base.transform.forward * 0.5f, Vector3.zero, default(Quaternion));
		}
		this.currentlyCrafting = null;
		this.currentlyCraftingAmount = 0;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0006DB34 File Offset: 0x0006BD34
	private int GetInputAmount(ItemDefinition def)
	{
		if (def == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 4; i <= 7; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null && def == slot.info)
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0006DB84 File Offset: 0x0006BD84
	private bool ConsumeInputIngredient(ItemAmount am)
	{
		if (am.itemDef == null)
		{
			return false;
		}
		float num = am.amount;
		for (int i = 4; i <= 7; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null && am.itemDef == slot.info)
			{
				float num2 = Mathf.Min(num, (float)slot.amount);
				slot.UseItem((int)num2);
				num -= num2;
				if (num2 <= 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0006DBFC File Offset: 0x0006BDFC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.currentlyCrafting != null)
			{
				info.msg.industrialCrafter = Facepunch.Pool.Get<ProtoBuf.IndustrialCrafter>();
				info.msg.industrialCrafter.currentlyCrafting = this.currentlyCrafting.itemid;
				info.msg.industrialCrafter.currentlyCraftingAmount = this.currentlyCraftingAmount;
			}
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
			}
		}
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0006DCA0 File Offset: 0x0006BEA0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null && this.inventory != null)
		{
			this.inventory.Load(info.msg.storageBox.contents);
			this.inventory.capacity = 12;
		}
		if (base.isServer && info.fromDisk && info.msg.industrialCrafter != null)
		{
			this.currentlyCrafting = ItemManager.FindItemDefinition(info.msg.industrialCrafter.currentlyCrafting);
			this.currentlyCraftingAmount = info.msg.industrialCrafter.currentlyCraftingAmount;
			this.CompleteCraft();
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000CA6 RID: 3238 RVA: 0x0006DD45 File Offset: 0x0006BF45
	public global::ItemContainer Container
	{
		get
		{
			return this.inventory;
		}
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0006DD4D File Offset: 0x0006BF4D
	public Vector2i InputSlotRange(int slotIndex)
	{
		if (slotIndex == 3)
		{
			return new Vector2i(0, 3);
		}
		return new Vector2i(4, 7);
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x0006DD62 File Offset: 0x0006BF62
	public Vector2i OutputSlotRange(int slotIndex)
	{
		if (slotIndex == 1)
		{
			return new Vector2i(0, 3);
		}
		return new Vector2i(8, 11);
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000CAB RID: 3243 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0006DD78 File Offset: 0x0006BF78
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 1)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount() && inputAmount > 0, false, true);
			this.currentEnergy = inputAmount;
			this.ensureOutputsUpdated = true;
			this.MarkDirty();
		}
		if (inputSlot == 1 && inputAmount <= 0 && base.IsOn())
		{
			this.SetSwitch(false);
		}
		if (inputSlot == 2)
		{
			if (base.IsOn() && inputAmount == 0)
			{
				this.SetSwitch(false);
			}
			else if (!base.IsOn() && inputAmount > 0 && base.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				this.SetSwitch(true);
			}
		}
		if (inputSlot == 4 && inputAmount > 0 && base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 5 && inputAmount > 0 && base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			this.SetSwitch(false);
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0006DE4C File Offset: 0x0006C04C
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, false);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, false);
		if (!wantsOn)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
		}
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00062BCC File Offset: 0x00060DCC
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0006DEB0 File Offset: 0x0006C0B0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void SvSwitch(global::BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0006DEC1 File Offset: 0x0006C0C1
	public override bool CanPickup(global::BasePlayer player)
	{
		if (base.isServer)
		{
			return this.inventory != null && this.inventory.IsEmpty() && base.CanPickup(player);
		}
		return base.CanPickup(player);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0004485D File Offset: 0x00042A5D
	public int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0006DEF4 File Offset: 0x0006C0F4
	public ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000CB4 RID: 3252 RVA: 0x0006DF0A File Offset: 0x0006C10A
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return new Translate.Phrase("industrial.crafter.loot", "Industrial Crafter");
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x0006DF1B File Offset: 0x0006C11B
	public SoundDefinition OpenSound
	{
		get
		{
			return this.ContainerOpenSound;
		}
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000CB6 RID: 3254 RVA: 0x0006DF23 File Offset: 0x0006C123
	public SoundDefinition CloseSound
	{
		get
		{
			return this.ContainerCloseSound;
		}
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0006DF2B File Offset: 0x0006C12B
	public Workbench GetWorkbench()
	{
		return base.GetParentEntity() as Workbench;
	}

	// Token: 0x04000815 RID: 2069
	public string LootPanelName = "generic";

	// Token: 0x04000816 RID: 2070
	public bool NeedsBuildingPrivilegeToUse;

	// Token: 0x04000817 RID: 2071
	public bool OnlyOneUser;

	// Token: 0x04000818 RID: 2072
	public SoundDefinition ContainerOpenSound;

	// Token: 0x04000819 RID: 2073
	public SoundDefinition ContainerCloseSound;

	// Token: 0x0400081A RID: 2074
	public AnimationCurve MaterialOffsetCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400081B RID: 2075
	public const global::BaseEntity.Flags Crafting = global::BaseEntity.Flags.Reserved1;

	// Token: 0x0400081C RID: 2076
	public const global::BaseEntity.Flags FullOutput = global::BaseEntity.Flags.Reserved2;

	// Token: 0x0400081D RID: 2077
	public Renderer[] MeshRenderers;

	// Token: 0x0400081E RID: 2078
	public ParticleSystemContainer JobCompleteFx;

	// Token: 0x0400081F RID: 2079
	public SoundDefinition JobCompleteSoundDef;

	// Token: 0x04000821 RID: 2081
	public const int BlueprintSlotStart = 0;

	// Token: 0x04000822 RID: 2082
	public const int BlueprintSlotEnd = 3;

	// Token: 0x04000826 RID: 2086
	private ItemDefinition currentlyCrafting;

	// Token: 0x04000827 RID: 2087
	private int currentlyCraftingAmount;

	// Token: 0x04000828 RID: 2088
	private const int StorageSize = 12;

	// Token: 0x04000829 RID: 2089
	private const int InputSlotStart = 4;

	// Token: 0x0400082A RID: 2090
	private const int InputSlotEnd = 7;

	// Token: 0x0400082B RID: 2091
	private const int OutputSlotStart = 8;

	// Token: 0x0400082C RID: 2092
	private const int OutputSlotEnd = 11;
}
