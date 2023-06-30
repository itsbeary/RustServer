using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8A RID: 2698
	[Serializable]
	public sealed class Vector2Parameter : ParameterOverride<Vector2>
	{
		// Token: 0x06004029 RID: 16425 RVA: 0x0017A6A4 File Offset: 0x001788A4
		public override void Interp(Vector2 from, Vector2 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
		}

		// Token: 0x0600402A RID: 16426 RVA: 0x0017A6F3 File Offset: 0x001788F3
		public static implicit operator Vector3(Vector2Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x0600402B RID: 16427 RVA: 0x0017A700 File Offset: 0x00178900
		public static implicit operator Vector4(Vector2Parameter prop)
		{
			return prop.value;
		}
	}
}
