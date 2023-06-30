using System;

namespace ConVar
{
	// Token: 0x02000AC9 RID: 2761
	[ConsoleSystem.Factory("halloween")]
	public class Halloween : ConsoleSystem
	{
		// Token: 0x04003BDC RID: 15324
		[ServerVar]
		public static bool enabled = false;

		// Token: 0x04003BDD RID: 15325
		[ServerVar(Help = "Population active on the server, per square km")]
		public static float murdererpopulation = 0f;

		// Token: 0x04003BDE RID: 15326
		[ServerVar(Help = "Population active on the server, per square km")]
		public static float scarecrowpopulation = 0f;

		// Token: 0x04003BDF RID: 15327
		[ServerVar(Help = "Scarecrows can throw beancans (Default: true).")]
		public static bool scarecrows_throw_beancans = true;

		// Token: 0x04003BE0 RID: 15328
		[ServerVar(Help = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).")]
		public static float scarecrow_throw_beancan_global_delay = 8f;

		// Token: 0x04003BE1 RID: 15329
		[ServerVar(Help = "Modified damage from beancan explosion vs players (Default: 0.1).")]
		public static float scarecrow_beancan_vs_player_dmg_modifier = 0.1f;

		// Token: 0x04003BE2 RID: 15330
		[ServerVar(Help = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)")]
		public static float scarecrow_body_dmg_modifier = 0.25f;

		// Token: 0x04003BE3 RID: 15331
		[ServerVar(Help = "Stopping distance for destinations set while chasing a target (Default: 0.5)")]
		public static float scarecrow_chase_stopping_distance = 0.5f;
	}
}
