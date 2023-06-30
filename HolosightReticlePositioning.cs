using System;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class HolosightReticlePositioning : MonoBehaviour
{
	// Token: 0x1700021F RID: 543
	// (get) Token: 0x0600193A RID: 6458 RVA: 0x000B98F4 File Offset: 0x000B7AF4
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000B9901 File Offset: 0x000B7B01
	private void Update()
	{
		if (MainCamera.isValid)
		{
			this.UpdatePosition(MainCamera.mainCamera);
		}
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000B9918 File Offset: 0x000B7B18
	private void UpdatePosition(Camera cam)
	{
		Vector3 position = this.aimPoint.targetPoint.transform.position;
		Vector2 vector = RectTransformUtility.WorldToScreenPoint(cam, position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform.parent as RectTransform, vector, cam, out vector);
		vector.x /= (this.rectTransform.parent as RectTransform).rect.width * 0.5f;
		vector.y /= (this.rectTransform.parent as RectTransform).rect.height * 0.5f;
		this.rectTransform.anchoredPosition = vector;
	}

	// Token: 0x040011EF RID: 4591
	public IronsightAimPoint aimPoint;
}
