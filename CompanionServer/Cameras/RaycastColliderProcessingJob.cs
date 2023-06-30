using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A15 RID: 2581
	public struct RaycastColliderProcessingJob : IJob
	{
		// Token: 0x06003D61 RID: 15713 RVA: 0x00167F44 File Offset: 0x00166144
		public void Execute()
		{
			int num = math.min(this.foundCollidersLength[0], this.foundColliders.Length);
			if (num <= 1)
			{
				return;
			}
			RaycastColliderProcessingJob.SortAscending(ref this.foundColliders, 0, num - 1);
			NativeArray<int> nativeArray = new NativeArray<int>(num, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int num2 = 0;
			int i = 0;
			while (i < num)
			{
				int num3 = this.foundColliders[i];
				int num4 = 1;
				while (i < num && this.foundColliders[i] == num3)
				{
					num4++;
					i++;
				}
				this.foundColliders[num2] = num3;
				nativeArray[num2] = num4;
				num2++;
			}
			RaycastColliderProcessingJob.SortByDescending(ref this.foundColliders, ref nativeArray, 0, num2 - 1);
			nativeArray.Dispose();
			int num5 = math.min(num2, 512);
			this.foundCollidersLength[0] = num5;
		}

		// Token: 0x06003D62 RID: 15714 RVA: 0x0016801C File Offset: 0x0016621C
		private static void SortByDescending(ref NativeArray<int> colliders, ref NativeArray<int> counts, int leftIndex, int rightIndex)
		{
			int i = leftIndex;
			int num = rightIndex;
			int num2 = counts[leftIndex];
			while (i <= num)
			{
				while (counts[i] > num2)
				{
					i++;
				}
				while (counts[num] < num2)
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = i;
					ref NativeArray<int> ptr = ref colliders;
					int num4 = num;
					int num5 = colliders[num];
					int num6 = colliders[i];
					colliders[num3] = num5;
					ptr[num4] = num6;
					num6 = i;
					ptr = ref counts;
					num5 = num;
					num4 = counts[num];
					num3 = counts[i];
					counts[num6] = num4;
					ptr[num5] = num3;
					i++;
					num--;
				}
			}
			if (leftIndex < num)
			{
				RaycastColliderProcessingJob.SortByDescending(ref colliders, ref counts, leftIndex, num);
			}
			if (i < rightIndex)
			{
				RaycastColliderProcessingJob.SortByDescending(ref colliders, ref counts, i, rightIndex);
			}
		}

		// Token: 0x06003D63 RID: 15715 RVA: 0x001680F0 File Offset: 0x001662F0
		private static void SortAscending(ref NativeArray<int> array, int leftIndex, int rightIndex)
		{
			int i = leftIndex;
			int num = rightIndex;
			int num2 = array[leftIndex];
			while (i <= num)
			{
				while (array[i] < num2)
				{
					i++;
				}
				while (array[num] > num2)
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = i;
					ref NativeArray<int> ptr = ref array;
					int num4 = num;
					int num5 = array[num];
					int num6 = array[i];
					array[num3] = num5;
					ptr[num4] = num6;
					i++;
					num--;
				}
			}
			if (leftIndex < num)
			{
				RaycastColliderProcessingJob.SortAscending(ref array, leftIndex, num);
			}
			if (i < rightIndex)
			{
				RaycastColliderProcessingJob.SortAscending(ref array, i, rightIndex);
			}
		}

		// Token: 0x0400378A RID: 14218
		public NativeArray<int> foundCollidersLength;

		// Token: 0x0400378B RID: 14219
		public NativeArray<int> foundColliders;
	}
}
