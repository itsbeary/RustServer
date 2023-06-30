using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000871 RID: 2161
public class ConnectionScreen : SingletonComponent<ConnectionScreen>
{
	// Token: 0x040030F1 RID: 12529
	public Text statusText;

	// Token: 0x040030F2 RID: 12530
	public GameObject disconnectButton;

	// Token: 0x040030F3 RID: 12531
	public GameObject retryButton;

	// Token: 0x040030F4 RID: 12532
	public ServerBrowserInfo browserInfo;
}
