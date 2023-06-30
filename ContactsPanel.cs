using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F2 RID: 2034
public class ContactsPanel : SingletonComponent<ContactsPanel>
{
	// Token: 0x04002DB6 RID: 11702
	public RectTransform alliesBucket;

	// Token: 0x04002DB7 RID: 11703
	public RectTransform seenBucket;

	// Token: 0x04002DB8 RID: 11704
	public RectTransform enemiesBucket;

	// Token: 0x04002DB9 RID: 11705
	public RectTransform contentsBucket;

	// Token: 0x04002DBA RID: 11706
	public ContactsEntry contactEntryPrefab;

	// Token: 0x04002DBB RID: 11707
	public RawImage mugshotTest;

	// Token: 0x04002DBC RID: 11708
	public RawImage fullBodyTest;

	// Token: 0x04002DBD RID: 11709
	public RustButton[] filterButtons;

	// Token: 0x04002DBE RID: 11710
	public RelationshipManager.RelationshipType selectedRelationshipType = RelationshipManager.RelationshipType.Friend;

	// Token: 0x04002DBF RID: 11711
	public RustButton lastSeenToggle;

	// Token: 0x04002DC0 RID: 11712
	public Translate.Phrase sortingByLastSeenPhrase;

	// Token: 0x04002DC1 RID: 11713
	public Translate.Phrase sortingByFirstSeen;

	// Token: 0x04002DC2 RID: 11714
	public RustText sortText;

	// Token: 0x02000E8F RID: 3727
	public enum SortMode
	{
		// Token: 0x04004C82 RID: 19586
		None,
		// Token: 0x04004C83 RID: 19587
		RecentlySeen
	}
}
