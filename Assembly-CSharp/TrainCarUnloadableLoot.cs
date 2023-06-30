using System;
using Rust;
using UnityEngine;

// Token: 0x020004B9 RID: 1209
public class TrainCarUnloadableLoot : TrainCarUnloadable
{
	// Token: 0x06002791 RID: 10129 RVA: 0x000F7464 File Offset: 0x000F5664
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			int num = UnityEngine.Random.Range(0, this.lootLayouts.Length);
			for (int i = 0; i < this.lootLayouts[num].crates.Length; i++)
			{
				GameObjectRef gameObjectRef = this.lootLayouts[num].crates[i];
				BaseEntity baseEntity = GameManager.server.CreateEntity(gameObjectRef.resourcePath, this.lootPositions[i].localPosition, this.lootPositions[i].localRotation, true);
				if (baseEntity != null)
				{
					baseEntity.Spawn();
					baseEntity.SetParent(this, false, false);
				}
			}
		}
	}

	// Token: 0x0400202E RID: 8238
	[SerializeField]
	private TrainCarUnloadableLoot.LootCrateSet[] lootLayouts;

	// Token: 0x0400202F RID: 8239
	[SerializeField]
	private Transform[] lootPositions;

	// Token: 0x02000D28 RID: 3368
	[Serializable]
	public class LootCrateSet
	{
		// Token: 0x040046EC RID: 18156
		public GameObjectRef[] crates;
	}
}
