using System;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class TriangleIdentifier : MonoBehaviour
{
	// Token: 0x06001F0C RID: 7948 RVA: 0x000D3294 File Offset: 0x000D1494
	private void OnDrawGizmosSelected()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component == null || component.sharedMesh == null)
		{
			return;
		}
		int[] triangles = component.sharedMesh.GetTriangles(this.SubmeshID);
		if (this.TriangleID < 0 || this.TriangleID * 3 > triangles.Length)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 vector = component.sharedMesh.vertices[this.TriangleID * 3];
		Vector3 vector2 = component.sharedMesh.vertices[this.TriangleID * 3 + 1];
		Vector3 vector3 = component.sharedMesh.vertices[this.TriangleID * 3 + 2];
		Vector3 vector4 = component.sharedMesh.normals[this.TriangleID * 3];
		Vector3 vector5 = component.sharedMesh.normals[this.TriangleID * 3 + 1];
		Vector3 vector6 = component.sharedMesh.normals[this.TriangleID * 3 + 2];
		Vector3 vector7 = (vector + vector2 + vector3) / 3f;
		Vector3 vector8 = (vector4 + vector5 + vector6) / 3f;
		Gizmos.DrawLine(vector7, vector7 + vector8 * this.LineLength);
	}

	// Token: 0x040017E8 RID: 6120
	public int TriangleID;

	// Token: 0x040017E9 RID: 6121
	public int SubmeshID;

	// Token: 0x040017EA RID: 6122
	public float LineLength = 1.5f;
}
