using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x020009B2 RID: 2482
	public static class Consts
	{
		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06003B2C RID: 15148 RVA: 0x0015E061 File Offset: 0x0015C261
		public static HideFlags ProceduralObjectsHideFlags
		{
			get
			{
				if (!Consts.ProceduralObjectsVisibleInEditor)
				{
					return HideFlags.HideAndDontSave;
				}
				return HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
			}
		}

		// Token: 0x06003B2D RID: 15149 RVA: 0x0015E070 File Offset: 0x0015C270
		// Note: this type is marked as 'beforefieldinit'.
		static Consts()
		{
			bool[] array = new bool[3];
			array[0] = true;
			array[1] = true;
			Consts.BlendingMode_AlphaAsBlack = array;
		}

		// Token: 0x040035EA RID: 13802
		private const string HelpUrlBase = "http://saladgamer.com/vlb-doc/";

		// Token: 0x040035EB RID: 13803
		public const string HelpUrlBeam = "http://saladgamer.com/vlb-doc/comp-lightbeam/";

		// Token: 0x040035EC RID: 13804
		public const string HelpUrlDustParticles = "http://saladgamer.com/vlb-doc/comp-dustparticles/";

		// Token: 0x040035ED RID: 13805
		public const string HelpUrlDynamicOcclusion = "http://saladgamer.com/vlb-doc/comp-dynocclusion/";

		// Token: 0x040035EE RID: 13806
		public const string HelpUrlTriggerZone = "http://saladgamer.com/vlb-doc/comp-triggerzone/";

		// Token: 0x040035EF RID: 13807
		public const string HelpUrlConfig = "http://saladgamer.com/vlb-doc/config/";

		// Token: 0x040035F0 RID: 13808
		public static readonly bool ProceduralObjectsVisibleInEditor = true;

		// Token: 0x040035F1 RID: 13809
		public static readonly Color FlatColor = Color.white;

		// Token: 0x040035F2 RID: 13810
		public const ColorMode ColorModeDefault = ColorMode.Flat;

		// Token: 0x040035F3 RID: 13811
		public const float Alpha = 1f;

		// Token: 0x040035F4 RID: 13812
		public const float SpotAngleDefault = 35f;

		// Token: 0x040035F5 RID: 13813
		public const float SpotAngleMin = 0.1f;

		// Token: 0x040035F6 RID: 13814
		public const float SpotAngleMax = 179.9f;

		// Token: 0x040035F7 RID: 13815
		public const float ConeRadiusStart = 0.1f;

		// Token: 0x040035F8 RID: 13816
		public const MeshType GeomMeshType = MeshType.Shared;

		// Token: 0x040035F9 RID: 13817
		public const int GeomSidesDefault = 18;

		// Token: 0x040035FA RID: 13818
		public const int GeomSidesMin = 3;

		// Token: 0x040035FB RID: 13819
		public const int GeomSidesMax = 256;

		// Token: 0x040035FC RID: 13820
		public const int GeomSegmentsDefault = 5;

		// Token: 0x040035FD RID: 13821
		public const int GeomSegmentsMin = 0;

		// Token: 0x040035FE RID: 13822
		public const int GeomSegmentsMax = 64;

		// Token: 0x040035FF RID: 13823
		public const bool GeomCap = false;

		// Token: 0x04003600 RID: 13824
		public const AttenuationEquation AttenuationEquationDefault = AttenuationEquation.Quadratic;

		// Token: 0x04003601 RID: 13825
		public const float AttenuationCustomBlending = 0.5f;

		// Token: 0x04003602 RID: 13826
		public const float FadeStart = 0f;

		// Token: 0x04003603 RID: 13827
		public const float FadeEnd = 3f;

		// Token: 0x04003604 RID: 13828
		public const float FadeMinThreshold = 0.01f;

		// Token: 0x04003605 RID: 13829
		public const float DepthBlendDistance = 2f;

		// Token: 0x04003606 RID: 13830
		public const float CameraClippingDistance = 0.5f;

		// Token: 0x04003607 RID: 13831
		public const float FresnelPowMaxValue = 10f;

		// Token: 0x04003608 RID: 13832
		public const float FresnelPow = 8f;

		// Token: 0x04003609 RID: 13833
		public const float GlareFrontal = 0.5f;

		// Token: 0x0400360A RID: 13834
		public const float GlareBehind = 0.5f;

		// Token: 0x0400360B RID: 13835
		public const float NoiseIntensityMin = 0f;

		// Token: 0x0400360C RID: 13836
		public const float NoiseIntensityMax = 1f;

		// Token: 0x0400360D RID: 13837
		public const float NoiseIntensityDefault = 0.5f;

		// Token: 0x0400360E RID: 13838
		public const float NoiseScaleMin = 0.01f;

		// Token: 0x0400360F RID: 13839
		public const float NoiseScaleMax = 2f;

		// Token: 0x04003610 RID: 13840
		public const float NoiseScaleDefault = 0.5f;

		// Token: 0x04003611 RID: 13841
		public static readonly Vector3 NoiseVelocityDefault = new Vector3(0.07f, 0.18f, 0.05f);

		// Token: 0x04003612 RID: 13842
		public const BlendingMode BlendingModeDefault = BlendingMode.Additive;

		// Token: 0x04003613 RID: 13843
		public static readonly BlendMode[] BlendingMode_SrcFactor = new BlendMode[]
		{
			BlendMode.One,
			BlendMode.OneMinusDstColor,
			BlendMode.SrcAlpha
		};

		// Token: 0x04003614 RID: 13844
		public static readonly BlendMode[] BlendingMode_DstFactor = new BlendMode[]
		{
			BlendMode.One,
			BlendMode.One,
			BlendMode.OneMinusSrcAlpha
		};

		// Token: 0x04003615 RID: 13845
		public static readonly bool[] BlendingMode_AlphaAsBlack;

		// Token: 0x04003616 RID: 13846
		public const float DynOcclusionMinSurfaceRatioDefault = 0.5f;

		// Token: 0x04003617 RID: 13847
		public const float DynOcclusionMinSurfaceRatioMin = 50f;

		// Token: 0x04003618 RID: 13848
		public const float DynOcclusionMinSurfaceRatioMax = 100f;

		// Token: 0x04003619 RID: 13849
		public const float DynOcclusionMaxSurfaceDotDefault = 0.25f;

		// Token: 0x0400361A RID: 13850
		public const float DynOcclusionMaxSurfaceAngleMin = 45f;

		// Token: 0x0400361B RID: 13851
		public const float DynOcclusionMaxSurfaceAngleMax = 90f;

		// Token: 0x0400361C RID: 13852
		public const int ConfigGeometryLayerIDDefault = 1;

		// Token: 0x0400361D RID: 13853
		public const string ConfigGeometryTagDefault = "Untagged";

		// Token: 0x0400361E RID: 13854
		public const RenderQueue ConfigGeometryRenderQueueDefault = RenderQueue.Transparent;

		// Token: 0x0400361F RID: 13855
		public const bool ConfigGeometryForceSinglePassDefault = false;

		// Token: 0x04003620 RID: 13856
		public const int ConfigNoise3DSizeDefault = 64;

		// Token: 0x04003621 RID: 13857
		public const int ConfigSharedMeshSides = 24;

		// Token: 0x04003622 RID: 13858
		public const int ConfigSharedMeshSegments = 5;
	}
}
