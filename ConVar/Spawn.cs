using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE3 RID: 2787
	[ConsoleSystem.Factory("spawn")]
	public class Spawn : ConsoleSystem
	{
		// Token: 0x06004315 RID: 17173 RVA: 0x0018C6C3 File Offset: 0x0018A8C3
		[ServerVar]
		public static void fill_populations(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillPopulations();
			}
		}

		// Token: 0x06004316 RID: 17174 RVA: 0x0018C6DB File Offset: 0x0018A8DB
		[ServerVar]
		public static void fill_groups(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillGroups();
			}
		}

		// Token: 0x06004317 RID: 17175 RVA: 0x0018C6F3 File Offset: 0x0018A8F3
		[ServerVar]
		public static void fill_individuals(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillIndividuals();
			}
		}

		// Token: 0x06004318 RID: 17176 RVA: 0x0018C70B File Offset: 0x0018A90B
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				args.ReplyWith(SingletonComponent<SpawnHandler>.Instance.GetReport(false));
				return;
			}
			args.ReplyWith("No spawn handler found.");
		}

		// Token: 0x06004319 RID: 17177 RVA: 0x0018C738 File Offset: 0x0018A938
		[ServerVar]
		public static void scalars(ConsoleSystem.Arg args)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("Type");
			textTable.AddColumn("Value");
			textTable.AddRow(new string[]
			{
				"Player Fraction",
				SpawnHandler.PlayerFraction().ToString()
			});
			textTable.AddRow(new string[]
			{
				"Player Excess",
				SpawnHandler.PlayerExcess().ToString()
			});
			textTable.AddRow(new string[]
			{
				"Population Rate",
				SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate).ToString()
			});
			textTable.AddRow(new string[]
			{
				"Population Density",
				SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density).ToString()
			});
			textTable.AddRow(new string[]
			{
				"Group Rate",
				SpawnHandler.PlayerScale(Spawn.player_scale).ToString()
			});
			args.ReplyWith(args.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x0600431A RID: 17178 RVA: 0x0018C850 File Offset: 0x0018AA50
		[ServerVar]
		public static void cargoshipevent(ConsoleSystem.Arg args)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/vehicles/boats/cargoship/cargoshiptest.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity != null)
			{
				baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
				baseEntity.Spawn();
				args.ReplyWith("Cargo ship event has been started");
				return;
			}
			args.ReplyWith("Couldn't find cargo ship prefab - maybe it has been renamed?");
		}

		// Token: 0x04003C84 RID: 15492
		[ServerVar]
		public static float min_rate = 0.5f;

		// Token: 0x04003C85 RID: 15493
		[ServerVar]
		public static float max_rate = 1f;

		// Token: 0x04003C86 RID: 15494
		[ServerVar]
		public static float min_density = 0.5f;

		// Token: 0x04003C87 RID: 15495
		[ServerVar]
		public static float max_density = 1f;

		// Token: 0x04003C88 RID: 15496
		[ServerVar]
		public static float player_base = 100f;

		// Token: 0x04003C89 RID: 15497
		[ServerVar]
		public static float player_scale = 2f;

		// Token: 0x04003C8A RID: 15498
		[ServerVar]
		public static bool respawn_populations = true;

		// Token: 0x04003C8B RID: 15499
		[ServerVar]
		public static bool respawn_groups = true;

		// Token: 0x04003C8C RID: 15500
		[ServerVar]
		public static bool respawn_individuals = true;

		// Token: 0x04003C8D RID: 15501
		[ServerVar]
		public static float tick_populations = 60f;

		// Token: 0x04003C8E RID: 15502
		[ServerVar]
		public static float tick_individuals = 300f;
	}
}
