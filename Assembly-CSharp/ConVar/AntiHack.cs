using System;

namespace ConVar
{
	// Token: 0x02000AAD RID: 2733
	[ConsoleSystem.Factory("antihack")]
	public class AntiHack : ConsoleSystem
	{
		// Token: 0x04003B00 RID: 15104
		[ReplicatedVar]
		[Help("collider margin when checking for noclipping on dismount")]
		public static float noclip_margin_dismount = 0.22f;

		// Token: 0x04003B01 RID: 15105
		[ReplicatedVar]
		[Help("collider backtracking when checking for noclipping")]
		public static float noclip_backtracking = 0.01f;

		// Token: 0x04003B02 RID: 15106
		[ServerVar]
		[Help("report violations to the anti cheat backend")]
		public static bool reporting = false;

		// Token: 0x04003B03 RID: 15107
		[ServerVar]
		[Help("are admins allowed to use their admin cheat")]
		public static bool admincheat = true;

		// Token: 0x04003B04 RID: 15108
		[ServerVar]
		[Help("use antihack to verify object placement by players")]
		public static bool objectplacement = true;

		// Token: 0x04003B05 RID: 15109
		[ServerVar]
		[Help("use antihack to verify model state sent by players")]
		public static bool modelstate = true;

		// Token: 0x04003B06 RID: 15110
		[ServerVar]
		[Help("whether or not to force the position on the client")]
		public static bool forceposition = true;

		// Token: 0x04003B07 RID: 15111
		[ServerVar]
		[Help("0 == users, 1 == admins, 2 == developers")]
		public static int userlevel = 2;

		// Token: 0x04003B08 RID: 15112
		[ServerVar]
		[Help("0 == no enforcement, 1 == kick, 2 == ban (DISABLED)")]
		public static int enforcementlevel = 1;

		// Token: 0x04003B09 RID: 15113
		[ServerVar]
		[Help("max allowed client desync, lower value = more false positives")]
		public static float maxdesync = 1f;

		// Token: 0x04003B0A RID: 15114
		[ServerVar]
		[Help("max allowed client tick interval delta time, lower value = more false positives")]
		public static float maxdeltatime = 1f;

		// Token: 0x04003B0B RID: 15115
		[ServerVar]
		[Help("for how many seconds to keep a tick history to use for distance checks")]
		public static float tickhistorytime = 0.5f;

		// Token: 0x04003B0C RID: 15116
		[ServerVar]
		[Help("how much forgiveness to add when checking the distance from the player tick history")]
		public static float tickhistoryforgiveness = 0.1f;

		// Token: 0x04003B0D RID: 15117
		[ServerVar]
		[Help("the rate at which violation values go back down")]
		public static float relaxationrate = 0.1f;

		// Token: 0x04003B0E RID: 15118
		[ServerVar]
		[Help("the time before violation values go back down")]
		public static float relaxationpause = 10f;

		// Token: 0x04003B0F RID: 15119
		[ServerVar]
		[Help("violation value above this results in enforcement")]
		public static float maxviolation = 100f;

		// Token: 0x04003B10 RID: 15120
		[ServerVar]
		[Help("0 == disabled, 1 == enabled")]
		public static int terrain_protection = 1;

		// Token: 0x04003B11 RID: 15121
		[ServerVar]
		[Help("how many slices to subdivide players into for the terrain check")]
		public static int terrain_timeslice = 64;

		// Token: 0x04003B12 RID: 15122
		[ServerVar]
		[Help("how far to penetrate the terrain before violating")]
		public static float terrain_padding = 0.3f;

		// Token: 0x04003B13 RID: 15123
		[ServerVar]
		[Help("violation penalty to hand out when terrain is detected")]
		public static float terrain_penalty = 100f;

		// Token: 0x04003B14 RID: 15124
		[ServerVar]
		[Help("whether or not to kill the player when terrain is detected")]
		public static bool terrain_kill = true;

		// Token: 0x04003B15 RID: 15125
		[ServerVar]
		[Help("whether or not to check for player inside geometry like rocks as well as base terrain")]
		public static bool terrain_check_geometry = false;

		// Token: 0x04003B16 RID: 15126
		[ServerVar]
		[Help("0 == disabled, 1 == ray, 2 == sphere, 3 == curve")]
		public static int noclip_protection = 3;

		// Token: 0x04003B17 RID: 15127
		[ServerVar]
		[Help("whether or not to reject movement when noclip is detected")]
		public static bool noclip_reject = true;

		// Token: 0x04003B18 RID: 15128
		[ServerVar]
		[Help("violation penalty to hand out when noclip is detected")]
		public static float noclip_penalty = 0f;

		// Token: 0x04003B19 RID: 15129
		[ServerVar]
		[Help("collider margin when checking for noclipping")]
		public static float noclip_margin = 0.09f;

		// Token: 0x04003B1A RID: 15130
		[ServerVar]
		[Help("movement curve step size, lower value = less false positives")]
		public static float noclip_stepsize = 0.1f;

		// Token: 0x04003B1B RID: 15131
		[ServerVar]
		[Help("movement curve max steps, lower value = more false positives")]
		public static int noclip_maxsteps = 15;

		// Token: 0x04003B1C RID: 15132
		[ServerVar]
		[Help("0 == disabled, 1 == simple, 2 == advanced")]
		public static int speedhack_protection = 2;

		// Token: 0x04003B1D RID: 15133
		[ServerVar]
		[Help("whether or not to reject movement when speedhack is detected")]
		public static bool speedhack_reject = true;

		// Token: 0x04003B1E RID: 15134
		[ServerVar]
		[Help("violation penalty to hand out when speedhack is detected")]
		public static float speedhack_penalty = 0f;

		// Token: 0x04003B1F RID: 15135
		[ServerVar]
		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		public static float speedhack_forgiveness = 2f;

		// Token: 0x04003B20 RID: 15136
		[ServerVar]
		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		public static float speedhack_forgiveness_inertia = 10f;

		// Token: 0x04003B21 RID: 15137
		[ServerVar]
		[Help("speed forgiveness when moving down slopes, lower value = more false positives")]
		public static float speedhack_slopespeed = 10f;

		// Token: 0x04003B22 RID: 15138
		[ServerVar]
		[Help("0 == disabled, 1 == client, 2 == capsule, 3 == curve")]
		public static int flyhack_protection = 3;

		// Token: 0x04003B23 RID: 15139
		[ServerVar]
		[Help("whether or not to reject movement when flyhack is detected")]
		public static bool flyhack_reject = false;

		// Token: 0x04003B24 RID: 15140
		[ServerVar]
		[Help("violation penalty to hand out when flyhack is detected")]
		public static float flyhack_penalty = 100f;

		// Token: 0x04003B25 RID: 15141
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_vertical = 1.5f;

		// Token: 0x04003B26 RID: 15142
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_vertical_inertia = 10f;

		// Token: 0x04003B27 RID: 15143
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_horizontal = 1.5f;

		// Token: 0x04003B28 RID: 15144
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_horizontal_inertia = 10f;

		// Token: 0x04003B29 RID: 15145
		[ServerVar]
		[Help("collider downwards extrusion when checking for flyhacking")]
		public static float flyhack_extrusion = 2f;

		// Token: 0x04003B2A RID: 15146
		[ServerVar]
		[Help("collider margin when checking for flyhacking")]
		public static float flyhack_margin = 0.05f;

		// Token: 0x04003B2B RID: 15147
		[ServerVar]
		[Help("movement curve step size, lower value = less false positives")]
		public static float flyhack_stepsize = 0.1f;

		// Token: 0x04003B2C RID: 15148
		[ServerVar]
		[Help("movement curve max steps, lower value = more false positives")]
		public static int flyhack_maxsteps = 15;

		// Token: 0x04003B2D RID: 15149
		[ServerVar]
		[Help("0 == disabled, 1 == speed, 2 == speed + entity, 3 == speed + entity + LOS, 4 == speed + entity + LOS + trajectory, 5 == speed + entity + LOS + trajectory + update, 6 == speed + entity + LOS + trajectory + tickhistory")]
		public static int projectile_protection = 6;

		// Token: 0x04003B2E RID: 15150
		[ServerVar]
		[Help("violation penalty to hand out when projectile hack is detected")]
		public static float projectile_penalty = 0f;

		// Token: 0x04003B2F RID: 15151
		[ServerVar]
		[Help("projectile speed forgiveness in percent, lower value = more false positives")]
		public static float projectile_forgiveness = 0.5f;

		// Token: 0x04003B30 RID: 15152
		[ServerVar]
		[Help("projectile server frames to include in delay, lower value = more false positives")]
		public static float projectile_serverframes = 2f;

		// Token: 0x04003B31 RID: 15153
		[ServerVar]
		[Help("projectile client frames to include in delay, lower value = more false positives")]
		public static float projectile_clientframes = 2f;

		// Token: 0x04003B32 RID: 15154
		[ServerVar]
		[Help("projectile trajectory forgiveness, lower value = more false positives")]
		public static float projectile_trajectory = 1f;

		// Token: 0x04003B33 RID: 15155
		[ServerVar]
		[Help("projectile penetration angle change, lower value = more false positives")]
		public static float projectile_anglechange = 60f;

		// Token: 0x04003B34 RID: 15156
		[ServerVar]
		[Help("projectile penetration velocity change, lower value = more false positives")]
		public static float projectile_velocitychange = 1.1f;

		// Token: 0x04003B35 RID: 15157
		[ServerVar]
		[Help("projectile desync forgiveness, lower value = more false positives")]
		public static float projectile_desync = 1f;

		// Token: 0x04003B36 RID: 15158
		[ServerVar]
		[Help("projectile backtracking when checking for LOS")]
		public static float projectile_backtracking = 0.01f;

		// Token: 0x04003B37 RID: 15159
		[ServerVar]
		[Help("line of sight directional forgiveness when checking eye or center position")]
		public static float projectile_losforgiveness = 0.2f;

		// Token: 0x04003B38 RID: 15160
		[ServerVar]
		[Help("how often a projectile is allowed to penetrate something before its damage is ignored")]
		public static int projectile_damagedepth = 2;

		// Token: 0x04003B39 RID: 15161
		[ServerVar]
		[Help("how often a projectile is allowed to penetrate something before its impact spawn is ignored")]
		public static int projectile_impactspawndepth = 1;

		// Token: 0x04003B3A RID: 15162
		[ServerVar]
		[Help("whether or not to include terrain in the projectile LOS checks")]
		public static bool projectile_terraincheck = true;

		// Token: 0x04003B3B RID: 15163
		[ServerVar]
		[Help("0 == disabled, 1 == initiator, 2 == initiator + target, 3 == initiator + target + LOS, 4 == initiator + target + LOS + tickhistory")]
		public static int melee_protection = 4;

		// Token: 0x04003B3C RID: 15164
		[ServerVar]
		[Help("violation penalty to hand out when melee hack is detected")]
		public static float melee_penalty = 0f;

		// Token: 0x04003B3D RID: 15165
		[ServerVar]
		[Help("melee distance forgiveness in percent, lower value = more false positives")]
		public static float melee_forgiveness = 0.5f;

		// Token: 0x04003B3E RID: 15166
		[ServerVar]
		[Help("melee server frames to include in delay, lower value = more false positives")]
		public static float melee_serverframes = 2f;

		// Token: 0x04003B3F RID: 15167
		[ServerVar]
		[Help("melee client frames to include in delay, lower value = more false positives")]
		public static float melee_clientframes = 2f;

		// Token: 0x04003B40 RID: 15168
		[ServerVar]
		[Help("line of sight directional forgiveness when checking eye or center position")]
		public static float melee_losforgiveness = 0.2f;

		// Token: 0x04003B41 RID: 15169
		[ServerVar]
		[Help("whether or not to include terrain in the melee LOS checks")]
		public static bool melee_terraincheck = true;

		// Token: 0x04003B42 RID: 15170
		[ServerVar]
		[Help("0 == disabled, 1 == distance, 2 == distance + LOS, 3 = distance + LOS + altitude, 4 = distance + LOS + altitude + noclip, 5 = distance + LOS + altitude + noclip + history")]
		public static int eye_protection = 4;

		// Token: 0x04003B43 RID: 15171
		[ServerVar]
		[Help("violation penalty to hand out when eye hack is detected")]
		public static float eye_penalty = 0f;

		// Token: 0x04003B44 RID: 15172
		[ServerVar]
		[Help("eye speed forgiveness in percent, lower value = more false positives")]
		public static float eye_forgiveness = 0.5f;

		// Token: 0x04003B45 RID: 15173
		[ServerVar]
		[Help("eye server frames to include in delay, lower value = more false positives")]
		public static float eye_serverframes = 2f;

		// Token: 0x04003B46 RID: 15174
		[ServerVar]
		[Help("eye client frames to include in delay, lower value = more false positives")]
		public static float eye_clientframes = 2f;

		// Token: 0x04003B47 RID: 15175
		[ServerVar]
		[Help("whether or not to include terrain in the eye LOS checks")]
		public static bool eye_terraincheck = true;

		// Token: 0x04003B48 RID: 15176
		[ServerVar]
		[Help("distance at which to start testing eye noclipping")]
		public static float eye_noclip_cutoff = 0.06f;

		// Token: 0x04003B49 RID: 15177
		[ServerVar]
		[Help("collider margin when checking for noclipping")]
		public static float eye_noclip_margin = 0.21f;

		// Token: 0x04003B4A RID: 15178
		[ServerVar]
		[Help("collider backtracking when checking for noclipping")]
		public static float eye_noclip_backtracking = 0.01f;

		// Token: 0x04003B4B RID: 15179
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float eye_losradius = 0.18f;

		// Token: 0x04003B4C RID: 15180
		[ServerVar]
		[Help("violation penalty to hand out when eye history mismatch is detected")]
		public static float eye_history_penalty = 100f;

		// Token: 0x04003B4D RID: 15181
		[ServerVar]
		[Help("how much forgiveness to add when checking the distance between player tick history and player eye history")]
		public static float eye_history_forgiveness = 0.1f;

		// Token: 0x04003B4E RID: 15182
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float build_losradius = 0.01f;

		// Token: 0x04003B4F RID: 15183
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float build_losradius_sleepingbag = 0.3f;

		// Token: 0x04003B50 RID: 15184
		[ServerVar]
		[Help("whether or not to include terrain in the build LOS checks")]
		public static bool build_terraincheck = true;

		// Token: 0x04003B51 RID: 15185
		[ServerVar]
		[Help("whether or not to check for building being done on the wrong side of something (e.g. inside rocks). 0 = Disabled, 1 = Info only, 2 = Enabled")]
		public static int build_inside_check = 2;

		// Token: 0x04003B52 RID: 15186
		[ServerVar]
		[Help("0 == silent, 1 == print max violation, 2 == print nonzero violation, 3 == print any violation except noclip, 4 == print any violation")]
		public static int debuglevel = 1;
	}
}
