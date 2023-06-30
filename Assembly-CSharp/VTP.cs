using System;
using UnityEngine;

// Token: 0x020009A7 RID: 2471
public class VTP : MonoBehaviour
{
	// Token: 0x06003AD0 RID: 15056 RVA: 0x0015B93C File Offset: 0x00159B3C
	public static Color getSingleVertexColorAtHit(Transform transform, RaycastHit hit)
	{
		Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = hit.triangleIndex;
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < 3; i++)
		{
			float num3 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + i]]), hit.point);
			if (num3 < num)
			{
				num2 = triangles[triangleIndex * 3 + i];
				num = num3;
			}
		}
		return colors[num2];
	}

	// Token: 0x06003AD1 RID: 15057 RVA: 0x0015B9DC File Offset: 0x00159BDC
	public static Color getFaceVerticesColorAtHit(Transform transform, RaycastHit hit)
	{
		int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = hit.triangleIndex;
		int num = triangles[triangleIndex * 3];
		return (colors[num] + colors[num + 1] + colors[num + 2]) / 3f;
	}

	// Token: 0x06003AD2 RID: 15058 RVA: 0x0015BA44 File Offset: 0x00159C44
	public static void paintSingleVertexOnHit(Transform transform, RaycastHit hit, Color color, float strength)
	{
		Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = hit.triangleIndex;
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < 3; i += 3)
		{
			float num3 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + i]]), hit.point);
			if (num3 < num)
			{
				num2 = triangles[triangleIndex * 3 + i];
				num = num3;
			}
		}
		Color color2 = VTP.VertexColorLerp(colors[num2], color, strength);
		colors[num2] = color2;
		transform.GetComponent<MeshFilter>().sharedMesh.colors = colors;
	}

	// Token: 0x06003AD3 RID: 15059 RVA: 0x0015BB08 File Offset: 0x00159D08
	public static void paintFaceVerticesOnHit(Transform transform, RaycastHit hit, Color color, float strength)
	{
		int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = transform.GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = hit.triangleIndex;
		for (int i = 0; i < 3; i++)
		{
			int num = triangles[triangleIndex * 3 + i];
			Color color2 = VTP.VertexColorLerp(colors[num], color, strength);
			colors[num] = color2;
		}
		transform.GetComponent<MeshFilter>().sharedMesh.colors = colors;
	}

	// Token: 0x06003AD4 RID: 15060 RVA: 0x0015BB84 File Offset: 0x00159D84
	public static void deformSingleVertexOnHit(Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow)
	{
		Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Vector3[] normals = transform.GetComponent<MeshFilter>().sharedMesh.normals;
		int triangleIndex = hit.triangleIndex;
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < 3; i++)
		{
			float num3 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + i]]), hit.point);
			if (num3 < num)
			{
				num2 = triangles[triangleIndex * 3 + i];
				num = num3;
			}
		}
		int num4 = 1;
		if (!up)
		{
			num4 = -1;
		}
		vertices[num2] += (float)num4 * 0.1f * strength * normals[num2];
		transform.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
		if (recalculateNormals)
		{
			transform.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		}
		if (recalculateCollider)
		{
			transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
		}
		if (recalculateFlow)
		{
			Vector4[] array = VTP.calculateMeshTangents(triangles, vertices, transform.GetComponent<MeshCollider>().sharedMesh.uv, normals);
			transform.GetComponent<MeshCollider>().sharedMesh.tangents = array;
			VTP.recalculateMeshForFlow(transform, vertices, normals, array);
		}
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x0015BCD0 File Offset: 0x00159ED0
	public static void deformFaceVerticesOnHit(Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow)
	{
		Vector3[] vertices = transform.GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = transform.GetComponent<MeshFilter>().sharedMesh.triangles;
		Vector3[] normals = transform.GetComponent<MeshFilter>().sharedMesh.normals;
		int triangleIndex = hit.triangleIndex;
		int num = 1;
		if (!up)
		{
			num = -1;
		}
		for (int i = 0; i < 3; i++)
		{
			int num2 = triangles[triangleIndex * 3 + i];
			vertices[num2] += (float)num * 0.1f * strength * normals[num2];
		}
		transform.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
		if (recalculateNormals)
		{
			transform.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		}
		if (recalculateCollider)
		{
			transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
		}
		if (recalculateFlow)
		{
			Vector4[] array = VTP.calculateMeshTangents(triangles, vertices, transform.GetComponent<MeshCollider>().sharedMesh.uv, normals);
			transform.GetComponent<MeshCollider>().sharedMesh.tangents = array;
			VTP.recalculateMeshForFlow(transform, vertices, normals, array);
		}
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x0015BDE8 File Offset: 0x00159FE8
	private static void recalculateMeshForFlow(Transform transform, Vector3[] currentVertices, Vector3[] currentNormals, Vector4[] currentTangents)
	{
		Vector2[] uv = transform.GetComponent<MeshFilter>().sharedMesh.uv4;
		for (int i = 0; i < currentVertices.Length; i++)
		{
			Vector3 vector = transform.TransformDirection(Vector3.Cross(currentNormals[i], new Vector3(currentTangents[i].x, currentTangents[i].y, currentTangents[i].z)).normalized * currentTangents[i].w);
			Vector3 vector2 = transform.TransformDirection(currentTangents[i].normalized);
			float num = 0.5f + 0.5f * vector2.y;
			float num2 = 0.5f + 0.5f * vector.y;
			uv[i] = new Vector2(num, num2);
		}
		transform.GetComponent<MeshFilter>().sharedMesh.uv4 = uv;
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x0015BED4 File Offset: 0x0015A0D4
	private static Vector4[] calculateMeshTangents(int[] triangles, Vector3[] vertices, Vector2[] uv, Vector3[] normals)
	{
		int num = triangles.Length;
		int num2 = vertices.Length;
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Vector4[] array3 = new Vector4[num2];
		for (long num3 = 0L; num3 < (long)num; num3 += 3L)
		{
			long num4 = (long)triangles[(int)(checked((IntPtr)num3))];
			long num5 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 1L))))];
			long num6 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 2L))))];
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector2 vector4;
			Vector2 vector5;
			Vector2 vector6;
			checked
			{
				vector = vertices[(int)((IntPtr)num4)];
				vector2 = vertices[(int)((IntPtr)num5)];
				vector3 = vertices[(int)((IntPtr)num6)];
				vector4 = uv[(int)((IntPtr)num4)];
				vector5 = uv[(int)((IntPtr)num5)];
				vector6 = uv[(int)((IntPtr)num6)];
			}
			float num7 = vector2.x - vector.x;
			float num8 = vector3.x - vector.x;
			float num9 = vector2.y - vector.y;
			float num10 = vector3.y - vector.y;
			float num11 = vector2.z - vector.z;
			float num12 = vector3.z - vector.z;
			float num13 = vector5.x - vector4.x;
			float num14 = vector6.x - vector4.x;
			float num15 = vector5.y - vector4.y;
			float num16 = vector6.y - vector4.y;
			float num17 = num13 * num16 - num14 * num15;
			float num18 = ((num17 == 0f) ? 0f : (1f / num17));
			Vector3 vector7 = new Vector3((num16 * num7 - num15 * num8) * num18, (num16 * num9 - num15 * num10) * num18, (num16 * num11 - num15 * num12) * num18);
			Vector3 vector8 = new Vector3((num13 * num8 - num14 * num7) * num18, (num13 * num10 - num14 * num9) * num18, (num13 * num12 - num14 * num11) * num18);
			checked
			{
				array[(int)((IntPtr)num4)] += vector7;
				array[(int)((IntPtr)num5)] += vector7;
				array[(int)((IntPtr)num6)] += vector7;
				array2[(int)((IntPtr)num4)] += vector8;
				array2[(int)((IntPtr)num5)] += vector8;
				array2[(int)((IntPtr)num6)] += vector8;
			}
		}
		for (long num19 = 0L; num19 < (long)num2; num19 += 1L)
		{
			checked
			{
				Vector3 vector9 = normals[(int)((IntPtr)num19)];
				Vector3 vector10 = array[(int)((IntPtr)num19)];
				Vector3.OrthoNormalize(ref vector9, ref vector10);
				array3[(int)((IntPtr)num19)].x = vector10.x;
				array3[(int)((IntPtr)num19)].y = vector10.y;
				array3[(int)((IntPtr)num19)].z = vector10.z;
				array3[(int)((IntPtr)num19)].w = ((Vector3.Dot(Vector3.Cross(vector9, vector10), array2[(int)((IntPtr)num19)]) < 0f) ? (-1f) : 1f);
			}
		}
		return array3;
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x0015C1FC File Offset: 0x0015A3FC
	public static Color VertexColorLerp(Color colorA, Color colorB, float value)
	{
		if (value >= 1f)
		{
			return colorB;
		}
		if (value <= 0f)
		{
			return colorA;
		}
		return new Color(colorA.r + (colorB.r - colorA.r) * value, colorA.g + (colorB.g - colorA.g) * value, colorA.b + (colorB.b - colorA.b) * value, colorA.a + (colorB.a - colorA.a) * value);
	}
}
