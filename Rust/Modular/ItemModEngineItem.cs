using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B3B RID: 2875
	public class ItemModEngineItem : ItemMod
	{
		// Token: 0x04003E9A RID: 16026
		public EngineStorage.EngineItemTypes engineItemType;

		// Token: 0x04003E9B RID: 16027
		[Range(1f, 3f)]
		public int tier = 1;
	}
}
