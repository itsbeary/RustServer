using System;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class RotateObject : MonoBehaviour
{
	// Token: 0x06001EF1 RID: 7921 RVA: 0x000D2704 File Offset: 0x000D0904
	private void Awake()
	{
		this.rotateVector = new Vector3(this.rotateSpeed_X, this.rotateSpeed_Y, this.rotateSpeed_Z);
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x000D2724 File Offset: 0x000D0924
	private void Update()
	{
		if (this.localSpace)
		{
			base.transform.Rotate(this.rotateVector * Time.deltaTime, Space.Self);
			return;
		}
		if (this.rotateSpeed_X != 0f)
		{
			base.transform.Rotate(Vector3.up, Time.deltaTime * this.rotateSpeed_X);
		}
		if (this.rotateSpeed_Y != 0f)
		{
			base.transform.Rotate(base.transform.forward, Time.deltaTime * this.rotateSpeed_Y);
		}
		if (this.rotateSpeed_Z != 0f)
		{
			base.transform.Rotate(base.transform.right, Time.deltaTime * this.rotateSpeed_Z);
		}
	}

	// Token: 0x040017D9 RID: 6105
	public float rotateSpeed_X = 1f;

	// Token: 0x040017DA RID: 6106
	public float rotateSpeed_Y = 1f;

	// Token: 0x040017DB RID: 6107
	public float rotateSpeed_Z = 1f;

	// Token: 0x040017DC RID: 6108
	public bool localSpace;

	// Token: 0x040017DD RID: 6109
	private Vector3 rotateVector;
}
