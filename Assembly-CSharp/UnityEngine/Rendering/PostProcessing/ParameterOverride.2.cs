using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A85 RID: 2693
	[Serializable]
	public class ParameterOverride<T> : ParameterOverride
	{
		// Token: 0x06004018 RID: 16408 RVA: 0x0017A514 File Offset: 0x00178714
		public ParameterOverride()
			: this(default(T), false)
		{
		}

		// Token: 0x06004019 RID: 16409 RVA: 0x0017A531 File Offset: 0x00178731
		public ParameterOverride(T value)
			: this(value, false)
		{
		}

		// Token: 0x0600401A RID: 16410 RVA: 0x0017A53B File Offset: 0x0017873B
		public ParameterOverride(T value, bool overrideState)
		{
			this.value = value;
			this.overrideState = overrideState;
		}

		// Token: 0x0600401B RID: 16411 RVA: 0x0017A551 File Offset: 0x00178751
		internal override void Interp(ParameterOverride from, ParameterOverride to, float t)
		{
			this.Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		}

		// Token: 0x0600401C RID: 16412 RVA: 0x0017A566 File Offset: 0x00178766
		public virtual void Interp(T from, T to, float t)
		{
			this.value = ((t > 0f) ? to : from);
		}

		// Token: 0x0600401D RID: 16413 RVA: 0x0017A57A File Offset: 0x0017877A
		public void Override(T x)
		{
			this.overrideState = true;
			this.value = x;
		}

		// Token: 0x0600401E RID: 16414 RVA: 0x0017A58A File Offset: 0x0017878A
		internal override void SetValue(ParameterOverride parameter)
		{
			this.value = parameter.GetValue<T>();
		}

		// Token: 0x0600401F RID: 16415 RVA: 0x0017A598 File Offset: 0x00178798
		public override int GetHash()
		{
			return (17 * 23 + this.overrideState.GetHashCode()) * 23 + this.value.GetHashCode();
		}

		// Token: 0x06004020 RID: 16416 RVA: 0x0017A5C0 File Offset: 0x001787C0
		public static implicit operator T(ParameterOverride<T> prop)
		{
			return prop.value;
		}

		// Token: 0x040039B2 RID: 14770
		public T value;
	}
}
