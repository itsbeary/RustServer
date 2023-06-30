using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009C9 RID: 2505
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
	public class SMAA : MonoBehaviour
	{
		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06003BAB RID: 15275 RVA: 0x001601A9 File Offset: 0x0015E3A9
		public Material Material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new Material(this.Shader);
					this.m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_Material;
			}
		}

		// Token: 0x040036A6 RID: 13990
		public DebugPass DebugPass;

		// Token: 0x040036A7 RID: 13991
		public QualityPreset Quality = QualityPreset.High;

		// Token: 0x040036A8 RID: 13992
		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		// Token: 0x040036A9 RID: 13993
		public bool UsePredication;

		// Token: 0x040036AA RID: 13994
		public Preset CustomPreset;

		// Token: 0x040036AB RID: 13995
		public PredicationPreset CustomPredicationPreset;

		// Token: 0x040036AC RID: 13996
		public Shader Shader;

		// Token: 0x040036AD RID: 13997
		public Texture2D AreaTex;

		// Token: 0x040036AE RID: 13998
		public Texture2D SearchTex;

		// Token: 0x040036AF RID: 13999
		protected Camera m_Camera;

		// Token: 0x040036B0 RID: 14000
		protected Preset m_LowPreset;

		// Token: 0x040036B1 RID: 14001
		protected Preset m_MediumPreset;

		// Token: 0x040036B2 RID: 14002
		protected Preset m_HighPreset;

		// Token: 0x040036B3 RID: 14003
		protected Preset m_UltraPreset;

		// Token: 0x040036B4 RID: 14004
		protected Material m_Material;
	}
}
