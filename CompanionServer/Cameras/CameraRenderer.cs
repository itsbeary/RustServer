using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A18 RID: 2584
	public class CameraRenderer : Pool.IPooled
	{
		// Token: 0x06003D68 RID: 15720 RVA: 0x00168203 File Offset: 0x00166403
		public CameraRenderer()
		{
			this.Reset();
		}

		// Token: 0x06003D69 RID: 15721 RVA: 0x00168227 File Offset: 0x00166427
		public void EnterPool()
		{
			this.Reset();
		}

		// Token: 0x06003D6A RID: 15722 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x06003D6B RID: 15723 RVA: 0x00168230 File Offset: 0x00166430
		public void Reset()
		{
			this._knownColliders.Clear();
			this._colliderToEntity.Clear();
			this._lastRenderTimestamp = 0.0;
			this._fieldOfView = 0f;
			this._sampleOffset = 0;
			this._nextSampleOffset = 0;
			this._sampleCount = 0;
			if (this._task != null)
			{
				CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
				if (instance != null)
				{
					instance.ReturnTask(ref this._task);
				}
			}
			this._cachedViewerSteamId = null;
			this._cachedViewer = null;
			this.state = CameraRendererState.Invalid;
			this.rc = null;
			this.entity = null;
		}

		// Token: 0x06003D6C RID: 15724 RVA: 0x001682D0 File Offset: 0x001664D0
		public void Init(IRemoteControllable remoteControllable)
		{
			if (remoteControllable == null)
			{
				throw new ArgumentNullException("remoteControllable");
			}
			this.rc = remoteControllable;
			this.entity = remoteControllable.GetEnt();
			if (this.entity == null || !this.entity.IsValid())
			{
				throw new ArgumentException("RemoteControllable's entity is null or invalid", "rc");
			}
			this.state = CameraRendererState.WaitingToRender;
		}

		// Token: 0x06003D6D RID: 15725 RVA: 0x00168330 File Offset: 0x00166530
		public bool CanRender()
		{
			return this.state == CameraRendererState.WaitingToRender && TimeEx.realtimeSinceStartup - this._lastRenderTimestamp >= (double)CameraRenderer.renderInterval;
		}

		// Token: 0x06003D6E RID: 15726 RVA: 0x00168354 File Offset: 0x00166554
		public void Render(int maxSampleCount)
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this.state != CameraRendererState.WaitingToRender)
			{
				throw new InvalidOperationException(string.Format("CameraRenderer cannot render in state {0}", this.state));
			}
			if (this.rc.IsUnityNull<IRemoteControllable>() || !this.entity.IsValid())
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			Transform eyes = this.rc.GetEyes();
			if (eyes == null)
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this._task != null)
			{
				Debug.LogError("CameraRenderer: Trying to render but a task is already allocated?", this.entity);
				instance.ReturnTask(ref this._task);
			}
			this._fieldOfView = CameraRenderer.verticalFov / Mathf.Clamp(this.rc.GetFovScale(), 1f, 8f);
			this._sampleCount = Mathf.Clamp(CameraRenderer.samplesPerRender, 1, Mathf.Min(CameraRenderer.width * CameraRenderer.height, maxSampleCount));
			this._task = instance.BorrowTask();
			this._nextSampleOffset = this._task.Start(CameraRenderer.width, CameraRenderer.height, this._fieldOfView, CameraRenderer.nearPlane, CameraRenderer.farPlane, CameraRenderer.layerMask, eyes, this._sampleCount, this._sampleOffset, this._knownColliders);
			this.state = CameraRendererState.Rendering;
		}

		// Token: 0x06003D6F RID: 15727 RVA: 0x001684A0 File Offset: 0x001666A0
		public void CompleteRender()
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this.state != CameraRendererState.Rendering)
			{
				throw new InvalidOperationException(string.Format("CameraRenderer cannot complete render in state {0}", this.state));
			}
			if (this._task == null)
			{
				Debug.LogError("CameraRenderer: Trying to complete render but no task is allocated?", this.entity);
				this.state = CameraRendererState.Invalid;
				return;
			}
			if (this._task.keepWaiting)
			{
				return;
			}
			if (this.rc.IsUnityNull<IRemoteControllable>() || !this.entity.IsValid())
			{
				instance.ReturnTask(ref this._task);
				this.state = CameraRendererState.Invalid;
				return;
			}
			Transform eyes = this.rc.GetEyes();
			if (eyes == null)
			{
				instance.ReturnTask(ref this._task);
				this.state = CameraRendererState.Invalid;
				return;
			}
			int num = this._sampleCount * 4;
			byte[] array = System.Buffers.ArrayPool<byte>.Shared.Rent(num);
			List<int> list = Pool.GetList<int>();
			List<int> list2 = Pool.GetList<int>();
			int num2 = this._task.ExtractRayData(array, list, list2);
			instance.ReturnTask(ref this._task);
			this.UpdateCollidersMap(list2);
			Pool.FreeList<int>(ref list);
			Pool.FreeList<int>(ref list2);
			CameraViewerId? cameraViewerId;
			ulong num3 = ((this.rc.ControllingViewerId != null) ? cameraViewerId.GetValueOrDefault().SteamId : 0UL);
			if (num3 == 0UL)
			{
				this._cachedViewerSteamId = null;
				this._cachedViewer = null;
			}
			else
			{
				ulong num4 = num3;
				ulong? cachedViewerSteamId = this._cachedViewerSteamId;
				if (!((num4 == cachedViewerSteamId.GetValueOrDefault()) & (cachedViewerSteamId != null)))
				{
					this._cachedViewerSteamId = new ulong?(num3);
					this._cachedViewer = global::BasePlayer.FindByID(num3) ?? global::BasePlayer.FindSleeping(num3);
				}
			}
			float num5 = (this._cachedViewer.IsValid() ? Mathf.Clamp01(Vector3.Distance(this._cachedViewer.transform.position, this.entity.transform.position) / this.rc.MaxRange) : 0f);
			Vector3 position = eyes.position;
			Quaternion rotation = eyes.rotation;
			Matrix4x4 worldToLocalMatrix = eyes.worldToLocalMatrix;
			NetworkableId id = this.entity.net.ID;
			CameraRenderer._entityIdMap.Clear();
			AppBroadcast appBroadcast = Pool.Get<AppBroadcast>();
			appBroadcast.cameraRays = Pool.Get<AppCameraRays>();
			appBroadcast.cameraRays.verticalFov = this._fieldOfView;
			appBroadcast.cameraRays.sampleOffset = this._sampleOffset;
			appBroadcast.cameraRays.rayData = new ArraySegment<byte>(array, 0, num2);
			appBroadcast.cameraRays.distance = num5;
			appBroadcast.cameraRays.entities = Pool.GetList<AppCameraRays.Entity>();
			appBroadcast.cameraRays.timeOfDay = ((TOD_Sky.Instance != null) ? TOD_Sky.Instance.LerpValue : 1f);
			foreach (global::BaseEntity baseEntity in this._colliderToEntity.Values)
			{
				if (baseEntity.IsValid())
				{
					Vector3 position2 = baseEntity.transform.position;
					float num6 = Vector3.Distance(position2, position);
					if (num6 <= (float)CameraRenderer.entityMaxDistance)
					{
						string text = null;
						global::BasePlayer basePlayer;
						if ((basePlayer = baseEntity as global::BasePlayer) != null)
						{
							if (num6 > (float)CameraRenderer.playerMaxDistance)
							{
								continue;
							}
							if (num6 <= (float)CameraRenderer.playerNameMaxDistance)
							{
								text = basePlayer.displayName;
							}
						}
						AppCameraRays.Entity entity = Pool.Get<AppCameraRays.Entity>();
						entity.entityId = CameraRenderer.RandomizeEntityId(baseEntity.net.ID);
						entity.type = ((baseEntity is TreeEntity) ? AppCameraRays.EntityType.Tree : AppCameraRays.EntityType.Player);
						entity.position = worldToLocalMatrix.MultiplyPoint3x4(position2);
						entity.rotation = (Quaternion.Inverse(baseEntity.transform.rotation) * rotation).eulerAngles * 0.017453292f;
						entity.size = Vector3.Scale(baseEntity.bounds.size, baseEntity.transform.localScale);
						entity.name = text;
						appBroadcast.cameraRays.entities.Add(entity);
					}
				}
			}
			appBroadcast.cameraRays.entities.Sort((AppCameraRays.Entity x, AppCameraRays.Entity y) => x.entityId.Value.CompareTo(y.entityId.Value));
			Server.Broadcast(new CameraTarget(id), appBroadcast);
			this._sampleOffset = this._nextSampleOffset;
			if (!Server.HasAnySubscribers(new CameraTarget(id)))
			{
				this.state = CameraRendererState.Invalid;
				return;
			}
			this._lastRenderTimestamp = TimeEx.realtimeSinceStartup;
			this.state = CameraRendererState.WaitingToRender;
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x0016893C File Offset: 0x00166B3C
		private void UpdateCollidersMap(List<int> foundColliderIds)
		{
			List<int> list = Pool.GetList<int>();
			foreach (int num in this._knownColliders.Keys)
			{
				list.Add(num);
			}
			List<int> list2 = Pool.GetList<int>();
			foreach (int num2 in list)
			{
				ValueTuple<byte, int> valueTuple;
				if (this._knownColliders.TryGetValue(num2, out valueTuple))
				{
					if (valueTuple.Item2 > CameraRenderer.entityMaxAge)
					{
						list2.Add(num2);
					}
					else
					{
						this._knownColliders[num2] = new ValueTuple<byte, int>(valueTuple.Item1, valueTuple.Item2 + 1);
					}
				}
			}
			Pool.FreeList<int>(ref list);
			foreach (int num3 in list2)
			{
				this._knownColliders.Remove(num3);
				this._colliderToEntity.Remove(num3);
			}
			Pool.FreeList<int>(ref list2);
			foreach (int num4 in foundColliderIds)
			{
				if (this._knownColliders.Count >= 512)
				{
					break;
				}
				Collider collider = BurstUtil.GetCollider(num4);
				if (!(collider == null))
				{
					byte b;
					if (collider is TerrainCollider)
					{
						b = 1;
					}
					else
					{
						global::BaseEntity baseEntity = collider.ToBaseEntity();
						b = CameraRenderer.GetMaterialIndex(collider.sharedMaterial, baseEntity);
						if (baseEntity is TreeEntity || baseEntity is global::BasePlayer)
						{
							this._colliderToEntity[num4] = baseEntity;
						}
					}
					this._knownColliders[num4] = new ValueTuple<byte, int>(b, 0);
				}
			}
		}

		// Token: 0x06003D71 RID: 15729 RVA: 0x00168B4C File Offset: 0x00166D4C
		private static NetworkableId RandomizeEntityId(NetworkableId realId)
		{
			NetworkableId networkableId;
			if (CameraRenderer._entityIdMap.TryGetValue(realId, out networkableId))
			{
				return networkableId;
			}
			NetworkableId networkableId2;
			do
			{
				networkableId2 = new NetworkableId((ulong)((long)UnityEngine.Random.Range(0, 2500)));
			}
			while (CameraRenderer._entityIdMap.ContainsKey(networkableId2));
			CameraRenderer._entityIdMap.Add(realId, networkableId2);
			return networkableId2;
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x00168B98 File Offset: 0x00166D98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte GetMaterialIndex(PhysicMaterial material, global::BaseEntity entity)
		{
			string name = material.GetName();
			if (name == "Water")
			{
				return 2;
			}
			if (name == "Rock")
			{
				return 3;
			}
			if (name == "Stones")
			{
				return 4;
			}
			if (name == "Wood")
			{
				return 5;
			}
			if (name == "Metal")
			{
				return 6;
			}
			if (entity != null && entity is global::BasePlayer)
			{
				return 7;
			}
			return 0;
		}

		// Token: 0x04003790 RID: 14224
		[ServerVar]
		public static bool enabled = true;

		// Token: 0x04003791 RID: 14225
		[ServerVar]
		public static float completionFrameBudgetMs = 5f;

		// Token: 0x04003792 RID: 14226
		[ServerVar]
		public static int maxRendersPerFrame = 25;

		// Token: 0x04003793 RID: 14227
		[ServerVar]
		public static int maxRaysPerFrame = 100000;

		// Token: 0x04003794 RID: 14228
		[ServerVar]
		public static int width = 160;

		// Token: 0x04003795 RID: 14229
		[ServerVar]
		public static int height = 90;

		// Token: 0x04003796 RID: 14230
		[ServerVar]
		public static float verticalFov = 65f;

		// Token: 0x04003797 RID: 14231
		[ServerVar]
		public static float nearPlane = 0f;

		// Token: 0x04003798 RID: 14232
		[ServerVar]
		public static float farPlane = 250f;

		// Token: 0x04003799 RID: 14233
		[ServerVar]
		public static int layerMask = 1218656529;

		// Token: 0x0400379A RID: 14234
		[ServerVar]
		public static float renderInterval = 0.05f;

		// Token: 0x0400379B RID: 14235
		[ServerVar]
		public static int samplesPerRender = 3000;

		// Token: 0x0400379C RID: 14236
		[ServerVar]
		public static int entityMaxAge = 5;

		// Token: 0x0400379D RID: 14237
		[ServerVar]
		public static int entityMaxDistance = 100;

		// Token: 0x0400379E RID: 14238
		[ServerVar]
		public static int playerMaxDistance = 30;

		// Token: 0x0400379F RID: 14239
		[ServerVar]
		public static int playerNameMaxDistance = 10;

		// Token: 0x040037A0 RID: 14240
		private static readonly Dictionary<NetworkableId, NetworkableId> _entityIdMap = new Dictionary<NetworkableId, NetworkableId>();

		// Token: 0x040037A1 RID: 14241
		[TupleElementNames(new string[] { "MaterialIndex", "Age" })]
		private readonly Dictionary<int, ValueTuple<byte, int>> _knownColliders = new Dictionary<int, ValueTuple<byte, int>>();

		// Token: 0x040037A2 RID: 14242
		private readonly Dictionary<int, global::BaseEntity> _colliderToEntity = new Dictionary<int, global::BaseEntity>();

		// Token: 0x040037A3 RID: 14243
		private double _lastRenderTimestamp;

		// Token: 0x040037A4 RID: 14244
		private float _fieldOfView;

		// Token: 0x040037A5 RID: 14245
		private int _sampleOffset;

		// Token: 0x040037A6 RID: 14246
		private int _nextSampleOffset;

		// Token: 0x040037A7 RID: 14247
		private int _sampleCount;

		// Token: 0x040037A8 RID: 14248
		private CameraRenderTask _task;

		// Token: 0x040037A9 RID: 14249
		private ulong? _cachedViewerSteamId;

		// Token: 0x040037AA RID: 14250
		private global::BasePlayer _cachedViewer;

		// Token: 0x040037AB RID: 14251
		public CameraRendererState state;

		// Token: 0x040037AC RID: 14252
		public IRemoteControllable rc;

		// Token: 0x040037AD RID: 14253
		public global::BaseEntity entity;
	}
}
