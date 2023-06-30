using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008C5 RID: 2245
public class ToolsHUDUI : MonoBehaviour
{
	// Token: 0x06003743 RID: 14147 RVA: 0x0014C13C File Offset: 0x0014A33C
	protected void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x0014C144 File Offset: 0x0014A344
	private void Init()
	{
		if (this.initialised)
		{
			return;
		}
		UIHUD instance = SingletonComponent<UIHUD>.Instance;
		if (instance == null)
		{
			return;
		}
		this.initialised = true;
		foreach (Transform transform in instance.GetComponentsInChildren<Transform>())
		{
			string name = transform.name;
			if (name.ToLower().StartsWith("gameui.hud."))
			{
				if (name.ToLower() == "gameui.hud.crosshair")
				{
					foreach (object obj in transform)
					{
						Transform transform2 = (Transform)obj;
						this.AddToggleObj(transform2.name, "<color=yellow>Crosshair sub:</color> " + transform2.name);
					}
				}
				this.AddToggleObj(name, name.Substring(11));
			}
		}
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x0014C238 File Offset: 0x0014A438
	private void AddToggleObj(string trName, string labelText)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, Vector3.zero, Quaternion.identity, this.parent);
		gameObject.name = trName;
		ToggleHUDLayer component = gameObject.GetComponent<ToggleHUDLayer>();
		component.hudComponentName = trName;
		component.textControl.text = labelText;
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x0014C274 File Offset: 0x0014A474
	public void SelectAll()
	{
		Toggle[] componentsInChildren = this.parent.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isOn = true;
		}
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x0014C2A4 File Offset: 0x0014A4A4
	public void SelectNone()
	{
		Toggle[] componentsInChildren = this.parent.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isOn = false;
		}
	}

	// Token: 0x040032A1 RID: 12961
	[SerializeField]
	private GameObject prefab;

	// Token: 0x040032A2 RID: 12962
	[SerializeField]
	private Transform parent;

	// Token: 0x040032A3 RID: 12963
	private bool initialised;
}
