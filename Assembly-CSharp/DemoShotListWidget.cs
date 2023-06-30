using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007B8 RID: 1976
public class DemoShotListWidget : SingletonComponent<DemoShotListWidget>
{
	// Token: 0x04002BDD RID: 11229
	public GameObjectRef ShotListEntry;

	// Token: 0x04002BDE RID: 11230
	public GameObjectRef FolderEntry;

	// Token: 0x04002BDF RID: 11231
	public Transform ShotListParent;

	// Token: 0x04002BE0 RID: 11232
	public RustInput FolderNameInput;

	// Token: 0x04002BE1 RID: 11233
	public GameObject ShotsRoot;

	// Token: 0x04002BE2 RID: 11234
	public GameObject NoShotsRoot;

	// Token: 0x04002BE3 RID: 11235
	public GameObject TopUpArrow;

	// Token: 0x04002BE4 RID: 11236
	public GameObject TopDownArrow;

	// Token: 0x04002BE5 RID: 11237
	public Canvas DragCanvas;
}
