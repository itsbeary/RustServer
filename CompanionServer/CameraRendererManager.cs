using System;
using System.Collections.Generic;
using System.Diagnostics;
using CompanionServer.Cameras;
using Facepunch;
using Facepunch.Extend;

namespace CompanionServer
{
	// Token: 0x020009E8 RID: 2536
	public class CameraRendererManager : SingletonComponent<CameraRendererManager>
	{
		// Token: 0x06003C74 RID: 15476 RVA: 0x00163090 File Offset: 0x00161290
		protected override void OnDestroy()
		{
			base.OnDestroy();
			foreach (CameraRenderer cameraRenderer in this._renderers)
			{
				cameraRenderer.Reset();
			}
			this._renderers.Clear();
			CameraRenderTask.FreeCachedSamplePositions();
			while (this._taskPool.Count > 0)
			{
				this._taskPool.Pop().Dispose();
			}
		}

		// Token: 0x06003C75 RID: 15477 RVA: 0x00163118 File Offset: 0x00161318
		public void StartRendering(IRemoteControllable rc)
		{
			if (rc == null || rc.IsUnityNull<IRemoteControllable>())
			{
				throw new ArgumentNullException("rc");
			}
			if (this._renderers.FindWith((CameraRenderer r) => r.rc, rc, null) == null)
			{
				CameraRenderer cameraRenderer = Pool.Get<CameraRenderer>();
				this._renderers.Add(cameraRenderer);
				cameraRenderer.Init(rc);
			}
		}

		// Token: 0x06003C76 RID: 15478 RVA: 0x00163182 File Offset: 0x00161382
		public void Tick()
		{
			if (!CameraRenderer.enabled)
			{
				return;
			}
			this.DispatchRenderers();
			this.CompleteRenderers();
			this.CleanupRenderers();
		}

		// Token: 0x06003C77 RID: 15479 RVA: 0x0016319E File Offset: 0x0016139E
		public CameraRenderTask BorrowTask()
		{
			if (this._taskPool.Count > 0)
			{
				this._tasksTaken++;
				return this._taskPool.Pop();
			}
			this._tasksCreated++;
			return new CameraRenderTask();
		}

		// Token: 0x06003C78 RID: 15480 RVA: 0x001631DB File Offset: 0x001613DB
		public void ReturnTask(ref CameraRenderTask task)
		{
			if (task == null)
			{
				return;
			}
			task.Reset();
			this._tasksReturned++;
			this._taskPool.Push(task);
			task = null;
		}

		// Token: 0x06003C79 RID: 15481 RVA: 0x00163208 File Offset: 0x00161408
		[ServerVar]
		public static void pool_stats(ConsoleSystem.Arg arg)
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				arg.ReplyWith("Camera renderer manager is null!");
				return;
			}
			arg.ReplyWith(string.Format("Active renderers: {0}\nTasks in pool: {1}\nTasks taken: {2}\nTasks returned: {3}\nTasks created: {4}", new object[]
			{
				instance._renderers.Count,
				instance._taskPool.Count,
				instance._tasksTaken,
				instance._tasksReturned,
				instance._tasksCreated
			}));
		}

		// Token: 0x06003C7A RID: 15482 RVA: 0x00163298 File Offset: 0x00161498
		private void DispatchRenderers()
		{
			List<CameraRenderer> list = Pool.GetList<CameraRenderer>();
			int count = this._renderers.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._renderIndex >= count)
				{
					this._renderIndex = 0;
				}
				List<CameraRenderer> renderers = this._renderers;
				int renderIndex = this._renderIndex;
				this._renderIndex = renderIndex + 1;
				CameraRenderer cameraRenderer = renderers[renderIndex];
				if (cameraRenderer.CanRender())
				{
					list.Add(cameraRenderer);
					if (list.Count >= CameraRenderer.maxRendersPerFrame)
					{
						break;
					}
				}
			}
			if (list.Count > 0)
			{
				int num = CameraRenderer.maxRaysPerFrame / list.Count;
				foreach (CameraRenderer cameraRenderer2 in list)
				{
					cameraRenderer2.Render(num);
				}
			}
			Pool.FreeList<CameraRenderer>(ref list);
		}

		// Token: 0x06003C7B RID: 15483 RVA: 0x00163370 File Offset: 0x00161570
		private void CompleteRenderers()
		{
			this._stopwatch.Restart();
			int count = this._renderers.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._completeIndex >= count)
				{
					this._completeIndex = 0;
				}
				List<CameraRenderer> renderers = this._renderers;
				int completeIndex = this._completeIndex;
				this._completeIndex = completeIndex + 1;
				CameraRenderer cameraRenderer = renderers[completeIndex];
				if (cameraRenderer.state == CameraRendererState.Rendering)
				{
					cameraRenderer.CompleteRender();
					if (this._stopwatch.Elapsed.TotalMilliseconds >= (double)CameraRenderer.completionFrameBudgetMs)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06003C7C RID: 15484 RVA: 0x001633F8 File Offset: 0x001615F8
		private void CleanupRenderers()
		{
			List<CameraRenderer> list = Pool.GetList<CameraRenderer>();
			foreach (CameraRenderer cameraRenderer in this._renderers)
			{
				if (cameraRenderer.state == CameraRendererState.Invalid)
				{
					list.Add(cameraRenderer);
				}
			}
			this._renderers.RemoveAll((CameraRenderer r) => r.state == CameraRendererState.Invalid);
			foreach (CameraRenderer cameraRenderer2 in list)
			{
				Pool.Free<CameraRenderer>(ref cameraRenderer2);
			}
			Pool.FreeList<CameraRenderer>(ref list);
		}

		// Token: 0x040036F2 RID: 14066
		private readonly Stack<CameraRenderTask> _taskPool = new Stack<CameraRenderTask>();

		// Token: 0x040036F3 RID: 14067
		private int _tasksTaken;

		// Token: 0x040036F4 RID: 14068
		private int _tasksReturned;

		// Token: 0x040036F5 RID: 14069
		private int _tasksCreated;

		// Token: 0x040036F6 RID: 14070
		private readonly Stopwatch _stopwatch = new Stopwatch();

		// Token: 0x040036F7 RID: 14071
		private readonly List<CameraRenderer> _renderers = new List<CameraRenderer>();

		// Token: 0x040036F8 RID: 14072
		private int _renderIndex;

		// Token: 0x040036F9 RID: 14073
		private int _completeIndex;
	}
}
