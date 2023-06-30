using System;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000211 RID: 529
public class RealmedNavMeshObstacle : BasePrefab
{
	// Token: 0x06001BA5 RID: 7077 RVA: 0x000C38B0 File Offset: 0x000C1AB0
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		base.PreProcess(process, rootObj, name, serverside, clientside, false);
		if (base.isServer && this.Obstacle)
		{
			if (AiManager.nav_disable)
			{
				process.RemoveComponent(this.Obstacle);
				this.Obstacle = null;
			}
			else if (AiManager.nav_obstacles_carve_state >= 2)
			{
				this.Obstacle.carving = true;
			}
			else if (AiManager.nav_obstacles_carve_state == 1)
			{
				this.Obstacle.carving = this.Obstacle.gameObject.layer == 21;
			}
			else
			{
				this.Obstacle.carving = false;
			}
		}
		process.RemoveComponent(this);
	}

	// Token: 0x04001382 RID: 4994
	public NavMeshObstacle Obstacle;
}
