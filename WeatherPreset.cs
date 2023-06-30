using System;
using System.Text;
using UnityEngine;

// Token: 0x020005BB RID: 1467
[CreateAssetMenu(menuName = "Rust/WeatherPreset")]
public class WeatherPreset : ScriptableObject
{
	// Token: 0x06002C53 RID: 11347 RVA: 0x0010C35C File Offset: 0x0010A55C
	public void Apply(TOD_Sky sky)
	{
		sky.Atmosphere.RayleighMultiplier = this.Atmosphere.RayleighMultiplier;
		sky.Atmosphere.MieMultiplier = this.Atmosphere.MieMultiplier;
		sky.Atmosphere.Brightness = this.Atmosphere.Brightness;
		sky.Atmosphere.Contrast = this.Atmosphere.Contrast;
		sky.Atmosphere.Directionality = this.Atmosphere.Directionality;
		sky.Atmosphere.Fogginess = this.Atmosphere.Fogginess;
		sky.Clouds.Size = this.Clouds.Size;
		sky.Clouds.Opacity = this.Clouds.Opacity;
		sky.Clouds.Coverage = this.Clouds.Coverage;
		sky.Clouds.Sharpness = this.Clouds.Sharpness;
		sky.Clouds.Coloring = this.Clouds.Coloring;
		sky.Clouds.Attenuation = this.Clouds.Attenuation;
		sky.Clouds.Saturation = this.Clouds.Saturation;
		sky.Clouds.Scattering = this.Clouds.Scattering;
		sky.Clouds.Brightness = this.Clouds.Brightness;
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x0010C4B4 File Offset: 0x0010A6B4
	public void Copy(TOD_Sky sky)
	{
		this.Atmosphere.RayleighMultiplier = sky.Atmosphere.RayleighMultiplier;
		this.Atmosphere.MieMultiplier = sky.Atmosphere.MieMultiplier;
		this.Atmosphere.Brightness = sky.Atmosphere.Brightness;
		this.Atmosphere.Contrast = sky.Atmosphere.Contrast;
		this.Atmosphere.Directionality = sky.Atmosphere.Directionality;
		this.Atmosphere.Fogginess = sky.Atmosphere.Fogginess;
		this.Clouds.Size = sky.Clouds.Size;
		this.Clouds.Opacity = sky.Clouds.Opacity;
		this.Clouds.Coverage = sky.Clouds.Coverage;
		this.Clouds.Sharpness = sky.Clouds.Sharpness;
		this.Clouds.Coloring = sky.Clouds.Coloring;
		this.Clouds.Attenuation = sky.Clouds.Attenuation;
		this.Clouds.Saturation = sky.Clouds.Saturation;
		this.Clouds.Scattering = sky.Clouds.Scattering;
		this.Clouds.Brightness = sky.Clouds.Brightness;
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x0010C60C File Offset: 0x0010A80C
	public void Reset()
	{
		this.Wind = -1f;
		this.Rain = -1f;
		this.Thunder = -1f;
		this.Rainbow = -1f;
		this.Atmosphere = new TOD_AtmosphereParameters();
		this.Atmosphere.RayleighMultiplier = -1f;
		this.Atmosphere.MieMultiplier = -1f;
		this.Atmosphere.Brightness = -1f;
		this.Atmosphere.Contrast = -1f;
		this.Atmosphere.Directionality = -1f;
		this.Atmosphere.Fogginess = -1f;
		this.Clouds = new TOD_CloudParameters();
		this.Clouds.Size = -1f;
		this.Clouds.Opacity = -1f;
		this.Clouds.Coverage = -1f;
		this.Clouds.Sharpness = -1f;
		this.Clouds.Coloring = -1f;
		this.Clouds.Attenuation = -1f;
		this.Clouds.Saturation = -1f;
		this.Clouds.Scattering = -1f;
		this.Clouds.Brightness = -1f;
		this.OceanScale = -1f;
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x0010C758 File Offset: 0x0010A958
	public void Set(WeatherPreset other)
	{
		this.Wind = other.Wind;
		this.Rain = other.Rain;
		this.Thunder = other.Thunder;
		this.Rainbow = other.Rainbow;
		this.Atmosphere.RayleighMultiplier = other.Atmosphere.RayleighMultiplier;
		this.Atmosphere.MieMultiplier = other.Atmosphere.MieMultiplier;
		this.Atmosphere.Brightness = other.Atmosphere.Brightness;
		this.Atmosphere.Contrast = other.Atmosphere.Contrast;
		this.Atmosphere.Directionality = other.Atmosphere.Directionality;
		this.Atmosphere.Fogginess = other.Atmosphere.Fogginess;
		this.Clouds.Size = other.Clouds.Size;
		this.Clouds.Opacity = other.Clouds.Opacity;
		this.Clouds.Coverage = other.Clouds.Coverage;
		this.Clouds.Sharpness = other.Clouds.Sharpness;
		this.Clouds.Coloring = other.Clouds.Coloring;
		this.Clouds.Attenuation = other.Clouds.Attenuation;
		this.Clouds.Saturation = other.Clouds.Saturation;
		this.Clouds.Scattering = other.Clouds.Scattering;
		this.Clouds.Brightness = other.Clouds.Brightness;
		this.OceanScale = other.OceanScale;
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x0010C8EC File Offset: 0x0010AAEC
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Wind {0}", this.Wind));
		stringBuilder.AppendLine(string.Format("Rain {0}", this.Rain));
		stringBuilder.AppendLine(string.Format("Thunder {0}", this.Thunder));
		stringBuilder.AppendLine(string.Format("Rainbow {0}", this.Rainbow));
		stringBuilder.AppendLine(string.Format("RayleighMultiplier {0}", this.Atmosphere.RayleighMultiplier));
		stringBuilder.AppendLine(string.Format("MieMultiplier {0}", this.Atmosphere.MieMultiplier));
		stringBuilder.AppendLine(string.Format("Brightness {0}", this.Atmosphere.Brightness));
		stringBuilder.AppendLine(string.Format("Contrast {0}", this.Atmosphere.Contrast));
		stringBuilder.AppendLine(string.Format("Directionality {0}", this.Atmosphere.Directionality));
		stringBuilder.AppendLine(string.Format("Fogginess {0}", this.Atmosphere.Fogginess));
		stringBuilder.AppendLine(string.Format("Size {0}", this.Clouds.Size));
		stringBuilder.AppendLine(string.Format("Opacity {0}", this.Clouds.Opacity));
		stringBuilder.AppendLine(string.Format("Coverage {0}", this.Clouds.Coverage));
		stringBuilder.AppendLine(string.Format("Sharpness {0}", this.Clouds.Sharpness));
		stringBuilder.AppendLine(string.Format("Coloring {0}", this.Clouds.Coloring));
		stringBuilder.AppendLine(string.Format("Attenuation {0}", this.Clouds.Attenuation));
		stringBuilder.AppendLine(string.Format("Saturation {0}", this.Clouds.Saturation));
		stringBuilder.AppendLine(string.Format("Scattering {0}", this.Clouds.Scattering));
		stringBuilder.AppendLine(string.Format("Brightness {0}", this.Clouds.Brightness));
		stringBuilder.AppendLine(string.Format("Ocean {0}", this.OceanScale));
		return stringBuilder.ToString();
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x0010CB80 File Offset: 0x0010AD80
	public void Fade(WeatherPreset a, WeatherPreset b, float t)
	{
		this.Fade(ref this.Wind, a.Wind, b.Wind, t);
		this.Fade(ref this.Rain, a.Rain, b.Rain, t);
		this.Fade(ref this.Thunder, a.Thunder, b.Thunder, t);
		this.Fade(ref this.Rainbow, a.Rainbow, b.Rainbow, t);
		this.Fade(ref this.Atmosphere.RayleighMultiplier, a.Atmosphere.RayleighMultiplier, b.Atmosphere.RayleighMultiplier, t);
		this.Fade(ref this.Atmosphere.MieMultiplier, a.Atmosphere.MieMultiplier, b.Atmosphere.MieMultiplier, t);
		this.Fade(ref this.Atmosphere.Brightness, a.Atmosphere.Brightness, b.Atmosphere.Brightness, t);
		this.Fade(ref this.Atmosphere.Contrast, a.Atmosphere.Contrast, b.Atmosphere.Contrast, t);
		this.Fade(ref this.Atmosphere.Directionality, a.Atmosphere.Directionality, b.Atmosphere.Directionality, t);
		this.Fade(ref this.Atmosphere.Fogginess, a.Atmosphere.Fogginess, b.Atmosphere.Fogginess, t);
		this.Fade(ref this.Clouds.Size, a.Clouds.Size, b.Clouds.Size, t);
		this.Fade(ref this.Clouds.Opacity, a.Clouds.Opacity, b.Clouds.Opacity, t);
		this.Fade(ref this.Clouds.Coverage, a.Clouds.Coverage, b.Clouds.Coverage, t);
		this.Fade(ref this.Clouds.Sharpness, a.Clouds.Sharpness, b.Clouds.Sharpness, t);
		this.Fade(ref this.Clouds.Coloring, a.Clouds.Coloring, b.Clouds.Coloring, t);
		this.Fade(ref this.Clouds.Attenuation, a.Clouds.Attenuation, b.Clouds.Attenuation, t);
		this.Fade(ref this.Clouds.Saturation, a.Clouds.Saturation, b.Clouds.Saturation, t);
		this.Fade(ref this.Clouds.Scattering, a.Clouds.Scattering, b.Clouds.Scattering, t);
		this.Fade(ref this.Clouds.Brightness, a.Clouds.Brightness, b.Clouds.Brightness, t);
		this.Fade(ref this.OceanScale, a.OceanScale, b.OceanScale, t);
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x0010CE64 File Offset: 0x0010B064
	public void Override(WeatherPreset other)
	{
		this.Override(ref this.Wind, other.Wind);
		this.Override(ref this.Rain, other.Rain);
		this.Override(ref this.Thunder, other.Thunder);
		this.Override(ref this.Rainbow, other.Rainbow);
		this.Override(ref this.Atmosphere.RayleighMultiplier, other.Atmosphere.RayleighMultiplier);
		this.Override(ref this.Atmosphere.MieMultiplier, other.Atmosphere.MieMultiplier);
		this.Override(ref this.Atmosphere.Brightness, other.Atmosphere.Brightness);
		this.Override(ref this.Atmosphere.Contrast, other.Atmosphere.Contrast);
		this.Override(ref this.Atmosphere.Directionality, other.Atmosphere.Directionality);
		this.Override(ref this.Atmosphere.Fogginess, other.Atmosphere.Fogginess);
		this.Override(ref this.Clouds.Size, other.Clouds.Size);
		this.Override(ref this.Clouds.Opacity, other.Clouds.Opacity);
		this.Override(ref this.Clouds.Coverage, other.Clouds.Coverage);
		this.Override(ref this.Clouds.Sharpness, other.Clouds.Sharpness);
		this.Override(ref this.Clouds.Coloring, other.Clouds.Coloring);
		this.Override(ref this.Clouds.Attenuation, other.Clouds.Attenuation);
		this.Override(ref this.Clouds.Saturation, other.Clouds.Saturation);
		this.Override(ref this.Clouds.Scattering, other.Clouds.Scattering);
		this.Override(ref this.Clouds.Brightness, other.Clouds.Brightness);
		this.Override(ref this.OceanScale, other.OceanScale);
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x0010D070 File Offset: 0x0010B270
	public void Max(WeatherPreset other)
	{
		this.Max(ref this.Wind, other.Wind);
		this.Max(ref this.Rain, other.Rain);
		this.Max(ref this.Thunder, other.Thunder);
		this.Max(ref this.Rainbow, other.Rainbow);
		this.Max(ref this.Atmosphere.RayleighMultiplier, other.Atmosphere.RayleighMultiplier);
		this.Max(ref this.Atmosphere.MieMultiplier, other.Atmosphere.MieMultiplier);
		this.Max(ref this.Atmosphere.Brightness, other.Atmosphere.Brightness);
		this.Max(ref this.Atmosphere.Contrast, other.Atmosphere.Contrast);
		this.Max(ref this.Atmosphere.Directionality, other.Atmosphere.Directionality);
		this.Max(ref this.Atmosphere.Fogginess, other.Atmosphere.Fogginess);
		this.Max(ref this.Clouds.Size, other.Clouds.Size);
		this.Max(ref this.Clouds.Opacity, other.Clouds.Opacity);
		this.Max(ref this.Clouds.Coverage, other.Clouds.Coverage);
		this.Max(ref this.Clouds.Sharpness, other.Clouds.Sharpness);
		this.Max(ref this.Clouds.Coloring, other.Clouds.Coloring);
		this.Max(ref this.Clouds.Attenuation, other.Clouds.Attenuation);
		this.Max(ref this.Clouds.Saturation, other.Clouds.Saturation);
		this.Max(ref this.Clouds.Scattering, other.Clouds.Scattering);
		this.Max(ref this.Clouds.Brightness, other.Clouds.Brightness);
		this.Max(ref this.OceanScale, other.OceanScale);
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x0010D27C File Offset: 0x0010B47C
	public void Min(WeatherPreset other)
	{
		this.Min(ref this.Wind, other.Wind);
		this.Min(ref this.Rain, other.Rain);
		this.Min(ref this.Thunder, other.Thunder);
		this.Min(ref this.Rainbow, other.Rainbow);
		this.Min(ref this.Atmosphere.RayleighMultiplier, other.Atmosphere.RayleighMultiplier);
		this.Min(ref this.Atmosphere.MieMultiplier, other.Atmosphere.MieMultiplier);
		this.Min(ref this.Atmosphere.Brightness, other.Atmosphere.Brightness);
		this.Min(ref this.Atmosphere.Contrast, other.Atmosphere.Contrast);
		this.Min(ref this.Atmosphere.Directionality, other.Atmosphere.Directionality);
		this.Min(ref this.Atmosphere.Fogginess, other.Atmosphere.Fogginess);
		this.Min(ref this.Clouds.Size, other.Clouds.Size);
		this.Min(ref this.Clouds.Opacity, other.Clouds.Opacity);
		this.Min(ref this.Clouds.Coverage, other.Clouds.Coverage);
		this.Min(ref this.Clouds.Sharpness, other.Clouds.Sharpness);
		this.Min(ref this.Clouds.Coloring, other.Clouds.Coloring);
		this.Min(ref this.Clouds.Attenuation, other.Clouds.Attenuation);
		this.Min(ref this.Clouds.Saturation, other.Clouds.Saturation);
		this.Min(ref this.Clouds.Scattering, other.Clouds.Scattering);
		this.Min(ref this.Clouds.Brightness, other.Clouds.Brightness);
		this.Min(ref this.OceanScale, other.OceanScale);
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x0010D487 File Offset: 0x0010B687
	private void Fade(ref float x, float a, float b, float t)
	{
		x = Mathf.SmoothStep(a, b, t);
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x0010D494 File Offset: 0x0010B694
	private void Override(ref float x, float other)
	{
		if (other >= 0f)
		{
			x = other;
		}
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x0010D4A1 File Offset: 0x0010B6A1
	private void Max(ref float x, float other)
	{
		x = Mathf.Max(x, other);
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x0010D4AD File Offset: 0x0010B6AD
	private void Min(ref float x, float other)
	{
		if (other >= 0f)
		{
			x = Mathf.Min(x, other);
		}
	}

	// Token: 0x04002417 RID: 9239
	public WeatherPresetType Type;

	// Token: 0x04002418 RID: 9240
	public float Wind;

	// Token: 0x04002419 RID: 9241
	public float Rain;

	// Token: 0x0400241A RID: 9242
	public float Thunder;

	// Token: 0x0400241B RID: 9243
	public float Rainbow;

	// Token: 0x0400241C RID: 9244
	public TOD_AtmosphereParameters Atmosphere;

	// Token: 0x0400241D RID: 9245
	public TOD_CloudParameters Clouds;

	// Token: 0x0400241E RID: 9246
	[Range(0f, 10f)]
	public float OceanScale;
}
