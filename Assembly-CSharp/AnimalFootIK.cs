using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class AnimalFootIK : MonoBehaviour
{
	// Token: 0x06001D46 RID: 7494 RVA: 0x000CA1A4 File Offset: 0x000C83A4
	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		return Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, out hit, 1f, 455155969);
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Start()
	{
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x000CA1D5 File Offset: 0x000C83D5
	public AvatarIKGoal GoalFromIndex(int index)
	{
		if (index == 0)
		{
			return AvatarIKGoal.LeftHand;
		}
		if (index == 1)
		{
			return AvatarIKGoal.RightHand;
		}
		if (index == 2)
		{
			return AvatarIKGoal.LeftFoot;
		}
		if (index == 3)
		{
			return AvatarIKGoal.RightFoot;
		}
		return AvatarIKGoal.LeftHand;
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x000CA1F0 File Offset: 0x000C83F0
	private void OnAnimatorIK(int layerIndex)
	{
		Debug.Log("animal ik!");
		for (int i = 0; i < 4; i++)
		{
			Transform transform = this.Feet[i];
			AvatarIKGoal avatarIKGoal = this.GoalFromIndex(i);
			Vector3 up = Vector3.up;
			Vector3 vector = transform.transform.position;
			float num = this.animator.GetIKPositionWeight(avatarIKGoal);
			RaycastHit raycastHit;
			if (this.GroundSample(transform.transform.position - Vector3.down * this.actualFootOffset, out raycastHit))
			{
				Vector3 normal = raycastHit.normal;
				vector = raycastHit.point;
				float num2 = Vector3.Distance(transform.transform.position - Vector3.down * this.actualFootOffset, vector);
				num = 1f - Mathf.InverseLerp(this.minWeightDistance, this.maxWeightDistance, num2);
				this.animator.SetIKPosition(avatarIKGoal, vector + Vector3.up * this.actualFootOffset);
			}
			else
			{
				num = 0f;
			}
			this.animator.SetIKPositionWeight(avatarIKGoal, num);
		}
	}

	// Token: 0x040015D9 RID: 5593
	public Transform[] Feet;

	// Token: 0x040015DA RID: 5594
	public Animator animator;

	// Token: 0x040015DB RID: 5595
	public float maxWeightDistance = 0.1f;

	// Token: 0x040015DC RID: 5596
	public float minWeightDistance = 0.025f;

	// Token: 0x040015DD RID: 5597
	public float actualFootOffset = 0.01f;
}
