using System;
using UnityEngine;

// Token: 0x02000974 RID: 2420
public class RagdollEditor : SingletonComponent<RagdollEditor>
{
	// Token: 0x060039D0 RID: 14800 RVA: 0x001562E1 File Offset: 0x001544E1
	private void OnGUI()
	{
		GUI.Box(new Rect((float)Screen.width * 0.5f - 2f, (float)Screen.height * 0.5f - 2f, 4f, 4f), "");
	}

	// Token: 0x060039D1 RID: 14801 RVA: 0x00156320 File Offset: 0x00154520
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060039D2 RID: 14802 RVA: 0x00156328 File Offset: 0x00154528
	private void Update()
	{
		Camera.main.fieldOfView = 75f;
		if (Input.GetKey(KeyCode.Mouse1))
		{
			this.view.y = this.view.y + Input.GetAxisRaw("Mouse X") * 3f;
			this.view.x = this.view.x - Input.GetAxisRaw("Mouse Y") * 3f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		Camera.main.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		Camera.main.transform.position += base.transform.rotation * vector * 0.05f;
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.StartGrab();
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			this.StopGrab();
		}
	}

	// Token: 0x060039D3 RID: 14803 RVA: 0x00156475 File Offset: 0x00154675
	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			this.UpdateGrab();
		}
	}

	// Token: 0x060039D4 RID: 14804 RVA: 0x0015648C File Offset: 0x0015468C
	private void StartGrab()
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, 100f))
		{
			return;
		}
		this.grabbedRigid = raycastHit.collider.GetComponent<Rigidbody>();
		if (this.grabbedRigid == null)
		{
			return;
		}
		this.grabPos = this.grabbedRigid.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
		this.grabOffset = base.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
	}

	// Token: 0x060039D5 RID: 14805 RVA: 0x00156524 File Offset: 0x00154724
	private void UpdateGrab()
	{
		if (this.grabbedRigid == null)
		{
			return;
		}
		Vector3 vector = base.transform.TransformPoint(this.grabOffset);
		Vector3 vector2 = this.grabbedRigid.transform.TransformPoint(this.grabPos);
		Vector3 vector3 = vector - vector2;
		this.grabbedRigid.AddForceAtPosition(vector3 * 100f, vector2, ForceMode.Acceleration);
	}

	// Token: 0x060039D6 RID: 14806 RVA: 0x00156587 File Offset: 0x00154787
	private void StopGrab()
	{
		this.grabbedRigid = null;
	}

	// Token: 0x04003453 RID: 13395
	private Vector3 view;

	// Token: 0x04003454 RID: 13396
	private Rigidbody grabbedRigid;

	// Token: 0x04003455 RID: 13397
	private Vector3 grabPos;

	// Token: 0x04003456 RID: 13398
	private Vector3 grabOffset;
}
