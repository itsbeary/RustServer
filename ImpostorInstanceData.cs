using System;
using UnityEngine;

// Token: 0x0200072C RID: 1836
public class ImpostorInstanceData
{
	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x0600332B RID: 13099 RVA: 0x00139C01 File Offset: 0x00137E01
	// (set) Token: 0x0600332A RID: 13098 RVA: 0x00139BF8 File Offset: 0x00137DF8
	public Renderer Renderer { get; private set; }

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x0600332D RID: 13101 RVA: 0x00139C12 File Offset: 0x00137E12
	// (set) Token: 0x0600332C RID: 13100 RVA: 0x00139C09 File Offset: 0x00137E09
	public Mesh Mesh { get; private set; }

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x0600332F RID: 13103 RVA: 0x00139C23 File Offset: 0x00137E23
	// (set) Token: 0x0600332E RID: 13102 RVA: 0x00139C1A File Offset: 0x00137E1A
	public Material Material { get; private set; }

	// Token: 0x06003330 RID: 13104 RVA: 0x00139C2B File Offset: 0x00137E2B
	public ImpostorInstanceData(Renderer renderer, Mesh mesh, Material material)
	{
		this.Renderer = renderer;
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x00139C68 File Offset: 0x00137E68
	public ImpostorInstanceData(Vector3 position, Vector3 scale, Mesh mesh, Material material)
	{
		this.positionAndScale = new Vector4(position.x, position.y, position.z, scale.x);
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x00139CCA File Offset: 0x00137ECA
	private int GenerateHashCode()
	{
		return (17 * 31 + this.Material.GetHashCode()) * 31 + this.Mesh.GetHashCode();
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x00139CEC File Offset: 0x00137EEC
	public override bool Equals(object obj)
	{
		ImpostorInstanceData impostorInstanceData = obj as ImpostorInstanceData;
		return impostorInstanceData.Material == this.Material && impostorInstanceData.Mesh == this.Mesh;
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x00139D26 File Offset: 0x00137F26
	public override int GetHashCode()
	{
		return this.hash;
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x00139D30 File Offset: 0x00137F30
	public Vector4 PositionAndScale()
	{
		if (this.Renderer != null)
		{
			Transform transform = this.Renderer.transform;
			Vector3 position = transform.position;
			Vector3 lossyScale = transform.lossyScale;
			float num = (this.Renderer.enabled ? lossyScale.x : (-lossyScale.x));
			this.positionAndScale = new Vector4(position.x, position.y, position.z, num);
		}
		return this.positionAndScale;
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x00139DA4 File Offset: 0x00137FA4
	public void Update()
	{
		if (this.Batch != null)
		{
			this.Batch.Positions[this.BatchIndex] = this.PositionAndScale();
			this.Batch.IsDirty = true;
		}
	}

	// Token: 0x04002A02 RID: 10754
	public ImpostorBatch Batch;

	// Token: 0x04002A03 RID: 10755
	public int BatchIndex;

	// Token: 0x04002A04 RID: 10756
	private int hash;

	// Token: 0x04002A05 RID: 10757
	private Vector4 positionAndScale = Vector4.zero;
}
