using System;
using UnityEngine;

// Token: 0x0200090B RID: 2315
public class MeshToggle : MonoBehaviour
{
	// Token: 0x060037F3 RID: 14323 RVA: 0x0014DE1C File Offset: 0x0014C01C
	public void SwitchRenderer(int index)
	{
		if (this.RendererMeshes.Length == 0)
		{
			return;
		}
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.RendererMeshes[Mathf.Clamp(index, 0, this.RendererMeshes.Length - 1)];
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x0014DE64 File Offset: 0x0014C064
	public void SwitchRenderer(float factor)
	{
		int num = Mathf.RoundToInt(factor * (float)this.RendererMeshes.Length);
		this.SwitchRenderer(num);
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x0014DE8C File Offset: 0x0014C08C
	public void SwitchCollider(int index)
	{
		if (this.ColliderMeshes.Length == 0)
		{
			return;
		}
		MeshCollider component = base.GetComponent<MeshCollider>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.ColliderMeshes[Mathf.Clamp(index, 0, this.ColliderMeshes.Length - 1)];
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x0014DED4 File Offset: 0x0014C0D4
	public void SwitchCollider(float factor)
	{
		int num = Mathf.RoundToInt(factor * (float)this.ColliderMeshes.Length);
		this.SwitchCollider(num);
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x0014DEF9 File Offset: 0x0014C0F9
	public void SwitchAll(int index)
	{
		this.SwitchRenderer(index);
		this.SwitchCollider(index);
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x0014DF09 File Offset: 0x0014C109
	public void SwitchAll(float factor)
	{
		this.SwitchRenderer(factor);
		this.SwitchCollider(factor);
	}

	// Token: 0x04003354 RID: 13140
	public Mesh[] RendererMeshes;

	// Token: 0x04003355 RID: 13141
	public Mesh[] ColliderMeshes;
}
