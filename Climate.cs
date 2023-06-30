using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020004F2 RID: 1266
public class Climate : SingletonComponent<Climate>
{
	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06002901 RID: 10497 RVA: 0x000FCFED File Offset: 0x000FB1ED
	// (set) Token: 0x06002902 RID: 10498 RVA: 0x000FCFF5 File Offset: 0x000FB1F5
	public float WeatherStateBlend { get; private set; }

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x06002903 RID: 10499 RVA: 0x000FCFFE File Offset: 0x000FB1FE
	// (set) Token: 0x06002904 RID: 10500 RVA: 0x000FD006 File Offset: 0x000FB206
	public uint WeatherSeedPrevious { get; private set; }

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x06002905 RID: 10501 RVA: 0x000FD00F File Offset: 0x000FB20F
	// (set) Token: 0x06002906 RID: 10502 RVA: 0x000FD017 File Offset: 0x000FB217
	public uint WeatherSeedTarget { get; private set; }

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x06002907 RID: 10503 RVA: 0x000FD020 File Offset: 0x000FB220
	// (set) Token: 0x06002908 RID: 10504 RVA: 0x000FD028 File Offset: 0x000FB228
	public uint WeatherSeedNext { get; private set; }

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x06002909 RID: 10505 RVA: 0x000FD031 File Offset: 0x000FB231
	// (set) Token: 0x0600290A RID: 10506 RVA: 0x000FD039 File Offset: 0x000FB239
	public WeatherPreset WeatherStatePrevious { get; private set; }

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x0600290B RID: 10507 RVA: 0x000FD042 File Offset: 0x000FB242
	// (set) Token: 0x0600290C RID: 10508 RVA: 0x000FD04A File Offset: 0x000FB24A
	public WeatherPreset WeatherStateTarget { get; private set; }

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x0600290D RID: 10509 RVA: 0x000FD053 File Offset: 0x000FB253
	// (set) Token: 0x0600290E RID: 10510 RVA: 0x000FD05B File Offset: 0x000FB25B
	public WeatherPreset WeatherStateNext { get; private set; }

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x0600290F RID: 10511 RVA: 0x000FD064 File Offset: 0x000FB264
	// (set) Token: 0x06002910 RID: 10512 RVA: 0x000FD06C File Offset: 0x000FB26C
	public WeatherPreset WeatherState { get; private set; }

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06002911 RID: 10513 RVA: 0x000FD075 File Offset: 0x000FB275
	// (set) Token: 0x06002912 RID: 10514 RVA: 0x000FD07D File Offset: 0x000FB27D
	public WeatherPreset WeatherClampsMin { get; private set; }

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06002913 RID: 10515 RVA: 0x000FD086 File Offset: 0x000FB286
	// (set) Token: 0x06002914 RID: 10516 RVA: 0x000FD08E File Offset: 0x000FB28E
	public WeatherPreset WeatherClampsMax { get; private set; }

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06002915 RID: 10517 RVA: 0x000FD097 File Offset: 0x000FB297
	// (set) Token: 0x06002916 RID: 10518 RVA: 0x000FD09F File Offset: 0x000FB29F
	public WeatherPreset WeatherOverrides { get; private set; }

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06002917 RID: 10519 RVA: 0x000FD0A8 File Offset: 0x000FB2A8
	// (set) Token: 0x06002918 RID: 10520 RVA: 0x000FD0B0 File Offset: 0x000FB2B0
	public LegacyWeatherState Overrides { get; private set; }

	// Token: 0x06002919 RID: 10521 RVA: 0x000FD0BC File Offset: 0x000FB2BC
	protected override void Awake()
	{
		base.Awake();
		this.WeatherState = ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset;
		this.WeatherClampsMin = ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset;
		this.WeatherClampsMax = ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset;
		this.WeatherOverrides = ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset;
		this.WeatherState.Reset();
		this.WeatherClampsMin.Reset();
		this.WeatherClampsMax.Reset();
		this.WeatherOverrides.Reset();
		this.Overrides = new LegacyWeatherState(this.WeatherOverrides);
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x000FD174 File Offset: 0x000FB374
	protected override void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.OnDestroy();
		if (this.WeatherState != null)
		{
			UnityEngine.Object.Destroy(this.WeatherState);
		}
		if (this.WeatherClampsMin != null)
		{
			UnityEngine.Object.Destroy(this.WeatherClampsMin);
		}
		if (this.WeatherClampsMax != null)
		{
			UnityEngine.Object.Destroy(this.WeatherClampsMax);
		}
		if (this.WeatherOverrides != null)
		{
			UnityEngine.Object.Destroy(this.WeatherOverrides);
		}
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000FD1F4 File Offset: 0x000FB3F4
	protected void Update()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		long num = (long)((ulong)World.Seed + (ulong)instance.Cycle.Ticks);
		long num2 = 648000000000L;
		long num3 = 216000000000L;
		long num4 = num / num2;
		this.WeatherStateBlend = Mathf.InverseLerp(0f, (float)num3, (float)(num % num2));
		this.WeatherStatePrevious = this.GetWeatherPreset(this.WeatherSeedPrevious = this.GetSeedFromLong(num4));
		this.WeatherStateTarget = this.GetWeatherPreset(this.WeatherSeedTarget = this.GetSeedFromLong(num4 + 1L));
		this.WeatherStateNext = this.GetWeatherPreset(this.WeatherSeedNext = this.GetSeedFromLong(num4 + 2L));
		this.WeatherState.Fade(this.WeatherStatePrevious, this.WeatherStateTarget, this.WeatherStateBlend);
		this.WeatherState.Override(this.WeatherOverrides);
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000FD2F8 File Offset: 0x000FB4F8
	private static bool Initialized()
	{
		return SingletonComponent<Climate>.Instance && SingletonComponent<Climate>.Instance.WeatherStatePrevious && SingletonComponent<Climate>.Instance.WeatherStateTarget && SingletonComponent<Climate>.Instance.WeatherStateNext && SingletonComponent<Climate>.Instance.WeatherState && SingletonComponent<Climate>.Instance.WeatherClampsMin && SingletonComponent<Climate>.Instance.WeatherOverrides;
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x000FD386 File Offset: 0x000FB586
	public static float GetClouds(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Clouds.Coverage;
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x000FD3A9 File Offset: 0x000FB5A9
	public static float GetFog(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Atmosphere.Fogginess;
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x000FD3CC File Offset: 0x000FB5CC
	public static float GetWind(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Wind;
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000FD3EC File Offset: 0x000FB5EC
	public static float GetThunder(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float thunder = SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
		if (thunder >= 0f)
		{
			return thunder;
		}
		float thunder2 = SingletonComponent<Climate>.Instance.WeatherState.Thunder;
		float thunder3 = SingletonComponent<Climate>.Instance.WeatherStatePrevious.Thunder;
		float thunder4 = SingletonComponent<Climate>.Instance.WeatherStateTarget.Thunder;
		if (thunder3 > 0f && thunder2 > 0.5f * thunder3)
		{
			return thunder2;
		}
		if (thunder4 > 0f && thunder2 > 0.5f * thunder4)
		{
			return thunder2;
		}
		return 0f;
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000FD480 File Offset: 0x000FB680
	public static float GetRainbow(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance || !instance.IsDay || instance.LerpValue < 1f)
		{
			return 0f;
		}
		if (Climate.GetFog(position) > 0.25f)
		{
			return 0f;
		}
		float num = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 3) : 0f);
		if (num <= 0f)
		{
			return 0f;
		}
		float rainbow = SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
		if (rainbow >= 0f)
		{
			return rainbow * num;
		}
		if (SingletonComponent<Climate>.Instance.WeatherState.Rainbow <= 0f)
		{
			return 0f;
		}
		if (SingletonComponent<Climate>.Instance.WeatherStateTarget.Rainbow > 0f)
		{
			return 0f;
		}
		float rainbow2 = SingletonComponent<Climate>.Instance.WeatherStatePrevious.Rainbow;
		float num2 = SeedRandom.Value(SingletonComponent<Climate>.Instance.WeatherSeedPrevious);
		if (rainbow2 < num2)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000FD584 File Offset: 0x000FB784
	public static float GetAurora(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance || !instance.IsNight || instance.LerpValue > 0f)
		{
			return 0f;
		}
		if (Climate.GetClouds(position) > 0.1f)
		{
			return 0f;
		}
		if (Climate.GetFog(position) > 0.1f)
		{
			return 0f;
		}
		if (!TerrainMeta.BiomeMap)
		{
			return 0f;
		}
		return TerrainMeta.BiomeMap.GetBiome(position, 8);
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000FD60C File Offset: 0x000FB80C
	public static float GetRain(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float num = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 1) : 0f);
		float num2 = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f);
		return SingletonComponent<Climate>.Instance.WeatherState.Rain * Mathf.Lerp(1f, 0.5f, num) * (1f - num2);
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000FD690 File Offset: 0x000FB890
	public static float GetSnow(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float num = (TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f);
		return SingletonComponent<Climate>.Instance.WeatherState.Rain * num;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000FD6DC File Offset: 0x000FB8DC
	public static float GetTemperature(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 15f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance)
		{
			return 15f;
		}
		Climate.ClimateParameters climateParameters;
		Climate.ClimateParameters climateParameters2;
		float num = SingletonComponent<Climate>.Instance.FindBlendParameters(position, out climateParameters, out climateParameters2);
		if (climateParameters == null || climateParameters2 == null)
		{
			return 15f;
		}
		float hour = instance.Cycle.Hour;
		float num2 = climateParameters.Temperature.Evaluate(hour);
		float num3 = climateParameters2.Temperature.Evaluate(hour);
		return Mathf.Lerp(num2, num3, num);
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000FD758 File Offset: 0x000FB958
	private uint GetSeedFromLong(long val)
	{
		uint num = (uint)((val % (long)((ulong)(-1)) + (long)((ulong)(-1))) % (long)((ulong)(-1)));
		SeedRandom.Wanghash(ref num);
		SeedRandom.Wanghash(ref num);
		SeedRandom.Wanghash(ref num);
		return num;
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000FD78C File Offset: 0x000FB98C
	private WeatherPreset GetWeatherPreset(uint seed)
	{
		float num = this.Weather.ClearChance + this.Weather.DustChance + this.Weather.FogChance + this.Weather.OvercastChance + this.Weather.StormChance + this.Weather.RainChance;
		float num2 = SeedRandom.Range(ref seed, 0f, num);
		if (num2 < this.Weather.RainChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Rain);
		}
		if (num2 < this.Weather.RainChance + this.Weather.StormChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Storm);
		}
		if (num2 < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Overcast);
		}
		if (num2 < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance + this.Weather.FogChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Fog);
		}
		if (num2 < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance + this.Weather.FogChance + this.Weather.DustChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Dust);
		}
		return this.GetWeatherPreset(seed, WeatherPresetType.Clear);
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000FD8E4 File Offset: 0x000FBAE4
	private WeatherPreset GetWeatherPreset(uint seed, WeatherPresetType type)
	{
		if (this.presetLookup == null)
		{
			this.presetLookup = new Dictionary<WeatherPresetType, WeatherPreset[]>();
		}
		WeatherPreset[] array;
		if (!this.presetLookup.TryGetValue(type, out array))
		{
			this.presetLookup.Add(type, array = this.CacheWeatherPresets(type));
		}
		return array.GetRandom(ref seed);
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000FD934 File Offset: 0x000FBB34
	private WeatherPreset[] CacheWeatherPresets(WeatherPresetType type)
	{
		return this.WeatherPresets.Where((WeatherPreset x) => x.Type == type).ToArray<WeatherPreset>();
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000FD96C File Offset: 0x000FBB6C
	private float FindBlendParameters(Vector3 pos, out Climate.ClimateParameters src, out Climate.ClimateParameters dst)
	{
		if (this.climateLookup == null)
		{
			this.climateLookup = new Climate.ClimateParameters[] { this.Arid, this.Temperate, this.Tundra, this.Arctic };
		}
		if (TerrainMeta.BiomeMap == null)
		{
			src = this.Temperate;
			dst = this.Temperate;
			return 0.5f;
		}
		int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
		int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
		src = this.climateLookup[TerrainBiome.TypeToIndex(biomeMaxType)];
		dst = this.climateLookup[TerrainBiome.TypeToIndex(biomeMaxType2)];
		return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
	}

	// Token: 0x04002127 RID: 8487
	private const float fadeAngle = 20f;

	// Token: 0x04002128 RID: 8488
	private const float defaultTemp = 15f;

	// Token: 0x04002129 RID: 8489
	private const int weatherDurationHours = 18;

	// Token: 0x0400212A RID: 8490
	private const int weatherFadeHours = 6;

	// Token: 0x0400212B RID: 8491
	[Range(0f, 1f)]
	public float BlendingSpeed = 1f;

	// Token: 0x0400212C RID: 8492
	[Range(1f, 9f)]
	public float FogMultiplier = 5f;

	// Token: 0x0400212D RID: 8493
	public float FogDarknessDistance = 200f;

	// Token: 0x0400212E RID: 8494
	public bool DebugLUTBlending;

	// Token: 0x0400212F RID: 8495
	public Climate.WeatherParameters Weather;

	// Token: 0x04002130 RID: 8496
	public WeatherPreset[] WeatherPresets;

	// Token: 0x04002131 RID: 8497
	public Climate.ClimateParameters Arid;

	// Token: 0x04002132 RID: 8498
	public Climate.ClimateParameters Temperate;

	// Token: 0x04002133 RID: 8499
	public Climate.ClimateParameters Tundra;

	// Token: 0x04002134 RID: 8500
	public Climate.ClimateParameters Arctic;

	// Token: 0x04002141 RID: 8513
	private Dictionary<WeatherPresetType, WeatherPreset[]> presetLookup;

	// Token: 0x04002142 RID: 8514
	private Climate.ClimateParameters[] climateLookup;

	// Token: 0x02000D40 RID: 3392
	[Serializable]
	public class ClimateParameters
	{
		// Token: 0x04004744 RID: 18244
		public AnimationCurve Temperature;

		// Token: 0x04004745 RID: 18245
		[Horizontal(4, -1)]
		public Climate.Float4 AerialDensity;

		// Token: 0x04004746 RID: 18246
		[Horizontal(4, -1)]
		public Climate.Float4 FogDensity;

		// Token: 0x04004747 RID: 18247
		[Horizontal(4, -1)]
		public Climate.Texture2D4 LUT;
	}

	// Token: 0x02000D41 RID: 3393
	[Serializable]
	public class WeatherParameters
	{
		// Token: 0x04004748 RID: 18248
		[Range(0f, 1f)]
		public float ClearChance = 1f;

		// Token: 0x04004749 RID: 18249
		[Range(0f, 1f)]
		public float DustChance;

		// Token: 0x0400474A RID: 18250
		[Range(0f, 1f)]
		public float FogChance;

		// Token: 0x0400474B RID: 18251
		[Range(0f, 1f)]
		public float OvercastChance;

		// Token: 0x0400474C RID: 18252
		[Range(0f, 1f)]
		public float StormChance;

		// Token: 0x0400474D RID: 18253
		[Range(0f, 1f)]
		public float RainChance;
	}

	// Token: 0x02000D42 RID: 3394
	public class Value4<T>
	{
		// Token: 0x060050B9 RID: 20665 RVA: 0x001AA3A8 File Offset: 0x001A85A8
		public float FindBlendParameters(TOD_Sky sky, out T src, out T dst)
		{
			float num = Mathf.Abs(sky.SunriseTime - sky.Cycle.Hour);
			float num2 = Mathf.Abs(sky.SunsetTime - sky.Cycle.Hour);
			float num3 = (180f - sky.SunZenith) / 180f;
			float num4 = 0.11111111f;
			if (num < num2)
			{
				if (num3 < 0.5f)
				{
					src = this.Night;
					dst = this.Dawn;
					return Mathf.InverseLerp(0.5f - num4, 0.5f, num3);
				}
				src = this.Dawn;
				dst = this.Noon;
				return Mathf.InverseLerp(0.5f, 0.5f + num4, num3);
			}
			else
			{
				if (num3 > 0.5f)
				{
					src = this.Noon;
					dst = this.Dusk;
					return Mathf.InverseLerp(0.5f + num4, 0.5f, num3);
				}
				src = this.Dusk;
				dst = this.Night;
				return Mathf.InverseLerp(0.5f, 0.5f - num4, num3);
			}
		}

		// Token: 0x0400474E RID: 18254
		public T Dawn;

		// Token: 0x0400474F RID: 18255
		public T Noon;

		// Token: 0x04004750 RID: 18256
		public T Dusk;

		// Token: 0x04004751 RID: 18257
		public T Night;
	}

	// Token: 0x02000D43 RID: 3395
	[Serializable]
	public class Float4 : Climate.Value4<float>
	{
	}

	// Token: 0x02000D44 RID: 3396
	[Serializable]
	public class Color4 : Climate.Value4<Color>
	{
	}

	// Token: 0x02000D45 RID: 3397
	[Serializable]
	public class Texture2D4 : Climate.Value4<Texture2D>
	{
	}
}
