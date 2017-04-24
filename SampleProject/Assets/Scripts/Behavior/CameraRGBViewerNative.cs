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

using UnityEngine;

public class CameraRGBViewerNative : MonoBehaviour
{
	public SenseInput m_senseInput;

	private Texture2D m_texColor;
	private System.IntPtr m_texColorNative;

	private bool shuttingDown = false;

	private void Start()
	{
		// Get events when a new image comes in, or when RealSense is shutdown
		m_senseInput.m_OnSample += OnSampleCallback;
		m_senseInput.m_ShutDown += OnShutdownCallback;

		// Create a custom texture and bind it to the material
		m_texColor = new Texture2D(m_senseInput.m_colorWidth, m_senseInput.m_colorHeight, TextureFormat.RGBA32, false);
		m_texColorNative = m_texColor.GetNativeTexturePtr();
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.SetTexture("mainTex", m_texColor);
	}

	private void OnSampleCallback(PXCMCapture.Sample sample)
	{
		if (shuttingDown) return;

		UseTexturePlugin.CopyPXCImageToTexture(sample.color, m_texColorNative);
	}

	private void OnShutdownCallback()
	{
		shuttingDown = true;
		UseTexturePlugin.RealSenseShutdown();
	}
}