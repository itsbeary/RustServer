using System;
using Rust;

namespace UnityEngine
{
	// Token: 0x02000A27 RID: 2599
	public static class ColliderEx
	{
		// Token: 0x06003DA1 RID: 15777 RVA: 0x0016942B File Offset: 0x0016762B
		public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
		{
			if (obj == null)
			{
				return TerrainMeta.Config.WaterMaterial;
			}
			if (obj is TerrainCollider)
			{
				return TerrainMeta.Physics.GetMaterial(pos);
			}
			return obj.sharedMaterial;
		}

		// Token: 0x06003DA2 RID: 15778 RVA: 0x0016945B File Offset: 0x0016765B
		public static bool IsOnLayer(this Collider col, Layer rustLayer)
		{
			return col != null && col.gameObject.IsOnLayer(rustLayer);
		}

		// Token: 0x06003DA3 RID: 15779 RVA: 0x00169474 File Offset: 0x00167674
		public static bool IsOnLayer(this Collider col, int layer)
		{
			return col != null && col.gameObject.IsOnLayer(layer);
		}

		// Token: 0x06003DA4 RID: 15780 RVA: 0x00169490 File Offset: 0x00167690
		public static float GetRadius(this Collider col, Vector3 transformScale)
		{
			float num = 1f;
			SphereCollider sphereCollider;
			BoxCollider boxCollider;
			CapsuleCollider capsuleCollider;
			MeshCollider meshCollider;
			if ((sphereCollider = col as SphereCollider) != null)
			{
				num = sphereCollider.radius * transformScale.Max();
			}
			else if ((boxCollider = col as BoxCollider) != null)
			{
				num = Vector3.Scale(boxCollider.size, transformScale).Max() * 0.5f;
			}
			else if ((capsuleCollider = col as CapsuleCollider) != null)
			{
				int direction = capsuleCollider.direction;
				float num2;
				if (direction != 0)
				{
					if (direction != 1)
					{
						num2 = transformScale.x;
					}
					else
					{
						num2 = transformScale.x;
					}
				}
				else
				{
					num2 = transformScale.y;
				}
				num = capsuleCollider.radius * num2;
			}
			else if ((meshCollider = col as MeshCollider) != null)
			{
				num = Vector3.Scale(meshCollider.bounds.size, transformScale).Max() * 0.5f;
			}
			return num;
		}
	}
}
