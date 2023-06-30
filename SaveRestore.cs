using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Math;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000569 RID: 1385
public class SaveRestore : SingletonComponent<SaveRestore>
{
	// Token: 0x06002AA0 RID: 10912 RVA: 0x00103B28 File Offset: 0x00101D28
	public static void ClearMapEntities()
	{
		global::BaseEntity[] array = UnityEngine.Object.FindObjectsOfType<global::BaseEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log("Destroying " + array.Length + " old entities", StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(global::BaseNetworkable.DestroyMode.None);
				if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[]
					{
						"\t",
						i + 1,
						" / ",
						array.Length
					}), StackTraceLogType.None);
				}
			}
			ItemManager.Heartbeat();
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x00103BE8 File Offset: 0x00101DE8
	public static bool Load(string strFilename = "", bool allowOutOfDateSaves = false)
	{
		SaveRestore.SaveCreatedTime = DateTime.UtcNow;
		bool flag;
		try
		{
			if (strFilename == "")
			{
				strFilename = global::World.SaveFolderName + "/" + global::World.SaveFileName;
			}
			if (!File.Exists(strFilename))
			{
				if (!File.Exists("TestSaves/" + strFilename))
				{
					UnityEngine.Debug.LogWarning("Couldn't load " + strFilename + " - file doesn't exist");
					return false;
				}
				strFilename = "TestSaves/" + strFilename;
			}
			Dictionary<global::BaseEntity, ProtoBuf.Entity> dictionary = new Dictionary<global::BaseEntity, ProtoBuf.Entity>();
			using (FileStream fileStream = File.OpenRead(strFilename))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					SaveRestore.SaveCreatedTime = File.GetCreationTime(strFilename);
					if (binaryReader.ReadSByte() != 83 || binaryReader.ReadSByte() != 65 || binaryReader.ReadSByte() != 86 || binaryReader.ReadSByte() != 82)
					{
						UnityEngine.Debug.LogWarning("Invalid save (missing header)");
						return false;
					}
					if (binaryReader.PeekChar() == 74)
					{
						binaryReader.ReadChar();
						SaveRestore.WipeId = JsonConvert.DeserializeObject<SaveRestore.SaveExtraData>(binaryReader.ReadString()).WipeId;
					}
					if (binaryReader.PeekChar() == 68)
					{
						binaryReader.ReadChar();
						SaveRestore.SaveCreatedTime = Epoch.ToDateTime((long)binaryReader.ReadInt32());
					}
					if (binaryReader.ReadUInt32() != 239U)
					{
						if (allowOutOfDateSaves)
						{
							UnityEngine.Debug.LogWarning("This save is from an older (possibly incompatible) version!");
						}
						else
						{
							UnityEngine.Debug.LogWarning("This save is from an older version. It might not load properly.");
						}
					}
					SaveRestore.ClearMapEntities();
					Assert.IsTrue(global::BaseEntity.saveList.Count == 0, "BaseEntity.saveList isn't empty!");
					Network.Net.sv.Reset();
					Rust.Application.isLoadingSave = true;
					HashSet<NetworkableId> hashSet = new HashSet<NetworkableId>();
					while (fileStream.Position < fileStream.Length)
					{
						RCon.Update();
						uint num = binaryReader.ReadUInt32();
						long position = fileStream.Position;
						ProtoBuf.Entity entData = null;
						try
						{
							entData = ProtoBuf.Entity.DeserializeLength(fileStream, (int)num);
						}
						catch (Exception ex)
						{
							UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Skipping entity since it could not be deserialized - stream position: ", position, " size: ", num }));
							UnityEngine.Debug.LogException(ex);
							fileStream.Position = position + (long)((ulong)num);
							continue;
						}
						if (entData.basePlayer != null && dictionary.Any((KeyValuePair<global::BaseEntity, ProtoBuf.Entity> x) => x.Value.basePlayer != null && x.Value.basePlayer.userid == entData.basePlayer.userid))
						{
							UnityEngine.Debug.LogWarning(string.Concat(new object[]
							{
								"Skipping entity ",
								entData.baseNetworkable.uid,
								" - it's a player ",
								entData.basePlayer.userid,
								" who is in the save multiple times"
							}));
						}
						else if (entData.baseNetworkable.uid.IsValid && hashSet.Contains(entData.baseNetworkable.uid))
						{
							UnityEngine.Debug.LogWarning(string.Concat(new object[]
							{
								"Skipping entity ",
								entData.baseNetworkable.uid,
								" ",
								StringPool.Get(entData.baseNetworkable.prefabID),
								" - uid is used multiple times"
							}));
						}
						else
						{
							if (entData.baseNetworkable.uid.IsValid)
							{
								hashSet.Add(entData.baseNetworkable.uid);
							}
							global::BaseEntity baseEntity = GameManager.server.CreateEntity(StringPool.Get(entData.baseNetworkable.prefabID), entData.baseEntity.pos, Quaternion.Euler(entData.baseEntity.rot), true);
							if (baseEntity)
							{
								baseEntity.InitLoad(entData.baseNetworkable.uid);
								dictionary.Add(baseEntity, entData);
							}
						}
					}
				}
			}
			DebugEx.Log("Spawning " + dictionary.Count + " entities", StackTraceLogType.None);
			global::BaseNetworkable.LoadInfo loadInfo = default(global::BaseNetworkable.LoadInfo);
			loadInfo.fromDisk = true;
			Stopwatch stopwatch = Stopwatch.StartNew();
			int num2 = 0;
			foreach (KeyValuePair<global::BaseEntity, ProtoBuf.Entity> keyValuePair in dictionary)
			{
				global::BaseEntity key = keyValuePair.Key;
				if (!(key == null))
				{
					RCon.Update();
					loadInfo.msg = keyValuePair.Value;
					key.Spawn();
					key.Load(loadInfo);
					if (key.IsValid())
					{
						num2++;
						if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
						{
							stopwatch.Reset();
							stopwatch.Start();
							DebugEx.Log(string.Concat(new object[] { "\t", num2, " / ", dictionary.Count }), StackTraceLogType.None);
						}
					}
				}
			}
			foreach (KeyValuePair<global::BaseEntity, ProtoBuf.Entity> keyValuePair2 in dictionary)
			{
				global::BaseEntity key2 = keyValuePair2.Key;
				if (!(key2 == null))
				{
					RCon.Update();
					if (key2.IsValid())
					{
						key2.UpdateNetworkGroup();
						key2.PostServerLoad();
					}
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				DebugEx.Log("Enforcing SpawnPopulation Limits", StackTraceLogType.None);
				SingletonComponent<SpawnHandler>.Instance.EnforceLimits(false);
				DebugEx.Log("\tdone.", StackTraceLogType.None);
			}
			SaveRestore.InitializeWipeId();
			Rust.Application.isLoadingSave = false;
			flag = true;
		}
		catch (Exception ex2)
		{
			UnityEngine.Debug.LogWarning("Error loading save (" + strFilename + ")");
			UnityEngine.Debug.LogException(ex2);
			flag = false;
		}
		return flag;
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x00104250 File Offset: 0x00102450
	public static void GetSaveCache()
	{
		global::BaseEntity[] array = global::BaseEntity.saveList.ToArray<global::BaseEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log("Initializing " + array.Length + " entity save caches", StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < array.Length; i++)
			{
				global::BaseEntity baseEntity = array[i];
				if (baseEntity.IsValid())
				{
					baseEntity.GetSaveCache();
					if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
					{
						stopwatch.Reset();
						stopwatch.Start();
						DebugEx.Log(string.Concat(new object[]
						{
							"\t",
							i + 1,
							" / ",
							array.Length
						}), StackTraceLogType.None);
					}
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x0010431C File Offset: 0x0010251C
	public static void InitializeEntityLinks()
	{
		global::BaseEntity[] array = (from x in global::BaseNetworkable.serverEntities
			where x is global::BaseEntity
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log("Initializing " + array.Length + " entity links", StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < array.Length; i++)
			{
				RCon.Update();
				array[i].RefreshEntityLinks();
				if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[]
					{
						"\t",
						i + 1,
						" / ",
						array.Length
					}), StackTraceLogType.None);
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x00104428 File Offset: 0x00102628
	public static void InitializeEntitySupports()
	{
		if (!ConVar.Server.stability)
		{
			return;
		}
		global::StabilityEntity[] array = (from x in global::BaseNetworkable.serverEntities
			where x is global::StabilityEntity
			select x as global::StabilityEntity).ToArray<global::StabilityEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log("Initializing " + array.Length + " stability supports", StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < array.Length; i++)
			{
				RCon.Update();
				array[i].InitializeSupports();
				if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[]
					{
						"\t",
						i + 1,
						" / ",
						array.Length
					}), StackTraceLogType.None);
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x0010453C File Offset: 0x0010273C
	public static void InitializeEntityConditionals()
	{
		global::BuildingBlock[] array = (from x in global::BaseNetworkable.serverEntities
			where x is global::BuildingBlock
			select x as global::BuildingBlock).ToArray<global::BuildingBlock>();
		if (array.Length != 0)
		{
			DebugEx.Log("Initializing " + array.Length + " conditional models", StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < array.Length; i++)
			{
				RCon.Update();
				array[i].UpdateSkin(true);
				if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[]
					{
						"\t",
						i + 1,
						" / ",
						array.Length
					}), StackTraceLogType.None);
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x0010464C File Offset: 0x0010284C
	public static void InitializeWipeId()
	{
		if (SaveRestore.WipeId == null)
		{
			SaveRestore.WipeId = Guid.NewGuid().ToString("N");
		}
	}

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002AA7 RID: 10919 RVA: 0x00104677 File Offset: 0x00102877
	// (set) Token: 0x06002AA8 RID: 10920 RVA: 0x0010467E File Offset: 0x0010287E
	public static string WipeId { get; private set; }

	// Token: 0x06002AA9 RID: 10921 RVA: 0x00104686 File Offset: 0x00102886
	public static IEnumerator Save(string strFilename, bool AndWait = false)
	{
		if (Rust.Application.isQuitting)
		{
			yield break;
		}
		Stopwatch timerCache = new Stopwatch();
		Stopwatch timerWrite = new Stopwatch();
		Stopwatch timerDisk = new Stopwatch();
		int iEnts = 0;
		timerCache.Start();
		using (TimeWarning.New("SaveCache", 100))
		{
			Stopwatch sw = Stopwatch.StartNew();
			foreach (global::BaseEntity baseEntity in global::BaseEntity.saveList.ToArray<global::BaseEntity>())
			{
				if (!(baseEntity == null) && baseEntity.IsValid())
				{
					try
					{
						baseEntity.GetSaveCache();
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogException(ex);
					}
					if (sw.Elapsed.TotalMilliseconds > 5.0)
					{
						if (!AndWait)
						{
							yield return CoroutineEx.waitForEndOfFrame;
						}
						sw.Reset();
						sw.Start();
					}
				}
			}
			global::BaseEntity[] array = null;
			sw = null;
		}
		TimeWarning timeWarning = null;
		timerCache.Stop();
		SaveRestore.SaveBuffer.Position = 0L;
		SaveRestore.SaveBuffer.SetLength(0L);
		timerWrite.Start();
		using (TimeWarning.New("SaveWrite", 100))
		{
			BinaryWriter writer = new BinaryWriter(SaveRestore.SaveBuffer);
			writer.Write(83);
			writer.Write(65);
			writer.Write(86);
			writer.Write(82);
			SaveRestore.InitializeWipeId();
			SaveRestore.SaveExtraData saveExtraData = new SaveRestore.SaveExtraData();
			saveExtraData.WipeId = SaveRestore.WipeId;
			writer.Write(74);
			writer.Write(JsonConvert.SerializeObject(saveExtraData));
			writer.Write(68);
			writer.Write(Epoch.FromDateTime(SaveRestore.SaveCreatedTime));
			writer.Write(239U);
			default(global::BaseNetworkable.SaveInfo).forDisk = true;
			if (!AndWait)
			{
				yield return CoroutineEx.waitForEndOfFrame;
			}
			foreach (global::BaseEntity baseEntity2 in global::BaseEntity.saveList)
			{
				if (baseEntity2 == null || baseEntity2.IsDestroyed)
				{
					UnityEngine.Debug.LogWarning("Entity is NULL but is still in saveList - not destroyed properly? " + baseEntity2, baseEntity2);
				}
				else
				{
					MemoryStream memoryStream = null;
					try
					{
						memoryStream = baseEntity2.GetSaveCache();
					}
					catch (Exception ex2)
					{
						UnityEngine.Debug.LogException(ex2);
					}
					if (memoryStream == null || memoryStream.Length <= 0L)
					{
						UnityEngine.Debug.LogWarningFormat("Skipping saving entity {0} - because {1}", new object[]
						{
							baseEntity2,
							(memoryStream == null) ? "savecache is null" : "savecache is 0"
						});
					}
					else
					{
						writer.Write((uint)memoryStream.Length);
						writer.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
						int num = iEnts;
						iEnts = num + 1;
					}
				}
			}
			writer = null;
		}
		timeWarning = null;
		timerWrite.Stop();
		if (!AndWait)
		{
			yield return CoroutineEx.waitForEndOfFrame;
		}
		timerDisk.Start();
		using (TimeWarning.New("SaveBackup", 100))
		{
			SaveRestore.ShiftSaveBackups(strFilename);
		}
		using (TimeWarning.New("SaveDisk", 100))
		{
			try
			{
				string text = strFilename + ".new";
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				try
				{
					using (FileStream fileStream = File.OpenWrite(text))
					{
						SaveRestore.SaveBuffer.Position = 0L;
						SaveRestore.SaveBuffer.CopyTo(fileStream);
					}
				}
				catch (Exception ex3)
				{
					UnityEngine.Debug.LogError("Couldn't write save file! We got an exception: " + ex3);
					if (File.Exists(text))
					{
						File.Delete(text);
					}
					yield break;
				}
				File.Copy(text, strFilename, true);
				File.Delete(text);
			}
			catch (Exception ex4)
			{
				UnityEngine.Debug.LogError("Error when saving to disk: " + ex4);
				yield break;
			}
		}
		timerDisk.Stop();
		UnityEngine.Debug.LogFormat("Saved {0} ents, cache({1}), write({2}), disk({3}).", new object[]
		{
			iEnts.ToString("N0"),
			timerCache.Elapsed.TotalSeconds.ToString("0.00"),
			timerWrite.Elapsed.TotalSeconds.ToString("0.00"),
			timerDisk.Elapsed.TotalSeconds.ToString("0.00")
		});
		yield break;
		yield break;
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x0010469C File Offset: 0x0010289C
	private static void ShiftSaveBackups(string fileName)
	{
		SaveRestore.<>c__DisplayClass16_0 CS$<>8__locals1;
		CS$<>8__locals1.fileName = fileName;
		int num = Mathf.Max(ConVar.Server.saveBackupCount, 2);
		if (!File.Exists(CS$<>8__locals1.fileName))
		{
			return;
		}
		try
		{
			int num2 = 0;
			int num3 = 1;
			while (num3 <= num && File.Exists(CS$<>8__locals1.fileName + "." + num3))
			{
				num2++;
				num3++;
			}
			string text = SaveRestore.<ShiftSaveBackups>g__GetBackupName|16_0(num2 + 1, ref CS$<>8__locals1);
			for (int i = num2; i > 0; i--)
			{
				string text2 = SaveRestore.<ShiftSaveBackups>g__GetBackupName|16_0(i, ref CS$<>8__locals1);
				if (i == num)
				{
					File.Delete(text2);
				}
				else if (File.Exists(text2))
				{
					if (File.Exists(text))
					{
						File.Delete(text);
					}
					File.Move(text2, text);
				}
				text = text2;
			}
			File.Copy(CS$<>8__locals1.fileName, text, true);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Error while backing up old saves: " + ex.Message);
			UnityEngine.Debug.LogException(ex);
			throw;
		}
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x00104798 File Offset: 0x00102998
	private void Start()
	{
		base.StartCoroutine(this.SaveRegularly());
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x001047A7 File Offset: 0x001029A7
	private IEnumerator SaveRegularly()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForSeconds((float)ConVar.Server.saveinterval);
			yield return base.StartCoroutine(this.DoAutomatedSave(false));
		}
		yield break;
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x001047B6 File Offset: 0x001029B6
	private IEnumerator DoAutomatedSave(bool AndWait = false)
	{
		SaveRestore.IsSaving = true;
		string folder = ConVar.Server.rootFolder;
		if (!AndWait)
		{
			yield return CoroutineEx.waitForEndOfFrame;
		}
		if (AndWait)
		{
			IEnumerator enumerator = SaveRestore.Save(folder + "/" + global::World.SaveFileName, AndWait);
			while (enumerator.MoveNext())
			{
			}
		}
		else
		{
			yield return base.StartCoroutine(SaveRestore.Save(folder + "/" + global::World.SaveFileName, AndWait));
		}
		if (!AndWait)
		{
			yield return CoroutineEx.waitForEndOfFrame;
		}
		UnityEngine.Debug.Log("Saving complete");
		SaveRestore.IsSaving = false;
		yield break;
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x001047CC File Offset: 0x001029CC
	public static bool Save(bool AndWait)
	{
		if (SingletonComponent<SaveRestore>.Instance == null)
		{
			return false;
		}
		if (SaveRestore.IsSaving)
		{
			return false;
		}
		IEnumerator enumerator = SingletonComponent<SaveRestore>.Instance.DoAutomatedSave(true);
		while (enumerator.MoveNext())
		{
		}
		return true;
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x00104825 File Offset: 0x00102A25
	[CompilerGenerated]
	internal static string <ShiftSaveBackups>g__GetBackupName|16_0(int i, ref SaveRestore.<>c__DisplayClass16_0 A_1)
	{
		return string.Format("{0}.{1}", A_1.fileName, i);
	}

	// Token: 0x040022DD RID: 8925
	public static bool IsSaving = false;

	// Token: 0x040022DE RID: 8926
	public static DateTime SaveCreatedTime;

	// Token: 0x040022DF RID: 8927
	private static MemoryStream SaveBuffer = new MemoryStream(33554432);

	// Token: 0x02000D64 RID: 3428
	public class SaveExtraData
	{
		// Token: 0x040047BE RID: 18366
		public string WipeId;
	}
}
