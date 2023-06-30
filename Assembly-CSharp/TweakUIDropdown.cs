using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200088D RID: 2189
public class TweakUIDropdown : TweakUIBase
{
	// Token: 0x060036AA RID: 13994 RVA: 0x00149868 File Offset: 0x00147A68
	protected override void Init()
	{
		base.Init();
		this.DropdownItemPrefab.SetActive(false);
		this.UpdateDropdownOptions();
		this.Opener.SetToggleFalse();
		this.ResetToConvar();
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x00149824 File Offset: 0x00147A24
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x00149894 File Offset: 0x00147A94
	public void UpdateDropdownOptions()
	{
		List<RustButton> list = Pool.GetList<RustButton>();
		this.DropdownContainer.GetComponentsInChildren<RustButton>(false, list);
		foreach (RustButton rustButton in list)
		{
			UnityEngine.Object.Destroy(rustButton.gameObject);
		}
		Pool.FreeList<RustButton>(ref list);
		for (int i = 0; i < this.nameValues.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.DropdownItemPrefab, this.DropdownContainer);
			int itemIndex = i;
			RustButton component = gameObject.GetComponent<RustButton>();
			component.Text.SetPhrase(this.nameValues[i].label);
			component.OnPressed.AddListener(delegate
			{
				this.ChangeValue(itemIndex);
			});
			gameObject.SetActive(true);
		}
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x00149970 File Offset: 0x00147B70
	public void OnValueChanged()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x060036AE RID: 13998 RVA: 0x00149980 File Offset: 0x00147B80
	public void OnDropdownOpen()
	{
		RectTransform rectTransform = (RectTransform)base.transform;
		if (rectTransform.position.y <= (float)Screen.height / 2f)
		{
			this.Dropdown.pivot = new Vector2(0.5f, 0f);
			this.Dropdown.anchoredPosition = this.Dropdown.anchoredPosition.WithY(0f);
			return;
		}
		this.Dropdown.pivot = new Vector2(0.5f, 1f);
		this.Dropdown.anchoredPosition = this.Dropdown.anchoredPosition.WithY(-rectTransform.rect.height);
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x00149A38 File Offset: 0x00147C38
	public void ChangeValue(int index)
	{
		this.Opener.SetToggleFalse();
		int num = Mathf.Clamp(index, 0, this.nameValues.Length - 1);
		bool flag = num != this.currentValue;
		this.currentValue = num;
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
		else
		{
			this.ShowValue(this.nameValues[this.currentValue].value);
		}
		if (flag)
		{
			UnityEvent unityEvent = this.onValueChanged;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x00149AB0 File Offset: 0x00147CB0
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		TweakUIDropdown.NameValue nameValue = this.nameValues[this.currentValue];
		if (this.conVar == null)
		{
			return;
		}
		if (this.conVar.String == nameValue.value)
		{
			return;
		}
		this.conVar.Set(nameValue.value);
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x00149B04 File Offset: 0x00147D04
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		this.ShowValue(@string);
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x00149B34 File Offset: 0x00147D34
	private void ShowValue(string value)
	{
		int i = 0;
		while (i < this.nameValues.Length)
		{
			if (!(this.nameValues[i].value != value))
			{
				this.Current.SetPhrase(this.nameValues[i].label);
				this.currentValue = i;
				if (this.assignImageColor)
				{
					this.BackgroundImage.color = this.nameValues[i].imageColor;
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04003164 RID: 12644
	public RustText Current;

	// Token: 0x04003165 RID: 12645
	public Image BackgroundImage;

	// Token: 0x04003166 RID: 12646
	public RustButton Opener;

	// Token: 0x04003167 RID: 12647
	public RectTransform Dropdown;

	// Token: 0x04003168 RID: 12648
	public RectTransform DropdownContainer;

	// Token: 0x04003169 RID: 12649
	public GameObject DropdownItemPrefab;

	// Token: 0x0400316A RID: 12650
	public TweakUIDropdown.NameValue[] nameValues;

	// Token: 0x0400316B RID: 12651
	public bool assignImageColor;

	// Token: 0x0400316C RID: 12652
	public UnityEvent onValueChanged = new UnityEvent();

	// Token: 0x0400316D RID: 12653
	public int currentValue;

	// Token: 0x02000EAB RID: 3755
	[Serializable]
	public class NameValue
	{
		// Token: 0x04004CE3 RID: 19683
		public string value;

		// Token: 0x04004CE4 RID: 19684
		public Color imageColor;

		// Token: 0x04004CE5 RID: 19685
		public Translate.Phrase label;
	}
}
