using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A84 RID: 2692
	public abstract class ParameterOverride
	{
		// Token: 0x06004011 RID: 16401
		internal abstract void Interp(ParameterOverride from, ParameterOverride to, float t);

		// Token: 0x06004012 RID: 16402
		public abstract int GetHash();

		// Token: 0x06004013 RID: 16403 RVA: 0x0017A507 File Offset: 0x00178707
		public T GetValue<T>()
		{
			return ((ParameterOverride<T>)this).value;
		}

		// Token: 0x06004014 RID: 16404 RVA: 0x000063A5 File Offset: 0x000045A5
		protected internal virtual void OnEnable()
		{
		}

		// Token: 0x06004015 RID: 16405 RVA: 0x000063A5 File Offset: 0x000045A5
		protected internal virtual void OnDisable()
		{
		}

		// Token: 0x06004016 RID: 16406
		internal abstract void SetValue(ParameterOverride parameter);

		// Token: 0x040039B1 RID: 14769
		public bool overrideState;
	}
}
