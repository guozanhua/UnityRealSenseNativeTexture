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
using System.Diagnostics;

public class BlockProfiler
{
	private int[] mEntries;
	private int mIndex = 0;
	private int mEntryCount = 0;
	private long mSum = 0;
	private bool mInBlock = false;

	private Stopwatch mStopwatch = new Stopwatch();

	public BlockProfiler(int size = 128)
	{
		mEntries = new int[size];
	}

	public void BeginBlock()
	{
		mInBlock = true;
		mStopwatch.Start();
	}

	public float AverageMicroseconds { get { return (mEntryCount > 0) ? (mSum / mEntryCount) : 0; } }
	public float AverageMilliseconds { get { return AverageMicroseconds / 1000.0f; } }
	public float AverageSeconds { get { return AverageMicroseconds / 1000000.0f; } }

	public void EndBlock()
	{
		if (mInBlock)
		{
			mStopwatch.Stop();

			int elapsedUS = (int)(mStopwatch.Elapsed.TotalMilliseconds * 1000);

			mSum = mSum - mEntries[mIndex] + elapsedUS;
			mEntries[mIndex] = elapsedUS;
			mIndex = (mIndex + 1) % mEntries.Length;
			mEntryCount = Math.Min(mEntryCount + 1, mEntries.Length);

			mStopwatch.Reset();
			mInBlock = false;
		}
	}
}