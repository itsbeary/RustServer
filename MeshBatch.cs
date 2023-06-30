using System;
using Rust;
using UnityEngine;

// Token: 0x02000947 RID: 2375
public abstract class MeshBatch : MonoBehaviour
{
	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x060038B9 RID: 14521 RVA: 0x001518E1 File Offset: 0x0014FAE1
	// (set) Token: 0x060038BA RID: 14522 RVA: 0x001518E9 File Offset: 0x0014FAE9
	public bool NeedsRefresh { get; private set; }

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x060038BB RID: 14523 RVA: 0x001518F2 File Offset: 0x0014FAF2
	// (set) Token: 0x060038BC RID: 14524 RVA: 0x001518FA File Offset: 0x0014FAFA
	public int Count { get; private set; }

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x060038BD RID: 14525 RVA: 0x00151903 File Offset: 0x0014FB03
	// (set) Token: 0x060038BE RID: 14526 RVA: 0x0015190B File Offset: 0x0014FB0B
	public int BatchedCount { get; private set; }

	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x060038BF RID: 14527 RVA: 0x00151914 File Offset: 0x0014FB14
	// (set) Token: 0x060038C0 RID: 14528 RVA: 0x0015191C File Offset: 0x0014FB1C
	public int VertexCount { get; private set; }

	// Token: 0x060038C1 RID: 14529
	protected abstract void AllocMemory();

	// Token: 0x060038C2 RID: 14530
	protected abstract void FreeMemory();

	// Token: 0x060038C3 RID: 14531
	protected abstract void RefreshMesh();

	// Token: 0x060038C4 RID: 14532
	protected abstract void ApplyMesh();

	// Token: 0x060038C5 RID: 14533
	protected abstract void ToggleMesh(bool state);

	// Token: 0x060038C6 RID: 14534
	protected abstract void OnPooled();

	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x060038C7 RID: 14535
	public abstract int VertexCapacity { get; }

	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x060038C8 RID: 14536
	public abstract int VertexCutoff { get; }

	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x060038C9 RID: 14537 RVA: 0x00151925 File Offset: 0x0014FB25
	public int AvailableVertices
	{
		get
		{
			return Mathf.Clamp(this.VertexCapacity, this.VertexCutoff, 65534) - this.VertexCount;
		}
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x00151944 File Offset: 0x0014FB44
	public void Alloc()
	{
		this.AllocMemory();
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x0015194C File Offset: 0x0014FB4C
	public void Free()
	{
		this.FreeMemory();
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x00151954 File Offset: 0x0014FB54
	public void Refresh()
	{
		this.RefreshMesh();
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x0015195C File Offset: 0x0014FB5C
	public void Apply()
	{
		this.NeedsRefresh = false;
		this.ApplyMesh();
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x0015196B File Offset: 0x0014FB6B
	public void Display()
	{
		this.ToggleMesh(true);
		this.BatchedCount = this.Count;
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x00151980 File Offset: 0x0014FB80
	public void Invalidate()
	{
		this.ToggleMesh(false);
		this.BatchedCount = 0;
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x00151990 File Offset: 0x0014FB90
	protected void AddVertices(int vertices)
	{
		this.NeedsRefresh = true;
		int count = this.Count;
		this.Count = count + 1;
		this.VertexCount += vertices;
	}

	// Token: 0x060038D1 RID: 14545 RVA: 0x001519C2 File Offset: 0x0014FBC2
	protected void OnEnable()
	{
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
	}

	// Token: 0x060038D2 RID: 14546 RVA: 0x001519E0 File Offset: 0x0014FBE0
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
		this.OnPooled();
	}
}
