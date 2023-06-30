using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B1E RID: 2846
	public class SteamInventoryNewItem : MonoBehaviour
	{
		// Token: 0x0600452A RID: 17706 RVA: 0x00194C24 File Offset: 0x00192E24
		public async Task Open(IPlayerItem item)
		{
			base.gameObject.SetActive(true);
			base.GetComponentInChildren<SteamInventoryItem>().Setup(item);
			while (this && base.gameObject.activeSelf)
			{
				await Task.Delay(100);
			}
		}
	}
}
