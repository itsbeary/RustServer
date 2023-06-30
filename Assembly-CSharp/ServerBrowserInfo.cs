using System;
using Rust.UI;
using UnityEngine.UI;

// Token: 0x02000893 RID: 2195
public class ServerBrowserInfo : SingletonComponent<ServerBrowserInfo>
{
	// Token: 0x04003179 RID: 12665
	public bool isMain;

	// Token: 0x0400317A RID: 12666
	public Text serverName;

	// Token: 0x0400317B RID: 12667
	public Text serverMeta;

	// Token: 0x0400317C RID: 12668
	public Text serverText;

	// Token: 0x0400317D RID: 12669
	public Button viewWebpage;

	// Token: 0x0400317E RID: 12670
	public Button refresh;

	// Token: 0x0400317F RID: 12671
	public ServerInfo? currentServer;

	// Token: 0x04003180 RID: 12672
	public HttpImage headerImage;

	// Token: 0x04003181 RID: 12673
	public HttpImage logoImage;
}
