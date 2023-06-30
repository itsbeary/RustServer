using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A47 RID: 2631
	public class PostProcessRenderContext
	{
		// Token: 0x06003F23 RID: 16163 RVA: 0x00172DA8 File Offset: 0x00170FA8
		public void Resize(int width, int height, bool dlssEnabled)
		{
			this.screenWidth = width;
			this.width = width;
			this.screenHeight = height;
			this.height = height;
			this.dlssEnabled = dlssEnabled;
			this.m_sourceDescriptor.width = width;
			this.m_sourceDescriptor.height = height;
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06003F24 RID: 16164 RVA: 0x00172DF4 File Offset: 0x00170FF4
		// (set) Token: 0x06003F25 RID: 16165 RVA: 0x00172DFC File Offset: 0x00170FFC
		public Camera camera
		{
			get
			{
				return this.m_Camera;
			}
			set
			{
				this.m_Camera = value;
				if (!this.m_Camera.stereoEnabled)
				{
					this.width = this.m_Camera.pixelWidth;
					this.height = this.m_Camera.pixelHeight;
					this.m_sourceDescriptor.width = this.width;
					this.m_sourceDescriptor.height = this.height;
					this.screenWidth = this.width;
					this.screenHeight = this.height;
					this.stereoActive = false;
					this.numberOfEyes = 1;
				}
			}
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06003F26 RID: 16166 RVA: 0x00172E87 File Offset: 0x00171087
		// (set) Token: 0x06003F27 RID: 16167 RVA: 0x00172E8F File Offset: 0x0017108F
		public CommandBuffer command { get; set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06003F28 RID: 16168 RVA: 0x00172E98 File Offset: 0x00171098
		// (set) Token: 0x06003F29 RID: 16169 RVA: 0x00172EA0 File Offset: 0x001710A0
		public RenderTargetIdentifier source { get; set; }

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06003F2A RID: 16170 RVA: 0x00172EA9 File Offset: 0x001710A9
		// (set) Token: 0x06003F2B RID: 16171 RVA: 0x00172EB1 File Offset: 0x001710B1
		public RenderTargetIdentifier destination { get; set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06003F2C RID: 16172 RVA: 0x00172EBA File Offset: 0x001710BA
		// (set) Token: 0x06003F2D RID: 16173 RVA: 0x00172EC2 File Offset: 0x001710C2
		public RenderTextureFormat sourceFormat { get; set; }

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06003F2E RID: 16174 RVA: 0x00172ECB File Offset: 0x001710CB
		// (set) Token: 0x06003F2F RID: 16175 RVA: 0x00172ED3 File Offset: 0x001710D3
		public bool flip { get; set; }

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06003F30 RID: 16176 RVA: 0x00172EDC File Offset: 0x001710DC
		// (set) Token: 0x06003F31 RID: 16177 RVA: 0x00172EE4 File Offset: 0x001710E4
		public PostProcessResources resources { get; internal set; }

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06003F32 RID: 16178 RVA: 0x00172EED File Offset: 0x001710ED
		// (set) Token: 0x06003F33 RID: 16179 RVA: 0x00172EF5 File Offset: 0x001710F5
		public PropertySheetFactory propertySheets { get; internal set; }

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06003F34 RID: 16180 RVA: 0x00172EFE File Offset: 0x001710FE
		// (set) Token: 0x06003F35 RID: 16181 RVA: 0x00172F06 File Offset: 0x00171106
		public Dictionary<string, object> userData { get; private set; }

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06003F36 RID: 16182 RVA: 0x00172F0F File Offset: 0x0017110F
		// (set) Token: 0x06003F37 RID: 16183 RVA: 0x00172F17 File Offset: 0x00171117
		public PostProcessDebugLayer debugLayer { get; internal set; }

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06003F38 RID: 16184 RVA: 0x00172F20 File Offset: 0x00171120
		// (set) Token: 0x06003F39 RID: 16185 RVA: 0x00172F28 File Offset: 0x00171128
		public int width { get; set; }

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06003F3A RID: 16186 RVA: 0x00172F31 File Offset: 0x00171131
		// (set) Token: 0x06003F3B RID: 16187 RVA: 0x00172F39 File Offset: 0x00171139
		public int height { get; set; }

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06003F3C RID: 16188 RVA: 0x00172F42 File Offset: 0x00171142
		// (set) Token: 0x06003F3D RID: 16189 RVA: 0x00172F4A File Offset: 0x0017114A
		public bool stereoActive { get; private set; }

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06003F3E RID: 16190 RVA: 0x00172F53 File Offset: 0x00171153
		// (set) Token: 0x06003F3F RID: 16191 RVA: 0x00172F5B File Offset: 0x0017115B
		public int xrActiveEye { get; private set; }

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06003F40 RID: 16192 RVA: 0x00172F64 File Offset: 0x00171164
		// (set) Token: 0x06003F41 RID: 16193 RVA: 0x00172F6C File Offset: 0x0017116C
		public int numberOfEyes { get; private set; }

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06003F42 RID: 16194 RVA: 0x00172F75 File Offset: 0x00171175
		// (set) Token: 0x06003F43 RID: 16195 RVA: 0x00172F7D File Offset: 0x0017117D
		public PostProcessRenderContext.StereoRenderingMode stereoRenderingMode { get; private set; }

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06003F44 RID: 16196 RVA: 0x00172F86 File Offset: 0x00171186
		// (set) Token: 0x06003F45 RID: 16197 RVA: 0x00172F8E File Offset: 0x0017118E
		public int screenWidth { get; set; }

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06003F46 RID: 16198 RVA: 0x00172F97 File Offset: 0x00171197
		// (set) Token: 0x06003F47 RID: 16199 RVA: 0x00172F9F File Offset: 0x0017119F
		public int screenHeight { get; set; }

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06003F48 RID: 16200 RVA: 0x00172FA8 File Offset: 0x001711A8
		// (set) Token: 0x06003F49 RID: 16201 RVA: 0x00172FB0 File Offset: 0x001711B0
		public bool isSceneView { get; internal set; }

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06003F4A RID: 16202 RVA: 0x00172FB9 File Offset: 0x001711B9
		// (set) Token: 0x06003F4B RID: 16203 RVA: 0x00172FC1 File Offset: 0x001711C1
		public PostProcessLayer.Antialiasing antialiasing { get; internal set; }

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06003F4C RID: 16204 RVA: 0x00172FCA File Offset: 0x001711CA
		// (set) Token: 0x06003F4D RID: 16205 RVA: 0x00172FD2 File Offset: 0x001711D2
		public TemporalAntialiasing temporalAntialiasing { get; internal set; }

		// Token: 0x06003F4E RID: 16206 RVA: 0x00172FDC File Offset: 0x001711DC
		public void Reset()
		{
			this.m_Camera = null;
			this.width = 0;
			this.height = 0;
			this.dlssEnabled = false;
			this.m_sourceDescriptor = new RenderTextureDescriptor(0, 0);
			this.physicalCamera = false;
			this.stereoActive = false;
			this.xrActiveEye = 0;
			this.screenWidth = 0;
			this.screenHeight = 0;
			this.command = null;
			this.source = 0;
			this.destination = 0;
			this.sourceFormat = RenderTextureFormat.ARGB32;
			this.flip = false;
			this.resources = null;
			this.propertySheets = null;
			this.debugLayer = null;
			this.isSceneView = false;
			this.antialiasing = PostProcessLayer.Antialiasing.None;
			this.temporalAntialiasing = null;
			this.uberSheet = null;
			this.autoExposureTexture = null;
			this.logLut = null;
			this.autoExposure = null;
			this.bloomBufferNameID = -1;
			if (this.userData == null)
			{
				this.userData = new Dictionary<string, object>();
			}
			this.userData.Clear();
		}

		// Token: 0x06003F4F RID: 16207 RVA: 0x001730CD File Offset: 0x001712CD
		public bool IsTemporalAntialiasingActive()
		{
			return this.antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !this.isSceneView && this.temporalAntialiasing.IsSupported();
		}

		// Token: 0x06003F50 RID: 16208 RVA: 0x001730ED File Offset: 0x001712ED
		public bool IsDebugOverlayEnabled(DebugOverlay overlay)
		{
			return this.debugLayer.debugOverlay == overlay;
		}

		// Token: 0x06003F51 RID: 16209 RVA: 0x001730FD File Offset: 0x001712FD
		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			this.debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
		}

		// Token: 0x06003F52 RID: 16210 RVA: 0x00173110 File Offset: 0x00171310
		private RenderTextureDescriptor GetDescriptor(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default)
		{
			RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(this.m_sourceDescriptor.width, this.m_sourceDescriptor.height, this.m_sourceDescriptor.colorFormat, depthBufferBits);
			renderTextureDescriptor.dimension = this.m_sourceDescriptor.dimension;
			renderTextureDescriptor.volumeDepth = this.m_sourceDescriptor.volumeDepth;
			renderTextureDescriptor.vrUsage = this.m_sourceDescriptor.vrUsage;
			renderTextureDescriptor.msaaSamples = this.m_sourceDescriptor.msaaSamples;
			renderTextureDescriptor.memoryless = this.m_sourceDescriptor.memoryless;
			renderTextureDescriptor.useMipMap = this.m_sourceDescriptor.useMipMap;
			renderTextureDescriptor.autoGenerateMips = this.m_sourceDescriptor.autoGenerateMips;
			renderTextureDescriptor.enableRandomWrite = this.m_sourceDescriptor.enableRandomWrite;
			renderTextureDescriptor.shadowSamplingMode = this.m_sourceDescriptor.shadowSamplingMode;
			if (colorFormat != RenderTextureFormat.Default)
			{
				renderTextureDescriptor.colorFormat = colorFormat;
			}
			if (readWrite == RenderTextureReadWrite.sRGB)
			{
				renderTextureDescriptor.sRGB = true;
			}
			else if (readWrite == RenderTextureReadWrite.Linear)
			{
				renderTextureDescriptor.sRGB = false;
			}
			else if (readWrite == RenderTextureReadWrite.Default)
			{
				renderTextureDescriptor.sRGB = QualitySettings.activeColorSpace > ColorSpace.Gamma;
			}
			return renderTextureDescriptor;
		}

		// Token: 0x06003F53 RID: 16211 RVA: 0x00173224 File Offset: 0x00171424
		public void GetScreenSpaceTemporaryRT(CommandBuffer cmd, int nameID, int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, FilterMode filter = FilterMode.Bilinear, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = this.GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			if (this.stereoActive && descriptor.dimension == TextureDimension.Tex2DArray)
			{
				descriptor.dimension = TextureDimension.Tex2D;
			}
			cmd.GetTemporaryRT(nameID, descriptor, filter);
		}

		// Token: 0x06003F54 RID: 16212 RVA: 0x00173280 File Offset: 0x00171480
		public RenderTexture GetScreenSpaceTemporaryRT(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = this.GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			return RenderTexture.GetTemporary(descriptor);
		}

		// Token: 0x0400389C RID: 14492
		public bool dlssEnabled;

		// Token: 0x0400389D RID: 14493
		private Camera m_Camera;

		// Token: 0x040038B2 RID: 14514
		internal PropertySheet uberSheet;

		// Token: 0x040038B3 RID: 14515
		internal Texture autoExposureTexture;

		// Token: 0x040038B4 RID: 14516
		internal LogHistogram logHistogram;

		// Token: 0x040038B5 RID: 14517
		internal Texture logLut;

		// Token: 0x040038B6 RID: 14518
		internal AutoExposure autoExposure;

		// Token: 0x040038B7 RID: 14519
		internal int bloomBufferNameID;

		// Token: 0x040038B8 RID: 14520
		internal bool physicalCamera;

		// Token: 0x040038B9 RID: 14521
		private RenderTextureDescriptor m_sourceDescriptor;

		// Token: 0x02000F30 RID: 3888
		public enum StereoRenderingMode
		{
			// Token: 0x04004F06 RID: 20230
			MultiPass,
			// Token: 0x04004F07 RID: 20231
			SinglePass,
			// Token: 0x04004F08 RID: 20232
			SinglePassInstanced,
			// Token: 0x04004F09 RID: 20233
			SinglePassMultiview
		}
	}
}
