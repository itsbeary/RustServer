using System;
using UnityEngine;

// Token: 0x020009AD RID: 2477
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[AddComponentMenu("Rendering/Visualize Texture Density")]
public class VisualizeTexelDensity : MonoBehaviour
{
	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06003AF8 RID: 15096 RVA: 0x0015CD94 File Offset: 0x0015AF94
	public static VisualizeTexelDensity Instance
	{
		get
		{
			return VisualizeTexelDensity.instance;
		}
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x0015CD9B File Offset: 0x0015AF9B
	private void Awake()
	{
		VisualizeTexelDensity.instance = this;
		this.mainCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06003AFA RID: 15098 RVA: 0x0015CDAF File Offset: 0x0015AFAF
	private void OnEnable()
	{
		this.mainCamera = base.GetComponent<Camera>();
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		this.LoadResources();
		this.initialized = true;
	}

	// Token: 0x06003AFB RID: 15099 RVA: 0x0015CDE0 File Offset: 0x0015AFE0
	private void OnDisable()
	{
		this.SafeDestroyViewTexelDensity();
		this.SafeDestroyViewTexelDensityRT();
		this.initialized = false;
	}

	// Token: 0x06003AFC RID: 15100 RVA: 0x0015CDF8 File Offset: 0x0015AFF8
	private void LoadResources()
	{
		if (this.texelDensityGradTex == null)
		{
			this.texelDensityGradTex = Resources.Load("TexelDensityGrad") as Texture;
		}
		if (this.texelDensityOverlayMat == null)
		{
			this.texelDensityOverlayMat = new Material(Shader.Find("Hidden/TexelDensityOverlay"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
	}

	// Token: 0x06003AFD RID: 15101 RVA: 0x0015CE54 File Offset: 0x0015B054
	private void SafeDestroyViewTexelDensity()
	{
		if (this.texelDensityCamera != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texelDensityCamera.gameObject);
			this.texelDensityCamera = null;
		}
		if (this.texelDensityGradTex != null)
		{
			Resources.UnloadAsset(this.texelDensityGradTex);
			this.texelDensityGradTex = null;
		}
		if (this.texelDensityOverlayMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texelDensityOverlayMat);
			this.texelDensityOverlayMat = null;
		}
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x0015CEC6 File Offset: 0x0015B0C6
	private void SafeDestroyViewTexelDensityRT()
	{
		if (this.texelDensityRT != null)
		{
			Graphics.SetRenderTarget(null);
			this.texelDensityRT.Release();
			UnityEngine.Object.DestroyImmediate(this.texelDensityRT);
			this.texelDensityRT = null;
		}
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x0015CEFC File Offset: 0x0015B0FC
	private void UpdateViewTexelDensity(bool screenResized)
	{
		if (this.texelDensityCamera == null)
		{
			GameObject gameObject = new GameObject("Texel Density Camera", new Type[] { typeof(Camera) })
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.transform.parent = this.mainCamera.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			this.texelDensityCamera = gameObject.GetComponent<Camera>();
			this.texelDensityCamera.CopyFrom(this.mainCamera);
			this.texelDensityCamera.renderingPath = RenderingPath.Forward;
			this.texelDensityCamera.allowMSAA = false;
			this.texelDensityCamera.allowHDR = false;
			this.texelDensityCamera.clearFlags = CameraClearFlags.Skybox;
			this.texelDensityCamera.depthTextureMode = DepthTextureMode.None;
			this.texelDensityCamera.SetReplacementShader(this.shader, this.shaderTag);
			this.texelDensityCamera.enabled = false;
		}
		if (this.texelDensityRT == null || screenResized || !this.texelDensityRT.IsCreated())
		{
			this.texelDensityCamera.targetTexture = null;
			this.SafeDestroyViewTexelDensityRT();
			this.texelDensityRT = new RenderTexture(this.screenWidth, this.screenHeight, 24, RenderTextureFormat.ARGB32)
			{
				hideFlags = HideFlags.DontSave
			};
			this.texelDensityRT.name = "TexelDensityRT";
			this.texelDensityRT.filterMode = FilterMode.Point;
			this.texelDensityRT.wrapMode = TextureWrapMode.Clamp;
			this.texelDensityRT.Create();
		}
		if (this.texelDensityCamera.targetTexture != this.texelDensityRT)
		{
			this.texelDensityCamera.targetTexture = this.texelDensityRT;
		}
		Shader.SetGlobalFloat("global_TexelsPerMeter", (float)this.texelsPerMeter);
		Shader.SetGlobalTexture("global_TexelDensityGrad", this.texelDensityGradTex);
		this.texelDensityCamera.fieldOfView = this.mainCamera.fieldOfView;
		this.texelDensityCamera.nearClipPlane = this.mainCamera.nearClipPlane;
		this.texelDensityCamera.farClipPlane = this.mainCamera.farClipPlane;
		this.texelDensityCamera.cullingMask = this.mainCamera.cullingMask;
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x0015D119 File Offset: 0x0015B319
	private bool CheckScreenResized(int width, int height)
	{
		if (this.screenWidth != width || this.screenHeight != height)
		{
			this.screenWidth = width;
			this.screenHeight = height;
			return true;
		}
		return false;
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x0015D140 File Offset: 0x0015B340
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.initialized)
		{
			this.UpdateViewTexelDensity(this.CheckScreenResized(source.width, source.height));
			this.texelDensityCamera.Render();
			this.texelDensityOverlayMat.SetTexture("_TexelDensityMap", this.texelDensityRT);
			this.texelDensityOverlayMat.SetFloat("_Opacity", this.overlayOpacity);
			Graphics.Blit(source, destination, this.texelDensityOverlayMat, 0);
			return;
		}
		Graphics.Blit(source, destination);
	}

	// Token: 0x06003B02 RID: 15106 RVA: 0x0015D1BC File Offset: 0x0015B3BC
	private void DrawGUIText(float x, float y, Vector2 size, string text, GUIStyle fontStyle)
	{
		fontStyle.normal.textColor = Color.black;
		GUI.Label(new Rect(x - 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y - 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x - 1f, y - 1f, size.x, size.y), text, fontStyle);
		fontStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(x, y, size.x, size.y), text, fontStyle);
	}

	// Token: 0x06003B03 RID: 15107 RVA: 0x0015D2A8 File Offset: 0x0015B4A8
	private void OnGUI()
	{
		if (this.initialized && this.showHUD)
		{
			string text = "Texels Per Meter";
			string text2 = "0";
			string text3 = this.texelsPerMeter.ToString();
			string text4 = (this.texelsPerMeter << 1).ToString() + "+";
			float num = (float)this.texelDensityGradTex.width;
			float num2 = (float)(this.texelDensityGradTex.height * 2);
			float num3 = (float)((Screen.width - this.texelDensityGradTex.width) / 2);
			float num4 = 32f;
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)Screen.width, (float)Screen.height, 0f);
			Graphics.DrawTexture(new Rect(num3 - 2f, num4 - 2f, num + 4f, num2 + 4f), Texture2D.whiteTexture);
			Graphics.DrawTexture(new Rect(num3, num4, num, num2), this.texelDensityGradTex);
			GL.PopMatrix();
			GUIStyle guistyle = new GUIStyle();
			guistyle.fontSize = 13;
			Vector2 vector = guistyle.CalcSize(new GUIContent(text));
			Vector2 vector2 = guistyle.CalcSize(new GUIContent(text2));
			Vector2 vector3 = guistyle.CalcSize(new GUIContent(text3));
			Vector2 vector4 = guistyle.CalcSize(new GUIContent(text4));
			this.DrawGUIText(((float)Screen.width - vector.x) / 2f, num4 - vector.y - 5f, vector, text, guistyle);
			this.DrawGUIText(num3, num4 + num2 + 6f, vector2, text2, guistyle);
			this.DrawGUIText(((float)Screen.width - vector3.x) / 2f, num4 + num2 + 6f, vector3, text3, guistyle);
			this.DrawGUIText(num3 + num - vector4.x, num4 + num2 + 6f, vector4, text4, guistyle);
		}
	}

	// Token: 0x040035C4 RID: 13764
	public Shader shader;

	// Token: 0x040035C5 RID: 13765
	public string shaderTag = "RenderType";

	// Token: 0x040035C6 RID: 13766
	[Range(1f, 1024f)]
	public int texelsPerMeter = 256;

	// Token: 0x040035C7 RID: 13767
	[Range(0f, 1f)]
	public float overlayOpacity = 0.5f;

	// Token: 0x040035C8 RID: 13768
	public bool showHUD = true;

	// Token: 0x040035C9 RID: 13769
	private Camera mainCamera;

	// Token: 0x040035CA RID: 13770
	private bool initialized;

	// Token: 0x040035CB RID: 13771
	private int screenWidth;

	// Token: 0x040035CC RID: 13772
	private int screenHeight;

	// Token: 0x040035CD RID: 13773
	private Camera texelDensityCamera;

	// Token: 0x040035CE RID: 13774
	private RenderTexture texelDensityRT;

	// Token: 0x040035CF RID: 13775
	private Texture texelDensityGradTex;

	// Token: 0x040035D0 RID: 13776
	private Material texelDensityOverlayMat;

	// Token: 0x040035D1 RID: 13777
	private static VisualizeTexelDensity instance;
}
