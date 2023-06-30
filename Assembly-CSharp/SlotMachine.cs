using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CE RID: 206
public class SlotMachine : BaseMountable
{
	// Token: 0x0600126C RID: 4716 RVA: 0x00094BE8 File Offset: 0x00092DE8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SlotMachine.OnRpcMessage", 0))
		{
			if (rpc == 1251063754U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Deposit ");
				}
				using (TimeWarning.New("RPC_Deposit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1251063754U, "RPC_Deposit", this, player, 3f))
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
							this.RPC_Deposit(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Deposit");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455840454U, "RPC_Spin", this, player, 3f))
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
							this.RPC_Spin(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
			if (rpc == 3942337446U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestMultiplierChange ");
				}
				using (TimeWarning.New("Server_RequestMultiplierChange", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3942337446U, "Server_RequestMultiplierChange", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3942337446U, "Server_RequestMultiplierChange", this, player, 3f))
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
							this.Server_RequestMultiplierChange(rpcmessage3);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in Server_RequestMultiplierChange");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x0600126D RID: 4717 RVA: 0x0000564C File Offset: 0x0000384C
	private bool IsSpinning
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x0600126E RID: 4718 RVA: 0x00095060 File Offset: 0x00093260
	// (set) Token: 0x0600126F RID: 4719 RVA: 0x00095068 File Offset: 0x00093268
	public int CurrentMultiplier { get; private set; } = 1;

	// Token: 0x06001270 RID: 4720 RVA: 0x00095074 File Offset: 0x00093274
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.slotMachine = Facepunch.Pool.Get<ProtoBuf.SlotMachine>();
		info.msg.slotMachine.oldResult1 = this.SpinResultPrevious1;
		info.msg.slotMachine.oldResult2 = this.SpinResultPrevious2;
		info.msg.slotMachine.oldResult3 = this.SpinResultPrevious3;
		info.msg.slotMachine.newResult1 = this.SpinResult1;
		info.msg.slotMachine.newResult2 = this.SpinResult2;
		info.msg.slotMachine.newResult3 = this.SpinResult3;
		info.msg.slotMachine.isSpinning = this.IsSpinning;
		info.msg.slotMachine.spinTime = this.SpinTime;
		info.msg.slotMachine.storageID = this.StorageInstance.uid;
		info.msg.slotMachine.multiplier = this.CurrentMultiplier;
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x0009517C File Offset: 0x0009337C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.slotMachine != null)
		{
			this.SpinResultPrevious1 = info.msg.slotMachine.oldResult1;
			this.SpinResultPrevious2 = info.msg.slotMachine.oldResult2;
			this.SpinResultPrevious3 = info.msg.slotMachine.oldResult3;
			this.SpinResult1 = info.msg.slotMachine.newResult1;
			this.SpinResult2 = info.msg.slotMachine.newResult2;
			this.SpinResult3 = info.msg.slotMachine.newResult3;
			this.CurrentMultiplier = info.msg.slotMachine.multiplier;
			if (base.isServer)
			{
				this.SpinTime = info.msg.slotMachine.spinTime;
			}
			this.StorageInstance.uid = info.msg.slotMachine.storageID;
			if (info.fromDisk && base.isServer)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			}
		}
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float GetComfort()
	{
		return 1f;
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00095294 File Offset: 0x00093494
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.StoragePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, false, false);
			this.StorageInstance.Set(baseEntity);
		}
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x000952F4 File Offset: 0x000934F4
	internal override void DoServerDestroy()
	{
		SlotMachineStorage slotMachineStorage = this.StorageInstance.Get(base.isServer) as SlotMachineStorage;
		if (slotMachineStorage.IsValid())
		{
			slotMachineStorage.DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x00095330 File Offset: 0x00093530
	private int GetBettingAmount()
	{
		SlotMachineStorage component = this.StorageInstance.Get(base.isServer).GetComponent<SlotMachineStorage>();
		if (component == null)
		{
			return 0;
		}
		global::Item slot = component.inventory.GetSlot(0);
		if (slot != null)
		{
			return slot.amount;
		}
		return 0;
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x00095378 File Offset: 0x00093578
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(global::BaseEntity.RPCMessage rpc)
	{
		if (this.IsSpinning)
		{
			return;
		}
		if (rpc.player != base.GetMounted())
		{
			return;
		}
		SlotMachineStorage component = this.StorageInstance.Get(base.isServer).GetComponent<SlotMachineStorage>();
		int num = (int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier;
		if (this.GetBettingAmount() < num)
		{
			return;
		}
		if (rpc.player == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		this.CurrentSpinPlayer = player;
		player.inventory.loot.Clear();
		global::Item slot = component.inventory.GetSlot(0);
		int num2 = 0;
		if (slot != null)
		{
			if (slot.amount > num)
			{
				slot.MarkDirty();
				slot.amount -= num;
				num2 = slot.amount;
			}
			else
			{
				slot.amount -= num;
				slot.RemoveFromContainer();
			}
		}
		component.UpdateAmount(num2);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		this.SpinResultPrevious1 = this.SpinResult1;
		this.SpinResultPrevious2 = this.SpinResult2;
		this.SpinResultPrevious3 = this.SpinResult3;
		this.CalculateSpinResults();
		this.SpinTime = UnityEngine.Time.time;
		base.ClientRPC<sbyte, sbyte, sbyte>(null, "RPC_OnSpin", (sbyte)this.SpinResult1, (sbyte)this.SpinResult2, (sbyte)this.SpinResult3);
		base.Invoke(new Action(this.CheckPayout), this.SpinDuration);
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x000954DC File Offset: 0x000936DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Deposit(global::BaseEntity.RPCMessage rpc)
	{
		global::BasePlayer player = rpc.player;
		if (player == null)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		if (this.StorageInstance.IsValid(base.isServer))
		{
			this.StorageInstance.Get(base.isServer).GetComponent<StorageContainer>().PlayerOpenLoot(player, "", false);
		}
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x00095540 File Offset: 0x00093740
	private void CheckPayout()
	{
		bool flag = false;
		if (this.PayoutSettings != null)
		{
			SlotMachinePayoutSettings.PayoutInfo payoutInfo;
			int num;
			if (this.CalculatePayout(out payoutInfo, out num))
			{
				int num2 = ((int)payoutInfo.Item.amount + num) * this.CurrentMultiplier;
				global::BaseEntity baseEntity = this.StorageInstance.Get(true);
				SlotMachineStorage slotMachineStorage;
				if (baseEntity != null && (slotMachineStorage = baseEntity as SlotMachineStorage) != null)
				{
					global::Item slot = slotMachineStorage.inventory.GetSlot(1);
					if (slot != null)
					{
						slot.amount += num2;
						slot.MarkDirty();
					}
					else
					{
						ItemManager.Create(payoutInfo.Item.itemDef, num2, 0UL).MoveToContainer(slotMachineStorage.inventory, 1, true, false, null, true);
					}
				}
				if (this.CurrentSpinPlayer.IsValid() && this.CurrentSpinPlayer == this._mounted)
				{
					this.CurrentSpinPlayer.ChatMessage(string.Format("You received {0}x {1} for slots payout!", num2, payoutInfo.Item.itemDef.displayName.english));
				}
				Analytics.Server.SlotMachineTransaction((int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, num2);
				Analytics.Azure.OnGamblingResult(this.CurrentSpinPlayer, this, (int)this.PayoutSettings.SpinCost.amount, num2, null);
				if (payoutInfo.OverrideWinEffect != null && payoutInfo.OverrideWinEffect.isValid)
				{
					Effect.server.Run(payoutInfo.OverrideWinEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				else if (this.PayoutSettings.DefaultWinEffect != null && this.PayoutSettings.DefaultWinEffect.isValid)
				{
					Effect.server.Run(this.PayoutSettings.DefaultWinEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				if (payoutInfo.OverrideWinEffect != null && payoutInfo.OverrideWinEffect.isValid)
				{
					flag = true;
				}
			}
			else
			{
				Analytics.Server.SlotMachineTransaction((int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, 0);
				Analytics.Azure.OnGamblingResult(this.CurrentSpinPlayer, this, (int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, 0, null);
			}
		}
		else
		{
			Debug.LogError(string.Format("Failed to process spin results: PayoutSettings != null {0} CurrentSpinPlayer.IsValid {1} CurrentSpinPlayer == mounted {2}", this.PayoutSettings != null, this.CurrentSpinPlayer.IsValid(), this.CurrentSpinPlayer == this._mounted));
		}
		if (!flag)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		}
		else
		{
			base.Invoke(new Action(this.DelayedSpinningReset), 4f);
		}
		this.CurrentSpinPlayer = null;
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x000957EE File Offset: 0x000939EE
	private void DelayedSpinningReset()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00095800 File Offset: 0x00093A00
	private void CalculateSpinResults()
	{
		if (global::SlotMachine.ForcePayoutIndex != -1)
		{
			this.SpinResult1 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result1;
			this.SpinResult2 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result2;
			this.SpinResult3 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result3;
			return;
		}
		this.SpinResult1 = this.RandomSpinResult();
		this.SpinResult2 = this.RandomSpinResult();
		this.SpinResult3 = this.RandomSpinResult();
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x0009589C File Offset: 0x00093A9C
	private int RandomSpinResult()
	{
		int num = new System.Random(UnityEngine.Random.Range(0, 1000)).Next(0, this.PayoutSettings.TotalStops);
		int num2 = 0;
		int num3 = 0;
		foreach (int num4 in this.PayoutSettings.VirtualFaces)
		{
			if (num < num4 + num2)
			{
				return num3;
			}
			num2 += num4;
			num3++;
		}
		return 15;
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00095908 File Offset: 0x00093B08
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		global::BaseEntity baseEntity = this.StorageInstance.Get(true);
		SlotMachineStorage slotMachineStorage;
		if (baseEntity != null && (slotMachineStorage = baseEntity as SlotMachineStorage) != null)
		{
			global::Item slot = slotMachineStorage.inventory.GetSlot(1);
			if (slot != null)
			{
				slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00095964 File Offset: 0x00093B64
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void Server_RequestMultiplierChange(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this._mounted)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		this.CurrentMultiplier = Mathf.Clamp(msg.read.Int32(), 1, 5);
		this.OnBettingScrapUpdated(this.GetBettingAmount());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x000959BE File Offset: 0x00093BBE
	public void OnBettingScrapUpdated(int amount)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, (float)amount >= this.PayoutSettings.SpinCost.amount * (float)this.CurrentMultiplier, false, true);
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x000959EC File Offset: 0x00093BEC
	private bool CalculatePayout(out SlotMachinePayoutSettings.PayoutInfo info, out int bonus)
	{
		info = default(SlotMachinePayoutSettings.PayoutInfo);
		bonus = 0;
		foreach (SlotMachinePayoutSettings.IndividualPayouts individualPayouts in this.PayoutSettings.FacePayouts)
		{
			if (individualPayouts.Result == this.SpinResult1)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == this.SpinResult2)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == this.SpinResult3)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (bonus > 0)
			{
				info.Item = new ItemAmount(individualPayouts.Item.itemDef, 0f);
			}
		}
		foreach (SlotMachinePayoutSettings.PayoutInfo payoutInfo in this.PayoutSettings.Payouts)
		{
			if (payoutInfo.Result1 == this.SpinResult1 && payoutInfo.Result2 == this.SpinResult2 && payoutInfo.Result3 == this.SpinResult3)
			{
				info = payoutInfo;
				return true;
			}
		}
		return bonus > 0;
	}

	// Token: 0x04000B6A RID: 2922
	[ServerVar]
	public static int ForcePayoutIndex = -1;

	// Token: 0x04000B6B RID: 2923
	[Header("Slot Machine")]
	public Transform Reel1;

	// Token: 0x04000B6C RID: 2924
	public Transform Reel2;

	// Token: 0x04000B6D RID: 2925
	public Transform Reel3;

	// Token: 0x04000B6E RID: 2926
	public Transform Arm;

	// Token: 0x04000B6F RID: 2927
	public AnimationCurve Curve;

	// Token: 0x04000B70 RID: 2928
	public int Reel1Spins = 16;

	// Token: 0x04000B71 RID: 2929
	public int Reel2Spins = 48;

	// Token: 0x04000B72 RID: 2930
	public int Reel3Spins = 80;

	// Token: 0x04000B73 RID: 2931
	public int MaxReelSpins = 96;

	// Token: 0x04000B74 RID: 2932
	public float SpinDuration = 2f;

	// Token: 0x04000B75 RID: 2933
	private int SpinResult1;

	// Token: 0x04000B76 RID: 2934
	private int SpinResult2;

	// Token: 0x04000B77 RID: 2935
	private int SpinResult3;

	// Token: 0x04000B78 RID: 2936
	private int SpinResultPrevious1;

	// Token: 0x04000B79 RID: 2937
	private int SpinResultPrevious2;

	// Token: 0x04000B7A RID: 2938
	private int SpinResultPrevious3;

	// Token: 0x04000B7B RID: 2939
	private float SpinTime;

	// Token: 0x04000B7C RID: 2940
	public GameObjectRef StoragePrefab;

	// Token: 0x04000B7D RID: 2941
	public EntityRef StorageInstance;

	// Token: 0x04000B7E RID: 2942
	public SoundDefinition SpinSound;

	// Token: 0x04000B7F RID: 2943
	public SlotMachinePayoutDisplay PayoutDisplay;

	// Token: 0x04000B80 RID: 2944
	public SlotMachinePayoutSettings PayoutSettings;

	// Token: 0x04000B81 RID: 2945
	public Transform HandIkTarget;

	// Token: 0x04000B82 RID: 2946
	private const global::BaseEntity.Flags HasScrapForSpin = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000B83 RID: 2947
	private const global::BaseEntity.Flags IsSpinningFlag = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000B84 RID: 2948
	public Material PayoutIconMaterial;

	// Token: 0x04000B85 RID: 2949
	public bool UseTimeOfDayAdjustedSprite = true;

	// Token: 0x04000B86 RID: 2950
	public MeshRenderer[] PulseRenderers;

	// Token: 0x04000B87 RID: 2951
	public float PulseSpeed = 5f;

	// Token: 0x04000B88 RID: 2952
	[ColorUsage(true, true)]
	public Color PulseFrom;

	// Token: 0x04000B89 RID: 2953
	[ColorUsage(true, true)]
	public Color PulseTo;

	// Token: 0x04000B8B RID: 2955
	private global::BasePlayer CurrentSpinPlayer;

	// Token: 0x02000C11 RID: 3089
	public enum SlotFaces
	{
		// Token: 0x04004246 RID: 16966
		Scrap,
		// Token: 0x04004247 RID: 16967
		Rope,
		// Token: 0x04004248 RID: 16968
		Apple,
		// Token: 0x04004249 RID: 16969
		LowGrade,
		// Token: 0x0400424A RID: 16970
		Wood,
		// Token: 0x0400424B RID: 16971
		Bandage,
		// Token: 0x0400424C RID: 16972
		Charcoal,
		// Token: 0x0400424D RID: 16973
		Gunpowder,
		// Token: 0x0400424E RID: 16974
		Rust,
		// Token: 0x0400424F RID: 16975
		Meat,
		// Token: 0x04004250 RID: 16976
		Hammer,
		// Token: 0x04004251 RID: 16977
		Sulfur,
		// Token: 0x04004252 RID: 16978
		TechScrap,
		// Token: 0x04004253 RID: 16979
		Frags,
		// Token: 0x04004254 RID: 16980
		Cloth,
		// Token: 0x04004255 RID: 16981
		LuckySeven
	}
}
