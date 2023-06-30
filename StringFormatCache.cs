using System;
using System.Collections.Generic;

// Token: 0x02000768 RID: 1896
public static class StringFormatCache
{
	// Token: 0x060034A3 RID: 13475 RVA: 0x00144E38 File Offset: 0x00143038
	public static string Get(string format, string value1)
	{
		StringFormatCache.Key1 key = new StringFormatCache.Key1(format, value1);
		string text;
		if (!StringFormatCache.dict1.TryGetValue(key, out text))
		{
			text = string.Format(format, value1);
			StringFormatCache.dict1.Add(key, text);
		}
		return text;
	}

	// Token: 0x060034A4 RID: 13476 RVA: 0x00144E74 File Offset: 0x00143074
	public static string Get(string format, string value1, string value2)
	{
		StringFormatCache.Key2 key = new StringFormatCache.Key2(format, value1, value2);
		string text;
		if (!StringFormatCache.dict2.TryGetValue(key, out text))
		{
			text = string.Format(format, value1, value2);
			StringFormatCache.dict2.Add(key, text);
		}
		return text;
	}

	// Token: 0x060034A5 RID: 13477 RVA: 0x00144EB0 File Offset: 0x001430B0
	public static string Get(string format, string value1, string value2, string value3)
	{
		StringFormatCache.Key3 key = new StringFormatCache.Key3(format, value1, value2, value3);
		string text;
		if (!StringFormatCache.dict3.TryGetValue(key, out text))
		{
			text = string.Format(format, value1, value2, value3);
			StringFormatCache.dict3.Add(key, text);
		}
		return text;
	}

	// Token: 0x060034A6 RID: 13478 RVA: 0x00144EF0 File Offset: 0x001430F0
	public static string Get(string format, string value1, string value2, string value3, string value4)
	{
		StringFormatCache.Key4 key = new StringFormatCache.Key4(format, value1, value2, value3, value4);
		string text;
		if (!StringFormatCache.dict4.TryGetValue(key, out text))
		{
			text = string.Format(format, new object[] { value1, value2, value3, value4 });
			StringFormatCache.dict4.Add(key, text);
		}
		return text;
	}

	// Token: 0x04002B42 RID: 11074
	private static Dictionary<StringFormatCache.Key1, string> dict1 = new Dictionary<StringFormatCache.Key1, string>();

	// Token: 0x04002B43 RID: 11075
	private static Dictionary<StringFormatCache.Key2, string> dict2 = new Dictionary<StringFormatCache.Key2, string>();

	// Token: 0x04002B44 RID: 11076
	private static Dictionary<StringFormatCache.Key3, string> dict3 = new Dictionary<StringFormatCache.Key3, string>();

	// Token: 0x04002B45 RID: 11077
	private static Dictionary<StringFormatCache.Key4, string> dict4 = new Dictionary<StringFormatCache.Key4, string>();

	// Token: 0x02000E84 RID: 3716
	private struct Key1 : IEquatable<StringFormatCache.Key1>
	{
		// Token: 0x060052DC RID: 21212 RVA: 0x001B152B File Offset: 0x001AF72B
		public Key1(string format, string value1)
		{
			this.format = format;
			this.value1 = value1;
		}

		// Token: 0x060052DD RID: 21213 RVA: 0x001B153B File Offset: 0x001AF73B
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode();
		}

		// Token: 0x060052DE RID: 21214 RVA: 0x001B1554 File Offset: 0x001AF754
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key1 && this.Equals((StringFormatCache.Key1)other);
		}

		// Token: 0x060052DF RID: 21215 RVA: 0x001B156C File Offset: 0x001AF76C
		public bool Equals(StringFormatCache.Key1 other)
		{
			return this.format == other.format && this.value1 == other.value1;
		}

		// Token: 0x04004C5C RID: 19548
		public string format;

		// Token: 0x04004C5D RID: 19549
		public string value1;
	}

	// Token: 0x02000E85 RID: 3717
	private struct Key2 : IEquatable<StringFormatCache.Key2>
	{
		// Token: 0x060052E0 RID: 21216 RVA: 0x001B1594 File Offset: 0x001AF794
		public Key2(string format, string value1, string value2)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
		}

		// Token: 0x060052E1 RID: 21217 RVA: 0x001B15AB File Offset: 0x001AF7AB
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode();
		}

		// Token: 0x060052E2 RID: 21218 RVA: 0x001B15D0 File Offset: 0x001AF7D0
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key2 && this.Equals((StringFormatCache.Key2)other);
		}

		// Token: 0x060052E3 RID: 21219 RVA: 0x001B15E8 File Offset: 0x001AF7E8
		public bool Equals(StringFormatCache.Key2 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2;
		}

		// Token: 0x04004C5E RID: 19550
		public string format;

		// Token: 0x04004C5F RID: 19551
		public string value1;

		// Token: 0x04004C60 RID: 19552
		public string value2;
	}

	// Token: 0x02000E86 RID: 3718
	private struct Key3 : IEquatable<StringFormatCache.Key3>
	{
		// Token: 0x060052E4 RID: 21220 RVA: 0x001B1623 File Offset: 0x001AF823
		public Key3(string format, string value1, string value2, string value3)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

		// Token: 0x060052E5 RID: 21221 RVA: 0x001B1642 File Offset: 0x001AF842
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode();
		}

		// Token: 0x060052E6 RID: 21222 RVA: 0x001B1673 File Offset: 0x001AF873
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key3 && this.Equals((StringFormatCache.Key3)other);
		}

		// Token: 0x060052E7 RID: 21223 RVA: 0x001B168C File Offset: 0x001AF88C
		public bool Equals(StringFormatCache.Key3 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3;
		}

		// Token: 0x04004C61 RID: 19553
		public string format;

		// Token: 0x04004C62 RID: 19554
		public string value1;

		// Token: 0x04004C63 RID: 19555
		public string value2;

		// Token: 0x04004C64 RID: 19556
		public string value3;
	}

	// Token: 0x02000E87 RID: 3719
	private struct Key4 : IEquatable<StringFormatCache.Key4>
	{
		// Token: 0x060052E8 RID: 21224 RVA: 0x001B16E5 File Offset: 0x001AF8E5
		public Key4(string format, string value1, string value2, string value3, string value4)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
			this.value4 = value4;
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x001B170C File Offset: 0x001AF90C
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode() ^ this.value4.GetHashCode();
		}

		// Token: 0x060052EA RID: 21226 RVA: 0x001B1749 File Offset: 0x001AF949
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key4 && this.Equals((StringFormatCache.Key4)other);
		}

		// Token: 0x060052EB RID: 21227 RVA: 0x001B1764 File Offset: 0x001AF964
		public bool Equals(StringFormatCache.Key4 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3 && this.value4 == other.value4;
		}

		// Token: 0x04004C65 RID: 19557
		public string format;

		// Token: 0x04004C66 RID: 19558
		public string value1;

		// Token: 0x04004C67 RID: 19559
		public string value2;

		// Token: 0x04004C68 RID: 19560
		public string value3;

		// Token: 0x04004C69 RID: 19561
		public string value4;
	}
}
