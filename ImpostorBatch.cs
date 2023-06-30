using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200072D RID: 1837
public class ImpostorBatch
{
	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06003338 RID: 13112 RVA: 0x00139DDF File Offset: 0x00137FDF
	// (set) Token: 0x06003337 RID: 13111 RVA: 0x00139DD6 File Offset: 0x00137FD6
	public Mesh Mesh { get; private set; }

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x0600333A RID: 13114 RVA: 0x00139DF0 File Offset: 0x00137FF0
	// (set) Token: 0x06003339 RID: 13113 RVA: 0x00139DE7 File Offset: 0x00137FE7
	public Material Material { get; private set; }

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x0600333C RID: 13116 RVA: 0x00139E01 File Offset: 0x00138001
	// (set) Token: 0x0600333B RID: 13115 RVA: 0x00139DF8 File Offset: 0x00137FF8
	public ComputeBuffer PositionBuffer { get; private set; }

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x0600333E RID: 13118 RVA: 0x00139E12 File Offset: 0x00138012
	// (set) Token: 0x0600333D RID: 13117 RVA: 0x00139E09 File Offset: 0x00138009
	public ComputeBuffer ArgsBuffer { get; private set; }

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x0600333F RID: 13119 RVA: 0x00139E1A File Offset: 0x0013801A
	// (set) Token: 0x06003340 RID: 13120 RVA: 0x00139E22 File Offset: 0x00138022
	public bool IsDirty { get; set; }

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x06003341 RID: 13121 RVA: 0x00139E2B File Offset: 0x0013802B
	public int Count
	{
		get
		{
			return this.Positions.Count;
		}
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x06003342 RID: 13122 RVA: 0x00139E38 File Offset: 0x00138038
	public bool Visible
	{
		get
		{
			return this.Positions.Count - this.recycle.Count > 0;
		}
	}

	// Token: 0x06003343 RID: 13123 RVA: 0x00139E54 File Offset: 0x00138054
	private ComputeBuffer SafeRelease(ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
		}
		return null;
	}

	// Token: 0x06003344 RID: 13124 RVA: 0x00139E60 File Offset: 0x00138060
	public void Initialize(Mesh mesh, Material material)
	{
		this.Mesh = mesh;
		this.Material = material;
		this.Positions = Pool.Get<FPNativeList<Vector4>>();
		this.args = Pool.Get<FPNativeList<uint>>();
		this.args.Resize(5);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
		this.ArgsBuffer = new ComputeBuffer(1, this.args.Count * 4, ComputeBufferType.DrawIndirect);
		this.args[0] = this.Mesh.GetIndexCount(0);
		this.args[2] = this.Mesh.GetIndexStart(0);
		this.args[3] = this.Mesh.GetBaseVertex(0);
	}

	// Token: 0x06003345 RID: 13125 RVA: 0x00139F18 File Offset: 0x00138118
	public void Release()
	{
		this.recycle.Clear();
		Pool.Free<FPNativeList<Vector4>>(ref this.Positions);
		Pool.Free<FPNativeList<uint>>(ref this.args);
		this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x00139F6C File Offset: 0x0013816C
	public void AddInstance(ImpostorInstanceData data)
	{
		data.Batch = this;
		if (this.recycle.Count > 0)
		{
			data.BatchIndex = this.recycle.Dequeue();
			this.Positions[data.BatchIndex] = data.PositionAndScale();
		}
		else
		{
			data.BatchIndex = this.Positions.Count;
			this.Positions.Add(data.PositionAndScale());
		}
		this.IsDirty = true;
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x00139FE4 File Offset: 0x001381E4
	public void RemoveInstance(ImpostorInstanceData data)
	{
		this.Positions[data.BatchIndex] = new Vector4(0f, 0f, 0f, -1f);
		this.recycle.Enqueue(data.BatchIndex);
		data.BatchIndex = 0;
		data.Batch = null;
		this.IsDirty = true;
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x0013A044 File Offset: 0x00138244
	public void UpdateBuffers()
	{
		if (!this.IsDirty)
		{
			return;
		}
		bool flag = false;
		if (this.PositionBuffer == null || this.PositionBuffer.count != this.Positions.Count)
		{
			this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
			this.PositionBuffer = new ComputeBuffer(this.Positions.Count, 16);
			flag = true;
		}
		if (this.PositionBuffer != null)
		{
			this.PositionBuffer.SetData<Vector4>(this.Positions.Array, 0, 0, this.Positions.Count);
		}
		if (this.ArgsBuffer != null && flag)
		{
			this.args[1] = (uint)this.Positions.Count;
			this.ArgsBuffer.SetData<uint>(this.args.Array, 0, 0, this.args.Count);
		}
		this.IsDirty = false;
	}

	// Token: 0x04002A09 RID: 10761
	public FPNativeList<Vector4> Positions;

	// Token: 0x04002A0B RID: 10763
	private FPNativeList<uint> args;

	// Token: 0x04002A0D RID: 10765
	private Queue<int> recycle = new Queue<int>(32);
}
