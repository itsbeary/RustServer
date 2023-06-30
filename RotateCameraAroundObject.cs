using System;
using UnityEngine;

// Token: 0x0200091A RID: 2330
public class RotateCameraAroundObject : MonoBehaviour
{
	// Token: 0x06003826 RID: 14374 RVA: 0x0014E408 File Offset: 0x0014C608
	private void FixedUpdate()
	{
		if (this.m_goObjectToRotateAround != null)
		{
			base.transform.LookAt(this.m_goObjectToRotateAround.transform.position + Vector3.up * 0.75f);
			base.transform.Translate(Vector3.right * this.m_flRotateSpeed * Time.deltaTime);
		}
	}

	// Token: 0x04003392 RID: 13202
	public GameObject m_goObjectToRotateAround;

	// Token: 0x04003393 RID: 13203
	public float m_flRotateSpeed = 10f;
}
