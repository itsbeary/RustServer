using System;
using Rust.AI;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AAC RID: 2732
	[ConsoleSystem.Factory("ai")]
	public class AI : ConsoleSystem
	{
		// Token: 0x0600415F RID: 16735 RVA: 0x00183108 File Offset: 0x00181308
		[ServerVar]
		public static void sleepwakestats(ConsoleSystem.Arg args)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
			{
				if (!(aiinformationZone == null) && aiinformationZone.ShouldSleepAI)
				{
					num++;
					if (aiinformationZone.Sleeping)
					{
						num2++;
						num3 += aiinformationZone.SleepingCount;
					}
				}
			}
			args.ReplyWith(string.Concat(new object[] { "Sleeping AIZs: ", num2, " / ", num, ". Total sleeping ents: ", num3 }));
		}

		// Token: 0x06004160 RID: 16736 RVA: 0x001831CC File Offset: 0x001813CC
		[ServerVar]
		public static void wakesleepingai(ConsoleSystem.Arg args)
		{
			int num = 0;
			int num2 = 0;
			foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
			{
				if (!(aiinformationZone == null) && aiinformationZone.ShouldSleepAI && aiinformationZone.Sleeping)
				{
					num++;
					num2 += aiinformationZone.SleepingCount;
					aiinformationZone.WakeAI();
				}
			}
			args.ReplyWith(string.Concat(new object[] { "Woke ", num, " sleeping AIZs containing ", num2, " sleeping entities." }));
		}

		// Token: 0x06004161 RID: 16737 RVA: 0x00183284 File Offset: 0x00181484
		[ServerVar]
		public static void brainstats(ConsoleSystem.Arg args)
		{
			args.ReplyWith(string.Concat(new object[]
			{
				"Animal: ",
				AnimalBrain.Count,
				". Scientist: ",
				ScientistBrain.Count,
				". Pet: ",
				PetBrain.Count,
				". Total: ",
				AnimalBrain.Count + ScientistBrain.Count + PetBrain.Count
			}));
		}

		// Token: 0x06004162 RID: 16738 RVA: 0x00183304 File Offset: 0x00181504
		[ServerVar]
		public static void killscientists(ConsoleSystem.Arg args)
		{
			ScientistNPC[] array = BaseEntity.Util.FindAll<ScientistNPC>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
			TunnelDweller[] array2 = BaseEntity.Util.FindAll<TunnelDweller>();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004163 RID: 16739 RVA: 0x0018334C File Offset: 0x0018154C
		[ServerVar]
		public static void killanimals(ConsoleSystem.Arg args)
		{
			BaseAnimalNPC[] array = BaseEntity.Util.FindAll<BaseAnimalNPC>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x00183378 File Offset: 0x00181578
		[ServerVar(Help = "Add a player (or command user if no player is specified) to the AIs ignore list.")]
		public static void addignoreplayer(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer;
			if (!args.HasArgs(1))
			{
				basePlayer = args.Player();
			}
			else
			{
				basePlayer = args.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null || basePlayer.net.connection == null)
			{
				args.ReplyWith("Player not found.");
				return;
			}
			SimpleAIMemory.AddIgnorePlayer(basePlayer);
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x001833D4 File Offset: 0x001815D4
		[ServerVar(Help = "Remove a player (or command user if no player is specified) from the AIs ignore list.")]
		public static void removeignoreplayer(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer;
			if (!args.HasArgs(1))
			{
				basePlayer = args.Player();
			}
			else
			{
				basePlayer = args.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null || basePlayer.net.connection == null)
			{
				args.ReplyWith("Player not found.");
				return;
			}
			SimpleAIMemory.RemoveIgnorePlayer(basePlayer);
		}

		// Token: 0x06004166 RID: 16742 RVA: 0x0018342D File Offset: 0x0018162D
		[ServerVar(Help = "Remove all players from the AIs ignore list.")]
		public static void clearignoredplayers(ConsoleSystem.Arg args)
		{
			SimpleAIMemory.ClearIgnoredPlayers();
		}

		// Token: 0x06004167 RID: 16743 RVA: 0x00183434 File Offset: 0x00181634
		[ServerVar(Help = "Print a lost of all the players in the AI ignore list.")]
		public static void printignoredplayers(ConsoleSystem.Arg args)
		{
			args.ReplyWith(SimpleAIMemory.GetIgnoredPlayers());
		}

		// Token: 0x06004168 RID: 16744 RVA: 0x00183441 File Offset: 0x00181641
		public static float TickDelta()
		{
			return 1f / AI.tickrate;
		}

		// Token: 0x06004169 RID: 16745 RVA: 0x000063A5 File Offset: 0x000045A5
		[ServerVar]
		public static void selectNPCLookatServer(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x04003ABE RID: 15038
		[ReplicatedVar(Saved = true)]
		public static bool allowdesigning = true;

		// Token: 0x04003ABF RID: 15039
		[ServerVar]
		public static bool think = true;

		// Token: 0x04003AC0 RID: 15040
		[ServerVar]
		public static bool navthink = true;

		// Token: 0x04003AC1 RID: 15041
		[ServerVar]
		public static bool ignoreplayers = false;

		// Token: 0x04003AC2 RID: 15042
		[ServerVar]
		public static bool groups = true;

		// Token: 0x04003AC3 RID: 15043
		[ServerVar]
		public static bool spliceupdates = true;

		// Token: 0x04003AC4 RID: 15044
		[ServerVar]
		public static bool setdestinationsamplenavmesh = true;

		// Token: 0x04003AC5 RID: 15045
		[ServerVar]
		public static bool usecalculatepath = true;

		// Token: 0x04003AC6 RID: 15046
		[ServerVar]
		public static bool usesetdestinationfallback = true;

		// Token: 0x04003AC7 RID: 15047
		[ServerVar]
		public static bool npcswimming = true;

		// Token: 0x04003AC8 RID: 15048
		[ServerVar]
		public static bool accuratevisiondistance = true;

		// Token: 0x04003AC9 RID: 15049
		[ServerVar]
		public static bool move = true;

		// Token: 0x04003ACA RID: 15050
		[ServerVar]
		public static bool usegrid = true;

		// Token: 0x04003ACB RID: 15051
		[ServerVar]
		public static bool sleepwake = true;

		// Token: 0x04003ACC RID: 15052
		[ServerVar]
		public static float sensetime = 1f;

		// Token: 0x04003ACD RID: 15053
		[ServerVar]
		public static float frametime = 5f;

		// Token: 0x04003ACE RID: 15054
		[ServerVar]
		public static int ocean_patrol_path_iterations = 100000;

		// Token: 0x04003ACF RID: 15055
		[ServerVar(Help = "If npc_enable is set to false then npcs won't spawn. (default: true)")]
		public static bool npc_enable = true;

		// Token: 0x04003AD0 RID: 15056
		[ServerVar(Help = "npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)")]
		public static int npc_max_population_military_tunnels = 3;

		// Token: 0x04003AD1 RID: 15057
		[ServerVar(Help = "npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)")]
		public static int npc_spawn_per_tick_max_military_tunnels = 1;

		// Token: 0x04003AD2 RID: 15058
		[ServerVar(Help = "npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)")]
		public static int npc_spawn_per_tick_min_military_tunnels = 1;

		// Token: 0x04003AD3 RID: 15059
		[ServerVar(Help = "npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)")]
		public static float npc_respawn_delay_max_military_tunnels = 1920f;

		// Token: 0x04003AD4 RID: 15060
		[ServerVar(Help = "npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)")]
		public static float npc_respawn_delay_min_military_tunnels = 480f;

		// Token: 0x04003AD5 RID: 15061
		[ServerVar(Help = "npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)")]
		public static float npc_valid_aim_cone = 0.8f;

		// Token: 0x04003AD6 RID: 15062
		[ServerVar(Help = "npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)")]
		public static float npc_valid_mounted_aim_cone = 0.92f;

		// Token: 0x04003AD7 RID: 15063
		[ServerVar(Help = "npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)")]
		public static float npc_cover_compromised_cooldown = 10f;

		// Token: 0x04003AD8 RID: 15064
		[ServerVar(Help = "If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.")]
		public static bool npc_cover_use_path_distance = true;

		// Token: 0x04003AD9 RID: 15065
		[ServerVar(Help = "npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)")]
		public static float npc_cover_path_vs_straight_dist_max_diff = 2f;

		// Token: 0x04003ADA RID: 15066
		[ServerVar(Help = "npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)")]
		public static float npc_door_trigger_size = 1.5f;

		// Token: 0x04003ADB RID: 15067
		[ServerVar(Help = "npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)")]
		public static float npc_patrol_point_cooldown = 5f;

		// Token: 0x04003ADC RID: 15068
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)")]
		public static float npc_speed_walk = 0.18f;

		// Token: 0x04003ADD RID: 15069
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)")]
		public static float npc_speed_run = 0.4f;

		// Token: 0x04003ADE RID: 15070
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)")]
		public static float npc_speed_sprint = 1f;

		// Token: 0x04003ADF RID: 15071
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)")]
		public static float npc_speed_crouch_walk = 0.1f;

		// Token: 0x04003AE0 RID: 15072
		[ServerVar(Help = "npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)")]
		public static float npc_speed_crouch_run = 0.25f;

		// Token: 0x04003AE1 RID: 15073
		[ServerVar(Help = "npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)")]
		public static float npc_alertness_drain_rate = 0.01f;

		// Token: 0x04003AE2 RID: 15074
		[ServerVar(Help = "npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)")]
		public static float npc_alertness_zero_detection_mod = 0.5f;

		// Token: 0x04003AE3 RID: 15075
		[ServerVar(Help = "defines the chance for scientists to spawn at NPC junkpiles. (Default: 0.1)")]
		public static float npc_junkpilespawn_chance = 0.07f;

		// Token: 0x04003AE4 RID: 15076
		[ServerVar(Help = "npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)")]
		public static float npc_junkpile_a_spawn_chance = 0.1f;

		// Token: 0x04003AE5 RID: 15077
		[ServerVar(Help = "npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)")]
		public static float npc_junkpile_g_spawn_chance = 0.1f;

		// Token: 0x04003AE6 RID: 15078
		[ServerVar(Help = "npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)")]
		public static float npc_junkpile_dist_aggro_gate = 8f;

		// Token: 0x04003AE7 RID: 15079
		[ServerVar(Help = "npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)")]
		public static int npc_max_junkpile_count = 30;

		// Token: 0x04003AE8 RID: 15080
		[ServerVar(Help = "If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)")]
		public static bool npc_families_no_hurt = true;

		// Token: 0x04003AE9 RID: 15081
		[ServerVar(Help = "If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)")]
		public static bool npc_ignore_chairs = true;

		// Token: 0x04003AEA RID: 15082
		[ServerVar(Help = "The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)")]
		public static float npc_sensory_system_tick_rate_multiplier = 5f;

		// Token: 0x04003AEB RID: 15083
		[ServerVar(Help = "The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)")]
		public static float npc_cover_info_tick_rate_multiplier = 20f;

		// Token: 0x04003AEC RID: 15084
		[ServerVar(Help = "The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)")]
		public static float npc_reasoning_system_tick_rate_multiplier = 1f;

		// Token: 0x04003AED RID: 15085
		[ServerVar(Help = "If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)")]
		public static bool animal_ignore_food = true;

		// Token: 0x04003AEE RID: 15086
		[ServerVar(Help = "The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)")]
		public static float npc_gun_noise_silencer_modifier = 0.15f;

		// Token: 0x04003AEF RID: 15087
		[ServerVar(Help = "If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)")]
		public static bool nav_carve_use_building_optimization = false;

		// Token: 0x04003AF0 RID: 15088
		[ServerVar(Help = "The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)")]
		public static int nav_carve_min_building_blocks_to_apply_optimization = 25;

		// Token: 0x04003AF1 RID: 15089
		[ServerVar(Help = "The minimum size we allow a carving volume to be. (default: 2)")]
		public static float nav_carve_min_base_size = 2f;

		// Token: 0x04003AF2 RID: 15090
		[ServerVar(Help = "The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)")]
		public static float nav_carve_size_multiplier = 4f;

		// Token: 0x04003AF3 RID: 15091
		[ServerVar(Help = "The height of the carve volume. (default: 2)")]
		public static float nav_carve_height = 2f;

		// Token: 0x04003AF4 RID: 15092
		[ServerVar(Help = "If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)")]
		public static bool npc_only_hurt_active_target_in_safezone = true;

		// Token: 0x04003AF5 RID: 15093
		[ServerVar(Help = "If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)")]
		public static bool npc_use_new_aim_system = true;

		// Token: 0x04003AF6 RID: 15094
		[ServerVar(Help = "If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)")]
		public static bool npc_use_thrown_weapons = true;

		// Token: 0x04003AF7 RID: 15095
		[ServerVar(Help = "This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)")]
		public static float npc_max_roam_multiplier = 3f;

		// Token: 0x04003AF8 RID: 15096
		[ServerVar(Help = "This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)")]
		public static float npc_alertness_to_aim_modifier = 0.5f;

		// Token: 0x04003AF9 RID: 15097
		[ServerVar(Help = "The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)")]
		public static float npc_deliberate_miss_to_hit_alignment_time = 1.5f;

		// Token: 0x04003AFA RID: 15098
		[ServerVar(Help = "The offset with which the NPC will maximum miss the target. (default: 1.25)")]
		public static float npc_deliberate_miss_offset_multiplier = 1.25f;

		// Token: 0x04003AFB RID: 15099
		[ServerVar(Help = "The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)")]
		public static float npc_deliberate_hit_randomizer = 0.85f;

		// Token: 0x04003AFC RID: 15100
		[ServerVar(Help = "Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)")]
		public static float npc_htn_player_base_damage_modifier = 1.15f;

		// Token: 0x04003AFD RID: 15101
		[ServerVar(Help = "Spawn NPCs on the Cargo Ship. (default: true)")]
		public static bool npc_spawn_on_cargo_ship = true;

		// Token: 0x04003AFE RID: 15102
		[ServerVar(Help = "npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)")]
		public static int npc_htn_player_frustration_threshold = 3;

		// Token: 0x04003AFF RID: 15103
		[ServerVar]
		public static float tickrate = 5f;
	}
}
