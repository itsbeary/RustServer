using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004E9 RID: 1257
public class RoadBradleys : TriggeredEvent
{
	// Token: 0x060028C6 RID: 10438 RVA: 0x000FBDED File Offset: 0x000F9FED
	public int GetNumBradleys()
	{
		this.CleanList();
		return this.spawnedAPCs.Count;
	}

	// Token: 0x060028C7 RID: 10439 RVA: 0x000FBE00 File Offset: 0x000FA000
	public int GetDesiredNumber()
	{
		return Mathf.CeilToInt(World.Size / 1000f) * 2;
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x000FBE18 File Offset: 0x000FA018
	private void CleanList()
	{
		for (int i = this.spawnedAPCs.Count - 1; i >= 0; i--)
		{
			if (this.spawnedAPCs[i] == null)
			{
				this.spawnedAPCs.RemoveAt(i);
			}
		}
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x000FBE60 File Offset: 0x000FA060
	private void RunEvent()
	{
		int numBradleys = this.GetNumBradleys();
		int num = this.GetDesiredNumber() - numBradleys;
		if (num <= 0)
		{
			return;
		}
		if (TerrainMeta.Path == null || TerrainMeta.Path.Roads.Count == 0)
		{
			return;
		}
		Debug.Log("Spawning :" + num + "Bradleys");
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = Vector3.zero;
			PathList pathList = TerrainMeta.Path.Roads[UnityEngine.Random.Range(0, TerrainMeta.Path.Roads.Count)];
			vector = pathList.Path.Points[UnityEngine.Random.Range(0, pathList.Path.Points.Length)];
			BradleyAPC bradleyAPC = BradleyAPC.SpawnRoadDrivingBradley(vector, Quaternion.identity);
			if (bradleyAPC)
			{
				this.spawnedAPCs.Add(bradleyAPC);
			}
			else
			{
				Debug.Log("Failed to spawn bradley at: " + vector);
			}
		}
	}

	// Token: 0x0400210E RID: 8462
	public List<BradleyAPC> spawnedAPCs = new List<BradleyAPC>();
}
