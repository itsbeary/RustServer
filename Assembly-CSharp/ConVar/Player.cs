using System;
using System.Collections.Generic;
using System.Text;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADC RID: 2780
	[ConsoleSystem.Factory("player")]
	public class Player : ConsoleSystem
	{
		// Token: 0x060042A5 RID: 17061 RVA: 0x00189DB0 File Offset: 0x00187FB0
		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_play(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return;
			}
			if (arg.IsServerside)
			{
				global::BasePlayer basePlayer = arg.Player();
				if (basePlayer == null)
				{
					return;
				}
				string text = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					text = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.FullString,
						" ",
						basePlayer.UserIDString
					});
				}
				else if (Server.cinematic)
				{
					text = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.GetString(0, ""),
						" ",
						basePlayer.UserIDString
					});
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(text, Array.Empty<object>());
					return;
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.Connection, text, Array.Empty<object>());
				}
			}
		}

		// Token: 0x060042A6 RID: 17062 RVA: 0x00189EB4 File Offset: 0x001880B4
		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_stop(ConsoleSystem.Arg arg)
		{
			if (arg.IsServerside)
			{
				global::BasePlayer basePlayer = arg.Player();
				if (basePlayer == null)
				{
					return;
				}
				string text = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					text = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.FullString,
						" ",
						basePlayer.UserIDString
					});
				}
				else if (Server.cinematic)
				{
					text = arg.cmd.FullName + " " + basePlayer.UserIDString;
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(text, Array.Empty<object>());
					return;
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.Connection, text, Array.Empty<object>());
				}
			}
		}

		// Token: 0x060042A7 RID: 17063 RVA: 0x00189F88 File Offset: 0x00188188
		[ServerUserVar]
		public static void cinematic_gesture(ConsoleSystem.Arg arg)
		{
			if (!Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			global::BasePlayer basePlayer = arg.GetPlayer(1);
			if (basePlayer == null)
			{
				basePlayer = arg.Player();
			}
			basePlayer.UpdateActiveItem(default(ItemId));
			basePlayer.SignalBroadcast(global::BaseEntity.Signal.Gesture, @string, null);
		}

		// Token: 0x060042A8 RID: 17064 RVA: 0x00189FDC File Offset: 0x001881DC
		[ServerUserVar]
		public static void copyrotation(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (basePlayer2 != null)
			{
				basePlayer2.CopyRotation(basePlayer);
				Debug.Log("Copied rotation of " + basePlayer2.UserIDString);
			}
		}

		// Token: 0x060042A9 RID: 17065 RVA: 0x0018A050 File Offset: 0x00188250
		[ServerUserVar]
		public static void abandonmission(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.HasActiveMission())
			{
				basePlayer.AbandonActiveMission();
			}
		}

		// Token: 0x060042AA RID: 17066 RVA: 0x0018A074 File Offset: 0x00188274
		[ServerUserVar]
		public static void mount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out raycastHit, 5f, 10496, QueryTriggerInteraction.Ignore))
			{
				global::BaseEntity entity = raycastHit.GetEntity();
				if (entity)
				{
					BaseMountable baseMountable = entity.GetComponent<BaseMountable>();
					if (!baseMountable)
					{
						global::BaseVehicle baseVehicle = entity.GetComponentInParent<global::BaseVehicle>();
						if (baseVehicle)
						{
							if (!baseVehicle.isServer)
							{
								baseVehicle = global::BaseNetworkable.serverEntities.Find(baseVehicle.net.ID) as global::BaseVehicle;
							}
							baseVehicle.AttemptMount(basePlayer2, true);
							return;
						}
					}
					if (baseMountable && !baseMountable.isServer)
					{
						baseMountable = global::BaseNetworkable.serverEntities.Find(baseMountable.net.ID) as BaseMountable;
					}
					if (baseMountable)
					{
						baseMountable.AttemptMount(basePlayer2, true);
					}
				}
			}
		}

		// Token: 0x060042AB RID: 17067 RVA: 0x0018A1A4 File Offset: 0x001883A4
		[ServerVar]
		public static void gotosleep(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindSleeping(@uint.ToString());
			if (!basePlayer2)
			{
				basePlayer2 = global::BasePlayer.FindBotClosestMatch(@uint.ToString());
				if (basePlayer2.IsSleeping())
				{
					basePlayer2 = null;
				}
			}
			if (!basePlayer2)
			{
				return;
			}
			basePlayer2.StartSleeping();
		}

		// Token: 0x060042AC RID: 17068 RVA: 0x0018A214 File Offset: 0x00188414
		[ServerVar]
		public static void dismount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			if (basePlayer2 && basePlayer2.isMounted)
			{
				basePlayer2.GetMounted().DismountPlayer(basePlayer2, false);
			}
		}

		// Token: 0x060042AD RID: 17069 RVA: 0x0018A288 File Offset: 0x00188488
		[ServerVar]
		public static void swapseat(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			int @int = arg.GetInt(1, 0);
			if (basePlayer2 && basePlayer2.isMounted && basePlayer2.GetMounted().VehicleParent())
			{
				basePlayer2.GetMounted().VehicleParent().SwapSeats(basePlayer2, @int);
			}
		}

		// Token: 0x060042AE RID: 17070 RVA: 0x0018A31C File Offset: 0x0018851C
		[ServerVar]
		public static void wakeup(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			global::BasePlayer basePlayer2 = global::BasePlayer.FindSleeping(arg.GetUInt(0, 0U).ToString());
			if (!basePlayer2)
			{
				return;
			}
			basePlayer2.EndSleeping();
		}

		// Token: 0x060042AF RID: 17071 RVA: 0x0018A370 File Offset: 0x00188570
		[ServerVar]
		public static void wakeupall(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer basePlayer2 in global::BasePlayer.sleepingPlayerList)
			{
				list.Add(basePlayer2);
			}
			foreach (global::BasePlayer basePlayer3 in list)
			{
				basePlayer3.EndSleeping();
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}

		// Token: 0x060042B0 RID: 17072 RVA: 0x0018A42C File Offset: 0x0018862C
		[ServerVar]
		public static void printstats(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("{0:F1}s alive", basePlayer.lifeStory.secondsAlive));
			stringBuilder.AppendLine(string.Format("{0:F1}s sleeping", basePlayer.lifeStory.secondsSleeping));
			stringBuilder.AppendLine(string.Format("{0:F1}s swimming", basePlayer.lifeStory.secondsSwimming));
			stringBuilder.AppendLine(string.Format("{0:F1}s in base", basePlayer.lifeStory.secondsInBase));
			stringBuilder.AppendLine(string.Format("{0:F1}s in wilderness", basePlayer.lifeStory.secondsWilderness));
			stringBuilder.AppendLine(string.Format("{0:F1}s in monuments", basePlayer.lifeStory.secondsInMonument));
			stringBuilder.AppendLine(string.Format("{0:F1}s flying", basePlayer.lifeStory.secondsFlying));
			stringBuilder.AppendLine(string.Format("{0:F1}s boating", basePlayer.lifeStory.secondsBoating));
			stringBuilder.AppendLine(string.Format("{0:F1}s driving", basePlayer.lifeStory.secondsDriving));
			stringBuilder.AppendLine(string.Format("{0:F1}m run", basePlayer.lifeStory.metersRun));
			stringBuilder.AppendLine(string.Format("{0:F1}m walked", basePlayer.lifeStory.metersWalked));
			stringBuilder.AppendLine(string.Format("{0:F1} damage taken", basePlayer.lifeStory.totalDamageTaken));
			stringBuilder.AppendLine(string.Format("{0:F1} damage healed", basePlayer.lifeStory.totalHealing));
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine(string.Format("{0} other players killed", basePlayer.lifeStory.killedPlayers));
			stringBuilder.AppendLine(string.Format("{0} scientists killed", basePlayer.lifeStory.killedScientists));
			stringBuilder.AppendLine(string.Format("{0} animals killed", basePlayer.lifeStory.killedAnimals));
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Weapon stats:");
			if (basePlayer.lifeStory.weaponStats != null)
			{
				foreach (PlayerLifeStory.WeaponStats weaponStats in basePlayer.lifeStory.weaponStats)
				{
					float num = weaponStats.shotsHit / weaponStats.shotsFired;
					num *= 100f;
					stringBuilder.AppendLine(string.Format("{0} - shots fired: {1} shots hit: {2} accuracy: {3:F1}%", new object[] { weaponStats.weaponName, weaponStats.shotsFired, weaponStats.shotsHit, num }));
				}
			}
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Misc stats:");
			if (basePlayer.lifeStory.genericStats != null)
			{
				foreach (PlayerLifeStory.GenericStat genericStat in basePlayer.lifeStory.genericStats)
				{
					stringBuilder.AppendLine(string.Format("{0} = {1}", genericStat.key, genericStat.value));
				}
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x060042B1 RID: 17073 RVA: 0x0018A7D4 File Offset: 0x001889D4
		[ServerVar]
		public static void printpresence(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			bool flag = (basePlayer.currentTimeCategory & 1) != 0;
			bool flag2 = (basePlayer.currentTimeCategory & 4) != 0;
			bool flag3 = (basePlayer.currentTimeCategory & 2) != 0;
			bool flag4 = (basePlayer.currentTimeCategory & 32) != 0;
			bool flag5 = (basePlayer.currentTimeCategory & 16) != 0;
			bool flag6 = (basePlayer.currentTimeCategory & 8) != 0;
			arg.ReplyWith(string.Format("Wilderness:{0} Base:{1} Monument:{2} Swimming: {3} Boating: {4} Flying: {5}", new object[] { flag, flag2, flag3, flag4, flag5, flag6 }));
		}

		// Token: 0x060042B2 RID: 17074 RVA: 0x0018A880 File Offset: 0x00188A80
		[ServerVar(Help = "Resets the PlayerState of the given player")]
		public static void resetstate(ConsoleSystem.Arg args)
		{
			global::BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				args.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.ResetPlayerState();
			args.ReplyWith("Player state reset");
		}

		// Token: 0x060042B3 RID: 17075 RVA: 0x0018A8BC File Offset: 0x00188ABC
		[ServerVar(ServerAdmin = true)]
		public static void fillwater(ConsoleSystem.Arg arg)
		{
			bool flag = arg.GetString(0, "").ToLower() == "salt";
			global::BasePlayer basePlayer = arg.Player();
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(flag ? "water.salt" : "water");
			for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
			{
				global::Item itemInSlot = basePlayer.Belt.GetItemInSlot(i);
				BaseLiquidVessel baseLiquidVessel;
				if (itemInSlot != null && (baseLiquidVessel = itemInSlot.GetHeldEntity() as BaseLiquidVessel) != null && baseLiquidVessel.hasLid)
				{
					int num = 999;
					ItemModContainer itemModContainer;
					if (baseLiquidVessel.GetItem().info.TryGetComponent<ItemModContainer>(out itemModContainer))
					{
						num = itemModContainer.maxStackSize;
					}
					baseLiquidVessel.AddLiquid(itemDefinition, num);
				}
			}
		}

		// Token: 0x060042B4 RID: 17076 RVA: 0x0018A968 File Offset: 0x00188B68
		[ServerVar(ServerAdmin = true)]
		public static void reloadweapons(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
			{
				global::Item itemInSlot = basePlayer.Belt.GetItemInSlot(i);
				if (itemInSlot != null)
				{
					global::BaseProjectile baseProjectile;
					FlameThrower flameThrower;
					LiquidWeapon liquidWeapon;
					if ((baseProjectile = itemInSlot.GetHeldEntity() as global::BaseProjectile) != null)
					{
						if (baseProjectile.primaryMagazine != null)
						{
							baseProjectile.primaryMagazine.contents = baseProjectile.primaryMagazine.capacity;
							baseProjectile.SendNetworkUpdateImmediate(false);
						}
					}
					else if ((flameThrower = itemInSlot.GetHeldEntity() as FlameThrower) != null)
					{
						flameThrower.ammo = flameThrower.maxAmmo;
						flameThrower.SendNetworkUpdateImmediate(false);
					}
					else if ((liquidWeapon = itemInSlot.GetHeldEntity() as LiquidWeapon) != null)
					{
						liquidWeapon.AddLiquid(ItemManager.FindItemDefinition("water"), 999);
					}
				}
			}
		}

		// Token: 0x060042B5 RID: 17077 RVA: 0x0018AA2C File Offset: 0x00188C2C
		[ServerVar]
		public static void createskull(ConsoleSystem.Arg arg)
		{
			string text = arg.GetString(0, "");
			global::BasePlayer basePlayer = arg.Player();
			if (string.IsNullOrEmpty(text))
			{
				text = RandomUsernames.Get(UnityEngine.Random.Range(0, 1000));
			}
			global::Item item = ItemManager.Create(ItemManager.FindItemDefinition("skull.human"), 1, 0UL);
			item.name = HumanBodyResourceDispenser.CreateSkullName(text);
			item.streamerName = item.name;
			basePlayer.inventory.GiveItem(item, null, false);
		}

		// Token: 0x060042B6 RID: 17078 RVA: 0x0018AAA0 File Offset: 0x00188CA0
		[ServerVar]
		public static void gesture_radius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<string> list = Pool.GetList<string>();
			for (int i = 0; i < 5; i++)
			{
				if (!string.IsNullOrEmpty(arg.GetString(i + 1, "")))
				{
					list.Add(arg.GetString(i + 1, ""));
				}
			}
			if (list.Count == 0)
			{
				arg.ReplyWith("No gestures provided. eg. player.gesture_radius 10f cabbagepatch raiseroof");
				return;
			}
			List<global::BasePlayer> list2 = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(basePlayer.transform.position, @float, list2, 131072, QueryTriggerInteraction.Collide);
			foreach (global::BasePlayer basePlayer2 in list2)
			{
				GestureConfig gestureConfig = basePlayer.gestureList.StringToGesture(list[UnityEngine.Random.Range(0, list.Count)]);
				basePlayer2.Server_StartGesture(gestureConfig);
			}
			Pool.FreeList<global::BasePlayer>(ref list2);
		}

		// Token: 0x060042B7 RID: 17079 RVA: 0x0018ABB0 File Offset: 0x00188DB0
		[ServerVar]
		public static void stopgesture_radius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(basePlayer.transform.position, @float, list, 131072, QueryTriggerInteraction.Collide);
			foreach (global::BasePlayer basePlayer2 in list)
			{
				basePlayer2.Server_CancelGesture();
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}

		// Token: 0x060042B8 RID: 17080 RVA: 0x0018AC48 File Offset: 0x00188E48
		[ServerVar]
		public static void markhostile(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer != null)
			{
				basePlayer.MarkHostileFor(60f);
			}
		}

		// Token: 0x04003BFB RID: 15355
		[ServerVar]
		public static int tickrate_cl = 20;

		// Token: 0x04003BFC RID: 15356
		[ServerVar]
		public static int tickrate_sv = 16;

		// Token: 0x04003BFD RID: 15357
		[ClientVar(ClientInfo = true)]
		public static bool InfiniteAmmo = false;

		// Token: 0x04003BFE RID: 15358
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Whether the crawling state expires")]
		public static bool woundforever = false;
	}
}
