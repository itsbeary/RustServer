using System;

namespace UnityEngine
{
	// Token: 0x02000A30 RID: 2608
	public static class RayEx
	{
		// Token: 0x06003DBD RID: 15805 RVA: 0x001698F2 File Offset: 0x00167AF2
		public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
		{
			return ray.origin + Vector3.Dot(pos - ray.origin, ray.direction) * ray.direction;
		}

		// Token: 0x06003DBE RID: 15806 RVA: 0x00169928 File Offset: 0x00167B28
		public static float Distance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).magnitude;
		}

		// Token: 0x06003DBF RID: 15807 RVA: 0x00169958 File Offset: 0x00167B58
		public static float SqrDistance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).sqrMagnitude;
		}

		// Token: 0x06003DC0 RID: 15808 RVA: 0x00169986 File Offset: 0x00167B86
		public static bool IsNaNOrInfinity(this Ray r)
		{
			return r.origin.IsNaNOrInfinity() || r.direction.IsNaNOrInfinity();
		}
	}
}
