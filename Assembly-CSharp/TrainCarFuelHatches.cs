using System;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class TrainCarFuelHatches : MonoBehaviour
{
	// Token: 0x0600278B RID: 10123 RVA: 0x000F7218 File Offset: 0x000F5418
	public void LinedUpStateChanged(bool linedUp)
	{
		this.openingQueued = linedUp;
		if (!this.isMoving)
		{
			this.opening = linedUp;
			bool flag = this.opening;
			this.isMoving = true;
			InvokeHandler.InvokeRepeating(this, new Action(this.MoveTick), 0f, 0f);
		}
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000F7268 File Offset: 0x000F5468
	private void MoveTick()
	{
		if (this.opening)
		{
			this._hatchLerp += Time.deltaTime * this.animSpeed;
			if (this._hatchLerp >= 1f)
			{
				this.EndMove();
				return;
			}
			this.SetAngleOnAll(this._hatchLerp, false);
			return;
		}
		else
		{
			this._hatchLerp += Time.deltaTime * this.animSpeed;
			if (this._hatchLerp >= 1f)
			{
				this.EndMove();
				return;
			}
			this.SetAngleOnAll(this._hatchLerp, true);
			return;
		}
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000F72F4 File Offset: 0x000F54F4
	private void EndMove()
	{
		this._hatchLerp = 0f;
		if (this.openingQueued == this.opening)
		{
			InvokeHandler.CancelInvoke(this, new Action(this.MoveTick));
			this.isMoving = false;
			return;
		}
		this.opening = this.openingQueued;
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000F7340 File Offset: 0x000F5540
	private void SetAngleOnAll(float lerpT, bool closing)
	{
		float num;
		float num2;
		float num3;
		if (closing)
		{
			num = LeanTween.easeOutBounce(-145f, 0f, Mathf.Clamp01(this._hatchLerp * 1.15f));
			num2 = LeanTween.easeOutBounce(-145f, 0f, this._hatchLerp);
			num3 = LeanTween.easeOutBounce(-145f, 0f, Mathf.Clamp01(this._hatchLerp * 1.25f));
		}
		else
		{
			num = LeanTween.easeOutBounce(0f, -145f, Mathf.Clamp01(this._hatchLerp * 1.15f));
			num2 = LeanTween.easeOutBounce(0f, -145f, this._hatchLerp);
			num3 = LeanTween.easeOutBounce(0f, -145f, Mathf.Clamp01(this._hatchLerp * 1.25f));
		}
		this.SetAngle(this.hatch1Col, num);
		this.SetAngle(this.hatch2Col, num2);
		this.SetAngle(this.hatch3Col, num3);
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x000F7429 File Offset: 0x000F5629
	private void SetAngle(Transform transform, float angle)
	{
		this._angles.x = angle;
		transform.localEulerAngles = this._angles;
	}

	// Token: 0x0400201D RID: 8221
	[SerializeField]
	private TrainCar owner;

	// Token: 0x0400201E RID: 8222
	[SerializeField]
	private float animSpeed = 1f;

	// Token: 0x0400201F RID: 8223
	[SerializeField]
	private Transform hatch1Col;

	// Token: 0x04002020 RID: 8224
	[SerializeField]
	private Transform hatch1Vis;

	// Token: 0x04002021 RID: 8225
	[SerializeField]
	private Transform hatch2Col;

	// Token: 0x04002022 RID: 8226
	[SerializeField]
	private Transform hatch2Vis;

	// Token: 0x04002023 RID: 8227
	[SerializeField]
	private Transform hatch3Col;

	// Token: 0x04002024 RID: 8228
	[SerializeField]
	private Transform hatch3Vis;

	// Token: 0x04002025 RID: 8229
	private const float closedXAngle = 0f;

	// Token: 0x04002026 RID: 8230
	private const float openXAngle = -145f;

	// Token: 0x04002027 RID: 8231
	[SerializeField]
	private SoundDefinition hatchOpenSoundDef;

	// Token: 0x04002028 RID: 8232
	[SerializeField]
	private SoundDefinition hatchCloseSoundDef;

	// Token: 0x04002029 RID: 8233
	private Vector3 _angles = Vector3.zero;

	// Token: 0x0400202A RID: 8234
	private float _hatchLerp;

	// Token: 0x0400202B RID: 8235
	private bool opening;

	// Token: 0x0400202C RID: 8236
	private bool openingQueued;

	// Token: 0x0400202D RID: 8237
	private bool isMoving;
}
