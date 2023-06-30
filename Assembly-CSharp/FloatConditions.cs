using System;

// Token: 0x02000938 RID: 2360
[Serializable]
public class FloatConditions
{
	// Token: 0x06003886 RID: 14470 RVA: 0x00150650 File Offset: 0x0014E850
	public bool AllTrue(float val)
	{
		foreach (FloatConditions.Condition condition in this.conditions)
		{
			if (!condition.Test(val))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040033A5 RID: 13221
	public FloatConditions.Condition[] conditions;

	// Token: 0x02000ECE RID: 3790
	[Serializable]
	public struct Condition
	{
		// Token: 0x0600536B RID: 21355 RVA: 0x001B26AC File Offset: 0x001B08AC
		public bool Test(float val)
		{
			switch (this.type)
			{
			case FloatConditions.Condition.Types.Equal:
				return val == this.value;
			case FloatConditions.Condition.Types.NotEqual:
				return val != this.value;
			case FloatConditions.Condition.Types.Higher:
				return val > this.value;
			case FloatConditions.Condition.Types.Lower:
				return val < this.value;
			default:
				return false;
			}
		}

		// Token: 0x04004D65 RID: 19813
		public FloatConditions.Condition.Types type;

		// Token: 0x04004D66 RID: 19814
		public float value;

		// Token: 0x02000FE8 RID: 4072
		public enum Types
		{
			// Token: 0x040051A6 RID: 20902
			Equal,
			// Token: 0x040051A7 RID: 20903
			NotEqual,
			// Token: 0x040051A8 RID: 20904
			Higher,
			// Token: 0x040051A9 RID: 20905
			Lower
		}
	}
}
