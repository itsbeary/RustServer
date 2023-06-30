using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8B RID: 2699
	[Serializable]
	public sealed class Vector3Parameter : ParameterOverride<Vector3>
	{
		// Token: 0x0600402D RID: 16429 RVA: 0x0017A718 File Offset: 0x00178918
		public override void Interp(Vector3 from, Vector3 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
			this.value.z = from.z + (to.z - from.z) * t;
		}

		// Token: 0x0600402E RID: 16430 RVA: 0x0017A788 File Offset: 0x00178988
		public static implicit operator Vector2(Vector3Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x0600402F RID: 16431 RVA: 0x0017A795 File Offset: 0x00178995
		public static implicit operator Vector4(Vector3Parameter prop)
		{
			return prop.value;
		}
	}
}
