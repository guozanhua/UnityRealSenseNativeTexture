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

public class GlobalSettings : MonoBehaviour
{
	public Toggle PluginToggle;
	public CameraRGBViewer rgbViewer;
	public CameraRGBViewerNative rgbViewerNative;
	public Text ToTextureTimeText;
	public Text FPSText;
	private BlockProfiler mFPSCounter = new BlockProfiler(15);

	public bool UsingNativePlugin { get { return PluginToggle.isOn; } }

	private void Start()
	{
		QualitySettings.vSyncCount = 0;
		PluginToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(PluginToggleChanged));
		PluginToggleChanged(PluginToggle.isOn);
	}

	private void PluginToggleChanged(bool value)
	{
		rgbViewer.gameObject.SetActive(!UsingNativePlugin);
		rgbViewerNative.gameObject.SetActive(UsingNativePlugin);
	}

	private void Update()
	{
		mFPSCounter.EndBlock();
		mFPSCounter.BeginBlock();

		float toTextureTime = UsingNativePlugin ? 0.0f : rgbViewer.GetToTextureTime();
		ToTextureTimeText.text = string.Format("<b>ToTexture2D: {0:0.00}ms</b>", toTextureTime * 1000.0f);

		float fps = (mFPSCounter.AverageSeconds > float.Epsilon) ? 1.0f / mFPSCounter.AverageSeconds : 0.0f;
		FPSText.text = string.Format("<b>FPS: {0:0.00}</b>", fps);
	}
}