using System;
using System.Collections.Generic;
using Rust.UI;
using UnityEngine;

// Token: 0x020007DB RID: 2011
public class TechTreeDialog : UIDialog, IInventoryChanged
{
	// Token: 0x04002D2C RID: 11564
	public TechTreeData data;

	// Token: 0x04002D2D RID: 11565
	public float graphScale = 1f;

	// Token: 0x04002D2E RID: 11566
	public TechTreeEntry entryPrefab;

	// Token: 0x04002D2F RID: 11567
	public TechTreeGroup groupPrefab;

	// Token: 0x04002D30 RID: 11568
	public TechTreeLine linePrefab;

	// Token: 0x04002D31 RID: 11569
	public RectTransform contents;

	// Token: 0x04002D32 RID: 11570
	public RectTransform contentParent;

	// Token: 0x04002D33 RID: 11571
	public TechTreeSelectedNodeUI selectedNodeUI;

	// Token: 0x04002D34 RID: 11572
	public float nodeSize = 128f;

	// Token: 0x04002D35 RID: 11573
	public float gridSize = 64f;

	// Token: 0x04002D36 RID: 11574
	public GameObjectRef unlockEffect;

	// Token: 0x04002D37 RID: 11575
	public RustText scrapCount;

	// Token: 0x04002D38 RID: 11576
	private Vector2 startPos = Vector2.zero;

	// Token: 0x04002D39 RID: 11577
	public List<int> processed = new List<int>();

	// Token: 0x04002D3A RID: 11578
	public Dictionary<int, TechTreeWidget> widgets = new Dictionary<int, TechTreeWidget>();

	// Token: 0x04002D3B RID: 11579
	public List<TechTreeLine> lines = new List<TechTreeLine>();

	// Token: 0x04002D3C RID: 11580
	public ScrollRectZoom zoom;
}
