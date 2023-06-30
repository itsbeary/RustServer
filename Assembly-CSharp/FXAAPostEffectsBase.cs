using System;
using UnityEngine;

// Token: 0x02000991 RID: 2449
public class FXAAPostEffectsBase : MonoBehaviour
{
	// Token: 0x06003A24 RID: 14884 RVA: 0x001573B8 File Offset: 0x001555B8
	public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + this.ToString());
			base.enabled = false;
			return null;
		}
		if (s.isSupported && m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			this.NotSupported();
			Debug.LogError(string.Concat(new string[]
			{
				"The shader ",
				s.ToString(),
				" on effect ",
				this.ToString(),
				" is not supported on this platform!"
			}));
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x00157470 File Offset: 0x00155670
	private Material CreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + this.ToString());
			return null;
		}
		if (m2Create && m2Create.shader == s && s.isSupported)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x001574E1 File Offset: 0x001556E1
	private void OnEnable()
	{
		this.isSupported = true;
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x001574EA File Offset: 0x001556EA
	private bool CheckSupport()
	{
		return this.CheckSupport(false);
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x001574F3 File Offset: 0x001556F3
	private bool CheckResources()
	{
		Debug.LogWarning("CheckResources () for " + this.ToString() + " should be overwritten.");
		return this.isSupported;
	}

	// Token: 0x06003A29 RID: 14889 RVA: 0x00157515 File Offset: 0x00155715
	private void Start()
	{
		this.CheckResources();
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x00157520 File Offset: 0x00155720
	public bool CheckSupport(bool needDepth)
	{
		this.isSupported = true;
		this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.NotSupported();
			return false;
		}
		if (needDepth)
		{
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
		return true;
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x00157580 File Offset: 0x00155780
	private bool CheckSupport(bool needDepth, bool needHdr)
	{
		if (!this.CheckSupport(needDepth))
		{
			return false;
		}
		if (needHdr && !this.supportHDRTextures)
		{
			this.NotSupported();
			return false;
		}
		return true;
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x001575A1 File Offset: 0x001557A1
	private void ReportAutoDisable()
	{
		Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}

	// Token: 0x06003A2D RID: 14893 RVA: 0x001575C0 File Offset: 0x001557C0
	private bool CheckShader(Shader s)
	{
		Debug.Log(string.Concat(new string[]
		{
			"The shader ",
			s.ToString(),
			" on effect ",
			this.ToString(),
			" is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."
		}));
		if (!s.isSupported)
		{
			this.NotSupported();
			return false;
		}
		return false;
	}

	// Token: 0x06003A2E RID: 14894 RVA: 0x00157618 File Offset: 0x00155818
	private void NotSupported()
	{
		base.enabled = false;
		this.isSupported = false;
	}

	// Token: 0x06003A2F RID: 14895 RVA: 0x00157628 File Offset: 0x00155828
	private void DrawBorder(RenderTexture dest, Material material)
	{
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float num;
			float num2;
			if (flag)
			{
				num = 1f;
				num2 = 0f;
			}
			else
			{
				num = 0f;
				num2 = 1f;
			}
			float num3 = 0f;
			float num4 = 0f + 1f / ((float)dest.width * 1f);
			float num5 = 0f;
			float num6 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, num);
			GL.Vertex3(num3, num5, 0.1f);
			GL.TexCoord2(1f, num);
			GL.Vertex3(num4, num5, 0.1f);
			GL.TexCoord2(1f, num2);
			GL.Vertex3(num4, num6, 0.1f);
			GL.TexCoord2(0f, num2);
			GL.Vertex3(num3, num6, 0.1f);
			float num7 = 1f - 1f / ((float)dest.width * 1f);
			num4 = 1f;
			num5 = 0f;
			num6 = 1f;
			GL.TexCoord2(0f, num);
			GL.Vertex3(num7, num5, 0.1f);
			GL.TexCoord2(1f, num);
			GL.Vertex3(num4, num5, 0.1f);
			GL.TexCoord2(1f, num2);
			GL.Vertex3(num4, num6, 0.1f);
			GL.TexCoord2(0f, num2);
			GL.Vertex3(num7, num6, 0.1f);
			float num8 = 0f;
			num4 = 1f;
			num5 = 0f;
			num6 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, num);
			GL.Vertex3(num8, num5, 0.1f);
			GL.TexCoord2(1f, num);
			GL.Vertex3(num4, num5, 0.1f);
			GL.TexCoord2(1f, num2);
			GL.Vertex3(num4, num6, 0.1f);
			GL.TexCoord2(0f, num2);
			GL.Vertex3(num8, num6, 0.1f);
			float num9 = 0f;
			num4 = 1f;
			num5 = 1f - 1f / ((float)dest.height * 1f);
			num6 = 1f;
			GL.TexCoord2(0f, num);
			GL.Vertex3(num9, num5, 0.1f);
			GL.TexCoord2(1f, num);
			GL.Vertex3(num4, num5, 0.1f);
			GL.TexCoord2(1f, num2);
			GL.Vertex3(num4, num6, 0.1f);
			GL.TexCoord2(0f, num2);
			GL.Vertex3(num9, num6, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	// Token: 0x040034D3 RID: 13523
	protected bool supportHDRTextures = true;

	// Token: 0x040034D4 RID: 13524
	protected bool isSupported = true;
}
