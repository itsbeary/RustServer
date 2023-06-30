using System;
using ConVar;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class JunkpileNPCSpawner : NPCSpawner
{
	// Token: 0x06002B06 RID: 11014 RVA: 0x00105E38 File Offset: 0x00104038
	protected override void Spawn(int numToSpawn)
	{
		if (this.UseSpawnChance && UnityEngine.Random.value > AI.npc_junkpilespawn_chance)
		{
			return;
		}
		base.Spawn(numToSpawn);
	}

	// Token: 0x04002320 RID: 8992
	[Header("Junkpile NPC Spawner")]
	public bool UseSpawnChance;
}
