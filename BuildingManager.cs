using System;
using ConVar;
using UnityEngine.AI;

// Token: 0x020004F0 RID: 1264
public abstract class BuildingManager
{
	// Token: 0x060028ED RID: 10477 RVA: 0x000FC624 File Offset: 0x000FA824
	public BuildingManager.Building GetBuilding(uint buildingID)
	{
		BuildingManager.Building building = null;
		this.buildingDictionary.TryGetValue(buildingID, out building);
		return building;
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x000FC644 File Offset: 0x000FA844
	public void Add(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			building = this.CreateBuilding(ent.buildingID);
			this.buildingDictionary.Add(ent.buildingID, building);
		}
		building.Add(ent);
		building.Dirty();
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x000FC6B0 File Offset: 0x000FA8B0
	public void Remove(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			this.decayEntities.Remove(ent);
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			return;
		}
		building.Remove(ent);
		if (building.IsEmpty())
		{
			this.buildingDictionary.Remove(ent.buildingID);
			this.DisposeBuilding(ref building);
			return;
		}
		building.Dirty();
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x000FC714 File Offset: 0x000FA914
	public void Clear()
	{
		this.buildingDictionary.Clear();
	}

	// Token: 0x060028F1 RID: 10481
	protected abstract BuildingManager.Building CreateBuilding(uint id);

	// Token: 0x060028F2 RID: 10482
	protected abstract void DisposeBuilding(ref BuildingManager.Building building);

	// Token: 0x0400211F RID: 8479
	public static ServerBuildingManager server = new ServerBuildingManager();

	// Token: 0x04002120 RID: 8480
	protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

	// Token: 0x04002121 RID: 8481
	protected ListDictionary<uint, BuildingManager.Building> buildingDictionary = new ListDictionary<uint, BuildingManager.Building>();

	// Token: 0x02000D3D RID: 3389
	public class Building
	{
		// Token: 0x060050A4 RID: 20644 RVA: 0x001AA078 File Offset: 0x001A8278
		public bool IsEmpty()
		{
			return !this.HasBuildingPrivileges() && !this.HasBuildingBlocks() && !this.HasDecayEntities();
		}

		// Token: 0x060050A5 RID: 20645 RVA: 0x001AA09C File Offset: 0x001A829C
		public BuildingPrivlidge GetDominatingBuildingPrivilege()
		{
			BuildingPrivlidge buildingPrivlidge = null;
			if (this.HasBuildingPrivileges())
			{
				for (int i = 0; i < this.buildingPrivileges.Count; i++)
				{
					BuildingPrivlidge buildingPrivlidge2 = this.buildingPrivileges[i];
					if (!(buildingPrivlidge2 == null) && buildingPrivlidge2.IsOlderThan(buildingPrivlidge))
					{
						buildingPrivlidge = buildingPrivlidge2;
					}
				}
			}
			return buildingPrivlidge;
		}

		// Token: 0x060050A6 RID: 20646 RVA: 0x001AA0EB File Offset: 0x001A82EB
		public bool HasBuildingPrivileges()
		{
			return this.buildingPrivileges != null && this.buildingPrivileges.Count > 0;
		}

		// Token: 0x060050A7 RID: 20647 RVA: 0x001AA105 File Offset: 0x001A8305
		public bool HasBuildingBlocks()
		{
			return this.buildingBlocks != null && this.buildingBlocks.Count > 0;
		}

		// Token: 0x060050A8 RID: 20648 RVA: 0x001AA11F File Offset: 0x001A831F
		public bool HasDecayEntities()
		{
			return this.decayEntities != null && this.decayEntities.Count > 0;
		}

		// Token: 0x060050A9 RID: 20649 RVA: 0x001AA139 File Offset: 0x001A8339
		public void AddBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingPrivileges.Contains(ent))
			{
				this.buildingPrivileges.Add(ent);
			}
		}

		// Token: 0x060050AA RID: 20650 RVA: 0x001AA15F File Offset: 0x001A835F
		public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingPrivileges.Remove(ent);
		}

		// Token: 0x060050AB RID: 20651 RVA: 0x001AA178 File Offset: 0x001A8378
		public void AddBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingBlocks.Contains(ent))
			{
				this.buildingBlocks.Add(ent);
				if (AI.nav_carve_use_building_optimization)
				{
					NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
					if (component != null)
					{
						this.isNavMeshCarvingDirty = true;
						if (this.navmeshCarvers == null)
						{
							this.navmeshCarvers = new ListHashSet<NavMeshObstacle>(8);
						}
						this.navmeshCarvers.Add(component);
					}
				}
			}
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x001AA1E8 File Offset: 0x001A83E8
		public void RemoveBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingBlocks.Remove(ent);
			if (AI.nav_carve_use_building_optimization && this.navmeshCarvers != null)
			{
				NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
				if (component != null)
				{
					this.navmeshCarvers.Remove(component);
					if (this.navmeshCarvers.Count == 0)
					{
						this.navmeshCarvers = null;
					}
					this.isNavMeshCarvingDirty = true;
					if (this.navmeshCarvers == null)
					{
						BuildingManager.Building building = ent.GetBuilding();
						if (building != null)
						{
							int num = 2;
							BuildingManager.server.UpdateNavMeshCarver(building, ref num, 0);
						}
					}
				}
			}
		}

		// Token: 0x060050AD RID: 20653 RVA: 0x001AA275 File Offset: 0x001A8475
		public void AddDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
		}

		// Token: 0x060050AE RID: 20654 RVA: 0x001AA29B File Offset: 0x001A849B
		public void RemoveDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			this.decayEntities.Remove(ent);
		}

		// Token: 0x060050AF RID: 20655 RVA: 0x001AA2B4 File Offset: 0x001A84B4
		public void Add(DecayEntity ent)
		{
			this.AddDecayEntity(ent);
			this.AddBuildingBlock(ent as BuildingBlock);
			this.AddBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x060050B0 RID: 20656 RVA: 0x001AA2D5 File Offset: 0x001A84D5
		public void Remove(DecayEntity ent)
		{
			this.RemoveDecayEntity(ent);
			this.RemoveBuildingBlock(ent as BuildingBlock);
			this.RemoveBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x060050B1 RID: 20657 RVA: 0x001AA2F8 File Offset: 0x001A84F8
		public void Dirty()
		{
			BuildingPrivlidge dominatingBuildingPrivilege = this.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null)
			{
				dominatingBuildingPrivilege.BuildingDirty();
			}
		}

		// Token: 0x04004739 RID: 18233
		public uint ID;

		// Token: 0x0400473A RID: 18234
		public ListHashSet<BuildingPrivlidge> buildingPrivileges = new ListHashSet<BuildingPrivlidge>(8);

		// Token: 0x0400473B RID: 18235
		public ListHashSet<BuildingBlock> buildingBlocks = new ListHashSet<BuildingBlock>(8);

		// Token: 0x0400473C RID: 18236
		public ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

		// Token: 0x0400473D RID: 18237
		public NavMeshObstacle buildingNavMeshObstacle;

		// Token: 0x0400473E RID: 18238
		public ListHashSet<NavMeshObstacle> navmeshCarvers;

		// Token: 0x0400473F RID: 18239
		public bool isNavMeshCarvingDirty;

		// Token: 0x04004740 RID: 18240
		public bool isNavMeshCarveOptimized;
	}
}
