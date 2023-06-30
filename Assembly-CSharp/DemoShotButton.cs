using System;
using Rust.UI;
using UnityEngine.EventSystems;

// Token: 0x020007B4 RID: 1972
public class DemoShotButton : RustButton, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06003521 RID: 13601 RVA: 0x0014588A File Offset: 0x00143A8A
	public override void OnPointerDown(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			return;
		}
		base.OnPointerDown(eventData);
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x0014589C File Offset: 0x00143A9C
	public override void OnPointerUp(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			return;
		}
		base.OnPointerUp(eventData);
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x001458AE File Offset: 0x00143AAE
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			base.Press();
		}
	}

	// Token: 0x04002BD1 RID: 11217
	public bool FireEventOnClicked;
}
