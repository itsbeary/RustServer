using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A69 RID: 2665
	[Preserve]
	[Serializable]
	public sealed class Fog
	{
		// Token: 0x06003F9E RID: 16286 RVA: 0x0000441C File Offset: 0x0000261C
		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003F9F RID: 16287 RVA: 0x00176620 File Offset: 0x00174820
		internal bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled && RenderSettings.fog && !RuntimeUtilities.scriptableRenderPipelineActive && context.resources.shaders.deferredFog && context.resources.shaders.deferredFog.isSupported && context.camera.actualRenderingPath == RenderingPath.DeferredShading;
		}

		// Token: 0x06003FA0 RID: 16288 RVA: 0x00176684 File Offset: 0x00174884
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.deferredFog);
			propertySheet.ClearKeywords();
			Color color = (RuntimeUtilities.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor);
			propertySheet.properties.SetVector(ShaderIDs.FogColor, color);
			propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, this.excludeSkybox ? 1 : 0, false, null);
		}

		// Token: 0x04003941 RID: 14657
		[Tooltip("Enables the internal deferred fog pass. Actual fog settings should be set in the Lighting panel.")]
		public bool enabled = true;

		// Token: 0x04003942 RID: 14658
		[Tooltip("Mark true for the fog to ignore the skybox")]
		public bool excludeSkybox = true;
	}
}
