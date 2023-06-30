using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A99 RID: 2713
	internal struct PostProcessEventComparer : IEqualityComparer<PostProcessEvent>
	{
		// Token: 0x06004072 RID: 16498 RVA: 0x0017B72A File Offset: 0x0017992A
		public bool Equals(PostProcessEvent x, PostProcessEvent y)
		{
			return x == y;
		}

		// Token: 0x06004073 RID: 16499 RVA: 0x00036ECC File Offset: 0x000350CC
		public int GetHashCode(PostProcessEvent obj)
		{
			return (int)obj;
		}
	}
}
