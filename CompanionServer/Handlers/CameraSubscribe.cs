using System;
using CompanionServer.Cameras;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FF RID: 2559
	public class CameraSubscribe : BaseHandler<AppCameraSubscribe>
	{
		// Token: 0x06003D27 RID: 15655 RVA: 0x0016652C File Offset: 0x0016472C
		public override void Execute()
		{
			if (!CameraRenderer.enabled)
			{
				base.SendError("not_enabled");
				return;
			}
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				base.SendError("server_error");
				return;
			}
			if (string.IsNullOrEmpty(base.Proto.cameraId))
			{
				base.Client.EndViewing();
				base.SendError("invalid_id");
				return;
			}
			if (!base.Player.IsValid())
			{
				base.Client.EndViewing();
				base.SendError("no_player");
				return;
			}
			if (base.Player.IsConnected)
			{
				base.Client.EndViewing();
				base.SendError("player_online");
				return;
			}
			IRemoteControllable remoteControllable = RemoteControlEntity.FindByID(base.Proto.cameraId);
			if (remoteControllable == null || !remoteControllable.CanControl(base.UserId))
			{
				base.Client.EndViewing();
				base.SendError("not_found");
				return;
			}
			CCTV_RC cctv_RC;
			if ((cctv_RC = remoteControllable as CCTV_RC) != null && cctv_RC.IsStatic())
			{
				base.Client.EndViewing();
				base.SendError("access_denied");
				return;
			}
			global::BaseEntity ent = remoteControllable.GetEnt();
			if (!ent.IsValid())
			{
				base.Client.EndViewing();
				base.SendError("not_found");
				return;
			}
			if (Vector3.Distance(base.Player.transform.position, ent.transform.position) >= remoteControllable.MaxRange)
			{
				base.Client.EndViewing();
				base.SendError("not_found");
				return;
			}
			if (!base.Client.BeginViewing(remoteControllable))
			{
				base.Client.EndViewing();
				base.SendError("not_found");
				return;
			}
			instance.StartRendering(remoteControllable);
			AppResponse appResponse = Pool.Get<AppResponse>();
			AppCameraInfo appCameraInfo = Pool.Get<AppCameraInfo>();
			appCameraInfo.width = CameraRenderer.width;
			appCameraInfo.height = CameraRenderer.height;
			appCameraInfo.nearPlane = CameraRenderer.nearPlane;
			appCameraInfo.farPlane = CameraRenderer.farPlane;
			appCameraInfo.controlFlags = (int)(base.Client.IsControllingCamera ? remoteControllable.RequiredControls : RemoteControllableControls.None);
			appResponse.cameraSubscribeInfo = appCameraInfo;
			base.Send(appResponse);
		}
	}
}
