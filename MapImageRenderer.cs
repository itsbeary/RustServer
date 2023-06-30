using System;
using UnityEngine;

// Token: 0x0200075A RID: 1882
public static class MapImageRenderer
{
	// Token: 0x06003473 RID: 13427 RVA: 0x00143FE0 File Offset: 0x001421E0
	public unsafe static byte[] Render(out int imageWidth, out int imageHeight, out Color background, float scale = 0.5f, bool lossy = true)
	{
		imageWidth = 0;
		imageHeight = 0;
		background = MapImageRenderer.OffShoreColor;
		TerrainTexturing instance = TerrainTexturing.Instance;
		if (instance == null)
		{
			return null;
		}
		UnityEngine.Object component = instance.GetComponent<Terrain>();
		TerrainMeta component2 = instance.GetComponent<TerrainMeta>();
		TerrainHeightMap terrainHeightMap = instance.GetComponent<TerrainHeightMap>();
		TerrainSplatMap terrainSplatMap = instance.GetComponent<TerrainSplatMap>();
		if (component == null || component2 == null || terrainHeightMap == null || terrainSplatMap == null)
		{
			return null;
		}
		int mapRes = (int)(World.Size * Mathf.Clamp(scale, 0.1f, 4f));
		float invMapRes = 1f / (float)mapRes;
		if (mapRes <= 0)
		{
			return null;
		}
		imageWidth = mapRes + 1000;
		imageHeight = mapRes + 1000;
		Color[] array = new Color[imageWidth * imageHeight];
		MapImageRenderer.Array2D<Color> output = new MapImageRenderer.Array2D<Color>(array, imageWidth, imageHeight);
		Parallel.For(0, imageHeight, delegate(int y)
		{
			y -= 500;
			float num = (float)y * invMapRes;
			int num2 = mapRes + 500;
			for (int i = -500; i < num2; i++)
			{
				float num3 = (float)i * invMapRes;
				Vector3 vector = MapImageRenderer.StartColor;
				float num4 = base.<Render>g__GetHeight|0(num3, num);
				float num5 = Math.Max(Vector3.Dot(base.<Render>g__GetNormal|1(num3, num), MapImageRenderer.SunDirection), 0f);
				vector = Vector3.Lerp(vector, MapImageRenderer.GravelColor, base.<Render>g__GetSplat|2(num3, num, 128) * MapImageRenderer.GravelColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.PebbleColor, base.<Render>g__GetSplat|2(num3, num, 64) * MapImageRenderer.PebbleColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.RockColor, base.<Render>g__GetSplat|2(num3, num, 8) * MapImageRenderer.RockColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.DirtColor, base.<Render>g__GetSplat|2(num3, num, 1) * MapImageRenderer.DirtColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.GrassColor, base.<Render>g__GetSplat|2(num3, num, 16) * MapImageRenderer.GrassColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.ForestColor, base.<Render>g__GetSplat|2(num3, num, 32) * MapImageRenderer.ForestColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.SandColor, base.<Render>g__GetSplat|2(num3, num, 4) * MapImageRenderer.SandColor.w);
				vector = Vector3.Lerp(vector, MapImageRenderer.SnowColor, base.<Render>g__GetSplat|2(num3, num, 2) * MapImageRenderer.SnowColor.w);
				float num6 = 0f - num4;
				if (num6 > 0f)
				{
					vector = Vector3.Lerp(vector, MapImageRenderer.WaterColor, Mathf.Clamp(0.5f + num6 / 5f, 0f, 1f));
					vector = Vector3.Lerp(vector, MapImageRenderer.OffShoreColor, Mathf.Clamp(num6 / 50f, 0f, 1f));
					num5 = 0.5f;
				}
				vector += (num5 - 0.5f) * 0.65f * vector;
				vector = (vector - MapImageRenderer.Half) * 0.94f + MapImageRenderer.Half;
				vector *= 1.05f;
				*output[i + 500, y + 500] = new Color(vector.x, vector.y, vector.z);
			}
		});
		background = *output[0, 0];
		return MapImageRenderer.EncodeToFile(imageWidth, imageHeight, array, lossy);
	}

	// Token: 0x06003474 RID: 13428 RVA: 0x0014411C File Offset: 0x0014231C
	private static byte[] EncodeToFile(int width, int height, Color[] pixels, bool lossy)
	{
		Texture2D texture2D = null;
		byte[] array;
		try
		{
			texture2D = new Texture2D(width, height);
			texture2D.SetPixels(pixels);
			texture2D.Apply();
			array = (lossy ? texture2D.EncodeToJPG(85) : texture2D.EncodeToPNG());
		}
		finally
		{
			if (texture2D != null)
			{
				UnityEngine.Object.Destroy(texture2D);
			}
		}
		return array;
	}

	// Token: 0x06003475 RID: 13429 RVA: 0x00144178 File Offset: 0x00142378
	private static Vector3 UnpackNormal(Vector4 value)
	{
		value.x *= value.w;
		Vector3 vector = default(Vector3);
		vector.x = value.x * 2f - 1f;
		vector.y = value.y * 2f - 1f;
		Vector2 vector2 = new Vector2(vector.x, vector.y);
		vector.z = Mathf.Sqrt(1f - Mathf.Clamp(Vector2.Dot(vector2, vector2), 0f, 1f));
		return vector;
	}

	// Token: 0x04002AE0 RID: 10976
	private static readonly Vector3 StartColor = new Vector3(0.28627452f, 0.27058825f, 0.24705884f);

	// Token: 0x04002AE1 RID: 10977
	private static readonly Vector4 WaterColor = new Vector4(0.16941601f, 0.31755757f, 0.36200002f, 1f);

	// Token: 0x04002AE2 RID: 10978
	private static readonly Vector4 GravelColor = new Vector4(0.25f, 0.24342105f, 0.22039475f, 1f);

	// Token: 0x04002AE3 RID: 10979
	private static readonly Vector4 DirtColor = new Vector4(0.6f, 0.47959462f, 0.33f, 1f);

	// Token: 0x04002AE4 RID: 10980
	private static readonly Vector4 SandColor = new Vector4(0.7f, 0.65968585f, 0.5277487f, 1f);

	// Token: 0x04002AE5 RID: 10981
	private static readonly Vector4 GrassColor = new Vector4(0.35486364f, 0.37f, 0.2035f, 1f);

	// Token: 0x04002AE6 RID: 10982
	private static readonly Vector4 ForestColor = new Vector4(0.24843751f, 0.3f, 0.0703125f, 1f);

	// Token: 0x04002AE7 RID: 10983
	private static readonly Vector4 RockColor = new Vector4(0.4f, 0.39379844f, 0.37519377f, 1f);

	// Token: 0x04002AE8 RID: 10984
	private static readonly Vector4 SnowColor = new Vector4(0.86274517f, 0.9294118f, 0.94117653f, 1f);

	// Token: 0x04002AE9 RID: 10985
	private static readonly Vector4 PebbleColor = new Vector4(0.13725491f, 0.2784314f, 0.2761563f, 1f);

	// Token: 0x04002AEA RID: 10986
	private static readonly Vector4 OffShoreColor = new Vector4(0.04090196f, 0.22060032f, 0.27450982f, 1f);

	// Token: 0x04002AEB RID: 10987
	private static readonly Vector3 SunDirection = Vector3.Normalize(new Vector3(0.95f, 2.87f, 2.37f));

	// Token: 0x04002AEC RID: 10988
	private const float SunPower = 0.65f;

	// Token: 0x04002AED RID: 10989
	private const float Brightness = 1.05f;

	// Token: 0x04002AEE RID: 10990
	private const float Contrast = 0.94f;

	// Token: 0x04002AEF RID: 10991
	private const float OceanWaterLevel = 0f;

	// Token: 0x04002AF0 RID: 10992
	private static readonly Vector3 Half = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x02000E7A RID: 3706
	private readonly struct Array2D<T>
	{
		// Token: 0x060052D4 RID: 21204 RVA: 0x001B11E0 File Offset: 0x001AF3E0
		public Array2D(T[] items, int width, int height)
		{
			this._items = items;
			this._width = width;
			this._height = height;
		}

		// Token: 0x170006F5 RID: 1781
		public T this[int x, int y]
		{
			get
			{
				int num = Mathf.Clamp(x, 0, this._width - 1);
				int num2 = Mathf.Clamp(y, 0, this._height - 1);
				return ref this._items[num2 * this._width + num];
			}
		}

		// Token: 0x04004C00 RID: 19456
		private readonly T[] _items;

		// Token: 0x04004C01 RID: 19457
		private readonly int _width;

		// Token: 0x04004C02 RID: 19458
		private readonly int _height;
	}
}
