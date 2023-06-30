using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200077F RID: 1919
public class EnumListUI : MonoBehaviour
{
	// Token: 0x060034D1 RID: 13521 RVA: 0x00145428 File Offset: 0x00143628
	private void Awake()
	{
		this.Hide();
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x00145430 File Offset: 0x00143630
	public void Show(List<object> values, Action<object> clicked)
	{
		base.gameObject.SetActive(true);
		this.clickedAction = clicked;
		foreach (object obj in this.Container)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (object obj2 in values)
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this.PrefabItem);
			transform.SetParent(this.Container, false);
			transform.GetComponent<EnumListItemUI>().Init(obj2, obj2.ToString(), this);
		}
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x00145500 File Offset: 0x00143700
	public void ItemClicked(object value)
	{
		Action<object> action = this.clickedAction;
		if (action != null)
		{
			action(value);
		}
		this.Hide();
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x0014551A File Offset: 0x0014371A
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04002B5F RID: 11103
	public Transform PrefabItem;

	// Token: 0x04002B60 RID: 11104
	public Transform Container;

	// Token: 0x04002B61 RID: 11105
	private Action<object> clickedAction;

	// Token: 0x04002B62 RID: 11106
	private CanvasScaler canvasScaler;
}
