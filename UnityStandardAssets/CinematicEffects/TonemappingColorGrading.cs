using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000A1E RID: 2590
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Cinematic/Tonemapping and Color Grading")]
	[ImageEffectAllowedInSceneView]
	public class TonemappingColorGrading : MonoBehaviour
	{
		// Token: 0x040037B4 RID: 14260
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.EyeAdaptationSettings m_EyeAdaptation = TonemappingColorGrading.EyeAdaptationSettings.defaultSettings;

		// Token: 0x040037B5 RID: 14261
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.TonemappingSettings m_Tonemapping = TonemappingColorGrading.TonemappingSettings.defaultSettings;

		// Token: 0x040037B6 RID: 14262
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.ColorGradingSettings m_ColorGrading = TonemappingColorGrading.ColorGradingSettings.defaultSettings;

		// Token: 0x040037B7 RID: 14263
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.LUTSettings m_Lut = TonemappingColorGrading.LUTSettings.defaultSettings;

		// Token: 0x040037B8 RID: 14264
		[SerializeField]
		private Shader m_Shader;

		// Token: 0x02000F0C RID: 3852
		[AttributeUsage(AttributeTargets.Field)]
		public class SettingsGroup : Attribute
		{
		}

		// Token: 0x02000F0D RID: 3853
		public class IndentedGroup : PropertyAttribute
		{
		}

		// Token: 0x02000F0E RID: 3854
		public class ChannelMixer : PropertyAttribute
		{
		}

		// Token: 0x02000F0F RID: 3855
		public class ColorWheelGroup : PropertyAttribute
		{
			// Token: 0x06005420 RID: 21536 RVA: 0x001B4C47 File Offset: 0x001B2E47
			public ColorWheelGroup()
			{
			}

			// Token: 0x06005421 RID: 21537 RVA: 0x001B4C62 File Offset: 0x001B2E62
			public ColorWheelGroup(int minSizePerWheel, int maxSizePerWheel)
			{
				this.minSizePerWheel = minSizePerWheel;
				this.maxSizePerWheel = maxSizePerWheel;
			}

			// Token: 0x04004E88 RID: 20104
			public int minSizePerWheel = 60;

			// Token: 0x04004E89 RID: 20105
			public int maxSizePerWheel = 150;
		}

		// Token: 0x02000F10 RID: 3856
		public class Curve : PropertyAttribute
		{
			// Token: 0x06005422 RID: 21538 RVA: 0x001B4C8B File Offset: 0x001B2E8B
			public Curve()
			{
			}

			// Token: 0x06005423 RID: 21539 RVA: 0x001B4C9E File Offset: 0x001B2E9E
			public Curve(float r, float g, float b, float a)
			{
				this.color = new Color(r, g, b, a);
			}

			// Token: 0x04004E8A RID: 20106
			public Color color = Color.white;
		}

		// Token: 0x02000F11 RID: 3857
		[Serializable]
		public struct EyeAdaptationSettings
		{
			// Token: 0x1700071E RID: 1822
			// (get) Token: 0x06005424 RID: 21540 RVA: 0x001B4CC4 File Offset: 0x001B2EC4
			public static TonemappingColorGrading.EyeAdaptationSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.EyeAdaptationSettings
					{
						enabled = false,
						showDebug = false,
						middleGrey = 0.5f,
						min = -3f,
						max = 3f,
						speed = 1.5f
					};
				}
			}

			// Token: 0x04004E8B RID: 20107
			public bool enabled;

			// Token: 0x04004E8C RID: 20108
			[Min(0f)]
			[Tooltip("Midpoint Adjustment.")]
			public float middleGrey;

			// Token: 0x04004E8D RID: 20109
			[Tooltip("The lowest possible exposure value; adjust this value to modify the brightest areas of your level.")]
			public float min;

			// Token: 0x04004E8E RID: 20110
			[Tooltip("The highest possible exposure value; adjust this value to modify the darkest areas of your level.")]
			public float max;

			// Token: 0x04004E8F RID: 20111
			[Min(0f)]
			[Tooltip("Speed of linear adaptation. Higher is faster.")]
			public float speed;

			// Token: 0x04004E90 RID: 20112
			[Tooltip("Displays a luminosity helper in the GameView.")]
			public bool showDebug;
		}

		// Token: 0x02000F12 RID: 3858
		public enum Tonemapper
		{
			// Token: 0x04004E92 RID: 20114
			ACES,
			// Token: 0x04004E93 RID: 20115
			Curve,
			// Token: 0x04004E94 RID: 20116
			Hable,
			// Token: 0x04004E95 RID: 20117
			HejlDawson,
			// Token: 0x04004E96 RID: 20118
			Photographic,
			// Token: 0x04004E97 RID: 20119
			Reinhard,
			// Token: 0x04004E98 RID: 20120
			Neutral
		}

		// Token: 0x02000F13 RID: 3859
		[Serializable]
		public struct TonemappingSettings
		{
			// Token: 0x1700071F RID: 1823
			// (get) Token: 0x06005425 RID: 21541 RVA: 0x001B4D1C File Offset: 0x001B2F1C
			public static TonemappingColorGrading.TonemappingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.TonemappingSettings
					{
						enabled = false,
						tonemapper = TonemappingColorGrading.Tonemapper.Neutral,
						exposure = 1f,
						curve = TonemappingColorGrading.CurvesSettings.defaultCurve,
						neutralBlackIn = 0.02f,
						neutralWhiteIn = 10f,
						neutralBlackOut = 0f,
						neutralWhiteOut = 10f,
						neutralWhiteLevel = 5.3f,
						neutralWhiteClip = 10f
					};
				}
			}

			// Token: 0x04004E99 RID: 20121
			public bool enabled;

			// Token: 0x04004E9A RID: 20122
			[Tooltip("Tonemapping technique to use. ACES is the recommended one.")]
			public TonemappingColorGrading.Tonemapper tonemapper;

			// Token: 0x04004E9B RID: 20123
			[Min(0f)]
			[Tooltip("Adjusts the overall exposure of the scene.")]
			public float exposure;

			// Token: 0x04004E9C RID: 20124
			[Tooltip("Custom tonemapping curve.")]
			public AnimationCurve curve;

			// Token: 0x04004E9D RID: 20125
			[Range(-0.1f, 0.1f)]
			public float neutralBlackIn;

			// Token: 0x04004E9E RID: 20126
			[Range(1f, 20f)]
			public float neutralWhiteIn;

			// Token: 0x04004E9F RID: 20127
			[Range(-0.09f, 0.1f)]
			public float neutralBlackOut;

			// Token: 0x04004EA0 RID: 20128
			[Range(1f, 19f)]
			public float neutralWhiteOut;

			// Token: 0x04004EA1 RID: 20129
			[Range(0.1f, 20f)]
			public float neutralWhiteLevel;

			// Token: 0x04004EA2 RID: 20130
			[Range(1f, 10f)]
			public float neutralWhiteClip;
		}

		// Token: 0x02000F14 RID: 3860
		[Serializable]
		public struct LUTSettings
		{
			// Token: 0x17000720 RID: 1824
			// (get) Token: 0x06005426 RID: 21542 RVA: 0x001B4DA4 File Offset: 0x001B2FA4
			public static TonemappingColorGrading.LUTSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.LUTSettings
					{
						enabled = false,
						texture = null,
						contribution = 1f
					};
				}
			}

			// Token: 0x04004EA3 RID: 20131
			public bool enabled;

			// Token: 0x04004EA4 RID: 20132
			[Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
			public Texture texture;

			// Token: 0x04004EA5 RID: 20133
			[Range(0f, 1f)]
			[Tooltip("Blending factor.")]
			public float contribution;
		}

		// Token: 0x02000F15 RID: 3861
		[Serializable]
		public struct ColorWheelsSettings
		{
			// Token: 0x17000721 RID: 1825
			// (get) Token: 0x06005427 RID: 21543 RVA: 0x001B4DD8 File Offset: 0x001B2FD8
			public static TonemappingColorGrading.ColorWheelsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorWheelsSettings
					{
						shadows = Color.white,
						midtones = Color.white,
						highlights = Color.white
					};
				}
			}

			// Token: 0x04004EA6 RID: 20134
			[ColorUsage(false)]
			public Color shadows;

			// Token: 0x04004EA7 RID: 20135
			[ColorUsage(false)]
			public Color midtones;

			// Token: 0x04004EA8 RID: 20136
			[ColorUsage(false)]
			public Color highlights;
		}

		// Token: 0x02000F16 RID: 3862
		[Serializable]
		public struct BasicsSettings
		{
			// Token: 0x17000722 RID: 1826
			// (get) Token: 0x06005428 RID: 21544 RVA: 0x001B4E14 File Offset: 0x001B3014
			public static TonemappingColorGrading.BasicsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.BasicsSettings
					{
						temperatureShift = 0f,
						tint = 0f,
						contrast = 1f,
						hue = 0f,
						saturation = 1f,
						value = 1f,
						vibrance = 0f,
						gain = 1f,
						gamma = 1f
					};
				}
			}

			// Token: 0x04004EA9 RID: 20137
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to a custom color temperature.")]
			public float temperatureShift;

			// Token: 0x04004EAA RID: 20138
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
			public float tint;

			// Token: 0x04004EAB RID: 20139
			[Space]
			[Range(-0.5f, 0.5f)]
			[Tooltip("Shift the hue of all colors.")]
			public float hue;

			// Token: 0x04004EAC RID: 20140
			[Range(0f, 2f)]
			[Tooltip("Pushes the intensity of all colors.")]
			public float saturation;

			// Token: 0x04004EAD RID: 20141
			[Range(-1f, 1f)]
			[Tooltip("Adjusts the saturation so that clipping is minimized as colors approach full saturation.")]
			public float vibrance;

			// Token: 0x04004EAE RID: 20142
			[Range(0f, 10f)]
			[Tooltip("Brightens or darkens all colors.")]
			public float value;

			// Token: 0x04004EAF RID: 20143
			[Space]
			[Range(0f, 2f)]
			[Tooltip("Expands or shrinks the overall range of tonal values.")]
			public float contrast;

			// Token: 0x04004EB0 RID: 20144
			[Range(0.01f, 5f)]
			[Tooltip("Contrast gain curve. Controls the steepness of the curve.")]
			public float gain;

			// Token: 0x04004EB1 RID: 20145
			[Range(0.01f, 5f)]
			[Tooltip("Applies a pow function to the source.")]
			public float gamma;
		}

		// Token: 0x02000F17 RID: 3863
		[Serializable]
		public struct ChannelMixerSettings
		{
			// Token: 0x17000723 RID: 1827
			// (get) Token: 0x06005429 RID: 21545 RVA: 0x001B4E98 File Offset: 0x001B3098
			public static TonemappingColorGrading.ChannelMixerSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ChannelMixerSettings
					{
						currentChannel = 0,
						channels = new Vector3[]
						{
							new Vector3(1f, 0f, 0f),
							new Vector3(0f, 1f, 0f),
							new Vector3(0f, 0f, 1f)
						}
					};
				}
			}

			// Token: 0x04004EB2 RID: 20146
			public int currentChannel;

			// Token: 0x04004EB3 RID: 20147
			public Vector3[] channels;
		}

		// Token: 0x02000F18 RID: 3864
		[Serializable]
		public struct CurvesSettings
		{
			// Token: 0x17000724 RID: 1828
			// (get) Token: 0x0600542A RID: 21546 RVA: 0x001B4F14 File Offset: 0x001B3114
			public static TonemappingColorGrading.CurvesSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.CurvesSettings
					{
						master = TonemappingColorGrading.CurvesSettings.defaultCurve,
						red = TonemappingColorGrading.CurvesSettings.defaultCurve,
						green = TonemappingColorGrading.CurvesSettings.defaultCurve,
						blue = TonemappingColorGrading.CurvesSettings.defaultCurve
					};
				}
			}

			// Token: 0x17000725 RID: 1829
			// (get) Token: 0x0600542B RID: 21547 RVA: 0x001B4F5C File Offset: 0x001B315C
			public static AnimationCurve defaultCurve
			{
				get
				{
					return new AnimationCurve(new Keyframe[]
					{
						new Keyframe(0f, 0f, 1f, 1f),
						new Keyframe(1f, 1f, 1f, 1f)
					});
				}
			}

			// Token: 0x04004EB4 RID: 20148
			[TonemappingColorGrading.Curve]
			public AnimationCurve master;

			// Token: 0x04004EB5 RID: 20149
			[TonemappingColorGrading.Curve(1f, 0f, 0f, 1f)]
			public AnimationCurve red;

			// Token: 0x04004EB6 RID: 20150
			[TonemappingColorGrading.Curve(0f, 1f, 0f, 1f)]
			public AnimationCurve green;

			// Token: 0x04004EB7 RID: 20151
			[TonemappingColorGrading.Curve(0f, 1f, 1f, 1f)]
			public AnimationCurve blue;
		}

		// Token: 0x02000F19 RID: 3865
		public enum ColorGradingPrecision
		{
			// Token: 0x04004EB9 RID: 20153
			Normal = 16,
			// Token: 0x04004EBA RID: 20154
			High = 32
		}

		// Token: 0x02000F1A RID: 3866
		[Serializable]
		public struct ColorGradingSettings
		{
			// Token: 0x17000726 RID: 1830
			// (get) Token: 0x0600542C RID: 21548 RVA: 0x001B4FB4 File Offset: 0x001B31B4
			public static TonemappingColorGrading.ColorGradingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorGradingSettings
					{
						enabled = false,
						useDithering = false,
						showDebug = false,
						precision = TonemappingColorGrading.ColorGradingPrecision.Normal,
						colorWheels = TonemappingColorGrading.ColorWheelsSettings.defaultSettings,
						basics = TonemappingColorGrading.BasicsSettings.defaultSettings,
						channelMixer = TonemappingColorGrading.ChannelMixerSettings.defaultSettings,
						curves = TonemappingColorGrading.CurvesSettings.defaultSettings
					};
				}
			}

			// Token: 0x0600542D RID: 21549 RVA: 0x001B501B File Offset: 0x001B321B
			internal void Reset()
			{
				this.curves = TonemappingColorGrading.CurvesSettings.defaultSettings;
			}

			// Token: 0x04004EBB RID: 20155
			public bool enabled;

			// Token: 0x04004EBC RID: 20156
			[Tooltip("Internal LUT precision. \"Normal\" is 256x16, \"High\" is 1024x32. Prefer \"Normal\" on mobile devices.")]
			public TonemappingColorGrading.ColorGradingPrecision precision;

			// Token: 0x04004EBD RID: 20157
			[Space]
			[TonemappingColorGrading.ColorWheelGroup]
			public TonemappingColorGrading.ColorWheelsSettings colorWheels;

			// Token: 0x04004EBE RID: 20158
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.BasicsSettings basics;

			// Token: 0x04004EBF RID: 20159
			[Space]
			[TonemappingColorGrading.ChannelMixer]
			public TonemappingColorGrading.ChannelMixerSettings channelMixer;

			// Token: 0x04004EC0 RID: 20160
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.CurvesSettings curves;

			// Token: 0x04004EC1 RID: 20161
			[Space]
			[Tooltip("Use dithering to try and minimize color banding in dark areas.")]
			public bool useDithering;

			// Token: 0x04004EC2 RID: 20162
			[Tooltip("Displays the generated LUT in the top left corner of the GameView.")]
			public bool showDebug;
		}
	}
}
