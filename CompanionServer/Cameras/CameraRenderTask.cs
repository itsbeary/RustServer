using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A0F RID: 2575
	public class CameraRenderTask : CustomYieldInstruction, IDisposable
	{
		// Token: 0x06003D4C RID: 15692 RVA: 0x00167044 File Offset: 0x00165244
		public CameraRenderTask()
		{
			this._raycastCommands = new NativeArray<RaycastCommand>(10000, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._raycastHits = new NativeArray<RaycastHit>(10000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._colliderIds = new NativeArray<int>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._colliderMaterials = new NativeArray<byte>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._colliderHits = new NativeArray<int>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._raycastOutput = new NativeArray<int>(10000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._foundCollidersLength = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._foundColliders = new NativeArray<int>(10000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._outputDataLength = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this._outputData = new NativeArray<byte>(40000, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}

		// Token: 0x06003D4D RID: 15693 RVA: 0x00167104 File Offset: 0x00165304
		~CameraRenderTask()
		{
			this.Dispose();
		}

		// Token: 0x06003D4E RID: 15694 RVA: 0x00167130 File Offset: 0x00165330
		public void Dispose()
		{
			this._raycastCommands.Dispose();
			this._raycastHits.Dispose();
			this._colliderIds.Dispose();
			this._colliderMaterials.Dispose();
			this._colliderHits.Dispose();
			this._raycastOutput.Dispose();
			this._foundCollidersLength.Dispose();
			this._foundColliders.Dispose();
			this._outputDataLength.Dispose();
			this._outputData.Dispose();
		}

		// Token: 0x06003D4F RID: 15695 RVA: 0x001671AC File Offset: 0x001653AC
		public new void Reset()
		{
			if (this._pendingJob != null)
			{
				if (!this._pendingJob.Value.IsCompleted)
				{
					Debug.LogWarning("CameraRenderTask is resetting before completion! This will cause it to synchronously block for completion.");
				}
				this._pendingJob.Value.Complete();
			}
			this._pendingJob = null;
			this._sampleCount = 0;
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06003D50 RID: 15696 RVA: 0x0016720C File Offset: 0x0016540C
		public override bool keepWaiting
		{
			get
			{
				return this._pendingJob != null && !this._pendingJob.Value.IsCompleted;
			}
		}

		// Token: 0x06003D51 RID: 15697 RVA: 0x00167240 File Offset: 0x00165440
		public int Start(int width, int height, float verticalFov, float nearPlane, float farPlane, int layerMask, Transform cameraTransform, int sampleCount, int sampleOffset, [TupleElementNames(new string[] { "MaterialIndex", "Age" })] Dictionary<int, ValueTuple<byte, int>> knownColliders)
		{
			if (cameraTransform == null)
			{
				throw new ArgumentNullException("cameraTransform");
			}
			if (sampleCount <= 0 || sampleCount > 10000)
			{
				throw new ArgumentOutOfRangeException("sampleCount");
			}
			if (sampleOffset < 0)
			{
				throw new ArgumentOutOfRangeException("sampleOffset");
			}
			if (knownColliders == null)
			{
				throw new ArgumentNullException("knownColliders");
			}
			if (knownColliders.Count > 512)
			{
				throw new ArgumentException("Too many colliders", "knownColliders");
			}
			if (this._pendingJob != null)
			{
				throw new InvalidOperationException("A render job was already started for this instance.");
			}
			this._sampleCount = sampleCount;
			this._colliderLength = knownColliders.Count;
			int num = 0;
			foreach (KeyValuePair<int, ValueTuple<byte, int>> keyValuePair in knownColliders)
			{
				this._colliderIds[num] = keyValuePair.Key;
				this._colliderMaterials[num] = keyValuePair.Value.Item1;
				num++;
			}
			NativeArray<int2> samplePositions = CameraRenderTask.GetSamplePositions(width, height);
			this._foundCollidersLength[0] = 0;
			RaycastBufferSetupJob raycastBufferSetupJob = new RaycastBufferSetupJob
			{
				colliderIds = this._colliderIds.GetSubArray(0, this._colliderLength),
				colliderMaterials = this._colliderMaterials.GetSubArray(0, this._colliderLength),
				colliderHits = this._colliderHits.GetSubArray(0, this._colliderLength)
			};
			RaycastRaySetupJob raycastRaySetupJob = new RaycastRaySetupJob
			{
				res = new float2((float)width, (float)height),
				halfRes = new float2((float)width / 2f, (float)height / 2f),
				aspectRatio = (float)width / (float)height,
				worldHeight = 2f * Mathf.Tan(0.008726646f * verticalFov),
				cameraPos = cameraTransform.position,
				cameraRot = cameraTransform.rotation,
				nearPlane = nearPlane,
				farPlane = farPlane,
				layerMask = layerMask,
				samplePositions = samplePositions,
				sampleOffset = sampleOffset % samplePositions.Length,
				raycastCommands = this._raycastCommands.GetSubArray(0, sampleCount)
			};
			RaycastRayProcessingJob raycastRayProcessingJob = new RaycastRayProcessingJob
			{
				cameraForward = -cameraTransform.forward,
				farPlane = farPlane,
				raycastHits = this._raycastHits.GetSubArray(0, sampleCount),
				colliderIds = this._colliderIds.GetSubArray(0, this._colliderLength),
				colliderMaterials = this._colliderMaterials.GetSubArray(0, this._colliderLength),
				colliderHits = this._colliderHits.GetSubArray(0, this._colliderLength),
				outputs = this._raycastOutput.GetSubArray(0, sampleCount),
				foundCollidersIndex = this._foundCollidersLength,
				foundColliders = this._foundColliders
			};
			RaycastColliderProcessingJob raycastColliderProcessingJob = new RaycastColliderProcessingJob
			{
				foundCollidersLength = this._foundCollidersLength,
				foundColliders = this._foundColliders
			};
			RaycastOutputCompressJob raycastOutputCompressJob = new RaycastOutputCompressJob
			{
				rayOutputs = this._raycastOutput.GetSubArray(0, sampleCount),
				dataLength = this._outputDataLength,
				data = this._outputData
			};
			JobHandle jobHandle = raycastBufferSetupJob.Schedule(default(JobHandle));
			JobHandle jobHandle2 = raycastRaySetupJob.Schedule(sampleCount, 100, default(JobHandle));
			JobHandle jobHandle3 = RaycastCommand.ScheduleBatch(this._raycastCommands.GetSubArray(0, sampleCount), this._raycastHits.GetSubArray(0, sampleCount), 100, jobHandle2);
			JobHandle jobHandle4 = raycastRayProcessingJob.Schedule(sampleCount, 100, JobHandle.CombineDependencies(jobHandle, jobHandle3));
			JobHandle jobHandle5 = raycastColliderProcessingJob.Schedule(jobHandle4);
			JobHandle jobHandle6 = raycastOutputCompressJob.Schedule(jobHandle4);
			this._pendingJob = new JobHandle?(JobHandle.CombineDependencies(jobHandle6, jobHandle5));
			return sampleOffset + sampleCount;
		}

		// Token: 0x06003D52 RID: 15698 RVA: 0x00167638 File Offset: 0x00165838
		public int ExtractRayData(byte[] buffer, List<int> hitColliderIds = null, List<int> foundColliderIds = null)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = this._sampleCount * 4;
			if (buffer.Length < num)
			{
				throw new ArgumentException("Output buffer is not large enough to hold all the ray data", "buffer");
			}
			if (this._pendingJob == null)
			{
				throw new InvalidOperationException("Job was not started for this CameraRenderTask");
			}
			if (!this._pendingJob.Value.IsCompleted)
			{
				Debug.LogWarning("Trying to extract ray data from CameraRenderTask before completion! This will cause it to synchronously block for completion.");
			}
			this._pendingJob.Value.Complete();
			int num2 = this._outputDataLength[0];
			NativeArray<byte>.Copy(this._outputData.GetSubArray(0, num2), buffer, num2);
			if (hitColliderIds != null)
			{
				hitColliderIds.Clear();
				for (int i = 0; i < this._colliderLength; i++)
				{
					if (this._colliderHits[i] > 0)
					{
						hitColliderIds.Add(this._colliderIds[i]);
					}
				}
			}
			if (foundColliderIds != null)
			{
				foundColliderIds.Clear();
				int num3 = this._foundCollidersLength[0];
				for (int j = 0; j < num3; j++)
				{
					foundColliderIds.Add(this._foundColliders[j]);
				}
			}
			return num2;
		}

		// Token: 0x06003D53 RID: 15699 RVA: 0x00167754 File Offset: 0x00165954
		private static NativeArray<int2> GetSamplePositions(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			ValueTuple<int, int> valueTuple = new ValueTuple<int, int>(width, height);
			NativeArray<int2> nativeArray;
			if (CameraRenderTask._samplePositions.TryGetValue(valueTuple, out nativeArray))
			{
				return nativeArray;
			}
			nativeArray = new NativeArray<int2>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			new RaycastSamplePositionsJob
			{
				res = new int2(width, height),
				random = new Unity.Mathematics.Random(1337U),
				positions = nativeArray
			}.Run<RaycastSamplePositionsJob>();
			CameraRenderTask._samplePositions.Add(valueTuple, nativeArray);
			return nativeArray;
		}

		// Token: 0x06003D54 RID: 15700 RVA: 0x001677E8 File Offset: 0x001659E8
		public static void FreeCachedSamplePositions()
		{
			foreach (KeyValuePair<ValueTuple<int, int>, NativeArray<int2>> keyValuePair in CameraRenderTask._samplePositions)
			{
				keyValuePair.Value.Dispose();
			}
			CameraRenderTask._samplePositions.Clear();
		}

		// Token: 0x0400375C RID: 14172
		public const int MaxSamplesPerRender = 10000;

		// Token: 0x0400375D RID: 14173
		public const int MaxColliders = 512;

		// Token: 0x0400375E RID: 14174
		private static readonly Dictionary<ValueTuple<int, int>, NativeArray<int2>> _samplePositions = new Dictionary<ValueTuple<int, int>, NativeArray<int2>>();

		// Token: 0x0400375F RID: 14175
		private NativeArray<RaycastCommand> _raycastCommands;

		// Token: 0x04003760 RID: 14176
		private NativeArray<RaycastHit> _raycastHits;

		// Token: 0x04003761 RID: 14177
		private NativeArray<int> _colliderIds;

		// Token: 0x04003762 RID: 14178
		private NativeArray<byte> _colliderMaterials;

		// Token: 0x04003763 RID: 14179
		private NativeArray<int> _colliderHits;

		// Token: 0x04003764 RID: 14180
		private NativeArray<int> _raycastOutput;

		// Token: 0x04003765 RID: 14181
		private NativeArray<int> _foundCollidersLength;

		// Token: 0x04003766 RID: 14182
		private NativeArray<int> _foundColliders;

		// Token: 0x04003767 RID: 14183
		private NativeArray<int> _outputDataLength;

		// Token: 0x04003768 RID: 14184
		private NativeArray<byte> _outputData;

		// Token: 0x04003769 RID: 14185
		private JobHandle? _pendingJob;

		// Token: 0x0400376A RID: 14186
		private int _sampleCount;

		// Token: 0x0400376B RID: 14187
		private int _colliderLength;
	}
}
