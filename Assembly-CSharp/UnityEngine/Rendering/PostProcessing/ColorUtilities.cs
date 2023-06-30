using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9E RID: 2718
	public static class ColorUtilities
	{
		// Token: 0x0600409C RID: 16540 RVA: 0x0017C530 File Offset: 0x0017A730
		public static float StandardIlluminantY(float x)
		{
			return 2.87f * x - 3f * x * x - 0.27509508f;
		}

		// Token: 0x0600409D RID: 16541 RVA: 0x0017C54C File Offset: 0x0017A74C
		public static Vector3 CIExyToLMS(float x, float y)
		{
			float num = 1f;
			float num2 = num * x / y;
			float num3 = num * (1f - x - y) / y;
			float num4 = 0.7328f * num2 + 0.4296f * num - 0.1624f * num3;
			float num5 = -0.7036f * num2 + 1.6975f * num + 0.0061f * num3;
			float num6 = 0.003f * num2 + 0.0136f * num + 0.9834f * num3;
			return new Vector3(num4, num5, num6);
		}

		// Token: 0x0600409E RID: 16542 RVA: 0x0017C5C4 File Offset: 0x0017A7C4
		public static Vector3 ComputeColorBalance(float temperature, float tint)
		{
			float num = temperature / 60f;
			float num2 = tint / 60f;
			float num3 = 0.31271f - num * ((num < 0f) ? 0.1f : 0.05f);
			float num4 = ColorUtilities.StandardIlluminantY(num3) + num2 * 0.05f;
			Vector3 vector = new Vector3(0.949237f, 1.03542f, 1.08728f);
			Vector3 vector2 = ColorUtilities.CIExyToLMS(num3, num4);
			return new Vector3(vector.x / vector2.x, vector.y / vector2.y, vector.z / vector2.z);
		}

		// Token: 0x0600409F RID: 16543 RVA: 0x0017C65C File Offset: 0x0017A85C
		public static Vector3 ColorToLift(Vector4 color)
		{
			Vector3 vector = new Vector3(color.x, color.y, color.z);
			float num = vector.x * 0.2126f + vector.y * 0.7152f + vector.z * 0.0722f;
			vector = new Vector3(vector.x - num, vector.y - num, vector.z - num);
			float w = color.w;
			return new Vector3(vector.x + w, vector.y + w, vector.z + w);
		}

		// Token: 0x060040A0 RID: 16544 RVA: 0x0017C6EC File Offset: 0x0017A8EC
		public static Vector3 ColorToInverseGamma(Vector4 color)
		{
			Vector3 vector = new Vector3(color.x, color.y, color.z);
			float num = vector.x * 0.2126f + vector.y * 0.7152f + vector.z * 0.0722f;
			vector = new Vector3(vector.x - num, vector.y - num, vector.z - num);
			float num2 = color.w + 1f;
			return new Vector3(1f / Mathf.Max(vector.x + num2, 0.001f), 1f / Mathf.Max(vector.y + num2, 0.001f), 1f / Mathf.Max(vector.z + num2, 0.001f));
		}

		// Token: 0x060040A1 RID: 16545 RVA: 0x0017C7B4 File Offset: 0x0017A9B4
		public static Vector3 ColorToGain(Vector4 color)
		{
			Vector3 vector = new Vector3(color.x, color.y, color.z);
			float num = vector.x * 0.2126f + vector.y * 0.7152f + vector.z * 0.0722f;
			vector = new Vector3(vector.x - num, vector.y - num, vector.z - num);
			float num2 = color.w + 1f;
			return new Vector3(vector.x + num2, vector.y + num2, vector.z + num2);
		}

		// Token: 0x060040A2 RID: 16546 RVA: 0x0017C84A File Offset: 0x0017AA4A
		public static float LogCToLinear(float x)
		{
			if (x <= 0.1530537f)
			{
				return (x - 0.092819f) / 5.301883f;
			}
			return (Mathf.Pow(10f, (x - 0.386036f) / 0.244161f) - 0.047996f) / 5.555556f;
		}

		// Token: 0x060040A3 RID: 16547 RVA: 0x0017C885 File Offset: 0x0017AA85
		public static float LinearToLogC(float x)
		{
			if (x <= 0.011361f)
			{
				return 5.301883f * x + 0.092819f;
			}
			return 0.244161f * Mathf.Log10(5.555556f * x + 0.047996f) + 0.386036f;
		}

		// Token: 0x060040A4 RID: 16548 RVA: 0x0017C8BC File Offset: 0x0017AABC
		public static uint ToHex(Color c)
		{
			return ((uint)(c.a * 255f) << 24) | ((uint)(c.r * 255f) << 16) | ((uint)(c.g * 255f) << 8) | (uint)(c.b * 255f);
		}

		// Token: 0x060040A5 RID: 16549 RVA: 0x0017C908 File Offset: 0x0017AB08
		public static Color ToRGBA(uint hex)
		{
			return new Color(((hex >> 16) & 255U) / 255f, ((hex >> 8) & 255U) / 255f, (hex & 255U) / 255f, ((hex >> 24) & 255U) / 255f);
		}

		// Token: 0x04003A01 RID: 14849
		private const float logC_cut = 0.011361f;

		// Token: 0x04003A02 RID: 14850
		private const float logC_a = 5.555556f;

		// Token: 0x04003A03 RID: 14851
		private const float logC_b = 0.047996f;

		// Token: 0x04003A04 RID: 14852
		private const float logC_c = 0.244161f;

		// Token: 0x04003A05 RID: 14853
		private const float logC_d = 0.386036f;

		// Token: 0x04003A06 RID: 14854
		private const float logC_e = 5.301883f;

		// Token: 0x04003A07 RID: 14855
		private const float logC_f = 0.092819f;
	}
}
