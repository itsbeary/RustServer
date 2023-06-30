using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A91 RID: 2705
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Debug", 1002)]
	public sealed class PostProcessDebug : MonoBehaviour
	{
		// Token: 0x06004045 RID: 16453 RVA: 0x0017AB8D File Offset: 0x00178D8D
		private void OnEnable()
		{
			this.m_CmdAfterEverything = new CommandBuffer
			{
				name = "Post-processing Debug Overlay"
			};
		}

		// Token: 0x06004046 RID: 16454 RVA: 0x0017ABA5 File Offset: 0x00178DA5
		private void OnDisable()
		{
			if (this.m_CurrentCamera != null)
			{
				this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
			}
			this.m_CurrentCamera = null;
			this.m_PreviousPostProcessLayer = null;
		}

		// Token: 0x06004047 RID: 16455 RVA: 0x0017ABD6 File Offset: 0x00178DD6
		private void Update()
		{
			this.UpdateStates();
		}

		// Token: 0x06004048 RID: 16456 RVA: 0x0017ABDE File Offset: 0x00178DDE
		private void Reset()
		{
			this.postProcessLayer = base.GetComponent<PostProcessLayer>();
		}

		// Token: 0x06004049 RID: 16457 RVA: 0x0017ABEC File Offset: 0x00178DEC
		private void UpdateStates()
		{
			if (this.m_PreviousPostProcessLayer != this.postProcessLayer)
			{
				if (this.m_CurrentCamera != null)
				{
					this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
					this.m_CurrentCamera = null;
				}
				this.m_PreviousPostProcessLayer = this.postProcessLayer;
				if (this.postProcessLayer != null)
				{
					this.m_CurrentCamera = this.postProcessLayer.GetComponent<Camera>();
					this.m_CurrentCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
				}
			}
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled)
			{
				return;
			}
			if (this.lightMeter)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.LightMeter);
			}
			if (this.histogram)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Histogram);
			}
			if (this.waveform)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Waveform);
			}
			if (this.vectorscope)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Vectorscope);
			}
			this.postProcessLayer.debugLayer.RequestDebugOverlay(this.debugOverlay);
		}

		// Token: 0x0600404A RID: 16458 RVA: 0x0017AD08 File Offset: 0x00178F08
		private void OnPostRender()
		{
			this.m_CmdAfterEverything.Clear();
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled || !this.postProcessLayer.debugLayer.debugOverlayActive)
			{
				return;
			}
			this.m_CmdAfterEverything.Blit(this.postProcessLayer.debugLayer.debugOverlayTarget, BuiltinRenderTextureType.CameraTarget);
		}

		// Token: 0x0600404B RID: 16459 RVA: 0x0017AD70 File Offset: 0x00178F70
		private void OnGUI()
		{
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled)
			{
				return;
			}
			RenderTexture.active = null;
			Rect rect = new Rect(5f, 5f, 0f, 0f);
			PostProcessDebugLayer debugLayer = this.postProcessLayer.debugLayer;
			this.DrawMonitor(ref rect, debugLayer.lightMeter, this.lightMeter);
			this.DrawMonitor(ref rect, debugLayer.histogram, this.histogram);
			this.DrawMonitor(ref rect, debugLayer.waveform, this.waveform);
			this.DrawMonitor(ref rect, debugLayer.vectorscope, this.vectorscope);
		}

		// Token: 0x0600404C RID: 16460 RVA: 0x0017AE18 File Offset: 0x00179018
		private void DrawMonitor(ref Rect rect, Monitor monitor, bool enabled)
		{
			if (!enabled || monitor.output == null)
			{
				return;
			}
			rect.width = (float)monitor.output.width;
			rect.height = (float)monitor.output.height;
			GUI.DrawTexture(rect, monitor.output);
			rect.x += (float)monitor.output.width + 5f;
		}

		// Token: 0x040039BD RID: 14781
		public PostProcessLayer postProcessLayer;

		// Token: 0x040039BE RID: 14782
		private PostProcessLayer m_PreviousPostProcessLayer;

		// Token: 0x040039BF RID: 14783
		public bool lightMeter;

		// Token: 0x040039C0 RID: 14784
		public bool histogram;

		// Token: 0x040039C1 RID: 14785
		public bool waveform;

		// Token: 0x040039C2 RID: 14786
		public bool vectorscope;

		// Token: 0x040039C3 RID: 14787
		public DebugOverlay debugOverlay;

		// Token: 0x040039C4 RID: 14788
		private Camera m_CurrentCamera;

		// Token: 0x040039C5 RID: 14789
		private CommandBuffer m_CmdAfterEverything;
	}
}
