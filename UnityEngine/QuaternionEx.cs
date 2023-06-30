using System;

namespace UnityEngine
{
	// Token: 0x02000A2F RID: 2607
	public static class QuaternionEx
	{
		// Token: 0x06003DB7 RID: 15799 RVA: 0x001697A8 File Offset: 0x001679A8
		public static Quaternion AlignToNormal(this Quaternion rot, Vector3 normal)
		{
			return Quaternion.FromToRotation(Vector3.up, normal) * rot;
		}

		// Token: 0x06003DB8 RID: 15800 RVA: 0x001697BB File Offset: 0x001679BB
		public static Quaternion LookRotationWithOffset(Vector3 offset, Vector3 forward, Vector3 up)
		{
			return Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(offset, Vector3.up));
		}

		// Token: 0x06003DB9 RID: 15801 RVA: 0x001697E0 File Offset: 0x001679E0
		public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
		{
			if (forward == up)
			{
				return Quaternion.LookRotation(up);
			}
			Vector3 vector = Vector3.Cross(forward, up);
			forward = Vector3.Cross(up, vector);
			return Quaternion.LookRotation(forward, up);
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x00169818 File Offset: 0x00167A18
		public static Quaternion LookRotationGradient(Vector3 normal, Vector3 up)
		{
			Vector3 vector = ((normal == Vector3.up) ? Vector3.forward : Vector3.Cross(normal, Vector3.up));
			return QuaternionEx.LookRotationForcedUp(Vector3.Cross(normal, vector), up);
		}

		// Token: 0x06003DBB RID: 15803 RVA: 0x00169854 File Offset: 0x00167A54
		public static Quaternion LookRotationNormal(Vector3 normal, Vector3 up = default(Vector3))
		{
			if (up != Vector3.zero)
			{
				return QuaternionEx.LookRotationForcedUp(up, normal);
			}
			if (normal == Vector3.up)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.forward, normal);
			}
			if (normal == Vector3.down)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.back, normal);
			}
			if (normal.y == 0f)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.up, normal);
			}
			Vector3 vector = Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(-Vector3.Cross(normal, vector), normal);
		}

		// Token: 0x06003DBC RID: 15804 RVA: 0x001698DF File Offset: 0x00167ADF
		public static Quaternion EnsureValid(this Quaternion rot, float epsilon = 1E-45f)
		{
			if (Quaternion.Dot(rot, rot) < epsilon)
			{
				return Quaternion.identity;
			}
			return rot;
		}
	}
}
