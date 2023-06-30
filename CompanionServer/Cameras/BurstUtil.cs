using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A16 RID: 2582
	internal static class BurstUtil
	{
		// Token: 0x06003D64 RID: 15716 RVA: 0x00168188 File Offset: 0x00166388
		public unsafe static ref readonly T GetReadonly<[IsUnmanaged] T>(this NativeArray<T> array, int index) where T : struct, ValueType
		{
			T* unsafeReadOnlyPtr = (T*)array.GetUnsafeReadOnlyPtr<T>();
			return unsafeReadOnlyPtr + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
		}

		// Token: 0x06003D65 RID: 15717 RVA: 0x001681AC File Offset: 0x001663AC
		public unsafe static ref T Get<[IsUnmanaged] T>(this NativeArray<T> array, int index) where T : struct, ValueType
		{
			T* unsafePtr = (T*)array.GetUnsafePtr<T>();
			return ref unsafePtr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}

		// Token: 0x06003D66 RID: 15718 RVA: 0x001681D0 File Offset: 0x001663D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetColliderId(this RaycastHit hit)
		{
			return ((BurstUtil.RaycastHitPublic*)(&hit))->m_Collider;
		}

		// Token: 0x06003D67 RID: 15719 RVA: 0x001681DC File Offset: 0x001663DC
		public unsafe static Collider GetCollider(int colliderInstanceId)
		{
			BurstUtil.RaycastHitPublic raycastHitPublic = new BurstUtil.RaycastHitPublic
			{
				m_Collider = colliderInstanceId
			};
			return ((RaycastHit*)(&raycastHitPublic))->collider;
		}

		// Token: 0x02000F0A RID: 3850
		private struct RaycastHitPublic
		{
			// Token: 0x04004E80 RID: 20096
			public Vector3 m_Point;

			// Token: 0x04004E81 RID: 20097
			public Vector3 m_Normal;

			// Token: 0x04004E82 RID: 20098
			public uint m_FaceID;

			// Token: 0x04004E83 RID: 20099
			public float m_Distance;

			// Token: 0x04004E84 RID: 20100
			public Vector2 m_UV;

			// Token: 0x04004E85 RID: 20101
			public int m_Collider;
		}
	}
}
