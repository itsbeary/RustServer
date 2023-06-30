using System;
using UnityEngine;

// Token: 0x02000995 RID: 2453
public class ExplosionPlatformActivator : MonoBehaviour
{
	// Token: 0x06003A42 RID: 14914 RVA: 0x00158193 File Offset: 0x00156393
	private void Start()
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
		base.Invoke("Init", this.TimeDelay);
	}

	// Token: 0x06003A43 RID: 14915 RVA: 0x001581B2 File Offset: 0x001563B2
	private void Init()
	{
		this.canUpdate = true;
		this.Effect.SetActive(true);
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x001581C8 File Offset: 0x001563C8
	private void Update()
	{
		if (!this.canUpdate || this.Effect == null)
		{
			return;
		}
		this.currentTime += Time.deltaTime;
		if (this.currentTime > this.currentRepeatTime)
		{
			this.currentTime = 0f;
			this.Effect.SetActive(false);
			this.Effect.SetActive(true);
		}
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x0015822F File Offset: 0x0015642F
	private void OnTriggerEnter(Collider coll)
	{
		this.currentRepeatTime = this.NearRepeatTime;
	}

	// Token: 0x06003A46 RID: 14918 RVA: 0x0015823D File Offset: 0x0015643D
	private void OnTriggerExit(Collider other)
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
	}

	// Token: 0x040034EA RID: 13546
	public GameObject Effect;

	// Token: 0x040034EB RID: 13547
	public float TimeDelay;

	// Token: 0x040034EC RID: 13548
	public float DefaultRepeatTime = 5f;

	// Token: 0x040034ED RID: 13549
	public float NearRepeatTime = 3f;

	// Token: 0x040034EE RID: 13550
	private float currentTime;

	// Token: 0x040034EF RID: 13551
	private float currentRepeatTime;

	// Token: 0x040034F0 RID: 13552
	private bool canUpdate;
}
