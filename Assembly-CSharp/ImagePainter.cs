using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020007F4 RID: 2036
public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06003581 RID: 13697 RVA: 0x000B98F4 File Offset: 0x000B7AF4
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x001463E8 File Offset: 0x001445E8
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out vector);
		this.DrawAt(vector, eventData.button);
		this.pointerState[(int)eventData.button].isDown = true;
	}

	// Token: 0x06003583 RID: 13699 RVA: 0x00146439 File Offset: 0x00144639
	public virtual void OnPointerUp(PointerEventData eventData)
	{
		this.pointerState[(int)eventData.button].isDown = false;
	}

	// Token: 0x06003584 RID: 13700 RVA: 0x00146450 File Offset: 0x00144650
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnDrag", eventData);
			}
			return;
		}
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out vector);
		this.DrawAt(vector, eventData.button);
	}

	// Token: 0x06003585 RID: 13701 RVA: 0x001464AC File Offset: 0x001446AC
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnBeginDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x06003586 RID: 13702 RVA: 0x001464D6 File Offset: 0x001446D6
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnEndDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x06003587 RID: 13703 RVA: 0x00146500 File Offset: 0x00144700
	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnInitializePotentialDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x06003588 RID: 13704 RVA: 0x0014652C File Offset: 0x0014472C
	private void DrawAt(Vector2 position, PointerEventData.InputButton button)
	{
		if (this.brush == null)
		{
			return;
		}
		ImagePainter.PointerState pointerState = this.pointerState[(int)button];
		Vector2 vector = this.rectTransform.Unpivot(position);
		if (pointerState.isDown)
		{
			Vector2 vector2 = pointerState.lastPos - vector;
			Vector2 normalized = vector2.normalized;
			for (float num = 0f; num < vector2.magnitude; num += Mathf.Max(this.brush.spacing, 1f) * Mathf.Max(this.spacingScale, 0.1f))
			{
				this.onDrawing.Invoke(vector + num * normalized, this.brush);
			}
			pointerState.lastPos = vector;
			return;
		}
		this.onDrawing.Invoke(vector, this.brush);
		pointerState.lastPos = vector;
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x001465F4 File Offset: 0x001447F4
	public void UpdateBrush(Brush brush)
	{
		this.brush = brush;
	}

	// Token: 0x04002DC8 RID: 11720
	public ImagePainter.OnDrawingEvent onDrawing = new ImagePainter.OnDrawingEvent();

	// Token: 0x04002DC9 RID: 11721
	public MonoBehaviour redirectRightClick;

	// Token: 0x04002DCA RID: 11722
	[Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
	public float spacingScale = 1f;

	// Token: 0x04002DCB RID: 11723
	internal Brush brush;

	// Token: 0x04002DCC RID: 11724
	internal ImagePainter.PointerState[] pointerState = new ImagePainter.PointerState[]
	{
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState()
	};

	// Token: 0x02000E92 RID: 3730
	[Serializable]
	public class OnDrawingEvent : UnityEvent<Vector2, Brush>
	{
	}

	// Token: 0x02000E93 RID: 3731
	internal class PointerState
	{
		// Token: 0x04004C8E RID: 19598
		public Vector2 lastPos;

		// Token: 0x04004C8F RID: 19599
		public bool isDown;
	}
}
