using System;
using Unity.Collections;
using Unity.Jobs;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A11 RID: 2577
	public struct RaycastBufferSetupJob : IJob
	{
		// Token: 0x06003D57 RID: 15703 RVA: 0x00167928 File Offset: 0x00165B28
		public void Execute()
		{
			if (this.colliderIds.Length > 1)
			{
				RaycastBufferSetupJob.SortByAscending(ref this.colliderIds, ref this.colliderMaterials, 0, this.colliderIds.Length - 1);
			}
			for (int i = 0; i < this.colliderHits.Length; i++)
			{
				this.colliderHits[i] = 0;
			}
		}

		// Token: 0x06003D58 RID: 15704 RVA: 0x00167988 File Offset: 0x00165B88
		private static void SortByAscending(ref NativeArray<int> colliderIds, ref NativeArray<byte> colliderMaterials, int leftIndex, int rightIndex)
		{
			int i = leftIndex;
			int num = rightIndex;
			int num2 = colliderIds[leftIndex];
			while (i <= num)
			{
				while (colliderIds[i] < num2)
				{
					i++;
				}
				while (colliderIds[num] > num2)
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = i;
					ref NativeArray<int> ptr = ref colliderIds;
					int num4 = num;
					int num5 = colliderIds[num];
					int num6 = colliderIds[i];
					colliderIds[num3] = num5;
					ptr[num4] = num6;
					num6 = i;
					ref NativeArray<byte> ptr2 = ref colliderMaterials;
					num5 = num;
					byte b = colliderMaterials[num];
					byte b2 = colliderMaterials[i];
					colliderMaterials[num6] = b;
					ptr2[num5] = b2;
					i++;
					num--;
				}
			}
			if (leftIndex < num)
			{
				RaycastBufferSetupJob.SortByAscending(ref colliderIds, ref colliderMaterials, leftIndex, num);
			}
			if (i < rightIndex)
			{
				RaycastBufferSetupJob.SortByAscending(ref colliderIds, ref colliderMaterials, i, rightIndex);
			}
		}

		// Token: 0x0400376F RID: 14191
		public NativeArray<int> colliderIds;

		// Token: 0x04003770 RID: 14192
		public NativeArray<byte> colliderMaterials;

		// Token: 0x04003771 RID: 14193
		[WriteOnly]
		public NativeArray<int> colliderHits;
	}
}
