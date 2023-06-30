using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000206 RID: 518
public class FishShoal : IDisposable
{
	// Token: 0x06001B52 RID: 6994 RVA: 0x000C1A3C File Offset: 0x000BFC3C
	public FishShoal(FishShoal.FishType fishType)
	{
		this.fishType = fishType;
		this.castCommands = new NativeArray<RaycastCommand>(fishType.castsPerFrame, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.castResults = new NativeArray<RaycastHit>(fishType.castsPerFrame, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.fishCastIndices = new NativeArray<int>(fishType.castsPerFrame, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.fishData = new NativeArray<FishShoal.FishData>(fishType.maxCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.fishRenderData = new NativeArray<FishShoal.FishRenderData>(fishType.maxCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.fishCount = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.fishBuffer = new ComputeBuffer(fishType.maxCount, UnsafeUtility.SizeOf<FishShoal.FishRenderData>());
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.materialPropertyBlock.SetBuffer("_FishData", this.fishBuffer);
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x000C1AFC File Offset: 0x000BFCFC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float3 GetTarget(float3 spawnPos, ref Unity.Mathematics.Random random)
	{
		float2 @float = random.NextFloat2Direction();
		return spawnPos + new float3(@float.x, 0f, @float.y) * random.NextFloat(10f, 15f);
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x0000441C File Offset: 0x0000261C
	private int GetPopulationScaleForPoint(float3 cameraPosition)
	{
		return 1;
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x000C1B44 File Offset: 0x000BFD44
	public void TrySpawn(float3 cameraPosition)
	{
		float num = TerrainMeta.WaterMap.GetHeight(cameraPosition) - 3f;
		float height = TerrainMeta.HeightMap.GetHeight(cameraPosition);
		if (math.abs(num - height) < 4f || num < height)
		{
			return;
		}
		int num2 = this.fishCount[0];
		int num3 = Mathf.CeilToInt((float)(this.fishType.maxCount * this.GetPopulationScaleForPoint(cameraPosition))) - num2;
		if (num3 <= 0)
		{
			return;
		}
		uint num4 = (uint)(Time.frameCount + this.fishType.mesh.vertexCount);
		int num5 = this.fishCount[0];
		int num6 = math.min(num5 + num3, this.fishType.maxCount);
		for (int i = num5; i < num6; i++)
		{
			Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)((long)(i * 3245) + (long)((ulong)num4)));
			float3 @float = cameraPosition + random.NextFloat3Direction() * random.NextFloat(40f);
			@float.y = random.NextFloat(math.max(height + 1f, cameraPosition.y - 30f), math.min(num, cameraPosition.y + 30f));
			if (!(WaterSystem.Instance == null) && WaterLevel.Test(@float, false, false, null) && TerrainMeta.HeightMap.GetHeight(@float) <= @float.y)
			{
				float3 target = FishShoal.GetTarget(@float, ref random);
				float3 float2 = math.normalize(target - @float);
				this.fishData[num2] = new FishShoal.FishData
				{
					isAlive = true,
					spawnX = @float.x,
					spawnZ = @float.z,
					destinationX = target.x,
					destinationZ = target.z,
					scale = random.NextFloat(0.9f, 1.4f)
				};
				this.fishRenderData[num2] = new FishShoal.FishRenderData
				{
					position = @float,
					rotation = math.atan2(float2.z, float2.x),
					scale = 0f
				};
				num2++;
			}
		}
		this.fishCount[0] = num2;
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x000C1D97 File Offset: 0x000BFF97
	public void OnUpdate(float3 cameraPosition)
	{
		this.UpdateJobs(cameraPosition);
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x000C1DA0 File Offset: 0x000BFFA0
	private unsafe void UpdateJobs(float3 cameraPosition)
	{
		this.jobHandle.Complete();
		int num = this.fishCount[0];
		if (num == 0)
		{
			return;
		}
		float num2 = ((TerrainMeta.WaterMap == null) ? 0f : (TerrainMeta.WaterMap.GetHeight(cameraPosition) - 3f));
		int num3 = math.min(this.fishType.castsPerFrame, num);
		uint num4 = (uint)(Time.frameCount + this.fishType.mesh.vertexCount);
		FishShoal.FishCollisionGatherJob fishCollisionGatherJob = new FishShoal.FishCollisionGatherJob
		{
			layerMask = -1,
			seed = num4,
			castCount = num3,
			fishCount = num,
			castCommands = this.castCommands,
			fishCastIndices = this.fishCastIndices,
			fishDataArray = this.fishData,
			fishRenderDataArray = this.fishRenderData
		};
		FishShoal.FishCollisionProcessJob fishCollisionProcessJob = new FishShoal.FishCollisionProcessJob
		{
			castCount = num3,
			castResults = this.castResults,
			fishCastIndices = this.fishCastIndices,
			fishDataArray = this.fishData,
			fishRenderDataArray = this.fishRenderData
		};
		FishShoal.FishUpdateJob fishUpdateJob = new FishShoal.FishUpdateJob
		{
			cameraPosition = cameraPosition,
			seed = num4,
			dt = Time.deltaTime,
			minSpeed = this.fishType.minSpeed,
			maxSpeed = this.fishType.maxSpeed,
			minTurnSpeed = this.fishType.minTurnSpeed,
			maxTurnSpeed = this.fishType.maxTurnSpeed,
			fishDataArray = (FishShoal.FishData*)this.fishData.GetUnsafePtr<FishShoal.FishData>(),
			fishRenderDataArray = (FishShoal.FishRenderData*)this.fishRenderData.GetUnsafePtr<FishShoal.FishRenderData>(),
			minDepth = num2 - 3f
		};
		FishShoal.KillFish killFish = new FishShoal.KillFish
		{
			fishCount = this.fishCount,
			fishDataArray = this.fishData,
			fishRenderDataArray = this.fishRenderData
		};
		this.jobHandle = fishCollisionGatherJob.Schedule(default(JobHandle));
		this.jobHandle = RaycastCommand.ScheduleBatch(this.castCommands, this.castResults, 5, this.jobHandle);
		this.jobHandle = fishCollisionProcessJob.Schedule(this.jobHandle);
		this.jobHandle = fishUpdateJob.Schedule(num, 10, this.jobHandle);
		this.jobHandle = killFish.Schedule(this.jobHandle);
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x000C200C File Offset: 0x000C020C
	public void OnLateUpdate(float3 cameraPosition)
	{
		this.jobHandle.Complete();
		if (this.fishCount[0] == 0)
		{
			return;
		}
		Bounds bounds = new Bounds(cameraPosition, Vector3.one * 40f);
		this.fishBuffer.SetData<FishShoal.FishRenderData>(this.fishRenderData);
		Graphics.DrawMeshInstancedProcedural(this.fishType.mesh, 0, this.fishType.material, bounds, this.fishCount[0], this.materialPropertyBlock, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x000C2098 File Offset: 0x000C0298
	public void Dispose()
	{
		this.jobHandle.Complete();
		this.castCommands.Dispose();
		this.castResults.Dispose();
		this.fishCastIndices.Dispose();
		this.fishData.Dispose();
		this.fishRenderData.Dispose();
		this.fishCount.Dispose();
		this.fishBuffer.Dispose();
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x000C20FD File Offset: 0x000C02FD
	public void OnDrawGizmos()
	{
		this.jobHandle.Complete();
		int num = this.fishCount[0];
	}

	// Token: 0x04001336 RID: 4918
	private const float maxFishDistance = 40f;

	// Token: 0x04001337 RID: 4919
	private FishShoal.FishType fishType;

	// Token: 0x04001338 RID: 4920
	private JobHandle jobHandle;

	// Token: 0x04001339 RID: 4921
	private NativeArray<RaycastCommand> castCommands;

	// Token: 0x0400133A RID: 4922
	private NativeArray<RaycastHit> castResults;

	// Token: 0x0400133B RID: 4923
	private NativeArray<int> fishCastIndices;

	// Token: 0x0400133C RID: 4924
	private NativeArray<FishShoal.FishData> fishData;

	// Token: 0x0400133D RID: 4925
	private NativeArray<FishShoal.FishRenderData> fishRenderData;

	// Token: 0x0400133E RID: 4926
	private NativeArray<int> fishCount;

	// Token: 0x0400133F RID: 4927
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x04001340 RID: 4928
	private ComputeBuffer fishBuffer;

	// Token: 0x02000C77 RID: 3191
	[Serializable]
	public struct FishType
	{
		// Token: 0x040043A2 RID: 17314
		public Mesh mesh;

		// Token: 0x040043A3 RID: 17315
		public Material material;

		// Token: 0x040043A4 RID: 17316
		public int castsPerFrame;

		// Token: 0x040043A5 RID: 17317
		public int maxCount;

		// Token: 0x040043A6 RID: 17318
		public float minSpeed;

		// Token: 0x040043A7 RID: 17319
		public float maxSpeed;

		// Token: 0x040043A8 RID: 17320
		public float idealDepth;

		// Token: 0x040043A9 RID: 17321
		public float minTurnSpeed;

		// Token: 0x040043AA RID: 17322
		public float maxTurnSpeed;
	}

	// Token: 0x02000C78 RID: 3192
	public struct FishData
	{
		// Token: 0x040043AB RID: 17323
		public bool isAlive;

		// Token: 0x040043AC RID: 17324
		public float updateTime;

		// Token: 0x040043AD RID: 17325
		public float startleTime;

		// Token: 0x040043AE RID: 17326
		public float spawnX;

		// Token: 0x040043AF RID: 17327
		public float spawnZ;

		// Token: 0x040043B0 RID: 17328
		public float destinationX;

		// Token: 0x040043B1 RID: 17329
		public float destinationZ;

		// Token: 0x040043B2 RID: 17330
		public float directionX;

		// Token: 0x040043B3 RID: 17331
		public float directionZ;

		// Token: 0x040043B4 RID: 17332
		public float speed;

		// Token: 0x040043B5 RID: 17333
		public float scale;
	}

	// Token: 0x02000C79 RID: 3193
	public struct FishRenderData
	{
		// Token: 0x040043B6 RID: 17334
		public float3 position;

		// Token: 0x040043B7 RID: 17335
		public float rotation;

		// Token: 0x040043B8 RID: 17336
		public float scale;

		// Token: 0x040043B9 RID: 17337
		public float distance;
	}

	// Token: 0x02000C7A RID: 3194
	public struct FishCollisionGatherJob : IJob
	{
		// Token: 0x06004F1E RID: 20254 RVA: 0x001A531C File Offset: 0x001A351C
		public void Execute()
		{
			Unity.Mathematics.Random random = new Unity.Mathematics.Random(this.seed);
			int length = this.castCommands.Length;
			for (int i = 0; i < length; i++)
			{
				if (i >= this.castCount)
				{
					this.castCommands[i] = default(RaycastCommand);
				}
				else
				{
					int num = random.NextInt(0, this.fishCount);
					FishShoal.FishData fishData = this.fishDataArray[num];
					FishShoal.FishRenderData fishRenderData = this.fishRenderDataArray[num];
					this.castCommands[i] = new RaycastCommand
					{
						from = fishRenderData.position,
						direction = new float3(fishData.directionX, 0f, fishData.directionZ),
						distance = 4f,
						layerMask = this.layerMask,
						maxHits = 1
					};
					this.fishCastIndices[i] = num;
				}
			}
		}

		// Token: 0x040043BA RID: 17338
		public int layerMask;

		// Token: 0x040043BB RID: 17339
		public uint seed;

		// Token: 0x040043BC RID: 17340
		public int castCount;

		// Token: 0x040043BD RID: 17341
		public int fishCount;

		// Token: 0x040043BE RID: 17342
		public NativeArray<RaycastCommand> castCommands;

		// Token: 0x040043BF RID: 17343
		public NativeArray<FishShoal.FishData> fishDataArray;

		// Token: 0x040043C0 RID: 17344
		public NativeArray<FishShoal.FishRenderData> fishRenderDataArray;

		// Token: 0x040043C1 RID: 17345
		public NativeArray<int> fishCastIndices;
	}

	// Token: 0x02000C7B RID: 3195
	public struct FishCollisionProcessJob : IJob
	{
		// Token: 0x06004F1F RID: 20255 RVA: 0x001A5420 File Offset: 0x001A3620
		public void Execute()
		{
			for (int i = 0; i < this.castCount; i++)
			{
				if (this.castResults[i].normal != default(Vector3))
				{
					int num = this.fishCastIndices[i];
					FishShoal.FishData fishData = this.fishDataArray[num];
					if (fishData.startleTime <= 0f)
					{
						float2 xz = this.fishRenderDataArray[num].position.xz;
						float2 @float = math.normalize(new float2(this.castResults[i].point.x, this.castResults[i].point.z) - xz);
						float2 float2 = xz - @float * 8f;
						fishData.destinationX = float2.x;
						fishData.destinationZ = float2.y;
						fishData.startleTime = 2f;
						fishData.updateTime = 6f;
						this.fishDataArray[num] = fishData;
					}
				}
			}
		}

		// Token: 0x040043C2 RID: 17346
		public int castCount;

		// Token: 0x040043C3 RID: 17347
		public NativeArray<FishShoal.FishData> fishDataArray;

		// Token: 0x040043C4 RID: 17348
		[global::ReadOnly]
		public NativeArray<RaycastHit> castResults;

		// Token: 0x040043C5 RID: 17349
		[global::ReadOnly]
		public NativeArray<int> fishCastIndices;

		// Token: 0x040043C6 RID: 17350
		[global::ReadOnly]
		public NativeArray<FishShoal.FishRenderData> fishRenderDataArray;
	}

	// Token: 0x02000C7C RID: 3196
	public struct FishUpdateJob : IJobParallelFor
	{
		// Token: 0x06004F20 RID: 20256 RVA: 0x001A5550 File Offset: 0x001A3750
		public unsafe void Execute(int i)
		{
			FishShoal.FishData* ptr = this.fishDataArray + i;
			FishShoal.FishRenderData* ptr2 = this.fishRenderDataArray + i;
			Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)((long)(i * 3245) + (long)((ulong)this.seed)));
			float num = math.distancesq(this.cameraPosition, ptr2->position);
			bool flag = ptr->startleTime > 0f;
			if (num > math.pow(40f, 2f) || ptr2->position.y > this.minDepth)
			{
				ptr->isAlive = false;
				return;
			}
			if (!flag && num < 100f)
			{
				ptr->startleTime = 2f;
				flag = true;
			}
			float3 @float = new float3(ptr->destinationX, ptr2->position.y, ptr->destinationZ);
			if (ptr->updateTime >= 8f || math.distancesq(@float, ptr2->position) < 1f)
			{
				float3 target = FishShoal.GetTarget(new float3(ptr->spawnX, 0f, ptr->spawnZ), ref random);
				ptr->updateTime = 0f;
				ptr->destinationX = target.x;
				ptr->destinationZ = target.z;
			}
			ptr2->scale = math.lerp(ptr2->scale, ptr->scale, this.dt * 5f);
			ptr->speed = math.lerp(ptr->speed, flag ? this.maxSpeed : this.minSpeed, this.dt * 4f);
			float3 float2 = math.normalize(@float - ptr2->position);
			float num2 = math.atan2(float2.z, float2.x);
			ptr2->rotation = -ptr2->rotation + 1.5707964f;
			float num3 = (flag ? this.maxTurnSpeed : this.minTurnSpeed);
			ptr2->rotation = FishShoal.FishUpdateJob.LerpAngle(ptr2->rotation, num2, this.dt * num3);
			float3 zero = float3.zero;
			math.sincos(ptr2->rotation, out zero.z, out zero.x);
			ptr->directionX = zero.x;
			ptr->directionZ = zero.z;
			ptr2->position += zero * ptr->speed * this.dt;
			ptr2->rotation = -ptr2->rotation + 1.5707964f;
			ptr2->distance += ptr->speed * this.dt;
			ptr->updateTime += this.dt;
			ptr->startleTime -= this.dt;
		}

		// Token: 0x06004F21 RID: 20257 RVA: 0x001A57F4 File Offset: 0x001A39F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float LerpAngle(float a0, float a1, float t)
		{
			float num = a1 - a0;
			num = math.clamp(num - math.floor(num / 6.2831855f) * 6.2831855f, 0f, 6.2831855f);
			return math.lerp(a0, a0 + ((num > 3.1415927f) ? (num - 6.2831855f) : num), t);
		}

		// Token: 0x040043C7 RID: 17351
		[global::ReadOnly]
		public float3 cameraPosition;

		// Token: 0x040043C8 RID: 17352
		[global::ReadOnly]
		public uint seed;

		// Token: 0x040043C9 RID: 17353
		[global::ReadOnly]
		public float dt;

		// Token: 0x040043CA RID: 17354
		[global::ReadOnly]
		public float minSpeed;

		// Token: 0x040043CB RID: 17355
		[global::ReadOnly]
		public float maxSpeed;

		// Token: 0x040043CC RID: 17356
		[global::ReadOnly]
		public float minTurnSpeed;

		// Token: 0x040043CD RID: 17357
		[global::ReadOnly]
		public float maxTurnSpeed;

		// Token: 0x040043CE RID: 17358
		[global::ReadOnly]
		public float minDepth;

		// Token: 0x040043CF RID: 17359
		[NativeDisableUnsafePtrRestriction]
		public unsafe FishShoal.FishData* fishDataArray;

		// Token: 0x040043D0 RID: 17360
		[NativeDisableUnsafePtrRestriction]
		public unsafe FishShoal.FishRenderData* fishRenderDataArray;
	}

	// Token: 0x02000C7D RID: 3197
	public struct KillFish : IJob
	{
		// Token: 0x06004F22 RID: 20258 RVA: 0x001A5844 File Offset: 0x001A3A44
		public void Execute()
		{
			int num = this.fishCount[0];
			for (int i = num - 1; i >= 0; i--)
			{
				if (!this.fishDataArray[i].isAlive)
				{
					if (i < num - 1)
					{
						this.fishDataArray[i] = this.fishDataArray[num - 1];
						this.fishRenderDataArray[i] = this.fishRenderDataArray[num - 1];
					}
					num--;
				}
			}
			this.fishCount[0] = num;
		}

		// Token: 0x040043D1 RID: 17361
		public NativeArray<FishShoal.FishData> fishDataArray;

		// Token: 0x040043D2 RID: 17362
		public NativeArray<FishShoal.FishRenderData> fishRenderDataArray;

		// Token: 0x040043D3 RID: 17363
		public NativeArray<int> fishCount;
	}
}
