/////////////////////////////////////////////////////////////////////////////////////////////
// Copyright 2017 Intel Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class UseTexturePlugin : MonoBehaviour
{

	[DllImport("NativeRSTextureCopy")]
	private static extern void CopyTextureData(System.IntPtr srcPXCImage, System.IntPtr dstTexture);

	[DllImport("NativeRSTextureCopy")]
	private static extern IntPtr GetRenderEventFunc();

	[DllImport("NativeRSTextureCopy")]
	private static extern void FlushQueuedCopies();

	public static void CopyPXCImageToTexture(PXCMImage srcImage, System.IntPtr dstTexture)
	{
		if (srcImage != null && dstTexture != System.IntPtr.Zero )
		{
			CopyTextureData(srcImage.QueryNativePointer(), dstTexture);
		}
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			GL.IssuePluginEvent(GetRenderEventFunc(), 1);
		}
	}

	public static void RealSenseShutdown()
	{
		FlushQueuedCopies();
	}
}