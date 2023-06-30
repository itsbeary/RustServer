using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Facepunch.Math;
using Facepunch.Rust;
using Facepunch.Sqlite;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200074D RID: 1869
public class UserPersistance : IDisposable
{
	// Token: 0x06003427 RID: 13351 RVA: 0x001424B4 File Offset: 0x001406B4
	public UserPersistance(string strFolder)
	{
		UserPersistance.blueprints = new Facepunch.Sqlite.Database();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		string text = strFolder + "/player.blueprints.";
		if (activeGameMode != null && activeGameMode.wipeBpsOnProtocol)
		{
			text = text + 239 + ".";
		}
		UserPersistance.blueprints.Open(text + 5 + ".db", false);
		if (!UserPersistance.blueprints.TableExists("data"))
		{
			UserPersistance.blueprints.Execute("CREATE TABLE data ( userid TEXT PRIMARY KEY, info BLOB, updated INTEGER )");
		}
		UserPersistance.deaths = new Facepunch.Sqlite.Database();
		UserPersistance.deaths.Open(string.Concat(new object[] { strFolder, "/player.deaths.", 5, ".db" }), false);
		if (!UserPersistance.deaths.TableExists("data"))
		{
			UserPersistance.deaths.Execute("CREATE TABLE data ( userid TEXT, born INTEGER, died INTEGER, info BLOB )");
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS userindex ON data ( userid )");
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS diedindex ON data ( died )");
		}
		UserPersistance.identities = new Facepunch.Sqlite.Database();
		UserPersistance.identities.Open(string.Concat(new object[] { strFolder, "/player.identities.", 5, ".db" }), false);
		if (!UserPersistance.identities.TableExists("data"))
		{
			UserPersistance.identities.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, username TEXT )");
		}
		UserPersistance.tokens = new Facepunch.Sqlite.Database();
		UserPersistance.tokens.Open(strFolder + "/player.tokens.db", false);
		if (!UserPersistance.tokens.TableExists("data"))
		{
			UserPersistance.tokens.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, token INT, locked BOOLEAN DEFAULT 0 )");
		}
		if (!UserPersistance.tokens.ColumnExists("data", "locked"))
		{
			UserPersistance.tokens.Execute("ALTER TABLE data ADD COLUMN locked BOOLEAN DEFAULT 0");
		}
		UserPersistance.playerState = new Facepunch.Sqlite.Database();
		UserPersistance.playerState.Open(string.Concat(new object[] { strFolder, "/player.states.", 239, ".db" }), false);
		if (!UserPersistance.playerState.TableExists("data"))
		{
			UserPersistance.playerState.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, state BLOB )");
		}
		UserPersistance.nameCache = new Dictionary<ulong, string>();
		UserPersistance.tokenCache = new MruDictionary<ulong, ValueTuple<int, bool>>(500, null);
		UserPersistance.wipeIdCache = new Dictionary<ulong, string>();
	}

	// Token: 0x06003428 RID: 13352 RVA: 0x0014270C File Offset: 0x0014090C
	public virtual void Dispose()
	{
		if (UserPersistance.blueprints != null)
		{
			UserPersistance.blueprints.Close();
			UserPersistance.blueprints = null;
		}
		if (UserPersistance.deaths != null)
		{
			UserPersistance.deaths.Close();
			UserPersistance.deaths = null;
		}
		if (UserPersistance.identities != null)
		{
			UserPersistance.identities.Close();
			UserPersistance.identities = null;
		}
		if (UserPersistance.tokens != null)
		{
			UserPersistance.tokens.Close();
			UserPersistance.tokens = null;
		}
		if (UserPersistance.playerState != null)
		{
			UserPersistance.playerState.Close();
			UserPersistance.playerState = null;
		}
	}

	// Token: 0x06003429 RID: 13353 RVA: 0x0014278C File Offset: 0x0014098C
	public PersistantPlayer GetPlayerInfo(ulong playerID)
	{
		PersistantPlayer persistantPlayer = this.FetchFromDatabase(playerID);
		if (persistantPlayer == null)
		{
			persistantPlayer = Pool.Get<PersistantPlayer>();
		}
		if (persistantPlayer.unlockedItems == null)
		{
			persistantPlayer.unlockedItems = Pool.GetList<int>();
		}
		return persistantPlayer;
	}

	// Token: 0x0600342A RID: 13354 RVA: 0x001427C0 File Offset: 0x001409C0
	private PersistantPlayer FetchFromDatabase(ulong playerID)
	{
		try
		{
			byte[] array = UserPersistance.blueprints.QueryBlob<string>("SELECT info FROM data WHERE userid = ?", playerID.ToString());
			if (array != null)
			{
				return PersistantPlayer.Deserialize(array);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error loading player blueprints: (" + ex.Message + ")");
		}
		return null;
	}

	// Token: 0x0600342B RID: 13355 RVA: 0x00142824 File Offset: 0x00140A24
	public void SetPlayerInfo(ulong playerID, PersistantPlayer info)
	{
		using (TimeWarning.New("SetPlayerInfo", 0))
		{
			byte[] array;
			using (TimeWarning.New("ToProtoBytes", 0))
			{
				array = info.ToProtoBytes();
			}
			UserPersistance.blueprints.Execute<string, byte[], int>("INSERT OR REPLACE INTO data ( userid, info, updated ) VALUES ( ?, ?, ? )", playerID.ToString(), array, Epoch.Current);
		}
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x001428A0 File Offset: 0x00140AA0
	public void AddLifeStory(ulong playerID, PlayerLifeStory lifeStory)
	{
		if (UserPersistance.deaths == null)
		{
			return;
		}
		if (lifeStory == null)
		{
			return;
		}
		using (TimeWarning.New("AddLifeStory", 0))
		{
			byte[] array;
			using (TimeWarning.New("ToProtoBytes", 0))
			{
				array = lifeStory.ToProtoBytes();
			}
			UserPersistance.deaths.Execute<string, int, int, byte[]>("INSERT INTO data ( userid, born, died, info ) VALUES ( ?, ?, ?, ? )", playerID.ToString(), (int)lifeStory.timeBorn, (int)lifeStory.timeDied, array);
		}
	}

	// Token: 0x0600342D RID: 13357 RVA: 0x00142930 File Offset: 0x00140B30
	public PlayerLifeStory GetLastLifeStory(ulong playerID)
	{
		if (UserPersistance.deaths == null)
		{
			return null;
		}
		PlayerLifeStory playerLifeStory2;
		using (TimeWarning.New("GetLastLifeStory", 0))
		{
			try
			{
				byte[] array = UserPersistance.deaths.QueryBlob<string>("SELECT info FROM data WHERE userid = ? ORDER BY died DESC LIMIT 1", playerID.ToString());
				if (array == null)
				{
					return null;
				}
				PlayerLifeStory playerLifeStory = PlayerLifeStory.Deserialize(array);
				playerLifeStory.ShouldPool = false;
				return playerLifeStory;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading lifestory from database: (" + ex.Message + ")");
			}
			playerLifeStory2 = null;
		}
		return playerLifeStory2;
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x001429C8 File Offset: 0x00140BC8
	public string GetPlayerName(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return null;
		}
		string text;
		if (UserPersistance.nameCache.TryGetValue(playerID, out text))
		{
			return text;
		}
		string text2 = UserPersistance.identities.QueryString<ulong>("SELECT username FROM data WHERE userid = ?", playerID);
		UserPersistance.nameCache[playerID] = text2;
		return text2;
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x00142A0C File Offset: 0x00140C0C
	public void SetPlayerName(ulong playerID, string name)
	{
		if (playerID == 0UL || string.IsNullOrEmpty(name))
		{
			return;
		}
		if (string.IsNullOrEmpty(this.GetPlayerName(playerID)))
		{
			UserPersistance.identities.Execute<ulong, string>("INSERT INTO data ( userid, username ) VALUES ( ?, ? )", playerID, name);
		}
		else
		{
			UserPersistance.identities.Execute<string, ulong>("UPDATE data SET username = ? WHERE userid = ?", name, playerID);
		}
		UserPersistance.nameCache[playerID] = name;
	}

	// Token: 0x06003430 RID: 13360 RVA: 0x00142A64 File Offset: 0x00140C64
	public int GetOrGenerateAppToken(ulong playerID, out bool locked)
	{
		if (UserPersistance.tokens == null)
		{
			locked = false;
			return 0;
		}
		int num;
		using (TimeWarning.New("GetOrGenerateAppToken", 0))
		{
			ValueTuple<int, bool> valueTuple;
			if (UserPersistance.tokenCache.TryGetValue(playerID, out valueTuple))
			{
				locked = valueTuple.Item2;
				num = valueTuple.Item1;
			}
			else
			{
				int num2 = UserPersistance.tokens.QueryInt<ulong>("SELECT token FROM data WHERE userid = ?", playerID);
				if (num2 != 0)
				{
					bool flag = UserPersistance.tokens.QueryInt<ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
					UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num2, flag));
					locked = flag;
					num = num2;
				}
				else
				{
					int num3 = UserPersistance.GenerateAppToken();
					UserPersistance.tokens.Execute<ulong, int>("INSERT INTO data ( userid, token ) VALUES ( ?, ? )", playerID, num3);
					UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num3, false));
					locked = false;
					num = num3;
				}
			}
		}
		return num;
	}

	// Token: 0x06003431 RID: 13361 RVA: 0x00142B40 File Offset: 0x00140D40
	public void RegenerateAppToken(ulong playerID)
	{
		if (UserPersistance.tokens == null)
		{
			return;
		}
		using (TimeWarning.New("RegenerateAppToken", 0))
		{
			UserPersistance.tokenCache.Remove(playerID);
			bool flag = UserPersistance.tokens.QueryInt<ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
			int num = UserPersistance.GenerateAppToken();
			UserPersistance.tokens.Execute<ulong, int, bool>("INSERT OR REPLACE INTO data ( userid, token, locked ) VALUES ( ?, ?, ? )", playerID, num, flag);
			UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num, false));
		}
	}

	// Token: 0x06003432 RID: 13362 RVA: 0x00142BC8 File Offset: 0x00140DC8
	private static int GenerateAppToken()
	{
		int num = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		if (num == 0)
		{
			num++;
		}
		return num;
	}

	// Token: 0x06003433 RID: 13363 RVA: 0x00142BF0 File Offset: 0x00140DF0
	public bool SetAppTokenLocked(ulong playerID, bool locked)
	{
		if (UserPersistance.tokens == null)
		{
			return false;
		}
		bool flag;
		this.GetOrGenerateAppToken(playerID, out flag);
		if (flag == locked)
		{
			return false;
		}
		UserPersistance.tokens.Execute<int, ulong>("UPDATE data SET locked = ? WHERE userid = ?", locked ? 1 : 0, playerID);
		UserPersistance.tokenCache.Remove(playerID);
		return true;
	}

	// Token: 0x06003434 RID: 13364 RVA: 0x00142C39 File Offset: 0x00140E39
	public byte[] GetPlayerState(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return null;
		}
		return UserPersistance.playerState.QueryBlob<ulong>("SELECT state FROM data WHERE userid = ?", playerID);
	}

	// Token: 0x06003435 RID: 13365 RVA: 0x00142C50 File Offset: 0x00140E50
	public void SetPlayerState(ulong playerID, byte[] state)
	{
		if (playerID == 0UL || state == null)
		{
			return;
		}
		UserPersistance.playerState.Execute<ulong, byte[]>("INSERT OR REPLACE INTO data ( userid, state ) VALUES ( ?, ? )", playerID, state);
	}

	// Token: 0x06003436 RID: 13366 RVA: 0x00142C6C File Offset: 0x00140E6C
	public string GetUserWipeId(ulong playerID)
	{
		if (playerID <= 10000000UL)
		{
			return null;
		}
		string text;
		if (UserPersistance.wipeIdCache.TryGetValue(playerID, out text))
		{
			return text;
		}
		text = (playerID.ToString() + SaveRestore.WipeId).Sha256().HexString();
		UserPersistance.wipeIdCache[playerID] = text;
		Analytics.Azure.OnPlayerInitializedWipeId(playerID, text);
		return text;
	}

	// Token: 0x06003437 RID: 13367 RVA: 0x00142CC5 File Offset: 0x00140EC5
	public void ResetPlayerState(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return;
		}
		UserPersistance.playerState.Execute<ulong>("DELETE FROM data WHERE userid = ?", playerID);
	}

	// Token: 0x04002A9F RID: 10911
	private static Facepunch.Sqlite.Database blueprints;

	// Token: 0x04002AA0 RID: 10912
	private static Facepunch.Sqlite.Database deaths;

	// Token: 0x04002AA1 RID: 10913
	private static Facepunch.Sqlite.Database identities;

	// Token: 0x04002AA2 RID: 10914
	private static Facepunch.Sqlite.Database tokens;

	// Token: 0x04002AA3 RID: 10915
	private static Facepunch.Sqlite.Database playerState;

	// Token: 0x04002AA4 RID: 10916
	private static Dictionary<ulong, string> nameCache;

	// Token: 0x04002AA5 RID: 10917
	private static Dictionary<ulong, string> wipeIdCache;

	// Token: 0x04002AA6 RID: 10918
	[TupleElementNames(new string[] { "Token", "Locked" })]
	private static MruDictionary<ulong, ValueTuple<int, bool>> tokenCache;
}
