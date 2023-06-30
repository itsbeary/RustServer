using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A97 RID: 2711
	[Serializable]
	public class PostProcessEffectSettings : ScriptableObject
	{
		// Token: 0x0600406B RID: 16491 RVA: 0x0017B51C File Offset: 0x0017971C
		private void OnEnable()
		{
			this.parameters = (from t in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
				where t.FieldType.IsSubclassOf(typeof(ParameterOverride))
				orderby t.MetadataToken
				select (ParameterOverride)t.GetValue(this)).ToList<ParameterOverride>().AsReadOnly();
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				parameterOverride.OnEnable();
			}
		}

		// Token: 0x0600406C RID: 16492 RVA: 0x0017B5DC File Offset: 0x001797DC
		private void OnDisable()
		{
			if (this.parameters == null)
			{
				return;
			}
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				parameterOverride.OnDisable();
			}
		}

		// Token: 0x0600406D RID: 16493 RVA: 0x0017B630 File Offset: 0x00179830
		public void SetAllOverridesTo(bool state, bool excludeEnabled = true)
		{
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				if (!excludeEnabled || parameterOverride != this.enabled)
				{
					parameterOverride.overrideState = state;
				}
			}
		}

		// Token: 0x0600406E RID: 16494 RVA: 0x0017B68C File Offset: 0x0017988C
		public virtual bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value;
		}

		// Token: 0x0600406F RID: 16495 RVA: 0x0017B69C File Offset: 0x0017989C
		public int GetHash()
		{
			int num = 17;
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				num = num * 23 + parameterOverride.GetHash();
			}
			return num;
		}

		// Token: 0x040039E3 RID: 14819
		public bool active = true;

		// Token: 0x040039E4 RID: 14820
		public BoolParameter enabled = new BoolParameter
		{
			overrideState = true,
			value = false
		};

		// Token: 0x040039E5 RID: 14821
		internal ReadOnlyCollection<ParameterOverride> parameters;
	}
}
