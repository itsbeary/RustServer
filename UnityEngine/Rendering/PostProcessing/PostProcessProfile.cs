using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9B RID: 2715
	public sealed class PostProcessProfile : ScriptableObject
	{
		// Token: 0x06004087 RID: 16519 RVA: 0x0017C02D File Offset: 0x0017A22D
		private void OnEnable()
		{
			this.settings.RemoveAll((PostProcessEffectSettings x) => x == null);
		}

		// Token: 0x06004088 RID: 16520 RVA: 0x0017C05A File Offset: 0x0017A25A
		public T AddSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)((object)this.AddSettings(typeof(T)));
		}

		// Token: 0x06004089 RID: 16521 RVA: 0x0017C074 File Offset: 0x0017A274
		public PostProcessEffectSettings AddSettings(Type type)
		{
			if (this.HasSettings(type))
			{
				throw new InvalidOperationException("Effect already exists in the stack");
			}
			PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
			postProcessEffectSettings.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
			postProcessEffectSettings.name = type.Name;
			postProcessEffectSettings.enabled.value = true;
			this.settings.Add(postProcessEffectSettings);
			this.isDirty = true;
			return postProcessEffectSettings;
		}

		// Token: 0x0600408A RID: 16522 RVA: 0x0017C0D4 File Offset: 0x0017A2D4
		public PostProcessEffectSettings AddSettings(PostProcessEffectSettings effect)
		{
			if (this.HasSettings(this.settings.GetType()))
			{
				throw new InvalidOperationException("Effect already exists in the stack");
			}
			this.settings.Add(effect);
			this.isDirty = true;
			return effect;
		}

		// Token: 0x0600408B RID: 16523 RVA: 0x0017C108 File Offset: 0x0017A308
		public void RemoveSettings<T>() where T : PostProcessEffectSettings
		{
			this.RemoveSettings(typeof(T));
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x0017C11C File Offset: 0x0017A31C
		public void RemoveSettings(Type type)
		{
			int num = -1;
			for (int i = 0; i < this.settings.Count; i++)
			{
				if (this.settings[i].GetType() == type)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				throw new InvalidOperationException("Effect doesn't exist in the profile");
			}
			this.settings.RemoveAt(num);
			this.isDirty = true;
		}

		// Token: 0x0600408D RID: 16525 RVA: 0x0017C180 File Offset: 0x0017A380
		public bool HasSettings<T>() where T : PostProcessEffectSettings
		{
			return this.HasSettings(typeof(T));
		}

		// Token: 0x0600408E RID: 16526 RVA: 0x0017C194 File Offset: 0x0017A394
		public bool HasSettings(Type type)
		{
			using (List<PostProcessEffectSettings>.Enumerator enumerator = this.settings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600408F RID: 16527 RVA: 0x0017C1F4 File Offset: 0x0017A3F4
		public T GetSetting<T>() where T : PostProcessEffectSettings
		{
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.settings)
			{
				if (postProcessEffectSettings is T)
				{
					return postProcessEffectSettings as T;
				}
			}
			return default(T);
		}

		// Token: 0x06004090 RID: 16528 RVA: 0x0017C264 File Offset: 0x0017A464
		public bool TryGetSettings<T>(out T outSetting) where T : PostProcessEffectSettings
		{
			Type typeFromHandle = typeof(T);
			outSetting = default(T);
			foreach (PostProcessEffectSettings postProcessEffectSettings in this.settings)
			{
				if (postProcessEffectSettings.GetType() == typeFromHandle)
				{
					outSetting = (T)((object)postProcessEffectSettings);
					return true;
				}
			}
			return false;
		}

		// Token: 0x040039F1 RID: 14833
		[Tooltip("A list of all settings currently stored in this profile.")]
		public List<PostProcessEffectSettings> settings = new List<PostProcessEffectSettings>();

		// Token: 0x040039F2 RID: 14834
		[NonSerialized]
		public bool isDirty = true;
	}
}
