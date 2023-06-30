using System;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class PingPongRotate : MonoBehaviour
{
	// Token: 0x06001EEE RID: 7918 RVA: 0x000D2634 File Offset: 0x000D0834
	private void Update()
	{
		Quaternion quaternion = Quaternion.identity;
		for (int i = 0; i < 3; i++)
		{
			quaternion *= this.GetRotation(i);
		}
		base.transform.rotation = quaternion;
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x000D2670 File Offset: 0x000D0870
	public Quaternion GetRotation(int index)
	{
		Vector3 vector = Vector3.zero;
		if (index == 0)
		{
			vector = Vector3.right;
		}
		else if (index == 1)
		{
			vector = Vector3.up;
		}
		else if (index == 2)
		{
			vector = Vector3.forward;
		}
		return Quaternion.AngleAxis(Mathf.Sin((this.offset[index] + Time.time) * this.rotationSpeed[index]) * this.rotationAmount[index], vector);
	}

	// Token: 0x040017D6 RID: 6102
	public Vector3 rotationSpeed = Vector3.zero;

	// Token: 0x040017D7 RID: 6103
	public Vector3 offset = Vector3.zero;

	// Token: 0x040017D8 RID: 6104
	public Vector3 rotationAmount = Vector3.zero;
}
