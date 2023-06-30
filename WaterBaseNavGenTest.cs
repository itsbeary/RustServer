using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000022 RID: 34
public class WaterBaseNavGenTest : MonoBehaviour
{
	// Token: 0x060000C3 RID: 195 RVA: 0x00005F14 File Offset: 0x00004114
	[ContextMenu("Nav Gen")]
	public void NavGen()
	{
		DungeonNavmesh dungeonNavmesh = base.gameObject.AddComponent<DungeonNavmesh>();
		dungeonNavmesh.NavmeshResolutionModifier = 0.3f;
		dungeonNavmesh.NavMeshCollectGeometry = NavMeshCollectGeometry.PhysicsColliders;
		dungeonNavmesh.LayerMask = 65537;
		this.co = dungeonNavmesh.UpdateNavMeshAndWait();
		base.StartCoroutine(this.co);
	}

	// Token: 0x040000C3 RID: 195
	private IEnumerator co;
}
