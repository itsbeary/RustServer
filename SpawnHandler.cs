using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using UnityEngine;

// Token: 0x02000581 RID: 1409
public class SpawnHandler : SingletonComponent<SpawnHandler>
{
	// Token: 0x06002B37 RID: 11063 RVA: 0x00106954 File Offset: 0x00104B54
	protected void OnEnable()
	{
		this.AllSpawnPopulations = this.SpawnPopulations.Concat(this.ConvarSpawnPopulations).ToArray<SpawnPopulation>();
		base.StartCoroutine(this.SpawnTick());
		base.StartCoroutine(this.SpawnGroupTick());
		base.StartCoroutine(this.SpawnIndividualTick());
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x001069A4 File Offset: 0x00104BA4
	public static BasePlayer.SpawnPoint GetSpawnPoint()
	{
		if (SingletonComponent<SpawnHandler>.Instance == null || SingletonComponent<SpawnHandler>.Instance.CharDistribution == null)
		{
			return null;
		}
		BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
		if (!((WaterSystem.OceanLevel < 0.5f) ? SpawnHandler.GetSpawnPointStandard(spawnPoint) : FloodedSpawnHandler.GetSpawnPoint(spawnPoint, WaterSystem.OceanLevel + 1f)))
		{
			return null;
		}
		return spawnPoint;
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x001069FC File Offset: 0x00104BFC
	private static bool GetSpawnPointStandard(BasePlayer.SpawnPoint spawnPoint)
	{
		for (int i = 0; i < 60; i++)
		{
			if (SingletonComponent<SpawnHandler>.Instance.CharDistribution.Sample(out spawnPoint.pos, out spawnPoint.rot, false, 0f))
			{
				bool flag = true;
				if (TerrainMeta.Path != null)
				{
					using (List<MonumentInfo>.Enumerator enumerator = TerrainMeta.Path.Monuments.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Distance(spawnPoint.pos) < 50f)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x00106AAC File Offset: 0x00104CAC
	public void UpdateDistributions()
	{
		SpawnHandler.<>c__DisplayClass23_0 CS$<>8__locals1 = new SpawnHandler.<>c__DisplayClass23_0();
		if (global::World.Size == 0U)
		{
			return;
		}
		this.SpawnDistributions = new SpawnDistribution[this.AllSpawnPopulations.Length];
		this.population2distribution = new Dictionary<SpawnPopulation, SpawnDistribution>();
		Vector3 size = TerrainMeta.Size;
		Vector3 position = TerrainMeta.Position;
		CS$<>8__locals1.pop_res = Mathf.NextPowerOfTwo((int)(global::World.Size * 0.25f));
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
			if (spawnPopulation == null)
			{
				Debug.LogError("Spawn handler contains null spawn population.");
			}
			else
			{
				byte[] map2 = new byte[CS$<>8__locals1.pop_res * CS$<>8__locals1.pop_res];
				SpawnFilter filter2 = spawnPopulation.Filter;
				float cutoff2 = spawnPopulation.FilterCutoff;
				Parallel.For(0, CS$<>8__locals1.pop_res, delegate(int z)
				{
					for (int j = 0; j < CS$<>8__locals1.pop_res; j++)
					{
						float num = ((float)j + 0.5f) / (float)CS$<>8__locals1.pop_res;
						float num2 = ((float)z + 0.5f) / (float)CS$<>8__locals1.pop_res;
						float factor = filter2.GetFactor(num, num2, true);
						map2[z * CS$<>8__locals1.pop_res + j] = (byte)((factor >= cutoff2) ? (255f * factor) : 0f);
					}
				});
				SpawnDistribution spawnDistribution = (this.SpawnDistributions[i] = new SpawnDistribution(this, map2, position, size));
				this.population2distribution.Add(spawnPopulation, spawnDistribution);
			}
		}
		CS$<>8__locals1.char_res = Mathf.NextPowerOfTwo((int)(global::World.Size * 0.5f));
		byte[] map = new byte[CS$<>8__locals1.char_res * CS$<>8__locals1.char_res];
		SpawnFilter filter = this.CharacterSpawn;
		float cutoff = this.CharacterSpawnCutoff;
		Parallel.For(0, CS$<>8__locals1.char_res, delegate(int z)
		{
			for (int k = 0; k < CS$<>8__locals1.char_res; k++)
			{
				float num3 = ((float)k + 0.5f) / (float)CS$<>8__locals1.char_res;
				float num4 = ((float)z + 0.5f) / (float)CS$<>8__locals1.char_res;
				float factor2 = filter.GetFactor(num3, num4, true);
				map[z * CS$<>8__locals1.char_res + k] = (byte)((factor2 >= cutoff) ? (255f * factor2) : 0f);
			}
		});
		this.CharDistribution = new SpawnDistribution(this, map, position, size);
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x00106C80 File Offset: 0x00104E80
	public void FillPopulations()
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			if (!(this.AllSpawnPopulations[i] == null))
			{
				this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
			}
		}
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x00106CD0 File Offset: 0x00104ED0
	public void FillGroups()
	{
		for (int i = 0; i < this.SpawnGroups.Count; i++)
		{
			this.SpawnGroups[i].Fill();
		}
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x00106D04 File Offset: 0x00104F04
	public void FillIndividuals()
	{
		for (int i = 0; i < this.SpawnIndividuals.Count; i++)
		{
			SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
			this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, null, null), spawnIndividual.Position, spawnIndividual.Rotation);
		}
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x00106D54 File Offset: 0x00104F54
	public void InitialSpawn()
	{
		if (ConVar.Spawn.respawn_populations && this.SpawnDistributions != null)
		{
			for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
			{
				if (!(this.AllSpawnPopulations[i] == null))
				{
					this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
				}
			}
		}
		if (ConVar.Spawn.respawn_groups)
		{
			for (int j = 0; j < this.SpawnGroups.Count; j++)
			{
				this.SpawnGroups[j].SpawnInitial();
			}
		}
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x00106DD7 File Offset: 0x00104FD7
	public void StartSpawnTick()
	{
		this.spawnTick = true;
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x00106DE0 File Offset: 0x00104FE0
	private IEnumerator SpawnTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_populations)
			{
				yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_populations);
				int num;
				for (int i = 0; i < this.AllSpawnPopulations.Length; i = num + 1)
				{
					SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
					if (!(spawnPopulation == null))
					{
						SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
						if (spawnDistribution != null)
						{
							try
							{
								if (this.SpawnDistributions != null)
								{
									this.SpawnRepeating(spawnPopulation, spawnDistribution);
								}
							}
							catch (Exception ex)
							{
								Debug.LogError(ex);
							}
							yield return CoroutineEx.waitForEndOfFrame;
						}
					}
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002B41 RID: 11073 RVA: 0x00106DEF File Offset: 0x00104FEF
	private IEnumerator SpawnGroupTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_groups)
			{
				yield return CoroutineEx.waitForSeconds(1f);
				int num;
				for (int i = 0; i < this.SpawnGroups.Count; i = num + 1)
				{
					ISpawnGroup spawnGroup = this.SpawnGroups[i];
					if (spawnGroup != null)
					{
						try
						{
							spawnGroup.SpawnRepeating();
						}
						catch (Exception ex)
						{
							Debug.LogError(ex);
						}
						yield return CoroutineEx.waitForEndOfFrame;
					}
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002B42 RID: 11074 RVA: 0x00106DFE File Offset: 0x00104FFE
	private IEnumerator SpawnIndividualTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_individuals)
			{
				yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_individuals);
				int num;
				for (int i = 0; i < this.SpawnIndividuals.Count; i = num + 1)
				{
					SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
					try
					{
						this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, null, null), spawnIndividual.Position, spawnIndividual.Rotation);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex);
					}
					yield return CoroutineEx.waitForEndOfFrame;
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x00106E10 File Offset: 0x00105010
	public void SpawnInitial(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = this.GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		this.Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsInitial);
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x00106E48 File Offset: 0x00105048
	public void SpawnRepeating(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = this.GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		num = Mathf.RoundToInt((float)num * population.GetCurrentSpawnRate());
		num = UnityEngine.Random.Range(Mathf.Min(num, this.MinSpawnsPerTick), Mathf.Min(num, this.MaxSpawnsPerTick));
		this.Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsRepeating);
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x00106EAC File Offset: 0x001050AC
	private void Fill(SpawnPopulation population, SpawnDistribution distribution, int targetCount, int numToFill, int numToTry)
	{
		if (targetCount == 0)
		{
			return;
		}
		if (!population.Initialize())
		{
			Debug.LogError("[Spawn] No prefabs to spawn in " + population.ResourceFolder, population);
			return;
		}
		if (Global.developer > 1)
		{
			Debug.Log(string.Concat(new object[] { "[Spawn] Population ", population.ResourceFolder, " needs to spawn ", numToFill }));
		}
		float num = Mathf.Max((float)population.ClusterSizeMax, distribution.GetGridCellArea() * population.GetMaximumSpawnDensity());
		population.UpdateWeights(distribution, targetCount);
		while (numToFill >= population.ClusterSizeMin && numToTry > 0)
		{
			ByteQuadtree.Element element = distribution.SampleNode();
			int num2 = UnityEngine.Random.Range(population.ClusterSizeMin, population.ClusterSizeMax + 1);
			num2 = Mathx.Min(numToTry, numToFill, num2);
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector;
				Quaternion quaternion;
				bool flag = distribution.Sample(out vector, out quaternion, element, population.AlignToNormal, (float)population.ClusterDithering) && population.Filter.GetFactor(vector, true) > 0f;
				if (flag && population.FilterRadius > 0f)
				{
					flag = population.Filter.GetFactor(vector + Vector3.forward * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector - Vector3.forward * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector + Vector3.right * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector - Vector3.right * population.FilterRadius, true) > 0f;
				}
				Prefab<Spawnable> prefab;
				if (flag && population.TryTakeRandomPrefab(out prefab))
				{
					if (population.GetSpawnPosOverride(prefab, ref vector, ref quaternion) && (float)distribution.GetCount(vector) < num)
					{
						this.Spawn(population, prefab, vector, quaternion);
						numToFill--;
					}
					else
					{
						population.ReturnPrefab(prefab);
					}
				}
				numToTry--;
			}
		}
		population.OnPostFill(this);
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x001070D0 File Offset: 0x001052D0
	public GameObject Spawn(SpawnPopulation population, Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (prefab == null)
		{
			return null;
		}
		if (prefab.Component == null)
		{
			Debug.LogError("[Spawn] Missing component 'Spawnable' on " + prefab.Name);
			return null;
		}
		Vector3 one = Vector3.one;
		DecorComponent[] array = PrefabAttribute.server.FindAll<DecorComponent>(prefab.ID);
		prefab.Object.transform.ApplyDecorComponents(array, ref pos, ref rot, ref one);
		if (!prefab.ApplyTerrainAnchors(ref pos, rot, one, TerrainAnchorMode.MinimizeMovement, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainChecks(pos, rot, one, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainFilters(pos, rot, one, null))
		{
			return null;
		}
		if (!prefab.ApplyWaterChecks(pos, rot, one))
		{
			return null;
		}
		if (!prefab.ApplyBoundsChecks(pos, rot, one, this.BoundsCheckMask))
		{
			return null;
		}
		if (Global.developer > 1)
		{
			Debug.Log("[Spawn] Spawning " + prefab.Name);
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, false);
		if (baseEntity == null)
		{
			Debug.LogWarning("[Spawn] Couldn't create prefab as entity - " + prefab.Name);
			return null;
		}
		Spawnable component = baseEntity.GetComponent<Spawnable>();
		if (component.Population != population)
		{
			component.Population = population;
		}
		baseEntity.gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		return baseEntity.gameObject;
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x0010720C File Offset: 0x0010540C
	private GameObject Spawn(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (!this.CheckBounds(prefab.Object, pos, rot, Vector3.one))
		{
			return null;
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, true);
		if (baseEntity == null)
		{
			Debug.LogWarning("[Spawn] Couldn't create prefab as entity - " + prefab.Name);
			return null;
		}
		baseEntity.Spawn();
		return baseEntity.gameObject;
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x00107266 File Offset: 0x00105466
	public bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		return SpawnHandler.CheckBounds(gameObject, pos, rot, scale, this.BoundsCheckMask);
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x00107278 File Offset: 0x00105478
	public static bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale, LayerMask mask)
	{
		if (gameObject == null)
		{
			return true;
		}
		if (mask != 0)
		{
			BaseEntity component = gameObject.GetComponent<BaseEntity>();
			if (component != null && UnityEngine.Physics.CheckBox(pos + rot * Vector3.Scale(component.bounds.center, scale), Vector3.Scale(component.bounds.extents, scale), rot, mask))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x001072EC File Offset: 0x001054EC
	public void EnforceLimits(bool forceAll = false)
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			if (!(this.AllSpawnPopulations[i] == null))
			{
				SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
				SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
				if (forceAll || spawnPopulation.EnforcePopulationLimits)
				{
					this.EnforceLimits(spawnPopulation, spawnDistribution);
				}
			}
		}
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x0010734C File Offset: 0x0010554C
	private void EnforceLimits(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		Spawnable[] array = this.FindAll(population);
		if (array.Length <= targetCount)
		{
			return;
		}
		Debug.Log(string.Concat(new object[] { population, " has ", array.Length, " objects, but max allowed is ", targetCount }));
		int num = array.Length - targetCount;
		Debug.Log(" - deleting " + num + " objects");
		foreach (Spawnable spawnable in array.Take(num))
		{
			BaseEntity baseEntity = spawnable.gameObject.ToBaseEntity();
			if (baseEntity.IsValid())
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			else
			{
				GameManager.Destroy(spawnable.gameObject, 0f);
			}
		}
	}

	// Token: 0x06002B4C RID: 11084 RVA: 0x00107438 File Offset: 0x00105638
	public Spawnable[] FindAll(SpawnPopulation population)
	{
		return (from x in UnityEngine.Object.FindObjectsOfType<Spawnable>()
			where x.gameObject.activeInHierarchy && x.Population == population
			select x).ToArray<Spawnable>();
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x00107470 File Offset: 0x00105670
	public int GetTargetCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		float num = TerrainMeta.Size.x * TerrainMeta.Size.z;
		float num2 = population.GetCurrentSpawnDensity();
		if (!population.ScaleWithLargeMaps)
		{
			num = Mathf.Min(num, 16000000f);
		}
		if (population.ScaleWithSpawnFilter)
		{
			num2 *= distribution.Density;
		}
		return Mathf.RoundToInt(num * num2);
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x001074C7 File Offset: 0x001056C7
	public int GetCurrentCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		return distribution.Count;
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x001074CF File Offset: 0x001056CF
	public void AddRespawn(SpawnIndividual individual)
	{
		this.SpawnIndividuals.Add(individual);
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x001074E0 File Offset: 0x001056E0
	public void AddInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			SpawnDistribution spawnDistribution;
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				Debug.LogWarning("[SpawnHandler] trying to add instance to invalid population: " + spawnable.Population);
				return;
			}
			spawnDistribution.AddInstance(spawnable);
		}
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x00107530 File Offset: 0x00105730
	public void RemoveInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			SpawnDistribution spawnDistribution;
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				Debug.LogWarning("[SpawnHandler] trying to remove instance from invalid population: " + spawnable.Population);
				return;
			}
			spawnDistribution.RemoveInstance(spawnable);
		}
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x00107580 File Offset: 0x00105780
	public static float PlayerFraction()
	{
		float num = (float)Mathf.Max(Server.maxplayers, 1);
		return Mathf.Clamp01((float)BasePlayer.activePlayerList.Count / num);
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x001075AC File Offset: 0x001057AC
	public static float PlayerLerp(float min, float max)
	{
		return Mathf.Lerp(min, max, SpawnHandler.PlayerFraction());
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x001075BC File Offset: 0x001057BC
	public static float PlayerExcess()
	{
		float num = Mathf.Max(ConVar.Spawn.player_base, 1f);
		float num2 = (float)BasePlayer.activePlayerList.Count;
		if (num2 <= num)
		{
			return 0f;
		}
		return (num2 - num) / num;
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x001075F4 File Offset: 0x001057F4
	public static float PlayerScale(float scalar)
	{
		return Mathf.Max(1f, SpawnHandler.PlayerExcess() * scalar);
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x00107607 File Offset: 0x00105807
	public void DumpReport(string filename)
	{
		File.AppendAllText(filename, "\r\n\r\nSpawnHandler Report:\r\n\r\n" + this.GetReport(true));
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x00107620 File Offset: 0x00105820
	public string GetReport(bool detailed = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.AllSpawnPopulations == null)
		{
			stringBuilder.AppendLine("Spawn population array is null.");
		}
		if (this.SpawnDistributions == null)
		{
			stringBuilder.AppendLine("Spawn distribution array is null.");
		}
		if (this.AllSpawnPopulations != null && this.SpawnDistributions != null)
		{
			for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
			{
				if (!(this.AllSpawnPopulations[i] == null))
				{
					SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
					SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
					if (spawnPopulation != null)
					{
						if (!string.IsNullOrEmpty(spawnPopulation.ResourceFolder))
						{
							stringBuilder.AppendLine(spawnPopulation.name + " (autospawn/" + spawnPopulation.ResourceFolder + ")");
						}
						else
						{
							stringBuilder.AppendLine(spawnPopulation.name);
						}
						if (detailed)
						{
							stringBuilder.AppendLine("\tPrefabs:");
							if (spawnPopulation.Prefabs != null)
							{
								foreach (Prefab<Spawnable> prefab in spawnPopulation.Prefabs)
								{
									stringBuilder.AppendLine(string.Concat(new object[] { "\t\t", prefab.Name, " - ", prefab.Object }));
								}
							}
							else
							{
								stringBuilder.AppendLine("\t\tN/A");
							}
						}
						if (spawnDistribution != null)
						{
							int currentCount = this.GetCurrentCount(spawnPopulation, spawnDistribution);
							int targetCount = this.GetTargetCount(spawnPopulation, spawnDistribution);
							stringBuilder.AppendLine(string.Concat(new object[] { "\tPopulation: ", currentCount, "/", targetCount }));
						}
						else
						{
							stringBuilder.AppendLine("\tDistribution #" + i + " is not set.");
						}
					}
					else
					{
						stringBuilder.AppendLine("Population #" + i + " is not set.");
					}
					stringBuilder.AppendLine();
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002348 RID: 9032
	public float TickInterval = 60f;

	// Token: 0x04002349 RID: 9033
	public int MinSpawnsPerTick = 100;

	// Token: 0x0400234A RID: 9034
	public int MaxSpawnsPerTick = 100;

	// Token: 0x0400234B RID: 9035
	public LayerMask PlacementMask;

	// Token: 0x0400234C RID: 9036
	public LayerMask PlacementCheckMask;

	// Token: 0x0400234D RID: 9037
	public float PlacementCheckHeight = 25f;

	// Token: 0x0400234E RID: 9038
	public LayerMask RadiusCheckMask;

	// Token: 0x0400234F RID: 9039
	public float RadiusCheckDistance = 5f;

	// Token: 0x04002350 RID: 9040
	public LayerMask BoundsCheckMask;

	// Token: 0x04002351 RID: 9041
	public SpawnFilter CharacterSpawn;

	// Token: 0x04002352 RID: 9042
	public float CharacterSpawnCutoff;

	// Token: 0x04002353 RID: 9043
	public SpawnPopulation[] SpawnPopulations;

	// Token: 0x04002354 RID: 9044
	internal SpawnDistribution[] SpawnDistributions;

	// Token: 0x04002355 RID: 9045
	internal SpawnDistribution CharDistribution;

	// Token: 0x04002356 RID: 9046
	internal ListHashSet<ISpawnGroup> SpawnGroups = new ListHashSet<ISpawnGroup>(8);

	// Token: 0x04002357 RID: 9047
	internal List<SpawnIndividual> SpawnIndividuals = new List<SpawnIndividual>();

	// Token: 0x04002358 RID: 9048
	[ReadOnly]
	public SpawnPopulation[] ConvarSpawnPopulations;

	// Token: 0x04002359 RID: 9049
	private Dictionary<SpawnPopulation, SpawnDistribution> population2distribution;

	// Token: 0x0400235A RID: 9050
	private bool spawnTick;

	// Token: 0x0400235B RID: 9051
	private SpawnPopulation[] AllSpawnPopulations;
}
