using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003FE RID: 1022
public class InstrumentDebugInput : MonoBehaviour
{
	// Token: 0x04001ACC RID: 6860
	public InstrumentKeyController KeyController;

	// Token: 0x04001ACD RID: 6861
	public InstrumentKeyController.KeySet Note = new InstrumentKeyController.KeySet
	{
		Note = Notes.A,
		NoteType = InstrumentKeyController.NoteType.Regular,
		OctaveShift = 3
	};

	// Token: 0x04001ACE RID: 6862
	public float Frequency = 0.75f;

	// Token: 0x04001ACF RID: 6863
	public float StopAfter = 0.1f;

	// Token: 0x04001AD0 RID: 6864
	public SoundDefinition OverrideDefinition;
}
