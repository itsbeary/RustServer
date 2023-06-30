using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class ChippyMoveTest : MonoBehaviour
{
	// Token: 0x0600173E RID: 5950 RVA: 0x000B0FC8 File Offset: 0x000AF1C8
	private void FixedUpdate()
	{
		float num = ((Mathf.Abs(this.heading.magnitude) > 0f) ? 1f : 0f);
		this.speed = Mathf.MoveTowards(this.speed, this.maxSpeed * num, Time.fixedDeltaTime * ((num == 0f) ? 2f : 2f));
		Ray ray = new Ray(base.transform.position, new Vector3(this.heading.x, this.heading.y, 0f).normalized);
		if (!Physics.Raycast(ray, this.speed * Time.fixedDeltaTime, 16777216))
		{
			base.transform.position += ray.direction * Time.fixedDeltaTime * this.speed;
			if (Mathf.Abs(this.heading.magnitude) > 0f)
			{
				base.transform.rotation = QuaternionEx.LookRotationForcedUp(base.transform.forward, new Vector3(this.heading.x, this.heading.y, 0f).normalized);
			}
		}
	}

	// Token: 0x04000FCC RID: 4044
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	// Token: 0x04000FCD RID: 4045
	public float speed = 0.2f;

	// Token: 0x04000FCE RID: 4046
	public float maxSpeed = 1f;
}
