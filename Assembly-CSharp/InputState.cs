using System;
using UnityEngine;

// Token: 0x020005CA RID: 1482
public class InputState
{
	// Token: 0x06002C7D RID: 11389 RVA: 0x0010D96A File Offset: 0x0010BB6A
	public bool IsDown(BUTTON btn)
	{
		return this.current != null && (this.SwallowedButtons & (int)btn) != (int)btn && (this.current.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x0010D993 File Offset: 0x0010BB93
	public bool WasDown(BUTTON btn)
	{
		return this.previous != null && (this.previous.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x0010D9AF File Offset: 0x0010BBAF
	public bool IsAnyDown()
	{
		return this.current != null && (float)(this.current.buttons & ~(float)this.SwallowedButtons) > 0f;
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x0010D9D6 File Offset: 0x0010BBD6
	public bool WasJustPressed(BUTTON btn)
	{
		return this.IsDown(btn) && !this.WasDown(btn);
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x0010D9ED File Offset: 0x0010BBED
	public bool WasJustReleased(BUTTON btn)
	{
		return !this.IsDown(btn) && this.WasDown(btn);
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x0010DA01 File Offset: 0x0010BC01
	public void SwallowButton(BUTTON btn)
	{
		if (this.current == null)
		{
			return;
		}
		this.SwallowedButtons |= (int)btn;
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x0010DA1A File Offset: 0x0010BC1A
	public Quaternion AimAngle()
	{
		if (this.current == null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(this.current.aimAngles);
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x0010DA3A File Offset: 0x0010BC3A
	public Vector3 MouseDelta()
	{
		if (this.current == null)
		{
			return Vector3.zero;
		}
		return this.current.mouseDelta;
	}

	// Token: 0x06002C85 RID: 11397 RVA: 0x0010DA58 File Offset: 0x0010BC58
	public void Flip(InputMessage newcurrent)
	{
		this.SwallowedButtons = 0;
		this.previous.aimAngles = this.current.aimAngles;
		this.previous.buttons = this.current.buttons;
		this.previous.mouseDelta = this.current.mouseDelta;
		this.current.aimAngles = newcurrent.aimAngles;
		this.current.buttons = newcurrent.buttons;
		this.current.mouseDelta = newcurrent.mouseDelta;
	}

	// Token: 0x06002C86 RID: 11398 RVA: 0x0010DAE1 File Offset: 0x0010BCE1
	public void Clear()
	{
		this.current.buttons = 0;
		this.previous.buttons = 0;
		this.current.mouseDelta = Vector3.zero;
		this.SwallowedButtons = 0;
	}

	// Token: 0x0400246B RID: 9323
	public InputMessage current = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x0400246C RID: 9324
	public InputMessage previous = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x0400246D RID: 9325
	private int SwallowedButtons;
}
