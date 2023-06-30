using System;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class BunkerEntrance : BaseEntity, IMissionEntityListener
{
	// Token: 0x0600181D RID: 6173 RVA: 0x000B5218 File Offset: 0x000B3418
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.portalPrefab.isValid)
		{
			this.portalInstance = GameManager.server.CreateEntity(this.portalPrefab.resourcePath, this.portalSpawnPoint.position, this.portalSpawnPoint.rotation, true).GetComponent<BasePortal>();
			this.portalInstance.SetParent(this, true, false);
			this.portalInstance.Spawn();
		}
		if (this.doorPrefab.isValid)
		{
			this.doorInstance = GameManager.server.CreateEntity(this.doorPrefab.resourcePath, this.doorSpawnPoint.position, this.doorSpawnPoint.rotation, true).GetComponent<Door>();
			this.doorInstance.SetParent(this, true, false);
			this.doorInstance.Spawn();
		}
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x000063A5 File Offset: 0x000045A5
	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x000063A5 File Offset: 0x000045A5
	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x040010EA RID: 4330
	public GameObjectRef portalPrefab;

	// Token: 0x040010EB RID: 4331
	public GameObjectRef doorPrefab;

	// Token: 0x040010EC RID: 4332
	public Transform portalSpawnPoint;

	// Token: 0x040010ED RID: 4333
	public Transform doorSpawnPoint;

	// Token: 0x040010EE RID: 4334
	public Door doorInstance;

	// Token: 0x040010EF RID: 4335
	public BasePortal portalInstance;
}
