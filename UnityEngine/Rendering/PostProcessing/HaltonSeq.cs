using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA0 RID: 2720
	public static class HaltonSeq
	{
		// Token: 0x060040B5 RID: 16565 RVA: 0x0017CEC4 File Offset: 0x0017B0C4
		public static float Get(int index, int radix)
		{
			float num = 0f;
			float num2 = 1f / (float)radix;
			while (index > 0)
			{
				num += (float)(index % radix) * num2;
				index /= radix;
				num2 /= (float)radix;
			}
			return num;
		}
	}
}
