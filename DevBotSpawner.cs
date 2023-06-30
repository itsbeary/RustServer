using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class DevBotSpawner : FacepunchBehaviour
{
	// Token: 0x06001F0E RID: 7950 RVA: 0x000D33F8 File Offset: 0x000D15F8
	public bool HasFreePopulation()
	{
		for (int i = this._spawned.Count - 1; i >= 0; i--)
		{
			BaseEntity baseEntity = this._spawned[i];
			if (baseEntity == null || baseEntity.Health() <= 0f)
			{
				this._spawned.Remove(baseEntity);
			}
		}
		return this._spawned.Count < this.maxPopulation;
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x000D3464 File Offset: 0x000D1664
	public void SpawnBot()
	{
		while (this.HasFreePopulation())
		{
			Vector3 position = this.waypoints[0].position;
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.bot.resourcePath, position, default(Quaternion), true);
			if (baseEntity == null)
			{
				return;
			}
			this._spawned.Add(baseEntity);
			baseEntity.SendMessage("SetWaypoints", this.waypoints, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x000D34D8 File Offset: 0x000D16D8
	public void Start()
	{
		this.waypoints = this.waypointParent.GetComponentsInChildren<Transform>();
		base.InvokeRepeating(new Action(this.SpawnBot), 5f, this.spawnRate);
	}

	// Token: 0x040017FB RID: 6139
	public GameObjectRef bot;

	// Token: 0x040017FC RID: 6140
	public Transform waypointParent;

	// Token: 0x040017FD RID: 6141
	public bool autoSelectLatestSpawnedGameObject = true;

	// Token: 0x040017FE RID: 6142
	public float spawnRate = 1f;

	// Token: 0x040017FF RID: 6143
	public int maxPopulation = 1;

	// Token: 0x04001800 RID: 6144
	private Transform[] waypoints;

	// Token: 0x04001801 RID: 6145
	private List<BaseEntity> _spawned = new List<BaseEntity>();
}
