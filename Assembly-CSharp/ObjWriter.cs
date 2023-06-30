using System;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x0200094C RID: 2380
public static class ObjWriter
{
	// Token: 0x060038F3 RID: 14579 RVA: 0x00152194 File Offset: 0x00150394
	public static string MeshToString(Mesh mesh)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(mesh.name).Append("\n");
		foreach (Vector3 vector in mesh.vertices)
		{
			stringBuilder.Append(string.Format("v {0} {1} {2}\n", -vector.x, vector.y, vector.z));
		}
		stringBuilder.Append("\n");
		foreach (Vector3 vector2 in mesh.normals)
		{
			stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -vector2.x, vector2.y, vector2.z));
		}
		stringBuilder.Append("\n");
		Vector2[] uv = mesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			Vector3 vector3 = uv[i];
			stringBuilder.Append(string.Format("vt {0} {1}\n", vector3.x, vector3.y));
		}
		stringBuilder.Append("\n");
		int[] triangles = mesh.triangles;
		for (int j = 0; j < triangles.Length; j += 3)
		{
			int num = triangles[j] + 1;
			int num2 = triangles[j + 1] + 1;
			int num3 = triangles[j + 2] + 1;
			stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", num, num2, num3));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060038F4 RID: 14580 RVA: 0x00152348 File Offset: 0x00150548
	public static void Write(Mesh mesh, string path)
	{
		using (StreamWriter streamWriter = new StreamWriter(path))
		{
			streamWriter.Write(ObjWriter.MeshToString(mesh));
		}
	}
}
