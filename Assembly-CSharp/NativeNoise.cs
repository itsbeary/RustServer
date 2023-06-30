using System;
using System.Runtime.InteropServices;
using System.Security;

// Token: 0x0200065A RID: 1626
[SuppressUnmanagedCodeSecurity]
public static class NativeNoise
{
	// Token: 0x06002F11 RID: 12049
	[DllImport("RustNative", EntryPoint = "snoise1_32")]
	public static extern float Simplex1D(float x);

	// Token: 0x06002F12 RID: 12050
	[DllImport("RustNative", EntryPoint = "sdnoise1_32")]
	public static extern float Simplex1D(float x, out float dx);

	// Token: 0x06002F13 RID: 12051
	[DllImport("RustNative", EntryPoint = "snoise2_32")]
	public static extern float Simplex2D(float x, float y);

	// Token: 0x06002F14 RID: 12052
	[DllImport("RustNative", EntryPoint = "sdnoise2_32")]
	public static extern float Simplex2D(float x, float y, out float dx, out float dy);

	// Token: 0x06002F15 RID: 12053
	[DllImport("RustNative", EntryPoint = "turbulence_32")]
	public static extern float Turbulence(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F16 RID: 12054
	[DllImport("RustNative", EntryPoint = "billow_32")]
	public static extern float Billow(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F17 RID: 12055
	[DllImport("RustNative", EntryPoint = "ridge_32")]
	public static extern float Ridge(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F18 RID: 12056
	[DllImport("RustNative", EntryPoint = "sharp_32")]
	public static extern float Sharp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F19 RID: 12057
	[DllImport("RustNative", EntryPoint = "turbulence_iq_32")]
	public static extern float TurbulenceIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F1A RID: 12058
	[DllImport("RustNative", EntryPoint = "billow_iq_32")]
	public static extern float BillowIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F1B RID: 12059
	[DllImport("RustNative", EntryPoint = "ridge_iq_32")]
	public static extern float RidgeIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F1C RID: 12060
	[DllImport("RustNative", EntryPoint = "sharp_iq_32")]
	public static extern float SharpIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002F1D RID: 12061
	[DllImport("RustNative", EntryPoint = "turbulence_warp_32")]
	public static extern float TurbulenceWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002F1E RID: 12062
	[DllImport("RustNative", EntryPoint = "billow_warp_32")]
	public static extern float BillowWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002F1F RID: 12063
	[DllImport("RustNative", EntryPoint = "ridge_warp_32")]
	public static extern float RidgeWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002F20 RID: 12064
	[DllImport("RustNative", EntryPoint = "sharp_warp_32")]
	public static extern float SharpWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002F21 RID: 12065
	[DllImport("RustNative", EntryPoint = "jordan_32")]
	public static extern float Jordan(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp, float damp, float damp_scale);
}
