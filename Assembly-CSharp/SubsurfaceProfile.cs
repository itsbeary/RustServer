using System;
using UnityEngine;

// Token: 0x02000722 RID: 1826
public class SubsurfaceProfile : ScriptableObject
{
	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x06003310 RID: 13072 RVA: 0x0013975A File Offset: 0x0013795A
	public static Texture2D Texture
	{
		get
		{
			if (SubsurfaceProfile.profileTexture == null)
			{
				return null;
			}
			return SubsurfaceProfile.profileTexture.Texture;
		}
	}

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x06003311 RID: 13073 RVA: 0x0013976F File Offset: 0x0013796F
	public static Vector4[] TransmissionTints
	{
		get
		{
			if (SubsurfaceProfile.profileTexture == null)
			{
				return null;
			}
			return SubsurfaceProfile.profileTexture.TransmissionTints;
		}
	}

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x06003312 RID: 13074 RVA: 0x00139784 File Offset: 0x00137984
	// (set) Token: 0x06003313 RID: 13075 RVA: 0x0013978C File Offset: 0x0013798C
	public int Id
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x00139795 File Offset: 0x00137995
	private void OnEnable()
	{
		SubsurfaceProfile.profileTexture.AddProfile(this);
	}

	// Token: 0x040029DB RID: 10715
	private static SubsurfaceProfileTexture profileTexture = new SubsurfaceProfileTexture();

	// Token: 0x040029DC RID: 10716
	public SubsurfaceProfileData Data = SubsurfaceProfileData.Default;

	// Token: 0x040029DD RID: 10717
	private int id = -1;
}
