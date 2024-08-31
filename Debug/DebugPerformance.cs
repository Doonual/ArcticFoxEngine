using ImGuiNET;
using CoolClassLibrary;
using ArcticFoxEngine.Backend;
using System.Linq;
using Newtonsoft.Json.Linq;
using SharpDX.Direct3D12;
using ArcticFoxEngine.Backend.Render;
using System;

namespace ArcticFoxEngine.Debug {
	internal class DebugPerformance : DebugWindow {

		internal class Metric {

			internal Metric parentMetric;
			List<Metric> subMetrics;

			static float palettePos;
			static float[] redParams = { 0.5f, 0.5f, 1.0f, 0.8f };
			static float[] greenParams = { 0.5f, 0.5f, 1.0f, 0.9f };
			static float[] blueParams = { 0.5f, 0.5f, 0.5f, 0.3f };


			internal string name;
			internal Color col;

			internal float[] startMs;
			internal float[] endMs;

			internal Metric(string name) {


				subMetrics = new List<Metric>();

				this.name = name;
				startMs = new float[2000];
				endMs = new float[2000];

				float red = redParams[0] + redParams[1] * MathF.Cos(2f * MathF.PI * (redParams[2] * palettePos + redParams[3]));
				float green = greenParams[0] + greenParams[1] * MathF.Cos(2f * MathF.PI * (greenParams[2] * palettePos + greenParams[3]));
				float blue = blueParams[0] + blueParams[1] * MathF.Cos(2f * MathF.PI * (blueParams[2] * palettePos + blueParams[3]));
				palettePos += 0.21f;

				col = new Color(red, green, blue);
				if (name == "Untracked") {
					col = new Color(170, 170, 170);
				}

			}
			internal Metric(Metric parentMetric, string name) {

				this.parentMetric = parentMetric;
				subMetrics = new List<Metric>();

				this.name = name;
				startMs = new float[2000];
				endMs = new float[2000];

				float red = redParams[0] + redParams[1] * MathF.Cos(2f * MathF.PI * (redParams[2] * palettePos + redParams[3]));
				float green = greenParams[0] + greenParams[1] * MathF.Cos(2f * MathF.PI * (greenParams[2] * palettePos + greenParams[3]));
				float blue = blueParams[0] + blueParams[1] * MathF.Cos(2f * MathF.PI * (blueParams[2] * palettePos + blueParams[3]));
				palettePos += 0.13f;

				col = new Color(red, green, blue);
				if (name == "Untracked") {
					col = new Color(170, 170, 170);
				}
			}


			internal void NewFrame() {
				for (int i = 0; i < startMs.Length - 1; i++) {
					startMs[i] = startMs[i + 1];
				}
				startMs[startMs.Length - 1] = 0;

				for (int i = 0; i < endMs.Length - 1; i++) {
					endMs[i] = endMs[i + 1];
				}
				endMs[endMs.Length - 1] = 0;

				for (int i = 0; i < subMetrics.Count; i ++) {
					subMetrics[i].NewFrame();
				}

			}
			/*
			internal float[] GetPlottable(int numSamples) {
				return SquishPlot(vals, numSamples);
			}
			internal float[] GetPlottable(float[] startVals) {
				
				float[] plotTimes = SquishPlot(vals, startVals.Length);
				for (int i = 0; i < plotTimes.Length; i ++) {
					plotTimes[i] += startVals[i];
				}
				return plotTimes;

			}
			
			internal float GetLast() {
				return vals[vals.Length - 1];
			}
			*/
			private float[] SquishPlot(float[] inVals, int targetNumVals) {

				float[] outVals = new float[targetNumVals];
				for (int i = 0; i < outVals.Length; i++) {
					outVals[i] = 0f;
				}
				float averageCount = (float)inVals.Length / targetNumVals;
				int currentDestIndex = 0;
				float destRemaining = averageCount;
				for (int i = 0; i < inVals.Length; i++) {

					float srcRemaining = 1f;

					while (srcRemaining > 0f && currentDestIndex < targetNumVals) {
						if (destRemaining > 0f) {

							float amntTransfer = MathF.Min(srcRemaining, destRemaining);
							outVals[currentDestIndex] += inVals[i] * amntTransfer / averageCount;
							srcRemaining -= amntTransfer;
							destRemaining -= amntTransfer;
						}
						else {
							currentDestIndex += 1;
							destRemaining = averageCount;
						}
					}
				}
				return outVals;

			}

			internal void DrawMetric(Vector2 topLeft, float width, float totalMs) {

				ImGui.PushID(name + "metric total");
				float start = startMs.Last() / totalMs;
				float end = endMs.Last() / totalMs;

				float ms = (endMs.Last() - startMs.Last());

				Vector2 buttonSize = new Vector2((end - start) * width, 20f);
				ImGui.SetCursorPos(topLeft + Vector2.right * start * width);
				Vector2 buttonCursorStartPos = ImGui.GetCursorPos();

				ImGui.ColorButton(name + " metric", (Vector4)col, ImGuiColorEditFlags.NoTooltip, buttonSize);
				

				ImGui.SetCursorPos(buttonCursorStartPos);
				ImGui.BeginChild(name + "metric text", buttonSize);
				ImGui.SetCursorPos(buttonSize / 2f - (Vector2)ImGui.CalcTextSize(name) * new Vector2(0.5f, 0.5f));
				ImGui.Text(name);
				ImGui.EndChild();
				if (ImGui.IsItemHovered() == true) {
					ImGui.BeginTooltip();
					ImGui.Text(name);
					ImGui.Text(ms.ToString("F3") + " ms");
					ImGui.EndTooltip();
				}

				for (int i = 0; i < subMetrics.Count; i ++) {
					subMetrics[i].DrawMetric(topLeft - Vector2.down * 20f, width, totalMs);
				}
				
				ImGui.PopID();

			}
			internal void DrawMetricHist(int numSamples, Vector2 minCoord, Vector2 maxCoord, float maxMs) {

				float[] startMsSquished = SquishPlot(startMs, numSamples);
				float[] endMsSquished = SquishPlot(endMs, numSamples);

				for (int i = 0; i < numSamples; i ++) {
					float currentX = (float)i / numSamples;
					currentX *= (maxCoord.x - minCoord.x);
					currentX += minCoord.x;

					float startHeight = startMsSquished[i];
					startHeight /= maxMs;
					if (startHeight >= 1f) { continue; }
					startHeight *= minCoord.y - maxCoord.y;
					startHeight += maxCoord.y;

					float endHeight = endMsSquished[i];
					endHeight /= maxMs;
					endHeight = MathF.Min(endHeight, 1f);
					endHeight *= minCoord.y - maxCoord.y;
					endHeight += maxCoord.y;

					ImGui.GetWindowDrawList().AddLine(new Vector2(currentX, startHeight), new Vector2(currentX, endHeight), ImGui.ColorConvertFloat4ToU32((Vector4)col));
				}
				
				for (int i = 0; i < subMetrics.Count; i ++) {
					subMetrics[i].DrawMetricHist(numSamples, minCoord, maxCoord, maxMs);
				}


			}

			internal Metric GetOrCreateChildMetric(string name) {

				for (int i = 0; i < subMetrics.Count; i ++) {
					if (subMetrics[i].name == name) {
						return subMetrics[i];
					}
				}
				Metric newMetric = new Metric(this, name);
				subMetrics.Add(newMetric);
				return newMetric;

			}

		}

		long frameStartTimestamp;
		float lastFrameTime = 0f;
		float msMax = 0.0f;
		float msMaxView = 0.0f;
		float msMin = 0.0f;
		float msMinView = 0.0f;

		internal Metric metric;
		private static Metric currentMetric;

		int numElements;
		float plotMaxMs = 10f;

		bool updatePlot = true;
		bool updatePlotActual = true;
		bool showMoreOptions = true;



		internal override string name => "Performance";

		internal DebugPerformance() {

			numElements = 2000;
			metric = new Metric("Frame time");

		}

		internal void ProcessMetrics() {
			if (updatePlotActual == false) { return; }
			metric.NewFrame();
			metric.startMs[metric.startMs.Length - 1] = 0f;
			currentMetric = metric;
		}
		internal void FrameStart(long timestamp) {
			if (updatePlotActual == false) { return; }

			frameStartTimestamp = timestamp;
			

		}
		internal void FrameDone(long timestamp, float frameTime) {


			if (updatePlotActual == false) {
				updatePlotActual = updatePlot;
				return;
			}
			updatePlotActual = updatePlot;
			lastFrameTime = frameTime * 1000f;

			float ms = frameTime * 1000;
			if (ms > msMax) {
				msMax = ms;
				msMaxView = ms;
			}
			msMax = (msMax - ms) * 0.994f + ms;

			if (ms < msMin) {
				msMin = ms;
				msMinView = ms;
			}
			msMin = (msMin - ms) * 0.994f + ms;


			metric.endMs[metric.endMs.Length - 1] = 1000f * (float)(timestamp - frameStartTimestamp) / Graphics.cmdQueue.TimestampFrequency;

		}

		internal void MetricBegin(long timestamp, string name) {
			if (updatePlotActual == false) { return; }
			currentMetric = currentMetric.GetOrCreateChildMetric(name);
			currentMetric.startMs[metric.startMs.Length - 1] = 1000f * (float)(timestamp - frameStartTimestamp) / Graphics.cmdQueue.TimestampFrequency;
		}
		internal void MetricEnd(long timestamp) {
			if (updatePlotActual == false) { return; }
			currentMetric.endMs[metric.endMs.Length - 1] = 1000f * (float)(timestamp - frameStartTimestamp) / Graphics.cmdQueue.TimestampFrequency;
			currentMetric = currentMetric.parentMetric;
		}

		internal override void Render() {

			
			#region FPS Table

			ImGuiTableFlags flags = ImGuiTableFlags.Borders;
			if (ImGui.BeginTable("Performance Table", 3, flags) == true) {
				
				ImGui.TableSetupColumn("Worst");
				ImGui.TableSetupColumn("Current");
				ImGui.TableSetupColumn("Best");
				ImGui.TableHeadersRow();
				ImGui.TableNextColumn();

				ImGui.Text(msMaxView.ToString("F") + " ms");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
				ImGui.TableNextColumn();
				ImGui.Text(lastFrameTime.ToString("F") + " ms");
				ImGui.TableNextColumn();
				ImGui.Text(msMinView.ToString("F") + " ms");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text((1000.0 / msMaxView).ToString("F") + " fps");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
				ImGui.TableNextColumn();
				ImGui.Text((1000.0 / lastFrameTime).ToString("F") + " fps");
				ImGui.TableNextColumn();
				ImGui.Text((1000.0 / msMinView).ToString("F") + " fps");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));

				ImGui.EndTable();

			}

			


			#endregion

			float preWidth = ImGui.GetColumnWidth();

			Vector2 histStartScreen;
			Vector2 histEndScreen;

			ImGui.Checkbox("Update plot", ref updatePlot);

			#region Histogram plot

			histStartScreen = ImGui.GetCursorScreenPos();
			histStartScreen += (Vector2)ImGui.GetStyle().FramePadding;

			System.Numerics.Vector2 histCurStart = ImGui.GetCursorPos();
			int histWidth = (int)ImGui.GetCursorPosX();

			ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 138f / 255f));
			float dummyPlot = 0f;
			ImGui.SetCursorPos(histCurStart);

			
			ImGui.PlotHistogram("", ref dummyPlot, 1, 0, "", 0f, plotMaxMs, new Vector2(-1f, 150f));

			
			

			ImGui.PopStyleColor();

			ImGui.SameLine();
			histWidth = (int)(ImGui.GetCursorPosX() - histWidth - ImGui.GetStyle().FramePadding.X * 2 - ImGui.GetStyle().ItemSpacing.X);
			histEndScreen = histStartScreen + new Vector2(histWidth, 150 - ImGui.GetStyle().FramePadding.Y * 2);
			numElements = Math.Max(1, histWidth);

			metric.DrawMetricHist(numElements, histStartScreen, histEndScreen, plotMaxMs);

			if (ImGui.GetMousePos().X > histStartScreen.x && ImGui.GetMousePos().X < histEndScreen.x && ImGui.GetMousePos().Y > histStartScreen.y && ImGui.GetMousePos().Y < histEndScreen.y) {
				plotMaxMs *= MathF.Exp(ImGui.GetIO().MouseWheel * -0.2f);
				plotMaxMs = MathF.Min(MathF.Max(0.01f, plotMaxMs), 80f);
			}

			#endregion
			#region Draw ms lines on plot

			float[] msLines = new float[] {0.1f, 0.25f, 0.5f, 1f, 2f, 4.17f, 8.33f, 16.67f, 33.33f, 50f, 100f, 1000f };
			for (int i = 0; i < msLines.Length; i ++) {

				float height = MathUtil.Map(msLines[i], 0f, plotMaxMs, histEndScreen.y, histStartScreen.y);
				
				if (msLines[i] > plotMaxMs) { break; }
				if (msLines[i] / plotMaxMs < 0.1) { continue; }

				string msReadout = msLines[i].ToString("F1") + " ms";
				string fpsReadout = MathF.Round(1000f / msLines[i]).ToString("F0") + " FPS";


				
				Vector2 msTextAlignment = new Vector2(-ImGui.CalcTextSize(msReadout).X, -ImGui.CalcTextSize(msReadout).Y / 2f);
				Vector2 fpsTextAlignment = new Vector2(0f, -ImGui.CalcTextSize(fpsReadout).Y / 2f);

				float msReadoutAlpha = 0.4f;

				ImGui.GetWindowDrawList().AddLine(new Vector2(histStartScreen.x + ImGui.CalcTextSize(fpsReadout).X + 5, height), new Vector2(histEndScreen.x - ImGui.CalcTextSize(msReadout).X - 5, height), ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, msReadoutAlpha)));
				ImGui.GetWindowDrawList().AddText(new Vector2(histEndScreen.x, height) + msTextAlignment, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, msReadoutAlpha)), msReadout);
				ImGui.GetWindowDrawList().AddText(new Vector2(histStartScreen.x, height) + fpsTextAlignment, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, msReadoutAlpha)), fpsReadout);

				

			}

			#endregion

			ImGui.NewLine();

			ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 138f / 255f));
			ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0f, 0f));
			ImGui.BeginChildFrame((uint)"metric recursive child".GetHashCode(), new Vector2(-1f, -1f));
			

			Vector2 childStart = ImGui.GetCursorPos();
			float width = ImGui.GetColumnWidth();

			metric.DrawMetric(childStart, width, metric.endMs.Last());

			ImGui.EndChildFrame();
			ImGui.PopStyleVar();
			ImGui.PopStyleColor();



			

		}

	}

	
}
