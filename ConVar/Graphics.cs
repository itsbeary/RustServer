using System;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConVar
{
	// Token: 0x02000AC7 RID: 2759
	[ConsoleSystem.Factory("graphics")]
	public class Graphics : ConsoleSystem
	{
		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06004233 RID: 16947 RVA: 0x00187BF2 File Offset: 0x00185DF2
		// (set) Token: 0x06004234 RID: 16948 RVA: 0x00187BF9 File Offset: 0x00185DF9
		[ClientVar(Help = "The currently selected quality level")]
		public static int quality
		{
			get
			{
				return QualitySettings.GetQualityLevel();
			}
			set
			{
				int shadowcascades = Graphics.shadowcascades;
				QualitySettings.SetQualityLevel(value, true);
				Graphics.shadowcascades = shadowcascades;
			}
		}

		// Token: 0x06004235 RID: 16949 RVA: 0x00187C0C File Offset: 0x00185E0C
		public static float EnforceShadowDistanceBounds(float distance)
		{
			if (QualitySettings.shadowCascades == 1)
			{
				distance = Mathf.Clamp(distance, 100f, 100f);
			}
			else if (QualitySettings.shadowCascades == 2)
			{
				distance = Mathf.Clamp(distance, 100f, 600f);
			}
			else
			{
				distance = Mathf.Clamp(distance, 100f, 1000f);
			}
			return distance;
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06004236 RID: 16950 RVA: 0x00187C64 File Offset: 0x00185E64
		// (set) Token: 0x06004237 RID: 16951 RVA: 0x00187C6B File Offset: 0x00185E6B
		[ClientVar(Saved = true)]
		public static float shadowdistance
		{
			get
			{
				return Graphics._shadowdistance;
			}
			set
			{
				Graphics._shadowdistance = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics._shadowdistance);
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06004238 RID: 16952 RVA: 0x00187C82 File Offset: 0x00185E82
		// (set) Token: 0x06004239 RID: 16953 RVA: 0x00187C89 File Offset: 0x00185E89
		[ClientVar(Saved = true)]
		public static int shadowcascades
		{
			get
			{
				return QualitySettings.shadowCascades;
			}
			set
			{
				QualitySettings.shadowCascades = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics.shadowdistance);
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x0600423A RID: 16954 RVA: 0x00187CA0 File Offset: 0x00185EA0
		// (set) Token: 0x0600423B RID: 16955 RVA: 0x00187CA8 File Offset: 0x00185EA8
		[ClientVar(Saved = true)]
		public static int shadowquality
		{
			get
			{
				return Graphics._shadowquality;
			}
			set
			{
				Graphics._shadowquality = Mathf.Clamp(value, 0, 3);
				Graphics.shadowmode = Graphics._shadowquality + 1;
				bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_HIGH", !flag && Graphics._shadowquality == 2);
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_VERYHIGH", !flag && Graphics._shadowquality == 3);
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600423C RID: 16956 RVA: 0x00187D08 File Offset: 0x00185F08
		// (set) Token: 0x0600423D RID: 16957 RVA: 0x00187D0F File Offset: 0x00185F0F
		[ClientVar(Saved = true)]
		public static float fov
		{
			get
			{
				return Graphics._fov;
			}
			set
			{
				Graphics._fov = Mathf.Clamp(value, 70f, 90f);
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x0600423E RID: 16958 RVA: 0x00187D26 File Offset: 0x00185F26
		// (set) Token: 0x0600423F RID: 16959 RVA: 0x00187D2D File Offset: 0x00185F2D
		[ClientVar]
		public static float lodbias
		{
			get
			{
				return QualitySettings.lodBias;
			}
			set
			{
				QualitySettings.lodBias = Mathf.Clamp(value, 0.25f, 5f);
			}
		}

		// Token: 0x06004240 RID: 16960 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar(ClientAdmin = true)]
		public static void dof_focus_target(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06004241 RID: 16961 RVA: 0x00187D44 File Offset: 0x00185F44
		[ClientVar]
		public static void dof_nudge(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Graphics.dof_focus_dist += @float;
			if (Graphics.dof_focus_dist < 0f)
			{
				Graphics.dof_focus_dist = 0f;
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06004242 RID: 16962 RVA: 0x00187D80 File Offset: 0x00185F80
		// (set) Token: 0x06004243 RID: 16963 RVA: 0x00187D87 File Offset: 0x00185F87
		[ClientVar(Saved = true)]
		public static int shaderlod
		{
			get
			{
				return Shader.globalMaximumLOD;
			}
			set
			{
				Shader.globalMaximumLOD = Mathf.Clamp(value, 100, 600);
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06004244 RID: 16964 RVA: 0x00187D9B File Offset: 0x00185F9B
		// (set) Token: 0x06004245 RID: 16965 RVA: 0x00187DA2 File Offset: 0x00185FA2
		[ClientVar(Saved = true)]
		public static float uiscale
		{
			get
			{
				return Graphics._uiscale;
			}
			set
			{
				Graphics._uiscale = Mathf.Clamp(value, 0.5f, 1f);
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06004246 RID: 16966 RVA: 0x00187DB9 File Offset: 0x00185FB9
		// (set) Token: 0x06004247 RID: 16967 RVA: 0x00187DC0 File Offset: 0x00185FC0
		[ClientVar(Saved = true)]
		public static int af
		{
			get
			{
				return Graphics._anisotropic;
			}
			set
			{
				value = Mathf.Clamp(value, 1, 16);
				Texture.SetGlobalAnisotropicFilteringLimits(1, value);
				if (value <= 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Disable;
				}
				if (value > 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Enable;
				}
				Graphics._anisotropic = value;
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06004248 RID: 16968 RVA: 0x00187DEE File Offset: 0x00185FEE
		// (set) Token: 0x06004249 RID: 16969 RVA: 0x00187DF8 File Offset: 0x00185FF8
		[ClientVar(Saved = true)]
		public static int parallax
		{
			get
			{
				return Graphics._parallax;
			}
			set
			{
				if (value != 1)
				{
					if (value != 2)
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
					else
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.EnableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
				}
				else
				{
					Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				Graphics._parallax = value;
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x0600424A RID: 16970 RVA: 0x00187E53 File Offset: 0x00186053
		// (set) Token: 0x0600424B RID: 16971 RVA: 0x00187E5A File Offset: 0x0018605A
		[ClientVar(ClientAdmin = true)]
		public static bool itemskins
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.AllowApply;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.AllowApply = value;
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x0600424C RID: 16972 RVA: 0x00187E62 File Offset: 0x00186062
		// (set) Token: 0x0600424D RID: 16973 RVA: 0x00187E69 File Offset: 0x00186069
		[ClientVar]
		public static bool itemskinunload
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.AllowUnload;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.AllowUnload = value;
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x0600424E RID: 16974 RVA: 0x00187E71 File Offset: 0x00186071
		// (set) Token: 0x0600424F RID: 16975 RVA: 0x00187E78 File Offset: 0x00186078
		[ClientVar(ClientAdmin = true)]
		public static float itemskintimeout
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.DownloadTimeout;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.DownloadTimeout = value;
			}
		}

		// Token: 0x04003BBF RID: 15295
		private const float MinShadowDistance = 100f;

		// Token: 0x04003BC0 RID: 15296
		private const float MaxShadowDistance2Split = 600f;

		// Token: 0x04003BC1 RID: 15297
		private const float MaxShadowDistance4Split = 1000f;

		// Token: 0x04003BC2 RID: 15298
		private static float _shadowdistance = 1000f;

		// Token: 0x04003BC3 RID: 15299
		[ClientVar(Saved = true)]
		public static int shadowmode = 2;

		// Token: 0x04003BC4 RID: 15300
		[ClientVar(Saved = true)]
		public static int shadowlights = 1;

		// Token: 0x04003BC5 RID: 15301
		private static int _shadowquality = 1;

		// Token: 0x04003BC6 RID: 15302
		[ClientVar(Saved = true)]
		public static bool grassshadows = false;

		// Token: 0x04003BC7 RID: 15303
		[ClientVar(Saved = true)]
		public static bool contactshadows = false;

		// Token: 0x04003BC8 RID: 15304
		[ClientVar(Saved = true)]
		public static float drawdistance = 2500f;

		// Token: 0x04003BC9 RID: 15305
		private static float _fov = 75f;

		// Token: 0x04003BCA RID: 15306
		[ClientVar]
		public static bool hud = true;

		// Token: 0x04003BCB RID: 15307
		[ClientVar(Saved = true)]
		public static bool chat = true;

		// Token: 0x04003BCC RID: 15308
		[ClientVar(Saved = true)]
		public static bool branding = true;

		// Token: 0x04003BCD RID: 15309
		[ClientVar(Saved = true)]
		public static int compass = 1;

		// Token: 0x04003BCE RID: 15310
		[ClientVar(Saved = true)]
		public static bool dof = false;

		// Token: 0x04003BCF RID: 15311
		[ClientVar(Saved = true)]
		public static float dof_aper = 12f;

		// Token: 0x04003BD0 RID: 15312
		[ClientVar(Saved = true)]
		public static float dof_blur = 1f;

		// Token: 0x04003BD1 RID: 15313
		[ClientVar(Saved = true, Help = "0 = auto 1 = manual 2 = dynamic based on target")]
		public static int dof_mode = 0;

		// Token: 0x04003BD2 RID: 15314
		[ClientVar(Saved = true, Help = "distance from camera to focus on")]
		public static float dof_focus_dist = 10f;

		// Token: 0x04003BD3 RID: 15315
		[ClientVar(Saved = true)]
		public static float dof_focus_time = 0.2f;

		// Token: 0x04003BD4 RID: 15316
		[ClientVar(Saved = true, ClientAdmin = true)]
		public static bool dof_debug = false;

		// Token: 0x04003BD5 RID: 15317
		[ClientVar(Saved = true, Help = "Goes from 0 - 3, higher = more dof samples but slower perf")]
		public static int dof_kernel_count = 0;

		// Token: 0x04003BD6 RID: 15318
		public static BaseEntity dof_focus_target_entity = null;

		// Token: 0x04003BD7 RID: 15319
		[ClientVar(Saved = true, Help = "Whether to scale vm models with fov")]
		public static bool vm_fov_scale = true;

		// Token: 0x04003BD8 RID: 15320
		[ClientVar(Saved = true, Help = "FLips viewmodels horizontally (for left handed players)")]
		public static bool vm_horizontal_flip = false;

		// Token: 0x04003BD9 RID: 15321
		private static float _uiscale = 1f;

		// Token: 0x04003BDA RID: 15322
		private static int _anisotropic = 1;

		// Token: 0x04003BDB RID: 15323
		private static int _parallax = 0;
	}
}
