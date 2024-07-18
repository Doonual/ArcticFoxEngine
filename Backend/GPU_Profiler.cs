using ArcticFoxEngine.Debug;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ArcticFoxEngine.Backend {
	public static class GPU_Profiler {

		private static long timestampFrequency;

		private static long frameStart;
		private static long frameEnd;

		private static long prevGpuTimestamp;

		private static double deltaTime;
		private static double frameTime;

		internal static void GpuTimestampFrameStart(long gpuTimestamp) {
			frameStart = gpuTimestamp;
		}
		internal static void GpuTimestampFrameEnd(long gpuTimestamp) {
			
			frameEnd = gpuTimestamp;
			frameTime = (frameEnd - frameStart) / (double)timestampFrequency;
			DebugPerformance.UpdateVals();

		}

		internal static void UpdateGpuTimestamp(long gpuTimestamp, long timestampFrequency) {

			GPU_Profiler.timestampFrequency = timestampFrequency;

			deltaTime = (gpuTimestamp - prevGpuTimestamp) / (double)timestampFrequency;
			prevGpuTimestamp = gpuTimestamp;
			

		}

		public static double GetFrameTime() {
			return frameTime;
		}

		public static double GetDeltaTime() {
			return deltaTime;
		}

	}
}
