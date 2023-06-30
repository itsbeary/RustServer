using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Facepunch;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AC0 RID: 2752
	[ConsoleSystem.Factory("entity")]
	public class Entity : ConsoleSystem
	{
		// Token: 0x060041C3 RID: 16835 RVA: 0x001856B8 File Offset: 0x001838B8
		private static TextTable GetEntityTable(Func<Entity.EntityInfo, bool> filter)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("entity");
			textTable.AddColumn("group");
			textTable.AddColumn("parent");
			textTable.AddColumn("name");
			textTable.AddColumn("position");
			textTable.AddColumn("local");
			textTable.AddColumn("rotation");
			textTable.AddColumn("local");
			textTable.AddColumn("status");
			textTable.AddColumn("invokes");
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities)
			{
				if (!(baseNetworkable == null))
				{
					Entity.EntityInfo entityInfo = new Entity.EntityInfo(baseNetworkable);
					if (filter(entityInfo))
					{
						textTable.AddRow(new string[]
						{
							"sv",
							entityInfo.entityID.Value.ToString(),
							entityInfo.groupID.ToString(),
							entityInfo.parentID.Value.ToString(),
							entityInfo.entity.ShortPrefabName,
							entityInfo.entity.transform.position.ToString(),
							entityInfo.entity.transform.localPosition.ToString(),
							entityInfo.entity.transform.rotation.eulerAngles.ToString(),
							entityInfo.entity.transform.localRotation.eulerAngles.ToString(),
							entityInfo.status,
							entityInfo.entity.InvokeString()
						});
					}
				}
			}
			return textTable;
		}

		// Token: 0x060041C4 RID: 16836 RVA: 0x001858BC File Offset: 0x00183ABC
		[ServerVar]
		[ClientVar]
		public static void find_entity(ConsoleSystem.Arg args)
		{
			string filter = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => string.IsNullOrEmpty(filter) || info.entity.PrefabName.Contains(filter));
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041C5 RID: 16837 RVA: 0x00185900 File Offset: 0x00183B00
		[ServerVar]
		[ClientVar]
		public static void find_id(ConsoleSystem.Arg args)
		{
			NetworkableId filter = args.GetEntityID(0, default(NetworkableId));
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041C6 RID: 16838 RVA: 0x00185948 File Offset: 0x00183B48
		[ServerVar]
		[ClientVar]
		public static void find_group(ConsoleSystem.Arg args)
		{
			uint filter = args.GetUInt(0, 0U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.groupID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041C7 RID: 16839 RVA: 0x00185988 File Offset: 0x00183B88
		[ServerVar]
		[ClientVar]
		public static void find_parent(ConsoleSystem.Arg args)
		{
			NetworkableId filter = args.GetEntityID(0, default(NetworkableId));
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.parentID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041C8 RID: 16840 RVA: 0x001859D0 File Offset: 0x00183BD0
		[ServerVar]
		[ClientVar]
		public static void find_status(ConsoleSystem.Arg args)
		{
			string filter = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => string.IsNullOrEmpty(filter) || info.status.Contains(filter));
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041C9 RID: 16841 RVA: 0x00185A14 File Offset: 0x00183C14
		[ServerVar]
		[ClientVar]
		public static void find_radius(ConsoleSystem.Arg args)
		{
			BasePlayer player = args.Player();
			if (player == null)
			{
				return;
			}
			uint filter = args.GetUInt(0, 10U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => Vector3.Distance(info.entity.transform.position, player.transform.position) <= filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x00185A70 File Offset: 0x00183C70
		[ServerVar]
		[ClientVar]
		public static void find_self(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.net == null)
			{
				return;
			}
			NetworkableId filter = basePlayer.net.ID;
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x00185ACC File Offset: 0x00183CCC
		[ServerVar]
		public static void debug_toggle(ConsoleSystem.Arg args)
		{
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.SetFlag(BaseEntity.Flags.Debugging, !baseEntity.IsDebugging(), false, true);
			if (baseEntity.IsDebugging())
			{
				baseEntity.OnDebugStart();
			}
			args.ReplyWith(string.Concat(new object[]
			{
				"Debugging for ",
				baseEntity.net.ID,
				" ",
				baseEntity.IsDebugging() ? "enabled" : "disabled"
			}));
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x00185B7C File Offset: 0x00183D7C
		[ServerVar]
		public static void nudge(ConsoleSystem.Arg args)
		{
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.BroadcastMessage("DebugNudge", SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x060041CD RID: 16845 RVA: 0x00185BCC File Offset: 0x00183DCC
		private static Entity.EntitySpawnRequest GetSpawnEntityFromName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Entity.EntitySpawnRequest
				{
					Error = "No entity name provided"
				};
			}
			string[] array = (from x in GameManifest.Current.entities
				where Path.GetFileNameWithoutExtension(x).Contains(name, CompareOptions.IgnoreCase)
				select x.ToLower()).ToArray<string>();
			if (array.Length == 0)
			{
				return new Entity.EntitySpawnRequest
				{
					Error = "Entity type not found"
				};
			}
			if (array.Length > 1)
			{
				string text = array.FirstOrDefault((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
				if (text == null)
				{
					return new Entity.EntitySpawnRequest
					{
						Error = "Unknown entity - could be:\n\n" + string.Join("\n", array.Select(new Func<string, string>(Path.GetFileNameWithoutExtension)).ToArray<string>())
					};
				}
				array[0] = text;
			}
			return new Entity.EntitySpawnRequest
			{
				PrefabName = array[0]
			};
		}

		// Token: 0x060041CE RID: 16846 RVA: 0x00185CDC File Offset: 0x00183EDC
		[ServerVar(Name = "spawn")]
		public static string svspawn(string name, Vector3 pos, Vector3 dir)
		{
			BasePlayer basePlayer = ConsoleSystem.CurrentArgs.Player();
			Entity.EntitySpawnRequest spawnEntityFromName = Entity.GetSpawnEntityFromName(name);
			if (!spawnEntityFromName.Valid)
			{
				return spawnEntityFromName.Error;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(spawnEntityFromName.PrefabName, pos, Quaternion.LookRotation(dir, Vector3.up), true);
			if (baseEntity == null)
			{
				Debug.Log(string.Format("{0} failed to spawn \"{1}\" (tried to spawn \"{2}\")", basePlayer, spawnEntityFromName.PrefabName, name));
				return "Couldn't spawn " + name;
			}
			BasePlayer basePlayer2 = baseEntity as BasePlayer;
			if (basePlayer2 != null)
			{
				basePlayer2.OverrideViewAngles(Quaternion.LookRotation(dir, Vector3.up).eulerAngles);
			}
			baseEntity.Spawn();
			Debug.Log(string.Format("{0} spawned \"{1}\" at {2}", basePlayer, baseEntity, pos));
			return string.Concat(new object[] { "spawned ", baseEntity, " at ", pos });
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x00185DC4 File Offset: 0x00183FC4
		[ServerVar(Name = "spawnitem")]
		public static string svspawnitem(string name, Vector3 pos)
		{
			BasePlayer basePlayer = ConsoleSystem.CurrentArgs.Player();
			if (string.IsNullOrEmpty(name))
			{
				return "No entity name provided";
			}
			string[] array = (from x in ItemManager.itemList
				select x.shortname into x
				where x.Contains(name, CompareOptions.IgnoreCase)
				select x).ToArray<string>();
			if (array.Length == 0)
			{
				return "Entity type not found";
			}
			if (array.Length > 1)
			{
				string text = array.FirstOrDefault((string x) => string.Compare(x, name, StringComparison.OrdinalIgnoreCase) == 0);
				if (text == null)
				{
					Debug.Log(string.Format("{0} failed to spawn \"{1}\"", basePlayer, name));
					return "Unknown entity - could be:\n\n" + string.Join("\n", array);
				}
				array[0] = text;
			}
			Item item = ItemManager.CreateByName(array[0], 1, 0UL);
			if (item == null)
			{
				Debug.Log(string.Format("{0} failed to spawn \"{1}\" (tried to spawnitem \"{2}\")", basePlayer, array[0], name));
				return "Couldn't spawn " + name;
			}
			BaseEntity baseEntity = item.CreateWorldObject(pos, default(Quaternion), null, 0U);
			Debug.Log(string.Format("{0} spawned \"{1}\" at {2} (via spawnitem)", basePlayer, baseEntity, pos));
			return string.Concat(new object[] { "spawned ", item, " at ", pos });
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x00185F28 File Offset: 0x00184128
		[ServerVar(Name = "spawngrid")]
		public static string svspawngrid(string name, int width = 5, int height = 5, int spacing = 5)
		{
			BasePlayer basePlayer = ConsoleSystem.CurrentArgs.Player();
			Entity.EntitySpawnRequest spawnEntityFromName = Entity.GetSpawnEntityFromName(name);
			if (!spawnEntityFromName.Valid)
			{
				return spawnEntityFromName.Error;
			}
			Quaternion rotation = basePlayer.transform.rotation;
			rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
			Matrix4x4 matrix4x = Matrix4x4.TRS(basePlayer.transform.position, basePlayer.transform.rotation, Vector3.one);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Vector3 vector = matrix4x.MultiplyPoint(new Vector3((float)(i * spacing), 0f, (float)(j * spacing)));
					BaseEntity baseEntity = GameManager.server.CreateEntity(spawnEntityFromName.PrefabName, vector, rotation, true);
					if (baseEntity == null)
					{
						Debug.Log(string.Format("{0} failed to spawn \"{1}\" (tried to spawn \"{2}\")", basePlayer, spawnEntityFromName.PrefabName, name));
						return "Couldn't spawn " + name;
					}
					baseEntity.Spawn();
				}
			}
			Debug.Log(string.Format("{0} spawned ({1}) ", basePlayer, width * height) + spawnEntityFromName.PrefabName);
			return string.Format("spawned ({0}) ", width * height) + spawnEntityFromName.PrefabName;
		}

		// Token: 0x060041D1 RID: 16849 RVA: 0x00186070 File Offset: 0x00184270
		[ServerVar]
		public static void spawnlootfrom(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			string @string = args.GetString(0, string.Empty);
			int @int = args.GetInt(1, 1);
			Vector3 vector = args.GetVector3(1, basePlayer ? basePlayer.CenterPoint() : Vector3.zero);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(@string, vector, default(Quaternion), true);
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.Spawn();
			basePlayer.ChatMessage(string.Concat(new object[] { "Contents of ", @string, " spawned ", @int, " times" }));
			LootContainer component = baseEntity.GetComponent<LootContainer>();
			if (component != null)
			{
				for (int i = 0; i < @int * component.maxDefinitionsToSpawn; i++)
				{
					component.lootDefinition.SpawnIntoContainer(basePlayer.inventory.containerMain);
				}
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x0018616C File Offset: 0x0018436C
		public static int DeleteBy(ulong id)
		{
			List<ulong> list = Pool.GetList<ulong>();
			list.Add(id);
			int num = Entity.DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			return num;
		}

		// Token: 0x060041D3 RID: 16851 RVA: 0x00186194 File Offset: 0x00184394
		[ServerVar(Help = "Destroy all entities created by provided users (separate users by space)")]
		public static int DeleteBy(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return 0;
			}
			List<ulong> list = Pool.GetList<ulong>();
			string[] args = arg.Args;
			for (int i = 0; i < args.Length; i++)
			{
				ulong num;
				if (ulong.TryParse(args[i], out num))
				{
					list.Add(num);
				}
			}
			int num2 = Entity.DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			return num2;
		}

		// Token: 0x060041D4 RID: 16852 RVA: 0x001861E8 File Offset: 0x001843E8
		private static int DeleteBy(List<ulong> ids)
		{
			int num = 0;
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities)
			{
				BaseEntity baseEntity = (BaseEntity)baseNetworkable;
				if (!(baseEntity == null))
				{
					bool flag = false;
					foreach (ulong num2 in ids)
					{
						if (baseEntity.OwnerID == num2)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						baseEntity.Invoke(new Action(baseEntity.KillMessage), (float)num * 0.2f);
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x060041D5 RID: 16853 RVA: 0x001862A8 File Offset: 0x001844A8
		[ServerVar(Help = "Destroy all entities created by users in the provided text block (can use with copied results from ent auth)")]
		public static void DeleteByTextBlock(ConsoleSystem.Arg arg)
		{
			if (arg.Args.Length != 1)
			{
				arg.ReplyWith("Invalid arguments, provide a text block surrounded by \" and listing player id's at the start of each line");
				return;
			}
			MatchCollection matchCollection = Regex.Matches(arg.GetString(0, ""), "^\\b\\d{17}", RegexOptions.Multiline);
			List<ulong> list = Pool.GetList<ulong>();
			using (IEnumerator enumerator = matchCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ulong num;
					if (ulong.TryParse(((Match)enumerator.Current).Value, out num))
					{
						list.Add(num);
					}
				}
			}
			int num2 = Entity.DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			arg.ReplyWith(string.Format("Destroyed {0} entities", num2));
		}

		// Token: 0x02000F65 RID: 3941
		private struct EntityInfo
		{
			// Token: 0x060054AF RID: 21679 RVA: 0x001B5E28 File Offset: 0x001B4028
			public EntityInfo(BaseNetworkable src)
			{
				this.entity = src;
				BaseEntity baseEntity = this.entity as BaseEntity;
				BaseEntity baseEntity2 = ((baseEntity != null) ? baseEntity.GetParentEntity() : null);
				this.entityID = ((this.entity != null && this.entity.net != null) ? this.entity.net.ID : default(NetworkableId));
				this.groupID = ((this.entity != null && this.entity.net != null && this.entity.net.group != null) ? this.entity.net.group.ID : 0U);
				this.parentID = ((baseEntity != null) ? baseEntity.parentEntity.uid : default(NetworkableId));
				if (!(baseEntity != null) || !baseEntity.parentEntity.uid.IsValid)
				{
					this.status = string.Empty;
					return;
				}
				if (baseEntity2 == null)
				{
					this.status = "orphan";
					return;
				}
				this.status = "child";
			}

			// Token: 0x0400500B RID: 20491
			public BaseNetworkable entity;

			// Token: 0x0400500C RID: 20492
			public NetworkableId entityID;

			// Token: 0x0400500D RID: 20493
			public uint groupID;

			// Token: 0x0400500E RID: 20494
			public NetworkableId parentID;

			// Token: 0x0400500F RID: 20495
			public string status;
		}

		// Token: 0x02000F66 RID: 3942
		private struct EntitySpawnRequest
		{
			// Token: 0x17000739 RID: 1849
			// (get) Token: 0x060054B0 RID: 21680 RVA: 0x001B5F51 File Offset: 0x001B4151
			public bool Valid
			{
				get
				{
					return string.IsNullOrEmpty(this.Error);
				}
			}

			// Token: 0x04005010 RID: 20496
			public string PrefabName;

			// Token: 0x04005011 RID: 20497
			public string Error;
		}
	}
}
