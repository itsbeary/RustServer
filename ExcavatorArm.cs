using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000074 RID: 116
public class ExcavatorArm : global::BaseEntity
{
	// Token: 0x06000AFA RID: 2810 RVA: 0x00063290 File Offset: 0x00061490
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ExcavatorArm.OnRpcMessage", 0))
		{
			if (rpc == 2059417170U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SetResourceTarget ");
				}
				using (TimeWarning.New("RPC_SetResourceTarget", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2059417170U, "RPC_SetResourceTarget", this, player, 3f))
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
							this.RPC_SetResourceTarget(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_SetResourceTarget");
					}
				}
				return true;
			}
			if (rpc == 2882020740U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StopMining ");
				}
				using (TimeWarning.New("RPC_StopMining", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2882020740U, "RPC_StopMining", this, player, 3f))
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
							this.RPC_StopMining(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_StopMining");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00007649 File Offset: 0x00005849
	public bool IsMining()
	{
		return base.IsOn();
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000AFD RID: 2813 RVA: 0x000349F2 File Offset: 0x00032BF2
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00063590 File Offset: 0x00061790
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		bool flag = this.IsMining() && this.IsPowered();
		float num = (flag ? 1f : 0f);
		this.currentTurnThrottle = Mathf.Lerp(this.currentTurnThrottle, num, UnityEngine.Time.fixedDeltaTime * (flag ? 0.333f : 1f));
		if (Mathf.Abs(num - this.currentTurnThrottle) < 0.025f)
		{
			this.currentTurnThrottle = num;
		}
		this.movedAmount += UnityEngine.Time.fixedDeltaTime * this.turnSpeed * this.currentTurnThrottle;
		float num2 = (Mathf.Sin(this.movedAmount) + 1f) / 2f;
		float num3 = Mathf.Lerp(this.yaw1, this.yaw2, num2);
		if (num3 != this.lastMoveYaw)
		{
			this.lastMoveYaw = num3;
			base.transform.rotation = Quaternion.Euler(0f, num3, 0f);
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0006368C File Offset: 0x0006188C
	public void BeginMining()
	{
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.ProduceResources), this.resourceProductionTickRate, this.resourceProductionTickRate);
		if (UnityEngine.Time.time > this.nextNotificationTime)
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (!basePlayer.IsNpc && basePlayer.IsConnected)
				{
					basePlayer.ShowToast(GameTip.Styles.Server_Event, this.excavatorPhrase, Array.Empty<string>());
				}
			}
			this.nextNotificationTime = UnityEngine.Time.time + 60f;
		}
		ExcavatorServerEffects.SetMining(true, false);
		Analytics.Server.ExcavatorStarted();
		this.excavatorStartTime = this.GetNetworkTime();
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x00063760 File Offset: 0x00061960
	public void StopMining()
	{
		ExcavatorServerEffects.SetMining(false, false);
		base.CancelInvoke(new Action(this.ProduceResources));
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			Analytics.Server.ExcavatorStopped(this.GetNetworkTime() - this.excavatorStartTime);
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x000637A0 File Offset: 0x000619A0
	public void ProduceResources()
	{
		float num = this.resourceProductionTickRate / this.timeForFullResources;
		float num2 = this.resourcesToMine[this.resourceMiningIndex].amount * num;
		this.pendingResources[this.resourceMiningIndex].amount += num2;
		foreach (ItemAmount itemAmount in this.pendingResources)
		{
			if (itemAmount.amount >= (float)this.outputPiles.Count)
			{
				int num3 = Mathf.FloorToInt(itemAmount.amount / (float)this.outputPiles.Count);
				itemAmount.amount -= (float)(num3 * 2);
				foreach (ExcavatorOutputPile excavatorOutputPile in this.outputPiles)
				{
					global::Item item = ItemManager.Create(this.resourcesToMine[this.resourceMiningIndex].itemDef, num3, 0UL);
					Analytics.Azure.OnExcavatorProduceItem(item, this);
					if (!item.MoveToContainer(excavatorOutputPile.inventory, -1, true, false, null, true))
					{
						item.Drop(excavatorOutputPile.GetDropPosition(), excavatorOutputPile.GetDropVelocity(), default(Quaternion));
					}
				}
			}
		}
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x000638EC File Offset: 0x00061AEC
	public override void OnEntityMessage(global::BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == "DieselEngineOn")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
			return;
		}
		if (msg == "DieselEngineOff")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
			this.StopMining();
		}
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00063940 File Offset: 0x00061B40
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SetResourceTarget(global::BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		if (text == "HQM")
		{
			this.resourceMiningIndex = 0;
		}
		else if (text == "Sulfur")
		{
			this.resourceMiningIndex = 1;
		}
		else if (text == "Stone")
		{
			this.resourceMiningIndex = 2;
		}
		else if (text == "Metal")
		{
			this.resourceMiningIndex = 3;
		}
		if (!base.IsOn())
		{
			this.BeginMining();
		}
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_StopMining(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x000639C2 File Offset: 0x00061BC2
	public override void Spawn()
	{
		base.Spawn();
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x000639CA File Offset: 0x00061BCA
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
		if (base.IsOn() && this.IsPowered())
		{
			this.BeginMining();
			return;
		}
		this.StopMining();
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x000639F8 File Offset: 0x00061BF8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.movedAmount;
		info.msg.ioEntity.genericInt1 = this.resourceMiningIndex;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x00063A48 File Offset: 0x00061C48
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.movedAmount = info.msg.ioEntity.genericFloat1;
			this.resourceMiningIndex = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x00063A95 File Offset: 0x00061C95
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x00063AA4 File Offset: 0x00061CA4
	public void Init()
	{
		this.pendingResources = new ItemAmount[this.resourcesToMine.Length];
		for (int i = 0; i < this.resourcesToMine.Length; i++)
		{
			this.pendingResources[i] = new ItemAmount(this.resourcesToMine[i].itemDef, 0f);
		}
		List<ExcavatorOutputPile> list = Facepunch.Pool.GetList<ExcavatorOutputPile>();
		global::Vis.Entities<ExcavatorOutputPile>(base.transform.position, 200f, list, 512, QueryTriggerInteraction.Collide);
		this.outputPiles = new List<ExcavatorOutputPile>();
		foreach (ExcavatorOutputPile excavatorOutputPile in list)
		{
			if (!excavatorOutputPile.isClient)
			{
				this.outputPiles.Add(excavatorOutputPile);
			}
		}
		Facepunch.Pool.FreeList<ExcavatorOutputPile>(ref list);
	}

	// Token: 0x0400071A RID: 1818
	public float yaw1;

	// Token: 0x0400071B RID: 1819
	public float yaw2;

	// Token: 0x0400071C RID: 1820
	public Transform wheel;

	// Token: 0x0400071D RID: 1821
	public float wheelSpeed = 2f;

	// Token: 0x0400071E RID: 1822
	public float turnSpeed = 0.1f;

	// Token: 0x0400071F RID: 1823
	public Transform miningOffset;

	// Token: 0x04000720 RID: 1824
	public GameObjectRef bounceEffect;

	// Token: 0x04000721 RID: 1825
	public LightGroupAtTime lights;

	// Token: 0x04000722 RID: 1826
	public Material conveyorMaterial;

	// Token: 0x04000723 RID: 1827
	public float beltSpeedMax = 0.1f;

	// Token: 0x04000724 RID: 1828
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000725 RID: 1829
	public List<ExcavatorOutputPile> outputPiles;

	// Token: 0x04000726 RID: 1830
	public SoundDefinition miningStartButtonSoundDef;

	// Token: 0x04000727 RID: 1831
	[Header("Production")]
	public ItemAmount[] resourcesToMine;

	// Token: 0x04000728 RID: 1832
	public float resourceProductionTickRate = 3f;

	// Token: 0x04000729 RID: 1833
	public float timeForFullResources = 120f;

	// Token: 0x0400072A RID: 1834
	private ItemAmount[] pendingResources;

	// Token: 0x0400072B RID: 1835
	public Translate.Phrase excavatorPhrase;

	// Token: 0x0400072C RID: 1836
	private float movedAmount;

	// Token: 0x0400072D RID: 1837
	private float currentTurnThrottle;

	// Token: 0x0400072E RID: 1838
	private float lastMoveYaw;

	// Token: 0x0400072F RID: 1839
	private float excavatorStartTime;

	// Token: 0x04000730 RID: 1840
	private float nextNotificationTime;

	// Token: 0x04000731 RID: 1841
	private int resourceMiningIndex;
}
