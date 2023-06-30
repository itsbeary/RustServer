using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200098B RID: 2443
internal class UVTextureAnimator : MonoBehaviour
{
	// Token: 0x06003A02 RID: 14850 RVA: 0x00156D44 File Offset: 0x00154F44
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003A03 RID: 14851 RVA: 0x00156D6C File Offset: 0x00154F6C
	private void InitDefaultVariables()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		if (this.currentRenderer == null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!this.currentRenderer.enabled)
		{
			this.currentRenderer.enabled = true;
		}
		this.allCount = 0;
		this.deltaFps = 1f / this.Fps;
		this.count = this.Rows * this.Columns;
		this.index = this.Columns - 1;
		Vector3 zero = Vector3.zero;
		this.OffsetMat -= this.OffsetMat / this.count * this.count;
		Vector2 vector = new Vector2(1f / (float)this.Columns, 1f / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", vector);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06003A04 RID: 14852 RVA: 0x00156E7F File Offset: 0x0015507F
	private void Play()
	{
		if (this.isCorutineStarted)
		{
			return;
		}
		if (this.StartDelay > 0.0001f)
		{
			base.Invoke("PlayDelay", this.StartDelay);
		}
		else
		{
			base.StartCoroutine(this.UpdateCorutine());
		}
		this.isCorutineStarted = true;
	}

	// Token: 0x06003A05 RID: 14853 RVA: 0x00156EBE File Offset: 0x001550BE
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06003A06 RID: 14854 RVA: 0x00156ECD File Offset: 0x001550CD
	private void OnEnable()
	{
		if (!this.isInizialised)
		{
			return;
		}
		this.InitDefaultVariables();
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003A07 RID: 14855 RVA: 0x00156EEB File Offset: 0x001550EB
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x06003A08 RID: 14856 RVA: 0x00156F0C File Offset: 0x0015510C
	private IEnumerator UpdateCorutine()
	{
		while (this.isVisible && (this.IsLoop || this.allCount != this.count))
		{
			this.UpdateCorutineFrame();
			if (!this.IsLoop && this.allCount == this.count)
			{
				break;
			}
			yield return new WaitForSeconds(this.deltaFps);
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06003A09 RID: 14857 RVA: 0x00156F1C File Offset: 0x0015511C
	private void UpdateCorutineFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		Vector2 vector = new Vector2((float)this.index / (float)this.Columns - (float)(this.index / this.Columns), 1f - (float)(this.index / this.Columns) / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", vector);
		}
	}

	// Token: 0x06003A0A RID: 14858 RVA: 0x00156FBA File Offset: 0x001551BA
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}

	// Token: 0x040034BA RID: 13498
	public int Rows = 4;

	// Token: 0x040034BB RID: 13499
	public int Columns = 4;

	// Token: 0x040034BC RID: 13500
	public float Fps = 20f;

	// Token: 0x040034BD RID: 13501
	public int OffsetMat;

	// Token: 0x040034BE RID: 13502
	public bool IsLoop = true;

	// Token: 0x040034BF RID: 13503
	public float StartDelay;

	// Token: 0x040034C0 RID: 13504
	private bool isInizialised;

	// Token: 0x040034C1 RID: 13505
	private int index;

	// Token: 0x040034C2 RID: 13506
	private int count;

	// Token: 0x040034C3 RID: 13507
	private int allCount;

	// Token: 0x040034C4 RID: 13508
	private float deltaFps;

	// Token: 0x040034C5 RID: 13509
	private bool isVisible;

	// Token: 0x040034C6 RID: 13510
	private bool isCorutineStarted;

	// Token: 0x040034C7 RID: 13511
	private Renderer currentRenderer;

	// Token: 0x040034C8 RID: 13512
	private Material instanceMaterial;
}
