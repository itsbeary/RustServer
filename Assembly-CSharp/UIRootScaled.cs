using System;
using ConVar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D8 RID: 2264
public class UIRootScaled : UIRoot
{
	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06003777 RID: 14199 RVA: 0x0014CB1C File Offset: 0x0014AD1C
	public static Canvas DragOverlayCanvas
	{
		get
		{
			return UIRootScaled.Instance.overlayCanvas;
		}
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x0014CB28 File Offset: 0x0014AD28
	protected override void Awake()
	{
		UIRootScaled.Instance = this;
		base.Awake();
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x0014CB38 File Offset: 0x0014AD38
	protected override void Refresh()
	{
		Vector2 vector = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.OverrideReference)
		{
			vector = new Vector2(this.TargetReference.x / ConVar.Graphics.uiscale, this.TargetReference.y / ConVar.Graphics.uiscale);
		}
		if (this.scaler.referenceResolution != vector)
		{
			this.scaler.referenceResolution = vector;
		}
	}

	// Token: 0x040032DF RID: 13023
	private static UIRootScaled Instance;

	// Token: 0x040032E0 RID: 13024
	public bool OverrideReference;

	// Token: 0x040032E1 RID: 13025
	public Vector2 TargetReference = new Vector2(1280f, 720f);

	// Token: 0x040032E2 RID: 13026
	public CanvasScaler scaler;
}
