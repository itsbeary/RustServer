using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x020009B0 RID: 2480
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class BeamGeometry : MonoBehaviour
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06003B08 RID: 15112 RVA: 0x0015D4C5 File Offset: 0x0015B6C5
		// (set) Token: 0x06003B09 RID: 15113 RVA: 0x0015D4CD File Offset: 0x0015B6CD
		public MeshRenderer meshRenderer { get; private set; }

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06003B0A RID: 15114 RVA: 0x0015D4D6 File Offset: 0x0015B6D6
		// (set) Token: 0x06003B0B RID: 15115 RVA: 0x0015D4DE File Offset: 0x0015B6DE
		public MeshFilter meshFilter { get; private set; }

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06003B0C RID: 15116 RVA: 0x0015D4E7 File Offset: 0x0015B6E7
		// (set) Token: 0x06003B0D RID: 15117 RVA: 0x0015D4EF File Offset: 0x0015B6EF
		public Material material { get; private set; }

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06003B0E RID: 15118 RVA: 0x0015D4F8 File Offset: 0x0015B6F8
		// (set) Token: 0x06003B0F RID: 15119 RVA: 0x0015D500 File Offset: 0x0015B700
		public Mesh coneMesh { get; private set; }

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06003B10 RID: 15120 RVA: 0x0015D509 File Offset: 0x0015B709
		// (set) Token: 0x06003B11 RID: 15121 RVA: 0x0015D516 File Offset: 0x0015B716
		public bool visible
		{
			get
			{
				return this.meshRenderer.enabled;
			}
			set
			{
				this.meshRenderer.enabled = value;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06003B12 RID: 15122 RVA: 0x0015D524 File Offset: 0x0015B724
		// (set) Token: 0x06003B13 RID: 15123 RVA: 0x0015D531 File Offset: 0x0015B731
		public int sortingLayerID
		{
			get
			{
				return this.meshRenderer.sortingLayerID;
			}
			set
			{
				this.meshRenderer.sortingLayerID = value;
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06003B14 RID: 15124 RVA: 0x0015D53F File Offset: 0x0015B73F
		// (set) Token: 0x06003B15 RID: 15125 RVA: 0x0015D54C File Offset: 0x0015B74C
		public int sortingOrder
		{
			get
			{
				return this.meshRenderer.sortingOrder;
			}
			set
			{
				this.meshRenderer.sortingOrder = value;
			}
		}

		// Token: 0x06003B16 RID: 15126 RVA: 0x000063A5 File Offset: 0x000045A5
		private void Start()
		{
		}

		// Token: 0x06003B17 RID: 15127 RVA: 0x0015D55A File Offset: 0x0015B75A
		private void OnDestroy()
		{
			if (this.material)
			{
				UnityEngine.Object.DestroyImmediate(this.material);
				this.material = null;
			}
		}

		// Token: 0x06003B18 RID: 15128 RVA: 0x0015D57B File Offset: 0x0015B77B
		private static bool IsUsingCustomRenderPipeline()
		{
			return RenderPipelineManager.currentPipeline != null || GraphicsSettings.renderPipelineAsset != null;
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x0015D591 File Offset: 0x0015B791
		private void OnEnable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
			}
		}

		// Token: 0x06003B1A RID: 15130 RVA: 0x0015D5AB File Offset: 0x0015B7AB
		private void OnDisable()
		{
			if (BeamGeometry.IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
			}
		}

		// Token: 0x06003B1B RID: 15131 RVA: 0x0015D5C8 File Offset: 0x0015B7C8
		public void Initialize(VolumetricLightBeam master, Shader shader)
		{
			HideFlags proceduralObjectsHideFlags = Consts.ProceduralObjectsHideFlags;
			this.m_Master = master;
			base.transform.SetParent(master.transform, false);
			this.material = new Material(shader);
			this.material.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer = base.gameObject.GetOrAddComponent<MeshRenderer>();
			this.meshRenderer.hideFlags = proceduralObjectsHideFlags;
			this.meshRenderer.material = this.material;
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if (SortingLayer.IsValid(this.m_Master.sortingLayerID))
			{
				this.sortingLayerID = this.m_Master.sortingLayerID;
			}
			else
			{
				Debug.LogError(string.Format("Beam '{0}' has an invalid sortingLayerID ({1}). Please fix it by setting a valid layer.", Utils.GetPath(this.m_Master.transform), this.m_Master.sortingLayerID));
			}
			this.sortingOrder = this.m_Master.sortingOrder;
			this.meshFilter = base.gameObject.GetOrAddComponent<MeshFilter>();
			this.meshFilter.hideFlags = proceduralObjectsHideFlags;
			base.gameObject.hideFlags = proceduralObjectsHideFlags;
		}

		// Token: 0x06003B1C RID: 15132 RVA: 0x0015D6EC File Offset: 0x0015B8EC
		public void RegenerateMesh()
		{
			Debug.Assert(this.m_Master);
			base.gameObject.layer = Config.Instance.geometryLayerID;
			base.gameObject.tag = Config.Instance.geometryTag;
			if (this.coneMesh && this.m_CurrentMeshType == MeshType.Custom)
			{
				UnityEngine.Object.DestroyImmediate(this.coneMesh);
			}
			this.m_CurrentMeshType = this.m_Master.geomMeshType;
			MeshType geomMeshType = this.m_Master.geomMeshType;
			if (geomMeshType != MeshType.Shared)
			{
				if (geomMeshType == MeshType.Custom)
				{
					this.coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, this.m_Master.geomCustomSides, this.m_Master.geomCustomSegments, this.m_Master.geomCap);
					this.coneMesh.hideFlags = Consts.ProceduralObjectsHideFlags;
					this.meshFilter.mesh = this.coneMesh;
				}
				else
				{
					Debug.LogError("Unsupported MeshType");
				}
			}
			else
			{
				this.coneMesh = GlobalMesh.mesh;
				this.meshFilter.sharedMesh = this.coneMesh;
			}
			this.UpdateMaterialAndBounds();
		}

		// Token: 0x06003B1D RID: 15133 RVA: 0x0015D804 File Offset: 0x0015BA04
		private void ComputeLocalMatrix()
		{
			float num = Mathf.Max(this.m_Master.coneRadiusStart, this.m_Master.coneRadiusEnd);
			base.transform.localScale = new Vector3(num, num, this.m_Master.fadeEnd);
		}

		// Token: 0x06003B1E RID: 15134 RVA: 0x0015D84C File Offset: 0x0015BA4C
		public void UpdateMaterialAndBounds()
		{
			Debug.Assert(this.m_Master);
			this.material.renderQueue = Config.Instance.geometryRenderQueue;
			float num = this.m_Master.coneAngle * 0.017453292f / 2f;
			this.material.SetVector("_ConeSlopeCosSin", new Vector2(Mathf.Cos(num), Mathf.Sin(num)));
			Vector2 vector = new Vector2(Mathf.Max(this.m_Master.coneRadiusStart, 0.0001f), Mathf.Max(this.m_Master.coneRadiusEnd, 0.0001f));
			this.material.SetVector("_ConeRadius", vector);
			float num2 = Mathf.Sign(this.m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(this.m_Master.coneApexOffsetZ), 0.0001f);
			this.material.SetFloat("_ConeApexOffsetZ", num2);
			if (this.m_Master.colorMode == ColorMode.Gradient)
			{
				Utils.FloatPackingPrecision floatPackingPrecision = Utils.GetFloatPackingPrecision();
				this.material.EnableKeyword((floatPackingPrecision == Utils.FloatPackingPrecision.High) ? "VLB_COLOR_GRADIENT_MATRIX_HIGH" : "VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.m_ColorGradientMatrix = this.m_Master.colorGradient.SampleInMatrix((int)floatPackingPrecision);
			}
			else
			{
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_HIGH");
				this.material.DisableKeyword("VLB_COLOR_GRADIENT_MATRIX_LOW");
				this.material.SetColor("_ColorFlat", this.m_Master.color);
			}
			if (Consts.BlendingMode_AlphaAsBlack[this.m_Master.blendingModeAsInt])
			{
				this.material.EnableKeyword("ALPHA_AS_BLACK");
			}
			else
			{
				this.material.DisableKeyword("ALPHA_AS_BLACK");
			}
			this.material.SetInt("_BlendSrcFactor", (int)Consts.BlendingMode_SrcFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetInt("_BlendDstFactor", (int)Consts.BlendingMode_DstFactor[this.m_Master.blendingModeAsInt]);
			this.material.SetFloat("_AlphaInside", this.m_Master.alphaInside);
			this.material.SetFloat("_AlphaOutside", this.m_Master.alphaOutside);
			this.material.SetFloat("_AttenuationLerpLinearQuad", this.m_Master.attenuationLerpLinearQuad);
			this.material.SetFloat("_DistanceFadeStart", this.m_Master.fadeStart);
			this.material.SetFloat("_DistanceFadeEnd", this.m_Master.fadeEnd);
			this.material.SetFloat("_DistanceCamClipping", this.m_Master.cameraClippingDistance);
			this.material.SetFloat("_FresnelPow", Mathf.Max(0.001f, this.m_Master.fresnelPow));
			this.material.SetFloat("_GlareBehind", this.m_Master.glareBehind);
			this.material.SetFloat("_GlareFrontal", this.m_Master.glareFrontal);
			this.material.SetFloat("_DrawCap", (float)(this.m_Master.geomCap ? 1 : 0));
			if (this.m_Master.depthBlendDistance > 0f)
			{
				this.material.EnableKeyword("VLB_DEPTH_BLEND");
				this.material.SetFloat("_DepthBlendDistance", this.m_Master.depthBlendDistance);
			}
			else
			{
				this.material.DisableKeyword("VLB_DEPTH_BLEND");
			}
			if (this.m_Master.noiseEnabled && this.m_Master.noiseIntensity > 0f && Noise3D.isSupported)
			{
				Noise3D.LoadIfNeeded();
				this.material.EnableKeyword("VLB_NOISE_3D");
				this.material.SetVector("_NoiseLocal", new Vector4(this.m_Master.noiseVelocityLocal.x, this.m_Master.noiseVelocityLocal.y, this.m_Master.noiseVelocityLocal.z, this.m_Master.noiseScaleLocal));
				this.material.SetVector("_NoiseParam", new Vector3(this.m_Master.noiseIntensity, this.m_Master.noiseVelocityUseGlobal ? 1f : 0f, this.m_Master.noiseScaleUseGlobal ? 1f : 0f));
			}
			else
			{
				this.material.DisableKeyword("VLB_NOISE_3D");
			}
			this.ComputeLocalMatrix();
		}

		// Token: 0x06003B1F RID: 15135 RVA: 0x0015DCAC File Offset: 0x0015BEAC
		public void SetClippingPlane(Plane planeWS)
		{
			Vector3 normal = planeWS.normal;
			this.material.EnableKeyword("VLB_CLIPPING_PLANE");
			this.material.SetVector("_ClippingPlaneWS", new Vector4(normal.x, normal.y, normal.z, planeWS.distance));
		}

		// Token: 0x06003B20 RID: 15136 RVA: 0x0015DCFF File Offset: 0x0015BEFF
		public void SetClippingPlaneOff()
		{
			this.material.DisableKeyword("VLB_CLIPPING_PLANE");
		}

		// Token: 0x06003B21 RID: 15137 RVA: 0x0015DD11 File Offset: 0x0015BF11
		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
		{
			this.UpdateCameraRelatedProperties(cam);
		}

		// Token: 0x06003B22 RID: 15138 RVA: 0x0015DD1C File Offset: 0x0015BF1C
		private void OnWillRenderObject()
		{
			if (!BeamGeometry.IsUsingCustomRenderPipeline())
			{
				Camera current = Camera.current;
				if (current != null)
				{
					this.UpdateCameraRelatedProperties(current);
				}
			}
		}

		// Token: 0x06003B23 RID: 15139 RVA: 0x0015DD48 File Offset: 0x0015BF48
		private void UpdateCameraRelatedProperties(Camera cam)
		{
			if (cam && this.m_Master)
			{
				if (this.material)
				{
					Vector3 vector = this.m_Master.transform.InverseTransformPoint(cam.transform.position);
					this.material.SetVector("_CameraPosObjectSpace", vector);
					Vector3 normalized = base.transform.InverseTransformDirection(cam.transform.forward).normalized;
					float num = (cam.orthographic ? (-1f) : this.m_Master.GetInsideBeamFactorFromObjectSpacePos(vector));
					this.material.SetVector("_CameraParams", new Vector4(normalized.x, normalized.y, normalized.z, num));
					if (this.m_Master.colorMode == ColorMode.Gradient)
					{
						this.material.SetMatrix("_ColorGradientMatrix", this.m_ColorGradientMatrix);
					}
				}
				if (this.m_Master.depthBlendDistance > 0f)
				{
					cam.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
		}

		// Token: 0x040035D5 RID: 13781
		private VolumetricLightBeam m_Master;

		// Token: 0x040035D6 RID: 13782
		private Matrix4x4 m_ColorGradientMatrix;

		// Token: 0x040035D7 RID: 13783
		private MeshType m_CurrentMeshType;
	}
}
