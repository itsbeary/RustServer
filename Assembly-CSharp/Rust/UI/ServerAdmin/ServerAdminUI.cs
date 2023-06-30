using System;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.ServerAdmin
{
	// Token: 0x02000B29 RID: 2857
	public class ServerAdminUI : SingletonComponent<ServerAdminUI>
	{
		// Token: 0x04003DF7 RID: 15863
		public GameObjectRef PlayerEntry;

		// Token: 0x04003DF8 RID: 15864
		public RectTransform PlayerInfoParent;

		// Token: 0x04003DF9 RID: 15865
		public RustText PlayerCount;

		// Token: 0x04003DFA RID: 15866
		public RustInput PlayerNameFilter;

		// Token: 0x04003DFB RID: 15867
		public GameObjectRef ServerInfoEntry;

		// Token: 0x04003DFC RID: 15868
		public RectTransform ServerInfoParent;

		// Token: 0x04003DFD RID: 15869
		public GameObjectRef ConvarInfoEntry;

		// Token: 0x04003DFE RID: 15870
		public GameObjectRef ConvarInfoLongEntry;

		// Token: 0x04003DFF RID: 15871
		public RectTransform ConvarInfoParent;

		// Token: 0x04003E00 RID: 15872
		public ServerAdminPlayerInfo PlayerInfo;

		// Token: 0x04003E01 RID: 15873
		public RustInput UgcNameFilter;

		// Token: 0x04003E02 RID: 15874
		public GameObjectRef ImageEntry;

		// Token: 0x04003E03 RID: 15875
		public GameObjectRef PatternEntry;

		// Token: 0x04003E04 RID: 15876
		public GameObjectRef SoundEntry;

		// Token: 0x04003E05 RID: 15877
		public VirtualScroll UgcVirtualScroll;

		// Token: 0x04003E06 RID: 15878
		public GameObject ExpandedUgcRoot;

		// Token: 0x04003E07 RID: 15879
		public RawImage ExpandedImage;

		// Token: 0x04003E08 RID: 15880
		public RectTransform ExpandedImageBacking;
	}
}
