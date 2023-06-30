using System;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009DE RID: 2526
	public sealed class ProxyNumber : Variant
	{
		// Token: 0x06003C1A RID: 15386 RVA: 0x00162888 File Offset: 0x00160A88
		public ProxyNumber(IConvertible value)
		{
			string text = value as string;
			this.value = ((text != null) ? ProxyNumber.Parse(text) : value);
		}

		// Token: 0x06003C1B RID: 15387 RVA: 0x001628B4 File Offset: 0x00160AB4
		private static IConvertible Parse(string value)
		{
			if (value.IndexOfAny(ProxyNumber.floatingPointCharacters) == -1)
			{
				ulong num2;
				if (value[0] == '-')
				{
					long num;
					if (long.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num))
					{
						return num;
					}
				}
				else if (ulong.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num2))
				{
					return num2;
				}
			}
			decimal num3;
			if (decimal.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num3))
			{
				double num4;
				if (num3 == 0m && double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num4) && Math.Abs(num4) > 5E-324)
				{
					return num4;
				}
				return num3;
			}
			else
			{
				double num5;
				if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num5))
				{
					return num5;
				}
				return 0;
			}
		}

		// Token: 0x06003C1C RID: 15388 RVA: 0x00162987 File Offset: 0x00160B87
		public override bool ToBoolean(IFormatProvider provider)
		{
			return this.value.ToBoolean(provider);
		}

		// Token: 0x06003C1D RID: 15389 RVA: 0x00162995 File Offset: 0x00160B95
		public override byte ToByte(IFormatProvider provider)
		{
			return this.value.ToByte(provider);
		}

		// Token: 0x06003C1E RID: 15390 RVA: 0x001629A3 File Offset: 0x00160BA3
		public override char ToChar(IFormatProvider provider)
		{
			return this.value.ToChar(provider);
		}

		// Token: 0x06003C1F RID: 15391 RVA: 0x001629B1 File Offset: 0x00160BB1
		public override decimal ToDecimal(IFormatProvider provider)
		{
			return this.value.ToDecimal(provider);
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x001629BF File Offset: 0x00160BBF
		public override double ToDouble(IFormatProvider provider)
		{
			return this.value.ToDouble(provider);
		}

		// Token: 0x06003C21 RID: 15393 RVA: 0x001629CD File Offset: 0x00160BCD
		public override short ToInt16(IFormatProvider provider)
		{
			return this.value.ToInt16(provider);
		}

		// Token: 0x06003C22 RID: 15394 RVA: 0x001629DB File Offset: 0x00160BDB
		public override int ToInt32(IFormatProvider provider)
		{
			return this.value.ToInt32(provider);
		}

		// Token: 0x06003C23 RID: 15395 RVA: 0x001629E9 File Offset: 0x00160BE9
		public override long ToInt64(IFormatProvider provider)
		{
			return this.value.ToInt64(provider);
		}

		// Token: 0x06003C24 RID: 15396 RVA: 0x001629F7 File Offset: 0x00160BF7
		public override sbyte ToSByte(IFormatProvider provider)
		{
			return this.value.ToSByte(provider);
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x00162A05 File Offset: 0x00160C05
		public override float ToSingle(IFormatProvider provider)
		{
			return this.value.ToSingle(provider);
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x00162A13 File Offset: 0x00160C13
		public override string ToString(IFormatProvider provider)
		{
			return this.value.ToString(provider);
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x00162A21 File Offset: 0x00160C21
		public override ushort ToUInt16(IFormatProvider provider)
		{
			return this.value.ToUInt16(provider);
		}

		// Token: 0x06003C28 RID: 15400 RVA: 0x00162A2F File Offset: 0x00160C2F
		public override uint ToUInt32(IFormatProvider provider)
		{
			return this.value.ToUInt32(provider);
		}

		// Token: 0x06003C29 RID: 15401 RVA: 0x00162A3D File Offset: 0x00160C3D
		public override ulong ToUInt64(IFormatProvider provider)
		{
			return this.value.ToUInt64(provider);
		}

		// Token: 0x040036E1 RID: 14049
		private static readonly char[] floatingPointCharacters = new char[] { '.', 'e' };

		// Token: 0x040036E2 RID: 14050
		private readonly IConvertible value;
	}
}
