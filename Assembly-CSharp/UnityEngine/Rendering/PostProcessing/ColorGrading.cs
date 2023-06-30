using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A61 RID: 2657
	[PostProcess(typeof(ColorGradingRenderer), "Unity/Color Grading", true)]
	[Serializable]
	public sealed class ColorGrading : PostProcessEffectSettings
	{
		// Token: 0x06003F82 RID: 16258 RVA: 0x0017456D File Offset: 0x0017276D
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return (this.gradingMode.value != GradingMode.External || (SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders)) && this.enabled.value;
		}

		// Token: 0x04003902 RID: 14594
		[DisplayName("Mode")]
		[Tooltip("Select a color grading mode that fits your dynamic range and workflow. Use HDR if your camera is set to render in HDR and your target platform supports it. Use LDR for low-end mobiles or devices that don't support HDR. Use External if you prefer authoring a Log LUT in an external software.")]
		public GradingModeParameter gradingMode = new GradingModeParameter
		{
			value = GradingMode.HighDefinitionRange
		};

		// Token: 0x04003903 RID: 14595
		[DisplayName("Lookup Texture")]
		[Tooltip("A custom 3D log-encoded texture.")]
		public TextureParameter externalLut = new TextureParameter
		{
			value = null
		};

		// Token: 0x04003904 RID: 14596
		[DisplayName("Mode")]
		[Tooltip("Select a tonemapping algorithm to use at the end of the color grading process.")]
		public TonemapperParameter tonemapper = new TonemapperParameter
		{
			value = Tonemapper.None
		};

		// Token: 0x04003905 RID: 14597
		[DisplayName("Toe Strength")]
		[Range(0f, 1f)]
		[Tooltip("Affects the transition between the toe and the mid section of the curve. A value of 0 means no toe, a value of 1 means a very hard transition.")]
		public FloatParameter toneCurveToeStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003906 RID: 14598
		[DisplayName("Toe Length")]
		[Range(0f, 1f)]
		[Tooltip("Affects how much of the dynamic range is in the toe. With a small value, the toe will be very short and quickly transition into the linear section, with a larger value, the toe will be longer.")]
		public FloatParameter toneCurveToeLength = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x04003907 RID: 14599
		[DisplayName("Shoulder Strength")]
		[Range(0f, 1f)]
		[Tooltip("Affects the transition between the mid section and the shoulder of the curve. A value of 0 means no shoulder, a value of 1 means a very hard transition.")]
		public FloatParameter toneCurveShoulderStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003908 RID: 14600
		[DisplayName("Shoulder Length")]
		[Min(0f)]
		[Tooltip("Affects how many F-stops (EV) to add to the dynamic range of the curve.")]
		public FloatParameter toneCurveShoulderLength = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x04003909 RID: 14601
		[DisplayName("Shoulder Angle")]
		[Range(0f, 1f)]
		[Tooltip("Affects how much overshoot to add to the shoulder.")]
		public FloatParameter toneCurveShoulderAngle = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400390A RID: 14602
		[DisplayName("Gamma")]
		[Min(0.001f)]
		[Tooltip("Applies a gamma function to the curve.")]
		public FloatParameter toneCurveGamma = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x0400390B RID: 14603
		[DisplayName("Lookup Texture")]
		[Tooltip("Custom lookup texture (strip format, for example 256x16) to apply before the rest of the color grading operators. If none is provided, a neutral one will be generated internally.")]
		public TextureParameter ldrLut = new TextureParameter
		{
			value = null,
			defaultState = TextureParameterDefault.Lut2D
		};

		// Token: 0x0400390C RID: 14604
		[DisplayName("Contribution")]
		[Range(0f, 1f)]
		[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
		public FloatParameter ldrLutContribution = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x0400390D RID: 14605
		[DisplayName("Temperature")]
		[Range(-100f, 100f)]
		[Tooltip("Sets the white balance to a custom color temperature.")]
		public FloatParameter temperature = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400390E RID: 14606
		[DisplayName("Tint")]
		[Range(-100f, 100f)]
		[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
		public FloatParameter tint = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400390F RID: 14607
		[DisplayName("Color Filter")]
		[ColorUsage(false, true)]
		[Tooltip("Tint the render by multiplying a color.")]
		public ColorParameter colorFilter = new ColorParameter
		{
			value = Color.white
		};

		// Token: 0x04003910 RID: 14608
		[DisplayName("Hue Shift")]
		[Range(-180f, 180f)]
		[Tooltip("Shift the hue of all colors.")]
		public FloatParameter hueShift = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003911 RID: 14609
		[DisplayName("Saturation")]
		[Range(-100f, 100f)]
		[Tooltip("Pushes the intensity of all colors.")]
		public FloatParameter saturation = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003912 RID: 14610
		[DisplayName("Brightness")]
		[Range(-100f, 100f)]
		[Tooltip("Makes the image brighter or darker.")]
		public FloatParameter brightness = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003913 RID: 14611
		[DisplayName("Post-exposure (EV)")]
		[Tooltip("Adjusts the overall exposure of the scene in EV units. This is applied after the HDR effect and right before tonemapping so it won't affect previous effects in the chain.")]
		public FloatParameter postExposure = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003914 RID: 14612
		[DisplayName("Contrast")]
		[Range(-100f, 100f)]
		[Tooltip("Expands or shrinks the overall range of tonal values.")]
		public FloatParameter contrast = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003915 RID: 14613
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerRedOutRedIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x04003916 RID: 14614
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerRedOutGreenIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003917 RID: 14615
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerRedOutBlueIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003918 RID: 14616
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerGreenOutRedIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003919 RID: 14617
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerGreenOutGreenIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x0400391A RID: 14618
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerGreenOutBlueIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400391B RID: 14619
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerBlueOutRedIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400391C RID: 14620
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerBlueOutGreenIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400391D RID: 14621
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerBlueOutBlueIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x0400391E RID: 14622
		[DisplayName("Lift")]
		[Tooltip("Controls the darkest portions of the render.")]
		[Trackball(TrackballAttribute.Mode.Lift)]
		public Vector4Parameter lift = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x0400391F RID: 14623
		[DisplayName("Gamma")]
		[Tooltip("Power function that controls mid-range tones.")]
		[Trackball(TrackballAttribute.Mode.Gamma)]
		public Vector4Parameter gamma = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x04003920 RID: 14624
		[DisplayName("Gain")]
		[Tooltip("Controls the lightest portions of the render.")]
		[Trackball(TrackballAttribute.Mode.Gain)]
		public Vector4Parameter gain = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x04003921 RID: 14625
		public SplineParameter masterCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x04003922 RID: 14626
		public SplineParameter redCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x04003923 RID: 14627
		public SplineParameter greenCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x04003924 RID: 14628
		public SplineParameter blueCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x04003925 RID: 14629
		public SplineParameter hueVsHueCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f))
		};

		// Token: 0x04003926 RID: 14630
		public SplineParameter hueVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f))
		};

		// Token: 0x04003927 RID: 14631
		public SplineParameter satVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f))
		};

		// Token: 0x04003928 RID: 14632
		public SplineParameter lumVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f))
		};
	}
}
