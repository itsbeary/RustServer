using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8C RID: 2700
	[Serializable]
	public sealed class Vector4Parameter : ParameterOverride<Vector4>
	{
		// Token: 0x06004031 RID: 16433 RVA: 0x0017A7AC File Offset: 0x001789AC
		public override void Interp(Vector4 from, Vector4 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
			this.value.z = from.z + (to.z - from.z) * t;
			this.value.w = from.w + (to.w - from.w) * t;
		}

		// Token: 0x06004032 RID: 16434 RVA: 0x0017A83D File Offset: 0x00178A3D
		public static implicit operator Vector2(Vector4Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x06004033 RID: 16435 RVA: 0x0017A84A File Offset: 0x00178A4A
		public static implicit operator Vector3(Vector4Parameter prop)
		{
			return prop.value;
		}
	}
}
