using System;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B0A RID: 2826
	public struct EventRecordField
	{
		// Token: 0x060044F2 RID: 17650 RVA: 0x001939BC File Offset: 0x00191BBC
		public EventRecordField(string key1)
		{
			this.Key1 = key1;
			this.Key2 = null;
			this.String = null;
			this.Number = null;
			this.Float = null;
			this.Vector = null;
			this.Guid = null;
			this.IsObject = false;
		}

		// Token: 0x060044F3 RID: 17651 RVA: 0x00193A18 File Offset: 0x00191C18
		public EventRecordField(string key1, string key2)
		{
			this.Key1 = key1;
			this.Key2 = key2;
			this.String = null;
			this.Number = null;
			this.Float = null;
			this.Vector = null;
			this.Guid = null;
			this.IsObject = false;
		}

		// Token: 0x04003D4F RID: 15695
		public string Key1;

		// Token: 0x04003D50 RID: 15696
		public string Key2;

		// Token: 0x04003D51 RID: 15697
		public string String;

		// Token: 0x04003D52 RID: 15698
		public long? Number;

		// Token: 0x04003D53 RID: 15699
		public double? Float;

		// Token: 0x04003D54 RID: 15700
		public Vector3? Vector;

		// Token: 0x04003D55 RID: 15701
		public Guid? Guid;

		// Token: 0x04003D56 RID: 15702
		public bool IsObject;
	}
}
