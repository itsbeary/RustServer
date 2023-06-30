using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA2 RID: 2722
	internal static class MeshUtilities
	{
		// Token: 0x060040BD RID: 16573 RVA: 0x0017D0E8 File Offset: 0x0017B2E8
		internal static Mesh GetColliderMesh(Collider collider)
		{
			Type type = collider.GetType();
			if (type == typeof(MeshCollider))
			{
				return ((MeshCollider)collider).sharedMesh;
			}
			Assert.IsTrue(MeshUtilities.s_ColliderPrimitives.ContainsKey(type), "Unknown collider");
			return MeshUtilities.GetPrimitive(MeshUtilities.s_ColliderPrimitives[type]);
		}

		// Token: 0x060040BE RID: 16574 RVA: 0x0017D140 File Offset: 0x0017B340
		internal static Mesh GetPrimitive(PrimitiveType primitiveType)
		{
			Mesh builtinMesh;
			if (!MeshUtilities.s_Primitives.TryGetValue(primitiveType, out builtinMesh))
			{
				builtinMesh = MeshUtilities.GetBuiltinMesh(primitiveType);
				MeshUtilities.s_Primitives.Add(primitiveType, builtinMesh);
			}
			return builtinMesh;
		}

		// Token: 0x060040BF RID: 16575 RVA: 0x0017D170 File Offset: 0x0017B370
		private static Mesh GetBuiltinMesh(PrimitiveType primitiveType)
		{
			GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
			Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			RuntimeUtilities.Destroy(gameObject);
			return sharedMesh;
		}

		// Token: 0x04003A12 RID: 14866
		private static Dictionary<PrimitiveType, Mesh> s_Primitives = new Dictionary<PrimitiveType, Mesh>();

		// Token: 0x04003A13 RID: 14867
		private static Dictionary<Type, PrimitiveType> s_ColliderPrimitives = new Dictionary<Type, PrimitiveType>
		{
			{
				typeof(BoxCollider),
				PrimitiveType.Cube
			},
			{
				typeof(SphereCollider),
				PrimitiveType.Sphere
			},
			{
				typeof(CapsuleCollider),
				PrimitiveType.Capsule
			}
		};
	}
}
