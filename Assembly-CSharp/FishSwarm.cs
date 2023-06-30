using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class FishSwarm : MonoBehaviour
{
	// Token: 0x06001B4B RID: 6987 RVA: 0x000C18D4 File Offset: 0x000BFAD4
	private void Awake()
	{
		this.fishShoals = new FishShoal[this.fishTypes.Length];
		for (int i = 0; i < this.fishTypes.Length; i++)
		{
			this.fishShoals[i] = new FishShoal(this.fishTypes[i]);
		}
		base.StartCoroutine(this.SpawnFish());
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x000C192D File Offset: 0x000BFB2D
	private IEnumerator SpawnFish()
	{
		for (;;)
		{
			if (!TerrainMeta.WaterMap || !TerrainMeta.HeightMap)
			{
				yield return CoroutineEx.waitForEndOfFrame;
			}
			else
			{
				if (this.lastFishUpdatePosition != null && Vector3.Distance(base.transform.position, this.lastFishUpdatePosition.Value) < 5f)
				{
					yield return CoroutineEx.waitForEndOfFrame;
				}
				FishShoal[] array = this.fishShoals;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].TrySpawn(base.transform.position);
					yield return CoroutineEx.waitForEndOfFrame;
				}
				array = null;
			}
		}
		yield break;
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x000C193C File Offset: 0x000BFB3C
	private void Update()
	{
		FishShoal[] array = this.fishShoals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnUpdate(base.transform.position);
		}
	}

	// Token: 0x06001B4E RID: 6990 RVA: 0x000C1978 File Offset: 0x000BFB78
	private void LateUpdate()
	{
		FishShoal[] array = this.fishShoals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnLateUpdate(base.transform.position);
		}
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x000C19B4 File Offset: 0x000BFBB4
	private void OnDestroy()
	{
		FishShoal[] array = this.fishShoals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Dispose();
		}
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x000C19E0 File Offset: 0x000BFBE0
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, 15f);
		Gizmos.DrawWireSphere(base.transform.position, 40f);
		if (!Application.isPlaying)
		{
			return;
		}
		FishShoal[] array = this.fishShoals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnDrawGizmos();
		}
	}

	// Token: 0x04001333 RID: 4915
	public FishShoal.FishType[] fishTypes;

	// Token: 0x04001334 RID: 4916
	public FishShoal[] fishShoals;

	// Token: 0x04001335 RID: 4917
	private Vector3? lastFishUpdatePosition;
}
