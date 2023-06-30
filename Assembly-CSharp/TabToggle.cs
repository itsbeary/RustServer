using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BF RID: 2239
public class TabToggle : MonoBehaviour
{
	// Token: 0x0600372D RID: 14125 RVA: 0x0014BC5C File Offset: 0x00149E5C
	public void Awake()
	{
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button c = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (c)
				{
					c.onClick.AddListener(delegate
					{
						this.SwitchTo(c);
					});
				}
			}
		}
	}

	// Token: 0x0600372E RID: 14126 RVA: 0x0014BCDC File Offset: 0x00149EDC
	public void SwitchTo(Button sourceTab)
	{
		string name = sourceTab.transform.name;
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button component = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (component)
				{
					component.interactable = component.name != name;
				}
			}
		}
		if (this.ContentHolder)
		{
			for (int j = 0; j < this.ContentHolder.childCount; j++)
			{
				Transform child = this.ContentHolder.GetChild(j);
				if (child.name == name)
				{
					this.Show(child.gameObject);
				}
				else
				{
					this.Hide(child.gameObject);
				}
			}
		}
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x0014BDA0 File Offset: 0x00149FA0
	private void Hide(GameObject go)
	{
		if (!go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeOut && component)
		{
			LeanTween.alphaCanvas(component, 0f, 0.1f).setOnComplete(delegate
			{
				go.SetActive(false);
			});
			return;
		}
		go.SetActive(false);
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x0014BE14 File Offset: 0x0014A014
	private void Show(GameObject go)
	{
		if (go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeIn && component)
		{
			component.alpha = 0f;
			LeanTween.alphaCanvas(component, 1f, 0.1f);
		}
		go.SetActive(true);
	}

	// Token: 0x04003284 RID: 12932
	public Transform TabHolder;

	// Token: 0x04003285 RID: 12933
	public Transform ContentHolder;

	// Token: 0x04003286 RID: 12934
	public bool FadeIn;

	// Token: 0x04003287 RID: 12935
	public bool FadeOut;
}
