using System;
using UnityEngine;

// Token: 0x02000932 RID: 2354
public class DistanceField
{
	// Token: 0x06003867 RID: 14439 RVA: 0x0014FA48 File Offset: 0x0014DC48
	public static void Generate(in int size, in byte threshold, in byte[] image, ref float[] distanceField)
	{
		int num = size + 2;
		int[] array = new int[num * num];
		int[] array2 = new int[num * num];
		float[] array3 = new float[num * num];
		int i = 0;
		int num2 = 0;
		while (i < num)
		{
			int j = 0;
			while (j < num)
			{
				array[num2] = -1;
				array2[num2] = -1;
				array3[num2] = float.PositiveInfinity;
				j++;
				num2++;
			}
			i++;
		}
		int k = 1;
		int num3 = k * size;
		int num4 = k * num;
		while (k < size - 2)
		{
			int l = 1;
			int num5 = num3 + l;
			int num6 = num4 + l;
			while (l < size - 2)
			{
				int num7 = num6 + num + 1;
				bool flag = image[num5] > threshold;
				if (flag && (image[num5 - 1] > threshold != flag || image[num5 + 1] > threshold != flag || image[num5 - size] > threshold != flag || image[num5 + size] > threshold != flag))
				{
					array[num7] = l + 1;
					array2[num7] = k + 1;
					array3[num7] = 0f;
				}
				l++;
				num5++;
				num6++;
			}
			k++;
			num3 += size;
			num4 += num;
		}
		int m = 1;
		int num8 = m * num;
		while (m < num - 1)
		{
			int n = 1;
			int num9 = num8 + n;
			while (n < num - 1)
			{
				int num10 = num9 - 1;
				int num11 = num9 - num;
				int num12 = num11 - 1;
				int num13 = num11 + 1;
				float num14 = array3[num9];
				if (array3[num12] + 1.4142135f < num14)
				{
					int num15 = (array[num9] = array[num12]);
					int num16 = (array2[num9] = array2[num12]);
					num14 = (array3[num9] = Vector2Ex.Length((float)(n - num15), (float)(m - num16)));
				}
				if (array3[num11] + 1f < num14)
				{
					int num17 = (array[num9] = array[num11]);
					int num18 = (array2[num9] = array2[num11]);
					num14 = (array3[num9] = Vector2Ex.Length((float)(n - num17), (float)(m - num18)));
				}
				if (array3[num13] + 1.4142135f < num14)
				{
					int num19 = (array[num9] = array[num13]);
					int num20 = (array2[num9] = array2[num13]);
					num14 = (array3[num9] = Vector2Ex.Length((float)(n - num19), (float)(m - num20)));
				}
				if (array3[num10] + 1f < num14)
				{
					int num21 = (array[num9] = array[num10]);
					int num22 = (array2[num9] = array2[num10]);
					float num23 = (array3[num9] = Vector2Ex.Length((float)(n - num21), (float)(m - num22)));
				}
				n++;
				num9++;
			}
			m++;
			num8 += num;
		}
		int num24 = num - 2;
		int num25 = num24 * num;
		while (num24 >= 1)
		{
			int num26 = num - 2;
			int num27 = num25 + num26;
			while (num26 >= 1)
			{
				int num28 = num27 + 1;
				int num29 = num27 + num;
				int num30 = num29 - 1;
				int num31 = num29 + 1;
				float num32 = array3[num27];
				if (array3[num28] + 1f < num32)
				{
					int num33 = (array[num27] = array[num28]);
					int num34 = (array2[num27] = array2[num28]);
					num32 = (array3[num27] = Vector2Ex.Length((float)(num26 - num33), (float)(num24 - num34)));
				}
				if (array3[num30] + 1.4142135f < num32)
				{
					int num35 = (array[num27] = array[num30]);
					int num36 = (array2[num27] = array2[num30]);
					num32 = (array3[num27] = Vector2Ex.Length((float)(num26 - num35), (float)(num24 - num36)));
				}
				if (array3[num29] + 1f < num32)
				{
					int num37 = (array[num27] = array[num29]);
					int num38 = (array2[num27] = array2[num29]);
					num32 = (array3[num27] = Vector2Ex.Length((float)(num26 - num37), (float)(num24 - num38)));
				}
				if (array3[num31] + 1f < num32)
				{
					int num39 = (array[num27] = array[num31]);
					int num40 = (array2[num27] = array2[num31]);
					float num23 = (array3[num27] = Vector2Ex.Length((float)(num26 - num39), (float)(num24 - num40)));
				}
				num26--;
				num27--;
			}
			num24--;
			num25 -= num;
		}
		int num41 = 0;
		int num42 = 0;
		int num43 = num;
		while (num41 < size)
		{
			int num44 = 0;
			int num45 = num43 + 1;
			while (num44 < size)
			{
				distanceField[num42] = ((image[num42] > threshold) ? (-array3[num45]) : array3[num45]);
				num44++;
				num42++;
				num45++;
			}
			num41++;
			num43 += num;
		}
	}

	// Token: 0x06003868 RID: 14440 RVA: 0x0014FEEC File Offset: 0x0014E0EC
	private static float SampleClamped(float[] data, int size, int x, int y)
	{
		x = ((x < 0) ? 0 : x);
		y = ((y < 0) ? 0 : y);
		x = ((x >= size) ? (size - 1) : x);
		y = ((y >= size) ? (size - 1) : y);
		return data[y * size + x];
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x0014FF21 File Offset: 0x0014E121
	private static Vector3 SampleClamped(Vector3[] data, int size, int x, int y)
	{
		x = ((x < 0) ? 0 : x);
		y = ((y < 0) ? 0 : y);
		x = ((x >= size) ? (size - 1) : x);
		y = ((y >= size) ? (size - 1) : y);
		return data[y * size + x];
	}

	// Token: 0x0600386A RID: 14442 RVA: 0x0014FF5A File Offset: 0x0014E15A
	private static ushort SampleClamped(ushort[] data, int size, int x, int y)
	{
		x = ((x < 0) ? 0 : x);
		y = ((y < 0) ? 0 : y);
		x = ((x >= size) ? (size - 1) : x);
		y = ((y >= size) ? (size - 1) : y);
		return data[y * size + x];
	}

	// Token: 0x0600386B RID: 14443 RVA: 0x0014FF90 File Offset: 0x0014E190
	public static void GenerateVectors(in int size, in float[] distanceField, ref Vector3[] vectorField)
	{
		for (int i = 1; i < size - 1; i++)
		{
			for (int j = 1; j < size - 1; j++)
			{
				float num = DistanceField.SampleClamped(distanceField, size, i, j);
				float num2 = DistanceField.SampleClamped(distanceField, size, i - 1, j - 1);
				float num3 = DistanceField.SampleClamped(distanceField, size, i - 1, j);
				float num4 = DistanceField.SampleClamped(distanceField, size, i - 1, j + 1);
				float num5 = DistanceField.SampleClamped(distanceField, size, i, j - 1);
				float num6 = DistanceField.SampleClamped(distanceField, size, i, j + 1);
				float num7 = DistanceField.SampleClamped(distanceField, size, i + 1, j - 1);
				float num8 = DistanceField.SampleClamped(distanceField, size, i + 1, j);
				float num9 = DistanceField.SampleClamped(distanceField, size, i + 1, j + 1);
				float num10 = num7 + 2f * num8 + num9 - (num2 + 2f * num3 + num4);
				float num11 = num4 + 2f * num6 + num9 - (num2 + 2f * num5 + num7);
				Vector2 normalized = new Vector2(-num10, -num11).normalized;
				vectorField[j * size + i] = new Vector3(normalized.x, normalized.y, num);
			}
		}
		for (int k = 1; k < size - 1; k++)
		{
			vectorField[k] = DistanceField.SampleClamped(vectorField, size, k, 1);
			vectorField[(size - 1) * size + k] = DistanceField.SampleClamped(vectorField, size, k, size - 2);
		}
		for (int l = 0; l < size; l++)
		{
			vectorField[l * size] = DistanceField.SampleClamped(vectorField, size, 1, l);
			vectorField[l * size + size - 1] = DistanceField.SampleClamped(vectorField, size, size - 2, l);
		}
	}

	// Token: 0x0600386C RID: 14444 RVA: 0x00150154 File Offset: 0x0014E354
	public static void ApplyGaussianBlur(int size, float[] distanceField, int steps = 1)
	{
		if (steps > 0)
		{
			float[] array = new float[size * size];
			int num = size - 1;
			for (int i = 0; i < steps; i++)
			{
				int j = 0;
				int num2 = 0;
				int num3 = 0;
				while (j < size)
				{
					int k = 0;
					while (k < size)
					{
						float num4 = 0f;
						for (int l = 0; l < 7; l++)
						{
							int num5 = k + DistanceField.GaussOffsets[l];
							num5 = ((num5 >= 0) ? num5 : 0);
							num5 = ((num5 <= num) ? num5 : num);
							num4 += distanceField[num3 + num5] * DistanceField.GaussWeights[l];
						}
						array[num2] = num4;
						k++;
						num2++;
					}
					j++;
					num3 += size;
				}
				int m = 0;
				int num6 = 0;
				while (m < size)
				{
					int n = 0;
					while (n < size)
					{
						float num7 = 0f;
						for (int num8 = 0; num8 < 7; num8++)
						{
							int num9 = m + DistanceField.GaussOffsets[num8];
							num9 = ((num9 >= 0) ? num9 : 0);
							num9 = ((num9 <= num) ? num9 : num);
							num7 += array[num9 * size + n] * DistanceField.GaussWeights[num8];
						}
						distanceField[num6] = num7;
						n++;
						num6++;
					}
					m++;
				}
			}
		}
	}

	// Token: 0x0400339B RID: 13211
	private static readonly int[] GaussOffsets = new int[] { -6, -4, -2, 0, 2, 4, 6 };

	// Token: 0x0400339C RID: 13212
	private static readonly float[] GaussWeights = new float[] { 0.03125f, 0.109375f, 0.21875f, 0.28125f, 0.21875f, 0.109375f, 0.03125f };
}
