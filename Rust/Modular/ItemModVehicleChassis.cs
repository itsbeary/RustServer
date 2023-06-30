using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B3C RID: 2876
	public class ItemModVehicleChassis : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x060045A4 RID: 17828 RVA: 0x001968C7 File Offset: 0x00194AC7
		public int SocketsTaken
		{
			get
			{
				return this.socketsTaken;
			}
		}

		// Token: 0x04003E9C RID: 16028
		public GameObjectRef entityPrefab;

		// Token: 0x04003E9D RID: 16029
		[Range(1f, 6f)]
		public int socketsTaken = 1;
	}
}
