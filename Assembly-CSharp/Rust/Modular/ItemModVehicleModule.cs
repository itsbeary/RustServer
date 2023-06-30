using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B3D RID: 2877
	public class ItemModVehicleModule : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x060045A6 RID: 17830 RVA: 0x001968DE File Offset: 0x00194ADE
		public int SocketsTaken
		{
			get
			{
				return this.socketsTaken;
			}
		}

		// Token: 0x060045A7 RID: 17831 RVA: 0x001968E8 File Offset: 0x00194AE8
		public BaseVehicleModule CreateModuleEntity(BaseEntity parent, Vector3 position, Quaternion rotation)
		{
			if (!this.entityPrefab.isValid)
			{
				Debug.LogError("Invalid entity prefab for module");
				return null;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, position, rotation, true);
			BaseVehicleModule baseVehicleModule = null;
			if (baseEntity != null)
			{
				if (parent != null)
				{
					baseEntity.SetParent(parent, true, false);
					baseEntity.canTriggerParent = false;
				}
				baseEntity.Spawn();
				baseVehicleModule = baseEntity.GetComponent<BaseVehicleModule>();
				if (this.doNonUserSpawn)
				{
					this.doNonUserSpawn = false;
					baseVehicleModule.NonUserSpawn();
				}
			}
			return baseVehicleModule;
		}

		// Token: 0x04003E9E RID: 16030
		public GameObjectRef entityPrefab;

		// Token: 0x04003E9F RID: 16031
		[Range(1f, 2f)]
		public int socketsTaken = 1;

		// Token: 0x04003EA0 RID: 16032
		public bool doNonUserSpawn;
	}
}
