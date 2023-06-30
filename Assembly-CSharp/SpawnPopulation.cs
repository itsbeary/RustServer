using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000570 RID: 1392
[CreateAssetMenu(menuName = "Rust/Spawn Population")]
public class SpawnPopulation : BaseScriptableObject
{
	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002AC7 RID: 10951 RVA: 0x001050DB File Offset: 0x001032DB
	public virtual float TargetDensity
	{
		get
		{
			return this._targetDensity;
		}
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x001050E4 File Offset: 0x001032E4
	public bool Initialize()
	{
		if (this.Prefabs == null || this.Prefabs.Length == 0)
		{
			if (!string.IsNullOrEmpty(this.ResourceFolder))
			{
				this.Prefabs = Prefab.Load<Spawnable>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, GameManager.server, PrefabAttribute.server, false);
			}
			if (this.ResourceList != null && this.ResourceList.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (GameObjectRef gameObjectRef in this.ResourceList)
				{
					string resourcePath = gameObjectRef.resourcePath;
					if (string.IsNullOrEmpty(resourcePath))
					{
						Debug.LogWarning(base.name + " resource list contains invalid resource path for GUID " + gameObjectRef.guid, this);
					}
					else
					{
						list.Add(resourcePath);
					}
				}
				this.Prefabs = Prefab.Load<Spawnable>(list.ToArray(), GameManager.server, PrefabAttribute.server);
			}
			if (this.Prefabs == null || this.Prefabs.Length == 0)
			{
				return false;
			}
			this.numToSpawn = new int[this.Prefabs.Length];
		}
		return true;
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x001051E4 File Offset: 0x001033E4
	public void UpdateWeights(SpawnDistribution distribution, int targetCount)
	{
		int num = 0;
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			Prefab<Spawnable> prefab = this.Prefabs[i];
			int prefabWeight = this.GetPrefabWeight(prefab);
			num += prefabWeight;
		}
		int num2 = Mathf.CeilToInt((float)targetCount / (float)num);
		this.sumToSpawn = 0;
		for (int j = 0; j < this.Prefabs.Length; j++)
		{
			Prefab<Spawnable> prefab2 = this.Prefabs[j];
			int prefabWeight2 = this.GetPrefabWeight(prefab2);
			int count = distribution.GetCount(prefab2.ID);
			int num3 = Mathf.Max(prefabWeight2 * num2 - count, 0);
			this.numToSpawn[j] = num3;
			this.sumToSpawn += num3;
		}
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x0010528F File Offset: 0x0010348F
	protected virtual int GetPrefabWeight(Prefab<Spawnable> prefab)
	{
		if (!prefab.Parameters)
		{
			return 1;
		}
		return prefab.Parameters.Count;
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x001052AC File Offset: 0x001034AC
	public bool TryTakeRandomPrefab(out Prefab<Spawnable> result)
	{
		int num = UnityEngine.Random.Range(0, this.sumToSpawn);
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			if ((num -= this.numToSpawn[i]) < 0)
			{
				this.numToSpawn[i]--;
				this.sumToSpawn--;
				result = this.Prefabs[i];
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x00105318 File Offset: 0x00103518
	public void ReturnPrefab(Prefab<Spawnable> prefab)
	{
		if (prefab == null)
		{
			return;
		}
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			if (this.Prefabs[i] == prefab)
			{
				this.numToSpawn[i]++;
				this.sumToSpawn++;
			}
		}
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x00105366 File Offset: 0x00103566
	public float GetCurrentSpawnRate()
	{
		if (this.ScaleWithServerPopulation)
		{
			return this.SpawnRate * SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate);
		}
		return this.SpawnRate * Spawn.max_rate;
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x00105393 File Offset: 0x00103593
	public float GetCurrentSpawnDensity()
	{
		if (this.ScaleWithServerPopulation)
		{
			return this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
		}
		return this.TargetDensity * Spawn.max_density * 1E-06f;
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x001053CC File Offset: 0x001035CC
	public float GetMaximumSpawnDensity()
	{
		if (this.ScaleWithServerPopulation)
		{
			return 2f * this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
		}
		return 2f * this.TargetDensity * Spawn.max_density * 1E-06f;
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool GetSpawnPosOverride(Prefab<Spawnable> prefab, ref Vector3 newPos, ref Quaternion newRot)
	{
		return true;
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPostFill(SpawnHandler spawnHandler)
	{
	}

	// Token: 0x040022F4 RID: 8948
	[Header("Spawnables")]
	public string ResourceFolder = string.Empty;

	// Token: 0x040022F5 RID: 8949
	public GameObjectRef[] ResourceList;

	// Token: 0x040022F6 RID: 8950
	[Header("Spawn Info")]
	[Tooltip("Usually per square km")]
	[SerializeField]
	[FormerlySerializedAs("TargetDensity")]
	private float _targetDensity = 1f;

	// Token: 0x040022F7 RID: 8951
	public float SpawnRate = 1f;

	// Token: 0x040022F8 RID: 8952
	public int ClusterSizeMin = 1;

	// Token: 0x040022F9 RID: 8953
	public int ClusterSizeMax = 1;

	// Token: 0x040022FA RID: 8954
	public int ClusterDithering;

	// Token: 0x040022FB RID: 8955
	public int SpawnAttemptsInitial = 20;

	// Token: 0x040022FC RID: 8956
	public int SpawnAttemptsRepeating = 10;

	// Token: 0x040022FD RID: 8957
	public bool EnforcePopulationLimits = true;

	// Token: 0x040022FE RID: 8958
	public bool ScaleWithLargeMaps = true;

	// Token: 0x040022FF RID: 8959
	public bool ScaleWithSpawnFilter = true;

	// Token: 0x04002300 RID: 8960
	public bool ScaleWithServerPopulation;

	// Token: 0x04002301 RID: 8961
	public bool AlignToNormal;

	// Token: 0x04002302 RID: 8962
	public SpawnFilter Filter = new SpawnFilter();

	// Token: 0x04002303 RID: 8963
	public float FilterCutoff;

	// Token: 0x04002304 RID: 8964
	public float FilterRadius;

	// Token: 0x04002305 RID: 8965
	internal Prefab<Spawnable>[] Prefabs;

	// Token: 0x04002306 RID: 8966
	private int[] numToSpawn;

	// Token: 0x04002307 RID: 8967
	private int sumToSpawn;
}
