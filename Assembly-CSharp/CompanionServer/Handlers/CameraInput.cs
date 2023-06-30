using System;
using CompanionServer.Cameras;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FE RID: 2558
	public class CameraInput : BaseHandler<AppCameraInput>
	{
		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06003D22 RID: 15650 RVA: 0x001663E5 File Offset: 0x001645E5
		protected override double TokenCost
		{
			get
			{
				return 0.01;
			}
		}

		// Token: 0x06003D23 RID: 15651 RVA: 0x001663F0 File Offset: 0x001645F0
		public override void Execute()
		{
			if (!CameraRenderer.enabled)
			{
				base.SendError("not_enabled");
				return;
			}
			if (base.Client.CurrentCamera == null || !base.Client.IsControllingCamera)
			{
				base.SendError("no_camera");
				return;
			}
			InputState inputState = base.Client.InputState;
			if (inputState == null)
			{
				inputState = new InputState();
				base.Client.InputState = inputState;
			}
			InputMessage inputMessage = Pool.Get<InputMessage>();
			inputMessage.buttons = base.Proto.buttons;
			inputMessage.mouseDelta = CameraInput.Sanitize(base.Proto.mouseDelta);
			inputMessage.aimAngles = Vector3.zero;
			inputState.Flip(inputMessage);
			Pool.Free<InputMessage>(ref inputMessage);
			base.Client.CurrentCamera.UserInput(inputState, new CameraViewerId(base.Client.ControllingSteamId, base.Client.ConnectionId));
			base.SendSuccess();
		}

		// Token: 0x06003D24 RID: 15652 RVA: 0x001664D4 File Offset: 0x001646D4
		private static Vector3 Sanitize(Vector3 value)
		{
			return new Vector3(CameraInput.Sanitize(value.x), CameraInput.Sanitize(value.y), CameraInput.Sanitize(value.z));
		}

		// Token: 0x06003D25 RID: 15653 RVA: 0x001664FC File Offset: 0x001646FC
		private static float Sanitize(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				return 0f;
			}
			return Mathf.Clamp(value, -100f, 100f);
		}
	}
}
