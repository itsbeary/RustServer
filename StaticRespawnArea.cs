using System;
using UnityEngine;

// Token: 0x02000429 RID: 1065
public class StaticRespawnArea : SleepingBag
{
	// Token: 0x06002427 RID: 9255 RVA: 0x000E6CF9 File Offset: 0x000E4EF9
	public override bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		return ignoreTimers || this.allowHostileSpawns || BasePlayer.FindByID(playerID).GetHostileDuration() <= 0f;
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000E6D20 File Offset: 0x000E4F20
	public override void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		Transform transform = this.spawnAreas[UnityEngine.Random.Range(0, this.spawnAreas.Length)];
		pos = transform.transform.position + this.spawnOffset;
		rot = Quaternion.Euler(0f, transform.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000E6D8C File Offset: 0x000E4F8C
	public override void SetUnlockTime(float newTime)
	{
		this.unlockTime = 0f;
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000E6D9C File Offset: 0x000E4F9C
	public override float GetUnlockSeconds(ulong playerID)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(playerID);
		if (basePlayer == null || this.allowHostileSpawns)
		{
			return base.unlockSeconds;
		}
		return Mathf.Max(basePlayer.GetHostileDuration(), base.unlockSeconds);
	}

	// Token: 0x04001C29 RID: 7209
	public Transform[] spawnAreas;

	// Token: 0x04001C2A RID: 7210
	public bool allowHostileSpawns;
}
