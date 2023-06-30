using System;
using Facepunch.Extend;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000886 RID: 2182
public class ToggleHUDLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x0600368C RID: 13964 RVA: 0x001494F8 File Offset: 0x001476F8
	protected void OnEnable()
	{
		UIHUD instance = SingletonComponent<UIHUD>.Instance;
		if (instance != null)
		{
			Transform transform = instance.transform.FindChildRecursive(this.hudComponentName);
			if (transform != null)
			{
				Canvas component = transform.GetComponent<Canvas>();
				if (component != null)
				{
					this.toggleControl.isOn = component.enabled;
					return;
				}
				this.toggleControl.isOn = transform.gameObject.activeSelf;
				return;
			}
			else
			{
				Debug.LogWarning(base.GetType().Name + ": Couldn't find child: " + this.hudComponentName);
			}
		}
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x00149588 File Offset: 0x00147788
	public void OnToggleChanged()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "global.hudcomponent", new object[]
		{
			this.hudComponentName,
			this.toggleControl.isOn
		});
	}

	// Token: 0x04003156 RID: 12630
	public Toggle toggleControl;

	// Token: 0x04003157 RID: 12631
	public TextMeshProUGUI textControl;

	// Token: 0x04003158 RID: 12632
	public string hudComponentName;
}
