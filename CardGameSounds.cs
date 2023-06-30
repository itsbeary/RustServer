using System;
using UnityEngine;

// Token: 0x02000410 RID: 1040
public class CardGameSounds : PrefabAttribute
{
	// Token: 0x06002370 RID: 9072 RVA: 0x000E255B File Offset: 0x000E075B
	protected override Type GetIndexedType()
	{
		return typeof(CardGameSounds);
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x000E2568 File Offset: 0x000E0768
	public void PlaySound(CardGameSounds.SoundType sound, GameObject forGameObject)
	{
		switch (sound)
		{
		case CardGameSounds.SoundType.Chips:
			this.ChipsSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Draw:
			this.DrawSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Play:
			this.PlaySfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Shuffle:
			this.ShuffleSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Win:
			this.WinSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.YourTurn:
			this.YourTurnSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Check:
			this.CheckSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Hit:
			this.HitSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Stand:
			this.StandSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Bet:
			this.BetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.IncreaseBet:
			this.IncreaseBetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DecreaseBet:
			this.DecreaseBetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.AllIn:
			this.AllInSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.UIInteract:
			this.UIInteractSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerCool:
			this.DealerCoolSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerHappy:
			this.DealerHappySfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerLove:
			this.DealerLoveSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerSad:
			this.DealerSadSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerShocked:
			this.DealerShockedSfx.Play(forGameObject);
			return;
		default:
			throw new ArgumentOutOfRangeException("sound", sound, null);
		}
	}

	// Token: 0x04001B41 RID: 6977
	public SoundDefinition ChipsSfx;

	// Token: 0x04001B42 RID: 6978
	public SoundDefinition DrawSfx;

	// Token: 0x04001B43 RID: 6979
	public SoundDefinition PlaySfx;

	// Token: 0x04001B44 RID: 6980
	public SoundDefinition ShuffleSfx;

	// Token: 0x04001B45 RID: 6981
	public SoundDefinition WinSfx;

	// Token: 0x04001B46 RID: 6982
	public SoundDefinition LoseSfx;

	// Token: 0x04001B47 RID: 6983
	public SoundDefinition YourTurnSfx;

	// Token: 0x04001B48 RID: 6984
	public SoundDefinition CheckSfx;

	// Token: 0x04001B49 RID: 6985
	public SoundDefinition HitSfx;

	// Token: 0x04001B4A RID: 6986
	public SoundDefinition StandSfx;

	// Token: 0x04001B4B RID: 6987
	public SoundDefinition BetSfx;

	// Token: 0x04001B4C RID: 6988
	public SoundDefinition IncreaseBetSfx;

	// Token: 0x04001B4D RID: 6989
	public SoundDefinition DecreaseBetSfx;

	// Token: 0x04001B4E RID: 6990
	public SoundDefinition AllInSfx;

	// Token: 0x04001B4F RID: 6991
	public SoundDefinition UIInteractSfx;

	// Token: 0x04001B50 RID: 6992
	[Header("Dealer Reactions")]
	public SoundDefinition DealerCoolSfx;

	// Token: 0x04001B51 RID: 6993
	public SoundDefinition DealerHappySfx;

	// Token: 0x04001B52 RID: 6994
	public SoundDefinition DealerLoveSfx;

	// Token: 0x04001B53 RID: 6995
	public SoundDefinition DealerSadSfx;

	// Token: 0x04001B54 RID: 6996
	public SoundDefinition DealerShockedSfx;

	// Token: 0x02000CF1 RID: 3313
	public enum SoundType
	{
		// Token: 0x040045F5 RID: 17909
		Chips,
		// Token: 0x040045F6 RID: 17910
		Draw,
		// Token: 0x040045F7 RID: 17911
		Play,
		// Token: 0x040045F8 RID: 17912
		Shuffle,
		// Token: 0x040045F9 RID: 17913
		Win,
		// Token: 0x040045FA RID: 17914
		YourTurn,
		// Token: 0x040045FB RID: 17915
		Check,
		// Token: 0x040045FC RID: 17916
		Hit,
		// Token: 0x040045FD RID: 17917
		Stand,
		// Token: 0x040045FE RID: 17918
		Bet,
		// Token: 0x040045FF RID: 17919
		IncreaseBet,
		// Token: 0x04004600 RID: 17920
		DecreaseBet,
		// Token: 0x04004601 RID: 17921
		AllIn,
		// Token: 0x04004602 RID: 17922
		UIInteract,
		// Token: 0x04004603 RID: 17923
		DealerCool,
		// Token: 0x04004604 RID: 17924
		DealerHappy,
		// Token: 0x04004605 RID: 17925
		DealerLove,
		// Token: 0x04004606 RID: 17926
		DealerSad,
		// Token: 0x04004607 RID: 17927
		DealerShocked
	}
}
