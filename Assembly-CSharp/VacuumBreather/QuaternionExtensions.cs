using System;
using UnityEngine;

namespace VacuumBreather
{
	// Token: 0x020009CC RID: 2508
	public static class QuaternionExtensions
	{
		// Token: 0x06003BC0 RID: 15296 RVA: 0x00160A32 File Offset: 0x0015EC32
		public static Quaternion Multiply(this Quaternion quaternion, float scalar)
		{
			return new Quaternion((float)((double)quaternion.x * (double)scalar), (float)((double)quaternion.y * (double)scalar), (float)((double)quaternion.z * (double)scalar), (float)((double)quaternion.w * (double)scalar));
		}

		// Token: 0x06003BC1 RID: 15297 RVA: 0x00160A68 File Offset: 0x0015EC68
		public static Quaternion RequiredRotation(Quaternion from, Quaternion to)
		{
			Quaternion quaternion = to * Quaternion.Inverse(from);
			if (quaternion.w < 0f)
			{
				quaternion.x *= -1f;
				quaternion.y *= -1f;
				quaternion.z *= -1f;
				quaternion.w *= -1f;
			}
			return quaternion;
		}

		// Token: 0x06003BC2 RID: 15298 RVA: 0x00160AD0 File Offset: 0x0015ECD0
		public static Quaternion Subtract(this Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion((float)((double)lhs.x - (double)rhs.x), (float)((double)lhs.y - (double)rhs.y), (float)((double)lhs.z - (double)rhs.z), (float)((double)lhs.w - (double)rhs.w));
		}
	}
}
