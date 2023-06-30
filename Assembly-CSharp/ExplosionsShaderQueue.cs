using System;
using UnityEngine;

// Token: 0x0200099E RID: 2462
public class ExplosionsShaderQueue : MonoBehaviour
{
	// Token: 0x06003A66 RID: 14950 RVA: 0x00158A14 File Offset: 0x00156C14
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue += this.AddQueue;
			return;
		}
		base.Invoke("SetProjectorQueue", 0.1f);
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x00158A69 File Offset: 0x00156C69
	private void SetProjectorQueue()
	{
		base.GetComponent<Projector>().material.renderQueue += this.AddQueue;
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x00158A88 File Offset: 0x00156C88
	private void OnDisable()
	{
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue = -1;
		}
	}

	// Token: 0x04003521 RID: 13601
	public int AddQueue = 1;

	// Token: 0x04003522 RID: 13602
	private Renderer rend;
}
