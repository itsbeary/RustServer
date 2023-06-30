using System;
using System.Collections.Generic;
using System.IO;
using Facepunch.Utility;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch.Unity
{
	// Token: 0x02000B07 RID: 2823
	public static class RenderInfo
	{
		// Token: 0x060044C9 RID: 17609 RVA: 0x00193114 File Offset: 0x00191314
		public static void GenerateReport()
		{
			Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
			List<RenderInfo.RendererInstance> list = new List<RenderInfo.RendererInstance>();
			Renderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RenderInfo.RendererInstance rendererInstance = RenderInfo.RendererInstance.From(array2[i]);
				list.Add(rendererInstance);
			}
			string text = string.Format(Application.dataPath + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
			string text2 = JsonConvert.SerializeObject(list, Formatting.Indented);
			File.WriteAllText(text, text2);
			string text3 = Application.streamingAssetsPath + "/RenderInfo.exe";
			string text4 = "\"" + text + "\"";
			Debug.Log("Launching " + text3 + " " + text4);
			Os.StartProcess(text3, text4);
		}

		// Token: 0x02000F96 RID: 3990
		public struct RendererInstance
		{
			// Token: 0x06005517 RID: 21783 RVA: 0x001B6ECC File Offset: 0x001B50CC
			public static RenderInfo.RendererInstance From(Renderer renderer)
			{
				RenderInfo.RendererInstance rendererInstance = default(RenderInfo.RendererInstance);
				rendererInstance.IsVisible = renderer.isVisible;
				rendererInstance.CastShadows = renderer.shadowCastingMode > ShadowCastingMode.Off;
				rendererInstance.RecieveShadows = renderer.receiveShadows;
				rendererInstance.Enabled = renderer.enabled && renderer.gameObject.activeInHierarchy;
				rendererInstance.Size = renderer.bounds.size.magnitude;
				rendererInstance.Distance = Vector3.Distance(renderer.bounds.center, Camera.main.transform.position);
				rendererInstance.MaterialCount = renderer.sharedMaterials.Length;
				rendererInstance.RenderType = renderer.GetType().Name;
				BaseEntity baseEntity = renderer.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					rendererInstance.EntityName = baseEntity.PrefabName;
					if (baseEntity.net != null)
					{
						rendererInstance.EntityId = baseEntity.net.ID.Value;
					}
				}
				else
				{
					rendererInstance.ObjectName = renderer.transform.GetRecursiveName("");
				}
				if (renderer is MeshRenderer)
				{
					rendererInstance.BoneCount = 0;
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					if (component)
					{
						rendererInstance.ReadMesh(component.sharedMesh);
					}
				}
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					rendererInstance.ReadMesh(skinnedMeshRenderer.sharedMesh);
					rendererInstance.UpdateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				}
				if (renderer is ParticleSystemRenderer)
				{
					ParticleSystem component2 = renderer.GetComponent<ParticleSystem>();
					if (component2)
					{
						rendererInstance.MeshName = component2.name;
						rendererInstance.ParticleCount = component2.particleCount;
					}
				}
				return rendererInstance;
			}

			// Token: 0x06005518 RID: 21784 RVA: 0x001B707C File Offset: 0x001B527C
			public void ReadMesh(UnityEngine.Mesh mesh)
			{
				if (mesh == null)
				{
					this.MeshName = "<NULL>";
					return;
				}
				this.VertexCount = mesh.vertexCount;
				this.SubMeshCount = mesh.subMeshCount;
				this.BlendShapeCount = mesh.blendShapeCount;
				this.MeshName = mesh.name;
			}

			// Token: 0x0400509F RID: 20639
			public bool IsVisible;

			// Token: 0x040050A0 RID: 20640
			public bool CastShadows;

			// Token: 0x040050A1 RID: 20641
			public bool Enabled;

			// Token: 0x040050A2 RID: 20642
			public bool RecieveShadows;

			// Token: 0x040050A3 RID: 20643
			public float Size;

			// Token: 0x040050A4 RID: 20644
			public float Distance;

			// Token: 0x040050A5 RID: 20645
			public int BoneCount;

			// Token: 0x040050A6 RID: 20646
			public int MaterialCount;

			// Token: 0x040050A7 RID: 20647
			public int VertexCount;

			// Token: 0x040050A8 RID: 20648
			public int TriangleCount;

			// Token: 0x040050A9 RID: 20649
			public int SubMeshCount;

			// Token: 0x040050AA RID: 20650
			public int BlendShapeCount;

			// Token: 0x040050AB RID: 20651
			public string RenderType;

			// Token: 0x040050AC RID: 20652
			public string MeshName;

			// Token: 0x040050AD RID: 20653
			public string ObjectName;

			// Token: 0x040050AE RID: 20654
			public string EntityName;

			// Token: 0x040050AF RID: 20655
			public ulong EntityId;

			// Token: 0x040050B0 RID: 20656
			public bool UpdateWhenOffscreen;

			// Token: 0x040050B1 RID: 20657
			public int ParticleCount;
		}
	}
}
