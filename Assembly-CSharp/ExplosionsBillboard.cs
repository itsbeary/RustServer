using System;
using UnityEngine;

// Token: 0x02000997 RID: 2455
public class ExplosionsBillboard : MonoBehaviour
{
	// Token: 0x06003A4C RID: 14924 RVA: 0x0015834C File Offset: 0x0015654C
	private void Awake()
	{
		if (this.AutoInitCamera)
		{
			this.Camera = Camera.main;
			this.Active = true;
		}
		this.t = base.transform;
		Vector3 localScale = this.t.parent.transform.localScale;
		localScale.z = localScale.x;
		this.t.parent.transform.localScale = localScale;
		this.camT = this.Camera.transform;
		Transform parent = this.t.parent;
		this.myContainer = new GameObject
		{
			name = "Billboard_" + this.t.gameObject.name
		};
		this.contT = this.myContainer.transform;
		this.contT.position = this.t.position;
		this.t.parent = this.myContainer.transform;
		this.contT.parent = parent;
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x0015844C File Offset: 0x0015664C
	private void Update()
	{
		if (this.Active)
		{
			this.contT.LookAt(this.contT.position + this.camT.rotation * Vector3.back, this.camT.rotation * Vector3.up);
		}
	}

	// Token: 0x040034F5 RID: 13557
	public Camera Camera;

	// Token: 0x040034F6 RID: 13558
	public bool Active = true;

	// Token: 0x040034F7 RID: 13559
	public bool AutoInitCamera = true;

	// Token: 0x040034F8 RID: 13560
	private GameObject myContainer;

	// Token: 0x040034F9 RID: 13561
	private Transform t;

	// Token: 0x040034FA RID: 13562
	private Transform camT;

	// Token: 0x040034FB RID: 13563
	private Transform contT;
}
