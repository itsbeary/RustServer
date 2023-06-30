using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace TinyJSON
{
	// Token: 0x020009D0 RID: 2512
	public sealed class Encoder
	{
		// Token: 0x06003BD9 RID: 15321 RVA: 0x001610F6 File Offset: 0x0015F2F6
		private Encoder(EncodeOptions options)
		{
			this.options = options;
			this.builder = new StringBuilder();
			this.indent = 0;
		}

		// Token: 0x06003BDA RID: 15322 RVA: 0x00161117 File Offset: 0x0015F317
		public static string Encode(object obj)
		{
			return Encoder.Encode(obj, EncodeOptions.None);
		}

		// Token: 0x06003BDB RID: 15323 RVA: 0x00161120 File Offset: 0x0015F320
		public static string Encode(object obj, EncodeOptions options)
		{
			Encoder encoder = new Encoder(options);
			encoder.EncodeValue(obj, false);
			return encoder.builder.ToString();
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06003BDC RID: 15324 RVA: 0x0016113A File Offset: 0x0015F33A
		private bool PrettyPrintEnabled
		{
			get
			{
				return (this.options & EncodeOptions.PrettyPrint) == EncodeOptions.PrettyPrint;
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06003BDD RID: 15325 RVA: 0x00161147 File Offset: 0x0015F347
		private bool TypeHintsEnabled
		{
			get
			{
				return (this.options & EncodeOptions.NoTypeHints) != EncodeOptions.NoTypeHints;
			}
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06003BDE RID: 15326 RVA: 0x00161157 File Offset: 0x0015F357
		private bool IncludePublicPropertiesEnabled
		{
			get
			{
				return (this.options & EncodeOptions.IncludePublicProperties) == EncodeOptions.IncludePublicProperties;
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06003BDF RID: 15327 RVA: 0x00161164 File Offset: 0x0015F364
		private bool EnforceHierarchyOrderEnabled
		{
			get
			{
				return (this.options & EncodeOptions.EnforceHierarchyOrder) == EncodeOptions.EnforceHierarchyOrder;
			}
		}

		// Token: 0x06003BE0 RID: 15328 RVA: 0x00161174 File Offset: 0x0015F374
		private void EncodeValue(object value, bool forceTypeHint)
		{
			if (value == null)
			{
				this.builder.Append("null");
				return;
			}
			if (value is string)
			{
				this.EncodeString((string)value);
				return;
			}
			if (value is ProxyString)
			{
				this.EncodeString(((ProxyString)value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (value is char)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is bool)
			{
				this.builder.Append(((bool)value) ? "true" : "false");
				return;
			}
			if (value is Enum)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is Array)
			{
				this.EncodeArray((Array)value, forceTypeHint);
				return;
			}
			if (value is IList)
			{
				this.EncodeList((IList)value, forceTypeHint);
				return;
			}
			if (value is IDictionary)
			{
				this.EncodeDictionary((IDictionary)value, forceTypeHint);
				return;
			}
			if (value is Guid)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is ProxyArray)
			{
				this.EncodeProxyArray((ProxyArray)value);
				return;
			}
			if (value is ProxyObject)
			{
				this.EncodeProxyObject((ProxyObject)value);
				return;
			}
			if (value is float || value is double || value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal || value is ProxyBoolean || value is ProxyNumber)
			{
				this.builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
				return;
			}
			this.EncodeObject(value, forceTypeHint);
		}

		// Token: 0x06003BE1 RID: 15329 RVA: 0x00161328 File Offset: 0x0015F528
		private IEnumerable<FieldInfo> GetFieldsForType(Type type)
		{
			if (this.EnforceHierarchyOrderEnabled)
			{
				Stack<Type> stack = new Stack<Type>();
				while (type != null)
				{
					stack.Push(type);
					type = type.BaseType;
				}
				List<FieldInfo> list = new List<FieldInfo>();
				while (stack.Count > 0)
				{
					list.AddRange(stack.Pop().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				}
				return list;
			}
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06003BE2 RID: 15330 RVA: 0x0016138C File Offset: 0x0015F58C
		private IEnumerable<PropertyInfo> GetPropertiesForType(Type type)
		{
			if (this.EnforceHierarchyOrderEnabled)
			{
				Stack<Type> stack = new Stack<Type>();
				while (type != null)
				{
					stack.Push(type);
					type = type.BaseType;
				}
				List<PropertyInfo> list = new List<PropertyInfo>();
				while (stack.Count > 0)
				{
					list.AddRange(stack.Pop().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				}
				return list;
			}
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06003BE3 RID: 15331 RVA: 0x001613F0 File Offset: 0x0015F5F0
		private void EncodeObject(object value, bool forceTypeHint)
		{
			Type type = value.GetType();
			this.AppendOpenBrace();
			forceTypeHint = forceTypeHint || this.TypeHintsEnabled;
			bool includePublicPropertiesEnabled = this.IncludePublicPropertiesEnabled;
			bool flag = !forceTypeHint;
			if (forceTypeHint)
			{
				if (this.PrettyPrintEnabled)
				{
					this.AppendIndent();
				}
				this.EncodeString("@type");
				this.AppendColon();
				this.EncodeString(type.FullName);
				flag = false;
			}
			foreach (FieldInfo fieldInfo in this.GetFieldsForType(type))
			{
				bool flag2 = false;
				bool flag3 = fieldInfo.IsPublic;
				foreach (object obj in fieldInfo.GetCustomAttributes(true))
				{
					if (Encoder.excludeAttrType.IsInstanceOfType(obj))
					{
						flag3 = false;
					}
					if (Encoder.includeAttrType.IsInstanceOfType(obj))
					{
						flag3 = true;
					}
					if (Encoder.typeHintAttrType.IsInstanceOfType(obj))
					{
						flag2 = true;
					}
				}
				if (flag3)
				{
					this.AppendComma(flag);
					this.EncodeString(fieldInfo.Name);
					this.AppendColon();
					this.EncodeValue(fieldInfo.GetValue(value), flag2);
					flag = false;
				}
			}
			foreach (PropertyInfo propertyInfo in this.GetPropertiesForType(type))
			{
				if (propertyInfo.CanRead)
				{
					bool flag4 = false;
					bool flag5 = includePublicPropertiesEnabled;
					foreach (object obj2 in propertyInfo.GetCustomAttributes(true))
					{
						if (Encoder.excludeAttrType.IsInstanceOfType(obj2))
						{
							flag5 = false;
						}
						if (Encoder.includeAttrType.IsInstanceOfType(obj2))
						{
							flag5 = true;
						}
						if (Encoder.typeHintAttrType.IsInstanceOfType(obj2))
						{
							flag4 = true;
						}
					}
					if (flag5)
					{
						this.AppendComma(flag);
						this.EncodeString(propertyInfo.Name);
						this.AppendColon();
						this.EncodeValue(propertyInfo.GetValue(value, null), flag4);
						flag = false;
					}
				}
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003BE4 RID: 15332 RVA: 0x0016160C File Offset: 0x0015F80C
		private void EncodeProxyArray(ProxyArray value)
		{
			if (value.Count == 0)
			{
				this.builder.Append("[]");
				return;
			}
			this.AppendOpenBracket();
			bool flag = true;
			foreach (Variant variant in ((IEnumerable<Variant>)value))
			{
				this.AppendComma(flag);
				this.EncodeValue(variant, false);
				flag = false;
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003BE5 RID: 15333 RVA: 0x00161688 File Offset: 0x0015F888
		private void EncodeProxyObject(ProxyObject value)
		{
			if (value.Count == 0)
			{
				this.builder.Append("{}");
				return;
			}
			this.AppendOpenBrace();
			bool flag = true;
			foreach (string text in value.Keys)
			{
				this.AppendComma(flag);
				this.EncodeString(text);
				this.AppendColon();
				this.EncodeValue(value[text], false);
				flag = false;
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003BE6 RID: 15334 RVA: 0x00161720 File Offset: 0x0015F920
		private void EncodeDictionary(IDictionary value, bool forceTypeHint)
		{
			if (value.Count == 0)
			{
				this.builder.Append("{}");
				return;
			}
			this.AppendOpenBrace();
			bool flag = true;
			foreach (object obj in value.Keys)
			{
				this.AppendComma(flag);
				this.EncodeString(obj.ToString());
				this.AppendColon();
				this.EncodeValue(value[obj], forceTypeHint);
				flag = false;
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003BE7 RID: 15335 RVA: 0x001617C0 File Offset: 0x0015F9C0
		private void EncodeList(IList value, bool forceTypeHint)
		{
			if (value.Count == 0)
			{
				this.builder.Append("[]");
				return;
			}
			this.AppendOpenBracket();
			bool flag = true;
			foreach (object obj in value)
			{
				this.AppendComma(flag);
				this.EncodeValue(obj, forceTypeHint);
				flag = false;
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003BE8 RID: 15336 RVA: 0x00161844 File Offset: 0x0015FA44
		private void EncodeArray(Array value, bool forceTypeHint)
		{
			if (value.Rank == 1)
			{
				this.EncodeList(value, forceTypeHint);
				return;
			}
			int[] array = new int[value.Rank];
			this.EncodeArrayRank(value, 0, array, forceTypeHint);
		}

		// Token: 0x06003BE9 RID: 15337 RVA: 0x0016187C File Offset: 0x0015FA7C
		private void EncodeArrayRank(Array value, int rank, int[] indices, bool forceTypeHint)
		{
			this.AppendOpenBracket();
			int lowerBound = value.GetLowerBound(rank);
			int upperBound = value.GetUpperBound(rank);
			if (rank == value.Rank - 1)
			{
				for (int i = lowerBound; i <= upperBound; i++)
				{
					indices[rank] = i;
					this.AppendComma(i == lowerBound);
					this.EncodeValue(value.GetValue(indices), forceTypeHint);
				}
			}
			else
			{
				for (int j = lowerBound; j <= upperBound; j++)
				{
					indices[rank] = j;
					this.AppendComma(j == lowerBound);
					this.EncodeArrayRank(value, rank + 1, indices, forceTypeHint);
				}
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003BEA RID: 15338 RVA: 0x00161904 File Offset: 0x0015FB04
		private void EncodeString(string value)
		{
			this.builder.Append('"');
			char[] array = value.ToCharArray();
			int i = 0;
			while (i < array.Length)
			{
				char c = array[i];
				switch (c)
				{
				case '\b':
					this.builder.Append("\\b");
					break;
				case '\t':
					this.builder.Append("\\t");
					break;
				case '\n':
					this.builder.Append("\\n");
					break;
				case '\v':
					goto IL_DD;
				case '\f':
					this.builder.Append("\\f");
					break;
				case '\r':
					this.builder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_DD;
						}
						this.builder.Append("\\\\");
					}
					else
					{
						this.builder.Append("\\\"");
					}
					break;
				}
				IL_123:
				i++;
				continue;
				IL_DD:
				int num = Convert.ToInt32(c);
				if (num >= 32 && num <= 126)
				{
					this.builder.Append(c);
					goto IL_123;
				}
				this.builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
				goto IL_123;
			}
			this.builder.Append('"');
		}

		// Token: 0x06003BEB RID: 15339 RVA: 0x00161A50 File Offset: 0x0015FC50
		private void AppendIndent()
		{
			for (int i = 0; i < this.indent; i++)
			{
				this.builder.Append('\t');
			}
		}

		// Token: 0x06003BEC RID: 15340 RVA: 0x00161A7C File Offset: 0x0015FC7C
		private void AppendOpenBrace()
		{
			this.builder.Append('{');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent++;
			}
		}

		// Token: 0x06003BED RID: 15341 RVA: 0x00161AB0 File Offset: 0x0015FCB0
		private void AppendCloseBrace()
		{
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent--;
				this.AppendIndent();
			}
			this.builder.Append('}');
		}

		// Token: 0x06003BEE RID: 15342 RVA: 0x00161AEA File Offset: 0x0015FCEA
		private void AppendOpenBracket()
		{
			this.builder.Append('[');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent++;
			}
		}

		// Token: 0x06003BEF RID: 15343 RVA: 0x00161B1E File Offset: 0x0015FD1E
		private void AppendCloseBracket()
		{
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent--;
				this.AppendIndent();
			}
			this.builder.Append(']');
		}

		// Token: 0x06003BF0 RID: 15344 RVA: 0x00161B58 File Offset: 0x0015FD58
		private void AppendComma(bool firstItem)
		{
			if (!firstItem)
			{
				this.builder.Append(',');
				if (this.PrettyPrintEnabled)
				{
					this.builder.Append('\n');
				}
			}
			if (this.PrettyPrintEnabled)
			{
				this.AppendIndent();
			}
		}

		// Token: 0x06003BF1 RID: 15345 RVA: 0x00161B8F File Offset: 0x0015FD8F
		private void AppendColon()
		{
			this.builder.Append(':');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append(' ');
			}
		}

		// Token: 0x040036CD RID: 14029
		private static readonly Type includeAttrType = typeof(Include);

		// Token: 0x040036CE RID: 14030
		private static readonly Type excludeAttrType = typeof(Exclude);

		// Token: 0x040036CF RID: 14031
		private static readonly Type typeHintAttrType = typeof(TypeHint);

		// Token: 0x040036D0 RID: 14032
		private readonly StringBuilder builder;

		// Token: 0x040036D1 RID: 14033
		private readonly EncodeOptions options;

		// Token: 0x040036D2 RID: 14034
		private int indent;
	}
}
