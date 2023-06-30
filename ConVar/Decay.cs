using System;

namespace ConVar
{
	// Token: 0x02000ABB RID: 2747
	[ConsoleSystem.Factory("decay")]
	public class Decay : ConsoleSystem
	{
		// Token: 0x04003B84 RID: 15236
		[ServerVar(Help = "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings")]
		public static float outside_test_range = 50f;

		// Token: 0x04003B85 RID: 15237
		[ServerVar]
		public static float tick = 600f;

		// Token: 0x04003B86 RID: 15238
		[ServerVar]
		public static float scale = 1f;

		// Token: 0x04003B87 RID: 15239
		[ServerVar]
		public static bool debug = false;

		// Token: 0x04003B88 RID: 15240
		[ServerVar(Help = "Is upkeep enabled")]
		public static bool upkeep = true;

		// Token: 0x04003B89 RID: 15241
		[ServerVar(Help = "How many minutes does the upkeep cost last? default : 1440 (24 hours)")]
		public static float upkeep_period_minutes = 1440f;

		// Token: 0x04003B8A RID: 15242
		[ServerVar(Help = "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)")]
		public static float upkeep_grief_protection = 1440f;

		// Token: 0x04003B8B RID: 15243
		[ServerVar(Help = "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay")]
		public static float upkeep_heal_scale = 1f;

		// Token: 0x04003B8C RID: 15244
		[ServerVar(Help = "Scale at which objects decay when they are inside, default of 0.1")]
		public static float upkeep_inside_decay_scale = 0.1f;

		// Token: 0x04003B8D RID: 15245
		[ServerVar(Help = "When set to a value above 0 everything will decay with this delay")]
		public static float delay_override = 0f;

		// Token: 0x04003B8E RID: 15246
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_twig = 0f;

		// Token: 0x04003B8F RID: 15247
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_wood = 0f;

		// Token: 0x04003B90 RID: 15248
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_stone = 0f;

		// Token: 0x04003B91 RID: 15249
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_metal = 0f;

		// Token: 0x04003B92 RID: 15250
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_toptier = 0f;

		// Token: 0x04003B93 RID: 15251
		[ServerVar(Help = "When set to a value above 0 everything will decay with this duration")]
		public static float duration_override = 0f;

		// Token: 0x04003B94 RID: 15252
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_twig = 1f;

		// Token: 0x04003B95 RID: 15253
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_wood = 3f;

		// Token: 0x04003B96 RID: 15254
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_stone = 5f;

		// Token: 0x04003B97 RID: 15255
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_metal = 8f;

		// Token: 0x04003B98 RID: 15256
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_toptier = 12f;

		// Token: 0x04003B99 RID: 15257
		[ServerVar(Help = "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain")]
		public static int bracket_0_blockcount = 15;

		// Token: 0x04003B9A RID: 15258
		[ServerVar(Help = "blocks within bracket 0 will cost this fraction per upkeep period to maintain")]
		public static float bracket_0_costfraction = 0.1f;

		// Token: 0x04003B9B RID: 15259
		[ServerVar(Help = "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain")]
		public static int bracket_1_blockcount = 50;

		// Token: 0x04003B9C RID: 15260
		[ServerVar(Help = "blocks within bracket 1 will cost this fraction per upkeep period to maintain")]
		public static float bracket_1_costfraction = 0.15f;

		// Token: 0x04003B9D RID: 15261
		[ServerVar(Help = "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain")]
		public static int bracket_2_blockcount = 125;

		// Token: 0x04003B9E RID: 15262
		[ServerVar(Help = "blocks within bracket 2 will cost this fraction per upkeep period to maintain")]
		public static float bracket_2_costfraction = 0.2f;

		// Token: 0x04003B9F RID: 15263
		[ServerVar(Help = "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain")]
		public static int bracket_3_blockcount = 200;

		// Token: 0x04003BA0 RID: 15264
		[ServerVar(Help = "blocks within bracket 3 will cost this fraction per upkeep period to maintain")]
		public static float bracket_3_costfraction = 0.333f;
	}
}
