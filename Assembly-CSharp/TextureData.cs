using System;
using UnityEngine;

// Token: 0x02000661 RID: 1633
public struct TextureData
{
	// Token: 0x06002F67 RID: 12135 RVA: 0x0011EB10 File Offset: 0x0011CD10
	public TextureData(Texture2D tex)
	{
		if (tex != null)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.colors = tex.GetPixels32();
			return;
		}
		this.width = 0;
		this.height = 0;
		this.colors = null;
	}

	// Token: 0x06002F68 RID: 12136 RVA: 0x0011EB60 File Offset: 0x0011CD60
	public Color32 GetColor(int x, int y)
	{
		return this.colors[y * this.width + x];
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x0011EB77 File Offset: 0x0011CD77
	public int GetShort(int x, int y)
	{
		return (int)BitUtility.DecodeShort(this.GetColor(x, y));
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x0011EB86 File Offset: 0x0011CD86
	public int GetInt(int x, int y)
	{
		return BitUtility.DecodeInt(this.GetColor(x, y));
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x0011EB95 File Offset: 0x0011CD95
	public float GetFloat(int x, int y)
	{
		return BitUtility.DecodeFloat(this.GetColor(x, y));
	}

	// Token: 0x06002F6C RID: 12140 RVA: 0x0011EBA4 File Offset: 0x0011CDA4
	public float GetHalf(int x, int y)
	{
		return BitUtility.Short2Float(this.GetShort(x, y));
	}

	// Token: 0x06002F6D RID: 12141 RVA: 0x0011EBB3 File Offset: 0x0011CDB3
	public Vector4 GetVector(int x, int y)
	{
		return BitUtility.DecodeVector(this.GetColor(x, y));
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x0011EBC2 File Offset: 0x0011CDC2
	public Vector3 GetNormal(int x, int y)
	{
		return BitUtility.DecodeNormal(this.GetColor(x, y));
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x0011EBD8 File Offset: 0x0011CDD8
	public Color32 GetInterpolatedColor(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int num5 = Mathf.Min(num3 + 1, this.width - 2);
		int num6 = Mathf.Min(num4 + 1, this.height - 2);
		Color color = this.GetColor(num3, num4);
		Color color2 = this.GetColor(num5, num4);
		Color color3 = this.GetColor(num3, num6);
		Color color4 = this.GetColor(num5, num6);
		float num7 = num - (float)num3;
		float num8 = num2 - (float)num4;
		Color color5 = Color.Lerp(color, color2, num7);
		Color color6 = Color.Lerp(color3, color4, num7);
		return Color.Lerp(color5, color6, num8);
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x0011ECB4 File Offset: 0x0011CEB4
	public int GetInterpolatedInt(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp(Mathf.RoundToInt(num), 1, this.width - 2);
		int num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 1, this.height - 2);
		return this.GetInt(num3, num4);
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x0011ED0C File Offset: 0x0011CF0C
	public int GetInterpolatedShort(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp(Mathf.RoundToInt(num), 1, this.width - 2);
		int num4 = Mathf.Clamp(Mathf.RoundToInt(num2), 1, this.height - 2);
		return this.GetShort(num3, num4);
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x0011ED64 File Offset: 0x0011CF64
	public float GetInterpolatedFloat(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int num5 = Mathf.Min(num3 + 1, this.width - 2);
		int num6 = Mathf.Min(num4 + 1, this.height - 2);
		float @float = this.GetFloat(num3, num4);
		float float2 = this.GetFloat(num5, num4);
		float float3 = this.GetFloat(num3, num6);
		float float4 = this.GetFloat(num5, num6);
		float num7 = num - (float)num3;
		float num8 = num2 - (float)num4;
		float num9 = Mathf.Lerp(@float, float2, num7);
		float num10 = Mathf.Lerp(float3, float4, num7);
		return Mathf.Lerp(num9, num10, num8);
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x0011EE24 File Offset: 0x0011D024
	public float GetInterpolatedHalf(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int num5 = Mathf.Min(num3 + 1, this.width - 2);
		int num6 = Mathf.Min(num4 + 1, this.height - 2);
		float half = this.GetHalf(num3, num4);
		float half2 = this.GetHalf(num5, num4);
		float half3 = this.GetHalf(num3, num6);
		float half4 = this.GetHalf(num5, num6);
		float num7 = num - (float)num3;
		float num8 = num2 - (float)num4;
		float num9 = Mathf.Lerp(half, half2, num7);
		float num10 = Mathf.Lerp(half3, half4, num7);
		return Mathf.Lerp(num9, num10, num8);
	}

	// Token: 0x06002F74 RID: 12148 RVA: 0x0011EEE4 File Offset: 0x0011D0E4
	public Vector4 GetInterpolatedVector(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int num5 = Mathf.Min(num3 + 1, this.width - 2);
		int num6 = Mathf.Min(num4 + 1, this.height - 2);
		Vector4 vector = this.GetVector(num3, num4);
		Vector4 vector2 = this.GetVector(num5, num4);
		Vector4 vector3 = this.GetVector(num3, num6);
		Vector4 vector4 = this.GetVector(num5, num6);
		float num7 = num - (float)num3;
		float num8 = num2 - (float)num4;
		Vector4 vector5 = Vector4.Lerp(vector, vector2, num7);
		Vector4 vector6 = Vector4.Lerp(vector3, vector4, num7);
		return Vector4.Lerp(vector5, vector6, num8);
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x0011EFA4 File Offset: 0x0011D1A4
	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int num5 = Mathf.Min(num3 + 1, this.width - 2);
		int num6 = Mathf.Min(num4 + 1, this.height - 2);
		Vector3 normal = this.GetNormal(num3, num4);
		Vector3 normal2 = this.GetNormal(num5, num4);
		Vector3 normal3 = this.GetNormal(num3, num6);
		Vector3 normal4 = this.GetNormal(num5, num6);
		float num7 = num - (float)num3;
		float num8 = num2 - (float)num4;
		Vector3 vector = Vector3.Lerp(normal, normal2, num7);
		Vector3 vector2 = Vector3.Lerp(normal3, normal4, num7);
		return Vector3.Lerp(vector, vector2, num8);
	}

	// Token: 0x0400271E RID: 10014
	public int width;

	// Token: 0x0400271F RID: 10015
	public int height;

	// Token: 0x04002720 RID: 10016
	public Color32[] colors;
}
