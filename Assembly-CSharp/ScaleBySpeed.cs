using System;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class ScaleBySpeed : MonoBehaviour
{
	// Token: 0x06001E33 RID: 7731 RVA: 0x000CDF37 File Offset: 0x000CC137
	private void Start()
	{
		this.prevPosition = base.transform.position;
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x000CDF4C File Offset: 0x000CC14C
	private void Update()
	{
		Vector3 position = base.transform.position;
		float num = (position - this.prevPosition).sqrMagnitude;
		float num2 = this.minScale;
		bool flag = WaterSystem.GetHeight(position) > position.y - this.submergedThickness;
		if (num > 0.0001f)
		{
			num = Mathf.Sqrt(num) / Time.deltaTime;
			float num3 = Mathf.Clamp(num, this.minSpeed, this.maxSpeed) / (this.maxSpeed - this.minSpeed);
			num2 = Mathf.Lerp(this.minScale, this.maxScale, Mathf.Clamp01(num3));
			if (this.component != null && this.toggleComponent)
			{
				this.component.enabled = flag;
			}
		}
		else if (this.component != null && this.toggleComponent)
		{
			this.component.enabled = false;
		}
		base.transform.localScale = new Vector3(num2, num2, num2);
		this.prevPosition = position;
	}

	// Token: 0x0400175A RID: 5978
	public float minScale = 0.001f;

	// Token: 0x0400175B RID: 5979
	public float maxScale = 1f;

	// Token: 0x0400175C RID: 5980
	public float minSpeed;

	// Token: 0x0400175D RID: 5981
	public float maxSpeed = 1f;

	// Token: 0x0400175E RID: 5982
	public MonoBehaviour component;

	// Token: 0x0400175F RID: 5983
	public bool toggleComponent = true;

	// Token: 0x04001760 RID: 5984
	public bool onlyWhenSubmerged;

	// Token: 0x04001761 RID: 5985
	public float submergedThickness = 0.33f;

	// Token: 0x04001762 RID: 5986
	private Vector3 prevPosition = Vector3.zero;
}
