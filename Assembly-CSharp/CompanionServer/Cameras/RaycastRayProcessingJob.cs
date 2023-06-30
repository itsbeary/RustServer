using System;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A13 RID: 2579
	public struct RaycastRayProcessingJob : IJobParallelFor
	{
		// Token: 0x06003D5A RID: 15706 RVA: 0x00167B38 File Offset: 0x00165D38
		public void Execute(int index)
		{
			ref readonly RaycastHit @readonly = ref (this.raycastHits).GetReadonly(index);
			int colliderId = @readonly.GetColliderId();
			bool flag = colliderId != 0;
			byte b = 0;
			if (flag)
			{
				int num = Interlocked.Increment((this.foundCollidersIndex).Get(0));
				if (num <= this.foundColliders.Length)
				{
					this.foundColliders[num - 1] = colliderId;
				}
				int num2 = RaycastRayProcessingJob.BinarySearch(this.colliderIds, colliderId);
				if (num2 >= 0)
				{
					b = this.colliderMaterials[num2];
					Interlocked.Increment((this.colliderHits).Get(num2));
				}
			}
			float distance;
			RaycastHit raycastHit;
			if (!flag)
			{
				distance = this.farPlane;
			}
			else
			{
				raycastHit = @readonly;
				distance = raycastHit.distance;
			}
			float num3 = distance;
			if (b == 7)
			{
				b = 0;
				num3 *= 1.1f;
			}
			float num4 = math.clamp(num3 / this.farPlane, 0f, 1f);
			float3 @float = this.cameraForward;
			raycastHit = @readonly;
			float num5 = math.max(math.dot(@float, raycastHit.normal), 0f);
			ushort num6 = (ushort)(num4 * 1023f);
			byte b2 = (byte)(num5 * 63f);
			this.outputs[index] = (num6 >> 8 << 24) | ((int)(num6 & 255) << 16) | ((int)b2 << 8) | (int)b;
		}

		// Token: 0x06003D5B RID: 15707 RVA: 0x00167C74 File Offset: 0x00165E74
		private static int BinarySearch(NativeArray<int> haystack, int needle)
		{
			int i = 0;
			int num = haystack.Length - 1;
			while (i <= num)
			{
				int num2 = i + (num - i / 2);
				int num3 = RaycastRayProcessingJob.Compare(haystack[num2], needle);
				if (num3 == 0)
				{
					return num2;
				}
				if (num3 < 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		// Token: 0x06003D5C RID: 15708 RVA: 0x00167CBF File Offset: 0x00165EBF
		private static int Compare(int x, int y)
		{
			if (x < y)
			{
				return -1;
			}
			if (x > y)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x0400377E RID: 14206
		public float3 cameraForward;

		// Token: 0x0400377F RID: 14207
		public float farPlane;

		// Token: 0x04003780 RID: 14208
		[Unity.Collections.ReadOnly]
		public NativeArray<RaycastHit> raycastHits;

		// Token: 0x04003781 RID: 14209
		[Unity.Collections.ReadOnly]
		public NativeArray<int> colliderIds;

		// Token: 0x04003782 RID: 14210
		[Unity.Collections.ReadOnly]
		public NativeArray<byte> colliderMaterials;

		// Token: 0x04003783 RID: 14211
		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<int> colliderHits;

		// Token: 0x04003784 RID: 14212
		[WriteOnly]
		[NativeMatchesParallelForLength]
		public NativeArray<int> outputs;

		// Token: 0x04003785 RID: 14213
		[NativeDisableParallelForRestriction]
		public NativeArray<int> foundCollidersIndex;

		// Token: 0x04003786 RID: 14214
		[NativeDisableParallelForRestriction]
		public NativeArray<int> foundColliders;
	}
}
