using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009DF RID: 2527
	public sealed class ProxyObject : Variant, IEnumerable<KeyValuePair<string, Variant>>, IEnumerable
	{
		// Token: 0x06003C2B RID: 15403 RVA: 0x00162A62 File Offset: 0x00160C62
		public ProxyObject()
		{
			this.dict = new Dictionary<string, Variant>();
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x00162A75 File Offset: 0x00160C75
		IEnumerator<KeyValuePair<string, Variant>> IEnumerable<KeyValuePair<string, Variant>>.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x00162A75 File Offset: 0x00160C75
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x00162A87 File Offset: 0x00160C87
		public void Add(string key, Variant item)
		{
			this.dict.Add(key, item);
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x00162A96 File Offset: 0x00160C96
		public bool TryGetValue(string key, out Variant item)
		{
			return this.dict.TryGetValue(key, out item);
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06003C30 RID: 15408 RVA: 0x00162AA8 File Offset: 0x00160CA8
		public string TypeHint
		{
			get
			{
				Variant variant;
				if (this.TryGetValue("@type", out variant))
				{
					return variant.ToString(CultureInfo.InvariantCulture);
				}
				return null;
			}
		}

		// Token: 0x170004DF RID: 1247
		public override Variant this[string key]
		{
			get
			{
				return this.dict[key];
			}
			set
			{
				this.dict[key] = value;
			}
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06003C33 RID: 15411 RVA: 0x00162AEE File Offset: 0x00160CEE
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06003C34 RID: 15412 RVA: 0x00162AFB File Offset: 0x00160CFB
		public Dictionary<string, Variant>.KeyCollection Keys
		{
			get
			{
				return this.dict.Keys;
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06003C35 RID: 15413 RVA: 0x00162B08 File Offset: 0x00160D08
		public Dictionary<string, Variant>.ValueCollection Values
		{
			get
			{
				return this.dict.Values;
			}
		}

		// Token: 0x040036E3 RID: 14051
		public const string TypeHintKey = "@type";

		// Token: 0x040036E4 RID: 14052
		private readonly Dictionary<string, Variant> dict;
	}
}
