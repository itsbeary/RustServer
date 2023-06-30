using System;
using UnityEngine;

// Token: 0x020002FF RID: 767
public class HitboxDefinition : MonoBehaviour
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001E93 RID: 7827 RVA: 0x000CF9B1 File Offset: 0x000CDBB1
	// (set) Token: 0x06001E94 RID: 7828 RVA: 0x000CF9B9 File Offset: 0x000CDBB9
	public Vector3 Scale
	{
		get
		{
			return this.scale;
		}
		set
		{
			this.scale = new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
		}
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001E95 RID: 7829 RVA: 0x000CF9E7 File Offset: 0x000CDBE7
	public Matrix4x4 LocalMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
		}
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000CFA05 File Offset: 0x000CDC05
	private void OnValidate()
	{
		this.Scale = this.Scale;
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000CFA14 File Offset: 0x000CDC14
	protected virtual void OnDrawGizmosSelected()
	{
		HitboxDefinition.Type type = this.type;
		if (type == HitboxDefinition.Type.BOX)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
			Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			return;
		}
		if (type != HitboxDefinition.Type.CAPSULE)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), Vector3.one);
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCapsuleY(Vector3.zero, this.scale.x, this.scale.y);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x000CFB44 File Offset: 0x000CDD44
	protected virtual void OnDrawGizmos()
	{
		HitboxDefinition.Type type = this.type;
		if (type == HitboxDefinition.Type.BOX)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(Vector3.zero, 0.005f);
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			return;
		}
		if (type != HitboxDefinition.Type.CAPSULE)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), Vector3.one);
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(Vector3.zero, 0.005f);
		GizmosUtil.DrawWireCapsuleY(Vector3.zero, this.scale.x, this.scale.y);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x040017A3 RID: 6051
	public Vector3 center;

	// Token: 0x040017A4 RID: 6052
	public Vector3 rotation;

	// Token: 0x040017A5 RID: 6053
	public HitboxDefinition.Type type;

	// Token: 0x040017A6 RID: 6054
	public int priority;

	// Token: 0x040017A7 RID: 6055
	public PhysicMaterial physicMaterial;

	// Token: 0x040017A8 RID: 6056
	[SerializeField]
	private Vector3 scale = Vector3.one;

	// Token: 0x02000CB9 RID: 3257
	public enum Type
	{
		// Token: 0x04004501 RID: 17665
		BOX,
		// Token: 0x04004502 RID: 17666
		CAPSULE
	}
}
