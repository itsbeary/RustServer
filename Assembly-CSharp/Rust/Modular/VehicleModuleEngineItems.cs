using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B39 RID: 2873
	[CreateAssetMenu(fileName = "Vehicle Module Engine Items", menuName = "Rust/Vehicles/Module Engine Items")]
	public class VehicleModuleEngineItems : ScriptableObject
	{
		// Token: 0x0600459C RID: 17820 RVA: 0x00196740 File Offset: 0x00194940
		public bool TryGetItem(int tier, EngineStorage.EngineItemTypes type, out ItemModEngineItem output)
		{
			List<ItemModEngineItem> list = Pool.GetList<ItemModEngineItem>();
			bool flag = false;
			output = null;
			foreach (ItemModEngineItem itemModEngineItem in this.engineItems)
			{
				if (itemModEngineItem.tier == tier && itemModEngineItem.engineItemType == type)
				{
					list.Add(itemModEngineItem);
				}
			}
			if (list.Count > 0)
			{
				output = list.GetRandom<ItemModEngineItem>();
				flag = true;
			}
			Pool.FreeList<ItemModEngineItem>(ref list);
			return flag;
		}

		// Token: 0x04003E92 RID: 16018
		[SerializeField]
		private ItemModEngineItem[] engineItems;
	}
}
