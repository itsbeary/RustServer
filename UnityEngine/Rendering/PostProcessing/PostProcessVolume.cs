using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9D RID: 2717
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Volume", 1001)]
	public sealed class PostProcessVolume : MonoBehaviour
	{
		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06004093 RID: 16531 RVA: 0x0017C300 File Offset: 0x0017A500
		// (set) Token: 0x06004094 RID: 16532 RVA: 0x0017C394 File Offset: 0x0017A594
		public PostProcessProfile profile
		{
			get
			{
				if (this.m_InternalProfile == null)
				{
					this.m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
					if (this.sharedProfile != null)
					{
						foreach (PostProcessEffectSettings postProcessEffectSettings in this.sharedProfile.settings)
						{
							PostProcessEffectSettings postProcessEffectSettings2 = Object.Instantiate<PostProcessEffectSettings>(postProcessEffectSettings);
							this.m_InternalProfile.settings.Add(postProcessEffectSettings2);
						}
					}
				}
				return this.m_InternalProfile;
			}
			set
			{
				this.m_InternalProfile = value;
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06004095 RID: 16533 RVA: 0x0017C39D File Offset: 0x0017A59D
		internal PostProcessProfile profileRef
		{
			get
			{
				if (!(this.m_InternalProfile == null))
				{
					return this.m_InternalProfile;
				}
				return this.sharedProfile;
			}
		}

		// Token: 0x06004096 RID: 16534 RVA: 0x0017C3BA File Offset: 0x0017A5BA
		public bool HasInstantiatedProfile()
		{
			return this.m_InternalProfile != null;
		}

		// Token: 0x06004097 RID: 16535 RVA: 0x0017C3C8 File Offset: 0x0017A5C8
		private void OnEnable()
		{
			PostProcessManager.instance.Register(this);
			this.m_PreviousLayer = base.gameObject.layer;
		}

		// Token: 0x06004098 RID: 16536 RVA: 0x0017C3E6 File Offset: 0x0017A5E6
		private void OnDisable()
		{
			PostProcessManager.instance.Unregister(this);
		}

		// Token: 0x06004099 RID: 16537 RVA: 0x0017C3F4 File Offset: 0x0017A5F4
		private void Update()
		{
			int layer = base.gameObject.layer;
			if (layer != this.m_PreviousLayer)
			{
				PostProcessManager.instance.UpdateVolumeLayer(this, this.m_PreviousLayer, layer);
				this.m_PreviousLayer = layer;
			}
			if (this.priority != this.m_PreviousPriority)
			{
				PostProcessManager.instance.SetLayerDirty(layer);
				this.m_PreviousPriority = this.priority;
			}
		}

		// Token: 0x0600409A RID: 16538 RVA: 0x0017C454 File Offset: 0x0017A654
		private void OnDrawGizmos()
		{
			if (this.isGlobal)
			{
				return;
			}
			Vector3 lossyScale = base.transform.lossyScale;
			Vector3 vector = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, lossyScale);
			Gizmos.DrawCube(this.bounds.center, this.bounds.size);
			Gizmos.DrawWireCube(this.bounds.center, this.bounds.size + vector * this.blendDistance * 4f);
			Gizmos.matrix = Matrix4x4.identity;
		}

		// Token: 0x040039F8 RID: 14840
		public PostProcessProfile sharedProfile;

		// Token: 0x040039F9 RID: 14841
		[Tooltip("Check this box to mark this volume as global. This volume's Profile will be applied to the whole Scene.")]
		public bool isGlobal;

		// Token: 0x040039FA RID: 14842
		public Bounds bounds;

		// Token: 0x040039FB RID: 14843
		[Min(0f)]
		[Tooltip("The distance (from the attached Collider) to start blending from. A value of 0 means there will be no blending and the Volume overrides will be applied immediatly upon entry to the attached Collider.")]
		public float blendDistance;

		// Token: 0x040039FC RID: 14844
		[Range(0f, 1f)]
		[Tooltip("The total weight of this Volume in the Scene. A value of 0 signifies that it will have no effect, 1 signifies full effect.")]
		public float weight = 1f;

		// Token: 0x040039FD RID: 14845
		[Tooltip("The volume priority in the stack. A higher value means higher priority. Negative values are supported.")]
		public float priority;

		// Token: 0x040039FE RID: 14846
		private int m_PreviousLayer;

		// Token: 0x040039FF RID: 14847
		private float m_PreviousPriority;

		// Token: 0x04003A00 RID: 14848
		private PostProcessProfile m_InternalProfile;
	}
}
