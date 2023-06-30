using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A10 RID: 2576
	public struct RaycastSamplePositionsJob : IJob
	{
		// Token: 0x06003D56 RID: 15702 RVA: 0x00167858 File Offset: 0x00165A58
		public void Execute()
		{
			int i = 0;
			for (int j = 0; j < this.res.y; j++)
			{
				for (int k = 0; k < this.res.x; k++)
				{
					this.positions[i++] = new int2(k, j);
				}
			}
			for (i = this.res.x * this.res.y - 1; i >= 1; i--)
			{
				int num = this.random.NextInt(i + 1);
				int num2 = i;
				ref NativeArray<int2> ptr = ref this.positions;
				int num3 = num;
				int2 @int = this.positions[num];
				int2 int2 = this.positions[i];
				this.positions[num2] = @int;
				ptr[num3] = int2;
			}
		}

		// Token: 0x0400376C RID: 14188
		public int2 res;

		// Token: 0x0400376D RID: 14189
		public Unity.Mathematics.Random random;

		// Token: 0x0400376E RID: 14190
		public NativeArray<int2> positions;
	}
}
