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
using UnityEngine.UI;

public class SenseInput : MonoBehaviour
{
	public Text resolutionText;

	public int m_colorWidth = 320;
	public int m_colorHeight = 240;
	public int m_fps = 30;

	private bool hasInitFailed = false;
	private bool m_exitFlag = false;

	public delegate void OnSampleDelegate(PXCMCapture.Sample sample);
	public event OnSampleDelegate m_OnSample = null;

	public delegate void OnShutDownDelegate();
	public event OnShutDownDelegate m_ShutDown = null;

	private PXCMSenseManager m_senseManager = null;
	private pxcmStatus m_status = pxcmStatus.PXCM_STATUS_INIT_FAILED;

	public bool IsStreaming { get { return m_status >= pxcmStatus.PXCM_STATUS_NO_ERROR; } }

	private pxcmStatus OnNewSample(int mid, PXCMCapture.Sample sample)
	{
		if (m_OnSample != null)
		{
			m_OnSample(sample);
		}
		return pxcmStatus.PXCM_STATUS_NO_ERROR;
	}

	private void OnStatus(int mid, pxcmStatus sts)
	{
		// Camera failed or disconnected
		if (sts == pxcmStatus.PXCM_STATUS_ITEM_UNAVAILABLE || sts == pxcmStatus.PXCM_STATUS_EXEC_ABORTED)
		{
			m_exitFlag = true;
		}
	}

	private void InitializeStreaming()
	{
		PXCMSenseManager.Handler handler = new PXCMSenseManager.Handler();
		handler.onNewSample = OnNewSample;
		handler.onStatus = OnStatus;
		m_status = m_senseManager.Init(handler);
		if (m_status < pxcmStatus.PXCM_STATUS_NO_ERROR)
		{
			Debug.Log("Init Failed; " + m_status);
			hasInitFailed = true;
			return;
		}

		// Start streaming
		m_status = m_senseManager.StreamFrames(false);
		if (m_status < pxcmStatus.PXCM_STATUS_NO_ERROR)
		{
			Debug.Log("StreamFrames Failed; " + m_status);
			hasInitFailed = true;
		}
	}

	private void Start()
	{
		resolutionText.text = "Resolution: " + m_colorWidth + "x" + m_colorHeight;

		if (m_senseManager == null)
		{
			// Create a SenseManager instance
			m_senseManager = PXCMSenseManager.CreateInstance();
			if (m_senseManager == null)
			{
				Debug.Log("SenseManager Instance Failed");
				return;
			}

			// Enable color stream only
			PXCMVideoModule.DataDesc ddesc = new PXCMVideoModule.DataDesc();
			ddesc.streams.color.sizeMin.width = ddesc.streams.color.sizeMax.width = m_colorWidth;
			ddesc.streams.color.sizeMin.height = ddesc.streams.color.sizeMax.height = m_colorHeight;
			ddesc.streams.color.frameRate.min = ddesc.streams.color.frameRate.max = m_fps;
			m_senseManager.EnableStreams(ddesc);
		}
	}

	private void Update()
	{
		if (m_exitFlag)
		{
			Application.Quit();
		}
		// lazy initialize streaming. This ensure that the events don't start firing until all objects have
		// complete their Start initiaition.
		if (m_senseManager != null && !IsStreaming && !hasInitFailed)
		{
			InitializeStreaming();
		}

	}

	private void OnDisable()
	{
		if (m_ShutDown != null)
		{
			m_ShutDown();
		}

		if (m_senseManager != null)
		{
			m_senseManager.Close();
			m_senseManager.Dispose();
			m_senseManager = null;
		}
	}

	private void OnGUI()
	{
		if (!IsStreaming)
		{
			GUI.skin.box.hover.textColor =
				GUI.skin.box.normal.textColor =
					GUI.skin.box.active.textColor = Color.green;
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;

			GUI.Box(new Rect(5, Screen.height - 35, 100, 30), "Setup Failed");
		}
	}
}