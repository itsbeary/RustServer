using System;
using UnityEngine;

// Token: 0x0200090C RID: 2316
public class MoveForward : MonoBehaviour
{
	// Token: 0x060037FA RID: 14330 RVA: 0x0014DF19 File Offset: 0x0014C119
	protected void Update()
	{
		base.GetComponent<Rigidbody>().velocity = this.Speed * base.transform.forward;
	}

	// Token: 0x04003356 RID: 13142
	public float Speed = 2f;
}
