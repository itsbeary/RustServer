using System;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009E1 RID: 2529
	public abstract class Variant : IConvertible
	{
		// Token: 0x06003C38 RID: 15416 RVA: 0x00162B2C File Offset: 0x00160D2C
		public void Make<T>(out T item)
		{
			JSON.MakeInto<T>(this, out item);
		}

		// Token: 0x06003C39 RID: 15417 RVA: 0x00162B38 File Offset: 0x00160D38
		public T Make<T>()
		{
			T t;
			JSON.MakeInto<T>(this, out t);
			return t;
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x00162B4E File Offset: 0x00160D4E
		public string ToJSON()
		{
			return JSON.Dump(this);
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x00162B56 File Offset: 0x00160D56
		public virtual object ToType(Type conversionType, IFormatProvider provider)
		{
			throw new InvalidCastException(string.Concat(new object[]
			{
				"Cannot convert ",
				base.GetType(),
				" to ",
				conversionType.Name
			}));
		}

		// Token: 0x06003C3D RID: 15421 RVA: 0x00162B8A File Offset: 0x00160D8A
		public virtual DateTime ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to DateTime");
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x00162BA6 File Offset: 0x00160DA6
		public virtual bool ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Boolean");
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x00162BC2 File Offset: 0x00160DC2
		public virtual byte ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Byte");
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x00162BDE File Offset: 0x00160DDE
		public virtual char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Char");
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x00162BFA File Offset: 0x00160DFA
		public virtual decimal ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Decimal");
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x00162C16 File Offset: 0x00160E16
		public virtual double ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Double");
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x00162C32 File Offset: 0x00160E32
		public virtual short ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int16");
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x00162C4E File Offset: 0x00160E4E
		public virtual int ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int32");
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x00162C6A File Offset: 0x00160E6A
		public virtual long ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int64");
		}

		// Token: 0x06003C46 RID: 15430 RVA: 0x00162C86 File Offset: 0x00160E86
		public virtual sbyte ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to SByte");
		}

		// Token: 0x06003C47 RID: 15431 RVA: 0x00162CA2 File Offset: 0x00160EA2
		public virtual float ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Single");
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x00162CBE File Offset: 0x00160EBE
		public virtual string ToString(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to String");
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x00162CDA File Offset: 0x00160EDA
		public virtual ushort ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt16");
		}

		// Token: 0x06003C4A RID: 15434 RVA: 0x00162CF6 File Offset: 0x00160EF6
		public virtual uint ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt32");
		}

		// Token: 0x06003C4B RID: 15435 RVA: 0x00162D12 File Offset: 0x00160F12
		public virtual ulong ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt64");
		}

		// Token: 0x06003C4C RID: 15436 RVA: 0x00162D2E File Offset: 0x00160F2E
		public override string ToString()
		{
			return this.ToString(Variant.FormatProvider);
		}

		// Token: 0x170004E3 RID: 1251
		public virtual Variant this[string key]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x170004E4 RID: 1252
		public virtual Variant this[int index]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06003C51 RID: 15441 RVA: 0x00162D42 File Offset: 0x00160F42
		public static implicit operator bool(Variant variant)
		{
			return variant.ToBoolean(Variant.FormatProvider);
		}

		// Token: 0x06003C52 RID: 15442 RVA: 0x00162D4F File Offset: 0x00160F4F
		public static implicit operator float(Variant variant)
		{
			return variant.ToSingle(Variant.FormatProvider);
		}

		// Token: 0x06003C53 RID: 15443 RVA: 0x00162D5C File Offset: 0x00160F5C
		public static implicit operator double(Variant variant)
		{
			return variant.ToDouble(Variant.FormatProvider);
		}

		// Token: 0x06003C54 RID: 15444 RVA: 0x00162D69 File Offset: 0x00160F69
		public static implicit operator ushort(Variant variant)
		{
			return variant.ToUInt16(Variant.FormatProvider);
		}

		// Token: 0x06003C55 RID: 15445 RVA: 0x00162D76 File Offset: 0x00160F76
		public static implicit operator short(Variant variant)
		{
			return variant.ToInt16(Variant.FormatProvider);
		}

		// Token: 0x06003C56 RID: 15446 RVA: 0x00162D83 File Offset: 0x00160F83
		public static implicit operator uint(Variant variant)
		{
			return variant.ToUInt32(Variant.FormatProvider);
		}

		// Token: 0x06003C57 RID: 15447 RVA: 0x00162D90 File Offset: 0x00160F90
		public static implicit operator int(Variant variant)
		{
			return variant.ToInt32(Variant.FormatProvider);
		}

		// Token: 0x06003C58 RID: 15448 RVA: 0x00162D9D File Offset: 0x00160F9D
		public static implicit operator ulong(Variant variant)
		{
			return variant.ToUInt64(Variant.FormatProvider);
		}

		// Token: 0x06003C59 RID: 15449 RVA: 0x00162DAA File Offset: 0x00160FAA
		public static implicit operator long(Variant variant)
		{
			return variant.ToInt64(Variant.FormatProvider);
		}

		// Token: 0x06003C5A RID: 15450 RVA: 0x00162DB7 File Offset: 0x00160FB7
		public static implicit operator decimal(Variant variant)
		{
			return variant.ToDecimal(Variant.FormatProvider);
		}

		// Token: 0x06003C5B RID: 15451 RVA: 0x00162D2E File Offset: 0x00160F2E
		public static implicit operator string(Variant variant)
		{
			return variant.ToString(Variant.FormatProvider);
		}

		// Token: 0x06003C5C RID: 15452 RVA: 0x00162DC4 File Offset: 0x00160FC4
		public static implicit operator Guid(Variant variant)
		{
			return new Guid(variant.ToString(Variant.FormatProvider));
		}

		// Token: 0x040036E6 RID: 14054
		protected static readonly IFormatProvider FormatProvider = new NumberFormatInfo();
	}
}
