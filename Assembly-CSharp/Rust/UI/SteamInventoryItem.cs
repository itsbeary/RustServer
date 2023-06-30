using System;
using Facepunch.Extend;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B1C RID: 2844
	public class SteamInventoryItem : MonoBehaviour
	{
		// Token: 0x06004527 RID: 17703 RVA: 0x00194BC0 File Offset: 0x00192DC0
		public bool Setup(IPlayerItem item)
		{
			this.Item = item;
			if (item.GetDefinition() == null)
			{
				return false;
			}
			base.transform.FindChildRecursive("ItemName").GetComponent<TextMeshProUGUI>().text = item.GetDefinition().Name;
			return this.Image.Load(item.GetDefinition().IconUrl);
		}

		// Token: 0x04003DC9 RID: 15817
		public IPlayerItem Item;

		// Token: 0x04003DCA RID: 15818
		public HttpImage Image;
	}
}
