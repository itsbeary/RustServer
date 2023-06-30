using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x020009C2 RID: 2498
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[SelectionBase]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	public class VolumetricLightBeam : MonoBehaviour
	{
		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06003B7A RID: 15226 RVA: 0x0015F843 File Offset: 0x0015DA43
		public float coneAngle
		{
			get
			{
				return Mathf.Atan2(this.coneRadiusEnd - this.coneRadiusStart, this.fadeEnd) * 57.29578f * 2f;
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06003B7B RID: 15227 RVA: 0x0015F869 File Offset: 0x0015DA69
		public float coneRadiusEnd
		{
			get
			{
				return this.fadeEnd * Mathf.Tan(this.spotAngle * 0.017453292f * 0.5f);
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06003B7C RID: 15228 RVA: 0x0015F88C File Offset: 0x0015DA8C
		public float coneVolume
		{
			get
			{
				float num = this.coneRadiusStart;
				float coneRadiusEnd = this.coneRadiusEnd;
				return 1.0471976f * (num * num + num * coneRadiusEnd + coneRadiusEnd * coneRadiusEnd) * this.fadeEnd;
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06003B7D RID: 15229 RVA: 0x0015F8C0 File Offset: 0x0015DAC0
		public float coneApexOffsetZ
		{
			get
			{
				float num = this.coneRadiusStart / this.coneRadiusEnd;
				if (num != 1f)
				{
					return this.fadeEnd * num / (1f - num);
				}
				return float.MaxValue;
			}
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06003B7E RID: 15230 RVA: 0x0015F8F9 File Offset: 0x0015DAF9
		// (set) Token: 0x06003B7F RID: 15231 RVA: 0x0015F915 File Offset: 0x0015DB15
		public int geomSides
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSides;
				}
				return this.geomCustomSides;
			}
			set
			{
				this.geomCustomSides = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSides is OBSOLETE and has been renamed to geomCustomSides.");
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06003B80 RID: 15232 RVA: 0x0015F928 File Offset: 0x0015DB28
		// (set) Token: 0x06003B81 RID: 15233 RVA: 0x0015F944 File Offset: 0x0015DB44
		public int geomSegments
		{
			get
			{
				if (this.geomMeshType != MeshType.Custom)
				{
					return Config.Instance.sharedMeshSegments;
				}
				return this.geomCustomSegments;
			}
			set
			{
				this.geomCustomSegments = value;
				Debug.LogWarning("The setter VLB.VolumetricLightBeam.geomSegments is OBSOLETE and has been renamed to geomCustomSegments.");
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06003B82 RID: 15234 RVA: 0x0015F957 File Offset: 0x0015DB57
		public float attenuationLerpLinearQuad
		{
			get
			{
				if (this.attenuationEquation == AttenuationEquation.Linear)
				{
					return 0f;
				}
				if (this.attenuationEquation == AttenuationEquation.Quadratic)
				{
					return 1f;
				}
				return this.attenuationCustomBlending;
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06003B83 RID: 15235 RVA: 0x0015F97C File Offset: 0x0015DB7C
		// (set) Token: 0x06003B84 RID: 15236 RVA: 0x0015F984 File Offset: 0x0015DB84
		public int sortingLayerID
		{
			get
			{
				return this._SortingLayerID;
			}
			set
			{
				this._SortingLayerID = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingLayerID = value;
				}
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06003B85 RID: 15237 RVA: 0x0015F9A6 File Offset: 0x0015DBA6
		// (set) Token: 0x06003B86 RID: 15238 RVA: 0x0015F9B3 File Offset: 0x0015DBB3
		public string sortingLayerName
		{
			get
			{
				return SortingLayer.IDToName(this.sortingLayerID);
			}
			set
			{
				this.sortingLayerID = SortingLayer.NameToID(value);
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06003B87 RID: 15239 RVA: 0x0015F9C1 File Offset: 0x0015DBC1
		// (set) Token: 0x06003B88 RID: 15240 RVA: 0x0015F9C9 File Offset: 0x0015DBC9
		public int sortingOrder
		{
			get
			{
				return this._SortingOrder;
			}
			set
			{
				this._SortingOrder = value;
				if (this.m_BeamGeom)
				{
					this.m_BeamGeom.sortingOrder = value;
				}
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06003B89 RID: 15241 RVA: 0x0015F9EB File Offset: 0x0015DBEB
		// (set) Token: 0x06003B8A RID: 15242 RVA: 0x0015F9F3 File Offset: 0x0015DBF3
		public bool trackChangesDuringPlaytime
		{
			get
			{
				return this._TrackChangesDuringPlaytime;
			}
			set
			{
				this._TrackChangesDuringPlaytime = value;
				this.StartPlaytimeUpdateIfNeeded();
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06003B8B RID: 15243 RVA: 0x0015FA02 File Offset: 0x0015DC02
		public bool isCurrentlyTrackingChanges
		{
			get
			{
				return this.m_CoPlaytimeUpdate != null;
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06003B8C RID: 15244 RVA: 0x0015FA0D File Offset: 0x0015DC0D
		public bool hasGeometry
		{
			get
			{
				return this.m_BeamGeom != null;
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06003B8D RID: 15245 RVA: 0x0015FA1B File Offset: 0x0015DC1B
		public Bounds bounds
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return new Bounds(Vector3.zero, Vector3.zero);
				}
				return this.m_BeamGeom.meshRenderer.bounds;
			}
		}

		// Token: 0x06003B8E RID: 15246 RVA: 0x0015FA4B File Offset: 0x0015DC4B
		public void SetClippingPlane(Plane planeWS)
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlane(planeWS);
			}
			this.m_PlaneWS = planeWS;
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x0015FA6D File Offset: 0x0015DC6D
		public void SetClippingPlaneOff()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.SetClippingPlaneOff();
			}
			this.m_PlaneWS = default(Plane);
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x0015FA94 File Offset: 0x0015DC94
		public bool IsColliderHiddenByDynamicOccluder(Collider collider)
		{
			Debug.Assert(collider, "You should pass a valid Collider to VLB.VolumetricLightBeam.IsColliderHiddenByDynamicOccluder");
			return this.m_PlaneWS.IsValid() && !GeometryUtility.TestPlanesAABB(new Plane[] { this.m_PlaneWS }, collider.bounds);
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06003B91 RID: 15249 RVA: 0x0015FAE1 File Offset: 0x0015DCE1
		public int blendingModeAsInt
		{
			get
			{
				return Mathf.Clamp((int)this.blendingMode, 0, Enum.GetValues(typeof(BlendingMode)).Length);
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06003B92 RID: 15250 RVA: 0x0015FB03 File Offset: 0x0015DD03
		public MeshRenderer Renderer
		{
			get
			{
				if (!(this.m_BeamGeom != null))
				{
					return null;
				}
				return this.m_BeamGeom.meshRenderer;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06003B93 RID: 15251 RVA: 0x0015FB20 File Offset: 0x0015DD20
		public string meshStats
		{
			get
			{
				Mesh mesh = (this.m_BeamGeom ? this.m_BeamGeom.coneMesh : null);
				if (mesh)
				{
					return string.Format("Cone angle: {0:0.0} degrees\nMesh: {1} vertices, {2} triangles", this.coneAngle, mesh.vertexCount, mesh.triangles.Length / 3);
				}
				return "no mesh available";
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06003B94 RID: 15252 RVA: 0x0015FB86 File Offset: 0x0015DD86
		public int meshVerticesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.vertexCount;
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06003B95 RID: 15253 RVA: 0x0015FBB9 File Offset: 0x0015DDB9
		public int meshTrianglesCount
		{
			get
			{
				if (!this.m_BeamGeom || !this.m_BeamGeom.coneMesh)
				{
					return 0;
				}
				return this.m_BeamGeom.coneMesh.triangles.Length / 3;
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06003B96 RID: 15254 RVA: 0x0015FBF0 File Offset: 0x0015DDF0
		private Light lightSpotAttached
		{
			get
			{
				if (this._CachedLight == null)
				{
					this._CachedLight = base.GetComponent<Light>();
				}
				if (this._CachedLight && this._CachedLight.type == LightType.Spot)
				{
					return this._CachedLight;
				}
				return null;
			}
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x0015FC2E File Offset: 0x0015DE2E
		public float GetInsideBeamFactor(Vector3 posWS)
		{
			return this.GetInsideBeamFactorFromObjectSpacePos(base.transform.InverseTransformPoint(posWS));
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x0015FC44 File Offset: 0x0015DE44
		public float GetInsideBeamFactorFromObjectSpacePos(Vector3 posOS)
		{
			if (posOS.z < 0f)
			{
				return -1f;
			}
			Vector2 normalized = new Vector2(posOS.xy().magnitude, posOS.z + this.coneApexOffsetZ).normalized;
			return Mathf.Clamp((Mathf.Abs(Mathf.Sin(this.coneAngle * 0.017453292f / 2f)) - Mathf.Abs(normalized.x)) / 0.1f, -1f, 1f);
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x0015FCCA File Offset: 0x0015DECA
		[Obsolete("Use 'GenerateGeometry()' instead")]
		public void Generate()
		{
			this.GenerateGeometry();
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x0015FCD4 File Offset: 0x0015DED4
		public virtual void GenerateGeometry()
		{
			this.HandleBackwardCompatibility(this.pluginVersion, 1510);
			this.pluginVersion = 1510;
			this.ValidateProperties();
			if (this.m_BeamGeom == null)
			{
				Shader beamShader = Config.Instance.beamShader;
				if (!beamShader)
				{
					Debug.LogError("Invalid BeamShader set in VLB Config");
					return;
				}
				this.m_BeamGeom = Utils.NewWithComponent<BeamGeometry>("Beam Geometry");
				this.m_BeamGeom.Initialize(this, beamShader);
			}
			this.m_BeamGeom.RegenerateMesh();
			this.m_BeamGeom.visible = base.enabled;
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x0015FD68 File Offset: 0x0015DF68
		public virtual void UpdateAfterManualPropertyChange()
		{
			this.ValidateProperties();
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.UpdateMaterialAndBounds();
			}
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x0015FCCA File Offset: 0x0015DECA
		private void Start()
		{
			this.GenerateGeometry();
		}

		// Token: 0x06003B9D RID: 15261 RVA: 0x0015FD88 File Offset: 0x0015DF88
		private void OnEnable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = true;
			}
			this.StartPlaytimeUpdateIfNeeded();
		}

		// Token: 0x06003B9E RID: 15262 RVA: 0x0015FDA9 File Offset: 0x0015DFA9
		private void OnDisable()
		{
			if (this.m_BeamGeom)
			{
				this.m_BeamGeom.visible = false;
			}
			this.m_CoPlaytimeUpdate = null;
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x000063A5 File Offset: 0x000045A5
		private void StartPlaytimeUpdateIfNeeded()
		{
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x0015FDCB File Offset: 0x0015DFCB
		private IEnumerator CoPlaytimeUpdate()
		{
			while (this.trackChangesDuringPlaytime && base.enabled)
			{
				this.UpdateAfterManualPropertyChange();
				yield return null;
			}
			this.m_CoPlaytimeUpdate = null;
			yield break;
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x0015FDDA File Offset: 0x0015DFDA
		private void OnDestroy()
		{
			this.DestroyBeam();
		}

		// Token: 0x06003BA2 RID: 15266 RVA: 0x0015FDE2 File Offset: 0x0015DFE2
		private void DestroyBeam()
		{
			if (this.m_BeamGeom)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BeamGeom.gameObject);
			}
			this.m_BeamGeom = null;
		}

		// Token: 0x06003BA3 RID: 15267 RVA: 0x0015FE08 File Offset: 0x0015E008
		private void AssignPropertiesFromSpotLight(Light lightSpot)
		{
			if (lightSpot && lightSpot.type == LightType.Spot)
			{
				if (this.fadeEndFromLight)
				{
					this.fadeEnd = lightSpot.range;
				}
				if (this.spotAngleFromLight)
				{
					this.spotAngle = lightSpot.spotAngle;
				}
				if (this.colorFromLight)
				{
					this.colorMode = ColorMode.Flat;
					this.color = lightSpot.color;
				}
			}
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x0015FE68 File Offset: 0x0015E068
		private void ClampProperties()
		{
			this.alphaInside = Mathf.Clamp01(this.alphaInside);
			this.alphaOutside = Mathf.Clamp01(this.alphaOutside);
			this.attenuationCustomBlending = Mathf.Clamp01(this.attenuationCustomBlending);
			this.fadeEnd = Mathf.Max(0.01f, this.fadeEnd);
			this.fadeStart = Mathf.Clamp(this.fadeStart, 0f, this.fadeEnd - 0.01f);
			this.spotAngle = Mathf.Clamp(this.spotAngle, 0.1f, 179.9f);
			this.coneRadiusStart = Mathf.Max(this.coneRadiusStart, 0f);
			this.depthBlendDistance = Mathf.Max(this.depthBlendDistance, 0f);
			this.cameraClippingDistance = Mathf.Max(this.cameraClippingDistance, 0f);
			this.geomCustomSides = Mathf.Clamp(this.geomCustomSides, 3, 256);
			this.geomCustomSegments = Mathf.Clamp(this.geomCustomSegments, 0, 64);
			this.fresnelPow = Mathf.Max(0f, this.fresnelPow);
			this.glareBehind = Mathf.Clamp01(this.glareBehind);
			this.glareFrontal = Mathf.Clamp01(this.glareFrontal);
			this.noiseIntensity = Mathf.Clamp(this.noiseIntensity, 0f, 1f);
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x0015FFBB File Offset: 0x0015E1BB
		private void ValidateProperties()
		{
			this.AssignPropertiesFromSpotLight(this.lightSpotAttached);
			this.ClampProperties();
		}

		// Token: 0x06003BA6 RID: 15270 RVA: 0x0015FFCF File Offset: 0x0015E1CF
		private void HandleBackwardCompatibility(int serializedVersion, int newVersion)
		{
			if (serializedVersion == -1)
			{
				return;
			}
			if (serializedVersion == newVersion)
			{
				return;
			}
			if (serializedVersion < 1301)
			{
				this.attenuationEquation = AttenuationEquation.Linear;
			}
			if (serializedVersion < 1501)
			{
				this.geomMeshType = MeshType.Custom;
				this.geomCustomSegments = 5;
			}
			Utils.MarkCurrentSceneDirty();
		}

		// Token: 0x04003664 RID: 13924
		public bool colorFromLight = true;

		// Token: 0x04003665 RID: 13925
		public ColorMode colorMode;

		// Token: 0x04003666 RID: 13926
		[ColorUsage(true, true)]
		[FormerlySerializedAs("colorValue")]
		public Color color = Consts.FlatColor;

		// Token: 0x04003667 RID: 13927
		public Gradient colorGradient;

		// Token: 0x04003668 RID: 13928
		[Range(0f, 1f)]
		public float alphaInside = 1f;

		// Token: 0x04003669 RID: 13929
		[FormerlySerializedAs("alpha")]
		[Range(0f, 1f)]
		public float alphaOutside = 1f;

		// Token: 0x0400366A RID: 13930
		public BlendingMode blendingMode;

		// Token: 0x0400366B RID: 13931
		[FormerlySerializedAs("angleFromLight")]
		public bool spotAngleFromLight = true;

		// Token: 0x0400366C RID: 13932
		[Range(0.1f, 179.9f)]
		public float spotAngle = 35f;

		// Token: 0x0400366D RID: 13933
		[FormerlySerializedAs("radiusStart")]
		public float coneRadiusStart = 0.1f;

		// Token: 0x0400366E RID: 13934
		public MeshType geomMeshType;

		// Token: 0x0400366F RID: 13935
		[FormerlySerializedAs("geomSides")]
		public int geomCustomSides = 18;

		// Token: 0x04003670 RID: 13936
		public int geomCustomSegments = 5;

		// Token: 0x04003671 RID: 13937
		public bool geomCap;

		// Token: 0x04003672 RID: 13938
		public bool fadeEndFromLight = true;

		// Token: 0x04003673 RID: 13939
		public AttenuationEquation attenuationEquation = AttenuationEquation.Quadratic;

		// Token: 0x04003674 RID: 13940
		[Range(0f, 1f)]
		public float attenuationCustomBlending = 0.5f;

		// Token: 0x04003675 RID: 13941
		public float fadeStart;

		// Token: 0x04003676 RID: 13942
		public float fadeEnd = 3f;

		// Token: 0x04003677 RID: 13943
		public float depthBlendDistance = 2f;

		// Token: 0x04003678 RID: 13944
		public float cameraClippingDistance = 0.5f;

		// Token: 0x04003679 RID: 13945
		[Range(0f, 1f)]
		public float glareFrontal = 0.5f;

		// Token: 0x0400367A RID: 13946
		[Range(0f, 1f)]
		public float glareBehind = 0.5f;

		// Token: 0x0400367B RID: 13947
		[Obsolete("Use 'glareFrontal' instead")]
		public float boostDistanceInside = 0.5f;

		// Token: 0x0400367C RID: 13948
		[Obsolete("This property has been merged with 'fresnelPow'")]
		public float fresnelPowInside = 6f;

		// Token: 0x0400367D RID: 13949
		[FormerlySerializedAs("fresnelPowOutside")]
		public float fresnelPow = 8f;

		// Token: 0x0400367E RID: 13950
		public bool noiseEnabled;

		// Token: 0x0400367F RID: 13951
		[Range(0f, 1f)]
		public float noiseIntensity = 0.5f;

		// Token: 0x04003680 RID: 13952
		public bool noiseScaleUseGlobal = true;

		// Token: 0x04003681 RID: 13953
		[Range(0.01f, 2f)]
		public float noiseScaleLocal = 0.5f;

		// Token: 0x04003682 RID: 13954
		public bool noiseVelocityUseGlobal = true;

		// Token: 0x04003683 RID: 13955
		public Vector3 noiseVelocityLocal = Consts.NoiseVelocityDefault;

		// Token: 0x04003684 RID: 13956
		private Plane m_PlaneWS;

		// Token: 0x04003685 RID: 13957
		[SerializeField]
		private int pluginVersion = -1;

		// Token: 0x04003686 RID: 13958
		[FormerlySerializedAs("trackChangesDuringPlaytime")]
		[SerializeField]
		private bool _TrackChangesDuringPlaytime;

		// Token: 0x04003687 RID: 13959
		[SerializeField]
		private int _SortingLayerID;

		// Token: 0x04003688 RID: 13960
		[SerializeField]
		private int _SortingOrder;

		// Token: 0x04003689 RID: 13961
		private BeamGeometry m_BeamGeom;

		// Token: 0x0400368A RID: 13962
		private Coroutine m_CoPlaytimeUpdate;

		// Token: 0x0400368B RID: 13963
		private Light _CachedLight;
	}
}
