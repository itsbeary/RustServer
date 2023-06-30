using System;
using UnityEngine;

// Token: 0x02000934 RID: 2356
public class sRGB
{
	// Token: 0x06003879 RID: 14457 RVA: 0x001503A4 File Offset: 0x0014E5A4
	static sRGB()
	{
		for (int i = 0; i < 256; i++)
		{
			sRGB.to_linear[i] = (byte)(sRGB.srgb_to_linear((float)i * 0.003921569f) * 255f + 0.5f);
		}
		for (int j = 0; j < 256; j++)
		{
			sRGB.to_srgb[j] = (byte)(sRGB.linear_to_srgb((float)j * 0.003921569f) * 255f + 0.5f);
		}
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x00150450 File Offset: 0x0014E650
	public static float linear_to_srgb(float linear)
	{
		if (float.IsNaN(linear))
		{
			return 0f;
		}
		if (linear > 1f)
		{
			return 1f;
		}
		if (linear < 0f)
		{
			return 0f;
		}
		if (linear < 0.0031308f)
		{
			return 12.92f * linear;
		}
		return 1.055f * Mathf.Pow(linear, 0.41666f) - 0.055f;
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x001504AE File Offset: 0x0014E6AE
	public static float srgb_to_linear(float srgb)
	{
		if (srgb <= 0.04045f)
		{
			return srgb / 12.92f;
		}
		return Mathf.Pow((srgb + 0.055f) / 1.055f, 2.4f);
	}

	// Token: 0x0400339F RID: 13215
	public static byte[] to_linear = new byte[256];

	// Token: 0x040033A0 RID: 13216
	public static byte[] to_srgb = new byte[256];
}
