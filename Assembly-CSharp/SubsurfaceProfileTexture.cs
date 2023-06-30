using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000723 RID: 1827
public class SubsurfaceProfileTexture
{
	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06003317 RID: 13079 RVA: 0x001397C8 File Offset: 0x001379C8
	public Texture2D Texture
	{
		get
		{
			if (this.texture == null)
			{
				this.CreateResources();
			}
			return this.texture;
		}
	}

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06003318 RID: 13080 RVA: 0x001397E4 File Offset: 0x001379E4
	public Vector4[] TransmissionTints
	{
		get
		{
			if (this.texture == null)
			{
				this.CreateResources();
			}
			return this.transmissionTints;
		}
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x00139800 File Offset: 0x00137A00
	public void AddProfile(SubsurfaceProfile profile)
	{
		this.entries.Add(profile);
		if (this.entries.Count > 15)
		{
			Debug.LogWarning(string.Format("[SubsurfaceScattering] Maximum number of supported Subsurface Profiles has been reached ({0}/{1}). Please remove some.", this.entries.Count, 15));
		}
		this.ReleaseResources();
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x00139858 File Offset: 0x00137A58
	public static Color Clamp(Color color, float min = 0f, float max = 1f)
	{
		Color color2;
		color2.r = Mathf.Clamp(color.r, min, max);
		color2.g = Mathf.Clamp(color.g, min, max);
		color2.b = Mathf.Clamp(color.b, min, max);
		color2.a = Mathf.Clamp(color.a, min, max);
		return color2;
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x001398B8 File Offset: 0x00137AB8
	private void WriteKernel(ref Color[] pixels, ref Color[] kernel, int id, int y, in SubsurfaceProfileData data)
	{
		Color color = SubsurfaceProfileTexture.Clamp(data.SubsurfaceColor, 0f, 1f);
		Color color2 = SubsurfaceProfileTexture.Clamp(data.FalloffColor, 0.009f, 1f);
		this.transmissionTints[id] = data.TransmissionTint;
		kernel[0] = color;
		kernel[0].a = data.ScatterRadius;
		SeparableSSS.CalculateKernel(kernel, 1, 24, color, color2);
		SeparableSSS.CalculateKernel(kernel, 25, 16, color, color2);
		SeparableSSS.CalculateKernel(kernel, 41, 8, color, color2);
		int num = 49 * y;
		for (int i = 0; i < 49; i++)
		{
			Color color3 = kernel[i];
			color3.a *= ((i > 0) ? (data.ScatterRadius / 1024f) : 1f);
			pixels[num + i] = color3;
		}
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x0013999C File Offset: 0x00137B9C
	private void CreateResources()
	{
		if (this.entries.Count > 0)
		{
			int num = Mathf.Min(this.entries.Count, 15) + 1;
			this.ReleaseResources();
			this.texture = new Texture2D(49, num, TextureFormat.RGBAHalf, false, true);
			this.texture.name = "SubsurfaceProfiles";
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.filterMode = FilterMode.Bilinear;
			Color[] pixels = this.texture.GetPixels(0);
			Color[] array = new Color[49];
			int num2 = num - 1;
			int num3 = 0;
			int num4 = num3++;
			int num5 = num2--;
			SubsurfaceProfileData @default = SubsurfaceProfileData.Default;
			this.WriteKernel(ref pixels, ref array, num4, num5, @default);
			foreach (SubsurfaceProfile subsurfaceProfile in this.entries)
			{
				subsurfaceProfile.Id = num3;
				this.WriteKernel(ref pixels, ref array, num3++, num2--, subsurfaceProfile.Data);
				if (num2 < 0)
				{
					break;
				}
			}
			this.texture.SetPixels(pixels, 0);
			this.texture.Apply(false, false);
		}
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x00139AD4 File Offset: 0x00137CD4
	public void ReleaseResources()
	{
		if (this.texture != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texture);
			this.texture = null;
		}
		if (this.transmissionTints != null)
		{
			for (int i = 0; i < this.transmissionTints.Length; i++)
			{
				this.transmissionTints[i] = SubsurfaceProfileData.Default.TransmissionTint.linear;
			}
		}
	}

	// Token: 0x040029DE RID: 10718
	public const int SUBSURFACE_PROFILE_COUNT = 16;

	// Token: 0x040029DF RID: 10719
	public const int MAX_SUBSURFACE_PROFILES = 15;

	// Token: 0x040029E0 RID: 10720
	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	// Token: 0x040029E1 RID: 10721
	public const int SUBSURFACE_KERNEL_SIZE = 3;

	// Token: 0x040029E2 RID: 10722
	private HashSet<SubsurfaceProfile> entries = new HashSet<SubsurfaceProfile>();

	// Token: 0x040029E3 RID: 10723
	private Texture2D texture;

	// Token: 0x040029E4 RID: 10724
	private Vector4[] transmissionTints = new Vector4[16];

	// Token: 0x040029E5 RID: 10725
	private const int KernelSize0 = 24;

	// Token: 0x040029E6 RID: 10726
	private const int KernelSize1 = 16;

	// Token: 0x040029E7 RID: 10727
	private const int KernelSize2 = 8;

	// Token: 0x040029E8 RID: 10728
	private const int KernelTotalSize = 49;

	// Token: 0x040029E9 RID: 10729
	private const int Width = 49;

	// Token: 0x02000E4D RID: 3661
	private struct SubsurfaceProfileEntry
	{
		// Token: 0x0600527F RID: 21119 RVA: 0x001B04AF File Offset: 0x001AE6AF
		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}

		// Token: 0x04004B5A RID: 19290
		public SubsurfaceProfileData data;

		// Token: 0x04004B5B RID: 19291
		public SubsurfaceProfile profile;
	}
}
