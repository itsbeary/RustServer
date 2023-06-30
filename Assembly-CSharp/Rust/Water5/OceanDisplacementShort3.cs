using System;
using Unity.Mathematics;
using UnityEngine;

namespace Rust.Water5
{
	// Token: 0x02000B2C RID: 2860
	public struct OceanDisplacementShort3
	{
		// Token: 0x0600453B RID: 17723 RVA: 0x00194DE8 File Offset: 0x00192FE8
		public static implicit operator Vector3(OceanDisplacementShort3 v)
		{
			return new Vector3
			{
				x = 3.051944E-05f * (float)v.x * 20f,
				y = 3.051944E-05f * (float)v.y * 20f,
				z = 3.051944E-05f * (float)v.z * 20f
			};
		}

		// Token: 0x0600453C RID: 17724 RVA: 0x00194E4C File Offset: 0x0019304C
		public static implicit operator OceanDisplacementShort3(Vector3 v)
		{
			return new OceanDisplacementShort3
			{
				x = (short)(v.x / 20f * 32766f + 0.5f),
				y = (short)(v.y / 20f * 32766f + 0.5f),
				z = (short)(v.z / 20f * 32766f + 0.5f)
			};
		}

		// Token: 0x0600453D RID: 17725 RVA: 0x00194EC4 File Offset: 0x001930C4
		public static implicit operator OceanDisplacementShort3(float3 v)
		{
			return new OceanDisplacementShort3
			{
				x = (short)(v.x / 20f * 32766f + 0.5f),
				y = (short)(v.y / 20f * 32766f + 0.5f),
				z = (short)(v.z / 20f * 32766f + 0.5f)
			};
		}

		// Token: 0x04003E16 RID: 15894
		private const float precision = 20f;

		// Token: 0x04003E17 RID: 15895
		private const float float2short = 32766f;

		// Token: 0x04003E18 RID: 15896
		private const float short2float = 3.051944E-05f;

		// Token: 0x04003E19 RID: 15897
		public short x;

		// Token: 0x04003E1A RID: 15898
		public short y;

		// Token: 0x04003E1B RID: 15899
		public short z;
	}
}
