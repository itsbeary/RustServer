using System;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public class LegacyWeatherState
{
	// Token: 0x06002C61 RID: 11361 RVA: 0x0010D4C1 File Offset: 0x0010B6C1
	public LegacyWeatherState(WeatherPreset preset)
	{
		this.preset = preset;
	}

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002C62 RID: 11362 RVA: 0x0010D4D0 File Offset: 0x0010B6D0
	// (set) Token: 0x06002C63 RID: 11363 RVA: 0x0010D4DD File Offset: 0x0010B6DD
	public float Wind
	{
		get
		{
			return this.preset.Wind;
		}
		set
		{
			this.preset.Wind = value;
		}
	}

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002C64 RID: 11364 RVA: 0x0010D4EB File Offset: 0x0010B6EB
	// (set) Token: 0x06002C65 RID: 11365 RVA: 0x0010D4F8 File Offset: 0x0010B6F8
	public float Rain
	{
		get
		{
			return this.preset.Rain;
		}
		set
		{
			this.preset.Rain = value;
		}
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x06002C66 RID: 11366 RVA: 0x0010D506 File Offset: 0x0010B706
	// (set) Token: 0x06002C67 RID: 11367 RVA: 0x0010D518 File Offset: 0x0010B718
	public float Clouds
	{
		get
		{
			return this.preset.Clouds.Coverage;
		}
		set
		{
			this.preset.Clouds.Opacity = Mathf.Sign(value);
			this.preset.Clouds.Coverage = value;
		}
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x06002C68 RID: 11368 RVA: 0x0010D541 File Offset: 0x0010B741
	// (set) Token: 0x06002C69 RID: 11369 RVA: 0x0010D553 File Offset: 0x0010B753
	public float Fog
	{
		get
		{
			return this.preset.Atmosphere.Fogginess;
		}
		set
		{
			this.preset.Atmosphere.Fogginess = value;
		}
	}

	// Token: 0x0400241F RID: 9247
	private WeatherPreset preset;
}
