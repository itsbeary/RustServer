using System;
using UnityEngine;

// Token: 0x0200057B RID: 1403
public class RadialSpawnPoint : BaseSpawnPoint
{
	// Token: 0x06002B0E RID: 11022 RVA: 0x001060BC File Offset: 0x001042BC
	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		Vector2 vector = UnityEngine.Random.insideUnitCircle * this.radius;
		pos = base.transform.position + new Vector3(vector.x, 0f, vector.y);
		rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		base.DropToGround(ref pos, ref rot);
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x00106132 File Offset: 0x00104332
	public override bool HasPlayersIntersecting()
	{
		return BaseNetworkable.HasCloseConnections(base.transform.position, this.radius + 1f);
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ObjectRetired(SpawnPointInstance instance)
	{
	}

	// Token: 0x0400232F RID: 9007
	public float radius = 10f;
}
