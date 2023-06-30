using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AAA RID: 2730
	internal class TextureLerper
	{
		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06004112 RID: 16658 RVA: 0x0017EF40 File Offset: 0x0017D140
		internal static TextureLerper instance
		{
			get
			{
				if (TextureLerper.m_Instance == null)
				{
					TextureLerper.m_Instance = new TextureLerper();
				}
				return TextureLerper.m_Instance;
			}
		}

		// Token: 0x06004113 RID: 16659 RVA: 0x0017EF58 File Offset: 0x0017D158
		private TextureLerper()
		{
			this.m_Recycled = new List<RenderTexture>();
			this.m_Actives = new List<RenderTexture>();
		}

		// Token: 0x06004114 RID: 16660 RVA: 0x0017EF76 File Offset: 0x0017D176
		internal void BeginFrame(PostProcessRenderContext context)
		{
			this.m_Command = context.command;
			this.m_PropertySheets = context.propertySheets;
			this.m_Resources = context.resources;
		}

		// Token: 0x06004115 RID: 16661 RVA: 0x0017EF9C File Offset: 0x0017D19C
		internal void EndFrame()
		{
			if (this.m_Recycled.Count > 0)
			{
				foreach (RenderTexture renderTexture in this.m_Recycled)
				{
					RuntimeUtilities.Destroy(renderTexture);
				}
				this.m_Recycled.Clear();
			}
			if (this.m_Actives.Count > 0)
			{
				foreach (RenderTexture renderTexture2 in this.m_Actives)
				{
					this.m_Recycled.Add(renderTexture2);
				}
				this.m_Actives.Clear();
			}
		}

		// Token: 0x06004116 RID: 16662 RVA: 0x0017F068 File Offset: 0x0017D268
		private RenderTexture Get(RenderTextureFormat format, int w, int h, int d = 1, bool enableRandomWrite = false, bool force3D = false)
		{
			RenderTexture renderTexture = null;
			int count = this.m_Recycled.Count;
			int i;
			for (i = 0; i < count; i++)
			{
				RenderTexture renderTexture2 = this.m_Recycled[i];
				if (renderTexture2.width == w && renderTexture2.height == h && renderTexture2.volumeDepth == d && renderTexture2.format == format && renderTexture2.enableRandomWrite == enableRandomWrite && (!force3D || renderTexture2.dimension == TextureDimension.Tex3D))
				{
					renderTexture = renderTexture2;
					break;
				}
			}
			if (renderTexture == null)
			{
				TextureDimension textureDimension = ((d > 1 || force3D) ? TextureDimension.Tex3D : TextureDimension.Tex2D);
				renderTexture = new RenderTexture(w, h, 0, format)
				{
					dimension = textureDimension,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0,
					volumeDepth = d,
					enableRandomWrite = enableRandomWrite
				};
				renderTexture.Create();
			}
			else
			{
				this.m_Recycled.RemoveAt(i);
			}
			this.m_Actives.Add(renderTexture);
			return renderTexture;
		}

		// Token: 0x06004117 RID: 16663 RVA: 0x0017F150 File Offset: 0x0017D350
		internal Texture Lerp(Texture from, Texture to, float t)
		{
			Assert.IsNotNull<Texture>(from);
			Assert.IsNotNull<Texture>(to);
			Assert.AreEqual(from.width, to.width);
			Assert.AreEqual(from.height, to.height);
			if (from == to)
			{
				return from;
			}
			if (t <= 0f)
			{
				return from;
			}
			if (t >= 1f)
			{
				return to;
			}
			RenderTexture renderTexture;
			if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
			{
				int num = ((from is Texture3D) ? ((Texture3D)from).depth : ((RenderTexture)from).volumeDepth);
				int num2 = Mathf.Max(Mathf.Max(from.width, from.height), num);
				renderTexture = this.Get(RenderTextureFormat.ARGBHalf, from.width, from.height, num, true, true);
				ComputeShader texture3dLerp = this.m_Resources.computeShaders.texture3dLerp;
				int num3 = texture3dLerp.FindKernel("KTexture3DLerp");
				this.m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4((float)from.width, (float)from.height, (float)num, t));
				this.m_Command.SetComputeTextureParam(texture3dLerp, num3, "_Output", renderTexture);
				this.m_Command.SetComputeTextureParam(texture3dLerp, num3, "_From", from);
				this.m_Command.SetComputeTextureParam(texture3dLerp, num3, "_To", to);
				uint num4;
				uint num5;
				uint num6;
				texture3dLerp.GetKernelThreadGroupSizes(num3, out num4, out num5, out num6);
				Assert.AreEqual(num4, num5);
				int num7 = Mathf.CeilToInt((float)num2 / num4);
				int num8 = Mathf.CeilToInt((float)num2 / num6);
				this.m_Command.DispatchCompute(texture3dLerp, num3, num7, num7, num8);
				return renderTexture;
			}
			RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(to);
			renderTexture = this.Get(uncompressedRenderTextureFormat, to.width, to.height, 1, false, false);
			PropertySheet propertySheet = this.m_PropertySheets.Get(this.m_Resources.shaders.texture2dLerp);
			propertySheet.properties.SetTexture(ShaderIDs.To, to);
			propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
			this.m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 0, false, null);
			return renderTexture;
		}

		// Token: 0x06004118 RID: 16664 RVA: 0x0017F384 File Offset: 0x0017D584
		internal Texture Lerp(Texture from, Color to, float t)
		{
			Assert.IsNotNull<Texture>(from);
			if ((double)t < 1E-05)
			{
				return from;
			}
			RenderTexture renderTexture;
			if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
			{
				int num = ((from is Texture3D) ? ((Texture3D)from).depth : ((RenderTexture)from).volumeDepth);
				float num2 = (float)Mathf.Max(Mathf.Max(from.width, from.height), num);
				renderTexture = this.Get(RenderTextureFormat.ARGBHalf, from.width, from.height, num, true, true);
				ComputeShader texture3dLerp = this.m_Resources.computeShaders.texture3dLerp;
				int num3 = texture3dLerp.FindKernel("KTexture3DLerpToColor");
				this.m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4((float)from.width, (float)from.height, (float)num, t));
				this.m_Command.SetComputeVectorParam(texture3dLerp, "_TargetColor", new Vector4(to.r, to.g, to.b, to.a));
				this.m_Command.SetComputeTextureParam(texture3dLerp, num3, "_Output", renderTexture);
				this.m_Command.SetComputeTextureParam(texture3dLerp, num3, "_From", from);
				int num4 = Mathf.CeilToInt(num2 / 4f);
				this.m_Command.DispatchCompute(texture3dLerp, num3, num4, num4, num4);
				return renderTexture;
			}
			RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(from);
			renderTexture = this.Get(uncompressedRenderTextureFormat, from.width, from.height, 1, false, false);
			PropertySheet propertySheet = this.m_PropertySheets.Get(this.m_Resources.shaders.texture2dLerp);
			propertySheet.properties.SetVector(ShaderIDs.TargetColor, new Vector4(to.r, to.g, to.b, to.a));
			propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
			this.m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 1, false, null);
			return renderTexture;
		}

		// Token: 0x06004119 RID: 16665 RVA: 0x0017F58C File Offset: 0x0017D78C
		internal void Clear()
		{
			foreach (RenderTexture renderTexture in this.m_Actives)
			{
				RuntimeUtilities.Destroy(renderTexture);
			}
			foreach (RenderTexture renderTexture2 in this.m_Recycled)
			{
				RuntimeUtilities.Destroy(renderTexture2);
			}
			this.m_Actives.Clear();
			this.m_Recycled.Clear();
		}

		// Token: 0x04003AB7 RID: 15031
		private static TextureLerper m_Instance;

		// Token: 0x04003AB8 RID: 15032
		private CommandBuffer m_Command;

		// Token: 0x04003AB9 RID: 15033
		private PropertySheetFactory m_PropertySheets;

		// Token: 0x04003ABA RID: 15034
		private PostProcessResources m_Resources;

		// Token: 0x04003ABB RID: 15035
		private List<RenderTexture> m_Recycled;

		// Token: 0x04003ABC RID: 15036
		private List<RenderTexture> m_Actives;
	}
}
