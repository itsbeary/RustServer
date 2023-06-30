using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003C RID: 60
public class BaseFishingRod : global::HeldEntity
{
	// Token: 0x0600037C RID: 892 RVA: 0x0002E0E4 File Offset: 0x0002C2E4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseFishingRod.OnRpcMessage", 0))
		{
			if (rpc == 4237324865U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Cancel ");
				}
				using (TimeWarning.New("Server_Cancel", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(4237324865U, "Server_Cancel", this, player))
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
							this.Server_Cancel(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Cancel");
					}
				}
				return true;
			}
			if (rpc == 4238539495U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestCast ");
				}
				using (TimeWarning.New("Server_RequestCast", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(4238539495U, "Server_RequestCast", this, player))
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
							this.Server_RequestCast(rpcmessage2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_RequestCast");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0002E3DC File Offset: 0x0002C5DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void Server_RequestCast(global::BaseEntity.RPCMessage msg)
	{
		Vector3 vector = msg.read.Vector3();
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		global::Item currentLure = this.GetCurrentLure();
		if (currentLure == null)
		{
			this.FailedCast(BaseFishingRod.FailReason.NoLure);
			return;
		}
		BaseFishingRod.FailReason failReason;
		if (!this.EvaluateFishingPosition(ref vector, ownerPlayer, out failReason, out this.surfaceBody))
		{
			this.FailedCast(failReason);
			return;
		}
		FishingBobber component = base.gameManager.CreateEntity(this.FishingBobberRef.resourcePath, base.transform.position + Vector3.up * 2.8f + ownerPlayer.eyes.BodyForward() * 1.8f, base.GetOwnerPlayer().ServerRotation, true).GetComponent<FishingBobber>();
		component.transform.forward = base.GetOwnerPlayer().eyes.BodyForward();
		component.Spawn();
		component.InitialiseBobber(ownerPlayer, this.surfaceBody, vector);
		this.lureUsed = currentLure.info;
		currentLure.UseItem(1);
		if (this.fishLookup == null)
		{
			this.fishLookup = PrefabAttribute.server.Find<FishLookup>(this.prefabID);
		}
		this.currentFishTarget = this.fishLookup.GetFish(component.transform.position, this.surfaceBody, this.lureUsed, out this.fishableModifier, this.lastFish);
		this.lastFish = this.fishableModifier;
		this.currentBobber.Set(component);
		base.ClientRPC<NetworkableId>(null, "Client_ReceiveCastPoint", component.net.ID);
		ownerPlayer.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		this.catchTime = (BaseFishingRod.ImmediateHook ? 0f : UnityEngine.Random.Range(10f, 20f));
		this.catchTime *= this.fishableModifier.CatchWaitTimeMultiplier;
		ItemModCompostable itemModCompostable;
		float num = (this.lureUsed.TryGetComponent<ItemModCompostable>(out itemModCompostable) ? itemModCompostable.BaitValue : 0f);
		num = Mathx.RemapValClamped(num, 0f, 20f, 1f, 10f);
		this.catchTime = Mathf.Clamp(this.catchTime - num, 3f, 20f);
		this.playerStartPosition = ownerPlayer.transform.position;
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		this.CurrentState = BaseFishingRod.CatchState.Waiting;
		base.InvokeRepeating(new Action(this.CatchProcess), 0f, 0f);
		this.inQueue = false;
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0002E659 File Offset: 0x0002C859
	private void FailedCast(BaseFishingRod.FailReason reason)
	{
		this.CurrentState = BaseFishingRod.CatchState.None;
		base.ClientRPC<int>(null, "Client_ResetLine", (int)reason);
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0002E66F File Offset: 0x0002C86F
	private void CatchProcess()
	{
		if (!this.inQueue)
		{
			this.inQueue = true;
			BaseFishingRod.updateFishingRodQueue.Add(this);
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0002E68C File Offset: 0x0002C88C
	private void CatchProcessBudgeted()
	{
		this.inQueue = false;
		FishingBobber fishingBobber = this.currentBobber.Get(true);
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null || ownerPlayer.IsSleeping() || ownerPlayer.IsWounded() || ownerPlayer.IsDead() || fishingBobber == null)
		{
			this.Server_Cancel(BaseFishingRod.FailReason.UserRequested);
			return;
		}
		Vector3 position = ownerPlayer.transform.position;
		float num = Vector3.Angle((fishingBobber.transform.position.WithY(0f) - position.WithY(0f)).normalized, ownerPlayer.eyes.HeadForward().WithY(0f));
		float num2 = Vector3.Distance(position, fishingBobber.transform.position.WithY(position.y));
		if (num > ((num2 > 1.2f) ? 60f : 180f))
		{
			this.Server_Cancel(BaseFishingRod.FailReason.BadAngle);
			return;
		}
		if (num2 > 1.2f && this.lastSightCheck > 0.4f)
		{
			if (!GamePhysics.LineOfSight(ownerPlayer.eyes.position, fishingBobber.transform.position, 1218511105, null))
			{
				this.Server_Cancel(BaseFishingRod.FailReason.Obstructed);
				return;
			}
			this.lastSightCheck = 0f;
		}
		if (Vector3.Distance(position, fishingBobber.transform.position) > this.MaxCastDistance * 2f)
		{
			this.Server_Cancel(BaseFishingRod.FailReason.TooFarAway);
			return;
		}
		if (Vector3.Distance(this.playerStartPosition, position) > 1f)
		{
			this.Server_Cancel(BaseFishingRod.FailReason.PlayerMoved);
			return;
		}
		if (this.CurrentState == BaseFishingRod.CatchState.Waiting)
		{
			if (this.catchTime < 0f)
			{
				base.ClientRPC(null, "Client_HookedSomething");
				this.CurrentState = BaseFishingRod.CatchState.Catching;
				fishingBobber.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
				this.nextFishStateChange = 0f;
				this.fishCatchDuration = 0f;
				this.strainTimer = 0f;
			}
			return;
		}
		BaseFishingRod.FishState fishState = this.currentFishState;
		if (this.nextFishStateChange < 0f)
		{
			float num3 = Mathx.RemapValClamped(fishingBobber.TireAmount, 0f, 20f, 0f, 1f);
			if (this.currentFishState != (BaseFishingRod.FishState)0)
			{
				this.currentFishState = (BaseFishingRod.FishState)0;
				this.nextFishStateChange = UnityEngine.Random.Range(2f, 4f) * (num3 + 1f);
			}
			else
			{
				this.nextFishStateChange = UnityEngine.Random.Range(3f, 7f) * (1f - num3);
				if (UnityEngine.Random.Range(0, 100) < 50)
				{
					this.currentFishState = BaseFishingRod.FishState.PullingLeft;
				}
				else
				{
					this.currentFishState = BaseFishingRod.FishState.PullingRight;
				}
				if (UnityEngine.Random.Range(0, 100) > 60 && Vector3.Distance(fishingBobber.transform.position, ownerPlayer.transform.position) < this.MaxCastDistance - 2f)
				{
					this.currentFishState |= BaseFishingRod.FishState.PullingBack;
				}
			}
		}
		if (this.fishCatchDuration > 120f)
		{
			this.Server_Cancel(BaseFishingRod.FailReason.TimeOut);
			return;
		}
		bool flag = ownerPlayer.serverInput.IsDown(BUTTON.RIGHT);
		bool flag2 = ownerPlayer.serverInput.IsDown(BUTTON.LEFT);
		bool flag3 = this.HasReelInInput(ownerPlayer.serverInput);
		if (flag2 && flag)
		{
			flag = (flag2 = false);
		}
		this.UpdateFlags(flag2, flag, flag3);
		if (this.CurrentState == BaseFishingRod.CatchState.Waiting)
		{
			flag2 = (flag = (flag3 = false));
		}
		if (flag2 && !this.AllowPullInDirection(-ownerPlayer.eyes.HeadRight(), fishingBobber.transform.position))
		{
			flag2 = false;
		}
		if (flag && !this.AllowPullInDirection(ownerPlayer.eyes.HeadRight(), fishingBobber.transform.position))
		{
			flag = false;
		}
		fishingBobber.ServerMovementUpdate(flag2, flag, flag3, ref this.currentFishState, position, this.fishableModifier);
		bool flag4 = false;
		float num4 = 0f;
		if (flag3 || flag2 || flag)
		{
			flag4 = true;
			num4 = 0.5f;
		}
		if (this.currentFishState != (BaseFishingRod.FishState)0 && flag4)
		{
			if (this.currentFishState.Contains(BaseFishingRod.FishState.PullingBack) && flag3)
			{
				num4 = 1.5f;
			}
			else if ((this.currentFishState.Contains(BaseFishingRod.FishState.PullingLeft) || this.currentFishState.Contains(BaseFishingRod.FishState.PullingRight)) && flag3)
			{
				num4 = 1.2f;
			}
			else if (this.currentFishState.Contains(BaseFishingRod.FishState.PullingLeft) && flag)
			{
				num4 = 0.8f;
			}
			else if (this.currentFishState.Contains(BaseFishingRod.FishState.PullingRight) && flag2)
			{
				num4 = 0.8f;
			}
		}
		if (flag3 && this.currentFishState != (BaseFishingRod.FishState)0)
		{
			num4 += 1f;
		}
		num4 *= this.fishableModifier.StrainModifier * this.GlobalStrainSpeedMultiplier;
		if (flag4)
		{
			this.strainTimer += UnityEngine.Time.deltaTime * num4;
		}
		else
		{
			this.strainTimer = Mathf.MoveTowards(this.strainTimer, 0f, UnityEngine.Time.deltaTime * 1.5f);
		}
		float num5 = this.strainTimer / 6f;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, flag4 && num5 > 0.25f, false, true);
		if (this.lastStrainUpdate > 0.4f || fishState != this.currentFishState)
		{
			base.ClientRPC<int, float>(null, "Client_UpdateFishState", (int)this.currentFishState, num5);
			this.lastStrainUpdate = 0f;
		}
		if (this.strainTimer > 7f || BaseFishingRod.ForceFail)
		{
			this.Server_Cancel(BaseFishingRod.FailReason.TensionBreak);
			return;
		}
		if (num2 <= this.FishCatchDistance || BaseFishingRod.ForceSuccess)
		{
			this.CurrentState = BaseFishingRod.CatchState.Caught;
			if (this.currentFishTarget != null)
			{
				global::Item item = ItemManager.Create(this.currentFishTarget, 1, 0UL);
				ownerPlayer.GiveItem(item, global::BaseEntity.GiveItemReason.Crafted);
				if (this.currentFishTarget.shortname == "skull.human")
				{
					item.name = RandomUsernames.Get(UnityEngine.Random.Range(0, 1000));
				}
				if (GameInfo.HasAchievements && !string.IsNullOrEmpty(this.fishableModifier.SteamStatName))
				{
					ownerPlayer.stats.Add(this.fishableModifier.SteamStatName, 1, global::Stats.Steam);
					ownerPlayer.stats.Save(true);
					this.fishLookup.CheckCatchAllAchievement(ownerPlayer);
				}
			}
			Analytics.Server.FishCaught(this.currentFishTarget);
			base.ClientRPC<int>(null, "Client_OnCaughtFish", this.currentFishTarget.itemid);
			ownerPlayer.SignalBroadcast(global::BaseEntity.Signal.Alt_Attack, null);
			base.Invoke(new Action(this.ResetLine), 6f);
			fishingBobber.Kill(global::BaseNetworkable.DestroyMode.None);
			this.currentBobber.Set(null);
			base.CancelInvoke(new Action(this.CatchProcess));
			return;
		}
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0002ECF8 File Offset: 0x0002CEF8
	private void ResetLine()
	{
		this.Server_Cancel(BaseFishingRod.FailReason.Success);
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0002ED01 File Offset: 0x0002CF01
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void Server_Cancel(global::BaseEntity.RPCMessage msg)
	{
		if (this.CurrentState == BaseFishingRod.CatchState.Caught)
		{
			return;
		}
		this.Server_Cancel(BaseFishingRod.FailReason.UserRequested);
	}

	// Token: 0x06000383 RID: 899 RVA: 0x0002ED14 File Offset: 0x0002CF14
	private void Server_Cancel(BaseFishingRod.FailReason reason)
	{
		if (this.GetItem() != null)
		{
			this.GetItem().LoseCondition((reason == BaseFishingRod.FailReason.Success) ? this.ConditionLossOnSuccess : this.ConditionLossOnFail);
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		this.UpdateFlags(false, false, false);
		base.CancelInvoke(new Action(this.CatchProcess));
		this.CurrentState = BaseFishingRod.CatchState.None;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
		FishingBobber fishingBobber = this.currentBobber.Get(true);
		if (fishingBobber != null)
		{
			fishingBobber.Kill(global::BaseNetworkable.DestroyMode.None);
			this.currentBobber.Set(null);
		}
		base.ClientRPC<int>(null, "Client_ResetLine", (int)reason);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x0002EDBA File Offset: 0x0002CFBA
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (this.CurrentState != BaseFishingRod.CatchState.None)
		{
			this.Server_Cancel(BaseFishingRod.FailReason.Unequipped);
		}
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0002EDD4 File Offset: 0x0002CFD4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.currentBobber.IsSet && info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Facepunch.Pool.Get<SimpleUID>();
			info.msg.simpleUID.uid = this.currentBobber.uid;
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x0002EE30 File Offset: 0x0002D030
	private void UpdateFlags(bool inputLeft = false, bool inputRight = false, bool back = false)
	{
		base.SetFlag(BaseFishingRod.PullingLeftFlag, this.CurrentState == BaseFishingRod.CatchState.Catching && inputLeft, false, true);
		base.SetFlag(BaseFishingRod.PullingRightFlag, this.CurrentState == BaseFishingRod.CatchState.Catching && inputRight, false, true);
		base.SetFlag(BaseFishingRod.ReelingInFlag, this.CurrentState == BaseFishingRod.CatchState.Catching && back, false, true);
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000387 RID: 903 RVA: 0x0002EE85 File Offset: 0x0002D085
	// (set) Token: 0x06000388 RID: 904 RVA: 0x0002EE8D File Offset: 0x0002D08D
	public BaseFishingRod.CatchState CurrentState { get; private set; }

	// Token: 0x06000389 RID: 905 RVA: 0x0002EE98 File Offset: 0x0002D098
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk)
		{
			return;
		}
		if (info.msg.simpleUID != null)
		{
			this.currentBobber.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0002EEE5 File Offset: 0x0002D0E5
	public override bool BlocksGestures()
	{
		return this.CurrentState > BaseFishingRod.CatchState.None;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0002EEF0 File Offset: 0x0002D0F0
	private bool AllowPullInDirection(Vector3 worldDirection, Vector3 bobberPosition)
	{
		Vector3 position = base.transform.position;
		Vector3 vector = bobberPosition.WithY(position.y);
		return Vector3.Dot(worldDirection, (vector - position).normalized) < 0f;
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0002EF34 File Offset: 0x0002D134
	private bool EvaluateFishingPosition(ref Vector3 pos, global::BasePlayer ply, out BaseFishingRod.FailReason reason, out WaterBody waterBody)
	{
		RaycastHit raycastHit;
		bool flag = GamePhysics.Trace(new Ray(pos + Vector3.up, Vector3.down), 0f, out raycastHit, 1.5f, 16, QueryTriggerInteraction.UseGlobal, null);
		if (flag)
		{
			waterBody = raycastHit.GetWaterBody();
			pos.y = raycastHit.point.y;
		}
		else
		{
			waterBody = null;
		}
		if (!flag)
		{
			reason = BaseFishingRod.FailReason.NoWaterFound;
			return false;
		}
		if (Vector3.Distance(ply.transform.position.WithY(pos.y), pos) < 5f)
		{
			reason = BaseFishingRod.FailReason.TooClose;
			return false;
		}
		if (!GamePhysics.LineOfSight(ply.eyes.position, pos, 1218652417, null))
		{
			reason = BaseFishingRod.FailReason.Obstructed;
			return false;
		}
		Vector3 vector = pos + Vector3.up * 2f;
		if (!GamePhysics.LineOfSight(ply.eyes.position, vector, 1218652417, null))
		{
			reason = BaseFishingRod.FailReason.Obstructed;
			return false;
		}
		Vector3 position = ply.transform.position;
		position.y = pos.y;
		float num = Vector3.Distance(pos, position);
		Vector3 vector2 = pos + (position - pos).normalized * (num - this.FishCatchDistance);
		if (!GamePhysics.LineOfSight(pos, vector2, 1218652417, null))
		{
			reason = BaseFishingRod.FailReason.Obstructed;
			return false;
		}
		if (WaterLevel.GetOverallWaterDepth(Vector3.Lerp(pos, ply.transform.position.WithY(pos.y), 0.95f), true, false, null, true) < 0.1f && ply.eyes.position.y > 0f)
		{
			reason = BaseFishingRod.FailReason.TooShallow;
			return false;
		}
		if (WaterLevel.GetOverallWaterDepth(pos, true, false, null, true) < 0.3f && ply.eyes.position.y > 0f)
		{
			reason = BaseFishingRod.FailReason.TooShallow;
			return false;
		}
		Vector3 vector3 = Vector3.MoveTowards(ply.transform.position.WithY(pos.y), pos, 1f);
		if (!GamePhysics.LineOfSight(ply.eyes.position, vector3, 1218652417, null))
		{
			reason = BaseFishingRod.FailReason.Obstructed;
			return false;
		}
		reason = BaseFishingRod.FailReason.Success;
		return true;
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0002F165 File Offset: 0x0002D365
	private global::Item GetCurrentLure()
	{
		if (this.GetItem() == null)
		{
			return null;
		}
		if (this.GetItem().contents == null)
		{
			return null;
		}
		return this.GetItem().contents.GetSlot(0);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0002F191 File Offset: 0x0002D391
	private bool HasReelInInput(InputState state)
	{
		return state.IsDown(BUTTON.BACKWARD) || state.IsDown(BUTTON.FIRE_PRIMARY);
	}

	// Token: 0x04000269 RID: 617
	public static BaseFishingRod.UpdateFishingRod updateFishingRodQueue = new BaseFishingRod.UpdateFishingRod();

	// Token: 0x0400026A RID: 618
	private FishLookup fishLookup;

	// Token: 0x0400026B RID: 619
	private TimeUntil nextFishStateChange;

	// Token: 0x0400026C RID: 620
	private TimeSince fishCatchDuration;

	// Token: 0x0400026D RID: 621
	private float strainTimer;

	// Token: 0x0400026E RID: 622
	private const float strainMax = 6f;

	// Token: 0x0400026F RID: 623
	private TimeSince lastStrainUpdate;

	// Token: 0x04000270 RID: 624
	private TimeUntil catchTime;

	// Token: 0x04000271 RID: 625
	private TimeSince lastSightCheck;

	// Token: 0x04000272 RID: 626
	private Vector3 playerStartPosition;

	// Token: 0x04000273 RID: 627
	private WaterBody surfaceBody;

	// Token: 0x04000274 RID: 628
	private ItemDefinition lureUsed;

	// Token: 0x04000275 RID: 629
	private ItemDefinition currentFishTarget;

	// Token: 0x04000276 RID: 630
	private ItemModFishable fishableModifier;

	// Token: 0x04000277 RID: 631
	private ItemModFishable lastFish;

	// Token: 0x04000278 RID: 632
	private bool inQueue;

	// Token: 0x04000279 RID: 633
	[ServerVar]
	public static bool ForceSuccess = false;

	// Token: 0x0400027A RID: 634
	[ServerVar]
	public static bool ForceFail = false;

	// Token: 0x0400027B RID: 635
	[ServerVar]
	public static bool ImmediateHook = false;

	// Token: 0x0400027C RID: 636
	public GameObjectRef FishingBobberRef;

	// Token: 0x0400027D RID: 637
	public float FishCatchDistance = 0.5f;

	// Token: 0x0400027E RID: 638
	public LineRenderer ReelLineRenderer;

	// Token: 0x0400027F RID: 639
	public Transform LineRendererWorldStartPos;

	// Token: 0x04000281 RID: 641
	private BaseFishingRod.FishState currentFishState;

	// Token: 0x04000282 RID: 642
	private EntityRef<FishingBobber> currentBobber;

	// Token: 0x04000283 RID: 643
	public float ConditionLossOnSuccess = 0.02f;

	// Token: 0x04000284 RID: 644
	public float ConditionLossOnFail = 0.04f;

	// Token: 0x04000285 RID: 645
	public float GlobalStrainSpeedMultiplier = 1f;

	// Token: 0x04000286 RID: 646
	public float MaxCastDistance = 10f;

	// Token: 0x04000287 RID: 647
	public const global::BaseEntity.Flags Straining = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000288 RID: 648
	public ItemModFishable ForceFish;

	// Token: 0x04000289 RID: 649
	public static global::BaseEntity.Flags PullingLeftFlag = global::BaseEntity.Flags.Reserved6;

	// Token: 0x0400028A RID: 650
	public static global::BaseEntity.Flags PullingRightFlag = global::BaseEntity.Flags.Reserved7;

	// Token: 0x0400028B RID: 651
	public static global::BaseEntity.Flags ReelingInFlag = global::BaseEntity.Flags.Reserved8;

	// Token: 0x0400028C RID: 652
	public GameObjectRef BobberPreview;

	// Token: 0x0400028D RID: 653
	public SoundDefinition onLineSoundDef;

	// Token: 0x0400028E RID: 654
	public SoundDefinition strainSoundDef;

	// Token: 0x0400028F RID: 655
	public AnimationCurve strainGainCurve;

	// Token: 0x04000290 RID: 656
	public SoundDefinition tensionBreakSoundDef;

	// Token: 0x02000B92 RID: 2962
	public class UpdateFishingRod : ObjectWorkQueue<BaseFishingRod>
	{
		// Token: 0x06004D42 RID: 19778 RVA: 0x001A0719 File Offset: 0x0019E919
		protected override void RunJob(BaseFishingRod entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.CatchProcessBudgeted();
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x001A072B File Offset: 0x0019E92B
		protected override bool ShouldAdd(BaseFishingRod entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}

	// Token: 0x02000B93 RID: 2963
	public enum CatchState
	{
		// Token: 0x04004011 RID: 16401
		None,
		// Token: 0x04004012 RID: 16402
		Aiming,
		// Token: 0x04004013 RID: 16403
		Waiting,
		// Token: 0x04004014 RID: 16404
		Catching,
		// Token: 0x04004015 RID: 16405
		Caught
	}

	// Token: 0x02000B94 RID: 2964
	[Flags]
	public enum FishState
	{
		// Token: 0x04004017 RID: 16407
		PullingLeft = 1,
		// Token: 0x04004018 RID: 16408
		PullingRight = 2,
		// Token: 0x04004019 RID: 16409
		PullingBack = 4
	}

	// Token: 0x02000B95 RID: 2965
	public enum FailReason
	{
		// Token: 0x0400401B RID: 16411
		UserRequested,
		// Token: 0x0400401C RID: 16412
		BadAngle,
		// Token: 0x0400401D RID: 16413
		TensionBreak,
		// Token: 0x0400401E RID: 16414
		Unequipped,
		// Token: 0x0400401F RID: 16415
		TimeOut,
		// Token: 0x04004020 RID: 16416
		Success,
		// Token: 0x04004021 RID: 16417
		NoWaterFound,
		// Token: 0x04004022 RID: 16418
		Obstructed,
		// Token: 0x04004023 RID: 16419
		NoLure,
		// Token: 0x04004024 RID: 16420
		TooShallow,
		// Token: 0x04004025 RID: 16421
		TooClose,
		// Token: 0x04004026 RID: 16422
		TooFarAway,
		// Token: 0x04004027 RID: 16423
		PlayerMoved
	}
}
