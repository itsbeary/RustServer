using System;
using UnityEngine;

namespace Rust.Water5
{
	// Token: 0x02000B30 RID: 2864
	[CreateAssetMenu(fileName = "New Spectrum Settings", menuName = "Water5/Spectrum Settings")]
	public class OceanSpectrumSettings : ScriptableObject
	{
		// Token: 0x06004556 RID: 17750 RVA: 0x001959BE File Offset: 0x00193BBE
		[Button("Update Spectrum")]
		public void UpdateSpectrum()
		{
			WaterSystem instance = WaterSystem.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Refresh();
		}

		// Token: 0x04003E45 RID: 15941
		public OceanSettings oceanSettings;

		// Token: 0x04003E46 RID: 15942
		[Header("Deep Wave Settings")]
		public float g;

		// Token: 0x04003E47 RID: 15943
		public float beaufort;

		// Token: 0x04003E48 RID: 15944
		public float depth;

		// Token: 0x04003E49 RID: 15945
		public SpectrumSettings local;

		// Token: 0x04003E4A RID: 15946
		public SpectrumSettings swell;

		// Token: 0x04003E4B RID: 15947
		[Header("Material Settings")]
		public Color color;

		// Token: 0x04003E4C RID: 15948
		public Color specColor;

		// Token: 0x04003E4D RID: 15949
		public float smoothness;

		// Token: 0x04003E4E RID: 15950
		public Color waterColor;

		// Token: 0x04003E4F RID: 15951
		public Color waterExtinction;

		// Token: 0x04003E50 RID: 15952
		public float scatteringCoefficient;

		// Token: 0x04003E51 RID: 15953
		public Color subSurfaceColor;

		// Token: 0x04003E52 RID: 15954
		public float subSurfaceFalloff;

		// Token: 0x04003E53 RID: 15955
		public float subSurfaceBase;

		// Token: 0x04003E54 RID: 15956
		public float subSurfaceSun;

		// Token: 0x04003E55 RID: 15957
		public float subSurfaceAmount;

		// Token: 0x04003E56 RID: 15958
		public float foamAmount;

		// Token: 0x04003E57 RID: 15959
		public float foamScale;

		// Token: 0x04003E58 RID: 15960
		public Color foamColor;

		// Token: 0x04003E59 RID: 15961
		public Color baseFoamColor;
	}
}
