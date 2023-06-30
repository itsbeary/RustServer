using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9A RID: 2714
	public sealed class PostProcessManager
	{
		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06004074 RID: 16500 RVA: 0x0017B730 File Offset: 0x00179930
		public static PostProcessManager instance
		{
			get
			{
				if (PostProcessManager.s_Instance == null)
				{
					PostProcessManager.s_Instance = new PostProcessManager();
				}
				return PostProcessManager.s_Instance;
			}
		}

		// Token: 0x06004075 RID: 16501 RVA: 0x0017B748 File Offset: 0x00179948
		private PostProcessManager()
		{
			this.m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
			this.m_Volumes = new List<PostProcessVolume>();
			this.m_SortNeeded = new Dictionary<int, bool>();
			this.m_BaseSettings = new List<PostProcessEffectSettings>();
			this.settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			this.ReloadBaseTypes();
		}

		// Token: 0x06004076 RID: 16502 RVA: 0x0017B798 File Offset: 0x00179998
		private void CleanBaseTypes()
		{
			this.settingsTypes.Clear();
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.m_BaseSettings)
			{
				RuntimeUtilities.Destroy(postProcessEffectSettings);
			}
			this.m_BaseSettings.Clear();
		}

		// Token: 0x06004077 RID: 16503 RVA: 0x0017B800 File Offset: 0x00179A00
		private void ReloadBaseTypes()
		{
			this.CleanBaseTypes();
			foreach (Type type in from t in RuntimeUtilities.GetAllAssemblyTypes()
				where t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), false) && !t.IsAbstract
				select t)
			{
				this.settingsTypes.Add(type, type.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
				postProcessEffectSettings.SetAllOverridesTo(true, false);
				this.m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		// Token: 0x06004078 RID: 16504 RVA: 0x0017B8A4 File Offset: 0x00179AA4
		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
		{
			int value = layer.volumeLayer.value;
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = (flag ? Vector3.zero : volumeTrigger.position);
			foreach (PostProcessVolume postProcessVolume in this.GrabVolumes(value))
			{
				if ((!skipDisabled || postProcessVolume.enabled) && !(postProcessVolume.profileRef == null) && (!skipZeroWeight || postProcessVolume.weight > 0f))
				{
					if (postProcessVolume.isGlobal)
					{
						results.Add(postProcessVolume);
					}
					else if (!flag)
					{
						OBB obb = new OBB(postProcessVolume.transform, postProcessVolume.bounds);
						float sqrMagnitude = ((obb.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
						float num = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
						if (sqrMagnitude <= num)
						{
							results.Add(postProcessVolume);
						}
					}
				}
			}
		}

		// Token: 0x06004079 RID: 16505 RVA: 0x0017B9C4 File Offset: 0x00179BC4
		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			return this.GetHighestPriorityVolume(layer.volumeLayer);
		}

		// Token: 0x0600407A RID: 16506 RVA: 0x0017B9E8 File Offset: 0x00179BE8
		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			float num = float.NegativeInfinity;
			PostProcessVolume postProcessVolume = null;
			List<PostProcessVolume> list;
			if (this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				foreach (PostProcessVolume postProcessVolume2 in list)
				{
					if (postProcessVolume2.priority > num)
					{
						num = postProcessVolume2.priority;
						postProcessVolume = postProcessVolume2;
					}
				}
			}
			return postProcessVolume;
		}

		// Token: 0x0600407B RID: 16507 RVA: 0x0017BA64 File Offset: 0x00179C64
		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			PostProcessVolume postProcessVolume = new GameObject
			{
				name = "Quick Volume",
				layer = layer,
				hideFlags = HideFlags.HideAndDontSave
			}.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings postProcessEffectSettings in settings)
			{
				Assert.IsNotNull<PostProcessEffectSettings>(postProcessEffectSettings, "Trying to create a volume with null effects");
				profile.AddSettings(postProcessEffectSettings);
			}
			return postProcessVolume;
		}

		// Token: 0x0600407C RID: 16508 RVA: 0x0017BAD8 File Offset: 0x00179CD8
		internal void SetLayerDirty(int layer)
		{
			Assert.IsTrue(layer >= 0 && layer <= 32, "Invalid layer bit");
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				int key = keyValuePair.Key;
				if ((key & (1 << layer)) != 0)
				{
					this.m_SortNeeded[key] = true;
				}
			}
		}

		// Token: 0x0600407D RID: 16509 RVA: 0x0017BB5C File Offset: 0x00179D5C
		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Assert.IsTrue(prevLayer >= 0 && prevLayer <= 32, "Invalid layer bit");
			this.Unregister(volume, prevLayer);
			this.Register(volume, newLayer);
		}

		// Token: 0x0600407E RID: 16510 RVA: 0x0017BB88 File Offset: 0x00179D88
		private void Register(PostProcessVolume volume, int layer)
		{
			this.m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & (1 << layer)) != 0)
				{
					keyValuePair.Value.Add(volume);
				}
			}
			this.SetLayerDirty(layer);
		}

		// Token: 0x0600407F RID: 16511 RVA: 0x0017BC04 File Offset: 0x00179E04
		internal void Register(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			this.Register(volume, layer);
		}

		// Token: 0x06004080 RID: 16512 RVA: 0x0017BC28 File Offset: 0x00179E28
		private void Unregister(PostProcessVolume volume, int layer)
		{
			this.m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> keyValuePair in this.m_SortedVolumes)
			{
				if ((keyValuePair.Key & (1 << layer)) != 0)
				{
					keyValuePair.Value.Remove(volume);
				}
			}
		}

		// Token: 0x06004081 RID: 16513 RVA: 0x0017BCA0 File Offset: 0x00179EA0
		internal void Unregister(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			this.Unregister(volume, layer);
		}

		// Token: 0x06004082 RID: 16514 RVA: 0x0017BCC4 File Offset: 0x00179EC4
		private void ReplaceData(PostProcessLayer postProcessLayer)
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.m_BaseSettings)
			{
				PostProcessEffectSettings settings = postProcessLayer.GetBundle(postProcessEffectSettings.GetType()).settings;
				int count = postProcessEffectSettings.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					settings.parameters[i].SetValue(postProcessEffectSettings.parameters[i]);
				}
			}
		}

		// Token: 0x06004083 RID: 16515 RVA: 0x0017BD60 File Offset: 0x00179F60
		internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
		{
			this.ReplaceData(postProcessLayer);
			int value = postProcessLayer.volumeLayer.value;
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = (flag ? Vector3.zero : volumeTrigger.position);
			foreach (PostProcessVolume postProcessVolume in this.GrabVolumes(value))
			{
				if (postProcessVolume.enabled && !(postProcessVolume.profileRef == null) && postProcessVolume.weight > 0f)
				{
					List<PostProcessEffectSettings> settings = postProcessVolume.profileRef.settings;
					if (postProcessVolume.isGlobal)
					{
						postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(postProcessVolume.weight));
					}
					else if (!flag)
					{
						OBB obb = new OBB(postProcessVolume.transform, postProcessVolume.bounds);
						float sqrMagnitude = ((obb.ClosestPoint(vector) - vector) / 2f).sqrMagnitude;
						float num = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
						if (sqrMagnitude <= num)
						{
							float num2 = 1f;
							if (num > 0f)
							{
								num2 = 1f - sqrMagnitude / num;
							}
							postProcessLayer.OverrideSettings(settings, num2 * Mathf.Clamp01(postProcessVolume.weight));
						}
					}
				}
			}
		}

		// Token: 0x06004084 RID: 16516 RVA: 0x0017BEE4 File Offset: 0x0017A0E4
		private List<PostProcessVolume> GrabVolumes(LayerMask mask)
		{
			List<PostProcessVolume> list;
			if (!this.m_SortedVolumes.TryGetValue(mask, out list))
			{
				list = new List<PostProcessVolume>();
				foreach (PostProcessVolume postProcessVolume in this.m_Volumes)
				{
					if ((mask & (1 << postProcessVolume.gameObject.layer)) != 0)
					{
						list.Add(postProcessVolume);
						this.m_SortNeeded[mask] = true;
					}
				}
				this.m_SortedVolumes.Add(mask, list);
			}
			bool flag;
			if (this.m_SortNeeded.TryGetValue(mask, out flag) && flag)
			{
				this.m_SortNeeded[mask] = false;
				PostProcessManager.SortByPriority(list);
			}
			return list;
		}

		// Token: 0x06004085 RID: 16517 RVA: 0x0017BFC0 File Offset: 0x0017A1C0
		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			Assert.IsNotNull<List<PostProcessVolume>>(volumes, "Trying to sort volumes of non-initialized layer");
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}

		// Token: 0x06004086 RID: 16518 RVA: 0x0000441C File Offset: 0x0000261C
		private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
		{
			return true;
		}

		// Token: 0x040039EA RID: 14826
		private static PostProcessManager s_Instance;

		// Token: 0x040039EB RID: 14827
		private const int k_MaxLayerCount = 32;

		// Token: 0x040039EC RID: 14828
		private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

		// Token: 0x040039ED RID: 14829
		private readonly List<PostProcessVolume> m_Volumes;

		// Token: 0x040039EE RID: 14830
		private readonly Dictionary<int, bool> m_SortNeeded;

		// Token: 0x040039EF RID: 14831
		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		// Token: 0x040039F0 RID: 14832
		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;
	}
}
