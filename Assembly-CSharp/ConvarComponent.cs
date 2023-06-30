using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020008FD RID: 2301
public class ConvarComponent : MonoBehaviour
{
	// Token: 0x060037D1 RID: 14289 RVA: 0x0014D808 File Offset: 0x0014BA08
	protected void OnEnable()
	{
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnEnable();
		}
	}

	// Token: 0x060037D2 RID: 14290 RVA: 0x0014D864 File Offset: 0x0014BA64
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnDisable();
		}
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x0014D8C8 File Offset: 0x0014BAC8
	private bool ShouldRun()
	{
		return this.runOnServer;
	}

	// Token: 0x0400332A RID: 13098
	public bool runOnServer = true;

	// Token: 0x0400332B RID: 13099
	public bool runOnClient = true;

	// Token: 0x0400332C RID: 13100
	public List<ConvarComponent.ConvarEvent> List = new List<ConvarComponent.ConvarEvent>();

	// Token: 0x02000EC2 RID: 3778
	[Serializable]
	public class ConvarEvent
	{
		// Token: 0x0600534B RID: 21323 RVA: 0x001B2028 File Offset: 0x001B0228
		public void OnEnable()
		{
			this.cmd = ConsoleSystem.Index.Client.Find(this.convar);
			if (this.cmd == null)
			{
				this.cmd = ConsoleSystem.Index.Server.Find(this.convar);
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged += this.cmd_OnValueChanged;
			this.cmd_OnValueChanged(this.cmd);
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x001B208C File Offset: 0x001B028C
		private void cmd_OnValueChanged(ConsoleSystem.Command obj)
		{
			if (this.component == null)
			{
				return;
			}
			bool flag = obj.String == this.on;
			if (this.component.enabled == flag)
			{
				return;
			}
			this.component.enabled = flag;
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x001B20D5 File Offset: 0x001B02D5
		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged -= this.cmd_OnValueChanged;
		}

		// Token: 0x04004D4B RID: 19787
		public string convar;

		// Token: 0x04004D4C RID: 19788
		public string on;

		// Token: 0x04004D4D RID: 19789
		public MonoBehaviour component;

		// Token: 0x04004D4E RID: 19790
		internal ConsoleSystem.Command cmd;
	}
}
