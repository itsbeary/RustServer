using System;
using System.Collections.Generic;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020007FC RID: 2044
public class MapView : FacepunchBehaviour
{
	// Token: 0x04002DFB RID: 11771
	public RawImage mapImage;

	// Token: 0x04002DFC RID: 11772
	public Image cameraPositon;

	// Token: 0x04002DFD RID: 11773
	public ScrollRectEx scrollRect;

	// Token: 0x04002DFE RID: 11774
	public GameObject monumentMarkerContainer;

	// Token: 0x04002DFF RID: 11775
	public Transform clusterMarkerContainer;

	// Token: 0x04002E00 RID: 11776
	public GameObjectRef monumentMarkerPrefab;

	// Token: 0x04002E01 RID: 11777
	public GameObject missionMarkerContainer;

	// Token: 0x04002E02 RID: 11778
	public GameObjectRef missionMarkerPrefab;

	// Token: 0x04002E03 RID: 11779
	public Transform activeInteractionParent;

	// Token: 0x04002E04 RID: 11780
	public Transform localPlayerInterestPointRoot;

	// Token: 0x04002E05 RID: 11781
	public TeamMemberMapMarker[] teamPositions;

	// Token: 0x04002E06 RID: 11782
	public List<PointOfInterestMapMarker> PointOfInterestMarkers;

	// Token: 0x04002E07 RID: 11783
	public List<PointOfInterestMapMarker> TeamPointOfInterestMarkers;

	// Token: 0x04002E08 RID: 11784
	public List<PointOfInterestMapMarker> LocalPings;

	// Token: 0x04002E09 RID: 11785
	public List<PointOfInterestMapMarker> TeamPings;

	// Token: 0x04002E0A RID: 11786
	public GameObject PlayerDeathMarker;

	// Token: 0x04002E0B RID: 11787
	public List<SleepingBagMapMarker> SleepingBagMarkers = new List<SleepingBagMapMarker>();

	// Token: 0x04002E0C RID: 11788
	public List<SleepingBagClusterMapMarker> SleepingBagClusters = new List<SleepingBagClusterMapMarker>();

	// Token: 0x04002E0D RID: 11789
	[FormerlySerializedAs("TrainLayer")]
	public RawImage UndergroundLayer;

	// Token: 0x04002E0E RID: 11790
	public bool ShowGrid;

	// Token: 0x04002E0F RID: 11791
	public bool ShowPointOfInterestMarkers;

	// Token: 0x04002E10 RID: 11792
	public bool ShowDeathMarker = true;

	// Token: 0x04002E11 RID: 11793
	public bool ShowSleepingBags = true;

	// Token: 0x04002E12 RID: 11794
	public bool AllowSleepingBagDeletion;

	// Token: 0x04002E13 RID: 11795
	public bool ShowLocalPlayer = true;

	// Token: 0x04002E14 RID: 11796
	public bool ShowTeamMembers = true;

	// Token: 0x04002E15 RID: 11797
	public bool ShowTrainLayer;

	// Token: 0x04002E16 RID: 11798
	public bool ShowMissions;

	// Token: 0x04002E17 RID: 11799
	[FormerlySerializedAs("ShowTrainLayer")]
	public bool ShowUndergroundLayers;

	// Token: 0x04002E18 RID: 11800
	public bool MLRSMarkerMode;

	// Token: 0x04002E19 RID: 11801
	public RustImageButton LockButton;

	// Token: 0x04002E1A RID: 11802
	public RustImageButton OverworldButton;

	// Token: 0x04002E1B RID: 11803
	public RustImageButton TrainButton;

	// Token: 0x04002E1C RID: 11804
	public RustImageButton[] UnderwaterButtons;

	// Token: 0x04002E1D RID: 11805
	public RustImageButton DungeonButton;
}
