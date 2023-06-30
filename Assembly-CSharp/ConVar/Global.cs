using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
	// Token: 0x02000AC6 RID: 2758
	[ConsoleSystem.Factory("global")]
	public class Global : ConsoleSystem
	{
		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06004201 RID: 16897 RVA: 0x001868D0 File Offset: 0x00184AD0
		// (set) Token: 0x06004200 RID: 16896 RVA: 0x001868C8 File Offset: 0x00184AC8
		[ServerVar]
		[ClientVar]
		public static int developer
		{
			get
			{
				return Global._developer;
			}
			set
			{
				Global._developer = value;
			}
		}

		// Token: 0x06004202 RID: 16898 RVA: 0x001868D8 File Offset: 0x00184AD8
		public static void ApplyAsyncLoadingPreset()
		{
			if (Global.asyncLoadingPreset != 0)
			{
				UnityEngine.Debug.Log(string.Format("Applying async loading preset number {0}", Global.asyncLoadingPreset));
			}
			switch (Global.asyncLoadingPreset)
			{
			case 0:
				break;
			case 1:
				if (Global.warmupConcurrency <= 1)
				{
					Global.warmupConcurrency = 256;
				}
				if (Global.preloadConcurrency <= 1)
				{
					Global.preloadConcurrency = 256;
				}
				Global.asyncWarmup = false;
				return;
			case 2:
				if (Global.warmupConcurrency <= 1)
				{
					Global.warmupConcurrency = 256;
				}
				if (Global.preloadConcurrency <= 1)
				{
					Global.preloadConcurrency = 256;
				}
				Global.asyncWarmup = false;
				return;
			default:
				UnityEngine.Debug.LogWarning(string.Format("There is no asyncLoading preset number {0}", Global.asyncLoadingPreset));
				break;
			}
		}

		// Token: 0x06004203 RID: 16899 RVA: 0x0018698E File Offset: 0x00184B8E
		[ServerVar]
		public static void restart(ConsoleSystem.Arg args)
		{
			ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
		}

		// Token: 0x06004204 RID: 16900 RVA: 0x001869AD File Offset: 0x00184BAD
		[ClientVar]
		[ServerVar]
		public static void quit(ConsoleSystem.Arg args)
		{
			SingletonComponent<ServerMgr>.Instance.Shutdown();
			Rust.Application.isQuitting = true;
			Net.sv.Stop("quit");
			Process.GetCurrentProcess().Kill();
			UnityEngine.Debug.Log("Quitting");
			Rust.Application.Quit();
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x001869E7 File Offset: 0x00184BE7
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			ServerPerformance.DoReport();
		}

		// Token: 0x06004206 RID: 16902 RVA: 0x001869F0 File Offset: 0x00184BF0
		[ServerVar]
		[ClientVar]
		public static void objects(ConsoleSystem.Arg args)
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
			string text = "";
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			Dictionary<Type, long> dictionary2 = new Dictionary<Type, long>();
			foreach (UnityEngine.Object @object in array)
			{
				int runtimeMemorySize = Profiler.GetRuntimeMemorySize(@object);
				if (dictionary.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, int> dictionary3 = dictionary;
					Type type = @object.GetType();
					int num = dictionary3[type];
					dictionary3[type] = num + 1;
				}
				else
				{
					dictionary.Add(@object.GetType(), 1);
				}
				if (dictionary2.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, long> dictionary4 = dictionary2;
					Type type = @object.GetType();
					dictionary4[type] += (long)runtimeMemorySize;
				}
				else
				{
					dictionary2.Add(@object.GetType(), (long)runtimeMemorySize);
				}
			}
			foreach (KeyValuePair<Type, long> keyValuePair in dictionary2.OrderByDescending(delegate(KeyValuePair<Type, long> x)
			{
				KeyValuePair<Type, long> keyValuePair2 = x;
				return keyValuePair2.Value;
			}))
			{
				text = string.Concat(new object[]
				{
					text,
					dictionary[keyValuePair.Key].ToString().PadLeft(10),
					" ",
					keyValuePair.Value.FormatBytes(false).PadLeft(15),
					"\t",
					keyValuePair.Key,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x06004207 RID: 16903 RVA: 0x00186B8C File Offset: 0x00184D8C
		[ServerVar]
		[ClientVar]
		public static void textures(ConsoleSystem.Arg args)
		{
			Texture[] array = UnityEngine.Object.FindObjectsOfType<Texture>();
			string text = "";
			foreach (Texture texture in array)
			{
				string text2 = Profiler.GetRuntimeMemorySize(texture).FormatBytes(false);
				text = string.Concat(new string[]
				{
					text,
					texture.ToString().PadRight(30),
					texture.name.PadRight(30),
					text2,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x06004208 RID: 16904 RVA: 0x00186C0C File Offset: 0x00184E0C
		[ServerVar]
		[ClientVar]
		public static void colliders(ConsoleSystem.Arg args)
		{
			int num = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
				where x.enabled
				select x).Count<Collider>();
			int num2 = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
				where !x.enabled
				select x).Count<Collider>();
			string text = string.Concat(new object[] { num, " colliders enabled, ", num2, " disabled" });
			args.ReplyWith(text);
		}

		// Token: 0x06004209 RID: 16905 RVA: 0x00186CAC File Offset: 0x00184EAC
		[ServerVar]
		[ClientVar]
		public static void error(ConsoleSystem.Arg args)
		{
			((GameObject)null).transform.position = Vector3.zero;
		}

		// Token: 0x0600420A RID: 16906 RVA: 0x00186CC0 File Offset: 0x00184EC0
		[ServerVar]
		[ClientVar]
		public static void queue(ConsoleSystem.Arg args)
		{
			string text = "";
			text = text + "stabilityCheckQueue:\t\t" + global::StabilityEntity.stabilityCheckQueue.Info() + "\n";
			text = text + "updateSurroundingsQueue:\t" + global::StabilityEntity.updateSurroundingsQueue.Info() + "\n";
			args.ReplyWith(text);
		}

		// Token: 0x0600420B RID: 16907 RVA: 0x00186D10 File Offset: 0x00184F10
		[ServerUserVar]
		public static void setinfo(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			string @string = args.GetString(0, null);
			string string2 = args.GetString(1, null);
			if (@string == null || string2 == null)
			{
				return;
			}
			basePlayer.SetInfo(@string, string2);
		}

		// Token: 0x0600420C RID: 16908 RVA: 0x00186D50 File Offset: 0x00184F50
		[ServerVar]
		public static void sleep(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			basePlayer.StartSleeping();
		}

		// Token: 0x0600420D RID: 16909 RVA: 0x00186D90 File Offset: 0x00184F90
		[ServerUserVar]
		public static void kill(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.CanSuicide())
			{
				basePlayer.MarkSuicide();
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
				return;
			}
			basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
		}

		// Token: 0x0600420E RID: 16910 RVA: 0x00186DE8 File Offset: 0x00184FE8
		[ServerUserVar]
		public static void respawn(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead() && !basePlayer.IsSpectating())
			{
				if (Global.developer > 0)
				{
					UnityEngine.Debug.LogWarning(basePlayer + " wanted to respawn but isn't dead or spectating");
				}
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			if (basePlayer.CanRespawn())
			{
				basePlayer.MarkRespawn(5f);
				basePlayer.Respawn();
				return;
			}
			basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x00186E5A File Offset: 0x0018505A
		[ServerVar]
		public static void injure(ConsoleSystem.Arg args)
		{
			Global.InjurePlayer(args.Player());
		}

		// Token: 0x06004210 RID: 16912 RVA: 0x00186E68 File Offset: 0x00185068
		public static void InjurePlayer(global::BasePlayer ply)
		{
			if (ply == null)
			{
				return;
			}
			if (ply.IsDead())
			{
				return;
			}
			if (!ConVar.Server.woundingenabled || ply.IsIncapacitated() || ply.IsSleeping() || ply.isMounted)
			{
				ply.ConsoleMessage("Can't go to wounded state right now.");
				return;
			}
			if (ply.IsCrawling())
			{
				ply.GoToIncapacitated(null);
				return;
			}
			ply.BecomeWounded(null);
		}

		// Token: 0x06004211 RID: 16913 RVA: 0x00186ED0 File Offset: 0x001850D0
		[ServerVar]
		public static void recover(ConsoleSystem.Arg args)
		{
			Global.RecoverPlayer(args.Player());
		}

		// Token: 0x06004212 RID: 16914 RVA: 0x00186EDD File Offset: 0x001850DD
		public static void RecoverPlayer(global::BasePlayer ply)
		{
			if (ply == null)
			{
				return;
			}
			if (ply.IsDead())
			{
				return;
			}
			ply.StopWounded(null);
		}

		// Token: 0x06004213 RID: 16915 RVA: 0x00186EFC File Offset: 0x001850FC
		[ServerVar]
		public static void spectate(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			string @string = args.GetString(0, "");
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(@string);
			}
		}

		// Token: 0x06004214 RID: 16916 RVA: 0x00186F4C File Offset: 0x0018514C
		[ServerVar]
		public static void spectateid(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			ulong @ulong = args.GetULong(0, 0UL);
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(@ulong);
			}
		}

		// Token: 0x06004215 RID: 16917 RVA: 0x00186F98 File Offset: 0x00185198
		[ServerUserVar]
		public static void respawn_sleepingbag(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				return;
			}
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			if (!basePlayer.CanRespawn())
			{
				basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
				return;
			}
			if (global::SleepingBag.SpawnPlayer(basePlayer, entityID))
			{
				basePlayer.MarkRespawn(5f);
				return;
			}
			args.ReplyWith("Couldn't spawn in sleeping bag!");
		}

		// Token: 0x06004216 RID: 16918 RVA: 0x00187018 File Offset: 0x00185218
		[ServerUserVar]
		public static void respawn_sleepingbag_remove(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			global::SleepingBag.DestroyBag(basePlayer, entityID);
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x00187064 File Offset: 0x00185264
		[ServerUserVar]
		public static void status_sv(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			args.ReplyWith(basePlayer.GetDebugStatus());
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar]
		public static void status_cl(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x00187090 File Offset: 0x00185290
		[ServerVar]
		public static void teleport(ConsoleSystem.Arg args)
		{
			if (args.HasArgs(2))
			{
				global::BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
				if (!playerOrSleeperOrBot)
				{
					return;
				}
				if (!playerOrSleeperOrBot.IsAlive())
				{
					return;
				}
				global::BasePlayer playerOrSleeperOrBot2 = args.GetPlayerOrSleeperOrBot(1);
				if (!playerOrSleeperOrBot2)
				{
					return;
				}
				if (!playerOrSleeperOrBot2.IsAlive())
				{
					return;
				}
				playerOrSleeperOrBot.Teleport(playerOrSleeperOrBot2);
				return;
			}
			else
			{
				global::BasePlayer basePlayer = args.Player();
				if (!basePlayer)
				{
					return;
				}
				if (!basePlayer.IsAlive())
				{
					return;
				}
				global::BasePlayer playerOrSleeperOrBot3 = args.GetPlayerOrSleeperOrBot(0);
				if (!playerOrSleeperOrBot3)
				{
					return;
				}
				if (!playerOrSleeperOrBot3.IsAlive())
				{
					return;
				}
				basePlayer.Teleport(playerOrSleeperOrBot3);
				return;
			}
		}

		// Token: 0x0600421A RID: 16922 RVA: 0x0018711C File Offset: 0x0018531C
		[ServerVar]
		public static void teleport2me(ConsoleSystem.Arg args)
		{
			global::BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
			if (playerOrSleeperOrBot == null)
			{
				args.ReplyWith("Player or bot not found");
				return;
			}
			if (!playerOrSleeperOrBot.IsAlive())
			{
				args.ReplyWith("Target is not alive");
				return;
			}
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			playerOrSleeperOrBot.Teleport(basePlayer);
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x0018717C File Offset: 0x0018537C
		[ServerVar]
		public static void teleportany(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetString(0, ""), false);
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x001871B8 File Offset: 0x001853B8
		[ServerVar]
		public static void teleportpos(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetVector3(0, Vector3.zero));
		}

		// Token: 0x0600421D RID: 16925 RVA: 0x001871F0 File Offset: 0x001853F0
		[ServerVar]
		public static void teleportlos(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			Ray ray = basePlayer.eyes.HeadRay();
			int @int = args.GetInt(0, 1000);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, (float)@int, 1218652417))
			{
				basePlayer.Teleport(raycastHit.point);
				return;
			}
			basePlayer.Teleport(ray.origin + ray.direction * (float)@int);
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x00187270 File Offset: 0x00185470
		[ServerVar]
		public static void teleport2owneditem(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong userID;
			if (playerOrSleeper != null)
			{
				userID = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out userID))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			global::BaseEntity[] array = global::BaseEntity.Util.FindTargetsOwnedBy(userID, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = UnityEngine.Random.Range(0, array.Length);
			arg.ReplyWith(string.Format("Teleporting to {0} at {1}", array[num].ShortPrefabName, array[num].transform.position));
			basePlayer.Teleport(array[num].transform.position);
		}

		// Token: 0x0600421F RID: 16927 RVA: 0x00187338 File Offset: 0x00185538
		[ServerVar]
		public static void teleport2autheditem(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong userID;
			if (playerOrSleeper != null)
			{
				userID = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out userID))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			global::BaseEntity[] array = global::BaseEntity.Util.FindTargetsAuthedTo(userID, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = UnityEngine.Random.Range(0, array.Length);
			arg.ReplyWith(string.Format("Teleporting to {0} at {1}", array[num].ShortPrefabName, array[num].transform.position));
			basePlayer.Teleport(array[num].transform.position);
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x00187400 File Offset: 0x00185600
		[ServerVar]
		public static void teleport2marker(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.State.pointsOfInterest == null || basePlayer.State.pointsOfInterest.Count == 0)
			{
				arg.ReplyWith("You don't have a marker set");
				return;
			}
			string @string = arg.GetString(0, "");
			if (!string.IsNullOrEmpty(@string))
			{
				foreach (MapNote mapNote in basePlayer.State.pointsOfInterest)
				{
					if (!string.IsNullOrEmpty(mapNote.label) && string.Equals(mapNote.label, @string, StringComparison.InvariantCultureIgnoreCase))
					{
						Global.TeleportToMarker(mapNote, basePlayer);
						return;
					}
				}
			}
			if (arg.HasArgs(1))
			{
				int @int = arg.GetInt(0, 0);
				if (@int >= 0 && @int < basePlayer.State.pointsOfInterest.Count)
				{
					Global.TeleportToMarker(basePlayer.State.pointsOfInterest[@int], basePlayer);
					return;
				}
			}
			int num = basePlayer.DebugMapMarkerIndex;
			num++;
			if (num >= basePlayer.State.pointsOfInterest.Count)
			{
				num = 0;
			}
			Global.TeleportToMarker(basePlayer.State.pointsOfInterest[num], basePlayer);
			basePlayer.DebugMapMarkerIndex = num;
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x00187548 File Offset: 0x00185748
		private static void TeleportToMarker(MapNote marker, global::BasePlayer player)
		{
			Vector3 worldPosition = marker.worldPosition;
			float height = TerrainMeta.HeightMap.GetHeight(worldPosition);
			float height2 = TerrainMeta.WaterMap.GetHeight(worldPosition);
			worldPosition.y = Mathf.Max(height, height2);
			player.Teleport(worldPosition);
		}

		// Token: 0x06004222 RID: 16930 RVA: 0x0018758C File Offset: 0x0018578C
		[ServerVar]
		public static void teleport2death(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.ServerCurrentDeathNote == null)
			{
				arg.ReplyWith("You don't have a current death note!");
			}
			Vector3 worldPosition = basePlayer.ServerCurrentDeathNote.worldPosition;
			basePlayer.Teleport(worldPosition);
		}

		// Token: 0x06004223 RID: 16931 RVA: 0x001875C4 File Offset: 0x001857C4
		[ServerVar]
		[ClientVar]
		public static void free(ConsoleSystem.Arg args)
		{
			Pool.clear_prefabs(args);
			Pool.clear_assets(args);
			Pool.clear_memory(args);
			ConVar.GC.collect();
			ConVar.GC.unload();
		}

		// Token: 0x06004224 RID: 16932 RVA: 0x001875E4 File Offset: 0x001857E4
		[ServerVar(ServerUser = true)]
		[ClientVar]
		public static void version(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(string.Format("Protocol: {0}\nBuild Date: {1}\nUnity Version: {2}\nChangeset: {3}\nBranch: {4}", new object[]
			{
				Protocol.printable,
				BuildInfo.Current.BuildDate,
				UnityEngine.Application.unityVersion,
				BuildInfo.Current.Scm.ChangeId,
				BuildInfo.Current.Scm.Branch
			}));
		}

		// Token: 0x06004225 RID: 16933 RVA: 0x0018764D File Offset: 0x0018584D
		[ServerVar]
		[ClientVar]
		public static void sysinfo(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfoGeneralText.currentInfo);
		}

		// Token: 0x06004226 RID: 16934 RVA: 0x0018765A File Offset: 0x0018585A
		[ServerVar]
		[ClientVar]
		public static void sysuid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfo.deviceUniqueIdentifier);
		}

		// Token: 0x06004227 RID: 16935 RVA: 0x00187668 File Offset: 0x00185868
		[ServerVar]
		public static void breakitem(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			global::Item activeItem = basePlayer.GetActiveItem();
			if (activeItem != null)
			{
				activeItem.LoseCondition(activeItem.condition);
			}
		}

		// Token: 0x06004228 RID: 16936 RVA: 0x0018769C File Offset: 0x0018589C
		[ServerVar]
		public static void breakclothing(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			foreach (global::Item item in basePlayer.inventory.containerWear.itemList)
			{
				if (item != null)
				{
					item.LoseCondition(item.condition);
				}
			}
		}

		// Token: 0x06004229 RID: 16937 RVA: 0x00187714 File Offset: 0x00185914
		[ServerVar]
		[ClientVar]
		public static void subscriptions(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("group");
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer)
			{
				foreach (Group group in basePlayer.net.subscriber.subscribed)
				{
					textTable.AddRow(new string[]
					{
						"sv",
						group.ID.ToString()
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x0600422A RID: 16938 RVA: 0x001877D8 File Offset: 0x001859D8
		public static uint GingerbreadMaterialID()
		{
			if (Global._gingerbreadMaterialID == 0U)
			{
				Global._gingerbreadMaterialID = StringPool.Get("Gingerbread");
			}
			return Global._gingerbreadMaterialID;
		}

		// Token: 0x0600422B RID: 16939 RVA: 0x001877F8 File Offset: 0x001859F8
		[ServerVar]
		public static void ClearAllSprays()
		{
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				list.Add(sprayCanSpray);
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			Pool.FreeList<SprayCanSpray>(ref list);
		}

		// Token: 0x0600422C RID: 16940 RVA: 0x00187894 File Offset: 0x00185A94
		[ServerVar]
		public static void ClearAllSpraysByPlayer(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return;
			}
			ulong @ulong = arg.GetULong(0, 0UL);
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				if (sprayCanSpray.sprayedByPlayer == @ulong)
				{
					list.Add(sprayCanSpray);
				}
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			int count = list.Count;
			Pool.FreeList<SprayCanSpray>(ref list);
			arg.ReplyWith(string.Format("Deleted {0} sprays by {1}", count, @ulong));
		}

		// Token: 0x0600422D RID: 16941 RVA: 0x00187974 File Offset: 0x00185B74
		[ServerVar]
		public static void ClearSpraysInRadius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			float @float = arg.GetFloat(0, 16f);
			int num = Global.ClearSpraysInRadius(basePlayer.transform.position, @float);
			arg.ReplyWith(string.Format("Deleted {0} sprays within {1} of {2}", num, @float, basePlayer.displayName));
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x001879D4 File Offset: 0x00185BD4
		private static int ClearSpraysInRadius(Vector3 position, float radius)
		{
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				if (sprayCanSpray.Distance(position) <= radius)
				{
					list.Add(sprayCanSpray);
				}
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			int count = list.Count;
			Pool.FreeList<SprayCanSpray>(ref list);
			return count;
		}

		// Token: 0x0600422F RID: 16943 RVA: 0x00187A80 File Offset: 0x00185C80
		[ServerVar]
		public static void ClearSpraysAtPositionInRadius(ConsoleSystem.Arg arg)
		{
			Vector3 vector = arg.GetVector3(0, default(Vector3));
			float @float = arg.GetFloat(1, 0f);
			if (@float == 0f)
			{
				return;
			}
			int num = Global.ClearSpraysInRadius(vector, @float);
			arg.ReplyWith(string.Format("Deleted {0} sprays within {1} of {2}", num, @float, vector));
		}

		// Token: 0x06004230 RID: 16944 RVA: 0x00187AE0 File Offset: 0x00185CE0
		[ServerVar]
		public static void ClearDroppedItems()
		{
			List<DroppedItem> list = Pool.GetList<DroppedItem>();
			using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DroppedItem droppedItem;
					if ((droppedItem = enumerator.Current as DroppedItem) != null)
					{
						list.Add(droppedItem);
					}
				}
			}
			foreach (DroppedItem droppedItem2 in list)
			{
				droppedItem2.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			Pool.FreeList<DroppedItem>(ref list);
		}

		// Token: 0x04003BAB RID: 15275
		private static int _developer;

		// Token: 0x04003BAC RID: 15276
		[ServerVar]
		[ClientVar(Help = "WARNING: This causes random crashes!")]
		public static bool skipAssetWarmup_crashes = false;

		// Token: 0x04003BAD RID: 15277
		[ServerVar]
		[ClientVar]
		public static int maxthreads = 8;

		// Token: 0x04003BAE RID: 15278
		private const int DefaultWarmupConcurrency = 1;

		// Token: 0x04003BAF RID: 15279
		private const int DefaultPreloadConcurrency = 1;

		// Token: 0x04003BB0 RID: 15280
		[ServerVar]
		[ClientVar]
		public static int warmupConcurrency = 1;

		// Token: 0x04003BB1 RID: 15281
		[ServerVar]
		[ClientVar]
		public static int preloadConcurrency = 1;

		// Token: 0x04003BB2 RID: 15282
		[ServerVar]
		[ClientVar]
		public static bool forceUnloadBundles = true;

		// Token: 0x04003BB3 RID: 15283
		private const bool DefaultAsyncWarmupEnabled = false;

		// Token: 0x04003BB4 RID: 15284
		[ServerVar]
		[ClientVar]
		public static bool asyncWarmup = false;

		// Token: 0x04003BB5 RID: 15285
		[ClientVar(Saved = true, Help = "Experimental faster loading, requires game restart (0 = off, 1 = partial, 2 = full)")]
		public static int asyncLoadingPreset = 0;

		// Token: 0x04003BB6 RID: 15286
		[ServerVar(Saved = true)]
		[ClientVar(Saved = true)]
		public static int perf = 0;

		// Token: 0x04003BB7 RID: 15287
		[ClientVar(ClientInfo = true, Saved = true, Help = "If you're an admin this will enable god mode")]
		public static bool god = false;

		// Token: 0x04003BB8 RID: 15288
		[ClientVar(ClientInfo = true, Saved = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
		public static bool specnet = false;

		// Token: 0x04003BB9 RID: 15289
		[ClientVar]
		[ServerVar(ClientAdmin = true, ServerAdmin = true, Help = "When enabled a player wearing a gingerbread suit will gib like the gingerbread NPC's")]
		public static bool cinematicGingerbreadCorpses = false;

		// Token: 0x04003BBA RID: 15290
		private static uint _gingerbreadMaterialID = 0U;

		// Token: 0x04003BBB RID: 15291
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Multiplier applied to SprayDuration if a spray isn't in the sprayers auth (cannot go above 1f)")]
		public static float SprayOutOfAuthMultiplier = 0.5f;

		// Token: 0x04003BBC RID: 15292
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Base time (in seconds) that sprays last")]
		public static float SprayDuration = 10800f;

		// Token: 0x04003BBD RID: 15293
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "If a player sprays more than this, the oldest spray will be destroyed. 0 will disable")]
		public static int MaxSpraysPerPlayer = 40;

		// Token: 0x04003BBE RID: 15294
		[ServerVar(Help = "Disables the backpacks that appear after a corpse times out")]
		public static bool disableBagDropping = false;
	}
}
