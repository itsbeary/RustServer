using System;
using UnityEngine;

// Token: 0x0200091B RID: 2331
public class RunConsoleCommand : MonoBehaviour
{
	// Token: 0x06003828 RID: 14376 RVA: 0x0014E48A File Offset: 0x0014C68A
	public void ClientRun(string command)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, command, Array.Empty<object>());
	}
}
