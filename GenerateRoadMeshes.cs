using System;
using UnityEngine;

// Token: 0x020006D6 RID: 1750
public class GenerateRoadMeshes : ProceduralComponent
{
	// Token: 0x06003210 RID: 12816 RVA: 0x00131A24 File Offset: 0x0012FC24
	public override void Process(uint seed)
	{
		if (this.RoadMeshes == null || this.RoadMeshes.Length == 0)
		{
			this.RoadMeshes = new Mesh[] { this.RoadMesh };
		}
		foreach (PathList pathList in TerrainMeta.Path.Roads)
		{
			if (pathList.Hierarchy < 2)
			{
				foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RoadMeshes, 0f, true, !pathList.Path.Circular, !pathList.Path.Circular))
				{
					GameObject gameObject = new GameObject("Road Mesh");
					gameObject.transform.position = meshObject.Position;
					gameObject.layer = 16;
					gameObject.SetHierarchyGroup(pathList.Name, true, false);
					gameObject.SetActive(false);
					MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMaterial = this.RoadPhysicMaterial;
					meshCollider.sharedMesh = meshObject.Meshes[0];
					gameObject.AddComponent<AddToHeightMap>();
					gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06003211 RID: 12817 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040028D3 RID: 10451
	public const float NormalSmoothing = 0f;

	// Token: 0x040028D4 RID: 10452
	public const bool SnapToTerrain = true;

	// Token: 0x040028D5 RID: 10453
	public Mesh RoadMesh;

	// Token: 0x040028D6 RID: 10454
	public Mesh[] RoadMeshes;

	// Token: 0x040028D7 RID: 10455
	public Material RoadMaterial;

	// Token: 0x040028D8 RID: 10456
	public Material RoadRingMaterial;

	// Token: 0x040028D9 RID: 10457
	public PhysicMaterial RoadPhysicMaterial;
}
