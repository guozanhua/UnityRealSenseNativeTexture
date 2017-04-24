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

public class CameraRGBViewer : MonoBehaviour
{
	public SenseInput m_senseInput;
	private Texture2D m_texColor;
	private PXCMImage m_sample;
	private bool m_shuttingDown = false;
	private BlockProfiler mBlockProfiler = new BlockProfiler();

	private void Start()
	{
		m_senseInput.m_OnSample += OnSample;
		m_senseInput.m_ShutDown += OnShutdown;

		m_texColor = new Texture2D(m_senseInput.m_colorWidth, m_senseInput.m_colorHeight, TextureFormat.RGBA32, false);
		GetComponent<Renderer>().material.SetTexture("mainTex", m_texColor);
	}

	private void Update()
	{
		PXCMImage sample = null;
		lock (this)
		{
			if (m_sample == null) return;
			sample = m_sample;
			m_sample = null;
		}

		// display the color image
		PXCMImage.ImageData data;
		pxcmStatus sts = sample.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32, out data);
		if (sts >= pxcmStatus.PXCM_STATUS_NO_ERROR)
		{
			mBlockProfiler.BeginBlock();
			data.ToTexture2D(0, m_texColor);
			mBlockProfiler.EndBlock();
			sample.ReleaseAccess(data);
			m_texColor.Apply();
		}

		sample.Dispose();
	}

	public float GetToTextureTime()
	{
		return mBlockProfiler.AverageSeconds;
	}

	private void OnSample(PXCMCapture.Sample sample)
	{
		if (m_shuttingDown) return;

		lock (this)
		{
			if (m_sample != null) m_sample.Dispose();
			m_sample = sample.color;
			m_sample.QueryInstance<PXCMAddRef>().AddRef();
		}
	}

	private void OnDisable()
	{
		lock (this)
		{
			if (m_sample != null)
			{
				m_sample.Dispose();
				m_sample = null;
			}
		}
	}

	private void OnShutdown()
	{
		m_shuttingDown = true;

		lock (this)
		{
			if (m_sample != null)
			{
				m_sample.Dispose();
				m_sample = null;
			}
		}
	}
}