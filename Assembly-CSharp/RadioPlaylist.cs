using System;
using UnityEngine;

// Token: 0x020003A9 RID: 937
[CreateAssetMenu]
public class RadioPlaylist : ScriptableObject
{
	// Token: 0x040019EA RID: 6634
	public string Url;

	// Token: 0x040019EB RID: 6635
	public AudioClip[] Playlist;

	// Token: 0x040019EC RID: 6636
	public float PlaylistLength;
}
