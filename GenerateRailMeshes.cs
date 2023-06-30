using System;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public class GenerateRailMeshes : ProceduralComponent
{
	// Token: 0x060031EF RID: 12783 RVA: 0x0012F0F4 File Offset: 0x0012D2F4
	public override void Process(uint seed)
	{
		if (this.RailMeshes == null || this.RailMeshes.Length == 0)
		{
			this.RailMeshes = new Mesh[] { this.RailMesh };
		}
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RailMeshes, 0f, false, !pathList.Path.Circular && !pathList.Start, !pathList.Path.Circular && !pathList.End))
			{
				GameObject gameObject = new GameObject("Rail Mesh");
				gameObject.transform.position = meshObject.Position;
				gameObject.tag = "Railway";
				gameObject.layer = 16;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
				gameObject.SetActive(false);
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RailPhysicMaterial;
				meshCollider.sharedMesh = meshObject.Meshes[0];
				gameObject.AddComponent<AddToHeightMap>();
				gameObject.SetActive(true);
			}
			this.AddTrackSpline(pathList);
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060031F0 RID: 12784 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x0012F260 File Offset: 0x0012D460
	private void AddTrackSpline(PathList rail)
	{
		TrainTrackSpline trainTrackSpline = HierarchyUtil.GetRoot(rail.Name, true, false).AddComponent<TrainTrackSpline>();
		trainTrackSpline.aboveGroundSpawn = rail.Hierarchy == 2;
		trainTrackSpline.hierarchy = rail.Hierarchy;
		if (trainTrackSpline.aboveGroundSpawn)
		{
			TrainTrackSpline.SidingSplines.Add(trainTrackSpline);
		}
		Vector3[] array = new Vector3[rail.Path.Points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = rail.Path.Points[i];
			Vector3[] array2 = array;
			int num = i;
			array2[num].y = array2[num].y + 0.41f;
		}
		Vector3[] array3 = new Vector3[rail.Path.Tangents.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array3[j] = rail.Path.Tangents[j];
		}
		trainTrackSpline.SetAll(array, array3, 0.25f);
	}

	// Token: 0x04002896 RID: 10390
	public const float NormalSmoothing = 0f;

	// Token: 0x04002897 RID: 10391
	public const bool SnapToTerrain = false;

	// Token: 0x04002898 RID: 10392
	public Mesh RailMesh;

	// Token: 0x04002899 RID: 10393
	public Mesh[] RailMeshes;

	// Token: 0x0400289A RID: 10394
	public Material RailMaterial;

	// Token: 0x0400289B RID: 10395
	public PhysicMaterial RailPhysicMaterial;
}
