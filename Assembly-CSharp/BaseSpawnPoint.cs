using System;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	// Token: 0x06002AE0 RID: 10976
	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	// Token: 0x06002AE1 RID: 10977
	public abstract void ObjectSpawned(SpawnPointInstance instance);

	// Token: 0x06002AE2 RID: 10978
	public abstract void ObjectRetired(SpawnPointInstance instance);

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0010587C File Offset: 0x00103A7C
	public virtual bool IsAvailableTo(GameObjectRef prefabRef)
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x00105889 File Offset: 0x00103A89
	public virtual bool HasPlayersIntersecting()
	{
		return BaseNetworkable.HasCloseConnections(base.transform.position, 2f);
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x001058A0 File Offset: 0x00103AA0
	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		RaycastHit raycastHit;
		if (TransformUtil.GetGroundInfo(pos, out raycastHit, 20f, 1235288065, null))
		{
			pos = raycastHit.point;
			rot = Quaternion.LookRotation(rot * Vector3.forward, raycastHit.normal);
		}
	}
}
