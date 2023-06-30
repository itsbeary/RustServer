using System;
using UnityEngine;

// Token: 0x02000996 RID: 2454
public class ExplosionsFPS : MonoBehaviour
{
	// Token: 0x06003A48 RID: 14920 RVA: 0x00158269 File Offset: 0x00156469
	private void Awake()
	{
		this.guiStyleHeader.fontSize = 14;
		this.guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x0015829C File Offset: 0x0015649C
	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 30f, 30f), "FPS: " + (int)this.fps, this.guiStyleHeader);
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x001582D8 File Offset: 0x001564D8
	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = (float)this.frames;
			this.timeleft = 1f;
			this.frames = 0;
		}
	}

	// Token: 0x040034F1 RID: 13553
	private readonly GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x040034F2 RID: 13554
	private float timeleft;

	// Token: 0x040034F3 RID: 13555
	private float fps;

	// Token: 0x040034F4 RID: 13556
	private int frames;
}
