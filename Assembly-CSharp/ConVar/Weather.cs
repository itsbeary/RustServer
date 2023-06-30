using System;
using System.Globalization;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AF1 RID: 2801
	[ConsoleSystem.Factory("weather")]
	public class Weather : ConsoleSystem
	{
		// Token: 0x06004350 RID: 17232 RVA: 0x0018D3DC File Offset: 0x0018B5DC
		[ClientVar]
		[ServerVar]
		public static void load(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			string name = args.GetString(0, "");
			if (string.IsNullOrEmpty(name))
			{
				args.ReplyWith("Weather preset name invalid.");
				return;
			}
			WeatherPreset weatherPreset = Array.Find<WeatherPreset>(SingletonComponent<Climate>.Instance.WeatherPresets, (WeatherPreset x) => x.name.Contains(name, CompareOptions.IgnoreCase));
			if (weatherPreset == null)
			{
				args.ReplyWith("Weather preset not found: " + name);
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Set(weatherPreset);
			if (args.IsServerside)
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		// Token: 0x06004351 RID: 17233 RVA: 0x0018D485 File Offset: 0x0018B685
		[ClientVar]
		[ServerVar]
		public static void reset(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Reset();
			if (args.IsServerside)
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		// Token: 0x06004352 RID: 17234 RVA: 0x0018D4B8 File Offset: 0x0018B6B8
		[ClientVar]
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStatePrevious.name);
			textTable.AddColumn("|");
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStateTarget.name);
			textTable.AddColumn("|");
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStateNext.name);
			int num = Mathf.RoundToInt(SingletonComponent<Climate>.Instance.WeatherStateBlend * 100f);
			if (num < 100)
			{
				textTable.AddRow(new string[]
				{
					"fading out (" + (100 - num) + "%)",
					"|",
					"fading in (" + num + "%)",
					"|",
					"up next"
				});
			}
			else
			{
				textTable.AddRow(new string[] { "previous", "|", "current", "|", "up next" });
			}
			args.ReplyWith(textTable.ToString() + Environment.NewLine + SingletonComponent<Climate>.Instance.WeatherState.ToString());
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06004353 RID: 17235 RVA: 0x0018D5FA File Offset: 0x0018B7FA
		// (set) Token: 0x06004354 RID: 17236 RVA: 0x0018D61D File Offset: 0x0018B81D
		[ReplicatedVar(Default = "1")]
		public static float clear_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 1f;
				}
				return SingletonComponent<Climate>.Instance.Weather.ClearChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.ClearChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06004355 RID: 17237 RVA: 0x0018D641 File Offset: 0x0018B841
		// (set) Token: 0x06004356 RID: 17238 RVA: 0x0018D664 File Offset: 0x0018B864
		[ReplicatedVar(Default = "0")]
		public static float dust_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.DustChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.DustChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06004357 RID: 17239 RVA: 0x0018D688 File Offset: 0x0018B888
		// (set) Token: 0x06004358 RID: 17240 RVA: 0x0018D6AB File Offset: 0x0018B8AB
		[ReplicatedVar(Default = "0")]
		public static float fog_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.FogChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.FogChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06004359 RID: 17241 RVA: 0x0018D6CF File Offset: 0x0018B8CF
		// (set) Token: 0x0600435A RID: 17242 RVA: 0x0018D6F2 File Offset: 0x0018B8F2
		[ReplicatedVar(Default = "0")]
		public static float overcast_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.OvercastChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.OvercastChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x0600435B RID: 17243 RVA: 0x0018D716 File Offset: 0x0018B916
		// (set) Token: 0x0600435C RID: 17244 RVA: 0x0018D739 File Offset: 0x0018B939
		[ReplicatedVar(Default = "0")]
		public static float storm_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.StormChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.StormChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x0600435D RID: 17245 RVA: 0x0018D75D File Offset: 0x0018B95D
		// (set) Token: 0x0600435E RID: 17246 RVA: 0x0018D780 File Offset: 0x0018B980
		[ReplicatedVar(Default = "0")]
		public static float rain_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.RainChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.RainChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x0600435F RID: 17247 RVA: 0x0018D7A4 File Offset: 0x0018B9A4
		// (set) Token: 0x06004360 RID: 17248 RVA: 0x0018D7C7 File Offset: 0x0018B9C7
		[ReplicatedVar(Default = "-1")]
		public static float rain
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rain;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Rain = value;
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06004361 RID: 17249 RVA: 0x0018D7E6 File Offset: 0x0018B9E6
		// (set) Token: 0x06004362 RID: 17250 RVA: 0x0018D809 File Offset: 0x0018BA09
		[ReplicatedVar(Default = "-1")]
		public static float wind
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Wind;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Wind = value;
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06004363 RID: 17251 RVA: 0x0018D828 File Offset: 0x0018BA28
		// (set) Token: 0x06004364 RID: 17252 RVA: 0x0018D84B File Offset: 0x0018BA4B
		[ReplicatedVar(Default = "-1")]
		public static float thunder
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder = value;
			}
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06004365 RID: 17253 RVA: 0x0018D86A File Offset: 0x0018BA6A
		// (set) Token: 0x06004366 RID: 17254 RVA: 0x0018D88D File Offset: 0x0018BA8D
		[ReplicatedVar(Default = "-1")]
		public static float rainbow
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow = value;
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06004367 RID: 17255 RVA: 0x0018D8AC File Offset: 0x0018BAAC
		// (set) Token: 0x06004368 RID: 17256 RVA: 0x0018D8D4 File Offset: 0x0018BAD4
		[ReplicatedVar(Default = "-1")]
		public static float fog
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess = value;
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06004369 RID: 17257 RVA: 0x0018D8F8 File Offset: 0x0018BAF8
		// (set) Token: 0x0600436A RID: 17258 RVA: 0x0018D920 File Offset: 0x0018BB20
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_rayleigh
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier = value;
			}
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x0600436B RID: 17259 RVA: 0x0018D944 File Offset: 0x0018BB44
		// (set) Token: 0x0600436C RID: 17260 RVA: 0x0018D96C File Offset: 0x0018BB6C
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_mie
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier = value;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x0600436D RID: 17261 RVA: 0x0018D990 File Offset: 0x0018BB90
		// (set) Token: 0x0600436E RID: 17262 RVA: 0x0018D9B8 File Offset: 0x0018BBB8
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_brightness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness = value;
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x0600436F RID: 17263 RVA: 0x0018D9DC File Offset: 0x0018BBDC
		// (set) Token: 0x06004370 RID: 17264 RVA: 0x0018DA04 File Offset: 0x0018BC04
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_contrast
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast = value;
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06004371 RID: 17265 RVA: 0x0018DA28 File Offset: 0x0018BC28
		// (set) Token: 0x06004372 RID: 17266 RVA: 0x0018DA50 File Offset: 0x0018BC50
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_directionality
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality = value;
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06004373 RID: 17267 RVA: 0x0018DA74 File Offset: 0x0018BC74
		// (set) Token: 0x06004374 RID: 17268 RVA: 0x0018DA9C File Offset: 0x0018BC9C
		[ReplicatedVar(Default = "-1")]
		public static float cloud_size
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size = value;
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06004375 RID: 17269 RVA: 0x0018DAC0 File Offset: 0x0018BCC0
		// (set) Token: 0x06004376 RID: 17270 RVA: 0x0018DAE8 File Offset: 0x0018BCE8
		[ReplicatedVar(Default = "-1")]
		public static float cloud_opacity
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity = value;
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06004377 RID: 17271 RVA: 0x0018DB0C File Offset: 0x0018BD0C
		// (set) Token: 0x06004378 RID: 17272 RVA: 0x0018DB34 File Offset: 0x0018BD34
		[ReplicatedVar(Default = "-1")]
		public static float cloud_coverage
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage = value;
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06004379 RID: 17273 RVA: 0x0018DB58 File Offset: 0x0018BD58
		// (set) Token: 0x0600437A RID: 17274 RVA: 0x0018DB80 File Offset: 0x0018BD80
		[ReplicatedVar(Default = "-1")]
		public static float cloud_sharpness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness = value;
			}
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x0600437B RID: 17275 RVA: 0x0018DBA4 File Offset: 0x0018BDA4
		// (set) Token: 0x0600437C RID: 17276 RVA: 0x0018DBCC File Offset: 0x0018BDCC
		[ReplicatedVar(Default = "-1")]
		public static float cloud_coloring
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring = value;
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x0600437D RID: 17277 RVA: 0x0018DBF0 File Offset: 0x0018BDF0
		// (set) Token: 0x0600437E RID: 17278 RVA: 0x0018DC18 File Offset: 0x0018BE18
		[ReplicatedVar(Default = "-1")]
		public static float cloud_attenuation
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation = value;
			}
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x0600437F RID: 17279 RVA: 0x0018DC3C File Offset: 0x0018BE3C
		// (set) Token: 0x06004380 RID: 17280 RVA: 0x0018DC64 File Offset: 0x0018BE64
		[ReplicatedVar(Default = "-1")]
		public static float cloud_saturation
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation = value;
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06004381 RID: 17281 RVA: 0x0018DC88 File Offset: 0x0018BE88
		// (set) Token: 0x06004382 RID: 17282 RVA: 0x0018DCB0 File Offset: 0x0018BEB0
		[ReplicatedVar(Default = "-1")]
		public static float cloud_scattering
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering = value;
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06004383 RID: 17283 RVA: 0x0018DCD4 File Offset: 0x0018BED4
		// (set) Token: 0x06004384 RID: 17284 RVA: 0x0018DCFC File Offset: 0x0018BEFC
		[ReplicatedVar(Default = "-1")]
		public static float cloud_brightness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness = value;
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06004385 RID: 17285 RVA: 0x0018DD20 File Offset: 0x0018BF20
		// (set) Token: 0x06004386 RID: 17286 RVA: 0x0018DD43 File Offset: 0x0018BF43
		[ReplicatedVar(Default = "-1")]
		public static float ocean_scale
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.OceanScale;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.OceanScale = value;
			}
		}

		// Token: 0x04003CB6 RID: 15542
		[ServerVar]
		public static float wetness_rain = 0.4f;

		// Token: 0x04003CB7 RID: 15543
		[ServerVar]
		public static float wetness_snow = 0.2f;

		// Token: 0x04003CB8 RID: 15544
		[ReplicatedVar(Default = "-1")]
		public static float ocean_time = -1f;
	}
}
