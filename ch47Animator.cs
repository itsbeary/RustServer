using System;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class ch47Animator : MonoBehaviour
{
	// Token: 0x060018A1 RID: 6305 RVA: 0x000B7CC0 File Offset: 0x000B5EC0
	private void Start()
	{
		this.EnableBlurredRotorBlades(false);
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x000B7CDA File Offset: 0x000B5EDA
	public void SetDropDoorOpen(bool isOpen)
	{
		this.bottomDoorOpen = isOpen;
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x000B7CE4 File Offset: 0x000B5EE4
	private void Update()
	{
		this.animator.SetBool("bottomdoor", this.bottomDoorOpen);
		this.animator.SetBool("landinggear", this.landingGearDown);
		this.animator.SetBool("leftdoor", this.leftDoorOpen);
		this.animator.SetBool("rightdoor", this.rightDoorOpen);
		this.animator.SetBool("reardoor", this.rearDoorOpen);
		this.animator.SetBool("reardoor_extension", this.rearDoorExtensionOpen);
		if (this.rotorBladeSpeed >= this.blurSpeedThreshold && !this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(true);
		}
		else if (this.rotorBladeSpeed < this.blurSpeedThreshold && this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(false);
		}
		if (this.rotorBladeSpeed <= 0f)
		{
			this.animator.SetBool("rotorblade_stop", true);
			return;
		}
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x000B7DE4 File Offset: 0x000B5FE4
	private void LateUpdate()
	{
		float num = Time.deltaTime * this.rotorBladeSpeed * 15f;
		Vector3 vector = this.frontRotorBlade.localEulerAngles;
		this.frontRotorBlade.localEulerAngles = new Vector3(vector.x, vector.y + num, vector.z);
		vector = this.rearRotorBlade.localEulerAngles;
		this.rearRotorBlade.localEulerAngles = new Vector3(vector.x, vector.y - num, vector.z);
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x000B7E64 File Offset: 0x000B6064
	private void EnableBlurredRotorBlades(bool enabled)
	{
		this.blurredRotorBladesEnabled = enabled;
		SkinnedMeshRenderer[] rotorBlades = this.blurredRotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = enabled;
		}
		rotorBlades = this.RotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = !enabled;
		}
	}

	// Token: 0x04001147 RID: 4423
	public Animator animator;

	// Token: 0x04001148 RID: 4424
	public bool bottomDoorOpen;

	// Token: 0x04001149 RID: 4425
	public bool landingGearDown;

	// Token: 0x0400114A RID: 4426
	public bool leftDoorOpen;

	// Token: 0x0400114B RID: 4427
	public bool rightDoorOpen;

	// Token: 0x0400114C RID: 4428
	public bool rearDoorOpen;

	// Token: 0x0400114D RID: 4429
	public bool rearDoorExtensionOpen;

	// Token: 0x0400114E RID: 4430
	public Transform rearRotorBlade;

	// Token: 0x0400114F RID: 4431
	public Transform frontRotorBlade;

	// Token: 0x04001150 RID: 4432
	public float rotorBladeSpeed;

	// Token: 0x04001151 RID: 4433
	public float wheelTurnSpeed;

	// Token: 0x04001152 RID: 4434
	public float wheelTurnAngle;

	// Token: 0x04001153 RID: 4435
	public SkinnedMeshRenderer[] blurredRotorBlades;

	// Token: 0x04001154 RID: 4436
	public SkinnedMeshRenderer[] RotorBlades;

	// Token: 0x04001155 RID: 4437
	private bool blurredRotorBladesEnabled;

	// Token: 0x04001156 RID: 4438
	public float blurSpeedThreshold = 100f;
}
