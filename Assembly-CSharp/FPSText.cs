using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E8 RID: 2024
public class FPSText : MonoBehaviour
{
	// Token: 0x06003566 RID: 13670 RVA: 0x00145EA4 File Offset: 0x001440A4
	protected void Update()
	{
		if (this.fpsTimer.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		this.text.enabled = true;
		this.fpsTimer.Reset();
		this.fpsTimer.Start();
		string text = Performance.current.frameRate + " FPS";
		this.text.text = text;
	}

	// Token: 0x04002D8A RID: 11658
	public Text text;

	// Token: 0x04002D8B RID: 11659
	private Stopwatch fpsTimer = Stopwatch.StartNew();
}
