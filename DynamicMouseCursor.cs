using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020007E4 RID: 2020
public class DynamicMouseCursor : MonoBehaviour
{
	// Token: 0x0600355B RID: 13659 RVA: 0x00145BCC File Offset: 0x00143DCC
	private void LateUpdate()
	{
		if (!Cursor.visible)
		{
			return;
		}
		GameObject gameObject = this.CurrentlyHoveredItem();
		if (gameObject != null)
		{
			using (TimeWarning.New("RustControl", 0))
			{
				RustControl componentInParent = gameObject.GetComponentInParent<RustControl>();
				if (componentInParent != null && componentInParent.IsDisabled)
				{
					this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
					return;
				}
			}
			using (TimeWarning.New("ISubmitHandler", 0))
			{
				if (gameObject.GetComponentInParent<ISubmitHandler>() != null)
				{
					this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
					return;
				}
			}
			using (TimeWarning.New("IPointerDownHandler", 0))
			{
				if (gameObject.GetComponentInParent<IPointerDownHandler>() != null)
				{
					this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
					return;
				}
			}
		}
		using (TimeWarning.New("UpdateCursor", 0))
		{
			this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
		}
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x00145D04 File Offset: 0x00143F04
	private void UpdateCursor(Texture2D cursor, Vector2 offs)
	{
		if (this.current == cursor)
		{
			return;
		}
		this.current = cursor;
		Cursor.SetCursor(cursor, offs, CursorMode.Auto);
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x00145D24 File Offset: 0x00143F24
	private GameObject CurrentlyHoveredItem()
	{
		FpStandaloneInputModule fpStandaloneInputModule = EventSystem.current.currentInputModule as FpStandaloneInputModule;
		if (fpStandaloneInputModule == null)
		{
			return null;
		}
		return fpStandaloneInputModule.CurrentData.pointerCurrentRaycast.gameObject;
	}

	// Token: 0x04002D72 RID: 11634
	public Texture2D RegularCursor;

	// Token: 0x04002D73 RID: 11635
	public Vector2 RegularCursorPos;

	// Token: 0x04002D74 RID: 11636
	public Texture2D HoverCursor;

	// Token: 0x04002D75 RID: 11637
	public Vector2 HoverCursorPos;

	// Token: 0x04002D76 RID: 11638
	private Texture2D current;
}
