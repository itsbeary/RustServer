using System;
using UnityEngine;

// Token: 0x02000104 RID: 260
public interface IRemoteControllable
{
	// Token: 0x060015B8 RID: 5560
	Transform GetEyes();

	// Token: 0x060015B9 RID: 5561
	float GetFovScale();

	// Token: 0x060015BA RID: 5562
	BaseEntity GetEnt();

	// Token: 0x060015BB RID: 5563
	string GetIdentifier();

	// Token: 0x060015BC RID: 5564
	float Health();

	// Token: 0x060015BD RID: 5565
	float MaxHealth();

	// Token: 0x060015BE RID: 5566
	void UpdateIdentifier(string newID, bool clientSend = false);

	// Token: 0x060015BF RID: 5567
	void RCSetup();

	// Token: 0x060015C0 RID: 5568
	void RCShutdown();

	// Token: 0x060015C1 RID: 5569
	bool CanControl(ulong playerID);

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x060015C2 RID: 5570
	bool RequiresMouse { get; }

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x060015C3 RID: 5571
	float MaxRange { get; }

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x060015C4 RID: 5572
	RemoteControllableControls RequiredControls { get; }

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x060015C5 RID: 5573
	CameraViewerId? ControllingViewerId { get; }

	// Token: 0x060015C6 RID: 5574
	void UserInput(InputState inputState, CameraViewerId viewerID);

	// Token: 0x060015C7 RID: 5575
	bool InitializeControl(CameraViewerId viewerID);

	// Token: 0x060015C8 RID: 5576
	void StopControl(CameraViewerId viewerID);

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x060015C9 RID: 5577
	bool CanPing { get; }
}
