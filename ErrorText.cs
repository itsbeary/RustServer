using System;
using System.Diagnostics;
using Facepunch;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x020007E6 RID: 2022
public class ErrorText : MonoBehaviour
{
	// Token: 0x06003560 RID: 13664 RVA: 0x00145D58 File Offset: 0x00143F58
	public void OnEnable()
	{
		Output.OnMessage += this.CaptureLog;
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x00145D6B File Offset: 0x00143F6B
	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Output.OnMessage -= this.CaptureLog;
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x00145D88 File Offset: 0x00143F88
	internal void CaptureLog(string error, string stacktrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (this.text == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = this.text;
		textMeshProUGUI.text = string.Concat(new string[] { textMeshProUGUI.text, error, "\n", stacktrace, "\n\n" });
		if (this.text.text.Length > this.maxLength)
		{
			this.text.text = this.text.text.Substring(this.text.text.Length - this.maxLength, this.maxLength);
		}
		this.stopwatch = Stopwatch.StartNew();
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x00145E44 File Offset: 0x00144044
	protected void Update()
	{
		if (this.stopwatch != null && this.stopwatch.Elapsed.TotalSeconds > 30.0)
		{
			this.text.text = string.Empty;
			this.stopwatch = null;
		}
	}

	// Token: 0x04002D77 RID: 11639
	public TextMeshProUGUI text;

	// Token: 0x04002D78 RID: 11640
	public int maxLength = 1024;

	// Token: 0x04002D79 RID: 11641
	private Stopwatch stopwatch;
}
