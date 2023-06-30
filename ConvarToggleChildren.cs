using System;
using UnityEngine;

// Token: 0x020007A4 RID: 1956
public class ConvarToggleChildren : MonoBehaviour
{
	// Token: 0x060034FE RID: 13566 RVA: 0x00145624 File Offset: 0x00143824
	protected void Awake()
	{
		this.Command = ConsoleSystem.Index.Client.Find(this.ConvarName);
		if (this.Command == null)
		{
			this.Command = ConsoleSystem.Index.Server.Find(this.ConvarName);
		}
		if (this.Command != null)
		{
			this.SetState(this.Command.String == this.ConvarEnabled);
		}
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x00145680 File Offset: 0x00143880
	protected void Update()
	{
		if (this.Command != null)
		{
			bool flag = this.Command.String == this.ConvarEnabled;
			if (this.state != flag)
			{
				this.SetState(flag);
			}
		}
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x001456BC File Offset: 0x001438BC
	private void SetState(bool newState)
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(newState);
		}
		this.state = newState;
	}

	// Token: 0x04002BAB RID: 11179
	public string ConvarName;

	// Token: 0x04002BAC RID: 11180
	public string ConvarEnabled = "True";

	// Token: 0x04002BAD RID: 11181
	private bool state;

	// Token: 0x04002BAE RID: 11182
	private ConsoleSystem.Command Command;
}
