using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
	// Token: 0x020009B1 RID: 2481
	[HelpURL("http://saladgamer.com/vlb-doc/config/")]
	public class Config : ScriptableObject
	{
		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06003B25 RID: 15141 RVA: 0x0015DE59 File Offset: 0x0015C059
		public Shader beamShader
		{
			get
			{
				if (!this.forceSinglePass)
				{
					return this.beamShader2Pass;
				}
				return this.beamShader1Pass;
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06003B26 RID: 15142 RVA: 0x0015DE70 File Offset: 0x0015C070
		public Vector4 globalNoiseParam
		{
			get
			{
				return new Vector4(this.globalNoiseVelocity.x, this.globalNoiseVelocity.y, this.globalNoiseVelocity.z, this.globalNoiseScale);
			}
		}

		// Token: 0x06003B27 RID: 15143 RVA: 0x0015DEA0 File Offset: 0x0015C0A0
		public void Reset()
		{
			this.geometryLayerID = 1;
			this.geometryTag = "Untagged";
			this.geometryRenderQueue = 3000;
			this.beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
			this.beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
			this.sharedMeshSides = 24;
			this.sharedMeshSegments = 5;
			this.globalNoiseScale = 0.5f;
			this.globalNoiseVelocity = Consts.NoiseVelocityDefault;
			this.noise3DData = Resources.Load("Noise3D_64x64x64") as TextAsset;
			this.noise3DSize = 64;
			this.dustParticlesPrefab = Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem;
		}

		// Token: 0x06003B28 RID: 15144 RVA: 0x0015DF4C File Offset: 0x0015C14C
		public ParticleSystem NewVolumetricDustParticles()
		{
			if (!this.dustParticlesPrefab)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
				}
				return null;
			}
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.dustParticlesPrefab);
			particleSystem.useAutoRandomSeed = false;
			particleSystem.name = "Dust Particles";
			particleSystem.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
			particleSystem.gameObject.SetActive(true);
			return particleSystem;
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06003B29 RID: 15145 RVA: 0x0015DFB4 File Offset: 0x0015C1B4
		public static Config Instance
		{
			get
			{
				if (Config.m_Instance == null)
				{
					Config[] array = Resources.LoadAll<Config>("Config");
					Debug.Assert(array.Length != 0, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
					Config.m_Instance = array[0];
				}
				return Config.m_Instance;
			}
		}

		// Token: 0x040035DC RID: 13788
		public int geometryLayerID = 1;

		// Token: 0x040035DD RID: 13789
		public string geometryTag = "Untagged";

		// Token: 0x040035DE RID: 13790
		public int geometryRenderQueue = 3000;

		// Token: 0x040035DF RID: 13791
		public bool forceSinglePass;

		// Token: 0x040035E0 RID: 13792
		[SerializeField]
		[HighlightNull]
		private Shader beamShader1Pass;

		// Token: 0x040035E1 RID: 13793
		[FormerlySerializedAs("BeamShader")]
		[FormerlySerializedAs("beamShader")]
		[SerializeField]
		[HighlightNull]
		private Shader beamShader2Pass;

		// Token: 0x040035E2 RID: 13794
		public int sharedMeshSides = 24;

		// Token: 0x040035E3 RID: 13795
		public int sharedMeshSegments = 5;

		// Token: 0x040035E4 RID: 13796
		[Range(0.01f, 2f)]
		public float globalNoiseScale = 0.5f;

		// Token: 0x040035E5 RID: 13797
		public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

		// Token: 0x040035E6 RID: 13798
		[HighlightNull]
		public TextAsset noise3DData;

		// Token: 0x040035E7 RID: 13799
		public int noise3DSize = 64;

		// Token: 0x040035E8 RID: 13800
		[HighlightNull]
		public ParticleSystem dustParticlesPrefab;

		// Token: 0x040035E9 RID: 13801
		private static Config m_Instance;
	}
}
