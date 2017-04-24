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
#pragma once
#include "IUnityInterface.h"
#ifndef __cplusplus
	#include <stdbool.h>
#endif

// Should only be used on the rendering thread unless noted otherwise.
UNITY_DECLARE_INTERFACE(IUnityGraphicsD3D12)
{
	ID3D12Device* (UNITY_INTERFACE_API * GetDevice)();
	ID3D12CommandQueue* (UNITY_INTERFACE_API * GetCommandQueue)();

	ID3D12Fence* (UNITY_INTERFACE_API * GetFrameFence)();
	// Returns the value set on the frame fence once the current frame completes
	UINT64 (UNITY_INTERFACE_API * GetNextFrameFenceValue)();

	// Returns the state a resource will be in after the last command list is executed
	bool (UNITY_INTERFACE_API * GetResourceState)(ID3D12Resource* resource, D3D12_RESOURCE_STATES* outState);
	// Specifies the state a resource will be in after a plugin command list with resource barriers is executed
	void (UNITY_INTERFACE_API * SetResourceState)(ID3D12Resource* resource, D3D12_RESOURCE_STATES state);
};
UNITY_REGISTER_INTERFACE_GUID(0xEF4CEC88A45F4C4CULL,0xBD295B6F2A38D9DEULL,IUnityGraphicsD3D12)
