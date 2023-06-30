using System;

// Token: 0x0200065B RID: 1627
public static class Noise
{
	// Token: 0x06002F22 RID: 12066 RVA: 0x0011BF4E File Offset: 0x0011A14E
	public static float Simplex1D(float x)
	{
		return NativeNoise.Simplex1D(x);
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x0011BF56 File Offset: 0x0011A156
	public static float Simplex2D(float x, float y)
	{
		return NativeNoise.Simplex2D(x, y);
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x0011BF5F File Offset: 0x0011A15F
	public static float Turbulence(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Turbulence(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x0011BF70 File Offset: 0x0011A170
	public static float Billow(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Billow(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x0011BF81 File Offset: 0x0011A181
	public static float Ridge(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Ridge(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x0011BF92 File Offset: 0x0011A192
	public static float Sharp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Sharp(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x0011BFA3 File Offset: 0x0011A1A3
	public static float TurbulenceIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.TurbulenceIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x0011BFB4 File Offset: 0x0011A1B4
	public static float BillowIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.BillowIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x0011BFC5 File Offset: 0x0011A1C5
	public static float RidgeIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.RidgeIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x0011BFD6 File Offset: 0x0011A1D6
	public static float SharpIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.SharpIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x0011BFE7 File Offset: 0x0011A1E7
	public static float TurbulenceWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.TurbulenceWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x0011BFFA File Offset: 0x0011A1FA
	public static float BillowWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.BillowWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x0011C00D File Offset: 0x0011A20D
	public static float RidgeWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.RidgeWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x0011C020 File Offset: 0x0011A220
	public static float SharpWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.SharpWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x0011C034 File Offset: 0x0011A234
	public static float Jordan(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 1f, float damp = 1f, float damp_scale = 1f)
	{
		return NativeNoise.Jordan(x, y, octaves, frequency, amplitude, lacunarity, gain, warp, damp, damp_scale);
	}

	// Token: 0x040026DD RID: 9949
	public const float MIN = -1000000f;

	// Token: 0x040026DE RID: 9950
	public const float MAX = 1000000f;
}
