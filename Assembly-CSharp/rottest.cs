using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class rottest : MonoBehaviour
{
	// Token: 0x060018A7 RID: 6311 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x000B7ECA File Offset: 0x000B60CA
	private void Update()
	{
		this.aimDir = new Vector3(0f, 45f * Mathf.Sin(Time.time * 6f), 0f);
		this.UpdateAiming();
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x000B7F00 File Offset: 0x000B6100
	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(0f, this.aimDir.y, 0f);
		if (base.transform.localRotation != quaternion)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * 8f);
		}
	}

	// Token: 0x04001157 RID: 4439
	public Transform turretBase;

	// Token: 0x04001158 RID: 4440
	public Vector3 aimDir;
}
