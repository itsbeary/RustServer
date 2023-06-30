using System;
using UnityEngine;

// Token: 0x02000626 RID: 1574
public class MoveOverTime : MonoBehaviour
{
	// Token: 0x06002E7D RID: 11901 RVA: 0x001171B4 File Offset: 0x001153B4
	private void Update()
	{
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles + this.rotation * this.speed * Time.deltaTime);
		base.transform.localScale += this.scale * this.speed * Time.deltaTime;
		base.transform.localPosition += this.position * this.speed * Time.deltaTime;
	}

	// Token: 0x04002629 RID: 9769
	[Range(-10f, 10f)]
	public float speed = 1f;

	// Token: 0x0400262A RID: 9770
	public Vector3 position;

	// Token: 0x0400262B RID: 9771
	public Vector3 rotation;

	// Token: 0x0400262C RID: 9772
	public Vector3 scale;
}
