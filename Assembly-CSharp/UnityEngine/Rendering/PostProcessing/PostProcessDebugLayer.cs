using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A94 RID: 2708
	[Serializable]
	public sealed class PostProcessDebugLayer
	{
		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x0600404E RID: 16462 RVA: 0x0017AE8B File Offset: 0x0017908B
		// (set) Token: 0x0600404F RID: 16463 RVA: 0x0017AE93 File Offset: 0x00179093
		public RenderTexture debugOverlayTarget { get; private set; }

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06004050 RID: 16464 RVA: 0x0017AE9C File Offset: 0x0017909C
		// (set) Token: 0x06004051 RID: 16465 RVA: 0x0017AEA4 File Offset: 0x001790A4
		public bool debugOverlayActive { get; private set; }

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06004052 RID: 16466 RVA: 0x0017AEAD File Offset: 0x001790AD
		// (set) Token: 0x06004053 RID: 16467 RVA: 0x0017AEB5 File Offset: 0x001790B5
		public DebugOverlay debugOverlay { get; private set; }

		// Token: 0x06004054 RID: 16468 RVA: 0x0017AEC0 File Offset: 0x001790C0
		internal void OnEnable()
		{
			RuntimeUtilities.CreateIfNull<LightMeterMonitor>(ref this.lightMeter);
			RuntimeUtilities.CreateIfNull<HistogramMonitor>(ref this.histogram);
			RuntimeUtilities.CreateIfNull<WaveformMonitor>(ref this.waveform);
			RuntimeUtilities.CreateIfNull<VectorscopeMonitor>(ref this.vectorscope);
			RuntimeUtilities.CreateIfNull<PostProcessDebugLayer.OverlaySettings>(ref this.overlaySettings);
			this.m_Monitors = new Dictionary<MonitorType, Monitor>
			{
				{
					MonitorType.LightMeter,
					this.lightMeter
				},
				{
					MonitorType.Histogram,
					this.histogram
				},
				{
					MonitorType.Waveform,
					this.waveform
				},
				{
					MonitorType.Vectorscope,
					this.vectorscope
				}
			};
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.OnEnable();
			}
		}

		// Token: 0x06004055 RID: 16469 RVA: 0x0017AF90 File Offset: 0x00179190
		internal void OnDisable()
		{
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.OnDisable();
			}
			this.DestroyDebugOverlayTarget();
		}

		// Token: 0x06004056 RID: 16470 RVA: 0x0017AFF0 File Offset: 0x001791F0
		private void DestroyDebugOverlayTarget()
		{
			RuntimeUtilities.Destroy(this.debugOverlayTarget);
			this.debugOverlayTarget = null;
		}

		// Token: 0x06004057 RID: 16471 RVA: 0x0017B004 File Offset: 0x00179204
		public void RequestMonitorPass(MonitorType monitor)
		{
			this.m_Monitors[monitor].requested = true;
		}

		// Token: 0x06004058 RID: 16472 RVA: 0x0017B018 File Offset: 0x00179218
		public void RequestDebugOverlay(DebugOverlay mode)
		{
			this.debugOverlay = mode;
		}

		// Token: 0x06004059 RID: 16473 RVA: 0x0017B021 File Offset: 0x00179221
		internal void SetFrameSize(int width, int height)
		{
			this.frameWidth = width;
			this.frameHeight = height;
			this.debugOverlayActive = false;
		}

		// Token: 0x0600405A RID: 16474 RVA: 0x0017B038 File Offset: 0x00179238
		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			if (this.debugOverlayTarget == null || !this.debugOverlayTarget.IsCreated() || this.debugOverlayTarget.width != this.frameWidth || this.debugOverlayTarget.height != this.frameHeight)
			{
				RuntimeUtilities.Destroy(this.debugOverlayTarget);
				this.debugOverlayTarget = new RenderTexture(this.frameWidth, this.frameHeight, 0, RenderTextureFormat.ARGB32)
				{
					name = "Debug Overlay Target",
					anisoLevel = 1,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					hideFlags = HideFlags.HideAndDontSave
				};
				this.debugOverlayTarget.Create();
			}
			cmd.BlitFullscreenTriangle(source, this.debugOverlayTarget, sheet, pass, false, null);
			this.debugOverlayActive = true;
		}

		// Token: 0x0600405B RID: 16475 RVA: 0x0017B104 File Offset: 0x00179304
		internal DepthTextureMode GetCameraFlags()
		{
			if (this.debugOverlay == DebugOverlay.Depth)
			{
				return DepthTextureMode.Depth;
			}
			if (this.debugOverlay == DebugOverlay.Normals)
			{
				return DepthTextureMode.DepthNormals;
			}
			if (this.debugOverlay == DebugOverlay.MotionVectors)
			{
				return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
			}
			return DepthTextureMode.None;
		}

		// Token: 0x0600405C RID: 16476 RVA: 0x0017B128 File Offset: 0x00179328
		internal void RenderMonitors(PostProcessRenderContext context)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				bool flag3 = keyValuePair.Value.IsRequestedAndSupported(context);
				flag = flag || flag3;
				flag2 |= flag3 && keyValuePair.Value.NeedsHalfRes();
			}
			if (!flag)
			{
				return;
			}
			CommandBuffer command = context.command;
			command.BeginSample("Monitors");
			if (flag2)
			{
				command.GetTemporaryRT(ShaderIDs.HalfResFinalCopy, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
				command.Blit(context.destination, ShaderIDs.HalfResFinalCopy);
			}
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair2 in this.m_Monitors)
			{
				Monitor value = keyValuePair2.Value;
				if (value.requested)
				{
					value.Render(context);
				}
			}
			if (flag2)
			{
				command.ReleaseTemporaryRT(ShaderIDs.HalfResFinalCopy);
			}
			command.EndSample("Monitors");
		}

		// Token: 0x0600405D RID: 16477 RVA: 0x0017B264 File Offset: 0x00179464
		internal void RenderSpecialOverlays(PostProcessRenderContext context)
		{
			if (this.debugOverlay == DebugOverlay.Depth)
			{
				PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.linearDepth ? 1f : 0f, 0f, 0f, 0f));
				this.PushDebugOverlay(context.command, BuiltinRenderTextureType.None, propertySheet, 0);
				return;
			}
			if (this.debugOverlay == DebugOverlay.Normals)
			{
				PropertySheet propertySheet2 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet2.ClearKeywords();
				if (context.camera.actualRenderingPath == RenderingPath.DeferredLighting)
				{
					propertySheet2.EnableKeyword("SOURCE_GBUFFER");
				}
				this.PushDebugOverlay(context.command, BuiltinRenderTextureType.None, propertySheet2, 1);
				return;
			}
			if (this.debugOverlay == DebugOverlay.MotionVectors)
			{
				PropertySheet propertySheet3 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet3.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.motionColorIntensity, (float)this.overlaySettings.motionGridSize, 0f, 0f));
				this.PushDebugOverlay(context.command, context.source, propertySheet3, 2);
				return;
			}
			if (this.debugOverlay == DebugOverlay.NANTracker)
			{
				PropertySheet propertySheet4 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				this.PushDebugOverlay(context.command, context.source, propertySheet4, 3);
				return;
			}
			if (this.debugOverlay == DebugOverlay.ColorBlindnessSimulation)
			{
				PropertySheet propertySheet5 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet5.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.colorBlindnessStrength, 0f, 0f, 0f));
				this.PushDebugOverlay(context.command, context.source, propertySheet5, (int)(4 + this.overlaySettings.colorBlindnessType));
			}
		}

		// Token: 0x0600405E RID: 16478 RVA: 0x0017B464 File Offset: 0x00179664
		internal void EndFrame()
		{
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.requested = false;
			}
			if (!this.debugOverlayActive)
			{
				this.DestroyDebugOverlayTarget();
			}
			this.debugOverlay = DebugOverlay.None;
		}

		// Token: 0x040039D6 RID: 14806
		public LightMeterMonitor lightMeter;

		// Token: 0x040039D7 RID: 14807
		public HistogramMonitor histogram;

		// Token: 0x040039D8 RID: 14808
		public WaveformMonitor waveform;

		// Token: 0x040039D9 RID: 14809
		public VectorscopeMonitor vectorscope;

		// Token: 0x040039DA RID: 14810
		private Dictionary<MonitorType, Monitor> m_Monitors;

		// Token: 0x040039DB RID: 14811
		private int frameWidth;

		// Token: 0x040039DC RID: 14812
		private int frameHeight;

		// Token: 0x040039E0 RID: 14816
		public PostProcessDebugLayer.OverlaySettings overlaySettings;

		// Token: 0x02000F40 RID: 3904
		[Serializable]
		public class OverlaySettings
		{
			// Token: 0x04004F5F RID: 20319
			public bool linearDepth;

			// Token: 0x04004F60 RID: 20320
			[Range(0f, 16f)]
			public float motionColorIntensity = 4f;

			// Token: 0x04004F61 RID: 20321
			[Range(4f, 128f)]
			public int motionGridSize = 64;

			// Token: 0x04004F62 RID: 20322
			public ColorBlindnessType colorBlindnessType;

			// Token: 0x04004F63 RID: 20323
			[Range(0f, 1f)]
			public float colorBlindnessStrength = 1f;
		}
	}
}
