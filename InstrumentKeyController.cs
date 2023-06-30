using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class InstrumentKeyController : MonoBehaviour
{
	// Token: 0x170002EF RID: 751
	// (get) Token: 0x0600231B RID: 8987 RVA: 0x000E0E7C File Offset: 0x000DF07C
	// (set) Token: 0x0600231C RID: 8988 RVA: 0x000E0E84 File Offset: 0x000DF084
	public bool PlayedNoteThisFrame { get; private set; }

	// Token: 0x0600231D RID: 8989 RVA: 0x000E0E8D File Offset: 0x000DF08D
	public void ProcessServerPlayedNote(BasePlayer forPlayer)
	{
		if (forPlayer == null)
		{
			return;
		}
		forPlayer.stats.Add(this.Bindings.NotePlayedStatName, 1, (Stats)5);
		forPlayer.stats.Add("played_notes", 1, (Stats)5);
	}

	// Token: 0x04001AE1 RID: 6881
	public const float DEFAULT_NOTE_VELOCITY = 1f;

	// Token: 0x04001AE2 RID: 6882
	public NoteBindingCollection Bindings;

	// Token: 0x04001AE3 RID: 6883
	public InstrumentKeyController.NoteBinding[] NoteBindings = new InstrumentKeyController.NoteBinding[0];

	// Token: 0x04001AE4 RID: 6884
	public Transform[] NoteSoundPositions;

	// Token: 0x04001AE5 RID: 6885
	public InstrumentIKController IKController;

	// Token: 0x04001AE6 RID: 6886
	public Transform LeftHandProp;

	// Token: 0x04001AE7 RID: 6887
	public Transform RightHandProp;

	// Token: 0x04001AE8 RID: 6888
	public Animator InstrumentAnimator;

	// Token: 0x04001AE9 RID: 6889
	public BaseEntity RPCHandler;

	// Token: 0x04001AEA RID: 6890
	public uint overrideAchievementId;

	// Token: 0x04001AEC RID: 6892
	private const string ALL_NOTES_STATNAME = "played_notes";

	// Token: 0x02000CE4 RID: 3300
	public struct NoteBinding
	{
	}

	// Token: 0x02000CE5 RID: 3301
	public enum IKType
	{
		// Token: 0x040045BC RID: 17852
		LeftHand,
		// Token: 0x040045BD RID: 17853
		RightHand,
		// Token: 0x040045BE RID: 17854
		RightFoot
	}

	// Token: 0x02000CE6 RID: 3302
	public enum NoteType
	{
		// Token: 0x040045C0 RID: 17856
		Regular,
		// Token: 0x040045C1 RID: 17857
		Sharp
	}

	// Token: 0x02000CE7 RID: 3303
	public enum InstrumentType
	{
		// Token: 0x040045C3 RID: 17859
		Note,
		// Token: 0x040045C4 RID: 17860
		Hold
	}

	// Token: 0x02000CE8 RID: 3304
	public enum AnimationSlot
	{
		// Token: 0x040045C6 RID: 17862
		None,
		// Token: 0x040045C7 RID: 17863
		One,
		// Token: 0x040045C8 RID: 17864
		Two,
		// Token: 0x040045C9 RID: 17865
		Three,
		// Token: 0x040045CA RID: 17866
		Four,
		// Token: 0x040045CB RID: 17867
		Five,
		// Token: 0x040045CC RID: 17868
		Six,
		// Token: 0x040045CD RID: 17869
		Seven
	}

	// Token: 0x02000CE9 RID: 3305
	[Serializable]
	public struct KeySet
	{
		// Token: 0x06005015 RID: 20501 RVA: 0x001A80B3 File Offset: 0x001A62B3
		public override string ToString()
		{
			return string.Format("{0}{1}{2}", this.Note, (this.NoteType == InstrumentKeyController.NoteType.Sharp) ? "#" : string.Empty, this.OctaveShift);
		}

		// Token: 0x040045CE RID: 17870
		public Notes Note;

		// Token: 0x040045CF RID: 17871
		public InstrumentKeyController.NoteType NoteType;

		// Token: 0x040045D0 RID: 17872
		public int OctaveShift;
	}

	// Token: 0x02000CEA RID: 3306
	public struct NoteOverride
	{
		// Token: 0x040045D1 RID: 17873
		public bool Override;

		// Token: 0x040045D2 RID: 17874
		public InstrumentKeyController.KeySet Note;
	}

	// Token: 0x02000CEB RID: 3307
	[Serializable]
	public struct IKNoteTarget
	{
		// Token: 0x040045D3 RID: 17875
		public InstrumentKeyController.IKType TargetType;

		// Token: 0x040045D4 RID: 17876
		public int IkIndex;
	}
}
