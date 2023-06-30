using System;
using UnityEngine;

// Token: 0x02000721 RID: 1825
[Serializable]
public struct SubsurfaceProfileData
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x0600330E RID: 13070 RVA: 0x001396A0 File Offset: 0x001378A0
	public static SubsurfaceProfileData Default
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 1.2f,
				SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f),
				FalloffColor = new Color(1f, 0.37f, 0.3f),
				TransmissionTint = new Color(0.48f, 0.41f, 0.28f)
			};
		}
	}

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x0600330F RID: 13071 RVA: 0x00139714 File Offset: 0x00137914
	public static SubsurfaceProfileData Invalid
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 0f,
				SubsurfaceColor = Color.clear,
				FalloffColor = Color.clear,
				TransmissionTint = Color.clear
			};
		}
	}

	// Token: 0x040029D7 RID: 10711
	[Range(0.1f, 100f)]
	public float ScatterRadius;

	// Token: 0x040029D8 RID: 10712
	[ColorUsage(false, false)]
	public Color SubsurfaceColor;

	// Token: 0x040029D9 RID: 10713
	[ColorUsage(false, false)]
	public Color FalloffColor;

	// Token: 0x040029DA RID: 10714
	[ColorUsage(false, true)]
	public Color TransmissionTint;
}
