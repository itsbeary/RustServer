using System;
using UnityEngine;

namespace Rust.UI.ServerAdmin
{
	// Token: 0x02000B28 RID: 2856
	public class ServerAdminUGCEntryStub : MonoBehaviour
	{
		// Token: 0x04003DF1 RID: 15857
		public ServerAdminUGCEntryAudio AudioWidget;

		// Token: 0x04003DF2 RID: 15858
		public ServerAdminUGCEntryImage ImageWidget;

		// Token: 0x04003DF3 RID: 15859
		public ServerAdminUGCEntryPattern PatternWidget;

		// Token: 0x04003DF4 RID: 15860
		public RustText PrefabName;

		// Token: 0x04003DF5 RID: 15861
		public RustButton HistoryButton;

		// Token: 0x04003DF6 RID: 15862
		public ServerAdminPlayerId[] HistoryIds = new ServerAdminPlayerId[0];
	}
}
