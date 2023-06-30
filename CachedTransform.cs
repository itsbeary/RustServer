using System;
using UnityEngine;

// Token: 0x020008F9 RID: 2297
public struct CachedTransform<T> where T : Component
{
	// Token: 0x060037C2 RID: 14274 RVA: 0x0014D270 File Offset: 0x0014B470
	public CachedTransform(T instance)
	{
		this.component = instance;
		if (this.component)
		{
			this.position = this.component.transform.position;
			this.rotation = this.component.transform.rotation;
			this.localScale = this.component.transform.localScale;
			return;
		}
		this.position = Vector3.zero;
		this.rotation = Quaternion.identity;
		this.localScale = Vector3.one;
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x0014D30C File Offset: 0x0014B50C
	public void Apply()
	{
		if (this.component)
		{
			this.component.transform.SetPositionAndRotation(this.position, this.rotation);
			this.component.transform.localScale = this.localScale;
		}
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x0014D368 File Offset: 0x0014B568
	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
		Vector3 vector = quaternion * (this.position - center);
		this.position = center + vector;
		this.rotation *= Quaternion.Inverse(this.rotation) * quaternion * this.rotation;
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x060037C5 RID: 14277 RVA: 0x0014D3CA File Offset: 0x0014B5CA
	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.position, this.rotation, this.localScale);
		}
	}

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x060037C6 RID: 14278 RVA: 0x0014D3E4 File Offset: 0x0014B5E4
	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			return this.localToWorldMatrix.inverse;
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x060037C7 RID: 14279 RVA: 0x0014D3FF File Offset: 0x0014B5FF
	public Vector3 forward
	{
		get
		{
			return this.rotation * Vector3.forward;
		}
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x060037C8 RID: 14280 RVA: 0x0014D411 File Offset: 0x0014B611
	public Vector3 up
	{
		get
		{
			return this.rotation * Vector3.up;
		}
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x060037C9 RID: 14281 RVA: 0x0014D423 File Offset: 0x0014B623
	public Vector3 right
	{
		get
		{
			return this.rotation * Vector3.right;
		}
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x0014D435 File Offset: 0x0014B635
	public static implicit operator bool(CachedTransform<T> instance)
	{
		return instance.component != null;
	}

	// Token: 0x0400331C RID: 13084
	public T component;

	// Token: 0x0400331D RID: 13085
	public Vector3 position;

	// Token: 0x0400331E RID: 13086
	public Quaternion rotation;

	// Token: 0x0400331F RID: 13087
	public Vector3 localScale;
}
