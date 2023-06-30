using System;
using UnityEngine;

// Token: 0x02000720 RID: 1824
public class SeparableSSS
{
	// Token: 0x0600330A RID: 13066 RVA: 0x00139304 File Offset: 0x00137504
	private static Vector3 Gaussian(float variance, float r, Color falloffColor)
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			float num = r / (0.001f + falloffColor[i]);
			zero[i] = Mathf.Exp(-(num * num) / (2f * variance)) / (6.28f * variance);
		}
		return zero;
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x00139358 File Offset: 0x00137558
	private static Vector3 Profile(float r, Color falloffColor)
	{
		return 0.1f * SeparableSSS.Gaussian(0.0484f, r, falloffColor) + 0.118f * SeparableSSS.Gaussian(0.187f, r, falloffColor) + 0.113f * SeparableSSS.Gaussian(0.567f, r, falloffColor) + 0.358f * SeparableSSS.Gaussian(1.99f, r, falloffColor) + 0.078f * SeparableSSS.Gaussian(7.41f, r, falloffColor);
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x001393E8 File Offset: 0x001375E8
	public static void CalculateKernel(Color[] target, int targetStart, int targetSize, Color subsurfaceColor, Color falloffColor)
	{
		int num = targetSize * 2 - 1;
		float num2 = ((num > 20) ? 3f : 2f);
		float num3 = 2f;
		Color[] array = new Color[num];
		float num4 = 2f * num2 / (float)(num - 1);
		for (int i = 0; i < num; i++)
		{
			float num5 = -num2 + (float)i * num4;
			float num6 = ((num5 < 0f) ? (-1f) : 1f);
			array[i].a = num2 * num6 * Mathf.Abs(Mathf.Pow(num5, num3)) / Mathf.Pow(num2, num3);
		}
		for (int j = 0; j < num; j++)
		{
			float num7 = ((j > 0) ? Mathf.Abs(array[j].a - array[j - 1].a) : 0f);
			float num8 = ((j < num - 1) ? Mathf.Abs(array[j].a - array[j + 1].a) : 0f);
			Vector3 vector = (num7 + num8) / 2f * SeparableSSS.Profile(array[j].a, falloffColor);
			array[j].r = vector.x;
			array[j].g = vector.y;
			array[j].b = vector.z;
		}
		Color color = array[num / 2];
		for (int k = num / 2; k > 0; k--)
		{
			array[k] = array[k - 1];
		}
		array[0] = color;
		Vector3 zero = Vector3.zero;
		for (int l = 0; l < num; l++)
		{
			zero.x += array[l].r;
			zero.y += array[l].g;
			zero.z += array[l].b;
		}
		for (int m = 0; m < num; m++)
		{
			Color[] array2 = array;
			int num9 = m;
			array2[num9].r = array2[num9].r / zero.x;
			Color[] array3 = array;
			int num10 = m;
			array3[num10].g = array3[num10].g / zero.y;
			Color[] array4 = array;
			int num11 = m;
			array4[num11].b = array4[num11].b / zero.z;
		}
		target[targetStart] = array[0];
		uint num12 = 0U;
		while ((ulong)num12 < (ulong)((long)(targetSize - 1)))
		{
			checked
			{
				target[(int)((IntPtr)(unchecked((long)targetStart + (long)((ulong)num12) + 1L)))] = array[(int)((IntPtr)(unchecked((long)targetSize + (long)((ulong)num12))))];
			}
			num12 += 1U;
		}
	}
}
