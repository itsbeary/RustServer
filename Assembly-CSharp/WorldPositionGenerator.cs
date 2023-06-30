using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using UnityEngine;

// Token: 0x02000625 RID: 1573
[CreateAssetMenu(menuName = "Rust/Missions/WorldPositionGenerator")]
public class WorldPositionGenerator : ScriptableObject
{
	// Token: 0x06002E78 RID: 11896 RVA: 0x00116C98 File Offset: 0x00114E98
	public bool TrySample(Vector3 origin, float minDist, float maxDist, out Vector3 position, List<Vector3> blocked = null)
	{
		WorldPositionGenerator.<>c__DisplayClass10_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this._quadtree == null)
		{
			this.PrecalculatePositions();
		}
		CS$<>8__locals1.inclusion = new Rect(origin.x - maxDist, origin.z - maxDist, maxDist * 2f, maxDist * 2f);
		CS$<>8__locals1.exclusion = new Rect(origin.x - minDist, origin.z - minDist, minDist * 2f, minDist * 2f);
		CS$<>8__locals1.blockedRects = Pool.GetList<Rect>();
		if (blocked != null)
		{
			float num = 10f;
			foreach (Vector3 vector in blocked)
			{
				Rect rect = new Rect(vector.x - num, vector.z - num, num * 2f, num * 2f);
				CS$<>8__locals1.blockedRects.Add(rect);
			}
		}
		CS$<>8__locals1.candidates = Pool.GetList<ByteQuadtree.Element>();
		CS$<>8__locals1.candidates.Add(this._quadtree.Root);
		for (int i = 0; i < CS$<>8__locals1.candidates.Count; i++)
		{
			ByteQuadtree.Element element = CS$<>8__locals1.candidates[i];
			if (!element.IsLeaf)
			{
				CS$<>8__locals1.candidates.RemoveUnordered(i--);
				this.<TrySample>g__EvaluateCandidate|10_0(element.Child1, ref CS$<>8__locals1);
				this.<TrySample>g__EvaluateCandidate|10_0(element.Child2, ref CS$<>8__locals1);
				this.<TrySample>g__EvaluateCandidate|10_0(element.Child3, ref CS$<>8__locals1);
				this.<TrySample>g__EvaluateCandidate|10_0(element.Child4, ref CS$<>8__locals1);
			}
		}
		if (CS$<>8__locals1.candidates.Count == 0)
		{
			position = origin;
			Pool.FreeList<ByteQuadtree.Element>(ref CS$<>8__locals1.candidates);
			Pool.FreeList<Rect>(ref CS$<>8__locals1.blockedRects);
			return false;
		}
		Vector3 vector3;
		if (this.CheckSphereRadius > 1E-45f)
		{
			while (CS$<>8__locals1.candidates.Count != 0)
			{
				int num2 = UnityEngine.Random.Range(0, CS$<>8__locals1.candidates.Count);
				ByteQuadtree.Element element2 = CS$<>8__locals1.candidates[num2];
				Vector3 vector2 = this.<TrySample>g__GetElementRect|10_1(element2, ref CS$<>8__locals1).center.XZ3D();
				vector2.y = TerrainMeta.HeightMap.GetHeight(vector2);
				if (!Physics.CheckSphere(vector2, this.CheckSphereRadius, this.CheckSphereMask.value))
				{
					vector3 = vector2;
					goto IL_2A2;
				}
				CS$<>8__locals1.candidates.RemoveAt(num2);
			}
			position = Vector3.zero;
			return false;
		}
		ByteQuadtree.Element random = CS$<>8__locals1.candidates.GetRandom<ByteQuadtree.Element>();
		Rect rect2 = this.<TrySample>g__GetElementRect|10_1(random, ref CS$<>8__locals1);
		vector3 = (rect2.min + rect2.size * new Vector2(UnityEngine.Random.value, UnityEngine.Random.value)).XZ3D();
		IL_2A2:
		position = vector3.WithY(this.aboveWater ? TerrainMeta.WaterMap.GetHeight(vector3) : TerrainMeta.HeightMap.GetHeight(vector3));
		Pool.FreeList<ByteQuadtree.Element>(ref CS$<>8__locals1.candidates);
		Pool.FreeList<Rect>(ref CS$<>8__locals1.blockedRects);
		return true;
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x00116FA0 File Offset: 0x001151A0
	public void PrecalculatePositions()
	{
		int res = Mathf.NextPowerOfTwo((int)(World.Size * 0.25f));
		byte[] map = new byte[res * res];
		Parallel.For(0, res, delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				float num = ((float)i + 0.5f) / (float)res;
				float num2 = ((float)z + 0.5f) / (float)res;
				float factor = this.Filter.GetFactor(num, num2, true);
				if (factor > 0f && this.MaxSlopeRadius > 0f)
				{
					TerrainMeta.HeightMap.ForEach(num, num2, this.MaxSlopeRadius / (float)res, delegate(int slopeX, int slopeZ)
					{
						if (TerrainMeta.HeightMap.GetSlope(slopeX, slopeZ) > this.MaxSlopeDegrees)
						{
							factor = 0f;
						}
					});
				}
				map[z * res + i] = (byte)((factor >= this.FilterCutoff) ? (255f * factor) : 0f);
			}
		});
		this._origin = TerrainMeta.Position;
		this._area = TerrainMeta.Size;
		this._quadtree = new ByteQuadtree();
		this._quadtree.UpdateValues(map);
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x00117054 File Offset: 0x00115254
	[CompilerGenerated]
	private void <TrySample>g__EvaluateCandidate|10_0(ByteQuadtree.Element child, ref WorldPositionGenerator.<>c__DisplayClass10_0 A_2)
	{
		if (child.Value == 0U)
		{
			return;
		}
		Rect rect = this.<TrySample>g__GetElementRect|10_1(child, ref A_2);
		if (!rect.Overlaps(A_2.inclusion))
		{
			return;
		}
		if (A_2.exclusion.Contains(rect.min) && A_2.exclusion.Contains(rect.max))
		{
			return;
		}
		if (A_2.blockedRects.Count > 0)
		{
			foreach (Rect rect2 in A_2.blockedRects)
			{
				if (rect2.Contains(rect.min) && rect2.Contains(rect.max))
				{
					return;
				}
			}
		}
		A_2.candidates.Add(child);
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x00117128 File Offset: 0x00115328
	[CompilerGenerated]
	private Rect <TrySample>g__GetElementRect|10_1(ByteQuadtree.Element element, ref WorldPositionGenerator.<>c__DisplayClass10_0 A_2)
	{
		int num = 1 << element.Depth;
		float num2 = 1f / (float)num;
		Vector2 vector = element.Coords * num2;
		return new Rect(this._origin.x + vector.x * this._area.x, this._origin.z + vector.y * this._area.z, this._area.x * num2, this._area.z * num2);
	}

	// Token: 0x0400261F RID: 9759
	public SpawnFilter Filter = new SpawnFilter();

	// Token: 0x04002620 RID: 9760
	public float FilterCutoff;

	// Token: 0x04002621 RID: 9761
	public bool aboveWater;

	// Token: 0x04002622 RID: 9762
	public float MaxSlopeRadius;

	// Token: 0x04002623 RID: 9763
	public float MaxSlopeDegrees = 90f;

	// Token: 0x04002624 RID: 9764
	public float CheckSphereRadius;

	// Token: 0x04002625 RID: 9765
	public LayerMask CheckSphereMask;

	// Token: 0x04002626 RID: 9766
	private Vector3 _origin;

	// Token: 0x04002627 RID: 9767
	private Vector3 _area;

	// Token: 0x04002628 RID: 9768
	private ByteQuadtree _quadtree;
}
