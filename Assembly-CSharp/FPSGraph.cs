using System;
using ConVar;
using UnityEngine;

// Token: 0x020007EB RID: 2027
public class FPSGraph : Graph
{
	// Token: 0x0600356B RID: 13675 RVA: 0x00145F70 File Offset: 0x00144170
	public void Refresh()
	{
		base.enabled = FPS.graph > 0;
		this.Area.width = (float)(this.Resolution = Mathf.Clamp(FPS.graph, 0, Screen.width));
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x00145FB0 File Offset: 0x001441B0
	protected void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x00145FB8 File Offset: 0x001441B8
	protected override float GetValue()
	{
		return 1f / UnityEngine.Time.deltaTime;
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x00145FC5 File Offset: 0x001441C5
	protected override Color GetColor(float value)
	{
		if (value < 10f)
		{
			return Color.red;
		}
		if (value >= 30f)
		{
			return Color.green;
		}
		return Color.yellow;
	}
}
