using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class JiggleBone : BaseMonoBehaviour
{
	// Token: 0x06001767 RID: 5991 RVA: 0x000B1AFC File Offset: 0x000AFCFC
	private void Awake()
	{
		Vector3 vector = base.transform.position + base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		this.dynamicPos = vector;
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x000B1B68 File Offset: 0x000AFD68
	private void LateUpdate()
	{
		base.transform.rotation = default(Quaternion);
		Vector3 vector = base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		Vector3 vector2 = base.transform.TransformDirection(new Vector3(0f, 1f, 0f));
		Vector3 vector3 = base.transform.position + base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		this.force.x = (vector3.x - this.dynamicPos.x) * this.bStiffness;
		this.acc.x = this.force.x / this.bMass;
		this.vel.x = this.vel.x + this.acc.x * (1f - this.bDamping);
		this.force.y = (vector3.y - this.dynamicPos.y) * this.bStiffness;
		this.force.y = this.force.y - this.bGravity / 10f;
		this.acc.y = this.force.y / this.bMass;
		this.vel.y = this.vel.y + this.acc.y * (1f - this.bDamping);
		this.force.z = (vector3.z - this.dynamicPos.z) * this.bStiffness;
		this.acc.z = this.force.z / this.bMass;
		this.vel.z = this.vel.z + this.acc.z * (1f - this.bDamping);
		this.dynamicPos += this.vel + this.force;
		base.transform.LookAt(this.dynamicPos, vector2);
		if (this.SquashAndStretch)
		{
			float magnitude = (this.dynamicPos - vector3).magnitude;
			float num;
			if (this.boneAxis.x == 0f)
			{
				num = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				num = 1f + magnitude * this.frontStretch;
			}
			float num2;
			if (this.boneAxis.y == 0f)
			{
				num2 = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				num2 = 1f + magnitude * this.frontStretch;
			}
			float num3;
			if (this.boneAxis.z == 0f)
			{
				num3 = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				num3 = 1f + magnitude * this.frontStretch;
			}
			base.transform.localScale = new Vector3(num, num2, num3);
		}
		if (this.debugMode)
		{
			Debug.DrawRay(base.transform.position, vector, Color.blue);
			Debug.DrawRay(base.transform.position, vector2, Color.green);
			Debug.DrawRay(vector3, Vector3.up * 0.2f, Color.yellow);
			Debug.DrawRay(this.dynamicPos, Vector3.up * 0.2f, Color.red);
		}
	}

	// Token: 0x04000FF8 RID: 4088
	public bool debugMode = true;

	// Token: 0x04000FF9 RID: 4089
	private Vector3 targetPos;

	// Token: 0x04000FFA RID: 4090
	private Vector3 dynamicPos;

	// Token: 0x04000FFB RID: 4091
	public Vector3 boneAxis = new Vector3(0f, 0f, 1f);

	// Token: 0x04000FFC RID: 4092
	public float targetDistance = 2f;

	// Token: 0x04000FFD RID: 4093
	public float bStiffness = 0.1f;

	// Token: 0x04000FFE RID: 4094
	public float bMass = 0.9f;

	// Token: 0x04000FFF RID: 4095
	public float bDamping = 0.75f;

	// Token: 0x04001000 RID: 4096
	public float bGravity = 0.75f;

	// Token: 0x04001001 RID: 4097
	private Vector3 force;

	// Token: 0x04001002 RID: 4098
	private Vector3 acc;

	// Token: 0x04001003 RID: 4099
	private Vector3 vel;

	// Token: 0x04001004 RID: 4100
	public bool SquashAndStretch = true;

	// Token: 0x04001005 RID: 4101
	public float sideStretch = 0.15f;

	// Token: 0x04001006 RID: 4102
	public float frontStretch = 0.2f;

	// Token: 0x04001007 RID: 4103
	public float disableDistance = 20f;
}
