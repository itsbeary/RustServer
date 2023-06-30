using System;
using ConVar;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x0200007F RID: 127
public class HackableLockedCrate : LootContainer
{
	// Token: 0x06000BFB RID: 3067 RVA: 0x00069400 File Offset: 0x00067600
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0))
		{
			if (rpc == 888500940U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Hack ");
				}
				using (TimeWarning.New("RPC_Hack", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(888500940U, "RPC_Hack", this, player, 3f))
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
							this.RPC_Hack(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Hack");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool IsBeingHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0000564C File Offset: 0x0000384C
	public bool IsFullyHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00069568 File Offset: 0x00067768
	public override void DestroyShared()
	{
		if (base.isServer && this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		base.DestroyShared();
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00069594 File Offset: 0x00067794
	public void CreateMapMarker(float durationMinutes)
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		baseEntity.transform.localPosition = Vector3.zero;
		baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x0006960E File Offset: 0x0006780E
	public void RefreshDecay()
	{
		base.CancelInvoke(new Action(this.DelayedDestroy));
		if (this.shouldDecay)
		{
			base.Invoke(new Action(this.DelayedDestroy), HackableLockedCrate.decaySeconds);
		}
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00069644 File Offset: 0x00067844
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			if (StringPool.Get(info.HitBone) == "laptopcollision")
			{
				Effect.server.Run(this.shockEffect.resourcePath, info.HitPositionWorld, Vector3.up, null, false);
				this.hackSeconds -= 8f * (info.damageTypes.Total() / 50f);
				if (this.hackSeconds < 0f)
				{
					this.hackSeconds = 0f;
				}
			}
			this.RefreshDecay();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x000696D6 File Offset: 0x000678D6
	public void SetWasDropped()
	{
		this.wasDropped = true;
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x000696E0 File Offset: 0x000678E0
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (!Rust.Application.isLoadingSave)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
			if (this.wasDropped)
			{
				base.InvokeRepeating(new Action(this.LandCheck), 0f, 0.015f);
			}
			Analytics.Azure.OnEntitySpawned(this);
		}
		this.RefreshDecay();
		this.isLootable = this.IsFullyHacked();
		this.CreateMapMarker(120f);
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00069768 File Offset: 0x00067968
	public void LandCheck()
	{
		if (this.hasLanded)
		{
			return;
		}
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, Vector3.down), out raycastHit, 1f, 1218511105))
		{
			Effect.server.Run(this.landEffect.resourcePath, raycastHit.point, Vector3.up, null, false);
			this.hasLanded = true;
			base.CancelInvoke(new Action(this.LandCheck));
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x000697F1 File Offset: 0x000679F1
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00069807 File Offset: 0x00067A07
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Hack(BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingHacked())
		{
			return;
		}
		Analytics.Azure.OnLockedCrateStarted(msg.player, this);
		this.OriginalHackerPlayer = msg.player.userID;
		this.StartHacking();
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00069838 File Offset: 0x00067A38
	public void StartHacking()
	{
		base.BroadcastEntityMessage("HackingStarted", 20f, 256);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		base.InvokeRepeating(new Action(this.HackProgress), 1f, 1f);
		base.ClientRPC<int, int>(null, "UpdateHackProgress", 0, (int)HackableLockedCrate.requiredHackSeconds);
		this.RefreshDecay();
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x000698A0 File Offset: 0x00067AA0
	public void HackProgress()
	{
		this.hackSeconds += 1f;
		if (this.hackSeconds > HackableLockedCrate.requiredHackSeconds)
		{
			Analytics.Azure.OnLockedCrateFinished(this.OriginalHackerPlayer, this);
			this.RefreshDecay();
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.isLootable = true;
			base.CancelInvoke(new Action(this.HackProgress));
		}
		base.ClientRPC<int, int>(null, "UpdateHackProgress", (int)this.hackSeconds, (int)HackableLockedCrate.requiredHackSeconds);
	}

	// Token: 0x040007B3 RID: 1971
	public const BaseEntity.Flags Flag_Hacking = BaseEntity.Flags.Reserved1;

	// Token: 0x040007B4 RID: 1972
	public const BaseEntity.Flags Flag_FullyHacked = BaseEntity.Flags.Reserved2;

	// Token: 0x040007B5 RID: 1973
	public Text timerText;

	// Token: 0x040007B6 RID: 1974
	[ServerVar(Help = "How many seconds for the crate to unlock")]
	public static float requiredHackSeconds = 900f;

	// Token: 0x040007B7 RID: 1975
	[ServerVar(Help = "How many seconds until the crate is destroyed without any hack attempts")]
	public static float decaySeconds = 7200f;

	// Token: 0x040007B8 RID: 1976
	public SoundPlayer hackProgressBeep;

	// Token: 0x040007B9 RID: 1977
	private float hackSeconds;

	// Token: 0x040007BA RID: 1978
	public GameObjectRef shockEffect;

	// Token: 0x040007BB RID: 1979
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x040007BC RID: 1980
	public GameObjectRef landEffect;

	// Token: 0x040007BD RID: 1981
	public bool shouldDecay = true;

	// Token: 0x040007BE RID: 1982
	[NonSerialized]
	public ulong OriginalHackerPlayer;

	// Token: 0x040007BF RID: 1983
	private BaseEntity mapMarkerInstance;

	// Token: 0x040007C0 RID: 1984
	private bool hasLanded;

	// Token: 0x040007C1 RID: 1985
	private bool wasDropped;
}
