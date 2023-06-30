using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200079C RID: 1948
public class ChatEntry : MonoBehaviour
{
	// Token: 0x04002B86 RID: 11142
	public TextMeshProUGUI text;

	// Token: 0x04002B87 RID: 11143
	public RawImage avatar;

	// Token: 0x04002B88 RID: 11144
	public CanvasGroup canvasGroup;

	// Token: 0x04002B89 RID: 11145
	public float lifeStarted;

	// Token: 0x04002B8A RID: 11146
	public ulong steamid;

	// Token: 0x04002B8B RID: 11147
	public Translate.Phrase LocalPhrase = new Translate.Phrase("local", "local");

	// Token: 0x04002B8C RID: 11148
	public Translate.Phrase CardsPhrase = new Translate.Phrase("cards", "cards");

	// Token: 0x04002B8D RID: 11149
	public Translate.Phrase TeamPhrase = new Translate.Phrase("team", "team");
}
