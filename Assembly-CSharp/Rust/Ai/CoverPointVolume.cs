using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000B47 RID: 2887
	public class CoverPointVolume : MonoBehaviour, IServerComponent
	{
		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x060045E0 RID: 17888 RVA: 0x0000441C File Offset: 0x0000261C
		public bool repeat
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060045E1 RID: 17889 RVA: 0x001973AC File Offset: 0x001955AC
		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (this.CoverPoints.Count == 0)
			{
				if (this._dynNavMeshBuildCompletionTime < 0f)
				{
					if (SingletonComponent<DynamicNavMesh>.Instance == null || !SingletonComponent<DynamicNavMesh>.Instance.enabled || !SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
					{
						this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					}
				}
				else if (this._genAttempts < 4 && Time.realtimeSinceStartup - this._dynNavMeshBuildCompletionTime > 0.25f)
				{
					this.GenerateCoverPoints(null);
					if (this.CoverPoints.Count != 0)
					{
						return null;
					}
					this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					this._genAttempts++;
					if (this._genAttempts >= 4)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return null;
					}
				}
			}
			return new float?(1f + UnityEngine.Random.value * 2f);
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0019748F File Offset: 0x0019568F
		[ContextMenu("Clear Cover Points")]
		private void ClearCoverPoints()
		{
			this.CoverPoints.Clear();
			this._coverPointBlockers.Clear();
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x001974A8 File Offset: 0x001956A8
		public Bounds GetBounds()
		{
			if (Mathf.Approximately(this.bounds.center.sqrMagnitude, 0f))
			{
				this.bounds = new Bounds(base.transform.position, base.transform.localScale);
			}
			return this.bounds;
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x001974FB File Offset: 0x001956FB
		[ContextMenu("Pre-Generate Cover Points")]
		public void PreGenerateCoverPoints()
		{
			this.GenerateCoverPoints(null);
		}

		// Token: 0x060045E5 RID: 17893 RVA: 0x00197504 File Offset: 0x00195704
		[ContextMenu("Convert to Manual Cover Points")]
		public void ConvertToManualCoverPoints()
		{
			foreach (CoverPoint coverPoint in this.CoverPoints)
			{
				ManualCoverPoint manualCoverPoint = new GameObject("MCP").AddComponent<ManualCoverPoint>();
				manualCoverPoint.transform.localPosition = Vector3.zero;
				manualCoverPoint.transform.position = coverPoint.Position;
				manualCoverPoint.Normal = coverPoint.Normal;
				manualCoverPoint.NormalCoverType = coverPoint.NormalCoverType;
				manualCoverPoint.Volume = this;
			}
		}

		// Token: 0x060045E6 RID: 17894 RVA: 0x001975A0 File Offset: 0x001957A0
		public void GenerateCoverPoints(Transform coverPointGroup)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			this.ClearCoverPoints();
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = coverPointGroup;
			}
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = base.transform;
			}
			if (this.ManualCoverPointGroup.childCount > 0)
			{
				ManualCoverPoint[] componentsInChildren = this.ManualCoverPointGroup.GetComponentsInChildren<ManualCoverPoint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					CoverPoint coverPoint = componentsInChildren[i].ToCoverPoint(this);
					this.CoverPoints.Add(coverPoint);
				}
			}
			if (this._coverPointBlockers.Count == 0 && this.BlockerGroup != null)
			{
				CoverPointBlockerVolume[] componentsInChildren2 = this.BlockerGroup.GetComponentsInChildren<CoverPointBlockerVolume>();
				if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
				{
					this._coverPointBlockers.AddRange(componentsInChildren2);
				}
			}
			NavMeshHit navMeshHit;
			if (this.CoverPoints.Count == 0 && NavMesh.SamplePosition(base.transform.position, out navMeshHit, base.transform.localScale.y * CoverPointVolume.cover_point_sample_step_height, -1))
			{
				Vector3 position = base.transform.position;
				Vector3 vector = base.transform.lossyScale * 0.5f;
				for (float num = position.x - vector.x + 1f; num < position.x + vector.x - 1f; num += CoverPointVolume.cover_point_sample_step_size)
				{
					for (float num2 = position.z - vector.z + 1f; num2 < position.z + vector.z - 1f; num2 += CoverPointVolume.cover_point_sample_step_size)
					{
						for (float num3 = position.y - vector.y; num3 < position.y + vector.y; num3 += CoverPointVolume.cover_point_sample_step_height)
						{
							NavMeshHit navMeshHit2;
							if (NavMesh.FindClosestEdge(new Vector3(num, num3, num2), out navMeshHit2, navMeshHit.mask))
							{
								navMeshHit2.position = new Vector3(navMeshHit2.position.x, navMeshHit2.position.y + 0.5f, navMeshHit2.position.z);
								bool flag = true;
								using (List<CoverPoint>.Enumerator enumerator = this.CoverPoints.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if ((enumerator.Current.Position - navMeshHit2.position).sqrMagnitude < CoverPointVolume.cover_point_sample_step_size * CoverPointVolume.cover_point_sample_step_size)
										{
											flag = false;
											break;
										}
									}
								}
								if (flag)
								{
									CoverPoint coverPoint2 = this.CalculateCoverPoint(navMeshHit2);
									if (coverPoint2 != null)
									{
										this.CoverPoints.Add(coverPoint2);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060045E7 RID: 17895 RVA: 0x00197860 File Offset: 0x00195A60
		private CoverPoint CalculateCoverPoint(NavMeshHit info)
		{
			RaycastHit raycastHit;
			CoverPointVolume.CoverType coverType = this.ProvidesCoverInDir(new Ray(info.position, -info.normal), this.CoverPointRayLength, out raycastHit);
			if (coverType == CoverPointVolume.CoverType.None)
			{
				return null;
			}
			CoverPoint coverPoint = new CoverPoint(this, this.DefaultCoverPointScore)
			{
				Position = info.position,
				Normal = -info.normal
			};
			if (coverType == CoverPointVolume.CoverType.Full)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
			}
			else if (coverType == CoverPointVolume.CoverType.Partial)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
			}
			return coverPoint;
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x001978E0 File Offset: 0x00195AE0
		internal CoverPointVolume.CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
		{
			rayHit = default(RaycastHit);
			if (ray.origin.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction == Vector3.zero)
			{
				return CoverPointVolume.CoverType.None;
			}
			ray.origin += PlayerEyes.EyeOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Full;
			}
			ray.origin += PlayerEyes.DuckOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Partial;
			}
			return CoverPointVolume.CoverType.None;
		}

		// Token: 0x060045E9 RID: 17897 RVA: 0x001979A4 File Offset: 0x00195BA4
		public bool Contains(Vector3 point)
		{
			Bounds bounds = new Bounds(base.transform.position, base.transform.localScale);
			return bounds.Contains(point);
		}

		// Token: 0x04003EC3 RID: 16067
		public float DefaultCoverPointScore = 1f;

		// Token: 0x04003EC4 RID: 16068
		public float CoverPointRayLength = 1f;

		// Token: 0x04003EC5 RID: 16069
		public LayerMask CoverLayerMask;

		// Token: 0x04003EC6 RID: 16070
		public Transform BlockerGroup;

		// Token: 0x04003EC7 RID: 16071
		public Transform ManualCoverPointGroup;

		// Token: 0x04003EC8 RID: 16072
		[ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
		public static float cover_point_sample_step_size = 6f;

		// Token: 0x04003EC9 RID: 16073
		[ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
		public static float cover_point_sample_step_height = 2f;

		// Token: 0x04003ECA RID: 16074
		public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

		// Token: 0x04003ECB RID: 16075
		private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

		// Token: 0x04003ECC RID: 16076
		private float _dynNavMeshBuildCompletionTime = -1f;

		// Token: 0x04003ECD RID: 16077
		private int _genAttempts;

		// Token: 0x04003ECE RID: 16078
		private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		// Token: 0x02000FAF RID: 4015
		internal enum CoverType
		{
			// Token: 0x0400510E RID: 20750
			None,
			// Token: 0x0400510F RID: 20751
			Partial,
			// Token: 0x04005110 RID: 20752
			Full
		}
	}
}
