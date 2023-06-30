using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009BC RID: 2492
	public static class MeshGenerator
	{
		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06003B40 RID: 15168 RVA: 0x0015E5DC File Offset: 0x0015C7DC
		private static bool duplicateBackFaces
		{
			get
			{
				return Config.Instance.forceSinglePass;
			}
		}

		// Token: 0x06003B41 RID: 15169 RVA: 0x0015E5E8 File Offset: 0x0015C7E8
		public static Mesh GenerateConeZ_RadiusAndAngle(float lengthZ, float radiusStart, float coneAngle, int numSides, int numSegments, bool cap)
		{
			Debug.Assert(lengthZ > 0f);
			Debug.Assert(coneAngle > 0f && coneAngle < 180f);
			float num = lengthZ * Mathf.Tan(coneAngle * 0.017453292f * 0.5f);
			return MeshGenerator.GenerateConeZ_Radius(lengthZ, radiusStart, num, numSides, numSegments, cap);
		}

		// Token: 0x06003B42 RID: 15170 RVA: 0x0015E63C File Offset: 0x0015C83C
		public static Mesh GenerateConeZ_Angle(float lengthZ, float coneAngle, int numSides, int numSegments, bool cap)
		{
			return MeshGenerator.GenerateConeZ_RadiusAndAngle(lengthZ, 0f, coneAngle, numSides, numSegments, cap);
		}

		// Token: 0x06003B43 RID: 15171 RVA: 0x0015E650 File Offset: 0x0015C850
		public static Mesh GenerateConeZ_Radius(float lengthZ, float radiusStart, float radiusEnd, int numSides, int numSegments, bool cap)
		{
			Debug.Assert(lengthZ > 0f);
			Debug.Assert(radiusStart >= 0f);
			Debug.Assert(numSides >= 3);
			Debug.Assert(numSegments >= 0);
			Mesh mesh = new Mesh();
			bool flag = cap && radiusStart > 0f;
			radiusStart = Mathf.Max(radiusStart, 0.001f);
			int num = numSides * (numSegments + 2);
			int num2 = num;
			if (flag)
			{
				num2 += numSides + 1;
			}
			Vector3[] array = new Vector3[num2];
			for (int i = 0; i < numSides; i++)
			{
				float num3 = 6.2831855f * (float)i / (float)numSides;
				float num4 = Mathf.Cos(num3);
				float num5 = Mathf.Sin(num3);
				for (int j = 0; j < numSegments + 2; j++)
				{
					float num6 = (float)j / (float)(numSegments + 1);
					Debug.Assert(num6 >= 0f && num6 <= 1f);
					float num7 = Mathf.Lerp(radiusStart, radiusEnd, num6);
					array[i + j * numSides] = new Vector3(num7 * num4, num7 * num5, num6 * lengthZ);
				}
			}
			if (flag)
			{
				int num8 = num;
				array[num8] = Vector3.zero;
				num8++;
				for (int k = 0; k < numSides; k++)
				{
					float num9 = 6.2831855f * (float)k / (float)numSides;
					float num10 = Mathf.Cos(num9);
					float num11 = Mathf.Sin(num9);
					array[num8] = new Vector3(radiusStart * num10, radiusStart * num11, 0f);
					num8++;
				}
				Debug.Assert(num8 == array.Length);
			}
			if (!MeshGenerator.duplicateBackFaces)
			{
				mesh.vertices = array;
			}
			else
			{
				Vector3[] array2 = new Vector3[array.Length * 2];
				array.CopyTo(array2, 0);
				array.CopyTo(array2, array.Length);
				mesh.vertices = array2;
			}
			Vector2[] array3 = new Vector2[num2];
			int num12 = 0;
			for (int l = 0; l < num; l++)
			{
				array3[num12++] = Vector2.zero;
			}
			if (flag)
			{
				for (int m = 0; m < numSides + 1; m++)
				{
					array3[num12++] = new Vector2(1f, 0f);
				}
			}
			Debug.Assert(num12 == array3.Length);
			if (!MeshGenerator.duplicateBackFaces)
			{
				mesh.uv = array3;
			}
			else
			{
				Vector2[] array4 = new Vector2[array3.Length * 2];
				array3.CopyTo(array4, 0);
				array3.CopyTo(array4, array3.Length);
				for (int n = 0; n < array3.Length; n++)
				{
					Vector2 vector = array4[n + array3.Length];
					array4[n + array3.Length] = new Vector2(vector.x, 1f);
				}
				mesh.uv = array4;
			}
			int num13 = numSides * 2 * Mathf.Max(numSegments + 1, 1) * 3;
			if (flag)
			{
				num13 += numSides * 3;
			}
			int[] array5 = new int[num13];
			int num14 = 0;
			for (int num15 = 0; num15 < numSides; num15++)
			{
				int num16 = num15 + 1;
				if (num16 == numSides)
				{
					num16 = 0;
				}
				for (int num17 = 0; num17 < numSegments + 1; num17++)
				{
					int num18 = num17 * numSides;
					array5[num14++] = num18 + num15;
					array5[num14++] = num18 + num16;
					array5[num14++] = num18 + num15 + numSides;
					array5[num14++] = num18 + num16 + numSides;
					array5[num14++] = num18 + num15 + numSides;
					array5[num14++] = num18 + num16;
				}
			}
			if (flag)
			{
				for (int num19 = 0; num19 < numSides - 1; num19++)
				{
					array5[num14++] = num;
					array5[num14++] = num + num19 + 2;
					array5[num14++] = num + num19 + 1;
				}
				array5[num14++] = num;
				array5[num14++] = num + 1;
				array5[num14++] = num + numSides;
			}
			Debug.Assert(num14 == array5.Length);
			if (!MeshGenerator.duplicateBackFaces)
			{
				mesh.triangles = array5;
			}
			else
			{
				int[] array6 = new int[array5.Length * 2];
				array5.CopyTo(array6, 0);
				for (int num20 = 0; num20 < array5.Length; num20 += 3)
				{
					array6[array5.Length + num20] = array5[num20] + num2;
					array6[array5.Length + num20 + 1] = array5[num20 + 2] + num2;
					array6[array5.Length + num20 + 2] = array5[num20 + 1] + num2;
				}
				mesh.triangles = array6;
			}
			Bounds bounds = new Bounds(new Vector3(0f, 0f, lengthZ * 0.5f), new Vector3(Mathf.Max(radiusStart, radiusEnd) * 2f, Mathf.Max(radiusStart, radiusEnd) * 2f, lengthZ));
			mesh.bounds = bounds;
			Debug.Assert(mesh.vertexCount == MeshGenerator.GetVertexCount(numSides, numSegments, flag));
			Debug.Assert(mesh.triangles.Length == MeshGenerator.GetIndicesCount(numSides, numSegments, flag));
			return mesh;
		}

		// Token: 0x06003B44 RID: 15172 RVA: 0x0015EB30 File Offset: 0x0015CD30
		public static int GetVertexCount(int numSides, int numSegments, bool geomCap)
		{
			Debug.Assert(numSides >= 2);
			Debug.Assert(numSegments >= 0);
			int num = numSides * (numSegments + 2);
			if (geomCap)
			{
				num += numSides + 1;
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				num *= 2;
			}
			return num;
		}

		// Token: 0x06003B45 RID: 15173 RVA: 0x0015EB70 File Offset: 0x0015CD70
		public static int GetIndicesCount(int numSides, int numSegments, bool geomCap)
		{
			Debug.Assert(numSides >= 2);
			Debug.Assert(numSegments >= 0);
			int num = numSides * (numSegments + 1) * 2 * 3;
			if (geomCap)
			{
				num += numSides * 3;
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				num *= 2;
			}
			return num;
		}

		// Token: 0x06003B46 RID: 15174 RVA: 0x0015EBB4 File Offset: 0x0015CDB4
		public static int GetSharedMeshVertexCount()
		{
			return MeshGenerator.GetVertexCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
		}

		// Token: 0x06003B47 RID: 15175 RVA: 0x0015EBD0 File Offset: 0x0015CDD0
		public static int GetSharedMeshIndicesCount()
		{
			return MeshGenerator.GetIndicesCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
		}

		// Token: 0x04003648 RID: 13896
		private const float kMinTruncatedRadius = 0.001f;
	}
}
