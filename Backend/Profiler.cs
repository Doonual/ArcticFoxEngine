using ArcticFoxEngine.Debug;
using CoolClassLibrary;



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

			PerformanceWindow.mainWindow.ProcessMetrics();
			Graphics.cmdQueueDirect.GetClockCalibration(out long gpuTimestamp, out _);

			frameBegin = gpuTimestamp;
			deltaTime = (gpuTimestamp - prevGpuTimestamp) / (float)Graphics.cmdQueueDirect.TimestampFrequency;
			prevGpuTimestamp = gpuTimestamp;

			PerformanceWindow.mainWindow.FrameStart(gpuTimestamp);

		}

		/// <summary>
		/// Notifies the profiler the current frame has ended. Everything to be profiled must be between FrameBegin and this
		/// </summary>
		internal static void FrameEnd() {

			long gpuTimestamp;
			Graphics.cmdQueueDirect.GetClockCalibration(out gpuTimestamp, out _);

			frameTime = (gpuTimestamp - frameBegin) / (float)Graphics.cmdQueueDirect.TimestampFrequency;

			PerformanceWindow.mainWindow.FrameDone(gpuTimestamp, frameTime);

		}


		/// <summary>
		/// Starts timing a new metric
		/// </summary>
		/// <param name="name">The name of the metric to be profiled</param>
		public static void MetricBegin(string name) {

			Graphics.cmdQueueDirect.GetClockCalibration(out long timestamp, out _);
			PerformanceWindow.mainWindow.MetricBegin(timestamp, name);

		}

		/// <summary>
		/// Stops timing the current metric
		/// </summary>
		public static void MetricEnd() {

			Graphics.cmdQueueDirect.GetClockCalibration(out long timestamp, out _);
			GuiManager.GetDebugWindow<PerformanceWindow>().MetricEnd(timestamp);

		}


	}
}
