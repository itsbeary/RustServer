using System;
using UnityEngine;

// Token: 0x020002CE RID: 718
[ExecuteInEditMode]
public class MainCamera : RustCamera<MainCamera>
{
	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001DC5 RID: 7621 RVA: 0x000CC6C4 File Offset: 0x000CA8C4
	public static bool isValid
	{
		get
		{
			return MainCamera.mainCamera != null && MainCamera.mainCamera.enabled;
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001DC6 RID: 7622 RVA: 0x000CC6DF File Offset: 0x000CA8DF
	// (set) Token: 0x06001DC7 RID: 7623 RVA: 0x000CC6E6 File Offset: 0x000CA8E6
	public static Vector3 velocity { get; private set; }

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001DC8 RID: 7624 RVA: 0x000CC6EE File Offset: 0x000CA8EE
	// (set) Token: 0x06001DC9 RID: 7625 RVA: 0x000CC6FA File Offset: 0x000CA8FA
	public static Vector3 position
	{
		get
		{
			return MainCamera.mainCameraTransform.position;
		}
		set
		{
			MainCamera.mainCameraTransform.position = value;
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001DCA RID: 7626 RVA: 0x000CC707 File Offset: 0x000CA907
	// (set) Token: 0x06001DCB RID: 7627 RVA: 0x000CC713 File Offset: 0x000CA913
	public static Vector3 forward
	{
		get
		{
			return MainCamera.mainCameraTransform.forward;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCameraTransform.forward = value;
			}
		}
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001DCC RID: 7628 RVA: 0x000CC72E File Offset: 0x000CA92E
	// (set) Token: 0x06001DCD RID: 7629 RVA: 0x000CC73A File Offset: 0x000CA93A
	public static Vector3 right
	{
		get
		{
			return MainCamera.mainCameraTransform.right;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCameraTransform.right = value;
			}
		}
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06001DCE RID: 7630 RVA: 0x000CC755 File Offset: 0x000CA955
	// (set) Token: 0x06001DCF RID: 7631 RVA: 0x000CC761 File Offset: 0x000CA961
	public static Vector3 up
	{
		get
		{
			return MainCamera.mainCameraTransform.up;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.up = value;
			}
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06001DD0 RID: 7632 RVA: 0x000CC781 File Offset: 0x000CA981
	// (set) Token: 0x06001DD1 RID: 7633 RVA: 0x000CC78D File Offset: 0x000CA98D
	public static Quaternion rotation
	{
		get
		{
			return MainCamera.mainCameraTransform.rotation;
		}
		set
		{
			MainCamera.mainCameraTransform.rotation = value;
		}
	}

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06001DD2 RID: 7634 RVA: 0x000CC79A File Offset: 0x000CA99A
	public static Ray Ray
	{
		get
		{
			return new Ray(MainCamera.position, MainCamera.forward);
		}
	}

	// Token: 0x040016AB RID: 5803
	public static Camera mainCamera;

	// Token: 0x040016AC RID: 5804
	public static Transform mainCameraTransform;
}
