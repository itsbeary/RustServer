using System;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class v_chainsaw : MonoBehaviour
{
	// Token: 0x06001942 RID: 6466 RVA: 0x000B9A42 File Offset: 0x000B7C42
	public void OnEnable()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.saveST = this.chainRenderer.sharedMaterial.GetVector("_MainTex_ST");
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x000B9A77 File Offset: 0x000B7C77
	private void Awake()
	{
		this.chainlink = this.chainRenderer.sharedMaterial;
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000B9A8C File Offset: 0x000B7C8C
	private void ScrollChainTexture()
	{
		float num = (this.chainAmount = (this.chainAmount + Time.deltaTime * this.chainSpeed) % 1f);
		this.block.Clear();
		this.block.SetVector("_MainTex_ST", new Vector4(this.saveST.x, this.saveST.y, num, 0f));
		this.chainRenderer.SetPropertyBlock(this.block);
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000B9B0C File Offset: 0x000B7D0C
	private void Update()
	{
		this.chainsawAnimator.SetBool("attacking", this.bAttacking);
		this.smokeEffect.enableEmission = this.bEngineOn;
		ParticleSystem[] array;
		if (this.bHitMetal)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitMetalSoundDef);
			return;
		}
		if (this.bHitWood)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitWoodSoundDef);
			return;
		}
		if (this.bHitFlesh)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			this.DoHitSound(this.hitFleshSoundDef);
			return;
		}
		this.chainsawAnimator.SetBool("attackHit", false);
		array = this.hitMetalFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitWoodFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitFleshFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x000063A5 File Offset: 0x000045A5
	private void DoHitSound(SoundDefinition soundDef)
	{
	}

	// Token: 0x040011FF RID: 4607
	public bool bAttacking;

	// Token: 0x04001200 RID: 4608
	public bool bHitMetal;

	// Token: 0x04001201 RID: 4609
	public bool bHitWood;

	// Token: 0x04001202 RID: 4610
	public bool bHitFlesh;

	// Token: 0x04001203 RID: 4611
	public bool bEngineOn;

	// Token: 0x04001204 RID: 4612
	public ParticleSystem[] hitMetalFX;

	// Token: 0x04001205 RID: 4613
	public ParticleSystem[] hitWoodFX;

	// Token: 0x04001206 RID: 4614
	public ParticleSystem[] hitFleshFX;

	// Token: 0x04001207 RID: 4615
	public SoundDefinition hitMetalSoundDef;

	// Token: 0x04001208 RID: 4616
	public SoundDefinition hitWoodSoundDef;

	// Token: 0x04001209 RID: 4617
	public SoundDefinition hitFleshSoundDef;

	// Token: 0x0400120A RID: 4618
	public Sound hitSound;

	// Token: 0x0400120B RID: 4619
	public GameObject hitSoundTarget;

	// Token: 0x0400120C RID: 4620
	public float hitSoundFadeTime = 0.1f;

	// Token: 0x0400120D RID: 4621
	public ParticleSystem smokeEffect;

	// Token: 0x0400120E RID: 4622
	public Animator chainsawAnimator;

	// Token: 0x0400120F RID: 4623
	public Renderer chainRenderer;

	// Token: 0x04001210 RID: 4624
	public Material chainlink;

	// Token: 0x04001211 RID: 4625
	private MaterialPropertyBlock block;

	// Token: 0x04001212 RID: 4626
	private Vector2 saveST;

	// Token: 0x04001213 RID: 4627
	private float chainSpeed;

	// Token: 0x04001214 RID: 4628
	private float chainAmount;

	// Token: 0x04001215 RID: 4629
	public float temp1;

	// Token: 0x04001216 RID: 4630
	public float temp2;
}
