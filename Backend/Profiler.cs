using ArcticFoxEngine.Debug;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ArcticFoxEngine.Backend {
	public static class Profiler {


		private static long frameBegin;
		public static float frameTime { get; private set; }

		private static long prevGpuTimestamp;
		public static float deltaTime { get; private set; }

		private static long metricTimestamp;

		internal static void FrameBegin() {

			long gpuTimestamp;
			GPU_Render.cmdQueue.GetClockCalibration(out gpuTimestamp, out _);
			
			frameBegin = gpuTimestamp;
			deltaTime = (gpuTimestamp - prevGpuTimestamp) / (float)GPU_Render.cmdQueue.TimestampFrequency;
			prevGpuTimestamp = gpuTimestamp;
		}
		internal static void FrameEnd() {

			long gpuTimestamp;
			GPU_Render.cmdQueue.GetClockCalibration(out gpuTimestamp, out _);

			frameTime = (gpuTimestamp - frameBegin) / (float)GPU_Render.cmdQueue.TimestampFrequency;

			DebugManager.GetDebugWindow<DebugPerformance>().FrameDone(frameTime);

		}

		internal static void MetricBegin() {
			GPU_Render.cmdQueue.GetClockCalibration(out metricTimestamp, out _);
		}
		internal static void MetricEnd(string name) {

			long currentTimestamp;
			GPU_Render.cmdQueue.GetClockCalibration(out currentTimestamp, out _);
			float metricTime = (currentTimestamp - metricTimestamp) / (float)GPU_Render.cmdQueue.TimestampFrequency;

			DebugManager.GetDebugWindow<DebugPerformance>().UpdateVal(name, metricTime);

		}


	}
}
