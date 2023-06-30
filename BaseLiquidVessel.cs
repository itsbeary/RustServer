using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000040 RID: 64
public class BaseLiquidVessel : AttackEntity
{
	// Token: 0x060003F4 RID: 1012 RVA: 0x00031F28 File Offset: 0x00030128
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLiquidVessel.OnRpcMessage", 0))
		{
			if (rpc == 4013436649U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDrink ");
				}
				using (TimeWarning.New("DoDrink", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4013436649U, "DoDrink", this, player))
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
							this.DoDrink(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDrink");
					}
				}
				return true;
			}
			if (rpc == 2781345828U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SendFilling ");
				}
				using (TimeWarning.New("SendFilling", 0))
				{
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
							this.SendFilling(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SendFilling");
					}
				}
				return true;
			}
			if (rpc == 3038767821U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ThrowContents ");
				}
				using (TimeWarning.New("ThrowContents", 0))
				{
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
							this.ThrowContents(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in ThrowContents");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x000322E0 File Offset: 0x000304E0
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.FillCheck), 1f, 1f);
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00032304 File Offset: 0x00030504
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			this.StopFilling();
		}
		if (!this.hasLid)
		{
			this.DoThrow(base.transform.position, Vector3.zero);
			Item item = this.GetItem();
			if (item == null || item.contents == null)
			{
				return;
			}
			item.contents.SetLocked(base.IsDisabled());
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0003236E File Offset: 0x0003056E
	public void SetFilling(bool isFilling)
	{
		base.SetFlag(BaseEntity.Flags.Open, isFilling, false, true);
		if (isFilling)
		{
			this.StartFilling();
		}
		else
		{
			this.StopFilling();
		}
		this.OnSetFilling(isFilling);
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnSetFilling(bool flag)
	{
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00032394 File Offset: 0x00030594
	public void StartFilling()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastFillTime;
		this.StopFilling();
		base.InvokeRepeating(new Action(this.FillCheck), 0f, 0.3f);
		if (num > 1f)
		{
			LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
			if (facingLiquidContainer != null && facingLiquidContainer.GetLiquidItem() != null)
			{
				if (this.fillFromContainer.isValid)
				{
					Effect.server.Run(this.fillFromContainer.resourcePath, facingLiquidContainer.transform.position, Vector3.up, null, false);
				}
				base.ClientRPC(null, "CLIENT_StartFillingSoundsContainer");
			}
			else if (this.CanFillFromWorld())
			{
				if (this.fillFromWorld.isValid)
				{
					Effect.server.Run(this.fillFromWorld.resourcePath, base.GetOwnerPlayer(), 0U, Vector3.zero, Vector3.up, null, false);
				}
				base.ClientRPC(null, "CLIENT_StartFillingSoundsWorld");
			}
		}
		this.lastFillTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x0003247D File Offset: 0x0003067D
	public void StopFilling()
	{
		base.ClientRPC(null, "CLIENT_StopFillingSounds");
		base.CancelInvoke(new Action(this.FillCheck));
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x000324A0 File Offset: 0x000306A0
	public void FillCheck()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		float num = (UnityEngine.Time.realtimeSinceStartup - this.lastFillTime) * this.fillMlPerSec;
		Vector3 vector = ownerPlayer.transform.position - new Vector3(0f, 1f, 0f);
		LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
		if (facingLiquidContainer == null && this.CanFillFromWorld())
		{
			this.AddLiquid(WaterResource.GetAtPoint(vector), Mathf.FloorToInt(num));
		}
		else if (facingLiquidContainer != null && facingLiquidContainer.HasLiquidItem())
		{
			int num2 = Mathf.CeilToInt((1f - this.HeldFraction()) * (float)this.MaxHoldable());
			if (num2 > 0)
			{
				Item liquidItem = facingLiquidContainer.GetLiquidItem();
				int num3 = Mathf.Min(Mathf.CeilToInt(num), Mathf.Min(liquidItem.amount, num2));
				this.AddLiquid(liquidItem.info, num3);
				liquidItem.UseItem(num3);
				facingLiquidContainer.OpenTap(2f);
			}
		}
		this.lastFillTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x000325B0 File Offset: 0x000307B0
	public void LoseWater(int amount)
	{
		if (base.UsingInfiniteAmmoCheat)
		{
			return;
		}
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot != null)
		{
			slot.UseItem(amount);
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000325F0 File Offset: 0x000307F0
	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item item2 = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (item2 == null)
		{
			Item item3 = ItemManager.Create(liquidType, amount, 0UL);
			if (item3 != null)
			{
				item3.MoveToContainer(item.contents, -1, true, false, null, true);
				return;
			}
		}
		else
		{
			int num = Mathf.Clamp(item2.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(item2.info, liquidType);
			if (itemDefinition != item2.info)
			{
				item2.Remove(0f);
				item2 = ItemManager.Create(itemDefinition, num, 0UL);
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
			else
			{
				item2.amount = num;
			}
			item2.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x000326BC File Offset: 0x000308BC
	public int AmountHeld()
	{
		Item item = this.GetItem();
		if (item == null || item.contents == null)
		{
			return 0;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000326F8 File Offset: 0x000308F8
	public float HeldFraction()
	{
		Item item = this.GetItem();
		if (item == null || item.contents == null)
		{
			return 0f;
		}
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0003272C File Offset: 0x0003092C
	public int MaxHoldable()
	{
		Item item = this.GetItem();
		if (item == null || item.contents == null)
		{
			return 1;
		}
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00032764 File Offset: 0x00030964
	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		if (!this.canDrinkFrom)
		{
			return false;
		}
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x000327D5 File Offset: 0x000309D5
	private bool IsWeaponBusy()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextFreeTime;
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x000327E4 File Offset: 0x000309E4
	private void SetBusyFor(float dur)
	{
		this.nextFreeTime = UnityEngine.Time.realtimeSinceStartup + dur;
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x000327F3 File Offset: 0x000309F3
	private void ClearBusy()
	{
		this.nextFreeTime = UnityEngine.Time.realtimeSinceStartup - 1f;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00032808 File Offset: 0x00030A08
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoDrink(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, msg.player))
			{
				component.DoAction(item2, msg.player);
				break;
			}
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x000328C4 File Offset: 0x00030AC4
	[BaseEntity.RPC_Server]
	private void ThrowContents(BaseEntity.RPCMessage msg)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		this.DoThrow(ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 1f, ownerPlayer.estimatedVelocity + ownerPlayer.eyes.BodyForward() * this.throwScale);
		Effect.server.Run(this.ThrowEffect3P.resourcePath, ownerPlayer.transform.position, ownerPlayer.eyes.BodyForward(), ownerPlayer.net.connection, false);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00032960 File Offset: 0x00030B60
	public void DoThrow(Vector3 pos, Vector3 velocity)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot != null && slot.amount > 0)
		{
			Vector3 vector = ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 1f;
			WaterBall waterBall = GameManager.server.CreateEntity(this.thrownWaterObject.resourcePath, vector, Quaternion.identity, true) as WaterBall;
			if (waterBall)
			{
				waterBall.liquidType = slot.info;
				waterBall.waterAmount = slot.amount;
				waterBall.transform.position = vector;
				waterBall.SetVelocity(velocity);
				waterBall.Spawn();
			}
			slot.UseItem(slot.amount);
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00032A54 File Offset: 0x00030C54
	[BaseEntity.RPC_Server]
	private void SendFilling(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		this.SetFilling(flag);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00032A74 File Offset: 0x00030C74
	public bool CanFillFromWorld()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && !ownerPlayer.IsInWaterVolume(base.transform.position) && ownerPlayer.WaterFactor() >= 0.05f;
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00032AB7 File Offset: 0x00030CB7
	public bool CanThrow()
	{
		return this.HeldFraction() > this.minThrowFrac;
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00032AC8 File Offset: 0x00030CC8
	public LiquidContainer GetFacingLiquidContainer()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(ownerPlayer.eyes.HeadRay(), out raycastHit, 2f, 1237003025))
		{
			BaseEntity baseEntity = raycastHit.GetEntity();
			if (baseEntity && !raycastHit.collider.gameObject.CompareTag("Not Player Usable") && !raycastHit.collider.gameObject.CompareTag("Usable Primary"))
			{
				baseEntity = baseEntity.ToServer<BaseEntity>();
				return baseEntity.GetComponent<LiquidContainer>();
			}
		}
		return null;
	}

	// Token: 0x0400030A RID: 778
	[Header("Liquid Vessel")]
	public GameObjectRef thrownWaterObject;

	// Token: 0x0400030B RID: 779
	public GameObjectRef ThrowEffect3P;

	// Token: 0x0400030C RID: 780
	public SoundDefinition throwSound3P;

	// Token: 0x0400030D RID: 781
	public GameObjectRef fillFromContainer;

	// Token: 0x0400030E RID: 782
	public GameObjectRef fillFromWorld;

	// Token: 0x0400030F RID: 783
	public SoundDefinition fillFromContainerStartSoundDef;

	// Token: 0x04000310 RID: 784
	public SoundDefinition fillFromContainerSoundDef;

	// Token: 0x04000311 RID: 785
	public SoundDefinition fillFromWorldStartSoundDef;

	// Token: 0x04000312 RID: 786
	public SoundDefinition fillFromWorldSoundDef;

	// Token: 0x04000313 RID: 787
	public bool hasLid;

	// Token: 0x04000314 RID: 788
	public float throwScale = 10f;

	// Token: 0x04000315 RID: 789
	public bool canDrinkFrom;

	// Token: 0x04000316 RID: 790
	public bool updateVMWater;

	// Token: 0x04000317 RID: 791
	public float minThrowFrac;

	// Token: 0x04000318 RID: 792
	public bool useThrowAnim;

	// Token: 0x04000319 RID: 793
	public float fillMlPerSec = 500f;

	// Token: 0x0400031A RID: 794
	private float lastFillTime;

	// Token: 0x0400031B RID: 795
	private float nextFreeTime;
}
