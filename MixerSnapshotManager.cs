using System;
using UnityEngine.Audio;

// Token: 0x02000231 RID: 561
public class MixerSnapshotManager : SingletonComponent<MixerSnapshotManager>, IClientComponent
{
	// Token: 0x04001437 RID: 5175
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04001438 RID: 5176
	public AudioMixerSnapshot underwaterSnapshot;

	// Token: 0x04001439 RID: 5177
	public AudioMixerSnapshot loadingSnapshot;

	// Token: 0x0400143A RID: 5178
	public AudioMixerSnapshot woundedSnapshot;

	// Token: 0x0400143B RID: 5179
	public AudioMixerSnapshot cctvSnapshot;

	// Token: 0x0400143C RID: 5180
	public SoundDefinition underwaterInSound;

	// Token: 0x0400143D RID: 5181
	public SoundDefinition underwaterOutSound;

	// Token: 0x0400143E RID: 5182
	public AudioMixerSnapshot recordingSnapshot;

	// Token: 0x0400143F RID: 5183
	public SoundDefinition woundedLoop;

	// Token: 0x04001440 RID: 5184
	private Sound woundedLoopSound;

	// Token: 0x04001441 RID: 5185
	public SoundDefinition cctvModeLoopDef;

	// Token: 0x04001442 RID: 5186
	private Sound cctvModeLoop;

	// Token: 0x04001443 RID: 5187
	public SoundDefinition cctvModeStartDef;

	// Token: 0x04001444 RID: 5188
	public SoundDefinition cctvModeStopDef;

	// Token: 0x04001445 RID: 5189
	public float deafness;
}
