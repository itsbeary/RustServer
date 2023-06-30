using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A41 RID: 2625
	internal static class SetPropertyUtility
	{
		// Token: 0x06003EAB RID: 16043 RVA: 0x0016FB28 File Offset: 0x0016DD28
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003EAC RID: 16044 RVA: 0x0016FB77 File Offset: 0x0016DD77
		public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003EAD RID: 16045 RVA: 0x0016FB92 File Offset: 0x0016DD92
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x0016FBB4 File Offset: 0x0016DDB4
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
