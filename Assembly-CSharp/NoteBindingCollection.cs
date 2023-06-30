using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x02000402 RID: 1026
[CreateAssetMenu]
public class NoteBindingCollection : ScriptableObject
{
	// Token: 0x0600231F RID: 8991 RVA: 0x000E0ED8 File Offset: 0x000DF0D8
	public bool FindNoteData(Notes note, int octave, InstrumentKeyController.NoteType type, out NoteBindingCollection.NoteData data, out int noteIndex)
	{
		for (int i = 0; i < this.BaseBindings.Length; i++)
		{
			NoteBindingCollection.NoteData noteData = this.BaseBindings[i];
			if (noteData.Note == note && noteData.Type == type && noteData.NoteOctave == octave)
			{
				data = noteData;
				noteIndex = i;
				return true;
			}
		}
		data = default(NoteBindingCollection.NoteData);
		noteIndex = -1;
		return false;
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000E0F3C File Offset: 0x000DF13C
	public bool FindNoteDataIndex(Notes note, int octave, InstrumentKeyController.NoteType type, out int noteIndex)
	{
		for (int i = 0; i < this.BaseBindings.Length; i++)
		{
			NoteBindingCollection.NoteData noteData = this.BaseBindings[i];
			if (noteData.Note == note && noteData.Type == type && noteData.NoteOctave == octave)
			{
				noteIndex = i;
				return true;
			}
		}
		noteIndex = -1;
		return false;
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000E0F90 File Offset: 0x000DF190
	public NoteBindingCollection.NoteData CreateMidiBinding(NoteBindingCollection.NoteData basedOn, int octave, int midiCode)
	{
		NoteBindingCollection.NoteData noteData = basedOn;
		noteData.NoteOctave = octave;
		noteData.MidiNoteNumber = midiCode;
		int num = octave - basedOn.NoteOctave;
		if (octave > basedOn.NoteOctave)
		{
			noteData.PitchOffset = (float)num * 2f;
		}
		else
		{
			noteData.PitchOffset = 1f - Mathf.Abs((float)num * 0.1f);
		}
		return noteData;
	}

	// Token: 0x04001AED RID: 6893
	public NoteBindingCollection.NoteData[] BaseBindings;

	// Token: 0x04001AEE RID: 6894
	public float MinimumNoteTime;

	// Token: 0x04001AEF RID: 6895
	public float MaximumNoteLength;

	// Token: 0x04001AF0 RID: 6896
	public bool AllowAutoplay = true;

	// Token: 0x04001AF1 RID: 6897
	public float AutoplayLoopDelay = 0.25f;

	// Token: 0x04001AF2 RID: 6898
	public string NotePlayedStatName;

	// Token: 0x04001AF3 RID: 6899
	public string KeyMidiMapShortname = "";

	// Token: 0x04001AF4 RID: 6900
	public bool AllowSustain;

	// Token: 0x04001AF5 RID: 6901
	public bool AllowFullKeyboardInput = true;

	// Token: 0x04001AF6 RID: 6902
	public string InstrumentShortName = "";

	// Token: 0x04001AF7 RID: 6903
	public InstrumentKeyController.InstrumentType NotePlayType;

	// Token: 0x04001AF8 RID: 6904
	public int MaxConcurrentNotes = 3;

	// Token: 0x04001AF9 RID: 6905
	public bool LoopSounds;

	// Token: 0x04001AFA RID: 6906
	public float SoundFadeInTime;

	// Token: 0x04001AFB RID: 6907
	public float minimumSoundFadeOutTime = 0.1f;

	// Token: 0x04001AFC RID: 6908
	public InstrumentKeyController.KeySet PrimaryClickNote;

	// Token: 0x04001AFD RID: 6909
	public InstrumentKeyController.KeySet SecondaryClickNote = new InstrumentKeyController.KeySet
	{
		Note = Notes.B
	};

	// Token: 0x04001AFE RID: 6910
	public bool RunInstrumentAnimationController;

	// Token: 0x04001AFF RID: 6911
	public bool PlayRepeatAnimations = true;

	// Token: 0x04001B00 RID: 6912
	public float AnimationDeadTime = 1f;

	// Token: 0x04001B01 RID: 6913
	public float AnimationResetDelay;

	// Token: 0x04001B02 RID: 6914
	public float RecentlyPlayedThreshold = 1f;

	// Token: 0x04001B03 RID: 6915
	[Range(0f, 1f)]
	public float CrossfadeNormalizedAnimationTarget;

	// Token: 0x04001B04 RID: 6916
	public float AnimationCrossfadeDuration = 0.15f;

	// Token: 0x04001B05 RID: 6917
	public float CrossfadePlayerSpeedMulti = 1f;

	// Token: 0x04001B06 RID: 6918
	public int DefaultOctave;

	// Token: 0x04001B07 RID: 6919
	public int ShiftedOctave = 1;

	// Token: 0x04001B08 RID: 6920
	public bool UseClosestMidiNote = true;

	// Token: 0x04001B09 RID: 6921
	private const float MidiNoteUpOctaveShift = 2f;

	// Token: 0x04001B0A RID: 6922
	private const float MidiNoteDownOctaveShift = 0.1f;

	// Token: 0x02000CEC RID: 3308
	[Serializable]
	public struct NoteData
	{
		// Token: 0x06005016 RID: 20502 RVA: 0x001A80EC File Offset: 0x001A62EC
		public bool MatchMidiCode(int code)
		{
			if (this.MidiNoteNumber == code)
			{
				return true;
			}
			if (this.AdditionalMidiTargets != null)
			{
				int[] additionalMidiTargets = this.AdditionalMidiTargets;
				for (int i = 0; i < additionalMidiTargets.Length; i++)
				{
					if (additionalMidiTargets[i] == code)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005017 RID: 20503 RVA: 0x001A812A File Offset: 0x001A632A
		public string ToNoteString()
		{
			return string.Format("{0}{1}{2}", this.Note, (this.Type == InstrumentKeyController.NoteType.Sharp) ? "#" : string.Empty, this.NoteOctave);
		}

		// Token: 0x040045D5 RID: 17877
		public SoundDefinition NoteSound;

		// Token: 0x040045D6 RID: 17878
		public SoundDefinition NoteStartSound;

		// Token: 0x040045D7 RID: 17879
		public Notes Note;

		// Token: 0x040045D8 RID: 17880
		public InstrumentKeyController.NoteType Type;

		// Token: 0x040045D9 RID: 17881
		public int MidiNoteNumber;

		// Token: 0x040045DA RID: 17882
		public int NoteOctave;

		// Token: 0x040045DB RID: 17883
		[InstrumentIKTarget]
		public InstrumentKeyController.IKNoteTarget NoteIKTarget;

		// Token: 0x040045DC RID: 17884
		public InstrumentKeyController.AnimationSlot AnimationSlot;

		// Token: 0x040045DD RID: 17885
		public int NoteSoundPositionTarget;

		// Token: 0x040045DE RID: 17886
		public int[] AdditionalMidiTargets;

		// Token: 0x040045DF RID: 17887
		public float PitchOffset;
	}
}
