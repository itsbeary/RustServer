using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020008F0 RID: 2288
public class BaseCommandBuffer : MonoBehaviour
{
	// Token: 0x060037A4 RID: 14244 RVA: 0x0014CE20 File Offset: 0x0014B020
	protected CommandBuffer GetCommandBuffer(string name, Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, out dictionary))
		{
			dictionary = new Dictionary<int, CommandBuffer>();
			this.cameras.Add(camera, dictionary);
		}
		CommandBuffer commandBuffer;
		if (dictionary.TryGetValue((int)cameraEvent, out commandBuffer))
		{
			commandBuffer.Clear();
		}
		else
		{
			commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			dictionary.Add((int)cameraEvent, commandBuffer);
			this.CleanupCamera(name, camera, cameraEvent);
			camera.AddCommandBuffer(cameraEvent, commandBuffer);
		}
		return commandBuffer;
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x0014CE8C File Offset: 0x0014B08C
	protected void CleanupCamera(string name, Camera camera, CameraEvent cameraEvent)
	{
		foreach (CommandBuffer commandBuffer in camera.GetCommandBuffers(cameraEvent))
		{
			if (commandBuffer.name == name)
			{
				camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
			}
		}
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x0014CECC File Offset: 0x0014B0CC
	protected void CleanupCommandBuffer(Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, out dictionary))
		{
			return;
		}
		CommandBuffer commandBuffer;
		if (!dictionary.TryGetValue((int)cameraEvent, out commandBuffer))
		{
			return;
		}
		camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0014CF00 File Offset: 0x0014B100
	protected void Cleanup()
	{
		foreach (KeyValuePair<Camera, Dictionary<int, CommandBuffer>> keyValuePair in this.cameras)
		{
			Camera key = keyValuePair.Key;
			Dictionary<int, CommandBuffer> value = keyValuePair.Value;
			if (key)
			{
				foreach (KeyValuePair<int, CommandBuffer> keyValuePair2 in value)
				{
					int key2 = keyValuePair2.Key;
					CommandBuffer value2 = keyValuePair2.Value;
					key.RemoveCommandBuffer((CameraEvent)key2, value2);
				}
			}
		}
	}

	// Token: 0x0400330F RID: 13071
	private Dictionary<Camera, Dictionary<int, CommandBuffer>> cameras = new Dictionary<Camera, Dictionary<int, CommandBuffer>>();
}
