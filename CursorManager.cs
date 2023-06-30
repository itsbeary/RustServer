using System;
using UnityEngine;

// Token: 0x020007A7 RID: 1959
public class CursorManager : SingletonComponent<CursorManager>
{
	// Token: 0x06003506 RID: 13574 RVA: 0x00145778 File Offset: 0x00143978
	private void Update()
	{
		if (SingletonComponent<CursorManager>.Instance != this)
		{
			return;
		}
		if (CursorManager.iHoldOpen == 0 && CursorManager.iPreviousOpen == 0)
		{
			this.SwitchToGame();
		}
		else
		{
			this.SwitchToUI();
		}
		CursorManager.iPreviousOpen = CursorManager.iHoldOpen;
		CursorManager.iHoldOpen = 0;
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x001457B4 File Offset: 0x001439B4
	public void SwitchToGame()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
		CursorManager.lastTimeInvisible = Time.time;
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x001457DB File Offset: 0x001439DB
	private void SwitchToUI()
	{
		if (Cursor.lockState != CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
		CursorManager.lastTimeVisible = Time.time;
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x00145801 File Offset: 0x00143A01
	public static void HoldOpen(bool cursorVisible = false)
	{
		CursorManager.iHoldOpen++;
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x0014580F File Offset: 0x00143A0F
	public static bool WasVisible(float deltaTime)
	{
		return Time.time - CursorManager.lastTimeVisible <= deltaTime;
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x00145822 File Offset: 0x00143A22
	public static bool WasInvisible(float deltaTime)
	{
		return Time.time - CursorManager.lastTimeInvisible <= deltaTime;
	}

	// Token: 0x04002BB9 RID: 11193
	private static int iHoldOpen;

	// Token: 0x04002BBA RID: 11194
	private static int iPreviousOpen;

	// Token: 0x04002BBB RID: 11195
	private static float lastTimeVisible;

	// Token: 0x04002BBC RID: 11196
	private static float lastTimeInvisible;
}
