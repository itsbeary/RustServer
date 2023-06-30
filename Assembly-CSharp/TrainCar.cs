using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x020000E3 RID: 227
public class TrainCar : global::BaseVehicle, TriggerHurtNotChild.IHurtTriggerUser, TrainTrackSpline.ITrainTrackUser, ITrainCollidable, IPrefabPreProcess
{
	// Token: 0x060013C0 RID: 5056 RVA: 0x0009E39C File Offset: 0x0009C59C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainCar.OnRpcMessage", 0))
		{
			if (rpc == 3930273067U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsUncouple ");
				}
				using (TimeWarning.New("RPC_WantsUncouple", 0))
				{
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
							this.RPC_WantsUncouple(rpcmessage);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_WantsUncouple");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x060013C1 RID: 5057 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0009E4C0 File Offset: 0x0009C6C0
	// (set) Token: 0x060013C3 RID: 5059 RVA: 0x0009E4C8 File Offset: 0x0009C6C8
	public float FrontWheelSplineDist { get; private set; }

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x060013C4 RID: 5060 RVA: 0x0009E4D1 File Offset: 0x0009C6D1
	public bool FrontAtEndOfLine
	{
		get
		{
			return this.frontAtEndOfLine;
		}
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x060013C5 RID: 5061 RVA: 0x0009E4D9 File Offset: 0x0009C6D9
	public bool RearAtEndOfLine
	{
		get
		{
			return this.rearAtEndOfLine;
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x060013C6 RID: 5062 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool networkUpdateOnCompleteTrainChange
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x060013C7 RID: 5063 RVA: 0x0009E4E1 File Offset: 0x0009C6E1
	// (set) Token: 0x060013C8 RID: 5064 RVA: 0x0009E4EC File Offset: 0x0009C6EC
	public TrainTrackSpline FrontTrackSection
	{
		get
		{
			return this._frontTrackSection;
		}
		private set
		{
			if (this._frontTrackSection != value)
			{
				if (this._frontTrackSection != null)
				{
					this._frontTrackSection.DeregisterTrackUser(this);
				}
				this._frontTrackSection = value;
				if (this._frontTrackSection != null)
				{
					this._frontTrackSection.RegisterTrackUser(this);
				}
			}
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x060013C9 RID: 5065 RVA: 0x0009E542 File Offset: 0x0009C742
	// (set) Token: 0x060013CA RID: 5066 RVA: 0x0009E54A File Offset: 0x0009C74A
	public TrainTrackSpline RearTrackSection { get; private set; }

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x060013CB RID: 5067 RVA: 0x0009E553 File Offset: 0x0009C753
	protected bool IsAtAStation
	{
		get
		{
			return this.FrontTrackSection != null && this.FrontTrackSection.isStation;
		}
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x060013CC RID: 5068 RVA: 0x0009E570 File Offset: 0x0009C770
	protected bool IsOnAboveGroundSpawnRail
	{
		get
		{
			return this.FrontTrackSection != null && this.FrontTrackSection.aboveGroundSpawn;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x060013CD RID: 5069 RVA: 0x0009E58D File Offset: 0x0009C78D
	private bool RecentlySpawned
	{
		get
		{
			return UnityEngine.Time.time < this.initialSpawnTime + 2f;
		}
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0009E5A4 File Offset: 0x0009C7A4
	public override void ServerInit()
	{
		base.ServerInit();
		this.spawnOrigin = base.transform.position;
		this.distFrontToBackWheel = Vector3.Distance(this.GetFrontWheelPos(), this.GetRearWheelPos());
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.UpdateCompleteTrain();
		this.lastDecayTick = UnityEngine.Time.time;
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(20f, 40f), this.decayTickSpacing, this.decayTickSpacing * 0.1f);
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0009E65A File Offset: 0x0009C85A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.health <= 0f)
		{
			this.ActualDeath();
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0009E694 File Offset: 0x0009C894
	public override void Spawn()
	{
		base.Spawn();
		this.initialSpawnTime = UnityEngine.Time.time;
		TrainTrackSpline trainTrackSpline;
		float num;
		if (TrainTrackSpline.TryFindTrackNear(this.GetFrontWheelPos(), 15f, out trainTrackSpline, out num))
		{
			this.FrontWheelSplineDist = num;
			Vector3 vector;
			Vector3 positionAndTangent = trainTrackSpline.GetPositionAndTangent(this.FrontWheelSplineDist, base.transform.forward, out vector);
			this.SetTheRestFromFrontWheelData(ref trainTrackSpline, positionAndTangent, vector, this.localTrackSelection, null, true);
			this.FrontTrackSection = trainTrackSpline;
			if (!Rust.Application.isLoadingSave && !this.SpaceIsClear())
			{
				base.Invoke(new Action(base.KillMessage), 0f);
				return;
			}
		}
		else
		{
			base.Invoke(new Action(base.KillMessage), 0f);
		}
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0009E740 File Offset: 0x0009C940
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseTrain = Facepunch.Pool.Get<BaseTrain>();
		info.msg.baseTrain.time = this.GetNetworkTime();
		info.msg.baseTrain.frontBogieYRot = this.frontBogieYRot;
		info.msg.baseTrain.rearBogieYRot = this.rearBogieYRot;
		NetworkableId networkableId;
		if (this.coupling.frontCoupling.TryGetCoupledToID(out networkableId))
		{
			info.msg.baseTrain.frontCouplingID = networkableId;
			info.msg.baseTrain.frontCouplingToFront = this.coupling.frontCoupling.CoupledTo.isFrontCoupling;
		}
		if (this.coupling.rearCoupling.TryGetCoupledToID(out networkableId))
		{
			info.msg.baseTrain.rearCouplingID = networkableId;
			info.msg.baseTrain.rearCouplingToFront = this.coupling.rearCoupling.CoupledTo.isFrontCoupling;
		}
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0009E83C File Offset: 0x0009CA3C
	protected virtual void ServerFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (this.isSpawned && (next.HasFlag(global::BaseEntity.Flags.Reserved2) != old.HasFlag(global::BaseEntity.Flags.Reserved2) || next.HasFlag(global::BaseEntity.Flags.Reserved3) != old.HasFlag(global::BaseEntity.Flags.Reserved3)))
		{
			this.UpdateCompleteTrain();
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0009E8B0 File Offset: 0x0009CAB0
	private void UpdateCompleteTrain()
	{
		List<TrainCar> list = Facepunch.Pool.GetList<TrainCar>();
		this.coupling.GetAll(ref list);
		if (this.completeTrain == null || !this.completeTrain.Matches(list))
		{
			this.SetNewCompleteTrain(new CompleteTrain(list));
			return;
		}
		Facepunch.Pool.FreeList<TrainCar>(ref list);
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0009E8FA File Offset: 0x0009CAFA
	public void SetNewCompleteTrain(CompleteTrain ct)
	{
		if (this.completeTrain == ct)
		{
			return;
		}
		this.RemoveFromCompleteTrain();
		this.completeTrain = ct;
		if (this.networkUpdateOnCompleteTrainChange)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0009E922 File Offset: 0x0009CB22
	public override void Hurt(HitInfo info)
	{
		if (this.RecentlySpawned)
		{
			return;
		}
		base.Hurt(info);
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x0009E934 File Offset: 0x0009CB34
	public override void OnKilled(HitInfo info)
	{
		float num = ((info == null) ? 0f : info.damageTypes.Get(DamageType.AntiVehicle));
		float num2 = ((info == null) ? 0f : info.damageTypes.Get(DamageType.Explosion));
		float num3 = ((info == null) ? 0f : info.damageTypes.Total());
		if ((num + num2) / num3 > 0.5f || vehicle.cinematictrains || this.corpseSeconds == 0f)
		{
			if (base.HasDriver())
			{
				base.GetDriver().Hurt(float.MaxValue);
			}
			base.OnKilled(info);
		}
		else
		{
			base.Invoke(new Action(this.ActualDeath), this.corpseSeconds);
		}
		if (base.IsDestroyed && this.fxDestroyed.isValid)
		{
			Effect.server.Run(this.fxDestroyed.resourcePath, this.GetExplosionPos(), Vector3.up, null, true);
		}
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x0009EA10 File Offset: 0x0009CC10
	protected virtual Vector3 GetExplosionPos()
	{
		return this.GetCentreOfTrainPos();
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x00029C50 File Offset: 0x00027E50
	public void ActualDeath()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x0009EA18 File Offset: 0x0009CC18
	public override void DoRepair(global::BasePlayer player)
	{
		base.DoRepair(player);
		if (this.IsDead() && this.Health() > 0f)
		{
			base.CancelInvoke(new Action(this.ActualDeath));
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x0009EA4F File Offset: 0x0009CC4F
	public float GetDamageMultiplier(global::BaseEntity ent)
	{
		return Mathf.Abs(this.GetTrackSpeed()) * 1f;
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnHurtTriggerOccupant(global::BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0009EA62 File Offset: 0x0009CC62
	internal override void DoServerDestroy()
	{
		if (this.FrontTrackSection != null)
		{
			this.FrontTrackSection.DeregisterTrackUser(this);
		}
		this.coupling.Uncouple(true);
		this.coupling.Uncouple(false);
		this.RemoveFromCompleteTrain();
		base.DoServerDestroy();
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0009EAA2 File Offset: 0x0009CCA2
	private void RemoveFromCompleteTrain()
	{
		if (this.completeTrain == null)
		{
			return;
		}
		if (this.completeTrain.ContainsOnly(this))
		{
			this.completeTrain.Dispose();
			this.completeTrain = null;
			return;
		}
		this.completeTrain.RemoveTrainCar(this);
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0009EADA File Offset: 0x0009CCDA
	public override bool MountEligable(global::BasePlayer player)
	{
		return !this.IsDead() && base.MountEligable(player);
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0009EAED File Offset: 0x0009CCED
	public override float MaxVelocity()
	{
		return TrainCar.TRAINCAR_MAX_SPEED;
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x0009EAF4 File Offset: 0x0009CCF4
	public float GetTrackSpeed()
	{
		if (this.completeTrain == null)
		{
			return 0f;
		}
		return this.completeTrain.GetTrackSpeedFor(this);
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x0009EB10 File Offset: 0x0009CD10
	public bool IsCoupledBackwards()
	{
		return this.completeTrain != null && this.completeTrain.IsCoupledBackwards(this);
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x0009EB28 File Offset: 0x0009CD28
	public float GetPrevTrackSpeed()
	{
		if (this.completeTrain == null)
		{
			return 0f;
		}
		return this.completeTrain.GetPrevTrackSpeedFor(this);
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x0009EB44 File Offset: 0x0009CD44
	public override Vector3 GetLocalVelocityServer()
	{
		return base.transform.forward * this.GetTrackSpeed();
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x0009EB5C File Offset: 0x0009CD5C
	public bool AnyPlayersOnTrainCar()
	{
		if (this.AnyMounted())
		{
			return true;
		}
		if (this.platformParentTrigger != null && this.platformParentTrigger.HasAnyEntityContents)
		{
			using (HashSet<global::BaseEntity>.Enumerator enumerator = this.platformParentTrigger.entityContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ToPlayer() != null)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x0009EBE8 File Offset: 0x0009CDE8
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.completeTrain == null)
		{
			return;
		}
		this.completeTrain.UpdateTick(UnityEngine.Time.fixedDeltaTime);
		float trackSpeed = this.GetTrackSpeed();
		this.hurtTriggerFront.gameObject.SetActive(!this.coupling.IsFrontCoupled && trackSpeed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(!this.coupling.IsRearCoupled && trackSpeed < -this.hurtTriggerMinSpeed);
		GameObject[] array = this.hurtOrRepelTriggersInternal;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(Mathf.Abs(trackSpeed) > this.hurtTriggerMinSpeed);
		}
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x0009EC99 File Offset: 0x0009CE99
	public override void PostVehicleFixedUpdate()
	{
		base.PostVehicleFixedUpdate();
		if (this.completeTrain == null)
		{
			return;
		}
		this.completeTrain.ResetUpdateTick();
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0009ECB5 File Offset: 0x0009CEB5
	public Vector3 GetCentreOfTrainPos()
	{
		return base.transform.position + base.transform.rotation * this.bounds.center;
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x0009ECE4 File Offset: 0x0009CEE4
	public Vector3 GetFrontOfTrainPos()
	{
		return base.transform.position + base.transform.rotation * (this.bounds.center + Vector3.forward * this.bounds.extents.z);
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x0009ED3C File Offset: 0x0009CF3C
	public Vector3 GetRearOfTrainPos()
	{
		return base.transform.position + base.transform.rotation * (this.bounds.center - Vector3.forward * this.bounds.extents.z);
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x0009ED94 File Offset: 0x0009CF94
	public void FrontTrainCarTick(TrainTrackSpline.TrackSelection trackSelection, float dt)
	{
		float num = this.GetTrackSpeed() * dt;
		TrainTrackSpline trainTrackSpline = ((this.RearTrackSection != this.FrontTrackSection) ? this.RearTrackSection : null);
		this.MoveFrontWheelsAlongTrackSpline(this.FrontTrackSection, this.FrontWheelSplineDist, num, trainTrackSpline, trackSelection);
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x0009EDDC File Offset: 0x0009CFDC
	public void OtherTrainCarTick(TrainTrackSpline theirTrackSpline, float prevSplineDist, float distanceOffset)
	{
		this.MoveFrontWheelsAlongTrackSpline(theirTrackSpline, prevSplineDist, distanceOffset, this.FrontTrackSection, TrainTrackSpline.TrackSelection.Default);
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0009EDEE File Offset: 0x0009CFEE
	public bool TryGetNextTrainCar(Vector3 forwardDir, out TrainCar result)
	{
		return this.TryGetTrainCar(true, forwardDir, out result);
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0009EDF9 File Offset: 0x0009CFF9
	public bool TryGetPrevTrainCar(Vector3 forwardDir, out TrainCar result)
	{
		return this.TryGetTrainCar(false, forwardDir, out result);
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0009EE04 File Offset: 0x0009D004
	public bool TryGetTrainCar(bool next, Vector3 forwardDir, out TrainCar result)
	{
		result = null;
		return this.completeTrain != null && this.completeTrain.TryGetAdjacentTrainCar(this, next, forwardDir, out result);
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0009EE24 File Offset: 0x0009D024
	private void MoveFrontWheelsAlongTrackSpline(TrainTrackSpline trackSpline, float prevSplineDist, float distToMove, TrainTrackSpline preferredAltTrack, TrainTrackSpline.TrackSelection trackSelection)
	{
		TrainTrackSpline trainTrackSpline;
		this.FrontWheelSplineDist = trackSpline.GetSplineDistAfterMove(prevSplineDist, base.transform.forward, distToMove, trackSelection, out trainTrackSpline, out this.frontAtEndOfLine, preferredAltTrack, null);
		Vector3 vector;
		Vector3 positionAndTangent = trainTrackSpline.GetPositionAndTangent(this.FrontWheelSplineDist, base.transform.forward, out vector);
		this.SetTheRestFromFrontWheelData(ref trainTrackSpline, positionAndTangent, vector, trackSelection, trackSpline, false);
		this.FrontTrackSection = trainTrackSpline;
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0009EE86 File Offset: 0x0009D086
	private Vector3 GetFrontWheelPos()
	{
		return base.transform.position + base.transform.rotation * this.frontBogieLocalOffset;
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0009EEAE File Offset: 0x0009D0AE
	private Vector3 GetRearWheelPos()
	{
		return base.transform.position + base.transform.rotation * this.rearBogieLocalOffset;
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0009EED8 File Offset: 0x0009D0D8
	private void SetTheRestFromFrontWheelData(ref TrainTrackSpline frontTS, Vector3 targetFrontWheelPos, Vector3 targetFrontWheelTangent, TrainTrackSpline.TrackSelection trackSelection, TrainTrackSpline additionalAlt, bool instantMove)
	{
		TrainTrackSpline trainTrackSpline;
		float splineDistAfterMove = frontTS.GetSplineDistAfterMove(this.FrontWheelSplineDist, base.transform.forward, -this.distFrontToBackWheel, trackSelection, out trainTrackSpline, out this.rearAtEndOfLine, this.RearTrackSection, additionalAlt);
		Vector3 vector;
		Vector3 positionAndTangent = trainTrackSpline.GetPositionAndTangent(splineDistAfterMove, base.transform.forward, out vector);
		if (this.rearAtEndOfLine)
		{
			this.FrontWheelSplineDist = trainTrackSpline.GetSplineDistAfterMove(splineDistAfterMove, base.transform.forward, this.distFrontToBackWheel, trackSelection, out frontTS, out this.frontAtEndOfLine, trainTrackSpline, additionalAlt);
			targetFrontWheelPos = frontTS.GetPositionAndTangent(this.FrontWheelSplineDist, base.transform.forward, out targetFrontWheelTangent);
		}
		this.RearTrackSection = trainTrackSpline;
		Vector3 normalized = (targetFrontWheelPos - positionAndTangent).normalized;
		Vector3 vector2 = targetFrontWheelPos - Quaternion.LookRotation(normalized) * this.frontBogieLocalOffset;
		if (instantMove)
		{
			base.transform.position = vector2;
			if (normalized.magnitude == 0f)
			{
				base.transform.rotation = Quaternion.identity;
			}
			else
			{
				base.transform.rotation = Quaternion.LookRotation(normalized);
			}
		}
		else
		{
			base.transform.position = vector2;
			if (normalized.magnitude == 0f)
			{
				base.transform.rotation = Quaternion.identity;
			}
			else
			{
				base.transform.rotation = Quaternion.LookRotation(normalized);
			}
		}
		this.frontBogieYRot = Vector3.SignedAngle(base.transform.forward, targetFrontWheelTangent, base.transform.up);
		this.rearBogieYRot = Vector3.SignedAngle(base.transform.forward, vector, base.transform.up);
		if (UnityEngine.Application.isEditor)
		{
			Debug.DrawLine(targetFrontWheelPos, positionAndTangent, Color.magenta, 0.2f);
			Debug.DrawLine(this.rigidBody.position, vector2, Color.yellow, 0.2f);
			Debug.DrawRay(vector2, Vector3.up, Color.yellow, 0.2f);
		}
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0009F0C0 File Offset: 0x0009D2C0
	public float GetForces()
	{
		float num = 0f;
		float num2 = base.transform.localEulerAngles.x;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		return num + num2 / 90f * -UnityEngine.Physics.gravity.y * this.RealisticMass + this.GetThrottleForce();
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x00029EBC File Offset: 0x000280BC
	protected virtual float GetThrottleForce()
	{
		return 0f;
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual bool HasThrottleInput()
	{
		return false;
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0009F118 File Offset: 0x0009D318
	public float ApplyCollisionDamage(float forceMagnitude)
	{
		float num;
		if (forceMagnitude > this.derailCollisionForce)
		{
			num = float.MaxValue;
		}
		else
		{
			num = Mathf.Pow(forceMagnitude, 1.3f) / this.collisionDamageDivide;
		}
		base.Hurt(num, DamageType.Collision, this, false);
		return num;
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0009F158 File Offset: 0x0009D358
	public bool SpaceIsClear()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(this.WorldSpaceBounds(), list, 32768, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in list)
		{
			if (!this.ColliderIsPartOfTrain(collider))
			{
				return false;
			}
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return true;
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x0009F1D0 File Offset: 0x0009D3D0
	public bool ColliderIsPartOfTrain(Collider collider)
	{
		global::BaseEntity baseEntity = collider.ToBaseEntity();
		if (baseEntity == null)
		{
			return false;
		}
		if (baseEntity == this)
		{
			return true;
		}
		global::BaseEntity baseEntity2 = baseEntity.parentEntity.Get(base.isServer);
		return baseEntity2.IsValid() && baseEntity2 == this;
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x0009F21D File Offset: 0x0009D41D
	private void UpdateClients()
	{
		if (base.IsMoving())
		{
			base.ClientRPC<float, float, float>(null, "BaseTrainUpdate", this.GetNetworkTime(), this.frontBogieYRot, this.rearBogieYRot);
		}
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x0009F248 File Offset: 0x0009D448
	private void DecayTick()
	{
		if (this.completeTrain == null)
		{
			return;
		}
		bool flag = base.HasDriver() || this.completeTrain.AnyPlayersOnTrain();
		if (flag)
		{
			this.decayingFor = 0f;
		}
		float num = this.GetDecayMinutes(flag) * 60f;
		float time = UnityEngine.Time.time;
		float num2 = time - this.lastDecayTick;
		this.lastDecayTick = time;
		if (num != float.PositiveInfinity)
		{
			this.decayingFor += num2;
			if (this.decayingFor >= num && this.CanDieFromDecayNow())
			{
				this.ActualDeath();
			}
		}
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x0009F2DC File Offset: 0x0009D4DC
	protected virtual float GetDecayMinutes(bool hasPassengers)
	{
		bool flag = this.IsAtAStation && Vector3.Distance(this.spawnOrigin, base.transform.position) < 50f;
		if (hasPassengers || this.AnyPlayersNearby(30f) || flag || this.IsOnAboveGroundSpawnRail)
		{
			return float.PositiveInfinity;
		}
		return TrainCar.decayminutes * this.decayTimeMultiplier;
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0009F341 File Offset: 0x0009D541
	protected virtual bool CanDieFromDecayNow()
	{
		return this.CarType == TrainCar.TrainCarType.Engine || !this.completeTrain.IncludesAnEngine();
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x0009F35C File Offset: 0x0009D55C
	private bool AnyPlayersNearby(float maxDist)
	{
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		global::Vis.Entities<global::BasePlayer>(base.transform.position, maxDist, list, 131072, QueryTriggerInteraction.Collide);
		bool flag = false;
		foreach (global::BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				flag = true;
				break;
			}
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
		return flag;
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0009F3E0 File Offset: 0x0009D5E0
	[global::BaseEntity.RPC_Server]
	public void RPC_WantsUncouple(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (Vector3.SqrMagnitude(base.transform.position - player.transform.position) > 200f)
		{
			return;
		}
		bool flag = msg.read.Bit();
		this.coupling.Uncouple(flag);
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x060013FF RID: 5119 RVA: 0x0009F43E File Offset: 0x0009D63E
	public TriggerTrainCollisions FrontCollisionTrigger
	{
		get
		{
			return this.frontCollisionTrigger;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06001400 RID: 5120 RVA: 0x0009F446 File Offset: 0x0009D646
	public TriggerTrainCollisions RearCollisionTrigger
	{
		get
		{
			return this.rearCollisionTrigger;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x06001401 RID: 5121 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Wagon;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06001402 RID: 5122 RVA: 0x00023AF4 File Offset: 0x00021CF4
	public bool LinedUpToUnload
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved4);
		}
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x0009F450 File Offset: 0x0009D650
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		this.frontBogieLocalOffset = base.transform.InverseTransformPoint(this.frontBogiePivot.position);
		float num;
		if (this.frontCoupling != null)
		{
			num = base.transform.InverseTransformPoint(this.frontCoupling.position).z;
		}
		else
		{
			num = this.bounds.extents.z + this.bounds.center.z;
		}
		float num2;
		if (this.rearCoupling != null)
		{
			num2 = base.transform.InverseTransformPoint(this.rearCoupling.position).z;
		}
		else
		{
			num2 = -this.bounds.extents.z + this.bounds.center.z;
		}
		this.DistFrontWheelToFrontCoupling = num - this.frontBogieLocalOffset.z;
		this.DistFrontWheelToBackCoupling = -num2 + this.frontBogieLocalOffset.z;
		this.rearBogieLocalOffset = base.transform.InverseTransformPoint(this.rearBogiePivot.position);
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x0009F568 File Offset: 0x0009D768
	public override void InitShared()
	{
		base.InitShared();
		this.coupling = new TrainCouplingController(this);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0009F57C File Offset: 0x0009D77C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null && base.isServer)
		{
			this.frontBogieYRot = info.msg.baseTrain.frontBogieYRot;
			this.rearBogieYRot = info.msg.baseTrain.rearBogieYRot;
		}
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0009F5D1 File Offset: 0x0009D7D1
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x00007A44 File Offset: 0x00005C44
	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		return false;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x00029084 File Offset: 0x00027284
	public override float InheritedVelocityScale()
	{
		return 0.5f;
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0009F5F0 File Offset: 0x0009D7F0
	protected virtual void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		if (this.localTrackSelection == trackSelection)
		{
			return;
		}
		this.localTrackSelection = trackSelection;
		if (base.isServer)
		{
			base.ClientRPC<sbyte>(null, "SetTrackSelection", (sbyte)this.localTrackSelection);
		}
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0009F61E File Offset: 0x0009D81E
	protected bool PlayerIsOnPlatform(global::BasePlayer player)
	{
		return player.GetParentEntity() == this;
	}

	// Token: 0x04000C48 RID: 3144
	protected bool trainDebug;

	// Token: 0x04000C49 RID: 3145
	public CompleteTrain completeTrain;

	// Token: 0x04000C4A RID: 3146
	private bool frontAtEndOfLine;

	// Token: 0x04000C4B RID: 3147
	private bool rearAtEndOfLine;

	// Token: 0x04000C4C RID: 3148
	private float frontBogieYRot;

	// Token: 0x04000C4D RID: 3149
	private float rearBogieYRot;

	// Token: 0x04000C4E RID: 3150
	private Vector3 spawnOrigin;

	// Token: 0x04000C4F RID: 3151
	public static float TRAINCAR_MAX_SPEED = 25f;

	// Token: 0x04000C50 RID: 3152
	private TrainTrackSpline _frontTrackSection;

	// Token: 0x04000C52 RID: 3154
	private float distFrontToBackWheel;

	// Token: 0x04000C53 RID: 3155
	private float initialSpawnTime;

	// Token: 0x04000C54 RID: 3156
	protected float decayingFor;

	// Token: 0x04000C55 RID: 3157
	private float decayTickSpacing = 60f;

	// Token: 0x04000C56 RID: 3158
	private float lastDecayTick;

	// Token: 0x04000C57 RID: 3159
	[Header("Train Car")]
	[SerializeField]
	private float corpseSeconds = 60f;

	// Token: 0x04000C58 RID: 3160
	[SerializeField]
	private TriggerTrainCollisions frontCollisionTrigger;

	// Token: 0x04000C59 RID: 3161
	[SerializeField]
	private TriggerTrainCollisions rearCollisionTrigger;

	// Token: 0x04000C5A RID: 3162
	[SerializeField]
	private float collisionDamageDivide = 100000f;

	// Token: 0x04000C5B RID: 3163
	[SerializeField]
	private float derailCollisionForce = 130000f;

	// Token: 0x04000C5C RID: 3164
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x04000C5D RID: 3165
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x04000C5E RID: 3166
	[SerializeField]
	private GameObject[] hurtOrRepelTriggersInternal;

	// Token: 0x04000C5F RID: 3167
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000C60 RID: 3168
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04000C61 RID: 3169
	[SerializeField]
	private Transform frontBogiePivot;

	// Token: 0x04000C62 RID: 3170
	[SerializeField]
	private bool frontBogieCanRotate = true;

	// Token: 0x04000C63 RID: 3171
	[SerializeField]
	private Transform rearBogiePivot;

	// Token: 0x04000C64 RID: 3172
	[SerializeField]
	private bool rearBogieCanRotate = true;

	// Token: 0x04000C65 RID: 3173
	[SerializeField]
	private Transform[] wheelVisuals;

	// Token: 0x04000C66 RID: 3174
	[SerializeField]
	private float wheelRadius = 0.615f;

	// Token: 0x04000C67 RID: 3175
	[FormerlySerializedAs("fxFinalExplosion")]
	[SerializeField]
	private GameObjectRef fxDestroyed;

	// Token: 0x04000C68 RID: 3176
	[SerializeField]
	protected TriggerParent platformParentTrigger;

	// Token: 0x04000C69 RID: 3177
	public GameObjectRef collisionEffect;

	// Token: 0x04000C6A RID: 3178
	public Transform frontCoupling;

	// Token: 0x04000C6B RID: 3179
	public Transform frontCouplingPivot;

	// Token: 0x04000C6C RID: 3180
	public Transform rearCoupling;

	// Token: 0x04000C6D RID: 3181
	public Transform rearCouplingPivot;

	// Token: 0x04000C6E RID: 3182
	[SerializeField]
	private SoundDefinition coupleSound;

	// Token: 0x04000C6F RID: 3183
	[SerializeField]
	private SoundDefinition uncoupleSound;

	// Token: 0x04000C70 RID: 3184
	[SerializeField]
	private TrainCarAudio trainCarAudio;

	// Token: 0x04000C71 RID: 3185
	[FormerlySerializedAs("frontCoupleFx")]
	[SerializeField]
	private ParticleSystem frontCouplingChangedFx;

	// Token: 0x04000C72 RID: 3186
	[FormerlySerializedAs("rearCoupleFx")]
	[SerializeField]
	private ParticleSystem rearCouplingChangedFx;

	// Token: 0x04000C73 RID: 3187
	[FormerlySerializedAs("fxCoupling")]
	[SerializeField]
	private ParticleSystem newCouplingFX;

	// Token: 0x04000C74 RID: 3188
	[SerializeField]
	private float decayTimeMultiplier = 1f;

	// Token: 0x04000C75 RID: 3189
	[SerializeField]
	[ReadOnly]
	private Vector3 frontBogieLocalOffset;

	// Token: 0x04000C76 RID: 3190
	[SerializeField]
	[ReadOnly]
	private Vector3 rearBogieLocalOffset;

	// Token: 0x04000C77 RID: 3191
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 2.3f;

	// Token: 0x04000C78 RID: 3192
	[ServerVar(Help = "Ratio of wagons to train engines that spawn")]
	public static int wagons_per_engine = 2;

	// Token: 0x04000C79 RID: 3193
	[ServerVar(Help = "How long before a train car despawns")]
	public static float decayminutes = 30f;

	// Token: 0x04000C7A RID: 3194
	[ReadOnly]
	public float DistFrontWheelToFrontCoupling;

	// Token: 0x04000C7B RID: 3195
	[ReadOnly]
	public float DistFrontWheelToBackCoupling;

	// Token: 0x04000C7C RID: 3196
	public TrainCouplingController coupling;

	// Token: 0x04000C7D RID: 3197
	[NonSerialized]
	public TrainTrackSpline.TrackSelection localTrackSelection;

	// Token: 0x04000C7E RID: 3198
	public const global::BaseEntity.Flags Flag_LinedUpToUnload = global::BaseEntity.Flags.Reserved4;

	// Token: 0x02000C1B RID: 3099
	public enum TrainCarType
	{
		// Token: 0x0400427C RID: 17020
		Wagon,
		// Token: 0x0400427D RID: 17021
		Engine,
		// Token: 0x0400427E RID: 17022
		Other
	}
}
