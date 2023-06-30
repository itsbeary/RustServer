using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009C1 RID: 2497
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dustparticles/")]
	public class VolumetricDustParticles : MonoBehaviour
	{
		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06003B69 RID: 15209 RVA: 0x0015F23E File Offset: 0x0015D43E
		// (set) Token: 0x06003B6A RID: 15210 RVA: 0x0015F246 File Offset: 0x0015D446
		public bool isCulled { get; private set; }

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06003B6B RID: 15211 RVA: 0x0015F24F File Offset: 0x0015D44F
		public bool particlesAreInstantiated
		{
			get
			{
				return this.m_Particles;
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06003B6C RID: 15212 RVA: 0x0015F25C File Offset: 0x0015D45C
		public int particlesCurrentCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.particleCount;
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06003B6D RID: 15213 RVA: 0x0015F278 File Offset: 0x0015D478
		public int particlesMaxCount
		{
			get
			{
				if (!this.m_Particles)
				{
					return 0;
				}
				return this.m_Particles.main.maxParticles;
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06003B6E RID: 15214 RVA: 0x0015F2A8 File Offset: 0x0015D4A8
		public Camera mainCamera
		{
			get
			{
				if (!VolumetricDustParticles.ms_MainCamera)
				{
					VolumetricDustParticles.ms_MainCamera = Camera.main;
					if (!VolumetricDustParticles.ms_MainCamera && !VolumetricDustParticles.ms_NoMainCameraLogged)
					{
						Debug.LogErrorFormat(base.gameObject, "In order to use 'VolumetricDustParticles' culling, you must have a MainCamera defined in your scene.", Array.Empty<object>());
						VolumetricDustParticles.ms_NoMainCameraLogged = true;
					}
				}
				return VolumetricDustParticles.ms_MainCamera;
			}
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x0015F2FE File Offset: 0x0015D4FE
		private void Start()
		{
			this.isCulled = false;
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
			this.InstantiateParticleSystem();
			this.SetActiveAndPlay();
		}

		// Token: 0x06003B70 RID: 15216 RVA: 0x0015F330 File Offset: 0x0015D530
		private void InstantiateParticleSystem()
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
			this.m_Particles = Config.Instance.NewVolumetricDustParticles();
			if (this.m_Particles)
			{
				this.m_Particles.transform.SetParent(base.transform, false);
				this.m_Renderer = this.m_Particles.GetComponent<ParticleSystemRenderer>();
			}
		}

		// Token: 0x06003B71 RID: 15217 RVA: 0x0015F3A7 File Offset: 0x0015D5A7
		private void OnEnable()
		{
			this.SetActiveAndPlay();
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x0015F3AF File Offset: 0x0015D5AF
		private void SetActiveAndPlay()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(true);
				this.SetParticleProperties();
				this.m_Particles.Play(true);
			}
		}

		// Token: 0x06003B73 RID: 15219 RVA: 0x0015F3E1 File Offset: 0x0015D5E1
		private void OnDisable()
		{
			if (this.m_Particles)
			{
				this.m_Particles.gameObject.SetActive(false);
			}
		}

		// Token: 0x06003B74 RID: 15220 RVA: 0x0015F401 File Offset: 0x0015D601
		private void OnDestroy()
		{
			if (this.m_Particles)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Particles.gameObject);
			}
			this.m_Particles = null;
		}

		// Token: 0x06003B75 RID: 15221 RVA: 0x0015F427 File Offset: 0x0015D627
		private void Update()
		{
			if (Application.isPlaying)
			{
				this.UpdateCulling();
			}
			this.SetParticleProperties();
		}

		// Token: 0x06003B76 RID: 15222 RVA: 0x0015F43C File Offset: 0x0015D63C
		private void SetParticleProperties()
		{
			if (this.m_Particles && this.m_Particles.gameObject.activeSelf)
			{
				float num = Mathf.Clamp01(1f - this.m_Master.fresnelPow / 10f);
				float num2 = this.m_Master.fadeEnd * this.spawnMaxDistance;
				float num3 = num2 * this.density;
				int num4 = (int)(num3 * 4f);
				ParticleSystem.MainModule main = this.m_Particles.main;
				ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
				startLifetime.mode = ParticleSystemCurveMode.TwoConstants;
				startLifetime.constantMin = 4f;
				startLifetime.constantMax = 6f;
				main.startLifetime = startLifetime;
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				startSize.mode = ParticleSystemCurveMode.TwoConstants;
				startSize.constantMin = this.size * 0.9f;
				startSize.constantMax = this.size * 1.1f;
				main.startSize = startSize;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				if (this.m_Master.colorMode == ColorMode.Flat)
				{
					startColor.mode = ParticleSystemGradientMode.Color;
					Color color = this.m_Master.color;
					color.a *= this.alpha;
					startColor.color = color;
				}
				else
				{
					startColor.mode = ParticleSystemGradientMode.Gradient;
					Gradient colorGradient = this.m_Master.colorGradient;
					GradientColorKey[] colorKeys = colorGradient.colorKeys;
					GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
					for (int i = 0; i < alphaKeys.Length; i++)
					{
						GradientAlphaKey[] array = alphaKeys;
						int num5 = i;
						array[num5].alpha = array[num5].alpha * this.alpha;
					}
					Gradient gradient = new Gradient();
					gradient.SetKeys(colorKeys, alphaKeys);
					startColor.gradient = gradient;
				}
				main.startColor = startColor;
				ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
				startSpeed.constant = this.speed;
				main.startSpeed = startSpeed;
				main.maxParticles = num4;
				ParticleSystem.ShapeModule shape = this.m_Particles.shape;
				shape.shapeType = ParticleSystemShapeType.ConeVolume;
				shape.radius = this.m_Master.coneRadiusStart * Mathf.Lerp(0.3f, 1f, num);
				shape.angle = this.m_Master.coneAngle * 0.5f * Mathf.Lerp(0.7f, 1f, num);
				shape.length = num2;
				shape.arc = 360f;
				shape.randomDirectionAmount = ((this.direction == VolumetricDustParticles.Direction.Random) ? 1f : 0f);
				ParticleSystem.EmissionModule emission = this.m_Particles.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				rateOverTime.constant = num3;
				emission.rateOverTime = rateOverTime;
				if (this.m_Renderer)
				{
					this.m_Renderer.sortingLayerID = this.m_Master.sortingLayerID;
					this.m_Renderer.sortingOrder = this.m_Master.sortingOrder;
				}
			}
		}

		// Token: 0x06003B77 RID: 15223 RVA: 0x0015F700 File Offset: 0x0015D900
		private void UpdateCulling()
		{
			if (this.m_Particles)
			{
				bool flag = true;
				if (this.cullingEnabled && this.m_Master.hasGeometry)
				{
					if (this.mainCamera)
					{
						float num = this.cullingMaxDistance * this.cullingMaxDistance;
						flag = this.m_Master.bounds.SqrDistance(this.mainCamera.transform.position) <= num;
					}
					else
					{
						this.cullingEnabled = false;
					}
				}
				if (this.m_Particles.gameObject.activeSelf != flag)
				{
					this.m_Particles.gameObject.SetActive(flag);
					this.isCulled = !flag;
				}
				if (flag && !this.m_Particles.isPlaying)
				{
					this.m_Particles.Play();
				}
			}
		}

		// Token: 0x04003655 RID: 13909
		[Range(0f, 1f)]
		public float alpha = 0.5f;

		// Token: 0x04003656 RID: 13910
		[Range(0.0001f, 0.1f)]
		public float size = 0.01f;

		// Token: 0x04003657 RID: 13911
		public VolumetricDustParticles.Direction direction = VolumetricDustParticles.Direction.Random;

		// Token: 0x04003658 RID: 13912
		public float speed = 0.03f;

		// Token: 0x04003659 RID: 13913
		public float density = 5f;

		// Token: 0x0400365A RID: 13914
		[Range(0f, 1f)]
		public float spawnMaxDistance = 0.7f;

		// Token: 0x0400365B RID: 13915
		public bool cullingEnabled = true;

		// Token: 0x0400365C RID: 13916
		public float cullingMaxDistance = 10f;

		// Token: 0x0400365E RID: 13918
		public static bool isFeatureSupported = true;

		// Token: 0x0400365F RID: 13919
		private ParticleSystem m_Particles;

		// Token: 0x04003660 RID: 13920
		private ParticleSystemRenderer m_Renderer;

		// Token: 0x04003661 RID: 13921
		private static bool ms_NoMainCameraLogged = false;

		// Token: 0x04003662 RID: 13922
		private static Camera ms_MainCamera = null;

		// Token: 0x04003663 RID: 13923
		private VolumetricLightBeam m_Master;

		// Token: 0x02000EF5 RID: 3829
		public enum Direction
		{
			// Token: 0x04004E1C RID: 19996
			Beam,
			// Token: 0x04004E1D RID: 19997
			Random
		}
	}
}
