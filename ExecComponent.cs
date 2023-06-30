using System;
using UnityEngine;

// Token: 0x02000900 RID: 2304
public class ExecComponent : MonoBehaviour
{
	// Token: 0x060037D7 RID: 14295 RVA: 0x0014D91F File Offset: 0x0014BB1F
	public void Run()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, this.ExecToRun, Array.Empty<object>());
	}

	// Token: 0x04003333 RID: 13107
	public string ExecToRun = string.Empty;
}
