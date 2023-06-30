using System;
using UnityEngine;

// Token: 0x020006D1 RID: 1745
public class GenerateRiverMeshes : ProceduralComponent
{
	// Token: 0x06003203 RID: 12803 RVA: 0x00130CB0 File Offset: 0x0012EEB0
	public override void Process(uint seed)
	{
		this.RiverMeshes = new Mesh[] { this.RiverMesh };
		foreach (PathList pathList in TerrainMeta.Path.Rivers)
		{
			foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RiverMeshes, 0.1f, true, !pathList.Path.Circular, !pathList.Path.Circular))
			{
				GameObject gameObject = new GameObject("River Mesh");
				gameObject.transform.position = meshObject.Position;
				gameObject.tag = "River";
				gameObject.layer = 4;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
				gameObject.SetActive(false);
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RiverPhysicMaterial;
				meshCollider.sharedMesh = meshObject.Meshes[0];
				gameObject.AddComponent<RiverInfo>();
				gameObject.AddComponent<WaterBody>().FishingType = WaterBody.FishingTag.River;
				gameObject.AddComponent<AddToWaterMap>();
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x06003204 RID: 12804 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040028BE RID: 10430
	public const float NormalSmoothing = 0.1f;

	// Token: 0x040028BF RID: 10431
	public const bool SnapToTerrain = true;

	// Token: 0x040028C0 RID: 10432
	public Mesh RiverMesh;

	// Token: 0x040028C1 RID: 10433
	public Mesh[] RiverMeshes;

	// Token: 0x040028C2 RID: 10434
	public Material RiverMaterial;

	// Token: 0x040028C3 RID: 10435
	public PhysicMaterial RiverPhysicMaterial;
}
