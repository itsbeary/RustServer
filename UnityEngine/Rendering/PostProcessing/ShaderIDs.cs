using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA6 RID: 2726
	internal static class ShaderIDs
	{
		// Token: 0x04003A27 RID: 14887
		internal static readonly int MainTex = Shader.PropertyToID("_MainTex");

		// Token: 0x04003A28 RID: 14888
		internal static readonly int Jitter = Shader.PropertyToID("_Jitter");

		// Token: 0x04003A29 RID: 14889
		internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");

		// Token: 0x04003A2A RID: 14890
		internal static readonly int FinalBlendParameters = Shader.PropertyToID("_FinalBlendParameters");

		// Token: 0x04003A2B RID: 14891
		internal static readonly int HistoryTex = Shader.PropertyToID("_HistoryTex");

		// Token: 0x04003A2C RID: 14892
		internal static readonly int SMAA_Flip = Shader.PropertyToID("_SMAA_Flip");

		// Token: 0x04003A2D RID: 14893
		internal static readonly int SMAA_Flop = Shader.PropertyToID("_SMAA_Flop");

		// Token: 0x04003A2E RID: 14894
		internal static readonly int AOParams = Shader.PropertyToID("_AOParams");

		// Token: 0x04003A2F RID: 14895
		internal static readonly int AOColor = Shader.PropertyToID("_AOColor");

		// Token: 0x04003A30 RID: 14896
		internal static readonly int OcclusionTexture1 = Shader.PropertyToID("_OcclusionTexture1");

		// Token: 0x04003A31 RID: 14897
		internal static readonly int OcclusionTexture2 = Shader.PropertyToID("_OcclusionTexture2");

		// Token: 0x04003A32 RID: 14898
		internal static readonly int SAOcclusionTexture = Shader.PropertyToID("_SAOcclusionTexture");

		// Token: 0x04003A33 RID: 14899
		internal static readonly int MSVOcclusionTexture = Shader.PropertyToID("_MSVOcclusionTexture");

		// Token: 0x04003A34 RID: 14900
		internal static readonly int DepthCopy = Shader.PropertyToID("DepthCopy");

		// Token: 0x04003A35 RID: 14901
		internal static readonly int LinearDepth = Shader.PropertyToID("LinearDepth");

		// Token: 0x04003A36 RID: 14902
		internal static readonly int LowDepth1 = Shader.PropertyToID("LowDepth1");

		// Token: 0x04003A37 RID: 14903
		internal static readonly int LowDepth2 = Shader.PropertyToID("LowDepth2");

		// Token: 0x04003A38 RID: 14904
		internal static readonly int LowDepth3 = Shader.PropertyToID("LowDepth3");

		// Token: 0x04003A39 RID: 14905
		internal static readonly int LowDepth4 = Shader.PropertyToID("LowDepth4");

		// Token: 0x04003A3A RID: 14906
		internal static readonly int TiledDepth1 = Shader.PropertyToID("TiledDepth1");

		// Token: 0x04003A3B RID: 14907
		internal static readonly int TiledDepth2 = Shader.PropertyToID("TiledDepth2");

		// Token: 0x04003A3C RID: 14908
		internal static readonly int TiledDepth3 = Shader.PropertyToID("TiledDepth3");

		// Token: 0x04003A3D RID: 14909
		internal static readonly int TiledDepth4 = Shader.PropertyToID("TiledDepth4");

		// Token: 0x04003A3E RID: 14910
		internal static readonly int Occlusion1 = Shader.PropertyToID("Occlusion1");

		// Token: 0x04003A3F RID: 14911
		internal static readonly int Occlusion2 = Shader.PropertyToID("Occlusion2");

		// Token: 0x04003A40 RID: 14912
		internal static readonly int Occlusion3 = Shader.PropertyToID("Occlusion3");

		// Token: 0x04003A41 RID: 14913
		internal static readonly int Occlusion4 = Shader.PropertyToID("Occlusion4");

		// Token: 0x04003A42 RID: 14914
		internal static readonly int Combined1 = Shader.PropertyToID("Combined1");

		// Token: 0x04003A43 RID: 14915
		internal static readonly int Combined2 = Shader.PropertyToID("Combined2");

		// Token: 0x04003A44 RID: 14916
		internal static readonly int Combined3 = Shader.PropertyToID("Combined3");

		// Token: 0x04003A45 RID: 14917
		internal static readonly int SSRResolveTemp = Shader.PropertyToID("_SSRResolveTemp");

		// Token: 0x04003A46 RID: 14918
		internal static readonly int Noise = Shader.PropertyToID("_Noise");

		// Token: 0x04003A47 RID: 14919
		internal static readonly int Test = Shader.PropertyToID("_Test");

		// Token: 0x04003A48 RID: 14920
		internal static readonly int Resolve = Shader.PropertyToID("_Resolve");

		// Token: 0x04003A49 RID: 14921
		internal static readonly int History = Shader.PropertyToID("_History");

		// Token: 0x04003A4A RID: 14922
		internal static readonly int ViewMatrix = Shader.PropertyToID("_ViewMatrix");

		// Token: 0x04003A4B RID: 14923
		internal static readonly int InverseViewMatrix = Shader.PropertyToID("_InverseViewMatrix");

		// Token: 0x04003A4C RID: 14924
		internal static readonly int InverseProjectionMatrix = Shader.PropertyToID("_InverseProjectionMatrix");

		// Token: 0x04003A4D RID: 14925
		internal static readonly int ScreenSpaceProjectionMatrix = Shader.PropertyToID("_ScreenSpaceProjectionMatrix");

		// Token: 0x04003A4E RID: 14926
		internal static readonly int Params2 = Shader.PropertyToID("_Params2");

		// Token: 0x04003A4F RID: 14927
		internal static readonly int FogColor = Shader.PropertyToID("_FogColor");

		// Token: 0x04003A50 RID: 14928
		internal static readonly int FogParams = Shader.PropertyToID("_FogParams");

		// Token: 0x04003A51 RID: 14929
		internal static readonly int VelocityScale = Shader.PropertyToID("_VelocityScale");

		// Token: 0x04003A52 RID: 14930
		internal static readonly int MaxBlurRadius = Shader.PropertyToID("_MaxBlurRadius");

		// Token: 0x04003A53 RID: 14931
		internal static readonly int RcpMaxBlurRadius = Shader.PropertyToID("_RcpMaxBlurRadius");

		// Token: 0x04003A54 RID: 14932
		internal static readonly int VelocityTex = Shader.PropertyToID("_VelocityTex");

		// Token: 0x04003A55 RID: 14933
		internal static readonly int Tile2RT = Shader.PropertyToID("_Tile2RT");

		// Token: 0x04003A56 RID: 14934
		internal static readonly int Tile4RT = Shader.PropertyToID("_Tile4RT");

		// Token: 0x04003A57 RID: 14935
		internal static readonly int Tile8RT = Shader.PropertyToID("_Tile8RT");

		// Token: 0x04003A58 RID: 14936
		internal static readonly int TileMaxOffs = Shader.PropertyToID("_TileMaxOffs");

		// Token: 0x04003A59 RID: 14937
		internal static readonly int TileMaxLoop = Shader.PropertyToID("_TileMaxLoop");

		// Token: 0x04003A5A RID: 14938
		internal static readonly int TileVRT = Shader.PropertyToID("_TileVRT");

		// Token: 0x04003A5B RID: 14939
		internal static readonly int NeighborMaxTex = Shader.PropertyToID("_NeighborMaxTex");

		// Token: 0x04003A5C RID: 14940
		internal static readonly int LoopCount = Shader.PropertyToID("_LoopCount");

		// Token: 0x04003A5D RID: 14941
		internal static readonly int DepthOfFieldTemp = Shader.PropertyToID("_DepthOfFieldTemp");

		// Token: 0x04003A5E RID: 14942
		internal static readonly int DepthOfFieldTex = Shader.PropertyToID("_DepthOfFieldTex");

		// Token: 0x04003A5F RID: 14943
		internal static readonly int Distance = Shader.PropertyToID("_Distance");

		// Token: 0x04003A60 RID: 14944
		internal static readonly int LensCoeff = Shader.PropertyToID("_LensCoeff");

		// Token: 0x04003A61 RID: 14945
		internal static readonly int MaxCoC = Shader.PropertyToID("_MaxCoC");

		// Token: 0x04003A62 RID: 14946
		internal static readonly int RcpMaxCoC = Shader.PropertyToID("_RcpMaxCoC");

		// Token: 0x04003A63 RID: 14947
		internal static readonly int RcpAspect = Shader.PropertyToID("_RcpAspect");

		// Token: 0x04003A64 RID: 14948
		internal static readonly int CoCTex = Shader.PropertyToID("_CoCTex");

		// Token: 0x04003A65 RID: 14949
		internal static readonly int TaaParams = Shader.PropertyToID("_TaaParams");

		// Token: 0x04003A66 RID: 14950
		internal static readonly int AutoExposureTex = Shader.PropertyToID("_AutoExposureTex");

		// Token: 0x04003A67 RID: 14951
		internal static readonly int HistogramBuffer = Shader.PropertyToID("_HistogramBuffer");

		// Token: 0x04003A68 RID: 14952
		internal static readonly int Params = Shader.PropertyToID("_Params");

		// Token: 0x04003A69 RID: 14953
		internal static readonly int ScaleOffsetRes = Shader.PropertyToID("_ScaleOffsetRes");

		// Token: 0x04003A6A RID: 14954
		internal static readonly int BloomTex = Shader.PropertyToID("_BloomTex");

		// Token: 0x04003A6B RID: 14955
		internal static readonly int SampleScale = Shader.PropertyToID("_SampleScale");

		// Token: 0x04003A6C RID: 14956
		internal static readonly int Threshold = Shader.PropertyToID("_Threshold");

		// Token: 0x04003A6D RID: 14957
		internal static readonly int ColorIntensity = Shader.PropertyToID("_ColorIntensity");

		// Token: 0x04003A6E RID: 14958
		internal static readonly int Bloom_DirtTex = Shader.PropertyToID("_Bloom_DirtTex");

		// Token: 0x04003A6F RID: 14959
		internal static readonly int Bloom_Settings = Shader.PropertyToID("_Bloom_Settings");

		// Token: 0x04003A70 RID: 14960
		internal static readonly int Bloom_Color = Shader.PropertyToID("_Bloom_Color");

		// Token: 0x04003A71 RID: 14961
		internal static readonly int Bloom_DirtTileOffset = Shader.PropertyToID("_Bloom_DirtTileOffset");

		// Token: 0x04003A72 RID: 14962
		internal static readonly int ChromaticAberration_Amount = Shader.PropertyToID("_ChromaticAberration_Amount");

		// Token: 0x04003A73 RID: 14963
		internal static readonly int ChromaticAberration_SpectralLut = Shader.PropertyToID("_ChromaticAberration_SpectralLut");

		// Token: 0x04003A74 RID: 14964
		internal static readonly int Distortion_CenterScale = Shader.PropertyToID("_Distortion_CenterScale");

		// Token: 0x04003A75 RID: 14965
		internal static readonly int Distortion_Amount = Shader.PropertyToID("_Distortion_Amount");

		// Token: 0x04003A76 RID: 14966
		internal static readonly int Lut2D = Shader.PropertyToID("_Lut2D");

		// Token: 0x04003A77 RID: 14967
		internal static readonly int Lut3D = Shader.PropertyToID("_Lut3D");

		// Token: 0x04003A78 RID: 14968
		internal static readonly int Lut3D_Params = Shader.PropertyToID("_Lut3D_Params");

		// Token: 0x04003A79 RID: 14969
		internal static readonly int Lut2D_Params = Shader.PropertyToID("_Lut2D_Params");

		// Token: 0x04003A7A RID: 14970
		internal static readonly int UserLut2D_Params = Shader.PropertyToID("_UserLut2D_Params");

		// Token: 0x04003A7B RID: 14971
		internal static readonly int PostExposure = Shader.PropertyToID("_PostExposure");

		// Token: 0x04003A7C RID: 14972
		internal static readonly int ColorBalance = Shader.PropertyToID("_ColorBalance");

		// Token: 0x04003A7D RID: 14973
		internal static readonly int ColorFilter = Shader.PropertyToID("_ColorFilter");

		// Token: 0x04003A7E RID: 14974
		internal static readonly int HueSatCon = Shader.PropertyToID("_HueSatCon");

		// Token: 0x04003A7F RID: 14975
		internal static readonly int Brightness = Shader.PropertyToID("_Brightness");

		// Token: 0x04003A80 RID: 14976
		internal static readonly int ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");

		// Token: 0x04003A81 RID: 14977
		internal static readonly int ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");

		// Token: 0x04003A82 RID: 14978
		internal static readonly int ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");

		// Token: 0x04003A83 RID: 14979
		internal static readonly int Lift = Shader.PropertyToID("_Lift");

		// Token: 0x04003A84 RID: 14980
		internal static readonly int InvGamma = Shader.PropertyToID("_InvGamma");

		// Token: 0x04003A85 RID: 14981
		internal static readonly int Gain = Shader.PropertyToID("_Gain");

		// Token: 0x04003A86 RID: 14982
		internal static readonly int Curves = Shader.PropertyToID("_Curves");

		// Token: 0x04003A87 RID: 14983
		internal static readonly int CustomToneCurve = Shader.PropertyToID("_CustomToneCurve");

		// Token: 0x04003A88 RID: 14984
		internal static readonly int ToeSegmentA = Shader.PropertyToID("_ToeSegmentA");

		// Token: 0x04003A89 RID: 14985
		internal static readonly int ToeSegmentB = Shader.PropertyToID("_ToeSegmentB");

		// Token: 0x04003A8A RID: 14986
		internal static readonly int MidSegmentA = Shader.PropertyToID("_MidSegmentA");

		// Token: 0x04003A8B RID: 14987
		internal static readonly int MidSegmentB = Shader.PropertyToID("_MidSegmentB");

		// Token: 0x04003A8C RID: 14988
		internal static readonly int ShoSegmentA = Shader.PropertyToID("_ShoSegmentA");

		// Token: 0x04003A8D RID: 14989
		internal static readonly int ShoSegmentB = Shader.PropertyToID("_ShoSegmentB");

		// Token: 0x04003A8E RID: 14990
		internal static readonly int Vignette_Color = Shader.PropertyToID("_Vignette_Color");

		// Token: 0x04003A8F RID: 14991
		internal static readonly int Vignette_Center = Shader.PropertyToID("_Vignette_Center");

		// Token: 0x04003A90 RID: 14992
		internal static readonly int Vignette_Settings = Shader.PropertyToID("_Vignette_Settings");

		// Token: 0x04003A91 RID: 14993
		internal static readonly int Vignette_Mask = Shader.PropertyToID("_Vignette_Mask");

		// Token: 0x04003A92 RID: 14994
		internal static readonly int Vignette_Opacity = Shader.PropertyToID("_Vignette_Opacity");

		// Token: 0x04003A93 RID: 14995
		internal static readonly int Vignette_Mode = Shader.PropertyToID("_Vignette_Mode");

		// Token: 0x04003A94 RID: 14996
		internal static readonly int Grain_Params1 = Shader.PropertyToID("_Grain_Params1");

		// Token: 0x04003A95 RID: 14997
		internal static readonly int Grain_Params2 = Shader.PropertyToID("_Grain_Params2");

		// Token: 0x04003A96 RID: 14998
		internal static readonly int GrainTex = Shader.PropertyToID("_GrainTex");

		// Token: 0x04003A97 RID: 14999
		internal static readonly int Phase = Shader.PropertyToID("_Phase");

		// Token: 0x04003A98 RID: 15000
		internal static readonly int GrainNoiseParameters = Shader.PropertyToID("_NoiseParameters");

		// Token: 0x04003A99 RID: 15001
		internal static readonly int LumaInAlpha = Shader.PropertyToID("_LumaInAlpha");

		// Token: 0x04003A9A RID: 15002
		internal static readonly int DitheringTex = Shader.PropertyToID("_DitheringTex");

		// Token: 0x04003A9B RID: 15003
		internal static readonly int Dithering_Coords = Shader.PropertyToID("_Dithering_Coords");

		// Token: 0x04003A9C RID: 15004
		internal static readonly int From = Shader.PropertyToID("_From");

		// Token: 0x04003A9D RID: 15005
		internal static readonly int To = Shader.PropertyToID("_To");

		// Token: 0x04003A9E RID: 15006
		internal static readonly int Interp = Shader.PropertyToID("_Interp");

		// Token: 0x04003A9F RID: 15007
		internal static readonly int TargetColor = Shader.PropertyToID("_TargetColor");

		// Token: 0x04003AA0 RID: 15008
		internal static readonly int HalfResFinalCopy = Shader.PropertyToID("_HalfResFinalCopy");

		// Token: 0x04003AA1 RID: 15009
		internal static readonly int WaveformSource = Shader.PropertyToID("_WaveformSource");

		// Token: 0x04003AA2 RID: 15010
		internal static readonly int WaveformBuffer = Shader.PropertyToID("_WaveformBuffer");

		// Token: 0x04003AA3 RID: 15011
		internal static readonly int VectorscopeBuffer = Shader.PropertyToID("_VectorscopeBuffer");

		// Token: 0x04003AA4 RID: 15012
		internal static readonly int RenderViewportScaleFactor = Shader.PropertyToID("_RenderViewportScaleFactor");

		// Token: 0x04003AA5 RID: 15013
		internal static readonly int UVTransform = Shader.PropertyToID("_UVTransform");

		// Token: 0x04003AA6 RID: 15014
		internal static readonly int DepthSlice = Shader.PropertyToID("_DepthSlice");

		// Token: 0x04003AA7 RID: 15015
		internal static readonly int UVScaleOffset = Shader.PropertyToID("_UVScaleOffset");

		// Token: 0x04003AA8 RID: 15016
		internal static readonly int PosScaleOffset = Shader.PropertyToID("_PosScaleOffset");
	}
}
