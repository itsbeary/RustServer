using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x02000616 RID: 1558
[CreateAssetMenu(menuName = "Rust/Missions/BaseMission")]
public class BaseMission : BaseScriptableObject
{
	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06002E21 RID: 11809 RVA: 0x001157C7 File Offset: 0x001139C7
	public uint id
	{
		get
		{
			return this.shortname.ManifestHash();
		}
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x001157D4 File Offset: 0x001139D4
	public static void PlayerDisconnected(BasePlayer player)
	{
		if (player.IsNpc)
		{
			return;
		}
		int activeMission = player.GetActiveMission();
		if (activeMission != -1 && activeMission < player.missions.Count)
		{
			BaseMission.MissionInstance missionInstance = player.missions[activeMission];
			BaseMission mission = missionInstance.GetMission();
			if (mission.missionEntities.Length != 0)
			{
				mission.MissionFailed(missionInstance, player, BaseMission.MissionFailReason.Disconnect);
			}
		}
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void PlayerKilled(BasePlayer player)
	{
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x06002E24 RID: 11812 RVA: 0x00115829 File Offset: 0x00113A29
	public bool isRepeatable
	{
		get
		{
			return this.repeatDelaySecondsSuccess != -1 || this.repeatDelaySecondsFailed != -1;
		}
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x00115842 File Offset: 0x00113A42
	public virtual Sprite GetIcon(BaseMission.MissionInstance instance)
	{
		return this.icon;
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x0011584C File Offset: 0x00113A4C
	public virtual void SetupPositions(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		foreach (BaseMission.PositionGenerator positionGenerator in this.positionGenerators)
		{
			if (!positionGenerator.IsDependant())
			{
				instance.missionPoints.Add(positionGenerator.GetIdentifier(), positionGenerator.GetPosition(assignee));
			}
		}
		foreach (BaseMission.PositionGenerator positionGenerator2 in this.positionGenerators)
		{
			if (positionGenerator2.IsDependant())
			{
				instance.missionPoints.Add(positionGenerator2.GetIdentifier(), positionGenerator2.GetPosition(assignee));
			}
		}
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x001158CC File Offset: 0x00113ACC
	public void AddBlockers(BaseMission.MissionInstance instance)
	{
		foreach (KeyValuePair<string, Vector3> keyValuePair in instance.missionPoints)
		{
			if (!BaseMission.blockedPoints.Contains(keyValuePair.Value))
			{
				BaseMission.blockedPoints.Add(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x0011593C File Offset: 0x00113B3C
	public void RemoveBlockers(BaseMission.MissionInstance instance)
	{
		foreach (KeyValuePair<string, Vector3> keyValuePair in instance.missionPoints)
		{
			if (BaseMission.blockedPoints.Contains(keyValuePair.Value))
			{
				BaseMission.blockedPoints.Remove(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x001159B0 File Offset: 0x00113BB0
	public virtual void SetupRewards(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		if (this.baseRewards.Length == 0)
		{
			return;
		}
		instance.rewards = new ItemAmount[this.baseRewards.Length];
		for (int i = 0; i < this.baseRewards.Length; i++)
		{
			instance.rewards[i] = new ItemAmount(this.baseRewards[i].itemDef, this.baseRewards[i].amount);
		}
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x00115A14 File Offset: 0x00113C14
	public static void DoMissionEffect(string effectString, BasePlayer assignee)
	{
		Effect effect = new Effect();
		effect.Init(Effect.Type.Generic, assignee, StringPool.Get("head"), Vector3.zero, Vector3.forward, null);
		effect.pooledString = effectString;
		EffectNetwork.Send(effect, assignee.net.connection);
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x00115A50 File Offset: 0x00113C50
	public virtual void MissionStart(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		this.SetupRewards(instance, assignee);
		this.SetupPositions(instance, assignee);
		this.AddBlockers(instance);
		for (int i = 0; i < this.objectives.Length; i++)
		{
			this.objectives[i].Get().MissionStarted(i, instance);
		}
		if (this.acceptEffect.isValid)
		{
			BaseMission.DoMissionEffect(this.acceptEffect.resourcePath, assignee);
		}
		foreach (BaseMission.MissionEntityEntry missionEntityEntry in this.missionEntities)
		{
			if (missionEntityEntry.entityRef.isValid)
			{
				Vector3 missionPoint = instance.GetMissionPoint(missionEntityEntry.spawnPositionToUse, assignee);
				BaseEntity baseEntity = GameManager.server.CreateEntity(missionEntityEntry.entityRef.resourcePath, missionPoint, Quaternion.identity, true);
				MissionEntity missionEntity = baseEntity.gameObject.AddComponent<MissionEntity>();
				missionEntity.Setup(assignee, instance, missionEntityEntry.cleanupOnMissionSuccess, missionEntityEntry.cleanupOnMissionFailed);
				instance.createdEntities.Add(missionEntity);
				baseEntity.Spawn();
			}
		}
		foreach (MissionEntity missionEntity2 in instance.createdEntities)
		{
			missionEntity2.MissionStarted(assignee, instance);
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x00115B88 File Offset: 0x00113D88
	public void CheckObjectives(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		bool flag = true;
		for (int i = 0; i < this.objectives.Length; i++)
		{
			if (!instance.objectiveStatuses[i].completed || instance.objectiveStatuses[i].failed)
			{
				flag = false;
			}
		}
		if (flag && instance.status == BaseMission.MissionStatus.Active)
		{
			this.MissionSuccess(instance, assignee);
		}
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x00115BE0 File Offset: 0x00113DE0
	public virtual void Think(BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		for (int i = 0; i < this.objectives.Length; i++)
		{
			this.objectives[i].Get().Think(i, instance, assignee, delta);
		}
		this.CheckObjectives(instance, assignee);
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x00115C20 File Offset: 0x00113E20
	public virtual void MissionComplete(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		BaseMission.DoMissionEffect(this.victoryEffect.resourcePath, assignee);
		assignee.ChatMessage("You have completed the mission : " + this.missionName.english);
		if (instance.rewards != null && instance.rewards.Length != 0)
		{
			foreach (ItemAmount itemAmount in instance.rewards)
			{
				if (itemAmount.itemDef == null || itemAmount.amount == 0f)
				{
					Debug.LogError("BIG REWARD SCREWUP, NULL ITEM DEF");
				}
				Item item = ItemManager.Create(itemAmount.itemDef, Mathf.CeilToInt(itemAmount.amount), 0UL);
				if (item != null)
				{
					assignee.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
				}
			}
		}
		Analytics.Server.MissionComplete(this);
		Analytics.Azure.OnMissionComplete(assignee, this, null);
		instance.status = BaseMission.MissionStatus.Completed;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty(true);
		if (GameInfo.HasAchievements)
		{
			assignee.stats.Add("missions_completed", 1, Stats.All);
			assignee.stats.Save(true);
		}
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x00115D1D File Offset: 0x00113F1D
	public virtual void MissionSuccess(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		instance.status = BaseMission.MissionStatus.Accomplished;
		this.MissionEnded(instance, assignee);
		this.MissionComplete(instance, assignee);
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x00115D38 File Offset: 0x00113F38
	public virtual void MissionFailed(BaseMission.MissionInstance instance, BasePlayer assignee, BaseMission.MissionFailReason failReason)
	{
		assignee.ChatMessage("You have failed the mission : " + this.missionName.english);
		BaseMission.DoMissionEffect(this.failedEffect.resourcePath, assignee);
		Analytics.Server.MissionFailed(this, failReason);
		Analytics.Azure.OnMissionComplete(assignee, this, new BaseMission.MissionFailReason?(failReason));
		instance.status = BaseMission.MissionStatus.Failed;
		this.MissionEnded(instance, assignee);
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x00115D94 File Offset: 0x00113F94
	public virtual void MissionEnded(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		if (instance.createdEntities != null)
		{
			for (int i = instance.createdEntities.Count - 1; i >= 0; i--)
			{
				MissionEntity missionEntity = instance.createdEntities[i];
				if (!(missionEntity == null))
				{
					missionEntity.MissionEnded(assignee, instance);
				}
			}
		}
		this.RemoveBlockers(instance);
		instance.endTime = Time.time;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty(true);
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x00115E00 File Offset: 0x00114000
	public void OnObjectiveCompleted(int objectiveIndex, BaseMission.MissionInstance instance, BasePlayer playerFor)
	{
		BaseMission.MissionObjectiveEntry missionObjectiveEntry = this.objectives[objectiveIndex];
		if (missionObjectiveEntry.autoCompleteOtherObjectives.Length != 0)
		{
			foreach (int num in missionObjectiveEntry.autoCompleteOtherObjectives)
			{
				BaseMission.MissionObjectiveEntry missionObjectiveEntry2 = this.objectives[num];
				if (!instance.objectiveStatuses[num].completed)
				{
					missionObjectiveEntry2.objective.CompleteObjective(num, instance, playerFor);
				}
			}
		}
		this.CheckObjectives(instance, playerFor);
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x00115E68 File Offset: 0x00114068
	public static bool AssignMission(BasePlayer assignee, IMissionProvider provider, BaseMission mission)
	{
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		if (!mission.IsEligableForMission(assignee, provider))
		{
			return false;
		}
		BaseMission.MissionInstance missionInstance = Pool.Get<BaseMission.MissionInstance>();
		missionInstance.missionID = mission.id;
		missionInstance.startTime = Time.time;
		missionInstance.providerID = provider.ProviderID();
		missionInstance.status = BaseMission.MissionStatus.Active;
		missionInstance.createdEntities = Pool.GetList<MissionEntity>();
		missionInstance.objectiveStatuses = new BaseMission.MissionInstance.ObjectiveStatus[mission.objectives.Length];
		for (int i = 0; i < mission.objectives.Length; i++)
		{
			missionInstance.objectiveStatuses[i] = new BaseMission.MissionInstance.ObjectiveStatus();
		}
		assignee.AddMission(missionInstance);
		mission.MissionStart(missionInstance, assignee);
		assignee.SetActiveMission(assignee.missions.Count - 1);
		assignee.MissionDirty(true);
		return true;
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x00115F24 File Offset: 0x00114124
	public bool IsEligableForMission(BasePlayer player, IMissionProvider provider)
	{
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		foreach (BaseMission.MissionInstance missionInstance in player.missions)
		{
			if (missionInstance.status == BaseMission.MissionStatus.Accomplished || missionInstance.status == BaseMission.MissionStatus.Active)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040025DC RID: 9692
	[ServerVar]
	public static bool missionsenabled = true;

	// Token: 0x040025DD RID: 9693
	public string shortname;

	// Token: 0x040025DE RID: 9694
	public Translate.Phrase missionName;

	// Token: 0x040025DF RID: 9695
	public Translate.Phrase missionDesc;

	// Token: 0x040025E0 RID: 9696
	public BaseMission.MissionObjectiveEntry[] objectives;

	// Token: 0x040025E1 RID: 9697
	public static List<Vector3> blockedPoints = new List<Vector3>();

	// Token: 0x040025E2 RID: 9698
	public const string MISSION_COMPLETE_STAT = "missions_completed";

	// Token: 0x040025E3 RID: 9699
	public GameObjectRef acceptEffect;

	// Token: 0x040025E4 RID: 9700
	public GameObjectRef failedEffect;

	// Token: 0x040025E5 RID: 9701
	public GameObjectRef victoryEffect;

	// Token: 0x040025E6 RID: 9702
	public int repeatDelaySecondsSuccess = -1;

	// Token: 0x040025E7 RID: 9703
	public int repeatDelaySecondsFailed = -1;

	// Token: 0x040025E8 RID: 9704
	public float timeLimitSeconds;

	// Token: 0x040025E9 RID: 9705
	public Sprite icon;

	// Token: 0x040025EA RID: 9706
	public Sprite providerIcon;

	// Token: 0x040025EB RID: 9707
	public BaseMission.MissionDependancy[] acceptDependancies;

	// Token: 0x040025EC RID: 9708
	public BaseMission.MissionDependancy[] completionDependancies;

	// Token: 0x040025ED RID: 9709
	public BaseMission.MissionEntityEntry[] missionEntities;

	// Token: 0x040025EE RID: 9710
	public BaseMission.PositionGenerator[] positionGenerators;

	// Token: 0x040025EF RID: 9711
	public ItemAmount[] baseRewards;

	// Token: 0x02000DA7 RID: 3495
	[Serializable]
	public class MissionDependancy
	{
		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06005155 RID: 20821 RVA: 0x001ABA2E File Offset: 0x001A9C2E
		public uint targetMissionID
		{
			get
			{
				return this.targetMissionShortname.ManifestHash();
			}
		}

		// Token: 0x040048C1 RID: 18625
		public string targetMissionShortname;

		// Token: 0x040048C2 RID: 18626
		public BaseMission.MissionStatus targetMissionDesiredStatus;

		// Token: 0x040048C3 RID: 18627
		public bool everAttempted;
	}

	// Token: 0x02000DA8 RID: 3496
	public enum MissionStatus
	{
		// Token: 0x040048C5 RID: 18629
		Default,
		// Token: 0x040048C6 RID: 18630
		Active,
		// Token: 0x040048C7 RID: 18631
		Accomplished,
		// Token: 0x040048C8 RID: 18632
		Failed,
		// Token: 0x040048C9 RID: 18633
		Completed
	}

	// Token: 0x02000DA9 RID: 3497
	public enum MissionEventType
	{
		// Token: 0x040048CB RID: 18635
		CUSTOM,
		// Token: 0x040048CC RID: 18636
		HARVEST,
		// Token: 0x040048CD RID: 18637
		CONVERSATION,
		// Token: 0x040048CE RID: 18638
		KILL_ENTITY,
		// Token: 0x040048CF RID: 18639
		ACQUIRE_ITEM,
		// Token: 0x040048D0 RID: 18640
		FREE_CRATE
	}

	// Token: 0x02000DAA RID: 3498
	[Serializable]
	public class MissionObjectiveEntry
	{
		// Token: 0x06005157 RID: 20823 RVA: 0x001ABA3B File Offset: 0x001A9C3B
		public MissionObjective Get()
		{
			return this.objective;
		}

		// Token: 0x040048D1 RID: 18641
		public Translate.Phrase description;

		// Token: 0x040048D2 RID: 18642
		public int[] startAfterCompletedObjectives;

		// Token: 0x040048D3 RID: 18643
		public int[] autoCompleteOtherObjectives;

		// Token: 0x040048D4 RID: 18644
		public bool onlyProgressIfStarted = true;

		// Token: 0x040048D5 RID: 18645
		public MissionObjective objective;
	}

	// Token: 0x02000DAB RID: 3499
	public class MissionInstance : Pool.IPooled
	{
		// Token: 0x06005159 RID: 20825 RVA: 0x001ABA52 File Offset: 0x001A9C52
		public BaseEntity ProviderEntity()
		{
			if (this._cachedProviderEntity == null)
			{
				this._cachedProviderEntity = BaseNetworkable.serverEntities.Find(this.providerID) as BaseEntity;
			}
			return this._cachedProviderEntity;
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x001ABA83 File Offset: 0x001A9C83
		public BaseMission GetMission()
		{
			if (this._cachedMission == null)
			{
				this._cachedMission = MissionManifest.GetFromID(this.missionID);
			}
			return this._cachedMission;
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x001ABAAA File Offset: 0x001A9CAA
		public bool ShouldShowOnMap()
		{
			return (this.status == BaseMission.MissionStatus.Active || this.status == BaseMission.MissionStatus.Accomplished) && this.missionLocation != Vector3.zero;
		}

		// Token: 0x0600515C RID: 20828 RVA: 0x001ABAD0 File Offset: 0x001A9CD0
		public bool ShouldShowOnCompass()
		{
			return this.ShouldShowOnMap();
		}

		// Token: 0x0600515D RID: 20829 RVA: 0x001ABAD8 File Offset: 0x001A9CD8
		public virtual void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionEventType type, string identifier, float amount)
		{
			if (this.status != BaseMission.MissionStatus.Active)
			{
				return;
			}
			BaseMission mission = this.GetMission();
			for (int i = 0; i < mission.objectives.Length; i++)
			{
				mission.objectives[i].objective.ProcessMissionEvent(playerFor, this, i, type, identifier, amount);
			}
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x001ABB24 File Offset: 0x001A9D24
		public void Think(BasePlayer assignee, float delta)
		{
			if (this.status == BaseMission.MissionStatus.Failed || this.status == BaseMission.MissionStatus.Completed)
			{
				return;
			}
			BaseMission mission = this.GetMission();
			this.timePassed += delta;
			mission.Think(this, assignee, delta);
			if (mission.timeLimitSeconds > 0f && this.timePassed >= mission.timeLimitSeconds)
			{
				mission.MissionFailed(this, assignee, BaseMission.MissionFailReason.TimeOut);
			}
		}

		// Token: 0x0600515F RID: 20831 RVA: 0x001ABB88 File Offset: 0x001A9D88
		public Vector3 GetMissionPoint(string identifier, BasePlayer playerFor)
		{
			if (this.missionPoints.ContainsKey(identifier))
			{
				return this.missionPoints[identifier];
			}
			if (!playerFor)
			{
				Debug.Log("Massive mission failure to get point, correct mission definition of : " + this.GetMission().shortname);
				return Vector3.zero;
			}
			this.GetMission().SetupPositions(this, playerFor);
			Debug.Log("Mission point not found, regenerating");
			if (this.missionPoints.ContainsKey(identifier))
			{
				return this.missionPoints[identifier];
			}
			return Vector3.zero;
		}

		// Token: 0x06005160 RID: 20832 RVA: 0x001ABC10 File Offset: 0x001A9E10
		public void EnterPool()
		{
			this.providerID = default(NetworkableId);
			this.missionID = 0U;
			this.status = BaseMission.MissionStatus.Default;
			this.completionScale = 0f;
			this.startTime = -1f;
			this.endTime = -1f;
			this.missionLocation = Vector3.zero;
			this._cachedMission = null;
			this.timePassed = 0f;
			this.rewards = null;
			this.missionPoints.Clear();
			if (this.createdEntities != null)
			{
				Pool.FreeList<MissionEntity>(ref this.createdEntities);
			}
		}

		// Token: 0x06005161 RID: 20833 RVA: 0x001ABC9A File Offset: 0x001A9E9A
		public void LeavePool()
		{
			this.createdEntities = Pool.GetList<MissionEntity>();
		}

		// Token: 0x040048D6 RID: 18646
		private BaseEntity _cachedProviderEntity;

		// Token: 0x040048D7 RID: 18647
		private BaseMission _cachedMission;

		// Token: 0x040048D8 RID: 18648
		public NetworkableId providerID;

		// Token: 0x040048D9 RID: 18649
		public uint missionID;

		// Token: 0x040048DA RID: 18650
		public BaseMission.MissionStatus status;

		// Token: 0x040048DB RID: 18651
		public float completionScale;

		// Token: 0x040048DC RID: 18652
		public float startTime;

		// Token: 0x040048DD RID: 18653
		public float endTime;

		// Token: 0x040048DE RID: 18654
		public Vector3 missionLocation;

		// Token: 0x040048DF RID: 18655
		public float timePassed;

		// Token: 0x040048E0 RID: 18656
		public Dictionary<string, Vector3> missionPoints = new Dictionary<string, Vector3>();

		// Token: 0x040048E1 RID: 18657
		public BaseMission.MissionInstance.ObjectiveStatus[] objectiveStatuses;

		// Token: 0x040048E2 RID: 18658
		public List<MissionEntity> createdEntities;

		// Token: 0x040048E3 RID: 18659
		public ItemAmount[] rewards;

		// Token: 0x02000FE1 RID: 4065
		[Serializable]
		public class ObjectiveStatus
		{
			// Token: 0x0400518C RID: 20876
			public bool started;

			// Token: 0x0400518D RID: 20877
			public bool completed;

			// Token: 0x0400518E RID: 20878
			public bool failed;

			// Token: 0x0400518F RID: 20879
			public int genericInt1;

			// Token: 0x04005190 RID: 20880
			public float genericFloat1;
		}

		// Token: 0x02000FE2 RID: 4066
		public enum ObjectiveType
		{
			// Token: 0x04005192 RID: 20882
			MOVE,
			// Token: 0x04005193 RID: 20883
			KILL
		}
	}

	// Token: 0x02000DAC RID: 3500
	[Serializable]
	public class PositionGenerator
	{
		// Token: 0x06005163 RID: 20835 RVA: 0x001ABCBA File Offset: 0x001A9EBA
		public bool IsDependant()
		{
			return !string.IsNullOrEmpty(this.centerOnPositionIdentifier);
		}

		// Token: 0x06005164 RID: 20836 RVA: 0x001ABCCA File Offset: 0x001A9ECA
		public string GetIdentifier()
		{
			return this.identifier;
		}

		// Token: 0x06005165 RID: 20837 RVA: 0x001ABCD4 File Offset: 0x001A9ED4
		public bool Validate(BasePlayer assignee, BaseMission missionDef)
		{
			Vector3 vector;
			if (this.positionType == BaseMission.PositionGenerator.PositionType.MissionPoint)
			{
				List<MissionPoint> list = Pool.GetList<MissionPoint>();
				bool missionPoints = MissionPoint.GetMissionPoints(ref list, assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, (int)this.Flags, (int)this.ExclusionFlags);
				Pool.FreeList<MissionPoint>(ref list);
				if (!missionPoints)
				{
					Debug.Log("FAILED TO FIND MISSION POINTS");
					return false;
				}
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.WorldPositionGenerator && this.worldPositionGenerator != null && !this.worldPositionGenerator.TrySample(assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, out vector, BaseMission.blockedPoints))
			{
				Debug.Log("FAILED TO GENERATE WORLD POSITION!!!!!");
				return false;
			}
			return true;
		}

		// Token: 0x06005166 RID: 20838 RVA: 0x001ABD80 File Offset: 0x001A9F80
		public Vector3 GetPosition(BasePlayer assignee)
		{
			Vector3 vector;
			if (this.positionType == BaseMission.PositionGenerator.PositionType.MissionPoint)
			{
				List<MissionPoint> list = Pool.GetList<MissionPoint>();
				if (MissionPoint.GetMissionPoints(ref list, assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, (int)this.Flags, (int)this.ExclusionFlags))
				{
					vector = list[UnityEngine.Random.Range(0, list.Count)].GetPosition();
				}
				else
				{
					Debug.LogError("UNABLE TO FIND MISSIONPOINT FOR MISSION!");
					vector = assignee.transform.position;
				}
				Pool.FreeList<MissionPoint>(ref list);
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.WorldPositionGenerator && this.worldPositionGenerator != null)
			{
				if (!this.worldPositionGenerator.TrySample(assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, out vector, BaseMission.blockedPoints))
				{
					Debug.LogError("UNABLE TO FIND WORLD POINT FOR MISSION!");
					vector = assignee.transform.position;
				}
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.DungeonPoint)
			{
				vector = DynamicDungeon.GetNextDungeonPoint();
			}
			else
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				onUnitSphere.y = 0f;
				onUnitSphere.Normalize();
				vector = (this.centerOnPlayer ? assignee.transform.position : assignee.transform.position) + onUnitSphere * UnityEngine.Random.Range(this.minDistForMovePoint, this.maxDistForMovePoint);
				float num = vector.y;
				float num2 = vector.y;
				if (TerrainMeta.WaterMap != null)
				{
					num2 = TerrainMeta.WaterMap.GetHeight(vector);
				}
				if (TerrainMeta.HeightMap != null)
				{
					num = TerrainMeta.HeightMap.GetHeight(vector);
				}
				vector.y = Mathf.Max(num2, num);
			}
			return vector;
		}

		// Token: 0x040048E4 RID: 18660
		public string identifier;

		// Token: 0x040048E5 RID: 18661
		public float minDistForMovePoint;

		// Token: 0x040048E6 RID: 18662
		public float maxDistForMovePoint = 25f;

		// Token: 0x040048E7 RID: 18663
		public bool centerOnProvider;

		// Token: 0x040048E8 RID: 18664
		public bool centerOnPlayer;

		// Token: 0x040048E9 RID: 18665
		public string centerOnPositionIdentifier = "";

		// Token: 0x040048EA RID: 18666
		public BaseMission.PositionGenerator.PositionType positionType;

		// Token: 0x040048EB RID: 18667
		[Header("MissionPoint")]
		[global::InspectorFlags]
		public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

		// Token: 0x040048EC RID: 18668
		[global::InspectorFlags]
		public MissionPoint.MissionPointEnum ExclusionFlags;

		// Token: 0x040048ED RID: 18669
		[Header("WorldPositionGenerator")]
		public WorldPositionGenerator worldPositionGenerator;

		// Token: 0x02000FE3 RID: 4067
		public enum PositionType
		{
			// Token: 0x04005195 RID: 20885
			MissionPoint,
			// Token: 0x04005196 RID: 20886
			WorldPositionGenerator,
			// Token: 0x04005197 RID: 20887
			DungeonPoint
		}
	}

	// Token: 0x02000DAD RID: 3501
	[Serializable]
	public class MissionEntityEntry
	{
		// Token: 0x040048EE RID: 18670
		public GameObjectRef entityRef;

		// Token: 0x040048EF RID: 18671
		public string spawnPositionToUse;

		// Token: 0x040048F0 RID: 18672
		public bool cleanupOnMissionFailed;

		// Token: 0x040048F1 RID: 18673
		public bool cleanupOnMissionSuccess;

		// Token: 0x040048F2 RID: 18674
		public string entityIdentifier;
	}

	// Token: 0x02000DAE RID: 3502
	public enum MissionFailReason
	{
		// Token: 0x040048F4 RID: 18676
		TimeOut,
		// Token: 0x040048F5 RID: 18677
		Disconnect,
		// Token: 0x040048F6 RID: 18678
		ResetPlayerState,
		// Token: 0x040048F7 RID: 18679
		Abandon
	}
}
