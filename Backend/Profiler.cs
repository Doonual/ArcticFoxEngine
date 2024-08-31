using ArcticFoxEngine.Debug;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;



namespace ArcticFoxEngine.Backend {
	public static class Profiler {


		private static long frameBegin;
		public static float frameTime { get; private set; }

		private static long prevGpuTimestamp;
		public static float deltaTime { get; private set; }

		internal static void FrameBegin() {

			DebugManager.GetDebugWindow<DebugPerformance>().ProcessMetrics();
			Graphics.cmdQueue.GetClockCalibration(out long gpuTimestamp, out _);
			
			frameBegin = gpuTimestamp;
			deltaTime = (gpuTimestamp - prevGpuTimestamp) / (float)Graphics.cmdQueue.TimestampFrequency;
			prevGpuTimestamp = gpuTimestamp;
			
			DebugManager.GetDebugWindow<DebugPerformance>().FrameStart(gpuTimestamp);

		}
		internal static void FrameEnd() {


			long gpuTimestamp;
			Graphics.cmdQueue.GetClockCalibration(out gpuTimestamp, out _);

			frameTime = (gpuTimestamp - frameBegin) / (float)Graphics.cmdQueue.TimestampFrequency;

			DebugManager.GetDebugWindow<DebugPerformance>().FrameDone(gpuTimestamp, frameTime);
			

		}

		

		internal static void MetricBegin(string name) {

			Graphics.cmdQueue.GetClockCalibration(out long timestamp, out _);
			DebugManager.GetDebugWindow<DebugPerformance>().MetricBegin(timestamp, name);
			

		}
		internal static void MetricEnd() {

			Graphics.cmdQueue.GetClockCalibration(out long timestamp, out _);
			DebugManager.GetDebugWindow<DebugPerformance>().MetricEnd(timestamp);
			

		}


	}
}
