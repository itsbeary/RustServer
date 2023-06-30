using System;
using UnityEngine;

// Token: 0x020001D5 RID: 469
public class ToolgunBeam : MonoBehaviour
{
	// Token: 0x06001953 RID: 6483 RVA: 0x000B9FA8 File Offset: 0x000B81A8
	public void Update()
	{
		if (this.fadeColor.a <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.electricalBeam.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(Time.time * this.scrollSpeed, 0f));
		this.fadeColor.a = this.fadeColor.a - Time.deltaTime * this.fadeSpeed;
		this.electricalBeam.startColor = this.fadeColor;
		this.electricalBeam.endColor = this.fadeColor;
	}

	// Token: 0x04001225 RID: 4645
	public LineRenderer electricalBeam;

	// Token: 0x04001226 RID: 4646
	public float scrollSpeed = -8f;

	// Token: 0x04001227 RID: 4647
	private Color fadeColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04001228 RID: 4648
	public float fadeSpeed = 4f;
}
