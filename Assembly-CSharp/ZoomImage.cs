using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020007E1 RID: 2017
public class ZoomImage : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x06003556 RID: 13654 RVA: 0x001459F7 File Offset: 0x00143BF7
	private void Awake()
	{
		this._thisTransform = base.transform as RectTransform;
		this._scale.Set(this._initialScale, this._initialScale, 1f);
		this._thisTransform.localScale = this._scale;
	}

	// Token: 0x06003557 RID: 13655 RVA: 0x00145A38 File Offset: 0x00143C38
	public void OnScroll(PointerEventData eventData)
	{
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._thisTransform, Input.mousePosition, null, out vector);
		float y = eventData.scrollDelta.y;
		if (y > 0f && this._scale.x < this._maximumScale)
		{
			this._scale.Set(this._scale.x + this._scaleIncrement, this._scale.y + this._scaleIncrement, 1f);
			this._thisTransform.localScale = this._scale;
			this._thisTransform.anchoredPosition -= vector * this._scaleIncrement;
			return;
		}
		if (y < 0f && this._scale.x > this._minimumScale)
		{
			this._scale.Set(this._scale.x - this._scaleIncrement, this._scale.y - this._scaleIncrement, 1f);
			this._thisTransform.localScale = this._scale;
			this._thisTransform.anchoredPosition += vector * this._scaleIncrement;
		}
	}

	// Token: 0x04002D62 RID: 11618
	[SerializeField]
	private float _minimumScale = 0.5f;

	// Token: 0x04002D63 RID: 11619
	[SerializeField]
	private float _initialScale = 1f;

	// Token: 0x04002D64 RID: 11620
	[SerializeField]
	private float _maximumScale = 3f;

	// Token: 0x04002D65 RID: 11621
	[SerializeField]
	private float _scaleIncrement = 0.5f;

	// Token: 0x04002D66 RID: 11622
	[HideInInspector]
	private Vector3 _scale;

	// Token: 0x04002D67 RID: 11623
	private RectTransform _thisTransform;
}
