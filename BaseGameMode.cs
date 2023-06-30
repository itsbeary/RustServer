using System;
using System.Collections.Generic;
using System.Linq;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200003D RID: 61
[Serializable]
public class BaseGameMode : global::BaseEntity
{
	// Token: 0x06000391 RID: 913 RVA: 0x0002F224 File Offset: 0x0002D424
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseGameMode.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0002F264 File Offset: 0x0002D464
	public GameMode GetGameScores()
	{
		return this.gameModeScores;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x0002F26C File Offset: 0x0002D46C
	public int ScoreColumnIndex(string scoreName)
	{
		for (int i = 0; i < this.scoreColumns.Length; i++)
		{
			if (this.scoreColumns[i] == scoreName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0002F2A0 File Offset: 0x0002D4A0
	public void InitScores()
	{
		this.gameModeScores = new GameMode();
		this.gameModeScores.scoreColumns = new List<GameMode.ScoreColumn>();
		this.gameModeScores.playerScores = new List<GameMode.PlayerScore>();
		this.gameModeScores.teams = new List<GameMode.TeamInfo>();
		foreach (BaseGameMode.GameModeTeam gameModeTeam in this.teams)
		{
			GameMode.TeamInfo teamInfo = new GameMode.TeamInfo();
			teamInfo.score = 0;
			teamInfo.ShouldPool = false;
			this.gameModeScores.teams.Add(teamInfo);
		}
		foreach (string text in this.scoreColumns)
		{
			GameMode.ScoreColumn scoreColumn = new GameMode.ScoreColumn();
			scoreColumn.name = text;
			scoreColumn.ShouldPool = false;
			this.gameModeScores.scoreColumns.Add(scoreColumn);
		}
		this.gameModeScores.ShouldPool = false;
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0002F378 File Offset: 0x0002D578
	public void CopyGameModeScores(GameMode from, GameMode to)
	{
		to.teams.Clear();
		to.scoreColumns.Clear();
		to.playerScores.Clear();
		foreach (GameMode.TeamInfo teamInfo in from.teams)
		{
			GameMode.TeamInfo teamInfo2 = new GameMode.TeamInfo();
			teamInfo2.score = teamInfo.score;
			to.teams.Add(teamInfo2);
		}
		foreach (GameMode.ScoreColumn scoreColumn in from.scoreColumns)
		{
			GameMode.ScoreColumn scoreColumn2 = new GameMode.ScoreColumn();
			scoreColumn2.name = scoreColumn.name;
			to.scoreColumns.Add(scoreColumn2);
		}
		foreach (GameMode.PlayerScore playerScore in from.playerScores)
		{
			GameMode.PlayerScore playerScore2 = new GameMode.PlayerScore();
			playerScore2.playerName = playerScore.playerName;
			playerScore2.userid = playerScore.userid;
			playerScore2.team = playerScore.team;
			playerScore2.scores = new List<int>();
			foreach (int num in playerScore.scores)
			{
				playerScore2.scores.Add(num);
			}
			to.playerScores.Add(playerScore2);
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0002F53C File Offset: 0x0002D73C
	public GameMode.PlayerScore GetPlayerScoreForPlayer(global::BasePlayer player)
	{
		GameMode.PlayerScore playerScore = null;
		foreach (GameMode.PlayerScore playerScore2 in this.gameModeScores.playerScores)
		{
			if (playerScore2.userid == player.userID)
			{
				playerScore = playerScore2;
				break;
			}
		}
		if (playerScore == null)
		{
			playerScore = new GameMode.PlayerScore();
			playerScore.ShouldPool = false;
			playerScore.playerName = player.displayName;
			playerScore.userid = player.userID;
			playerScore.scores = new List<int>();
			foreach (string text in this.scoreColumns)
			{
				playerScore.scores.Add(0);
			}
			this.gameModeScores.playerScores.Add(playerScore);
		}
		return playerScore;
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0002F610 File Offset: 0x0002D810
	public int GetScoreIndexByName(string name)
	{
		for (int i = 0; i < this.scoreColumns.Length; i++)
		{
			if (this.scoreColumns[i] == name)
			{
				return i;
			}
		}
		Debug.LogWarning("No score colum named : " + name + "returning default");
		return 0;
	}

	// Token: 0x06000398 RID: 920 RVA: 0x0002F658 File Offset: 0x0002D858
	public virtual bool IsDraw()
	{
		if (this.IsTeamGame())
		{
			int num = -1;
			int num2 = 1000000;
			for (int i = 0; i < this.teams.Length; i++)
			{
				int teamScore = this.GetTeamScore(i);
				if (teamScore < num2)
				{
					num2 = teamScore;
				}
				if (teamScore > num)
				{
					num = teamScore;
				}
			}
			return num == num2;
		}
		int num3 = -1;
		int num4 = 0;
		int num5 = this.ScoreColumnIndex(this.victoryScoreName);
		if (num5 != -1)
		{
			for (int j = 0; j < this.gameModeScores.playerScores.Count; j++)
			{
				GameMode.PlayerScore playerScore = this.gameModeScores.playerScores[j];
				if (playerScore.scores[num5] > num3)
				{
					num3 = playerScore.scores[num5];
					num4 = 1;
				}
				else if (playerScore.scores[num5] == num3)
				{
					num4++;
				}
			}
		}
		return num3 == 0 || num4 > 1;
	}

	// Token: 0x06000399 RID: 921 RVA: 0x0002F73C File Offset: 0x0002D93C
	public virtual string GetWinnerName()
	{
		int num = -1;
		int num2 = -1;
		if (this.IsTeamGame())
		{
			for (int i = 0; i < this.teams.Length; i++)
			{
				int teamScore = this.GetTeamScore(i);
				if (teamScore > num)
				{
					num = teamScore;
					num2 = i;
				}
			}
			if (num2 == -1)
			{
				return "NO ONE";
			}
			return this.teams[num2].name;
		}
		else
		{
			int num3 = this.ScoreColumnIndex(this.victoryScoreName);
			if (num3 != -1)
			{
				for (int j = 0; j < this.gameModeScores.playerScores.Count; j++)
				{
					GameMode.PlayerScore playerScore = this.gameModeScores.playerScores[j];
					if (playerScore.scores[num3] > num)
					{
						num = playerScore.scores[num3];
						num2 = j;
					}
				}
			}
			if (num2 != -1)
			{
				return this.gameModeScores.playerScores[num2].playerName;
			}
			return "";
		}
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00007A44 File Offset: 0x00005C44
	public virtual int GetPlayerTeamPosition(global::BasePlayer player)
	{
		return 0;
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0002F81C File Offset: 0x0002DA1C
	public virtual int GetPlayerRank(global::BasePlayer player)
	{
		int num = this.ScoreColumnIndex(this.victoryScoreName);
		if (num == -1)
		{
			return 10;
		}
		int num2 = this.GetPlayerScoreForPlayer(player).scores[num];
		int num3 = 0;
		foreach (GameMode.PlayerScore playerScore in this.gameModeScores.playerScores)
		{
			if (playerScore.scores[num] > num2 && playerScore.userid != player.userID)
			{
				num3++;
			}
		}
		return num3 + 1;
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0002F8C0 File Offset: 0x0002DAC0
	public int GetWinningTeamIndex()
	{
		int num = -1;
		int num2 = -1;
		if (!this.IsTeamGame())
		{
			return -1;
		}
		for (int i = 0; i < this.teams.Length; i++)
		{
			int teamScore = this.GetTeamScore(i);
			if (teamScore > num)
			{
				num = teamScore;
				num2 = i;
			}
		}
		if (num2 == -1)
		{
			return -1;
		}
		return num2;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0002F908 File Offset: 0x0002DB08
	public virtual bool DidPlayerWin(global::BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.IsDraw())
		{
			return false;
		}
		if (this.IsTeamGame())
		{
			GameMode.PlayerScore playerScoreForPlayer = this.GetPlayerScoreForPlayer(player);
			return playerScoreForPlayer.team != -1 && playerScoreForPlayer.team == this.GetWinningTeamIndex();
		}
		return this.GetPlayerRank(player) == 1;
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0002F95E File Offset: 0x0002DB5E
	public bool IsTeamGame()
	{
		return this.teams.Length > 1;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0002F96B File Offset: 0x0002DB6B
	public bool KeepScores()
	{
		return this.scoreColumns.Length != 0;
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x0002F977 File Offset: 0x0002DB77
	public void ModifyTeamScore(int teamIndex, int modifyAmount)
	{
		if (!this.KeepScores())
		{
			return;
		}
		this.gameModeScores.teams[teamIndex].score += modifyAmount;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.CheckGameConditions(false);
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0002F9AE File Offset: 0x0002DBAE
	public void SetTeamScore(int teamIndex, int score)
	{
		this.gameModeScores.teams[teamIndex].score = score;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x0002F9D0 File Offset: 0x0002DBD0
	public virtual void ResetPlayerScores(global::BasePlayer player)
	{
		if (base.isClient)
		{
			return;
		}
		for (int i = 0; i < this.scoreColumns.Length; i++)
		{
			this.SetPlayerGameScore(player, i, 0);
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0002FA04 File Offset: 0x0002DC04
	public void ModifyPlayerGameScore(global::BasePlayer player, string scoreName, int modifyAmount)
	{
		if (!this.KeepScores())
		{
			return;
		}
		int scoreIndexByName = this.GetScoreIndexByName(scoreName);
		this.ModifyPlayerGameScore(player, scoreIndexByName, modifyAmount);
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x0002FA2C File Offset: 0x0002DC2C
	public void ModifyPlayerGameScore(global::BasePlayer player, int scoreIndex, int modifyAmount)
	{
		if (!this.KeepScores())
		{
			return;
		}
		this.GetPlayerScoreForPlayer(player);
		int playerGameScore = this.GetPlayerGameScore(player, scoreIndex);
		if (this.IsTeamGame() && player.gamemodeteam >= 0 && scoreIndex == this.GetScoreIndexByName(this.teamScoreName))
		{
			this.gameModeScores.teams[player.gamemodeteam].score = this.gameModeScores.teams[player.gamemodeteam].score + modifyAmount;
		}
		this.SetPlayerGameScore(player, scoreIndex, playerGameScore + modifyAmount);
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x0002FAB6 File Offset: 0x0002DCB6
	public int GetPlayerGameScore(global::BasePlayer player, int scoreIndex)
	{
		return this.GetPlayerScoreForPlayer(player).scores[scoreIndex];
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0002FACA File Offset: 0x0002DCCA
	public void SetPlayerTeam(global::BasePlayer player, int newTeam)
	{
		player.gamemodeteam = newTeam;
		this.GetPlayerScoreForPlayer(player).team = newTeam;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0002FAE7 File Offset: 0x0002DCE7
	public void SetPlayerGameScore(global::BasePlayer player, int scoreIndex, int scoreValue)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.KeepScores())
		{
			return;
		}
		this.GetPlayerScoreForPlayer(player).scores[scoreIndex] = scoreValue;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.CheckGameConditions(false);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0002FB1C File Offset: 0x0002DD1C
	public int GetMaxBeds(global::BasePlayer player)
	{
		return this.maximumSleepingBags;
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0002FB24 File Offset: 0x0002DD24
	protected virtual void SetupTags()
	{
		this.gameModeTags.Add("missions-" + (this.missionSystem ? "enabled" : "disabled"));
		this.gameModeTags.Add("mlrs-" + (this.mlrs ? "enabled" : "disabled"));
		this.gameModeTags.Add("map-" + (this.ingameMap ? "enabled" : "disabled"));
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0002FBAC File Offset: 0x0002DDAC
	public virtual BaseGameMode.ResearchCostResult GetScrapCostForResearch(ItemDefinition item, global::ResearchTable.ResearchType researchType)
	{
		return default(BaseGameMode.ResearchCostResult);
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0002FBC4 File Offset: 0x0002DDC4
	public virtual float? EvaluateSleepingBagReset(global::SleepingBag bag, Vector3 position, global::SleepingBag.SleepingBagResetReason reason)
	{
		return null;
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0002FBDC File Offset: 0x0002DDDC
	private void DeleteEntities()
	{
		if (!SingletonComponent<ServerMgr>.Instance.runFrameUpdate)
		{
			base.Invoke(new Action(this.DeleteEntities), 5f);
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments.Where((MonumentInfo x) => x.IsSafeZone).ToArray<MonumentInfo>())
		{
			List<global::BaseEntity> list = new List<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(new OBB(monumentInfo.transform, monumentInfo.Bounds), list, -1, QueryTriggerInteraction.Collide);
			foreach (global::BaseEntity baseEntity in list)
			{
				if (!this.safeZone && (baseEntity is HumanNPC || baseEntity is NPCAutoTurret || baseEntity is Marketplace))
				{
					baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
			if (!this.safeZone)
			{
				NPCSpawner[] componentsInChildren = monumentInfo.GetComponentsInChildren<NPCSpawner>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].isSpawnerActive = false;
				}
			}
			if (!this.mlrs)
			{
				foreach (IndividualSpawner individualSpawner in monumentInfo.GetComponentsInChildren<IndividualSpawner>())
				{
					if (individualSpawner.entityPrefab.isValid && individualSpawner.entityPrefab.GetEntity() is global::MLRS)
					{
						individualSpawner.isSpawnerActive = false;
					}
				}
			}
		}
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			if (!this.mlrs && baseNetworkable is global::MLRS)
			{
				baseNetworkable.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			if (!this.missionSystem && baseNetworkable is NPCMissionProvider)
			{
				baseNetworkable.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0002FDCC File Offset: 0x0002DFCC
	protected void OnCreated_Vanilla()
	{
		if (this.rustPlus != CompanionServer.Server.IsEnabled)
		{
			if (this.rustPlus)
			{
				CompanionServer.Server.Initialize();
			}
			else
			{
				CompanionServer.Server.Shutdown();
			}
		}
		if (!this.teamSystem)
		{
			global::RelationshipManager.maxTeamSize = 0;
		}
		ConVar.Server.max_sleeping_bags = this.maximumSleepingBags;
		ConVar.Server.crawlingenabled = this.crawling;
		this.DeleteEntities();
		if (this.wipeBpsOnProtocol)
		{
			SingletonComponent<ServerMgr>.Instance.persistance.Dispose();
			SingletonComponent<ServerMgr>.Instance.persistance = new UserPersistance(ConVar.Server.rootFolder);
			global::BasePlayer[] array = UnityEngine.Object.FindObjectsOfType<global::BasePlayer>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].InvalidateCachedPeristantPlayer();
			}
		}
		global::RelationshipManager.contacts = this.contactSystem;
		Chat.globalchat = this.globalChat;
		Chat.localchat = this.localChat;
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060003AE RID: 942 RVA: 0x0002FE8C File Offset: 0x0002E08C
	// (remove) Token: 0x060003AF RID: 943 RVA: 0x0002FEC0 File Offset: 0x0002E0C0
	public static event Action<BaseGameMode> GameModeChanged;

	// Token: 0x060003B0 RID: 944 RVA: 0x0002FEF4 File Offset: 0x0002E0F4
	public bool HasAnyGameModeTag(string[] tags)
	{
		for (int i = 0; i < this.gameModeTags.Count; i++)
		{
			for (int j = 0; j < tags.Length; j++)
			{
				if (tags[j] == this.gameModeTags[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0002FF40 File Offset: 0x0002E140
	public bool HasGameModeTag(string tag)
	{
		for (int i = 0; i < this.gameModeTags.Count; i++)
		{
			if (this.gameModeTags[i] == tag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0002FF7A File Offset: 0x0002E17A
	public bool AllowsSleeping()
	{
		return this.allowSleeping;
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0002FF82 File Offset: 0x0002E182
	public bool HasLoadouts()
	{
		return this.loadouts.Length != 0 || (this.IsTeamGame() && this.teams[0].teamloadouts.Length != 0);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0002FFAA File Offset: 0x0002E1AA
	public int GetNumTeams()
	{
		if (this.teams.Length > 1)
		{
			return this.teams.Length;
		}
		return 1;
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0002FFC1 File Offset: 0x0002E1C1
	public int GetTeamScore(int teamIndex)
	{
		return this.gameModeScores.teams[teamIndex].score;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0002FFDC File Offset: 0x0002E1DC
	public static void CreateGameMode(string overrideMode = "")
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode)
		{
			activeGameMode.ShutdownGame();
			activeGameMode.Kill(global::BaseNetworkable.DestroyMode.None);
			BaseGameMode.SetActiveGameMode(null, true);
		}
		string text = ConVar.Server.gamemode;
		Debug.Log("Gamemode Convar :" + text);
		if (!string.IsNullOrEmpty(overrideMode))
		{
			text = overrideMode;
		}
		if (string.IsNullOrEmpty(text))
		{
			Debug.Log("No Gamemode.");
			if (BaseGameMode.GameModeChanged != null)
			{
				BaseGameMode.GameModeChanged(null);
				return;
			}
		}
		else
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/gamemodes/" + text + ".prefab", Vector3.zero, Quaternion.identity, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
				return;
			}
			Debug.Log("Failed to create gamemode : " + text);
		}
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00030096 File Offset: 0x0002E296
	public static void SetActiveGameMode(BaseGameMode newActive, bool serverside)
	{
		if (newActive)
		{
			newActive.InitScores();
		}
		if (BaseGameMode.GameModeChanged != null)
		{
			BaseGameMode.GameModeChanged(newActive);
		}
		if (serverside)
		{
			BaseGameMode.svActiveGameMode = newActive;
			return;
		}
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x000300C2 File Offset: 0x0002E2C2
	public static BaseGameMode GetActiveGameMode(bool serverside)
	{
		return BaseGameMode.svActiveGameMode;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x000300C9 File Offset: 0x0002E2C9
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.gameMode != null)
		{
			this.CopyGameModeScores(info.msg.gameMode, this.gameModeScores);
		}
	}

	// Token: 0x060003BA RID: 954 RVA: 0x000300F8 File Offset: 0x0002E2F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.gameMode = Facepunch.Pool.Get<GameMode>();
		info.msg.gameMode.scoreColumns = Facepunch.Pool.GetList<GameMode.ScoreColumn>();
		info.msg.gameMode.playerScores = Facepunch.Pool.GetList<GameMode.PlayerScore>();
		info.msg.gameMode.teams = Facepunch.Pool.GetList<GameMode.TeamInfo>();
		this.CopyGameModeScores(this.gameModeScores, info.msg.gameMode);
		info.msg.gameMode.ShouldPool = true;
	}

	// Token: 0x060003BB RID: 955 RVA: 0x00030183 File Offset: 0x0002E383
	public virtual float CorpseRemovalTime(BaseCorpse corpse)
	{
		return ConVar.Server.corpsedespawn;
	}

	// Token: 0x060003BC RID: 956 RVA: 0x000233C8 File Offset: 0x000215C8
	public virtual bool InWarmup()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0003018A File Offset: 0x0002E38A
	public virtual bool IsWaitingForPlayers()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0000564C File Offset: 0x0000384C
	public virtual bool IsMatchOver()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00030197 File Offset: 0x0002E397
	public virtual bool IsMatchActive()
	{
		return !this.InWarmup() && !this.IsWaitingForPlayers() && !this.IsMatchOver() && this.matchStartTime != -1f;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000301C4 File Offset: 0x0002E3C4
	public override void InitShared()
	{
		base.InitShared();
		if (BaseGameMode.GetActiveGameMode(base.isServer) != null && BaseGameMode.GetActiveGameMode(base.isServer) != this)
		{
			Debug.LogError("Already an active game mode! was : " + BaseGameMode.GetActiveGameMode(base.isServer).name);
			UnityEngine.Object.Destroy(BaseGameMode.GetActiveGameMode(base.isServer).gameObject);
		}
		this.SetupTags();
		BaseGameMode.SetActiveGameMode(this, base.isServer);
		this.OnCreated();
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00030249 File Offset: 0x0002E449
	public override void DestroyShared()
	{
		if (BaseGameMode.GetActiveGameMode(base.isServer) == this)
		{
			BaseGameMode.SetActiveGameMode(null, base.isServer);
		}
		base.DestroyShared();
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x00030270 File Offset: 0x0002E470
	protected virtual void OnCreated()
	{
		this.OnCreated_Vanilla();
		if (base.isServer)
		{
			foreach (string text in this.convars)
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, text, Array.Empty<object>());
			}
			this.gameModeSpawnGroups = UnityEngine.Object.FindObjectsOfType<GameModeSpawnGroup>();
			this.UnassignAllPlayers();
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				this.AutoAssignTeam(basePlayer);
			}
			this.InstallSpawnpoints();
			this.ResetMatch();
		}
		Debug.Log("Game created! type was : " + base.name);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00030354 File Offset: 0x0002E554
	protected virtual void OnMatchBegin()
	{
		this.matchStartTime = UnityEngine.Time.realtimeSinceStartup;
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0003038C File Offset: 0x0002E58C
	public virtual void ResetMatch()
	{
		if (this.IsWaitingForPlayers())
		{
			return;
		}
		BaseGameMode.isResetting = true;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		this.ResetTeamScores();
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.ResetPlayerScores(basePlayer);
			basePlayer.Hurt(100000f, DamageType.Suicide, null, false);
			basePlayer.Respawn();
		}
		GameModeSpawnGroup[] array = this.gameModeSpawnGroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetSpawnGroup();
		}
		this.matchStartTime = -1f;
		base.Invoke(new Action(this.OnMatchBegin), this.warmupDuration);
		BaseGameMode.isResetting = false;
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0003046C File Offset: 0x0002E66C
	public virtual void ResetTeamScores()
	{
		for (int i = 0; i < this.teams.Length; i++)
		{
			this.SetTeamScore(i, 0);
		}
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00030494 File Offset: 0x0002E694
	public virtual void ShutdownGame()
	{
		this.ResetTeamScores();
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.SetPlayerTeam(basePlayer, -1);
		}
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x000304F0 File Offset: 0x0002E6F0
	private void Update()
	{
		if (base.isClient)
		{
			return;
		}
		this.OnThink(UnityEngine.Time.deltaTime);
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00030508 File Offset: 0x0002E708
	protected virtual void OnThink(float delta)
	{
		if (this.matchStartTime != -1f)
		{
			float num = UnityEngine.Time.realtimeSinceStartup - this.matchStartTime;
			if (this.IsMatchActive() && this.matchDuration > 0f && num >= this.matchDuration)
			{
				this.OnMatchEnd();
			}
		}
		int num2 = 0;
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (this.autoHealDelay > 0f && basePlayer.healthFraction < 1f && basePlayer.IsAlive() && !basePlayer.IsWounded() && basePlayer.SecondsSinceAttacked >= this.autoHealDelay)
			{
				basePlayer.Heal(basePlayer.MaxHealth() * delta / this.autoHealDuration);
			}
			if (basePlayer.IsConnected)
			{
				num2++;
			}
		}
		if (num2 >= this.minPlayersToStart || this.IsWaitingForPlayers())
		{
			if (this.IsWaitingForPlayers() && num2 >= this.minPlayersToStart)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
				base.CancelInvoke(new Action(this.ResetMatch));
				this.ResetMatch();
			}
			return;
		}
		if (this.IsMatchActive())
		{
			this.OnMatchEnd();
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00030670 File Offset: 0x0002E870
	public virtual void OnMatchEnd()
	{
		this.matchEndTime = UnityEngine.Time.time;
		Debug.Log("Match over!");
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		base.Invoke(new Action(this.ResetMatch), this.timeBetweenMatches);
	}

	// Token: 0x060003CA RID: 970 RVA: 0x000306AE File Offset: 0x0002E8AE
	public virtual void OnNewPlayer(global::BasePlayer player)
	{
		player.Respawn();
		if (!this.AllowsSleeping())
		{
			player.EndSleeping();
			player.SendNetworkUpdateImmediate(false);
		}
		this.PostPlayerRespawn(player);
	}

	// Token: 0x060003CB RID: 971 RVA: 0x000306D2 File Offset: 0x0002E8D2
	public void PostPlayerRespawn(global::BasePlayer player)
	{
		if (this.startHealthOverride > 0f)
		{
			player.SetMaxHealth(this.startHealthOverride);
			player.health = this.startHealthOverride;
		}
	}

	// Token: 0x060003CC RID: 972 RVA: 0x000306F9 File Offset: 0x0002E8F9
	public virtual void OnPlayerConnected(global::BasePlayer player)
	{
		this.AutoAssignTeam(player);
		this.ResetPlayerScores(player);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0003070C File Offset: 0x0002E90C
	public virtual void UnassignAllPlayers()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.SetPlayerTeam(basePlayer, -1);
		}
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00030760 File Offset: 0x0002E960
	public void AutoAssignTeam(global::BasePlayer player)
	{
		int num = 0;
		int[] array = new int[this.teams.Length];
		int num2 = UnityEngine.Random.Range(0, this.teams.Length);
		int num3 = 0;
		if (this.teams.Length > 1)
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (basePlayer.gamemodeteam >= 0 && basePlayer.gamemodeteam < this.teams.Length)
				{
					array[basePlayer.gamemodeteam]++;
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < num3)
				{
					num2 = i;
				}
			}
			num = num2;
		}
		this.SetPlayerTeam(player, num);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0003082C File Offset: 0x0002EA2C
	public virtual void OnPlayerDisconnected(global::BasePlayer player)
	{
		if (this.gameModeScores == null)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		GameMode.PlayerScore playerScore = null;
		foreach (GameMode.PlayerScore playerScore2 in this.gameModeScores.playerScores)
		{
			if (playerScore2.userid == player.userID)
			{
				playerScore = playerScore2;
				break;
			}
		}
		if (playerScore != null)
		{
			this.gameModeScores.playerScores.Remove(playerScore);
		}
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPlayerWounded(global::BasePlayer instigator, global::BasePlayer victim, HitInfo info)
	{
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPlayerRevived(global::BasePlayer instigator, global::BasePlayer victim)
	{
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x000308B8 File Offset: 0x0002EAB8
	public virtual void OnPlayerHurt(global::BasePlayer instigator, global::BasePlayer victim, HitInfo deathInfo = null)
	{
		if (!this.allowBleeding && victim.metabolism.bleeding.value != 0f)
		{
			victim.metabolism.bleeding.value = 0f;
			victim.metabolism.SendChangesToClient();
		}
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00030904 File Offset: 0x0002EB04
	public virtual void OnPlayerDeath(global::BasePlayer instigator, global::BasePlayer victim, HitInfo deathInfo = null)
	{
		if (!this.IsMatchActive())
		{
			return;
		}
		if (victim != null && victim.IsConnected && !victim.IsNpc)
		{
			this.ModifyPlayerGameScore(victim, "deaths", 1);
		}
		bool flag = this.IsTeamGame() && instigator != null && victim != null && instigator.gamemodeteam == victim.gamemodeteam;
		if (instigator != null && victim != instigator && !flag && !instigator.IsNpc)
		{
			this.ModifyPlayerGameScore(instigator, "kills", 1);
		}
		if (instigator != null && instigator.IsConnected && !instigator.IsNpc && instigator != victim)
		{
			base.ClientRPCPlayer<string, int, bool>(null, instigator, "RPC_ScoreSplash", victim.displayName, 100, true);
		}
		if (this.hasKillFeed && instigator != null && victim != null && deathInfo.Weapon != null && deathInfo.Weapon.GetItem() != null)
		{
			string text = Vector3.Distance(instigator.transform.position, victim.transform.position).ToString("N0") + "m";
			string text2 = " with a " + deathInfo.Weapon.GetItem().info.displayName.translated + " from " + text;
			string text3 = "You Killed " + victim.displayName + text2;
			string text4 = instigator.displayName + " Killed You" + text2;
			string text5 = instigator.displayName + " Killed" + victim.displayName + text2;
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (basePlayer == instigator)
				{
					basePlayer.ChatMessage(text3);
				}
				else if (basePlayer == victim)
				{
					basePlayer.ChatMessage(text4);
				}
				else if (global::BasePlayer.activePlayerList.Count <= 5)
				{
					basePlayer.ChatMessage(text5);
				}
			}
		}
		this.CheckGameConditions(true);
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00030B38 File Offset: 0x0002ED38
	public virtual bool CanPlayerRespawn(global::BasePlayer player)
	{
		return !this.IsMatchOver() || this.IsWaitingForPlayers() || BaseGameMode.isResetting;
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00030B51 File Offset: 0x0002ED51
	public virtual void OnPlayerRespawn(global::BasePlayer player)
	{
		if (!this.AllowsSleeping())
		{
			player.EndSleeping();
			player.MarkRespawn(this.respawnDelayOverride);
			base.SendNetworkUpdateImmediate(false);
		}
		this.PostPlayerRespawn(player);
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00030B7C File Offset: 0x0002ED7C
	public virtual void CheckGameConditions(bool force = false)
	{
		if (!this.IsMatchActive())
		{
			return;
		}
		if (this.IsTeamGame())
		{
			for (int i = 0; i < this.teams.Length; i++)
			{
				if (this.GetTeamScore(i) >= this.numScoreForVictory)
				{
					this.OnMatchEnd();
				}
			}
			return;
		}
		int num = this.ScoreColumnIndex(this.victoryScoreName);
		if (num != -1)
		{
			using (List<GameMode.PlayerScore>.Enumerator enumerator = this.gameModeScores.playerScores.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.scores[num] >= this.numScoreForVictory)
					{
						this.OnMatchEnd();
					}
				}
			}
		}
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00030C30 File Offset: 0x0002EE30
	public virtual void LoadoutPlayer(global::BasePlayer player)
	{
		PlayerInventoryProperties playerInventoryProperties;
		if (this.IsTeamGame())
		{
			if (player.gamemodeteam == -1)
			{
				Debug.LogWarning("Player loading out without team assigned, auto assigning!");
				this.AutoAssignTeam(player);
			}
			playerInventoryProperties = this.teams[player.gamemodeteam].teamloadouts[SeedRandom.Range((uint)player.userID, 0, this.teams[player.gamemodeteam].teamloadouts.Length)];
		}
		else if (this.useStaticLoadoutPerPlayer)
		{
			playerInventoryProperties = this.loadouts[SeedRandom.Range((uint)player.userID, 0, this.loadouts.Length)];
		}
		else
		{
			playerInventoryProperties = this.loadouts[UnityEngine.Random.Range(0, this.loadouts.Length)];
		}
		if (playerInventoryProperties)
		{
			playerInventoryProperties.GiveToPlayer(player);
		}
		else
		{
			player.inventory.GiveItem(ItemManager.CreateByName("hazmatsuit", 1, 0UL), player.inventory.containerWear, false);
		}
		if (this.topUpMagazines)
		{
			foreach (global::Item item in player.inventory.containerBelt.itemList)
			{
				global::BaseEntity heldEntity = item.GetHeldEntity();
				if (heldEntity != null)
				{
					global::BaseProjectile component = heldEntity.GetComponent<global::BaseProjectile>();
					if (component != null)
					{
						component.TopUpAmmo();
					}
				}
			}
		}
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00030D80 File Offset: 0x0002EF80
	public virtual void InstallSpawnpoints()
	{
		this.allspawns = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (this.allspawns != null)
		{
			Debug.Log("Installed : " + this.allspawns.Length + "spawn points.");
		}
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00030DBC File Offset: 0x0002EFBC
	public virtual global::BasePlayer.SpawnPoint GetPlayerSpawn(global::BasePlayer forPlayer)
	{
		if (this.allspawns == null)
		{
			this.InstallSpawnpoints();
		}
		float num = 0f;
		int num2 = UnityEngine.Random.Range(0, this.allspawns.Length);
		if (this.allspawns.Length != 0 && forPlayer != null)
		{
			for (int i = 0; i < this.allspawns.Length; i++)
			{
				GameObject gameObject = this.allspawns[i];
				float num3 = 0f;
				for (int j = 0; j < global::BasePlayer.activePlayerList.Count; j++)
				{
					global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[j];
					if (!(basePlayer == null) && basePlayer.IsAlive() && !(basePlayer == forPlayer))
					{
						float num4 = Vector3.Distance(basePlayer.transform.position, gameObject.transform.position);
						num3 -= 100f * (1f - Mathf.InverseLerp(8f, 16f, num4));
						if (!this.IsTeamGame() || basePlayer.gamemodeteam != forPlayer.gamemodeteam)
						{
							num3 += 100f * Mathf.InverseLerp(16f, 32f, num4);
						}
					}
				}
				float num5 = Vector3.Distance((forPlayer.ServerCurrentDeathNote == null) ? this.allspawns[UnityEngine.Random.Range(0, this.allspawns.Length)].transform.position : forPlayer.ServerCurrentDeathNote.worldPosition, gameObject.transform.position);
				float num6 = Mathf.InverseLerp(8f, 25f, num5);
				num3 *= num6;
				if (num3 > num)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		GameObject gameObject2 = this.allspawns[num2];
		return new global::BasePlayer.SpawnPoint
		{
			pos = gameObject2.transform.position,
			rot = gameObject2.transform.rotation
		};
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00030F8A File Offset: 0x0002F18A
	public virtual int GetMaxRelationshipTeamSize()
	{
		return global::RelationshipManager.maxTeamSize;
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00030F91 File Offset: 0x0002F191
	public virtual global::SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return global::SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanMoveItemsFrom(global::PlayerInventory inv, global::BaseEntity source, global::Item item)
	{
		return true;
	}

	// Token: 0x04000291 RID: 657
	private GameMode gameModeScores;

	// Token: 0x04000292 RID: 658
	public string[] scoreColumns;

	// Token: 0x04000293 RID: 659
	[Header("Vanilla")]
	public bool globalChat = true;

	// Token: 0x04000294 RID: 660
	public bool localChat;

	// Token: 0x04000295 RID: 661
	public bool teamSystem = true;

	// Token: 0x04000296 RID: 662
	public bool safeZone = true;

	// Token: 0x04000297 RID: 663
	public bool ingameMap = true;

	// Token: 0x04000298 RID: 664
	public bool compass = true;

	// Token: 0x04000299 RID: 665
	public bool contactSystem = true;

	// Token: 0x0400029A RID: 666
	public bool crawling = true;

	// Token: 0x0400029B RID: 667
	public bool rustPlus = true;

	// Token: 0x0400029C RID: 668
	public bool wipeBpsOnProtocol;

	// Token: 0x0400029D RID: 669
	public int maximumSleepingBags = -1;

	// Token: 0x0400029E RID: 670
	public bool returnValidCombatlog = true;

	// Token: 0x0400029F RID: 671
	public bool missionSystem = true;

	// Token: 0x040002A0 RID: 672
	public bool mlrs = true;

	// Token: 0x040002A1 RID: 673
	public const global::BaseEntity.Flags Flag_Warmup = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040002A2 RID: 674
	public const global::BaseEntity.Flags Flag_GameOver = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040002A3 RID: 675
	public const global::BaseEntity.Flags Flag_WaitingForPlayers = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040002A4 RID: 676
	[Header("Changelog")]
	public Translate.Phrase[] addedFeatures;

	// Token: 0x040002A5 RID: 677
	public Translate.Phrase[] removedFeatures;

	// Token: 0x040002A6 RID: 678
	public Translate.Phrase[] changedFeatures;

	// Token: 0x040002A7 RID: 679
	public List<string> convars = new List<string>();

	// Token: 0x040002A9 RID: 681
	public string shortname = "vanilla";

	// Token: 0x040002AA RID: 682
	public float matchDuration = -1f;

	// Token: 0x040002AB RID: 683
	public float warmupDuration = 10f;

	// Token: 0x040002AC RID: 684
	public float timeBetweenMatches = 10f;

	// Token: 0x040002AD RID: 685
	public int minPlayersToStart = 1;

	// Token: 0x040002AE RID: 686
	public bool useCustomSpawns = true;

	// Token: 0x040002AF RID: 687
	public string victoryScoreName = "kills";

	// Token: 0x040002B0 RID: 688
	public string teamScoreName = "kills";

	// Token: 0x040002B1 RID: 689
	public int numScoreForVictory = 10;

	// Token: 0x040002B2 RID: 690
	public string gamemodeTitle;

	// Token: 0x040002B3 RID: 691
	public SoundDefinition[] warmupMusics;

	// Token: 0x040002B4 RID: 692
	public SoundDefinition[] lossMusics;

	// Token: 0x040002B5 RID: 693
	public SoundDefinition[] winMusics;

	// Token: 0x040002B6 RID: 694
	[NonSerialized]
	private float warmupStartTime;

	// Token: 0x040002B7 RID: 695
	[NonSerialized]
	private float matchStartTime = -1f;

	// Token: 0x040002B8 RID: 696
	[NonSerialized]
	private float matchEndTime;

	// Token: 0x040002B9 RID: 697
	public List<string> gameModeTags;

	// Token: 0x040002BA RID: 698
	public global::BasePlayer.CameraMode deathCameraMode = global::BasePlayer.CameraMode.Eyes;

	// Token: 0x040002BB RID: 699
	public bool permanent = true;

	// Token: 0x040002BC RID: 700
	public bool limitTeamAuths;

	// Token: 0x040002BD RID: 701
	public bool allowSleeping = true;

	// Token: 0x040002BE RID: 702
	public bool allowWounding = true;

	// Token: 0x040002BF RID: 703
	public bool allowBleeding = true;

	// Token: 0x040002C0 RID: 704
	public bool allowTemperature = true;

	// Token: 0x040002C1 RID: 705
	public bool quickRespawn;

	// Token: 0x040002C2 RID: 706
	public bool quickDeploy;

	// Token: 0x040002C3 RID: 707
	public float respawnDelayOverride = 5f;

	// Token: 0x040002C4 RID: 708
	public float startHealthOverride;

	// Token: 0x040002C5 RID: 709
	public float autoHealDelay;

	// Token: 0x040002C6 RID: 710
	public float autoHealDuration = 1f;

	// Token: 0x040002C7 RID: 711
	public bool hasKillFeed;

	// Token: 0x040002C8 RID: 712
	public bool allowPings = true;

	// Token: 0x040002C9 RID: 713
	public static BaseGameMode svActiveGameMode = null;

	// Token: 0x040002CA RID: 714
	public static List<BaseGameMode> svGameModeManifest = new List<BaseGameMode>();

	// Token: 0x040002CB RID: 715
	[NonSerialized]
	private GameObject[] allspawns;

	// Token: 0x040002CC RID: 716
	[NonSerialized]
	private GameModeSpawnGroup[] gameModeSpawnGroups;

	// Token: 0x040002CD RID: 717
	public PlayerInventoryProperties[] loadouts;

	// Token: 0x040002CE RID: 718
	[Tooltip("Use steamID to always pick the same loadout per player")]
	public bool useStaticLoadoutPerPlayer;

	// Token: 0x040002CF RID: 719
	public bool topUpMagazines;

	// Token: 0x040002D0 RID: 720
	public bool sendKillNotifications;

	// Token: 0x040002D1 RID: 721
	public BaseGameMode.GameModeTeam[] teams;

	// Token: 0x040002D2 RID: 722
	public float corpseRemovalTimeOverride;

	// Token: 0x040002D3 RID: 723
	private static bool isResetting = false;

	// Token: 0x02000B96 RID: 2966
	public struct ResearchCostResult
	{
		// Token: 0x04004028 RID: 16424
		public float? Scale;

		// Token: 0x04004029 RID: 16425
		public int? Amount;
	}

	// Token: 0x02000B97 RID: 2967
	[Serializable]
	public class GameModeTeam
	{
		// Token: 0x0400402A RID: 16426
		public string name;

		// Token: 0x0400402B RID: 16427
		public PlayerInventoryProperties[] teamloadouts;
	}
}
