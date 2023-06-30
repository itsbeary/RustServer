using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Network;
using Newtonsoft.Json;
using Rust;
using Steamworks;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B08 RID: 2824
	public static class Analytics
	{
		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x060044CA RID: 17610 RVA: 0x001931C5 File Offset: 0x001913C5
		// (set) Token: 0x060044CB RID: 17611 RVA: 0x001931CC File Offset: 0x001913CC
		public static string ClientAnalyticsUrl { get; set; } = "https://rust-api.facepunch.com/api/public/analytics/rust/client";

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x060044CC RID: 17612 RVA: 0x001931D4 File Offset: 0x001913D4
		// (set) Token: 0x060044CD RID: 17613 RVA: 0x001931DB File Offset: 0x001913DB
		[ServerVar(Name = "server_analytics_url")]
		public static string ServerAnalyticsUrl { get; set; } = "https://rust-api.facepunch.com/api/public/analytics/rust/server";

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x060044CE RID: 17614 RVA: 0x001931E3 File Offset: 0x001913E3
		// (set) Token: 0x060044CF RID: 17615 RVA: 0x001931EA File Offset: 0x001913EA
		[ServerVar(Name = "analytics_header", Saved = true)]
		public static string AnalyticsHeader { get; set; } = "X-API-KEY";

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x060044D0 RID: 17616 RVA: 0x001931F2 File Offset: 0x001913F2
		// (set) Token: 0x060044D1 RID: 17617 RVA: 0x001931F9 File Offset: 0x001913F9
		[ServerVar(Name = "analytics_enabled")]
		public static bool UploadAnalytics { get; set; } = true;

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x060044D2 RID: 17618 RVA: 0x00193201 File Offset: 0x00191401
		// (set) Token: 0x060044D3 RID: 17619 RVA: 0x00193208 File Offset: 0x00191408
		[ServerVar(Name = "analytics_secret", Saved = true)]
		public static string AnalyticsSecret { get; set; } = "";

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x060044D4 RID: 17620 RVA: 0x00193210 File Offset: 0x00191410
		// (set) Token: 0x060044D5 RID: 17621 RVA: 0x00193217 File Offset: 0x00191417
		public static string AnalyticsPublicKey { get; set; } = "pub878ABLezSB6onshSwBCRGYDCpEI";

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x060044D6 RID: 17622 RVA: 0x0019321F File Offset: 0x0019141F
		// (set) Token: 0x060044D7 RID: 17623 RVA: 0x00193226 File Offset: 0x00191426
		[ServerVar(Name = "high_freq_stats", Saved = true)]
		public static bool HighFrequencyStats { get; set; } = true;

		// Token: 0x060044D8 RID: 17624 RVA: 0x00193230 File Offset: 0x00191430
		[ClientVar(Name = "pending_analytics")]
		[ServerVar(Name = "pending_analytics")]
		public static void GetPendingAnalytics(ConsoleSystem.Arg arg)
		{
			int pendingCount = Analytics.AzureWebInterface.server.PendingCount;
			arg.ReplyWith(string.Format("Pending: {0}", pendingCount));
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x060044D9 RID: 17625 RVA: 0x0019325E File Offset: 0x0019145E
		// (set) Token: 0x060044DA RID: 17626 RVA: 0x0019327C File Offset: 0x0019147C
		[ServerVar(Name = "stats_blacklist", Saved = true)]
		public static string stats_blacklist
		{
			get
			{
				if (Analytics.StatsBlacklist != null)
				{
					return string.Join(",", Analytics.StatsBlacklist);
				}
				return "";
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Analytics.StatsBlacklist = null;
					return;
				}
				Analytics.StatsBlacklist = new HashSet<string>(value.Split(new char[] { ',' }));
			}
		}

		// Token: 0x04003D49 RID: 15689
		public static HashSet<string> StatsBlacklist;

		// Token: 0x04003D4A RID: 15690
		private static HashSet<NetworkableId> trackedSpawnedIds = new HashSet<NetworkableId>();

		// Token: 0x02000F97 RID: 3991
		public static class Azure
		{
			// Token: 0x06005519 RID: 21785 RVA: 0x001B70D0 File Offset: 0x001B52D0
			private static string GetGenesAsString(GrowableEntity plant)
			{
				int num = GrowableGeneEncoding.EncodeGenesToInt(plant.Genes);
				string text;
				if (!Analytics.Azure.geneCache.TryGetValue(num, out text))
				{
					text = string.Join("", from x in plant.Genes.Genes
						group x by x.GetDisplayCharacter() into x
						orderby x.Key
						select x.Count<GrowableGene>().ToString() + x.Key);
				}
				return text;
			}

			// Token: 0x0600551A RID: 21786 RVA: 0x001B7180 File Offset: 0x001B5380
			private static string GetMonument(BaseEntity entity)
			{
				if (entity == null)
				{
					return null;
				}
				SpawnGroup spawnGroup = null;
				BaseCorpse baseCorpse;
				if ((baseCorpse = entity as BaseCorpse) != null)
				{
					spawnGroup = baseCorpse.spawnGroup;
				}
				if (spawnGroup == null)
				{
					SpawnPointInstance component = entity.GetComponent<SpawnPointInstance>();
					if (component != null)
					{
						spawnGroup = component.parentSpawnPointUser as SpawnGroup;
					}
				}
				if (spawnGroup != null)
				{
					if (!string.IsNullOrEmpty(spawnGroup.category))
					{
						return spawnGroup.category;
					}
					if (spawnGroup.Monument != null)
					{
						return spawnGroup.Monument.name;
					}
				}
				MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(entity.transform.position);
				if (monumentInfo != null)
				{
					return monumentInfo.name;
				}
				return null;
			}

			// Token: 0x0600551B RID: 21787 RVA: 0x001B7230 File Offset: 0x001B5430
			private static string GetBiome(Vector3 position)
			{
				string text = null;
				TerrainBiome.Enum biomeMaxType = (TerrainBiome.Enum)TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1);
				switch (biomeMaxType)
				{
				case TerrainBiome.Enum.Arid:
					text = "arid";
					break;
				case TerrainBiome.Enum.Temperate:
					text = "grass";
					break;
				case (TerrainBiome.Enum)3:
					break;
				case TerrainBiome.Enum.Tundra:
					text = "tundra";
					break;
				default:
					if (biomeMaxType == TerrainBiome.Enum.Arctic)
					{
						text = "arctic";
					}
					break;
				}
				return text;
			}

			// Token: 0x0600551C RID: 21788 RVA: 0x001B7289 File Offset: 0x001B5489
			private static bool IsOcean(Vector3 position)
			{
				return TerrainMeta.TopologyMap.GetTopology(position) == 128;
			}

			// Token: 0x0600551D RID: 21789 RVA: 0x001B729D File Offset: 0x001B549D
			private static IEnumerator AggregateLoop()
			{
				int loop = 0;
				while (!Rust.Application.isQuitting)
				{
					yield return CoroutineEx.waitForSecondsRealtime(60f);
					if (Analytics.Azure.Stats)
					{
						yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregatePlayers(false, true));
						if (loop % 60 == 0)
						{
							Analytics.Azure.PushServerInfo();
							yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregateEntitiesAndItems());
							yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregatePlayers(true, false));
							yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregateTeams());
							Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData> dictionary = Analytics.Azure.pendingItems;
							Analytics.Azure.pendingItems = new Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>();
							yield return Analytics.Azure.PushPendingItemsLoopAsync(dictionary);
						}
						int num = loop;
						loop = num + 1;
					}
				}
				yield break;
			}

			// Token: 0x0600551E RID: 21790 RVA: 0x001B72A5 File Offset: 0x001B54A5
			private static IEnumerator TryCatch(IEnumerator coroutine)
			{
				for (;;)
				{
					try
					{
						if (!coroutine.MoveNext())
						{
							yield break;
						}
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogException(ex);
						yield break;
					}
					yield return coroutine.Current;
				}
				yield break;
			}

			// Token: 0x0600551F RID: 21791 RVA: 0x001B72B4 File Offset: 0x001B54B4
			private static IEnumerator AggregateEntitiesAndItems()
			{
				List<BaseNetworkable> entityQueue = new List<BaseNetworkable>();
				entityQueue.Clear();
				int totalCount = BaseNetworkable.serverEntities.Count;
				entityQueue.AddRange(BaseNetworkable.serverEntities);
				Dictionary<string, int> itemDict = new Dictionary<string, int>();
				Dictionary<Analytics.Azure.EntityKey, int> entityDict = new Dictionary<Analytics.Azure.EntityKey, int>();
				yield return null;
				UnityEngine.Debug.Log("Starting to aggregate entities & items...");
				DateTime startTime = DateTime.UtcNow;
				Stopwatch watch = Stopwatch.StartNew();
				foreach (BaseNetworkable entity in entityQueue)
				{
					if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
					{
						yield return null;
						watch.Restart();
					}
					if (!(entity == null) && !entity.IsDestroyed)
					{
						Analytics.Azure.EntityKey entityKey = new Analytics.Azure.EntityKey
						{
							PrefabId = entity.prefabID
						};
						BuildingBlock buildingBlock;
						if ((buildingBlock = entity as BuildingBlock) != null)
						{
							entityKey.Grade = (int)(buildingBlock.grade + 1);
						}
						int num;
						entityDict.TryGetValue(entityKey, out num);
						entityDict[entityKey] = num + 1;
						BasePlayer basePlayer;
						if (!(entity is LootContainer) && ((basePlayer = entity as BasePlayer) == null || !basePlayer.IsNpc) && !(entity is NPCPlayer))
						{
							BasePlayer basePlayer2;
							IItemContainerEntity itemContainerEntity;
							DroppedItemContainer droppedItemContainer;
							if ((basePlayer2 = entity as BasePlayer) != null)
							{
								Analytics.Azure.AddItemsToDict(basePlayer2.inventory.containerMain, itemDict);
								Analytics.Azure.AddItemsToDict(basePlayer2.inventory.containerBelt, itemDict);
								Analytics.Azure.AddItemsToDict(basePlayer2.inventory.containerWear, itemDict);
							}
							else if ((itemContainerEntity = entity as IItemContainerEntity) != null)
							{
								Analytics.Azure.AddItemsToDict(itemContainerEntity.inventory, itemDict);
							}
							else if ((droppedItemContainer = entity as DroppedItemContainer) != null && droppedItemContainer.inventory != null)
							{
								Analytics.Azure.AddItemsToDict(droppedItemContainer.inventory, itemDict);
							}
							entity = null;
						}
					}
				}
				List<BaseNetworkable>.Enumerator enumerator = default(List<BaseNetworkable>.Enumerator);
				UnityEngine.Debug.Log(string.Format("Took {0}s to aggregate {1} entities & items...", Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 1), totalCount));
				DateTime utcNow = DateTime.UtcNow;
				EventRecord.New("entity_sum", true).AddObject("counts", entityDict.Select((KeyValuePair<Analytics.Azure.EntityKey, int> x) => new Analytics.Azure.EntitySumItem
				{
					PrefabId = x.Key.PrefabId,
					Grade = x.Key.Grade,
					Count = x.Value
				})).Submit();
				yield return null;
				EventRecord.New("item_sum", true).AddObject("counts", itemDict).Submit();
				yield return null;
				yield break;
				yield break;
			}

			// Token: 0x06005520 RID: 21792 RVA: 0x001B72BC File Offset: 0x001B54BC
			private static void AddItemsToDict(ItemContainer container, Dictionary<string, int> dict)
			{
				if (container == null || container.itemList == null)
				{
					return;
				}
				foreach (Item item in container.itemList)
				{
					string shortname = item.info.shortname;
					int num;
					dict.TryGetValue(shortname, out num);
					dict[shortname] = num + item.amount;
					if (item.contents != null)
					{
						Analytics.Azure.AddItemsToDict(item.contents, dict);
					}
				}
			}

			// Token: 0x06005521 RID: 21793 RVA: 0x001B7350 File Offset: 0x001B5550
			private static IEnumerator PushPendingItemsLoopAsync(Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData> dict)
			{
				Stopwatch watch = Stopwatch.StartNew();
				foreach (Analytics.Azure.PendingItemsData pendingItemsData in dict.Values)
				{
					try
					{
						Analytics.Azure.LogResource(pendingItemsData.Key.Consumed ? Analytics.Azure.ResourceMode.Consumed : Analytics.Azure.ResourceMode.Produced, pendingItemsData.category, pendingItemsData.Key.Item, pendingItemsData.amount, null, null, false, null, 0UL, pendingItemsData.Key.Entity, null, null);
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogException(ex);
					}
					Analytics.Azure.PendingItemsData pendingItemsData2 = pendingItemsData;
					Pool.Free<Analytics.Azure.PendingItemsData>(ref pendingItemsData2);
					if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
					{
						yield return null;
						watch.Restart();
					}
				}
				Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>.ValueCollection.Enumerator enumerator = default(Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>.ValueCollection.Enumerator);
				dict.Clear();
				yield break;
				yield break;
			}

			// Token: 0x06005522 RID: 21794 RVA: 0x001B7360 File Offset: 0x001B5560
			public static void AddPendingItems(BaseEntity entity, string itemName, int amount, string category, bool consumed = true, bool perEntity = false)
			{
				Analytics.Azure.PendingItemsKey pendingItemsKey = new Analytics.Azure.PendingItemsKey
				{
					Entity = entity.ShortPrefabName,
					Category = category,
					Item = itemName,
					Consumed = consumed,
					EntityId = (perEntity ? entity.net.ID : default(NetworkableId))
				};
				Analytics.Azure.PendingItemsData pendingItemsData;
				if (!Analytics.Azure.pendingItems.TryGetValue(pendingItemsKey, out pendingItemsData))
				{
					pendingItemsData = Pool.Get<Analytics.Azure.PendingItemsData>();
					pendingItemsData.Key = pendingItemsKey;
					pendingItemsData.category = category;
					Analytics.Azure.pendingItems[pendingItemsKey] = pendingItemsData;
				}
				pendingItemsData.amount += amount;
			}

			// Token: 0x06005523 RID: 21795 RVA: 0x001B73FB File Offset: 0x001B55FB
			private static IEnumerator AggregatePlayers(bool blueprints = false, bool positions = false)
			{
				Stopwatch watch = Stopwatch.StartNew();
				List<BasePlayer> list = Pool.GetList<BasePlayer>();
				list.AddRange(BasePlayer.activePlayerList);
				Dictionary<int, int> playerBps = (blueprints ? new Dictionary<int, int>() : null);
				List<Analytics.Azure.PlayerAggregate> playerPositions = (positions ? Pool.GetList<Analytics.Azure.PlayerAggregate>() : null);
				foreach (BasePlayer basePlayer in list)
				{
					if (!(basePlayer == null) && !basePlayer.IsDestroyed)
					{
						if (blueprints)
						{
							foreach (int num in basePlayer.PersistantPlayerInfo.unlockedItems)
							{
								int num2;
								playerBps.TryGetValue(num, out num2);
								playerBps[num] = num2 + 1;
							}
						}
						if (positions)
						{
							Analytics.Azure.PlayerAggregate playerAggregate = Pool.Get<Analytics.Azure.PlayerAggregate>();
							playerAggregate.UserId = basePlayer.WipeId;
							playerAggregate.Position = basePlayer.transform.position;
							playerAggregate.Direction = basePlayer.eyes.bodyRotation.eulerAngles;
							foreach (Item item in basePlayer.inventory.containerBelt.itemList)
							{
								playerAggregate.Hotbar.Add(item.info.shortname);
							}
							foreach (Item item2 in basePlayer.inventory.containerWear.itemList)
							{
								playerAggregate.Hotbar.Add(item2.info.shortname);
							}
							Analytics.Azure.PlayerAggregate playerAggregate2 = playerAggregate;
							Item activeItem = basePlayer.GetActiveItem();
							playerAggregate2.ActiveItem = ((activeItem != null) ? activeItem.info.shortname : null);
							playerPositions.Add(playerAggregate);
						}
						if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
						{
							yield return null;
							watch.Restart();
						}
					}
				}
				List<BasePlayer>.Enumerator enumerator = default(List<BasePlayer>.Enumerator);
				if (blueprints)
				{
					EventRecord.New("blueprint_aggregate_online", true).AddObject("blueprints", playerBps.Select((KeyValuePair<int, int> x) => new
					{
						Key = ItemManager.FindItemDefinition(x.Key).shortname,
						value = x.Value
					})).Submit();
				}
				if (positions)
				{
					EventRecord.New("player_positions", true).AddObject("positions", playerPositions).AddObject("player_count", playerPositions.Count)
						.Submit();
					foreach (Analytics.Azure.PlayerAggregate playerAggregate3 in playerPositions)
					{
						Pool.Free<Analytics.Azure.PlayerAggregate>(ref playerAggregate3);
					}
					Pool.FreeList<Analytics.Azure.PlayerAggregate>(ref playerPositions);
				}
				yield break;
				yield break;
			}

			// Token: 0x06005524 RID: 21796 RVA: 0x001B7411 File Offset: 0x001B5611
			private static IEnumerator AggregateTeams()
			{
				yield return null;
				HashSet<ulong> teamIds = new HashSet<ulong>();
				int inTeam = 0;
				int notInTeam = 0;
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					if (basePlayer != null && !basePlayer.IsDestroyed && basePlayer.currentTeam != 0UL)
					{
						teamIds.Add(basePlayer.currentTeam);
						int num = inTeam;
						inTeam = num + 1;
					}
					else
					{
						int num = notInTeam;
						notInTeam = num + 1;
					}
				}
				yield return null;
				Stopwatch watch = Stopwatch.StartNew();
				List<Analytics.Azure.TeamInfo> teams = Pool.GetList<Analytics.Azure.TeamInfo>();
				foreach (ulong num2 in teamIds)
				{
					RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindTeam(num2);
					if (playerTeam != null && ((playerTeam.members != null) & (playerTeam.members.Count > 0)))
					{
						Analytics.Azure.TeamInfo teamInfo = Pool.Get<Analytics.Azure.TeamInfo>();
						teams.Add(teamInfo);
						foreach (ulong num3 in playerTeam.members)
						{
							BasePlayer basePlayer2 = RelationshipManager.FindByID(num3);
							if (basePlayer2 != null && !basePlayer2.IsDestroyed && basePlayer2.IsConnected && !basePlayer2.IsSleeping())
							{
								teamInfo.online.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(num3));
							}
							else
							{
								teamInfo.offline.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(num3));
							}
						}
						teamInfo.member_count = teamInfo.online.Count + teamInfo.offline.Count;
						if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
						{
							yield return null;
							watch.Restart();
						}
					}
				}
				HashSet<ulong>.Enumerator enumerator2 = default(HashSet<ulong>.Enumerator);
				EventRecord.New("online_teams", true).AddObject("teams", teams).AddField("users_in_team", inTeam)
					.AddField("users_not_in_team", notInTeam)
					.Submit();
				foreach (Analytics.Azure.TeamInfo teamInfo2 in teams)
				{
					Pool.Free<Analytics.Azure.TeamInfo>(ref teamInfo2);
				}
				Pool.FreeList<Analytics.Azure.TeamInfo>(ref teams);
				yield break;
				yield break;
			}

			// Token: 0x1700073C RID: 1852
			// (get) Token: 0x06005525 RID: 21797 RVA: 0x001B7419 File Offset: 0x001B5619
			public static bool Stats
			{
				get
				{
					return (!string.IsNullOrEmpty(Analytics.AnalyticsSecret) || ConVar.Server.official) && ConVar.Server.stats;
				}
			}

			// Token: 0x06005526 RID: 21798 RVA: 0x001B7435 File Offset: 0x001B5635
			public static void Initialize()
			{
				Analytics.Azure.PushItemDefinitions();
				Analytics.Azure.PushEntityManifest();
				SingletonComponent<ServerMgr>.Instance.StartCoroutine(Analytics.Azure.AggregateLoop());
			}

			// Token: 0x06005527 RID: 21799 RVA: 0x001B7454 File Offset: 0x001B5654
			private static void PushServerInfo()
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("server_info", true).AddField("seed", global::World.Seed).AddField("size", global::World.Size)
						.AddField("url", global::World.Url)
						.AddField("wipe_id", SaveRestore.WipeId)
						.AddField("ip_convar", Network.Net.sv.ip)
						.AddField("port_convar", Network.Net.sv.port)
						.AddField("net_protocol", Network.Net.sv.ProtocolId)
						.AddField("protocol_network", 2397)
						.AddField("protocol_save", 239);
					string text = "changeset";
					BuildInfo buildInfo = BuildInfo.Current;
					EventRecord eventRecord2 = eventRecord.AddField(text, ((buildInfo != null) ? buildInfo.Scm.ChangeId : null) ?? "0").AddField("unity_version", UnityEngine.Application.unityVersion);
					string text2 = "branch";
					BuildInfo buildInfo2 = BuildInfo.Current;
					eventRecord2.AddField(text2, ((buildInfo2 != null) ? buildInfo2.Scm.Branch : null) ?? "empty").AddField("server_tags", ConVar.Server.tags).AddField("device_id", SystemInfo.deviceUniqueIdentifier)
						.AddField("network_id", Network.Net.sv.GetLastUIDGiven())
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005528 RID: 21800 RVA: 0x001B75C8 File Offset: 0x001B57C8
			private static void PushItemDefinitions()
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (!(GameManifest.Current == null))
					{
						BuildInfo buildInfo = BuildInfo.Current;
						bool flag;
						if (buildInfo == null)
						{
							flag = null != null;
						}
						else
						{
							BuildInfo.ScmInfo scm = buildInfo.Scm;
							flag = ((scm != null) ? scm.ChangeId : null) != null;
						}
						if (flag)
						{
							EventRecord.New("item_definitions", true).AddObject("items", ItemManager.itemDictionary.Select((KeyValuePair<int, ItemDefinition> x) => x.Value).Select(delegate(ItemDefinition x)
							{
								int itemid = x.itemid;
								string shortname = x.shortname;
								ItemBlueprint blueprint = x.Blueprint;
								float num = ((blueprint != null) ? blueprint.time : 0f);
								ItemBlueprint blueprint2 = x.Blueprint;
								int num2 = ((blueprint2 != null) ? blueprint2.workbenchLevelRequired : 0);
								string text = x.category.ToString();
								string english = x.displayName.english;
								Rarity despawnRarity = x.despawnRarity;
								ItemBlueprint blueprint3 = x.Blueprint;
								var enumerable;
								if (blueprint3 == null)
								{
									enumerable = null;
								}
								else
								{
									enumerable = blueprint3.ingredients.Select((ItemAmount y) => new
									{
										shortname = y.itemDef.shortname,
										amount = (int)y.amount
									});
								}
								return new
								{
									item_id = itemid,
									shortname = shortname,
									craft_time = num,
									workbench = num2,
									category = text,
									display_name = english,
									despawn_rarity = despawnRarity,
									ingredients = enumerable
								};
							})).AddField("changeset", BuildInfo.Current.Scm.ChangeId)
								.Submit();
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005529 RID: 21801 RVA: 0x001B76A8 File Offset: 0x001B58A8
			private static void PushEntityManifest()
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (!(GameManifest.Current == null))
					{
						BuildInfo buildInfo = BuildInfo.Current;
						bool flag;
						if (buildInfo == null)
						{
							flag = null != null;
						}
						else
						{
							BuildInfo.ScmInfo scm = buildInfo.Scm;
							flag = ((scm != null) ? scm.ChangeId : null) != null;
						}
						if (flag)
						{
							EventRecord eventRecord = EventRecord.New("entity_manifest", true).AddObject("entities", GameManifest.Current.entities.Select((string x) => new
							{
								shortname = Path.GetFileNameWithoutExtension(x),
								prefab_id = StringPool.Get(x.ToLower())
							}));
							string text = "changeset";
							BuildInfo buildInfo2 = BuildInfo.Current;
							eventRecord.AddField(text, ((buildInfo2 != null) ? buildInfo2.Scm.ChangeId : null) ?? "editor").Submit();
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600552A RID: 21802 RVA: 0x001B7778 File Offset: 0x001B5978
			public static void OnFiredProjectile(BasePlayer player, BasePlayer.FiredProjectile projectile, Guid projectileGroupId)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("entity_damage", true).AddField("start_pos", projectile.position).AddField("start_vel", projectile.initialVelocity)
						.AddField("velocity_inherit", projectile.inheritedVelocity);
					string text = "ammo_item";
					ItemDefinition itemDef = projectile.itemDef;
					EventRecord eventRecord2 = eventRecord.AddField(text, (itemDef != null) ? itemDef.shortname : null).AddField("weapon", projectile.weaponSource).AddField("projectile_group", projectileGroupId)
						.AddField("projectile_id", projectile.id)
						.AddField("attacker", player)
						.AddField("look_dir", player.tickViewAngles)
						.AddField("model_state", (player.modelStateTick ?? player.modelState).flags);
					Analytics.Azure.PendingFiredProjectile pendingFiredProjectile = Pool.Get<Analytics.Azure.PendingFiredProjectile>();
					pendingFiredProjectile.Record = eventRecord2;
					pendingFiredProjectile.FiredProjectile = projectile;
					Analytics.Azure.firedProjectiles[new Analytics.Azure.FiredProjectileKey(player.userID, projectile.id)] = pendingFiredProjectile;
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600552B RID: 21803 RVA: 0x001B789C File Offset: 0x001B5A9C
			public static void OnFiredProjectileRemoved(BasePlayer player, BasePlayer.FiredProjectile projectile)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.FiredProjectileKey firedProjectileKey = new Analytics.Azure.FiredProjectileKey(player.userID, projectile.id);
					Analytics.Azure.PendingFiredProjectile pendingFiredProjectile;
					if (!Analytics.Azure.firedProjectiles.TryGetValue(firedProjectileKey, out pendingFiredProjectile))
					{
						UnityEngine.Debug.LogWarning(string.Format("Can't find projectile for player '{0}' with id {1}", player, projectile.id));
					}
					else
					{
						if (!pendingFiredProjectile.Hit)
						{
							EventRecord record = pendingFiredProjectile.Record;
							if (projectile.updates.Count > 0)
							{
								record.AddObject("projectile_updates", projectile.updates);
							}
							record.Submit();
						}
						Pool.Free<Analytics.Azure.PendingFiredProjectile>(ref pendingFiredProjectile);
						Analytics.Azure.firedProjectiles.Remove(firedProjectileKey);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600552C RID: 21804 RVA: 0x001B7954 File Offset: 0x001B5B54
			public static void OnQuarryItem(Analytics.Azure.ResourceMode mode, string item, int amount, MiningQuarry sourceEntity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.AddPendingItems(sourceEntity, item, amount, "quarry", mode == Analytics.Azure.ResourceMode.Consumed, false);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600552D RID: 21805 RVA: 0x001B7998 File Offset: 0x001B5B98
			public static void OnExcavatorProduceItem(Item item, BaseEntity sourceEntity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.AddPendingItems(sourceEntity, item.info.shortname, item.amount, "excavator", false, false);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600552E RID: 21806 RVA: 0x001B79E8 File Offset: 0x001B5BE8
			public static void OnExcavatorConsumeFuel(Item item, int amount, BaseEntity dieselEngine)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "excavator", item.info.shortname, amount, dieselEngine, null, false, null, 0UL, null, null, null);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600552F RID: 21807 RVA: 0x001B7A38 File Offset: 0x001B5C38
			public static void OnCraftItem(string item, int amount, BasePlayer player, BaseEntity workbench, bool inSafezone)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Produced, "craft", item, amount, null, null, inSafezone, workbench, (player != null) ? player.userID : 0UL, null, null, null);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005530 RID: 21808 RVA: 0x001B7A8C File Offset: 0x001B5C8C
			public static void OnCraftMaterialConsumed(string item, int amount, BasePlayer player, BaseEntity workbench, bool inSafezone, string targetItem)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "craft", item, amount, null, null, inSafezone, workbench, (player != null) ? player.userID : 0UL, null, null, targetItem);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005531 RID: 21809 RVA: 0x001B7AE4 File Offset: 0x001B5CE4
			public static void OnConsumableUsed(BasePlayer player, Item item)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("consumeable_used", true).AddField("player", player).AddField("item", item)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005532 RID: 21810 RVA: 0x001B7B3C File Offset: 0x001B5D3C
			public static void OnEntitySpawned(BaseEntity entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.trackedSpawnedIds.Add(entity.net.ID);
					EventRecord.New("entity_spawned", true).AddField("entity", entity).Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005533 RID: 21811 RVA: 0x001B7B9C File Offset: 0x001B5D9C
			private static void TryLogEntityKilled(BaseNetworkable entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (Analytics.trackedSpawnedIds.Contains(entity.net.ID))
					{
						EventRecord.New("entity_killed", true).AddField("entity", entity).Submit();
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005534 RID: 21812 RVA: 0x001B7C04 File Offset: 0x001B5E04
			public static void OnMedUsed(string itemName, BasePlayer player, BasePlayer target)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("med_used", true).AddField("player", player).AddField("target", target)
						.AddField("item_name", itemName)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005535 RID: 21813 RVA: 0x001B7C64 File Offset: 0x001B5E64
			public static void OnCodelockChanged(BasePlayer player, CodeLock codeLock, string oldCode, string newCode, bool isGuest)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("code_change", true).AddField("player", player).AddField("codelock", codeLock)
						.AddField("old_code", oldCode)
						.AddField("new_code", newCode)
						.AddField("is_guest", isGuest)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005536 RID: 21814 RVA: 0x001B7CDC File Offset: 0x001B5EDC
			public static void OnCodeLockEntered(BasePlayer player, CodeLock codeLock, bool isGuest)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("code_enter", true).AddField("player", player).AddField("codelock", codeLock)
						.AddField("is_guest", isGuest)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005537 RID: 21815 RVA: 0x001B7D3C File Offset: 0x001B5F3C
			public static void OnTeamChanged(string change, ulong teamId, ulong teamLeader, ulong user, List<ulong> members)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				List<string> list = Pool.GetList<string>();
				try
				{
					if (members != null)
					{
						foreach (ulong num in members)
						{
							list.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(num));
						}
					}
					EventRecord.New("team_change", true).AddField("team_leader", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(teamLeader)).AddField("team", teamId)
						.AddField("target_user", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(user))
						.AddField("change", change)
						.AddObject("users", list)
						.AddField("member_count", members.Count)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
				Pool.FreeList<string>(ref list);
			}

			// Token: 0x06005538 RID: 21816 RVA: 0x001B7E40 File Offset: 0x001B6040
			public static void OnEntityAuthChanged(BaseEntity entity, BasePlayer player, IEnumerable<ulong> authedList, string change, ulong targetUser)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(targetUser);
					EventRecord.New("auth_change", true).AddField("entity", entity).AddField("player", player)
						.AddField("target", userWipeId)
						.AddObject("auth_list", authedList.Select((ulong x) => SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(x)))
						.AddField("change", change)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005539 RID: 21817 RVA: 0x001B7EEC File Offset: 0x001B60EC
			public static void OnSleepingBagAssigned(BasePlayer player, SleepingBag bag, ulong targetUser)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string text = ((targetUser != 0UL) ? SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(targetUser) : "");
					EventRecord.New("sleeping_bag_assign", true).AddField("entity", bag).AddField("player", player)
						.AddField("target", text)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600553A RID: 21818 RVA: 0x001B7F68 File Offset: 0x001B6168
			public static void OnFallDamage(BasePlayer player, float velocity, float damage)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("fall_damage", true).AddField("player", player).AddField("velocity", velocity)
						.AddField("damage", damage)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600553B RID: 21819 RVA: 0x001B7FC8 File Offset: 0x001B61C8
			public static void OnResearchStarted(BasePlayer player, BaseEntity entity, Item item, int scrapCost)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("research_start", true).AddField("player", player).AddField("item", item.info.shortname)
						.AddField("scrap", scrapCost)
						.AddField("entity", entity)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600553C RID: 21820 RVA: 0x001B8040 File Offset: 0x001B6240
			public static void OnBlueprintLearned(BasePlayer player, ItemDefinition item, string reason, BaseEntity entity = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("blueprint_learned", true).AddField("player", player).AddField("item", item.shortname)
						.AddField("reason", reason)
						.AddField("entity", entity)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600553D RID: 21821 RVA: 0x001B80B0 File Offset: 0x001B62B0
			public static void OnItemRecycled(string item, int amount, Recycler recycler)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "recycler", item, amount, recycler, null, false, null, recycler.LastLootedBy, null, null, null);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600553E RID: 21822 RVA: 0x001B80FC File Offset: 0x001B62FC
			public static void OnRecyclerItemProduced(string item, int amount, Recycler recycler, Item sourceItem)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Produced, "recycler", item, amount, recycler, null, false, null, recycler.LastLootedBy, null, sourceItem, null);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600553F RID: 21823 RVA: 0x001B8148 File Offset: 0x001B6348
			public static void OnGatherItem(string item, int amount, BaseEntity sourceEntity, BasePlayer player, AttackEntity weapon = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Produced, "gather", item, amount, sourceEntity, weapon, false, null, player.userID, null, null, null);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005540 RID: 21824 RVA: 0x001B8194 File Offset: 0x001B6394
			public static void OnFirstLooted(BaseEntity entity, BasePlayer player)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					LootContainer lootContainer;
					LootableCorpse lootableCorpse;
					if ((lootContainer = entity as LootContainer) != null)
					{
						Analytics.Azure.LogItemsLooted(player, entity, lootContainer.inventory, null);
						EventRecord.New("loot_entity", true).AddField("entity", entity).AddField("player", player)
							.AddField("monument", Analytics.Azure.GetMonument(entity))
							.AddField("biome", Analytics.Azure.GetBiome(entity.transform.position))
							.Submit();
					}
					else if ((lootableCorpse = entity as LootableCorpse) != null)
					{
						foreach (ItemContainer itemContainer in lootableCorpse.containers)
						{
							Analytics.Azure.LogItemsLooted(player, entity, itemContainer, null);
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005541 RID: 21825 RVA: 0x001B825C File Offset: 0x001B645C
			public static void OnLootContainerDestroyed(LootContainer entity, BasePlayer player, AttackEntity weapon)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (entity.DropsLoot && player != null && Vector3.Distance(entity.transform.position, player.transform.position) < 50f)
					{
						ItemContainer inventory = entity.inventory;
						if (((inventory != null) ? inventory.itemList : null) != null && entity.inventory.itemList.Count > 0)
						{
							Analytics.Azure.LogItemsLooted(player, entity, entity.inventory, weapon);
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005542 RID: 21826 RVA: 0x001B82F4 File Offset: 0x001B64F4
			public static void OnEntityDestroyed(BaseNetworkable entity)
			{
				Analytics.Azure.TryLogEntityKilled(entity);
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					LootContainer lootContainer;
					if ((lootContainer = entity as LootContainer) != null && lootContainer.FirstLooted)
					{
						foreach (Item item in lootContainer.inventory.itemList)
						{
							Analytics.Azure.OnItemDespawn(lootContainer, item, 3, lootContainer.LastLootedBy);
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005543 RID: 21827 RVA: 0x001B838C File Offset: 0x001B658C
			public static void OnEntityBuilt(BaseEntity entity, BasePlayer player)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("entity_built", true).AddField("player", player).AddField("entity", entity);
					if (entity is SleepingBag)
					{
						int sleepingBagCount = SleepingBag.GetSleepingBagCount(player.userID);
						eventRecord.AddField("bags_active", sleepingBagCount);
						eventRecord.AddField("max_sleeping_bags", ConVar.Server.max_sleeping_bags);
					}
					eventRecord.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005544 RID: 21828 RVA: 0x001B8418 File Offset: 0x001B6618
			public static void OnKeycardSwiped(BasePlayer player, CardReader cardReader)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("keycard_swiped", true).AddField("player", player).AddField("card_level", cardReader.accessLevel)
						.AddField("entity", cardReader)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005545 RID: 21829 RVA: 0x001B8480 File Offset: 0x001B6680
			public static void OnLockedCrateStarted(BasePlayer player, HackableLockedCrate crate)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("hackable_crate_started", true).AddField("player", player).AddField("entity", crate)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005546 RID: 21830 RVA: 0x001B84D8 File Offset: 0x001B66D8
			public static void OnLockedCrateFinished(ulong player, HackableLockedCrate crate)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(player);
					EventRecord.New("hackable_crate_ended", true).AddField("player_userid", userWipeId).AddField("entity", crate)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005547 RID: 21831 RVA: 0x001B8540 File Offset: 0x001B6740
			public static void OnStashHidden(BasePlayer player, StashContainer entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("stash_hidden", true).AddField("player", player).AddField("entity", entity)
						.AddField("owner", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(entity.OwnerID))
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005548 RID: 21832 RVA: 0x001B85B4 File Offset: 0x001B67B4
			public static void OnStashRevealed(BasePlayer player, StashContainer entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("stash_reveal", true).AddField("player", player).AddField("entity", entity)
						.AddField("owner", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(entity.OwnerID))
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005549 RID: 21833 RVA: 0x001B8628 File Offset: 0x001B6828
			public static void OnAntihackViolation(BasePlayer player, int type, string message)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation", true).AddField("player", player).AddField("violation_type", type)
						.AddField("message", message)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600554A RID: 21834 RVA: 0x001B8688 File Offset: 0x001B6888
			public static void OnEyehackViolation(BasePlayer player, Vector3 eyePos)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 6)
						.AddField("eye_pos", eyePos)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600554B RID: 21835 RVA: 0x001B86E8 File Offset: 0x001B68E8
			public static void OnNoclipViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount, Collider collider)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 1)
						.AddField("start_pos", startPos)
						.AddField("end_pos", endPos)
						.AddField("tick_count", tickCount)
						.AddField("collider_name", collider.name)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600554C RID: 21836 RVA: 0x001B8778 File Offset: 0x001B6978
			public static void OnFlyhackViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 3)
						.AddField("start_pos", startPos)
						.AddField("end_pos", endPos)
						.AddField("tick_count", tickCount)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600554D RID: 21837 RVA: 0x001B87F0 File Offset: 0x001B69F0
			public static void OnProjectileHackViolation(BasePlayer.FiredProjectile projectile)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.FiredProjectileKey firedProjectileKey = new Analytics.Azure.FiredProjectileKey(projectile.attacker.userID, projectile.id);
					Analytics.Azure.PendingFiredProjectile pendingFiredProjectile;
					if (!Analytics.Azure.firedProjectiles.TryGetValue(firedProjectileKey, out pendingFiredProjectile))
					{
						UnityEngine.Debug.LogWarning(string.Format("Can't find projectile for player '{0}' with id {1}", projectile.attacker, projectile.id));
					}
					else
					{
						pendingFiredProjectile.Record.AddField("projectile_invalid", true).AddObject("updates", projectile.updates);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600554E RID: 21838 RVA: 0x001B888C File Offset: 0x001B6A8C
			public static void OnSpeedhackViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 2)
						.AddField("start_pos", startPos)
						.AddField("end_pos", endPos)
						.AddField("tick_count", tickCount)
						.AddField("distance", Vector3.Distance(startPos, endPos))
						.AddField("distance_2d", Vector3Ex.Distance2D(startPos, endPos))
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600554F RID: 21839 RVA: 0x001B8924 File Offset: 0x001B6B24
			public static void OnTerrainHackViolation(BasePlayer player)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 10)
						.AddField("seed", global::World.Seed)
						.AddField("size", global::World.Size)
						.AddField("map_url", global::World.Url)
						.AddField("map_checksum", global::World.Checksum)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005550 RID: 21840 RVA: 0x001B89B8 File Offset: 0x001B6BB8
			public static void OnEntityTakeDamage(HitInfo info, bool isDeath)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					BasePlayer initiatorPlayer = info.InitiatorPlayer;
					BasePlayer basePlayer = info.HitEntity as BasePlayer;
					if (!(info.Initiator == null) || isDeath)
					{
						if ((!(initiatorPlayer == null) && !initiatorPlayer.IsNpc && !initiatorPlayer.IsBot) || (!(basePlayer == null) && !basePlayer.IsNpc && !basePlayer.IsBot))
						{
							EventRecord eventRecord = null;
							float num = -1f;
							float num2 = -1f;
							if (initiatorPlayer != null)
							{
								if (info.IsProjectile())
								{
									Analytics.Azure.FiredProjectileKey firedProjectileKey = new Analytics.Azure.FiredProjectileKey(initiatorPlayer.userID, info.ProjectileID);
									Analytics.Azure.PendingFiredProjectile pendingFiredProjectile;
									if (Analytics.Azure.firedProjectiles.TryGetValue(firedProjectileKey, out pendingFiredProjectile))
									{
										eventRecord = pendingFiredProjectile.Record;
										num = Vector3.Distance(info.HitNormalWorld, pendingFiredProjectile.FiredProjectile.initialPosition);
										num = Vector3Ex.Distance2D(info.HitNormalWorld, pendingFiredProjectile.FiredProjectile.initialPosition);
										pendingFiredProjectile.Hit = info.DidHit;
									}
								}
								else
								{
									num = Vector3.Distance(info.HitNormalWorld, initiatorPlayer.eyes.position);
									num = Vector3Ex.Distance2D(info.HitNormalWorld, initiatorPlayer.eyes.position);
								}
							}
							if (eventRecord == null)
							{
								eventRecord = EventRecord.New("entity_damage", true);
							}
							eventRecord.AddField("is_hit", true).AddField("is_headshot", info.isHeadshot).AddField("victim", info.HitEntity)
								.AddField("damage", info.damageTypes.Total())
								.AddField("damage_type", info.damageTypes.GetMajorityDamageType().ToString())
								.AddField("pos_world", info.HitPositionWorld)
								.AddField("pos_local", info.HitPositionLocal)
								.AddField("point_start", info.PointStart)
								.AddField("point_end", info.PointEnd)
								.AddField("normal_world", info.HitNormalWorld)
								.AddField("normal_local", info.HitNormalLocal)
								.AddField("distance_cl", info.ProjectileDistance)
								.AddField("distance", num)
								.AddField("distance_2d", num2);
							if (!info.IsProjectile())
							{
								eventRecord.AddField("weapon", info.Weapon);
								eventRecord.AddField("attacker", info.Initiator);
							}
							if (info.HitBone != 0U)
							{
								eventRecord.AddField("bone", info.HitBone).AddField("bone_name", info.boneName).AddField("hit_area", (int)info.boneArea);
							}
							if (info.ProjectileID != 0)
							{
								eventRecord.AddField("projectile_id", info.ProjectileID).AddField("projectile_integrity", info.ProjectileIntegrity).AddField("projectile_hits", info.ProjectileHits)
									.AddField("trajectory_mismatch", info.ProjectileTrajectoryMismatch)
									.AddField("travel_time", info.ProjectileTravelTime)
									.AddField("projectile_velocity", info.ProjectileVelocity)
									.AddField("projectile_prefab", info.ProjectilePrefab.name);
							}
							if (initiatorPlayer != null && !info.IsProjectile())
							{
								eventRecord.AddField("attacker_eye_pos", initiatorPlayer.eyes.position);
								eventRecord.AddField("attacker_eye_dir", initiatorPlayer.eyes.BodyForward());
								if (initiatorPlayer.GetType() == typeof(BasePlayer))
								{
									eventRecord.AddField("attacker_life", initiatorPlayer.respawnId);
								}
								if (isDeath)
								{
									eventRecord.AddObject("attacker_worn", initiatorPlayer.inventory.containerWear.itemList.Select((Item x) => new Analytics.Azure.SimpleItemAmount(x)));
									eventRecord.AddObject("attacker_hotbar", initiatorPlayer.inventory.containerBelt.itemList.Select((Item x) => new Analytics.Azure.SimpleItemAmount(x)));
								}
							}
							if (basePlayer != null)
							{
								eventRecord.AddField("victim_life", basePlayer.respawnId);
								eventRecord.AddObject("victim_worn", basePlayer.inventory.containerWear.itemList.Select((Item x) => new Analytics.Azure.SimpleItemAmount(x)));
								eventRecord.AddObject("victim_hotbar", basePlayer.inventory.containerBelt.itemList.Select((Item x) => new Analytics.Azure.SimpleItemAmount(x)));
							}
							eventRecord.Submit();
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005551 RID: 21841 RVA: 0x001B8E90 File Offset: 0x001B7090
			public static void OnPlayerRespawned(BasePlayer player, BaseEntity targetEntity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("player_respawn", true).AddField("player", player).AddField("bag", targetEntity)
						.AddField("life_id", player.respawnId)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005552 RID: 21842 RVA: 0x001B8EF8 File Offset: 0x001B70F8
			public static void OnExplosiveLaunched(BasePlayer player, BaseEntity explosive, BaseEntity launcher = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("explosive_launch", true).AddField("player", player).AddField("explosive", explosive)
						.AddField("explosive_velocity", explosive.GetWorldVelocity())
						.AddField("explosive_direction", explosive.GetWorldVelocity().normalized);
					if (launcher != null)
					{
						eventRecord.AddField("launcher", launcher);
					}
					eventRecord.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005553 RID: 21843 RVA: 0x001B8F90 File Offset: 0x001B7190
			public static void OnExplosion(TimedExplosive explosive)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("explosion", true).AddField("entity", explosive).Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005554 RID: 21844 RVA: 0x001B8FDC File Offset: 0x001B71DC
			public static void OnItemDespawn(BaseEntity itemContainer, Item item, int dropReason, ulong userId)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("item_despawn", true).AddField("entity", itemContainer).AddField("item", item)
						.AddField("drop_reason", dropReason);
					if (userId != 0UL)
					{
						eventRecord.AddField("player_userid", userId);
					}
					eventRecord.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005555 RID: 21845 RVA: 0x001B9050 File Offset: 0x001B7250
			public static void OnItemDropped(BasePlayer player, WorldItem entity, DroppedItem.DropReasonEnum dropReason)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_drop", true).AddField("player", player).AddField("entity", entity)
						.AddField("item", entity.GetItem())
						.AddField("drop_reason", (int)dropReason)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005556 RID: 21846 RVA: 0x001B90C0 File Offset: 0x001B72C0
			public static void OnItemPickup(BasePlayer player, WorldItem entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_pickup", true).AddField("player", player).AddField("entity", entity)
						.AddField("item", entity.GetItem())
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005557 RID: 21847 RVA: 0x001B9128 File Offset: 0x001B7328
			public static void OnPlayerConnected(Connection connection)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(connection.userid);
					EventRecord.New("player_connect", true).AddField("player_userid", userWipeId).AddField("username", connection.username)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005558 RID: 21848 RVA: 0x001B9198 File Offset: 0x001B7398
			public static void OnPlayerDisconnected(Connection connection, string reason)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(connection.userid);
					EventRecord.New("player_disconnect", true).AddField("player_userid", userWipeId).AddField("username", connection.username)
						.AddField("reason", reason)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005559 RID: 21849 RVA: 0x001B9214 File Offset: 0x001B7414
			public static void OnEntityPickedUp(BasePlayer player, BaseEntity entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("entity_pickup", true).AddField("player", player).AddField("entity", entity)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600555A RID: 21850 RVA: 0x001B926C File Offset: 0x001B746C
			public static void OnChatMessage(BasePlayer player, string message, int channel)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("chat", true).AddField("player", player).AddField("message", message)
						.AddField("channel", channel)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600555B RID: 21851 RVA: 0x001B92CC File Offset: 0x001B74CC
			public static void OnVendingMachineOrderChanged(BasePlayer player, VendingMachine vendingMachine, int sellItemId, int sellAmount, bool sellingBp, int buyItemId, int buyAmount, bool buyingBp, bool added)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellItemId);
					ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(buyItemId);
					EventRecord.New("vending_changed", true).AddField("player", player).AddField("entity", vendingMachine)
						.AddField("sell_item", itemDefinition.shortname)
						.AddField("sell_amount", sellAmount)
						.AddField("buy_item", itemDefinition2.shortname)
						.AddField("buy_amount", buyAmount)
						.AddField("is_selling_bp", sellingBp)
						.AddField("is_buying_bp", buyingBp)
						.AddField("change", added ? "added" : "removed")
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600555C RID: 21852 RVA: 0x001B939C File Offset: 0x001B759C
			public static void OnBuyFromVendingMachine(BasePlayer player, VendingMachine vendingMachine, int sellItemId, int sellAmount, bool sellingBp, int buyItemId, int buyAmount, bool buyingBp, int numberOfTransactions, BaseEntity drone = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellItemId);
					ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(buyItemId);
					EventRecord.New("vending_sale", true).AddField("player", player).AddField("entity", vendingMachine)
						.AddField("sell_item", itemDefinition.shortname)
						.AddField("sell_amount", sellAmount)
						.AddField("buy_item", itemDefinition2.shortname)
						.AddField("buy_amount", buyAmount)
						.AddField("transactions", numberOfTransactions)
						.AddField("is_selling_bp", sellingBp)
						.AddField("is_buying_bp", buyingBp)
						.AddField("drone_terminal", drone)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600555D RID: 21853 RVA: 0x001B9468 File Offset: 0x001B7668
			public static void OnNPCVendor(BasePlayer player, NPCTalking vendor, int scrapCost, string action)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("npc_vendor", true).AddField("player", player).AddField("vendor", vendor)
						.AddField("scrap_amount", scrapCost)
						.AddField("action", action)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600555E RID: 21854 RVA: 0x001B94D4 File Offset: 0x001B76D4
			private static void LogItemsLooted(BasePlayer looter, BaseEntity entity, ItemContainer container, AttackEntity tool = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (!(entity == null) && container != null)
					{
						foreach (Item item in container.itemList)
						{
							if (item != null)
							{
								Analytics.Azure.ResourceMode resourceMode = Analytics.Azure.ResourceMode.Produced;
								string text = "loot";
								string shortname = item.info.shortname;
								int amount = item.amount;
								ulong num = ((looter != null) ? looter.userID : 0UL);
								Analytics.Azure.LogResource(resourceMode, text, shortname, amount, entity, tool, false, null, num, null, null, null);
							}
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600555F RID: 21855 RVA: 0x001B9584 File Offset: 0x001B7784
			public static void LogResource(Analytics.Azure.ResourceMode mode, string category, string itemName, int amount, BaseEntity sourceEntity = null, AttackEntity tool = null, bool safezone = false, BaseEntity workbench = null, ulong steamId = 0UL, string sourceEntityPrefab = null, Item sourceItem = null, string targetItem = null)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("item_event", true).AddField("item_mode", mode.ToString()).AddField("category", category)
						.AddField("item_name", itemName)
						.AddField("amount", amount);
					if (sourceEntity != null)
					{
						eventRecord.AddField("entity", sourceEntity);
						string biome = Analytics.Azure.GetBiome(sourceEntity.transform.position);
						if (biome != null)
						{
							eventRecord.AddField("biome", biome);
						}
						if (Analytics.Azure.IsOcean(sourceEntity.transform.position))
						{
							eventRecord.AddField("ocean", true);
						}
						string monument = Analytics.Azure.GetMonument(sourceEntity);
						if (monument != null)
						{
							eventRecord.AddField("monument", monument);
						}
					}
					if (sourceEntityPrefab != null)
					{
						eventRecord.AddField("entity_prefab", sourceEntityPrefab);
					}
					if (tool != null)
					{
						eventRecord.AddField("tool", tool);
					}
					if (safezone)
					{
						eventRecord.AddField("safezone", true);
					}
					if (workbench != null)
					{
						eventRecord.AddField("workbench", workbench);
					}
					GrowableEntity growableEntity;
					if ((growableEntity = sourceEntity as GrowableEntity) != null)
					{
						eventRecord.AddField("genes", Analytics.Azure.GetGenesAsString(growableEntity));
					}
					if (sourceItem != null)
					{
						eventRecord.AddField("source_item", sourceItem.info.shortname);
					}
					if (targetItem != null)
					{
						eventRecord.AddField("target_item", targetItem);
					}
					if (steamId != 0UL)
					{
						string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(steamId);
						eventRecord.AddField("player_userid", userWipeId);
					}
					eventRecord.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005560 RID: 21856 RVA: 0x001B9748 File Offset: 0x001B7948
			public static void OnSkinChanged(BasePlayer player, RepairBench repairBench, Item item, ulong workshopId)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_skinned", true).AddField("player", player).AddField("entity", repairBench)
						.AddField("item", item)
						.AddField("new_skin", workshopId)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005561 RID: 21857 RVA: 0x001B97B4 File Offset: 0x001B79B4
			public static void OnItemRepaired(BasePlayer player, BaseEntity repairBench, Item itemToRepair, float conditionBefore, float maxConditionBefore)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_repair", true).AddField("player", player).AddField("entity", repairBench)
						.AddField("item", itemToRepair)
						.AddField("old_condition", conditionBefore)
						.AddField("old_max_condition", maxConditionBefore)
						.AddField("max_condition", itemToRepair.maxConditionNormalized)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005562 RID: 21858 RVA: 0x001B983C File Offset: 0x001B7A3C
			public static void OnEntityRepaired(BasePlayer player, BaseEntity entity, float healthBefore, float healthAfter)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("entity_repair", true).AddField("player", player).AddField("entity", entity)
						.AddField("healing", healthAfter - healthBefore)
						.AddField("health_before", healthBefore)
						.AddField("health_after", healthAfter)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005563 RID: 21859 RVA: 0x001B98B4 File Offset: 0x001B7AB4
			public static void OnBuildingBlockUpgraded(BasePlayer player, BuildingBlock buildingBlock, BuildingGrade.Enum targetGrade)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("block_upgrade", true).AddField("player", player).AddField("entity", buildingBlock)
						.AddField("old_grade", (int)buildingBlock.grade)
						.AddField("new_grade", (int)targetGrade)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005564 RID: 21860 RVA: 0x001B9924 File Offset: 0x001B7B24
			public static void OnBuildingBlockDemolished(BasePlayer player, BuildingBlock buildingBlock)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("block_demolish", true).AddField("player", player).AddField("entity", buildingBlock)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005565 RID: 21861 RVA: 0x001B997C File Offset: 0x001B7B7C
			public static void OnPlayerInitializedWipeId(ulong userId, string wipeId)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("player_wipe_id_set", true).AddField("user_id", userId).AddField("player_wipe_id", wipeId)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005566 RID: 21862 RVA: 0x001B99D4 File Offset: 0x001B7BD4
			public static void OnFreeUnderwaterCrate(BasePlayer player, FreeableLootContainer freeableLootContainer)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("crate_untied", true).AddField("player", player).AddField("entity", freeableLootContainer)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005567 RID: 21863 RVA: 0x001B9A2C File Offset: 0x001B7C2C
			public static void OnVehiclePurchased(BasePlayer player, BaseEntity vehicle)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("vehicle_purchase", true).AddField("player", player).AddField("entity", vehicle)
						.AddField("price", vehicle)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005568 RID: 21864 RVA: 0x001B9A8C File Offset: 0x001B7C8C
			public static void OnMissionComplete(BasePlayer player, BaseMission mission, BaseMission.MissionFailReason? failReason = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("mission_complete", true).AddField("player", player).AddField("mission", mission.shortname)
						.AddField("mission_succeed", true);
					if (failReason != null)
					{
						eventRecord.AddField("mission_succeed", false).AddField("fail_reason", failReason.Value.ToString());
					}
					eventRecord.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x06005569 RID: 21865 RVA: 0x001B9B28 File Offset: 0x001B7D28
			public static void OnGamblingResult(BasePlayer player, BaseEntity entity, int scrapPaid, int scrapRecieved, Guid? gambleGroupId = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("gambing", true).AddField("player", player).AddField("entity", entity)
						.AddField("scrap_input", scrapPaid)
						.AddField("scrap_output", scrapRecieved);
					if (gambleGroupId != null)
					{
						eventRecord.AddField("gamble_grouping", gambleGroupId.Value);
					}
					eventRecord.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600556A RID: 21866 RVA: 0x001B9BB4 File Offset: 0x001B7DB4
			public static void OnPlayerPinged(BasePlayer player, BasePlayer.PingType type, bool wasViaWheel)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("player_pinged", true).AddField("player", player).AddField("pingType", (int)type)
						.AddField("viaWheel", wasViaWheel)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x0600556B RID: 21867 RVA: 0x001B9C14 File Offset: 0x001B7E14
			public static void OnBagUnclaimed(BasePlayer player, SleepingBag bag)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("bag_unclaim", true).AddField("player", player).AddField("entity", bag)
						.Submit();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}

			// Token: 0x040050B2 RID: 20658
			private static Dictionary<int, string> geneCache = new Dictionary<int, string>();

			// Token: 0x040050B3 RID: 20659
			public static int MaxMSPerFrame = 5;

			// Token: 0x040050B4 RID: 20660
			private static Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData> pendingItems = new Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>();

			// Token: 0x040050B5 RID: 20661
			private static Dictionary<Analytics.Azure.FiredProjectileKey, Analytics.Azure.PendingFiredProjectile> firedProjectiles = new Dictionary<Analytics.Azure.FiredProjectileKey, Analytics.Azure.PendingFiredProjectile>();

			// Token: 0x02000FF0 RID: 4080
			private struct EntitySumItem
			{
				// Token: 0x040051C4 RID: 20932
				public uint PrefabId;

				// Token: 0x040051C5 RID: 20933
				public int Count;

				// Token: 0x040051C6 RID: 20934
				public int Grade;
			}

			// Token: 0x02000FF1 RID: 4081
			private struct EntityKey : IEquatable<Analytics.Azure.EntityKey>
			{
				// Token: 0x060055E6 RID: 21990 RVA: 0x001BB327 File Offset: 0x001B9527
				public bool Equals(Analytics.Azure.EntityKey other)
				{
					return this.PrefabId == other.PrefabId && this.Grade == other.Grade;
				}

				// Token: 0x060055E7 RID: 21991 RVA: 0x001BB347 File Offset: 0x001B9547
				public override int GetHashCode()
				{
					return (17 * 23 + this.PrefabId.GetHashCode()) * 31 + this.Grade.GetHashCode();
				}

				// Token: 0x040051C7 RID: 20935
				public uint PrefabId;

				// Token: 0x040051C8 RID: 20936
				public int Grade;
			}

			// Token: 0x02000FF2 RID: 4082
			private class PendingItemsData : Pool.IPooled
			{
				// Token: 0x060055E8 RID: 21992 RVA: 0x001BB369 File Offset: 0x001B9569
				public void EnterPool()
				{
					this.Key = default(Analytics.Azure.PendingItemsKey);
					this.amount = 0;
					this.category = null;
				}

				// Token: 0x060055E9 RID: 21993 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}

				// Token: 0x040051C9 RID: 20937
				public Analytics.Azure.PendingItemsKey Key;

				// Token: 0x040051CA RID: 20938
				public int amount;

				// Token: 0x040051CB RID: 20939
				public string category;
			}

			// Token: 0x02000FF3 RID: 4083
			private struct PendingItemsKey : IEquatable<Analytics.Azure.PendingItemsKey>
			{
				// Token: 0x060055EB RID: 21995 RVA: 0x001BB388 File Offset: 0x001B9588
				public bool Equals(Analytics.Azure.PendingItemsKey other)
				{
					return this.Item == other.Item && this.Entity == other.Entity && this.EntityId == other.EntityId && this.Consumed == other.Consumed && this.Category == other.Category;
				}

				// Token: 0x060055EC RID: 21996 RVA: 0x001BB3F0 File Offset: 0x001B95F0
				public override int GetHashCode()
				{
					return ((((17 * 23 + this.Item.GetHashCode()) * 31 + this.Consumed.GetHashCode()) * 37 + this.Entity.GetHashCode()) * 47 + this.Category.GetHashCode()) * 53 + this.EntityId.GetHashCode();
				}

				// Token: 0x040051CC RID: 20940
				public string Item;

				// Token: 0x040051CD RID: 20941
				public bool Consumed;

				// Token: 0x040051CE RID: 20942
				public string Entity;

				// Token: 0x040051CF RID: 20943
				public string Category;

				// Token: 0x040051D0 RID: 20944
				public NetworkableId EntityId;
			}

			// Token: 0x02000FF4 RID: 4084
			private class PlayerAggregate : Pool.IPooled
			{
				// Token: 0x060055ED RID: 21997 RVA: 0x001BB450 File Offset: 0x001B9650
				public void EnterPool()
				{
					this.UserId = null;
					this.Position = default(Vector3);
					this.Direction = default(Vector3);
					this.Hotbar.Clear();
					this.Worn.Clear();
					this.ActiveItem = null;
				}

				// Token: 0x060055EE RID: 21998 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}

				// Token: 0x040051D1 RID: 20945
				public string UserId;

				// Token: 0x040051D2 RID: 20946
				public Vector3 Position;

				// Token: 0x040051D3 RID: 20947
				public Vector3 Direction;

				// Token: 0x040051D4 RID: 20948
				public List<string> Hotbar = new List<string>();

				// Token: 0x040051D5 RID: 20949
				public List<string> Worn = new List<string>();

				// Token: 0x040051D6 RID: 20950
				public string ActiveItem;
			}

			// Token: 0x02000FF5 RID: 4085
			private class TeamInfo : Pool.IPooled
			{
				// Token: 0x060055F0 RID: 22000 RVA: 0x001BB4AC File Offset: 0x001B96AC
				public void EnterPool()
				{
					this.online.Clear();
					this.offline.Clear();
					this.member_count = 0;
				}

				// Token: 0x060055F1 RID: 22001 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}

				// Token: 0x040051D7 RID: 20951
				public List<string> online = new List<string>();

				// Token: 0x040051D8 RID: 20952
				public List<string> offline = new List<string>();

				// Token: 0x040051D9 RID: 20953
				public int member_count;
			}

			// Token: 0x02000FF6 RID: 4086
			public enum ResourceMode
			{
				// Token: 0x040051DB RID: 20955
				Produced,
				// Token: 0x040051DC RID: 20956
				Consumed
			}

			// Token: 0x02000FF7 RID: 4087
			private static class EventIds
			{
				// Token: 0x040051DD RID: 20957
				public const string EntityBuilt = "entity_built";

				// Token: 0x040051DE RID: 20958
				public const string EntityPickup = "entity_pickup";

				// Token: 0x040051DF RID: 20959
				public const string EntityDamage = "entity_damage";

				// Token: 0x040051E0 RID: 20960
				public const string PlayerRespawn = "player_respawn";

				// Token: 0x040051E1 RID: 20961
				public const string ExplosiveLaunched = "explosive_launch";

				// Token: 0x040051E2 RID: 20962
				public const string Explosion = "explosion";

				// Token: 0x040051E3 RID: 20963
				public const string ItemEvent = "item_event";

				// Token: 0x040051E4 RID: 20964
				public const string EntitySum = "entity_sum";

				// Token: 0x040051E5 RID: 20965
				public const string ItemSum = "item_sum";

				// Token: 0x040051E6 RID: 20966
				public const string ItemDespawn = "item_despawn";

				// Token: 0x040051E7 RID: 20967
				public const string ItemDropped = "item_drop";

				// Token: 0x040051E8 RID: 20968
				public const string ItemPickup = "item_pickup";

				// Token: 0x040051E9 RID: 20969
				public const string AntihackViolation = "antihack_violation";

				// Token: 0x040051EA RID: 20970
				public const string AntihackViolationDetailed = "antihack_violation_detailed";

				// Token: 0x040051EB RID: 20971
				public const string PlayerConnect = "player_connect";

				// Token: 0x040051EC RID: 20972
				public const string PlayerDisconnect = "player_disconnect";

				// Token: 0x040051ED RID: 20973
				public const string ConsumableUsed = "consumeable_used";

				// Token: 0x040051EE RID: 20974
				public const string MedUsed = "med_used";

				// Token: 0x040051EF RID: 20975
				public const string ResearchStarted = "research_start";

				// Token: 0x040051F0 RID: 20976
				public const string BlueprintLearned = "blueprint_learned";

				// Token: 0x040051F1 RID: 20977
				public const string TeamChanged = "team_change";

				// Token: 0x040051F2 RID: 20978
				public const string EntityAuthChange = "auth_change";

				// Token: 0x040051F3 RID: 20979
				public const string VendingOrderChanged = "vending_changed";

				// Token: 0x040051F4 RID: 20980
				public const string VendingSale = "vending_sale";

				// Token: 0x040051F5 RID: 20981
				public const string ChatMessage = "chat";

				// Token: 0x040051F6 RID: 20982
				public const string BlockUpgrade = "block_upgrade";

				// Token: 0x040051F7 RID: 20983
				public const string BlockDemolish = "block_demolish";

				// Token: 0x040051F8 RID: 20984
				public const string ItemRepair = "item_repair";

				// Token: 0x040051F9 RID: 20985
				public const string EntityRepair = "entity_repair";

				// Token: 0x040051FA RID: 20986
				public const string ItemSkinned = "item_skinned";

				// Token: 0x040051FB RID: 20987
				public const string ItemAggregate = "item_aggregate";

				// Token: 0x040051FC RID: 20988
				public const string CodelockChanged = "code_change";

				// Token: 0x040051FD RID: 20989
				public const string CodelockEntered = "code_enter";

				// Token: 0x040051FE RID: 20990
				public const string SleepingBagAssign = "sleeping_bag_assign";

				// Token: 0x040051FF RID: 20991
				public const string FallDamage = "fall_damage";

				// Token: 0x04005200 RID: 20992
				public const string PlayerWipeIdSet = "player_wipe_id_set";

				// Token: 0x04005201 RID: 20993
				public const string ServerInfo = "server_info";

				// Token: 0x04005202 RID: 20994
				public const string UnderwaterCrateUntied = "crate_untied";

				// Token: 0x04005203 RID: 20995
				public const string VehiclePurchased = "vehicle_purchase";

				// Token: 0x04005204 RID: 20996
				public const string NPCVendor = "npc_vendor";

				// Token: 0x04005205 RID: 20997
				public const string BlueprintsOnline = "blueprint_aggregate_online";

				// Token: 0x04005206 RID: 20998
				public const string PlayerPositions = "player_positions";

				// Token: 0x04005207 RID: 20999
				public const string ProjectileInvalid = "projectile_invalid";

				// Token: 0x04005208 RID: 21000
				public const string ItemDefinitions = "item_definitions";

				// Token: 0x04005209 RID: 21001
				public const string KeycardSwiped = "keycard_swiped";

				// Token: 0x0400520A RID: 21002
				public const string EntitySpawned = "entity_spawned";

				// Token: 0x0400520B RID: 21003
				public const string EntityKilled = "entity_killed";

				// Token: 0x0400520C RID: 21004
				public const string HackableCrateStarted = "hackable_crate_started";

				// Token: 0x0400520D RID: 21005
				public const string HackableCrateEnded = "hackable_crate_ended";

				// Token: 0x0400520E RID: 21006
				public const string StashHidden = "stash_hidden";

				// Token: 0x0400520F RID: 21007
				public const string StashRevealed = "stash_reveal";

				// Token: 0x04005210 RID: 21008
				public const string EntityManifest = "entity_manifest";

				// Token: 0x04005211 RID: 21009
				public const string LootEntity = "loot_entity";

				// Token: 0x04005212 RID: 21010
				public const string OnlineTeams = "online_teams";

				// Token: 0x04005213 RID: 21011
				public const string Gambling = "gambing";

				// Token: 0x04005214 RID: 21012
				public const string MissionComplete = "mission_complete";

				// Token: 0x04005215 RID: 21013
				public const string PlayerPinged = "player_pinged";

				// Token: 0x04005216 RID: 21014
				public const string BagUnclaim = "bag_unclaim";
			}

			// Token: 0x02000FF8 RID: 4088
			private struct SimpleItemAmount
			{
				// Token: 0x060055F3 RID: 22003 RVA: 0x001BB4E9 File Offset: 0x001B96E9
				public SimpleItemAmount(Item item)
				{
					this.ItemName = item.info.shortname;
					this.Amount = item.amount;
					this.Skin = item.skin;
					this.Condition = item.conditionNormalized;
				}

				// Token: 0x04005217 RID: 21015
				public string ItemName;

				// Token: 0x04005218 RID: 21016
				public int Amount;

				// Token: 0x04005219 RID: 21017
				public ulong Skin;

				// Token: 0x0400521A RID: 21018
				public float Condition;
			}

			// Token: 0x02000FF9 RID: 4089
			private struct FiredProjectileKey : IEquatable<Analytics.Azure.FiredProjectileKey>
			{
				// Token: 0x060055F4 RID: 22004 RVA: 0x001BB520 File Offset: 0x001B9720
				public FiredProjectileKey(ulong userId, int projectileId)
				{
					this.UserId = userId;
					this.ProjectileId = projectileId;
				}

				// Token: 0x060055F5 RID: 22005 RVA: 0x001BB530 File Offset: 0x001B9730
				public bool Equals(Analytics.Azure.FiredProjectileKey other)
				{
					return other.UserId == this.UserId && other.ProjectileId == this.ProjectileId;
				}

				// Token: 0x0400521B RID: 21019
				public ulong UserId;

				// Token: 0x0400521C RID: 21020
				public int ProjectileId;
			}

			// Token: 0x02000FFA RID: 4090
			private class PendingFiredProjectile : Pool.IPooled
			{
				// Token: 0x060055F6 RID: 22006 RVA: 0x001BB550 File Offset: 0x001B9750
				public void EnterPool()
				{
					this.Hit = false;
					this.Record = null;
					this.FiredProjectile = null;
				}

				// Token: 0x060055F7 RID: 22007 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}

				// Token: 0x0400521D RID: 21021
				public EventRecord Record;

				// Token: 0x0400521E RID: 21022
				public BasePlayer.FiredProjectile FiredProjectile;

				// Token: 0x0400521F RID: 21023
				public bool Hit;
			}
		}

		// Token: 0x02000F98 RID: 3992
		public class AzureWebInterface
		{
			// Token: 0x1700073D RID: 1853
			// (get) Token: 0x0600556D RID: 21869 RVA: 0x001B9C92 File Offset: 0x001B7E92
			public int PendingCount
			{
				get
				{
					return this.pending.Count;
				}
			}

			// Token: 0x0600556E RID: 21870 RVA: 0x001B9CA0 File Offset: 0x001B7EA0
			public AzureWebInterface(bool isClient)
			{
				this.IsClient = isClient;
			}

			// Token: 0x0600556F RID: 21871 RVA: 0x001B9CF8 File Offset: 0x001B7EF8
			public void EnqueueEvent(EventRecord point)
			{
				DateTime utcNow = DateTime.UtcNow;
				this.pending.Add(point);
				if (this.pending.Count > this.FlushSize || utcNow > this.nextFlush)
				{
					Analytics.AzureWebInterface.<>c__DisplayClass13_0 CS$<>8__locals1 = new Analytics.AzureWebInterface.<>c__DisplayClass13_0();
					CS$<>8__locals1.<>4__this = this;
					this.nextFlush = utcNow.Add(this.FlushDelay);
					CS$<>8__locals1.toUpload = this.pending;
					Task.Run(delegate
					{
						Analytics.AzureWebInterface.<>c__DisplayClass13_0.<<EnqueueEvent>b__0>d <<EnqueueEvent>b__0>d;
						<<EnqueueEvent>b__0>d.<>4__this = CS$<>8__locals1;
						<<EnqueueEvent>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<EnqueueEvent>b__0>d.<>1__state = -1;
						AsyncTaskMethodBuilder <>t__builder = <<EnqueueEvent>b__0>d.<>t__builder;
						<>t__builder.Start<Analytics.AzureWebInterface.<>c__DisplayClass13_0.<<EnqueueEvent>b__0>d>(ref <<EnqueueEvent>b__0>d);
						return <<EnqueueEvent>b__0>d.<>t__builder.Task;
					});
					this.pending = Pool.GetList<EventRecord>();
				}
			}

			// Token: 0x06005570 RID: 21872 RVA: 0x001B9D80 File Offset: 0x001B7F80
			private void SerializeEvents(List<EventRecord> records, MemoryStream stream)
			{
				int num = 0;
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
				{
					streamWriter.Write("[");
					foreach (EventRecord eventRecord in records)
					{
						this.SerializeEvent(eventRecord, streamWriter, num);
						num++;
					}
					streamWriter.Write("]");
					streamWriter.Flush();
				}
			}

			// Token: 0x06005571 RID: 21873 RVA: 0x001B9E1C File Offset: 0x001B801C
			private void SerializeEvent(EventRecord record, StreamWriter writer, int index)
			{
				if (index > 0)
				{
					writer.Write(',');
				}
				writer.Write("{\"Timestamp\":\"");
				writer.Write(record.Timestamp.ToString("o"));
				writer.Write("\",\"Data\":{");
				bool flag = true;
				foreach (EventRecordField eventRecordField in record.Data)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						writer.Write(',');
					}
					writer.Write("\"");
					writer.Write(eventRecordField.Key1);
					if (eventRecordField.Key2 != null)
					{
						writer.Write(eventRecordField.Key2);
					}
					writer.Write("\":");
					if (!eventRecordField.IsObject)
					{
						writer.Write('"');
					}
					if (eventRecordField.String != null)
					{
						if (eventRecordField.IsObject)
						{
							writer.Write(eventRecordField.String);
						}
						else
						{
							string @string = eventRecordField.String;
							int length = eventRecordField.String.Length;
							for (int i = 0; i < length; i++)
							{
								char c = @string[i];
								if (c == '\\' || c == '"')
								{
									writer.Write('\\');
									writer.Write(c);
								}
								else if (c == '\n')
								{
									writer.Write("\\n");
								}
								else if (c == '\r')
								{
									writer.Write("\\r");
								}
								else if (c == '\t')
								{
									writer.Write("\\t");
								}
								else
								{
									writer.Write(c);
								}
							}
						}
					}
					else if (eventRecordField.Float != null)
					{
						writer.Write(eventRecordField.Float.Value);
					}
					else if (eventRecordField.Number != null)
					{
						writer.Write(eventRecordField.Number.Value);
					}
					else if (eventRecordField.Guid != null)
					{
						writer.Write(eventRecordField.Guid.Value.ToString("N"));
					}
					else if (eventRecordField.Vector != null)
					{
						writer.Write('(');
						Vector3 value = eventRecordField.Vector.Value;
						writer.Write(value.x);
						writer.Write(',');
						writer.Write(value.y);
						writer.Write(',');
						writer.Write(value.z);
						writer.Write(')');
					}
					if (!eventRecordField.IsObject)
					{
						writer.Write("\"");
					}
				}
				writer.Write('}');
				writer.Write('}');
			}

			// Token: 0x06005572 RID: 21874 RVA: 0x001BA0C0 File Offset: 0x001B82C0
			private async Task UploadAsync(List<EventRecord> records)
			{
				MemoryStream stream = Pool.Get<MemoryStream>();
				stream.Position = 0L;
				stream.SetLength(0L);
				try
				{
					this.SerializeEvents(records, stream);
					AuthTicket ticket = null;
					for (int attempt = 0; attempt < this.MaxRetries; attempt++)
					{
						try
						{
							using (ByteArrayContent content = new ByteArrayContent(stream.GetBuffer(), 0, (int)stream.Length))
							{
								content.Headers.ContentType = Analytics.AzureWebInterface.JsonContentType;
								if (!string.IsNullOrEmpty(Analytics.AnalyticsSecret))
								{
									content.Headers.Add(Analytics.AnalyticsHeader, Analytics.AnalyticsSecret);
								}
								else
								{
									content.Headers.Add(Analytics.AnalyticsHeader, Analytics.AnalyticsPublicKey);
								}
								if (!this.IsClient)
								{
									content.Headers.Add("X-SERVER-IP", Network.Net.sv.ip);
									content.Headers.Add("X-SERVER-PORT", Network.Net.sv.port.ToString());
								}
								if (Analytics.UploadAnalytics)
								{
									(await this.HttpClient.PostAsync(this.IsClient ? Analytics.ClientAnalyticsUrl : Analytics.ServerAnalyticsUrl, content)).EnsureSuccessStatusCode();
								}
								break;
							}
						}
						catch (Exception ex)
						{
							if (!(ex is HttpRequestException))
							{
								UnityEngine.Debug.LogException(ex);
							}
						}
						if (ticket != null)
						{
							try
							{
								ticket.Cancel();
							}
							catch (Exception ex2)
							{
								UnityEngine.Debug.LogError("Failed to cancel auth ticket in analytics: " + ex2.ToString());
							}
						}
					}
					ticket = null;
				}
				catch (Exception ex3)
				{
					if (this.IsClient)
					{
						UnityEngine.Debug.LogWarning(ex3.ToString());
					}
					else
					{
						UnityEngine.Debug.LogException(ex3);
					}
				}
				finally
				{
					List<EventRecord>.Enumerator enumerator = records.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							EventRecord eventRecord = enumerator.Current;
							Pool.Free<EventRecord>(ref eventRecord);
						}
					}
					finally
					{
						int num;
						if (num < 0)
						{
							((IDisposable)enumerator).Dispose();
						}
					}
					Pool.FreeList<EventRecord>(ref records);
					Pool.FreeMemoryStream(ref stream);
				}
			}

			// Token: 0x040050B6 RID: 20662
			public static readonly Analytics.AzureWebInterface client = new Analytics.AzureWebInterface(true);

			// Token: 0x040050B7 RID: 20663
			public static readonly Analytics.AzureWebInterface server = new Analytics.AzureWebInterface(false);

			// Token: 0x040050B8 RID: 20664
			public bool IsClient;

			// Token: 0x040050B9 RID: 20665
			public int MaxRetries = 1;

			// Token: 0x040050BA RID: 20666
			public int FlushSize = 1000;

			// Token: 0x040050BB RID: 20667
			public TimeSpan FlushDelay = TimeSpan.FromSeconds(30.0);

			// Token: 0x040050BC RID: 20668
			private DateTime nextFlush;

			// Token: 0x040050BD RID: 20669
			private List<EventRecord> pending = new List<EventRecord>();

			// Token: 0x040050BE RID: 20670
			private HttpClient HttpClient = new HttpClient();

			// Token: 0x040050BF RID: 20671
			private static readonly MediaTypeHeaderValue JsonContentType = new MediaTypeHeaderValue("application/json")
			{
				CharSet = Encoding.UTF8.WebName
			};
		}

		// Token: 0x02000F99 RID: 3993
		public static class Server
		{
			// Token: 0x1700073E RID: 1854
			// (get) Token: 0x06005574 RID: 21876 RVA: 0x001BA144 File Offset: 0x001B8344
			private static bool WriteToFile
			{
				get
				{
					return ConVar.Server.statBackup;
				}
			}

			// Token: 0x1700073F RID: 1855
			// (get) Token: 0x06005575 RID: 21877 RVA: 0x001BA14B File Offset: 0x001B834B
			private static bool CanSendAnalytics
			{
				get
				{
					return ConVar.Server.official && ConVar.Server.stats && Analytics.Server.Enabled;
				}
			}

			// Token: 0x06005576 RID: 21878 RVA: 0x001BA164 File Offset: 0x001B8364
			internal static void Death(BaseEntity initiator, BaseEntity weaponPrefab, Vector3 worldPosition)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (initiator != null)
				{
					if (initiator is BasePlayer)
					{
						if (weaponPrefab != null)
						{
							Analytics.Server.Death(weaponPrefab.ShortPrefabName, worldPosition, initiator.IsNpc ? Analytics.Server.DeathType.NPC : Analytics.Server.DeathType.Player);
							return;
						}
						Analytics.Server.Death("player", worldPosition, Analytics.Server.DeathType.Player);
						return;
					}
					else if (initiator is AutoTurret)
					{
						if (weaponPrefab != null)
						{
							Analytics.Server.Death(weaponPrefab.ShortPrefabName, worldPosition, Analytics.Server.DeathType.AutoTurret);
							return;
						}
					}
					else
					{
						Analytics.Server.Death(initiator.Categorize(), worldPosition, initiator.IsNpc ? Analytics.Server.DeathType.NPC : Analytics.Server.DeathType.Player);
					}
				}
			}

			// Token: 0x06005577 RID: 21879 RVA: 0x001BA1F0 File Offset: 0x001B83F0
			internal static void Death(string v, Vector3 worldPosition, Analytics.Server.DeathType deathType = Analytics.Server.DeathType.Player)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				string monumentStringFromPosition = Analytics.Server.GetMonumentStringFromPosition(worldPosition);
				if (!string.IsNullOrEmpty(monumentStringFromPosition))
				{
					switch (deathType)
					{
					case Analytics.Server.DeathType.Player:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:" + v, false);
						return;
					case Analytics.Server.DeathType.NPC:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:npc:" + v, false);
						return;
					case Analytics.Server.DeathType.AutoTurret:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:autoturret:" + v, false);
						return;
					default:
						return;
					}
				}
				else
				{
					switch (deathType)
					{
					case Analytics.Server.DeathType.Player:
						Analytics.Server.DesignEvent("player:death:" + v, false);
						return;
					case Analytics.Server.DeathType.NPC:
						Analytics.Server.DesignEvent("player:death:npc:" + v, false);
						return;
					case Analytics.Server.DeathType.AutoTurret:
						Analytics.Server.DesignEvent("player:death:autoturret:" + v, false);
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x06005578 RID: 21880 RVA: 0x001BA2B8 File Offset: 0x001B84B8
			private static string GetMonumentStringFromPosition(Vector3 worldPosition)
			{
				MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(worldPosition);
				if (monumentInfo != null && !string.IsNullOrEmpty(monumentInfo.displayPhrase.token))
				{
					return monumentInfo.displayPhrase.token;
				}
				if (SingletonComponent<EnvironmentManager>.Instance != null && (EnvironmentManager.Get(worldPosition) & EnvironmentType.TrainTunnels) == EnvironmentType.TrainTunnels)
				{
					return "train_tunnel_display_name";
				}
				return string.Empty;
			}

			// Token: 0x06005579 RID: 21881 RVA: 0x001BA31D File Offset: 0x001B851D
			public static void Crafting(string targetItemShortname, int skinId)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("player:craft:" + targetItemShortname, false);
				Analytics.Server.SkinUsed(targetItemShortname, skinId);
			}

			// Token: 0x0600557A RID: 21882 RVA: 0x001BA33F File Offset: 0x001B853F
			public static void SkinUsed(string itemShortName, int skinId)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (skinId == 0)
				{
					return;
				}
				Analytics.Server.DesignEvent(string.Format("skinUsed:{0}:{1}", itemShortName, skinId), false);
			}

			// Token: 0x0600557B RID: 21883 RVA: 0x001BA364 File Offset: 0x001B8564
			public static void ExcavatorStarted()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("monuments:excavatorstarted", false);
			}

			// Token: 0x0600557C RID: 21884 RVA: 0x001BA379 File Offset: 0x001B8579
			public static void ExcavatorStopped(float activeDuration)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("monuments:excavatorstopped", activeDuration, false);
			}

			// Token: 0x0600557D RID: 21885 RVA: 0x001BA38F File Offset: 0x001B858F
			public static void SlotMachineTransaction(int scrapSpent, int scrapReceived)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("slots:scrapSpent", scrapSpent, false);
				Analytics.Server.DesignEvent("slots:scrapReceived", scrapReceived, false);
			}

			// Token: 0x0600557E RID: 21886 RVA: 0x001BA3B1 File Offset: 0x001B85B1
			public static void VehiclePurchased(string vehicleType)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("vehiclePurchased:" + vehicleType, false);
			}

			// Token: 0x0600557F RID: 21887 RVA: 0x001BA3CC File Offset: 0x001B85CC
			public static void FishCaught(ItemDefinition fish)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (fish == null)
				{
					return;
				}
				Analytics.Server.DesignEvent("fishCaught:" + fish.shortname, false);
			}

			// Token: 0x06005580 RID: 21888 RVA: 0x001BA3F8 File Offset: 0x001B85F8
			public static void VendingMachineTransaction(NPCVendingOrder npcVendingOrder, ItemDefinition purchased, int amount)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (purchased == null)
				{
					return;
				}
				if (npcVendingOrder == null)
				{
					Analytics.Server.DesignEvent("vendingPurchase:player:" + purchased.shortname, amount, false);
					return;
				}
				Analytics.Server.DesignEvent("vendingPurchase:static:" + purchased.shortname, amount, false);
			}

			// Token: 0x06005581 RID: 21889 RVA: 0x001BA44F File Offset: 0x001B864F
			public static void Consume(string consumedItem)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (string.IsNullOrEmpty(consumedItem))
				{
					return;
				}
				Analytics.Server.DesignEvent("player:consume:" + consumedItem, false);
			}

			// Token: 0x06005582 RID: 21890 RVA: 0x001BA473 File Offset: 0x001B8673
			public static void TreeKilled(BaseEntity withWeapon)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (withWeapon != null)
				{
					Analytics.Server.DesignEvent("treekilled:" + withWeapon.ShortPrefabName, false);
					return;
				}
				Analytics.Server.DesignEvent("treekilled", false);
			}

			// Token: 0x06005583 RID: 21891 RVA: 0x001BA4A8 File Offset: 0x001B86A8
			public static void OreKilled(OreResourceEntity entity, HitInfo info)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				ResourceDispenser resourceDispenser;
				if (entity.TryGetComponent<ResourceDispenser>(out resourceDispenser) && resourceDispenser.containedItems.Count > 0 && resourceDispenser.containedItems[0].itemDef != null)
				{
					if (info.WeaponPrefab != null)
					{
						Analytics.Server.DesignEvent("orekilled:" + resourceDispenser.containedItems[0].itemDef.shortname + ":" + info.WeaponPrefab.ShortPrefabName, false);
						return;
					}
					Analytics.Server.DesignEvent(string.Format("orekilled:{0}", resourceDispenser.containedItems[0]), false);
				}
			}

			// Token: 0x06005584 RID: 21892 RVA: 0x001BA552 File Offset: 0x001B8752
			public static void MissionComplete(BaseMission mission)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("missionComplete:" + mission.shortname, true);
			}

			// Token: 0x06005585 RID: 21893 RVA: 0x001BA572 File Offset: 0x001B8772
			public static void MissionFailed(BaseMission mission, BaseMission.MissionFailReason reason)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent(string.Format("missionFailed:{0}:{1}", mission.shortname, reason), true);
			}

			// Token: 0x06005586 RID: 21894 RVA: 0x001BA598 File Offset: 0x001B8798
			public static void FreeUnderwaterCrate()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("loot:freeUnderWaterCrate", false);
			}

			// Token: 0x06005587 RID: 21895 RVA: 0x001BA5AD File Offset: 0x001B87AD
			public static void HeldItemDeployed(ItemDefinition def)
			{
				if (!Analytics.Server.CanSendAnalytics || Analytics.Server.lastHeldItemEvent < 0.1f)
				{
					return;
				}
				Analytics.Server.lastHeldItemEvent = 0f;
				Analytics.Server.DesignEvent("heldItemDeployed:" + def.shortname, false);
			}

			// Token: 0x06005588 RID: 21896 RVA: 0x001BA5ED File Offset: 0x001B87ED
			public static void UsedZipline()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("usedZipline", false);
			}

			// Token: 0x06005589 RID: 21897 RVA: 0x001BA602 File Offset: 0x001B8802
			public static void ReportCandiesCollectedByPlayer(int count)
			{
				if (!Analytics.Server.Enabled)
				{
					return;
				}
				Analytics.Server.DesignEvent("halloween:candiesCollected", count, false);
			}

			// Token: 0x0600558A RID: 21898 RVA: 0x001BA618 File Offset: 0x001B8818
			public static void ReportPlayersParticipatedInHalloweenEvent(int count)
			{
				if (!Analytics.Server.Enabled)
				{
					return;
				}
				Analytics.Server.DesignEvent("halloween:playersParticipated", count, false);
			}

			// Token: 0x0600558B RID: 21899 RVA: 0x001BA62E File Offset: 0x001B882E
			public static void Trigger(string message)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				Analytics.Server.DesignEvent(message, false);
			}

			// Token: 0x0600558C RID: 21900 RVA: 0x001BA647 File Offset: 0x001B8847
			private static void DesignEvent(string message, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, 1f);
				}
			}

			// Token: 0x0600558D RID: 21901 RVA: 0x001BA66D File Offset: 0x001B886D
			private static void DesignEvent(string message, float value, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message, value);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, value);
				}
			}

			// Token: 0x0600558E RID: 21902 RVA: 0x001BA690 File Offset: 0x001B8890
			private static void DesignEvent(string message, int value, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message, (float)value);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, (float)value);
				}
			}

			// Token: 0x0600558F RID: 21903 RVA: 0x001BA6B8 File Offset: 0x001B88B8
			private static string GetBackupPath(DateTime date)
			{
				return string.Format("{0}/{1}_{2}_{3}_analytics_backup.txt", new object[]
				{
					ConVar.Server.GetServerFolder("analytics"),
					date.Day,
					date.Month,
					date.Year
				});
			}

			// Token: 0x17000740 RID: 1856
			// (get) Token: 0x06005590 RID: 21904 RVA: 0x001BA70F File Offset: 0x001B890F
			private static DateTime currentDate
			{
				get
				{
					return DateTime.Now;
				}
			}

			// Token: 0x06005591 RID: 21905 RVA: 0x001BA718 File Offset: 0x001B8918
			private static void LocalBackup(string message, float value)
			{
				if (!Analytics.Server.WriteToFile)
				{
					return;
				}
				if (Analytics.Server.bufferData != null && Analytics.Server.backupDate.Date != Analytics.Server.currentDate.Date)
				{
					Analytics.Server.<LocalBackup>g__SaveBufferIntoDateFile|38_1(Analytics.Server.backupDate);
					Analytics.Server.bufferData.Clear();
					Analytics.Server.backupDate = Analytics.Server.currentDate;
				}
				if (Analytics.Server.bufferData == null)
				{
					if (Analytics.Server.bufferData == null)
					{
						Analytics.Server.bufferData = new Dictionary<string, float>();
					}
					Analytics.Server.lastAnalyticsSave = 0f;
					Analytics.Server.backupDate = Analytics.Server.currentDate;
				}
				if (Analytics.Server.bufferData.ContainsKey(message))
				{
					Dictionary<string, float> dictionary = Analytics.Server.bufferData;
					dictionary[message] += value;
				}
				else
				{
					Analytics.Server.bufferData.Add(message, value);
				}
				if (Analytics.Server.lastAnalyticsSave > 120f)
				{
					Analytics.Server.lastAnalyticsSave = 0f;
					Analytics.Server.<LocalBackup>g__SaveBufferIntoDateFile|38_1(Analytics.Server.currentDate);
					Analytics.Server.bufferData.Clear();
				}
			}

			// Token: 0x06005593 RID: 21907 RVA: 0x001BA808 File Offset: 0x001B8A08
			[CompilerGenerated]
			internal static void <LocalBackup>g__MergeBuffers|38_0(Dictionary<string, float> target, Dictionary<string, float> destination)
			{
				foreach (KeyValuePair<string, float> keyValuePair in target)
				{
					if (destination.ContainsKey(keyValuePair.Key))
					{
						string key = keyValuePair.Key;
						destination[key] += keyValuePair.Value;
					}
					else
					{
						destination.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}

			// Token: 0x06005594 RID: 21908 RVA: 0x001BA894 File Offset: 0x001B8A94
			[CompilerGenerated]
			internal static void <LocalBackup>g__SaveBufferIntoDateFile|38_1(DateTime date)
			{
				string backupPath = Analytics.Server.GetBackupPath(date);
				if (File.Exists(backupPath))
				{
					Dictionary<string, float> dictionary = (Dictionary<string, float>)JsonConvert.DeserializeObject(File.ReadAllText(backupPath), typeof(Dictionary<string, float>));
					if (dictionary != null)
					{
						Analytics.Server.<LocalBackup>g__MergeBuffers|38_0(dictionary, Analytics.Server.bufferData);
					}
				}
				string text = JsonConvert.SerializeObject(Analytics.Server.bufferData);
				File.WriteAllText(Analytics.Server.GetBackupPath(date), text);
			}

			// Token: 0x040050C0 RID: 20672
			public static bool Enabled;

			// Token: 0x040050C1 RID: 20673
			private static Dictionary<string, float> bufferData;

			// Token: 0x040050C2 RID: 20674
			private static TimeSince lastHeldItemEvent;

			// Token: 0x040050C3 RID: 20675
			private static TimeSince lastAnalyticsSave;

			// Token: 0x040050C4 RID: 20676
			private static DateTime backupDate;

			// Token: 0x02001004 RID: 4100
			public enum DeathType
			{
				// Token: 0x04005260 RID: 21088
				Player,
				// Token: 0x04005261 RID: 21089
				NPC,
				// Token: 0x04005262 RID: 21090
				AutoTurret
			}
		}
	}
}
