using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200099F RID: 2463
internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
	// Token: 0x06003A6A RID: 14954 RVA: 0x00158AB8 File Offset: 0x00156CB8
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x00158AE0 File Offset: 0x00156CE0
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
		this.animationStoped = false;
		this.animationLifeTime = (float)(this.TilesX * this.TilesY) / this.AnimationFPS;
		this.count = this.TilesY * this.TilesX;
		this.index = this.TilesX - 1;
		Vector3 zero = Vector3.zero;
		this.StartFrameOffset -= this.StartFrameOffset / this.count * this.count;
		Vector2 vector = new Vector2(1f / (float)this.TilesX, 1f / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", vector);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x00158C03 File Offset: 0x00156E03
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

	// Token: 0x06003A6D RID: 14957 RVA: 0x00158C42 File Offset: 0x00156E42
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x00158C51 File Offset: 0x00156E51
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

	// Token: 0x06003A6F RID: 14959 RVA: 0x00158C6F File Offset: 0x00156E6F
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x00158C90 File Offset: 0x00156E90
	private IEnumerator UpdateCorutine()
	{
		this.animationStartTime = Time.time;
		while (this.isVisible && (this.IsLoop || !this.animationStoped))
		{
			this.UpdateFrame();
			if (!this.IsLoop && this.animationStoped)
			{
				break;
			}
			float num = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num2 = this.FrameOverTime.Evaluate(Mathf.Clamp01(num));
			yield return new WaitForSeconds(1f / (this.AnimationFPS * num2));
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x00158CA0 File Offset: 0x00156EA0
	private void UpdateFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		if (this.count == this.allCount)
		{
			this.animationStartTime = Time.time;
			this.allCount = 0;
			this.animationStoped = true;
		}
		Vector2 vector = new Vector2((float)this.index / (float)this.TilesX - (float)(this.index / this.TilesX), 1f - (float)(this.index / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", vector);
		}
		if (this.IsInterpolateFrames)
		{
			this.currentInterpolatedTime = 0f;
		}
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x00158D78 File Offset: 0x00156F78
	private void Update()
	{
		if (!this.IsInterpolateFrames)
		{
			return;
		}
		this.currentInterpolatedTime += Time.deltaTime;
		int num = this.index + 1;
		if (this.allCount == 0)
		{
			num = this.index;
		}
		Vector4 vector = new Vector4(1f / (float)this.TilesX, 1f / (float)this.TilesY, (float)num / (float)this.TilesX - (float)(num / this.TilesX), 1f - (float)(num / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetVector("_MainTex_NextFrame", vector);
			float num2 = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num3 = this.FrameOverTime.Evaluate(Mathf.Clamp01(num2));
			this.instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(this.currentInterpolatedTime * this.AnimationFPS * num3));
		}
	}

	// Token: 0x06003A73 RID: 14963 RVA: 0x00158E6D File Offset: 0x0015706D
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}

	// Token: 0x04003523 RID: 13603
	public int TilesX = 4;

	// Token: 0x04003524 RID: 13604
	public int TilesY = 4;

	// Token: 0x04003525 RID: 13605
	public float AnimationFPS = 30f;

	// Token: 0x04003526 RID: 13606
	public bool IsInterpolateFrames;

	// Token: 0x04003527 RID: 13607
	public int StartFrameOffset;

	// Token: 0x04003528 RID: 13608
	public bool IsLoop = true;

	// Token: 0x04003529 RID: 13609
	public float StartDelay;

	// Token: 0x0400352A RID: 13610
	public AnimationCurve FrameOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x0400352B RID: 13611
	private bool isInizialised;

	// Token: 0x0400352C RID: 13612
	private int index;

	// Token: 0x0400352D RID: 13613
	private int count;

	// Token: 0x0400352E RID: 13614
	private int allCount;

	// Token: 0x0400352F RID: 13615
	private float animationLifeTime;

	// Token: 0x04003530 RID: 13616
	private bool isVisible;

	// Token: 0x04003531 RID: 13617
	private bool isCorutineStarted;

	// Token: 0x04003532 RID: 13618
	private Renderer currentRenderer;

	// Token: 0x04003533 RID: 13619
	private Material instanceMaterial;

	// Token: 0x04003534 RID: 13620
	private float currentInterpolatedTime;

	// Token: 0x04003535 RID: 13621
	private float animationStartTime;

	// Token: 0x04003536 RID: 13622
	private bool animationStoped;
}
