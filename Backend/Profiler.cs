using ArcticFoxEngine.Debug;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ArcticFoxEngine.Backend {
	public static class Profiler {


		private static long frameStart;
		private static long frameEnd;

		private static long prevGpuTimestamp;

		public static float deltaTime { get; private set; }
		public static float frameTime { get; private set; }

		internal static void FrameStart() {

			long gpuTimestamp;
			GPU_Render.cmdQueue.GetClockCalibration(out gpuTimestamp, out _);
			
			frameStart = gpuTimestamp;
			deltaTime = (gpuTimestamp - prevGpuTimestamp) / (float)GPU_Render.cmdQueue.TimestampFrequency;
			prevGpuTimestamp = gpuTimestamp;
		}
		internal static void FrameEnd() {

			long gpuTimestamp;
			GPU_Render.cmdQueue.GetClockCalibration(out gpuTimestamp, out _);

			frameEnd = gpuTimestamp;
			frameTime = (frameEnd - frameStart) / (float)GPU_Render.cmdQueue.TimestampFrequency;
			DebugManager.GetDebugWindow<DebugPerformance>().UpdateVals();

		}

	}
}
