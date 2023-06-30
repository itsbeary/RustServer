using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A12 RID: 2578
	public struct RaycastRaySetupJob : IJobParallelFor
	{
		// Token: 0x06003D59 RID: 15705 RVA: 0x00167A5C File Offset: 0x00165C5C
		public void Execute(int index)
		{
			int i;
			for (i = this.sampleOffset + index; i >= this.samplePositions.Length; i -= this.samplePositions.Length)
			{
			}
			float2 @float = (this.samplePositions[i] - this.halfRes) / this.res;
			float3 float2 = new float3(@float.x * this.worldHeight * this.aspectRatio, @float.y * this.worldHeight, 1f);
			float3 float3 = math.mul(this.cameraRot, float2);
			float3 float4 = this.cameraPos + float3 * this.nearPlane;
			this.raycastCommands[index] = new RaycastCommand(float4, float3, this.farPlane, this.layerMask, 1);
		}

		// Token: 0x04003772 RID: 14194
		public float2 res;

		// Token: 0x04003773 RID: 14195
		public float2 halfRes;

		// Token: 0x04003774 RID: 14196
		public float aspectRatio;

		// Token: 0x04003775 RID: 14197
		public float worldHeight;

		// Token: 0x04003776 RID: 14198
		public float3 cameraPos;

		// Token: 0x04003777 RID: 14199
		public quaternion cameraRot;

		// Token: 0x04003778 RID: 14200
		public float nearPlane;

		// Token: 0x04003779 RID: 14201
		public float farPlane;

		// Token: 0x0400377A RID: 14202
		public int layerMask;

		// Token: 0x0400377B RID: 14203
		public int sampleOffset;

		// Token: 0x0400377C RID: 14204
		[Unity.Collections.ReadOnly]
		public NativeArray<int2> samplePositions;

		// Token: 0x0400377D RID: 14205
		[WriteOnly]
		[NativeMatchesParallelForLength]
		public NativeArray<RaycastCommand> raycastCommands;
	}
}
