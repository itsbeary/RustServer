using System;
using UnityEngine;

// Token: 0x02000992 RID: 2450
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[ImageEffectAllowedInSceneView]
public class Explosion_Bloom : MonoBehaviour
{
	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x06003A31 RID: 14897 RVA: 0x001578DA File Offset: 0x00155ADA
	public Shader shader
	{
		get
		{
			if (this.m_Shader == null)
			{
				this.m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
			}
			return this.m_Shader;
		}
	}

	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x06003A32 RID: 14898 RVA: 0x00157900 File Offset: 0x00155B00
	public Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = Explosion_Bloom.CheckShaderAndCreateMaterial(this.shader);
			}
			return this.m_Material;
		}
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x00157928 File Offset: 0x00155B28
	public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
	{
		if (s == null || !s.isSupported)
		{
			Debug.LogWarningFormat("Missing shader for image effect {0}", new object[] { effect });
			return false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[] { effect });
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[] { effect });
			return false;
		}
		if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[] { effect });
			return false;
		}
		return true;
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x001579BC File Offset: 0x00155BBC
	public static Material CheckShaderAndCreateMaterial(Shader s)
	{
		if (s == null || !s.isSupported)
		{
			return null;
		}
		return new Material(s)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x06003A35 RID: 14901 RVA: 0x001579DF File Offset: 0x00155BDF
	public static bool supportsDX11
	{
		get
		{
			return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
		}
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x001579F4 File Offset: 0x00155BF4
	private void Awake()
	{
		this.m_Threshold = Shader.PropertyToID("_Threshold");
		this.m_Curve = Shader.PropertyToID("_Curve");
		this.m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
		this.m_SampleScale = Shader.PropertyToID("_SampleScale");
		this.m_Intensity = Shader.PropertyToID("_Intensity");
		this.m_BaseTex = Shader.PropertyToID("_BaseTex");
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x00157A61 File Offset: 0x00155C61
	private void OnEnable()
	{
		if (!Explosion_Bloom.IsSupported(this.shader, true, false, this))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x00157A7A File Offset: 0x00155C7A
	private void OnDisable()
	{
		if (this.m_Material != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
		}
		this.m_Material = null;
	}

	// Token: 0x06003A39 RID: 14905 RVA: 0x00157A9C File Offset: 0x00155C9C
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bool isMobilePlatform = Application.isMobilePlatform;
		int num = source.width;
		int num2 = source.height;
		if (!this.settings.highQuality)
		{
			num /= 2;
			num2 /= 2;
		}
		RenderTextureFormat renderTextureFormat = (isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);
		float num3 = Mathf.Log((float)num2, 2f) + this.settings.radius - 8f;
		int num4 = (int)num3;
		int num5 = Mathf.Clamp(num4, 1, 16);
		float thresholdLinear = this.settings.thresholdLinear;
		this.material.SetFloat(this.m_Threshold, thresholdLinear);
		float num6 = thresholdLinear * this.settings.softKnee + 1E-05f;
		Vector3 vector = new Vector3(thresholdLinear - num6, num6 * 2f, 0.25f / num6);
		this.material.SetVector(this.m_Curve, vector);
		bool flag = !this.settings.highQuality && this.settings.antiFlicker;
		this.material.SetFloat(this.m_PrefilterOffs, flag ? (-0.5f) : 0f);
		this.material.SetFloat(this.m_SampleScale, 0.5f + num3 - (float)num4);
		this.material.SetFloat(this.m_Intensity, Mathf.Max(0f, this.settings.intensity));
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, renderTextureFormat);
		Graphics.Blit(source, temporary, this.material, this.settings.antiFlicker ? 1 : 0);
		RenderTexture renderTexture = temporary;
		for (int i = 0; i < num5; i++)
		{
			this.m_blurBuffer1[i] = RenderTexture.GetTemporary(renderTexture.width / 2, renderTexture.height / 2, 0, renderTextureFormat);
			Graphics.Blit(renderTexture, this.m_blurBuffer1[i], this.material, (i == 0) ? (this.settings.antiFlicker ? 3 : 2) : 4);
			renderTexture = this.m_blurBuffer1[i];
		}
		for (int j = num5 - 2; j >= 0; j--)
		{
			RenderTexture renderTexture2 = this.m_blurBuffer1[j];
			this.material.SetTexture(this.m_BaseTex, renderTexture2);
			this.m_blurBuffer2[j] = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, renderTextureFormat);
			Graphics.Blit(renderTexture, this.m_blurBuffer2[j], this.material, this.settings.highQuality ? 6 : 5);
			renderTexture = this.m_blurBuffer2[j];
		}
		int num7 = 7;
		num7 += (this.settings.highQuality ? 1 : 0);
		this.material.SetTexture(this.m_BaseTex, source);
		Graphics.Blit(renderTexture, destination, this.material, num7);
		for (int k = 0; k < 16; k++)
		{
			if (this.m_blurBuffer1[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer1[k]);
			}
			if (this.m_blurBuffer2[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer2[k]);
			}
			this.m_blurBuffer1[k] = null;
			this.m_blurBuffer2[k] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x040034D5 RID: 13525
	[SerializeField]
	public Explosion_Bloom.Settings settings = Explosion_Bloom.Settings.defaultSettings;

	// Token: 0x040034D6 RID: 13526
	[SerializeField]
	[HideInInspector]
	private Shader m_Shader;

	// Token: 0x040034D7 RID: 13527
	private Material m_Material;

	// Token: 0x040034D8 RID: 13528
	private const int kMaxIterations = 16;

	// Token: 0x040034D9 RID: 13529
	private RenderTexture[] m_blurBuffer1 = new RenderTexture[16];

	// Token: 0x040034DA RID: 13530
	private RenderTexture[] m_blurBuffer2 = new RenderTexture[16];

	// Token: 0x040034DB RID: 13531
	private int m_Threshold;

	// Token: 0x040034DC RID: 13532
	private int m_Curve;

	// Token: 0x040034DD RID: 13533
	private int m_PrefilterOffs;

	// Token: 0x040034DE RID: 13534
	private int m_SampleScale;

	// Token: 0x040034DF RID: 13535
	private int m_Intensity;

	// Token: 0x040034E0 RID: 13536
	private int m_BaseTex;

	// Token: 0x02000EE3 RID: 3811
	[Serializable]
	public struct Settings
	{
		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x0600539D RID: 21405 RVA: 0x001B2E9C File Offset: 0x001B109C
		// (set) Token: 0x0600539C RID: 21404 RVA: 0x001B2E93 File Offset: 0x001B1093
		public float thresholdGamma
		{
			get
			{
				return Mathf.Max(0f, this.threshold);
			}
			set
			{
				this.threshold = value;
			}
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x0600539F RID: 21407 RVA: 0x001B2EBC File Offset: 0x001B10BC
		// (set) Token: 0x0600539E RID: 21406 RVA: 0x001B2EAE File Offset: 0x001B10AE
		public float thresholdLinear
		{
			get
			{
				return Mathf.GammaToLinearSpace(this.thresholdGamma);
			}
			set
			{
				this.threshold = Mathf.LinearToGammaSpace(value);
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x060053A0 RID: 21408 RVA: 0x001B2ECC File Offset: 0x001B10CC
		public static Explosion_Bloom.Settings defaultSettings
		{
			get
			{
				return new Explosion_Bloom.Settings
				{
					threshold = 2f,
					softKnee = 0f,
					radius = 7f,
					intensity = 0.7f,
					highQuality = true,
					antiFlicker = true
				};
			}
		}

		// Token: 0x04004DB9 RID: 19897
		[SerializeField]
		[Tooltip("Filters out pixels under this level of brightness.")]
		public float threshold;

		// Token: 0x04004DBA RID: 19898
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Makes transition between under/over-threshold gradual.")]
		public float softKnee;

		// Token: 0x04004DBB RID: 19899
		[SerializeField]
		[Range(1f, 7f)]
		[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
		public float radius;

		// Token: 0x04004DBC RID: 19900
		[SerializeField]
		[Tooltip("Blend factor of the result image.")]
		public float intensity;

		// Token: 0x04004DBD RID: 19901
		[SerializeField]
		[Tooltip("Controls filter quality and buffer resolution.")]
		public bool highQuality;

		// Token: 0x04004DBE RID: 19902
		[SerializeField]
		[Tooltip("Reduces flashing noise with an additional filter.")]
		public bool antiFlicker;
	}
}
