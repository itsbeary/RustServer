using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class HitboxSystem : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001E9A RID: 7834 RVA: 0x000CFCA4 File Offset: 0x000CDEA4
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		List<HitboxDefinition> list = Pool.GetList<HitboxDefinition>();
		base.GetComponentsInChildren<HitboxDefinition>(list);
		if (serverside)
		{
			foreach (HitboxDefinition hitboxDefinition in list)
			{
				if (preProcess != null)
				{
					preProcess.RemoveComponent(hitboxDefinition);
				}
			}
			if (preProcess != null)
			{
				preProcess.RemoveComponent(this);
			}
		}
		if (clientside)
		{
			this.hitboxes.Clear();
			foreach (HitboxDefinition hitboxDefinition2 in list.OrderBy((HitboxDefinition x) => x.priority))
			{
				HitboxSystem.HitboxShape hitboxShape = new HitboxSystem.HitboxShape
				{
					bone = hitboxDefinition2.transform,
					localTransform = hitboxDefinition2.LocalMatrix,
					colliderMaterial = hitboxDefinition2.physicMaterial,
					type = hitboxDefinition2.type
				};
				this.hitboxes.Add(hitboxShape);
				if (preProcess != null)
				{
					preProcess.RemoveComponent(hitboxDefinition2);
				}
			}
		}
		Pool.FreeList<HitboxDefinition>(ref list);
	}

	// Token: 0x040017A9 RID: 6057
	public List<HitboxSystem.HitboxShape> hitboxes = new List<HitboxSystem.HitboxShape>();

	// Token: 0x02000CBA RID: 3258
	[Serializable]
	public class HitboxShape
	{
		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06004F8A RID: 20362 RVA: 0x001A6E7B File Offset: 0x001A507B
		public Matrix4x4 Transform
		{
			get
			{
				return this.transform;
			}
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06004F8B RID: 20363 RVA: 0x001A6E83 File Offset: 0x001A5083
		public Vector3 Position
		{
			get
			{
				return this.transform.MultiplyPoint(Vector3.zero);
			}
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06004F8C RID: 20364 RVA: 0x001A6E95 File Offset: 0x001A5095
		public Quaternion Rotation
		{
			get
			{
				return this.transform.rotation;
			}
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06004F8D RID: 20365 RVA: 0x001A6EA2 File Offset: 0x001A50A2
		// (set) Token: 0x06004F8E RID: 20366 RVA: 0x001A6EAA File Offset: 0x001A50AA
		public Vector3 Size { get; private set; }

		// Token: 0x06004F8F RID: 20367 RVA: 0x001A6EB4 File Offset: 0x001A50B4
		public void UpdateTransform()
		{
			using (TimeWarning.New("HitboxSystem.UpdateTransform", 0))
			{
				this.transform = this.bone.localToWorldMatrix * this.localTransform;
				this.Size = this.transform.lossyScale;
				this.transform = Matrix4x4.TRS(this.Position, this.Rotation, Vector3.one);
				this.inverseTransform = this.transform.inverse;
			}
		}

		// Token: 0x06004F90 RID: 20368 RVA: 0x001A6F44 File Offset: 0x001A5144
		public Vector3 TransformPoint(Vector3 pt)
		{
			return this.transform.MultiplyPoint(pt);
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x001A6F52 File Offset: 0x001A5152
		public Vector3 InverseTransformPoint(Vector3 pt)
		{
			return this.inverseTransform.MultiplyPoint(pt);
		}

		// Token: 0x06004F92 RID: 20370 RVA: 0x001A6F60 File Offset: 0x001A5160
		public Vector3 TransformDirection(Vector3 pt)
		{
			return this.transform.MultiplyVector(pt);
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x001A6F6E File Offset: 0x001A516E
		public Vector3 InverseTransformDirection(Vector3 pt)
		{
			return this.inverseTransform.MultiplyVector(pt);
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x001A6F7C File Offset: 0x001A517C
		public bool Trace(Ray ray, out RaycastHit hit, float forgivness = 0f, float maxDistance = float.PositiveInfinity)
		{
			bool flag;
			using (TimeWarning.New("Hitbox.Trace", 0))
			{
				ray.origin = this.InverseTransformPoint(ray.origin);
				ray.direction = this.InverseTransformDirection(ray.direction);
				if (this.type == HitboxDefinition.Type.BOX)
				{
					AABB aabb = new AABB(Vector3.zero, this.Size);
					if (!aabb.Trace(ray, out hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				else
				{
					Capsule capsule = new Capsule(Vector3.zero, this.Size.x, this.Size.y * 0.5f);
					if (!capsule.Trace(ray, out hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				hit.point = this.TransformPoint(hit.point);
				hit.normal = this.TransformDirection(hit.normal);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x001A7068 File Offset: 0x001A5268
		public Bounds GetBounds()
		{
			Matrix4x4 matrix4x = this.Transform;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					matrix4x[i, j] = Mathf.Abs(matrix4x[i, j]);
				}
			}
			return new Bounds
			{
				center = this.Transform.MultiplyPoint(Vector3.zero),
				extents = matrix4x.MultiplyVector(this.Size)
			};
		}

		// Token: 0x04004503 RID: 17667
		public Transform bone;

		// Token: 0x04004504 RID: 17668
		public HitboxDefinition.Type type;

		// Token: 0x04004505 RID: 17669
		public Matrix4x4 localTransform;

		// Token: 0x04004506 RID: 17670
		public PhysicMaterial colliderMaterial;

		// Token: 0x04004507 RID: 17671
		private Matrix4x4 transform;

		// Token: 0x04004508 RID: 17672
		private Matrix4x4 inverseTransform;
	}
}
