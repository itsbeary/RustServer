using System;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020008C4 RID: 2244
public class ToggleGroupCookie : MonoBehaviour
{
	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x0600373D RID: 14141 RVA: 0x0014BF7A File Offset: 0x0014A17A
	public ToggleGroup group
	{
		get
		{
			return base.GetComponent<ToggleGroup>();
		}
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x0014BF84 File Offset: 0x0014A184
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("ToggleGroupCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			Transform transform = base.transform.Find(@string);
			if (transform)
			{
				Toggle component = transform.GetComponent<Toggle>();
				if (component)
				{
					Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].isOn = false;
					}
					component.isOn = false;
					component.isOn = true;
					this.SetupListeners();
					return;
				}
			}
		}
		Toggle toggle = this.group.ActiveToggles().FirstOrDefault((Toggle x) => x.isOn);
		if (toggle)
		{
			toggle.isOn = false;
			toggle.isOn = true;
		}
		this.SetupListeners();
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x0014C05C File Offset: 0x0014A25C
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x0014C0A0 File Offset: 0x0014A2A0
	private void SetupListeners()
	{
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x0014C0DC File Offset: 0x0014A2DC
	private void OnToggleChanged(bool b)
	{
		Toggle toggle = base.GetComponentsInChildren<Toggle>().FirstOrDefault((Toggle x) => x.isOn);
		if (toggle)
		{
			PlayerPrefs.SetString("ToggleGroupCookie_" + base.name, toggle.gameObject.name);
		}
	}
}
