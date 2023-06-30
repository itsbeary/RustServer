using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Facepunch;
using Facepunch.Unity;
using Rust;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ABA RID: 2746
	[ConsoleSystem.Factory("debug")]
	public class Debugging : ConsoleSystem
	{
		// Token: 0x060041A0 RID: 16800 RVA: 0x001849D7 File Offset: 0x00182BD7
		[ServerVar]
		[ClientVar]
		public static void renderinfo(ConsoleSystem.Arg arg)
		{
			RenderInfo.GenerateReport();
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x060041A2 RID: 16802 RVA: 0x001849EB File Offset: 0x00182BEB
		// (set) Token: 0x060041A1 RID: 16801 RVA: 0x001849DE File Offset: 0x00182BDE
		[ServerVar]
		[ClientVar]
		public static bool log
		{
			get
			{
				return Debug.unityLogger.logEnabled;
			}
			set
			{
				Debug.unityLogger.logEnabled = value;
			}
		}

		// Token: 0x060041A3 RID: 16803 RVA: 0x001849F8 File Offset: 0x00182BF8
		[ServerVar]
		public static void enable_player_movement(ConsoleSystem.Arg arg)
		{
			if (!arg.IsAdmin)
			{
				return;
			}
			bool @bool = arg.GetBool(0, true);
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				arg.ReplyWith("Must be called from client with player model");
				return;
			}
			basePlayer.ClientRPCPlayer<bool>(null, basePlayer, "TogglePlayerMovement", @bool);
			arg.ReplyWith((@bool ? "enabled" : "disabled") + " player movement");
		}

		// Token: 0x060041A4 RID: 16804 RVA: 0x00184A60 File Offset: 0x00182C60
		[ClientVar]
		[ServerVar]
		public static void stall(ConsoleSystem.Arg arg)
		{
			float num = Mathf.Clamp(arg.GetFloat(0, 0f), 0f, 1f);
			arg.ReplyWith("Stalling for " + num + " seconds...");
			Thread.Sleep(Mathf.RoundToInt(num * 1000f));
		}

		// Token: 0x060041A5 RID: 16805 RVA: 0x00184AB8 File Offset: 0x00182CB8
		[ServerVar(Help = "Repair all items in inventory")]
		public static void repair_inventory(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			foreach (Item item in basePlayer.inventory.AllItems())
			{
				if (item != null)
				{
					item.maxCondition = item.info.condition.max;
					item.condition = item.maxCondition;
					item.MarkDirty();
				}
				if (item.contents != null)
				{
					foreach (Item item2 in item.contents.itemList)
					{
						item2.maxCondition = item2.info.condition.max;
						item2.condition = item2.maxCondition;
						item2.MarkDirty();
					}
				}
			}
		}

		// Token: 0x060041A6 RID: 16806 RVA: 0x00184B98 File Offset: 0x00182D98
		[ServerVar(Help = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again")]
		public static void flushgroup(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			basePlayer.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
			basePlayer.UpdateNetworkGroup();
		}

		// Token: 0x060041A7 RID: 16807 RVA: 0x00184BD0 File Offset: 0x00182DD0
		[ServerVar(Help = "Break the current held object")]
		public static void breakheld(ConsoleSystem.Arg arg)
		{
			Item activeItem = arg.Player().GetActiveItem();
			if (activeItem == null)
			{
				return;
			}
			activeItem.LoseCondition(activeItem.condition * 2f);
		}

		// Token: 0x060041A8 RID: 16808 RVA: 0x00184C00 File Offset: 0x00182E00
		[ServerVar(Help = "reset all puzzles")]
		public static void puzzlereset(ConsoleSystem.Arg arg)
		{
			if (arg.Player() == null)
			{
				return;
			}
			PuzzleReset[] array = UnityEngine.Object.FindObjectsOfType<PuzzleReset>();
			Debug.Log("iterating...");
			foreach (PuzzleReset puzzleReset in array)
			{
				Debug.Log("resetting puzzle at :" + puzzleReset.transform.position);
				puzzleReset.DoReset();
				puzzleReset.ResetTimer();
			}
		}

		// Token: 0x060041A9 RID: 16809 RVA: 0x00184C6C File Offset: 0x00182E6C
		[ServerVar(EditorOnly = true, Help = "respawn all puzzles from their prefabs")]
		public static void puzzleprefabrespawn(ConsoleSystem.Arg arg)
		{
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities.Where((BaseNetworkable x) => x is IOEntity && PrefabAttribute.server.Find<Construction>(x.prefabID) == null).ToList<BaseNetworkable>())
			{
				baseNetworkable.Kill(BaseNetworkable.DestroyMode.None);
			}
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				GameObject gameObject = GameManager.server.FindPrefab(monumentInfo.gameObject.name);
				if (!(gameObject == null))
				{
					Dictionary<IOEntity, IOEntity> dictionary = new Dictionary<IOEntity, IOEntity>();
					IOEntity[] componentsInChildren = gameObject.GetComponentsInChildren<IOEntity>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						IOEntity ioentity = componentsInChildren[i];
						Quaternion quaternion = monumentInfo.transform.rotation * ioentity.transform.rotation;
						Vector3 vector = monumentInfo.transform.TransformPoint(ioentity.transform.position);
						BaseEntity newEntity = GameManager.server.CreateEntity(ioentity.PrefabName, vector, quaternion, true);
						IOEntity ioentity2 = newEntity as IOEntity;
						if (ioentity2 != null)
						{
							dictionary.Add(ioentity, ioentity2);
							DoorManipulator doorManipulator = newEntity as DoorManipulator;
							if (doorManipulator != null)
							{
								List<Door> list = Pool.GetList<Door>();
								Vis.Entities<Door>(newEntity.transform.position, 10f, list, -1, QueryTriggerInteraction.Collide);
								Door door = list.OrderBy((Door x) => x.Distance(newEntity.transform.position)).FirstOrDefault<Door>();
								if (door != null)
								{
									doorManipulator.targetDoor = door;
								}
								Pool.FreeList<Door>(ref list);
							}
							CardReader cardReader = newEntity as CardReader;
							if (cardReader != null)
							{
								CardReader cardReader2 = ioentity as CardReader;
								if (cardReader2 != null)
								{
									cardReader.accessLevel = cardReader2.accessLevel;
									cardReader.accessDuration = cardReader2.accessDuration;
								}
							}
							TimerSwitch timerSwitch = newEntity as TimerSwitch;
							if (timerSwitch != null)
							{
								TimerSwitch timerSwitch2 = ioentity as TimerSwitch;
								if (timerSwitch2 != null)
								{
									timerSwitch.timerLength = timerSwitch2.timerLength;
								}
							}
						}
					}
					foreach (KeyValuePair<IOEntity, IOEntity> keyValuePair in dictionary)
					{
						IOEntity key = keyValuePair.Key;
						IOEntity value = keyValuePair.Value;
						for (int j = 0; j < key.outputs.Length; j++)
						{
							if (!(key.outputs[j].connectedTo.ioEnt == null))
							{
								value.outputs[j].connectedTo.ioEnt = dictionary[key.outputs[j].connectedTo.ioEnt];
								value.outputs[j].connectedToSlot = key.outputs[j].connectedToSlot;
							}
						}
					}
					foreach (IOEntity ioentity3 in dictionary.Values)
					{
						ioentity3.Spawn();
					}
				}
			}
		}

		// Token: 0x060041AA RID: 16810 RVA: 0x00185034 File Offset: 0x00183234
		[ServerVar(Help = "Break all the items in your inventory whose name match the passed string")]
		public static void breakitem(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			foreach (Item item in arg.Player().inventory.containerMain.itemList)
			{
				if (item.info.shortname.Contains(@string, CompareOptions.IgnoreCase) && item.hasCondition)
				{
					item.LoseCondition(item.condition * 2f);
				}
			}
		}

		// Token: 0x060041AB RID: 16811 RVA: 0x001850CC File Offset: 0x001832CC
		[ServerVar]
		public static void refillvitals(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHealth(arg.Player(), 1000f, null);
			Debugging.AdjustCalories(arg.Player(), 1000f, 1f);
			Debugging.AdjustHydration(arg.Player(), 1000f, 1f);
		}

		// Token: 0x060041AC RID: 16812 RVA: 0x00185109 File Offset: 0x00183309
		[ServerVar]
		public static void heal(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHealth(arg.Player(), (float)arg.GetInt(0, 1), null);
		}

		// Token: 0x060041AD RID: 16813 RVA: 0x00185120 File Offset: 0x00183320
		[ServerVar]
		public static void hurt(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHealth(arg.Player(), (float)(-(float)arg.GetInt(0, 1)), arg.GetString(1, string.Empty));
		}

		// Token: 0x060041AE RID: 16814 RVA: 0x00185143 File Offset: 0x00183343
		[ServerVar]
		public static void eat(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustCalories(arg.Player(), (float)arg.GetInt(0, 1), (float)arg.GetInt(1, 1));
		}

		// Token: 0x060041AF RID: 16815 RVA: 0x00185162 File Offset: 0x00183362
		[ServerVar]
		public static void drink(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHydration(arg.Player(), (float)arg.GetInt(0, 1), (float)arg.GetInt(1, 1));
		}

		// Token: 0x060041B0 RID: 16816 RVA: 0x00185184 File Offset: 0x00183384
		private static void AdjustHealth(BasePlayer player, float amount, string bone = null)
		{
			HitInfo hitInfo = new HitInfo(player, player, DamageType.Bullet, -amount);
			if (!string.IsNullOrEmpty(bone))
			{
				hitInfo.HitBone = StringPool.Get(bone);
			}
			player.OnAttacked(hitInfo);
		}

		// Token: 0x060041B1 RID: 16817 RVA: 0x001851B8 File Offset: 0x001833B8
		private static void AdjustCalories(BasePlayer player, float amount, float time = 1f)
		{
			player.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, amount, time);
		}

		// Token: 0x060041B2 RID: 16818 RVA: 0x001851C8 File Offset: 0x001833C8
		private static void AdjustHydration(BasePlayer player, float amount, float time = 1f)
		{
			player.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, amount, time);
		}

		// Token: 0x060041B3 RID: 16819 RVA: 0x001851D8 File Offset: 0x001833D8
		[ServerVar]
		public static void ResetSleepingBagTimers(ConsoleSystem.Arg arg)
		{
			SleepingBag.ResetTimersForPlayer(arg.Player());
		}

		// Token: 0x060041B4 RID: 16820 RVA: 0x001851E8 File Offset: 0x001833E8
		[ServerVar(Help = "Spawn lots of IO entities to lag the server")]
		public static void bench_io(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			int @int = arg.GetInt(0, 50);
			string name = arg.GetString(1, "water_catcher_small");
			List<IOEntity> list = new List<IOEntity>();
			WaterCatcher waterCatcher = null;
			Vector3 position = arg.Player().transform.position;
			string[] array = (from x in GameManifest.Current.entities
				where Path.GetFileNameWithoutExtension(x).Contains(name, CompareOptions.IgnoreCase)
				select x.ToLower()).ToArray<string>();
			if (array.Length == 0)
			{
				arg.ReplyWith("Couldn't find io prefab \"" + array[0] + "\"");
				return;
			}
			if (array.Length > 1)
			{
				string text = array.FirstOrDefault((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
				if (text == null)
				{
					Debug.Log(string.Format("{0} failed to find io entity \"{1}\"", arg, name));
					arg.ReplyWith("Unknown entity - could be:\n\n" + string.Join("\n", array.Select(new Func<string, string>(Path.GetFileNameWithoutExtension)).ToArray<string>()));
					return;
				}
				array[0] = text;
			}
			for (int i = 0; i < @int; i++)
			{
				Vector3 vector = position + new Vector3((float)(i * 5), 0f, 0f);
				Quaternion identity = Quaternion.identity;
				BaseEntity baseEntity = GameManager.server.CreateEntity(array[0], vector, identity, true);
				if (baseEntity)
				{
					baseEntity.Spawn();
					WaterCatcher component = baseEntity.GetComponent<WaterCatcher>();
					if (component)
					{
						list.Add(component);
						if (waterCatcher != null)
						{
							Debugging.<bench_io>g__Connect|25_0(waterCatcher, component);
						}
						if (i == @int - 1)
						{
							Debugging.<bench_io>g__Connect|25_0(component, list.First<IOEntity>());
						}
						waterCatcher = component;
					}
				}
			}
		}

		// Token: 0x060041B7 RID: 16823 RVA: 0x001853E4 File Offset: 0x001835E4
		[CompilerGenerated]
		internal static void <bench_io>g__Connect|25_0(IOEntity InputIOEnt, IOEntity OutputIOEnt)
		{
			int num = 0;
			int num2 = 0;
			WireTool.WireColour wireColour = WireTool.WireColour.Default;
			IOEntity.IOSlot ioslot = InputIOEnt.inputs[num];
			IOEntity.IOSlot ioslot2 = OutputIOEnt.outputs[num2];
			ioslot.connectedTo.Set(OutputIOEnt);
			ioslot.connectedToSlot = num2;
			ioslot.wireColour = wireColour;
			ioslot.connectedTo.Init();
			ioslot2.connectedTo.Set(InputIOEnt);
			ioslot2.connectedToSlot = num;
			ioslot2.wireColour = wireColour;
			ioslot2.connectedTo.Init();
			ioslot2.linePoints = new Vector3[]
			{
				Vector3.zero,
				OutputIOEnt.transform.InverseTransformPoint(InputIOEnt.transform.TransformPoint(ioslot.handlePosition))
			};
			OutputIOEnt.MarkDirtyForceUpdateOutputs();
			OutputIOEnt.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			InputIOEnt.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			OutputIOEnt.SendChangedToRoot(true);
		}

		// Token: 0x04003B7F RID: 15231
		[ServerVar]
		[ClientVar]
		public static bool checktriggers = false;

		// Token: 0x04003B80 RID: 15232
		[ServerVar]
		public static bool checkparentingtriggers = true;

		// Token: 0x04003B81 RID: 15233
		[ServerVar]
		[ClientVar(Saved = false, Help = "Shows some debug info for dismount attempts.")]
		public static bool DebugDismounts = false;

		// Token: 0x04003B82 RID: 15234
		[ServerVar(Help = "Do not damage any items")]
		public static bool disablecondition = false;

		// Token: 0x04003B83 RID: 15235
		[ClientVar]
		[ServerVar]
		public static bool callbacks = false;
	}
}
