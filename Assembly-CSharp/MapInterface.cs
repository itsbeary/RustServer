using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F9 RID: 2041
public class MapInterface : SingletonComponent<MapInterface>
{
	// Token: 0x04002DE3 RID: 11747
	public static bool IsOpen;

	// Token: 0x04002DE4 RID: 11748
	public Image cameraPositon;

	// Token: 0x04002DE5 RID: 11749
	public ScrollRectEx scrollRect;

	// Token: 0x04002DE6 RID: 11750
	public Toggle showGridToggle;

	// Token: 0x04002DE7 RID: 11751
	public Button FocusButton;

	// Token: 0x04002DE8 RID: 11752
	public CanvasGroup CanvasGroup;

	// Token: 0x04002DE9 RID: 11753
	public SoundDefinition PlaceMarkerSound;

	// Token: 0x04002DEA RID: 11754
	public SoundDefinition ClearMarkerSound;

	// Token: 0x04002DEB RID: 11755
	public MapView View;

	// Token: 0x04002DEC RID: 11756
	public Color[] PointOfInterestColours;

	// Token: 0x04002DED RID: 11757
	public MapInterface.PointOfInterestSpriteConfig[] PointOfInterestSprites;

	// Token: 0x04002DEE RID: 11758
	public Sprite PingBackground;

	// Token: 0x04002DEF RID: 11759
	public bool DebugStayOpen;

	// Token: 0x04002DF0 RID: 11760
	public GameObjectRef MarkerListPrefab;

	// Token: 0x04002DF1 RID: 11761
	public GameObject MarkerHeader;

	// Token: 0x04002DF2 RID: 11762
	public Transform LocalPlayerMarkerListParent;

	// Token: 0x04002DF3 RID: 11763
	public Transform TeamMarkerListParent;

	// Token: 0x04002DF4 RID: 11764
	public GameObject TeamLeaderHeader;

	// Token: 0x04002DF5 RID: 11765
	public RustButton HideTeamLeaderMarkersToggle;

	// Token: 0x04002DF6 RID: 11766
	public CanvasGroup TeamMarkersCanvas;

	// Token: 0x04002DF7 RID: 11767
	public RustImageButton ShowSleepingBagsButton;

	// Token: 0x02000E95 RID: 3733
	[Serializable]
	public struct PointOfInterestSpriteConfig
	{
		// Token: 0x04004C92 RID: 19602
		public Sprite inner;

		// Token: 0x04004C93 RID: 19603
		public Sprite outer;
	}
}
