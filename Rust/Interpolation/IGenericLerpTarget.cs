using System;
using System.Collections.Generic;

namespace Rust.Interpolation
{
	// Token: 0x02000B3F RID: 2879
	public interface IGenericLerpTarget<T> : ILerpInfo where T : ISnapshot<T>, new()
	{
		// Token: 0x060045A9 RID: 17833
		void SetFrom(T snapshot);

		// Token: 0x060045AA RID: 17834
		T GetCurrentState();

		// Token: 0x060045AB RID: 17835
		void DebugInterpolationState(Interpolator<T>.Segment segment, List<T> entries);
	}
}
