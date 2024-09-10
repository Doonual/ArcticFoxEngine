using ArcticFoxEngine.Debug;



namespace ArcticFoxEngine {
	public static class Profiler {


		private static long frameBegin;
		public static float frameTime { get; private set; }

		private static long prevGpuTimestamp;
		public static float deltaTime { get; private set; }


		/// <summary>
		/// Notifies the profiler a new frame has begun. Everything to be profiled must be between this and FrameEnd
		/// </summary>
		internal static void FrameBegin() {

			DebugManager.GetDebugWindow<DebugPerformance>().ProcessMetrics();
			Graphics.cmdQueue.GetClockCalibration(out long gpuTimestamp, out _);

			frameBegin = gpuTimestamp;
			deltaTime = (gpuTimestamp - prevGpuTimestamp) / (float)Graphics.cmdQueue.TimestampFrequency;
			prevGpuTimestamp = gpuTimestamp;

			DebugManager.GetDebugWindow<DebugPerformance>().FrameStart(gpuTimestamp);

		}

		/// <summary>
		/// Notifies the profiler the current frame has ended. Everything to be profiled must be between FrameBegin and this
		/// </summary>
		internal static void FrameEnd() {

			long gpuTimestamp;
			Graphics.cmdQueue.GetClockCalibration(out gpuTimestamp, out _);

			frameTime = (gpuTimestamp - frameBegin) / (float)Graphics.cmdQueue.TimestampFrequency;

			DebugManager.GetDebugWindow<DebugPerformance>().FrameDone(gpuTimestamp, frameTime);

		}


		/// <summary>
		/// Starts timing a new metric
		/// </summary>
		/// <param name="name">The name of the metric to be profiled</param>
		internal static void MetricBegin(string name) {

			Graphics.cmdQueue.GetClockCalibration(out long timestamp, out _);
			DebugManager.GetDebugWindow<DebugPerformance>().MetricBegin(timestamp, name);

		}

		/// <summary>
		/// Stops timing the current metric
		/// </summary>
		internal static void MetricEnd() {

			Graphics.cmdQueue.GetClockCalibration(out long timestamp, out _);
			DebugManager.GetDebugWindow<DebugPerformance>().MetricEnd(timestamp);

		}


	}
}
