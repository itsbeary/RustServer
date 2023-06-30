using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B4C RID: 2892
	public class NavmeshPrefabInstantiator : MonoBehaviour
	{
		// Token: 0x0600460F RID: 17935 RVA: 0x00198615 File Offset: 0x00196815
		private void Start()
		{
			if (this.NavmeshPrefab != null)
			{
				this.NavmeshPrefab.Instantiate(base.transform).SetActive(true);
				UnityEngine.Object.Destroy(this);
			}
		}

		// Token: 0x04003EEE RID: 16110
		public GameObjectRef NavmeshPrefab;
	}
}
