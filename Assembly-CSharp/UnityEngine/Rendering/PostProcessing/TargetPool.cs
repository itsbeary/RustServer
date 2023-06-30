using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA8 RID: 2728
	internal class TargetPool
	{
		// Token: 0x06004109 RID: 16649 RVA: 0x0017EB56 File Offset: 0x0017CD56
		internal TargetPool()
		{
			this.m_Pool = new List<int>();
			this.Get();
		}

		// Token: 0x0600410A RID: 16650 RVA: 0x0017EB70 File Offset: 0x0017CD70
		internal int Get()
		{
			int num = this.Get(this.m_Current);
			this.m_Current++;
			return num;
		}

		// Token: 0x0600410B RID: 16651 RVA: 0x0017EB8C File Offset: 0x0017CD8C
		private int Get(int i)
		{
			int num;
			if (this.m_Pool.Count > i)
			{
				num = this.m_Pool[i];
			}
			else
			{
				while (this.m_Pool.Count <= i)
				{
					this.m_Pool.Add(Shader.PropertyToID("_TargetPool" + i));
				}
				num = this.m_Pool[i];
			}
			return num;
		}

		// Token: 0x0600410C RID: 16652 RVA: 0x0017EBF2 File Offset: 0x0017CDF2
		internal void Reset()
		{
			this.m_Current = 0;
		}

		// Token: 0x04003AB2 RID: 15026
		private readonly List<int> m_Pool;

		// Token: 0x04003AB3 RID: 15027
		private int m_Current;
	}
}
