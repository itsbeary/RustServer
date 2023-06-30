using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class InstrumentViewmodel : MonoBehaviour
{
	// Token: 0x06001701 RID: 5889 RVA: 0x000B0104 File Offset: 0x000AE304
	public void UpdateSlots(InstrumentKeyController.AnimationSlot currentSlot, bool recentlyPlayed, bool playedNoteThisFrame)
	{
		if (this.ViewAnimator == null)
		{
			return;
		}
		if (this.UpdateA)
		{
			this.UpdateState(this.note_a, currentSlot == InstrumentKeyController.AnimationSlot.One);
		}
		if (this.UpdateB)
		{
			this.UpdateState(this.note_b, currentSlot == InstrumentKeyController.AnimationSlot.Two);
		}
		if (this.UpdateC)
		{
			this.UpdateState(this.note_c, currentSlot == InstrumentKeyController.AnimationSlot.Three);
		}
		if (this.UpdateD)
		{
			this.UpdateState(this.note_d, currentSlot == InstrumentKeyController.AnimationSlot.Four);
		}
		if (this.UpdateE)
		{
			this.UpdateState(this.note_e, currentSlot == InstrumentKeyController.AnimationSlot.Five);
		}
		if (this.UpdateF)
		{
			this.UpdateState(this.note_f, currentSlot == InstrumentKeyController.AnimationSlot.Six);
		}
		if (this.UpdateG)
		{
			this.UpdateState(this.note_g, currentSlot == InstrumentKeyController.AnimationSlot.Seven);
		}
		if (this.UpdateRecentlyPlayed)
		{
			this.ViewAnimator.SetBool(this.recentlyPlayedHash, recentlyPlayed);
		}
		if (this.UpdatePlayedNoteTrigger && playedNoteThisFrame)
		{
			this.ViewAnimator.SetTrigger(this.playedNoteHash);
		}
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x000B01FD File Offset: 0x000AE3FD
	private void UpdateState(int param, bool state)
	{
		if (!this.UseTriggers)
		{
			this.ViewAnimator.SetBool(param, state);
			return;
		}
		if (state)
		{
			this.ViewAnimator.SetTrigger(param);
		}
	}

	// Token: 0x04000F2A RID: 3882
	public Animator ViewAnimator;

	// Token: 0x04000F2B RID: 3883
	public bool UpdateA = true;

	// Token: 0x04000F2C RID: 3884
	public bool UpdateB = true;

	// Token: 0x04000F2D RID: 3885
	public bool UpdateC = true;

	// Token: 0x04000F2E RID: 3886
	public bool UpdateD = true;

	// Token: 0x04000F2F RID: 3887
	public bool UpdateE = true;

	// Token: 0x04000F30 RID: 3888
	public bool UpdateF = true;

	// Token: 0x04000F31 RID: 3889
	public bool UpdateG = true;

	// Token: 0x04000F32 RID: 3890
	public bool UpdateRecentlyPlayed = true;

	// Token: 0x04000F33 RID: 3891
	public bool UpdatePlayedNoteTrigger;

	// Token: 0x04000F34 RID: 3892
	public bool UseTriggers;

	// Token: 0x04000F35 RID: 3893
	private readonly int note_a = Animator.StringToHash("play_A");

	// Token: 0x04000F36 RID: 3894
	private readonly int note_b = Animator.StringToHash("play_B");

	// Token: 0x04000F37 RID: 3895
	private readonly int note_c = Animator.StringToHash("play_C");

	// Token: 0x04000F38 RID: 3896
	private readonly int note_d = Animator.StringToHash("play_D");

	// Token: 0x04000F39 RID: 3897
	private readonly int note_e = Animator.StringToHash("play_E");

	// Token: 0x04000F3A RID: 3898
	private readonly int note_f = Animator.StringToHash("play_F");

	// Token: 0x04000F3B RID: 3899
	private readonly int note_g = Animator.StringToHash("play_G");

	// Token: 0x04000F3C RID: 3900
	private readonly int recentlyPlayedHash = Animator.StringToHash("recentlyPlayed");

	// Token: 0x04000F3D RID: 3901
	private readonly int playedNoteHash = Animator.StringToHash("playedNote");
}
