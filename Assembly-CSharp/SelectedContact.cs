using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000809 RID: 2057
public class SelectedContact : SingletonComponent<SelectedContact>
{
	// Token: 0x04002E5D RID: 11869
	public RustText nameText;

	// Token: 0x04002E5E RID: 11870
	public RustText seenText;

	// Token: 0x04002E5F RID: 11871
	public RawImage mugshotImage;

	// Token: 0x04002E60 RID: 11872
	public Texture2D unknownMugshot;

	// Token: 0x04002E61 RID: 11873
	public InputField noteInput;

	// Token: 0x04002E62 RID: 11874
	public GameObject[] relationshipTypeTags;

	// Token: 0x04002E63 RID: 11875
	public Translate.Phrase lastSeenPrefix;

	// Token: 0x04002E64 RID: 11876
	public Translate.Phrase nowPhrase;

	// Token: 0x04002E65 RID: 11877
	public Translate.Phrase agoSuffix;

	// Token: 0x04002E66 RID: 11878
	public RustButton FriendlyButton;

	// Token: 0x04002E67 RID: 11879
	public RustButton SeenButton;

	// Token: 0x04002E68 RID: 11880
	public RustButton EnemyButton;

	// Token: 0x04002E69 RID: 11881
	public RustButton chatMute;
}
