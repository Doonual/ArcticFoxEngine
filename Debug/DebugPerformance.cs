using ImGuiNET;
using CoolClassLibrary;
using ArcticFoxEngine.Backend;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ArcticFoxEngine.Debug {
	internal class DebugPerformance : DebugWindow {

		private class Metric {

			static float palettePos = 0f;
			static float[] redParams = { 0.5f, 0.5f, 1.0f, 0.8f };
			static float[] greenParams = { 0.5f, 0.5f, 1.0f, 0.9f };
			static float[] blueParams = { 0.5f, 0.5f, 0.5f, 0.3f };


			internal string name;
			float[] vals;
			internal Color col;

			internal Metric(string name) {

				this.name = name;
				vals = new float[2000];

				float red = redParams[0] + redParams[1] * MathF.Cos(2f * MathF.PI * (redParams[2] * palettePos + redParams[3]));
				float green = greenParams[0] + greenParams[1] * MathF.Cos(2f * MathF.PI * (greenParams[2] * palettePos + greenParams[3]));
				float blue = blueParams[0] + blueParams[1] * MathF.Cos(2f * MathF.PI * (blueParams[2] * palettePos + blueParams[3]));
				palettePos += 0.24f;

				col = new Color(red, green, blue);
				if (name == "Untracked") {
					col = new Color(170, 170, 170);
				}

			}

			internal void UpdateTime(float time) {
				vals[vals.Length - 1] = time;
			}
			internal void NewFrame() {
				for (int i = 0; i < vals.Length - 1; i++) {
					vals[i] = vals[i + 1];
				}
				vals[vals.Length - 1] = 0f;
			}

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



		}

		float lastFrameTime = 0f;
		float msMax = 0.0f;
		float msMaxView = 0.0f;
		float msMin = 0.0f;
		float msMinView = 0.0f;

		List<Metric> metrics;

		// First element is the index of the first metric
		// Last element is the index of the last metric
		List<int> naturalOrder;
		List<int> timeOrder;
		List<int> metricDrawOrder;
		int prevSortColumn = -1;
		ImGuiSortDirection prevSortDirection = ImGuiSortDirection.None;

		int numElements;
		float plotMaxMs = 10f;

		bool updatePlot = true;
		bool updatePlotActual = true;
		bool showMoreOptions = true;



		internal override string name => "Performance";

		internal DebugPerformance() {

			numElements = 2000;
			metrics = new List<Metric>();

			naturalOrder = new List<int>();
			timeOrder = new List<int>();
			metricDrawOrder = new List<int>();

		}

		internal void UpdateVal(string metric, float value) {

			if (updatePlotActual == false) { return; }

			bool foundTimeOrder;

			for (int i = 0; i < metrics.Count; i ++) {
				if (metrics[i].name == metric) {
					metrics[i].UpdateTime(value * 1000);

					naturalOrder.Add(i);

					foundTimeOrder = false;
					for (int n = 0; n < timeOrder.Count; n ++) {
						if (metrics[timeOrder[n]].GetLast() > value * 1000f) {
							timeOrder.Insert(n, i);
							foundTimeOrder = true;
							break;
						}
					}
					if (foundTimeOrder == false) {
						timeOrder.Add(i);
					}

					return;
				}
			}
			Metric newMetric = new Metric(metric);
			newMetric.UpdateTime(value * 1000);
			metrics.Add(newMetric);
			naturalOrder.Add(metrics.Count - 1);

			foundTimeOrder = false;
			for (int n = 0; n < timeOrder.Count; n++) {
				if (metrics[timeOrder[n]].GetLast() > value * 1000f) {
					timeOrder.Insert(n, metrics.Count - 1);
					foundTimeOrder = true;
					break;
				}
			}
			if (foundTimeOrder == false) {
				timeOrder.Add(metrics.Count - 1);
			}

		}

		internal void FrameBegin() {

			if (updatePlotActual == false) { return; }
			naturalOrder.Clear();
			timeOrder.Clear();
			for (int i = 0; i < metrics.Count; i ++) {
				metrics[i].NewFrame();
			}

		}

		internal void FrameDone(float frameTime) {

			float untrackedTime = frameTime;
			for (int i = 0; i < metrics.Count; i++) {
				untrackedTime -= metrics[i].GetLast() / 1000f;
			}
			UpdateVal("Untracked", untrackedTime);

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


			#region Histogram plot

			histStartScreen = ImGui.GetCursorScreenPos();
			histStartScreen += (Vector2)ImGui.GetStyle().FramePadding;

			System.Numerics.Vector2 histCurStart = ImGui.GetCursorPos();
			int histWidth = (int)ImGui.GetCursorPosX();

			float[][] plotVals = new float[metricDrawOrder.Count][];
			for (int i = 0; i < metricDrawOrder.Count; i ++) {



				if (i == 0) {
					plotVals[i] = metrics[metricDrawOrder[i]].GetPlottable(numElements);
				}
				else {
					plotVals[i] = metrics[metricDrawOrder[i]].GetPlottable(plotVals[i - 1]);
				}

			}
			for (int i = metricDrawOrder.Count - 1; i >= 0; i --) {

				Vector4 plotCol = metrics[metricDrawOrder[i]].col;
				
				ImGui.PushStyleColor(ImGuiCol.PlotHistogram, plotCol); // Set the colour of the current histogram
				// Remove background if not first
				if (i == metricDrawOrder.Count - 1) {
					ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 138f / 255f));
				}
				else {
					ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0f, 0f, 0f, 0f));
				}
				ImGui.SetCursorPos(histCurStart);

				// Plot
				ImGui.PlotHistogram("", ref plotVals[i][0], numElements, 0, "", 0f, plotMaxMs, new Vector2(-1f, 150f));

				ImGui.PopStyleColor();
				ImGui.PopStyleColor();

			}

			ImGui.SameLine();
			histWidth = (int)(ImGui.GetCursorPosX() - histWidth - ImGui.GetStyle().FramePadding.X * 2 - ImGui.GetStyle().ItemSpacing.X);
			histEndScreen = histStartScreen + new Vector2(histWidth, 150 - ImGui.GetStyle().FramePadding.Y * 2);
			numElements = Math.Max(1, histWidth);

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
			ImGui.Checkbox("Update plot", ref updatePlot);
			ImGui.SameLine();
			ImGui.Checkbox("More options", ref showMoreOptions);

			if (showMoreOptions == true) {

				ImGui.PushID("Performance breakdown pie");

				#region Pie chart

				Vector2 circleCenter = (Vector2)ImGui.GetCursorScreenPos() + new Vector2(100f, 100f);
				ImGui.BeginChild((uint)"Performance pie child".GetHashCode(), new System.Numerics.Vector2(200f, 200f));

				float circleRadius = 100f;
				float pieStart = 0f;
				for (int i = 0; i < metricDrawOrder.Count(); i ++) {

					int currentMetricIndex = metricDrawOrder[i];

					float pieEnd = pieStart + (metrics[currentMetricIndex].GetLast() / lastFrameTime);
					int segments = (int)((pieEnd - pieStart) * 64);
					segments = Math.Max(segments, 3);

					uint currentCol = ImGui.ColorConvertFloat4ToU32((Vector4)metrics[currentMetricIndex].col);

					for (int n = 0; n < segments; n ++) {
						float currentPieStart = MathUtil.Lerp((float)n / segments, pieStart, pieEnd);
						float currentPieEnd = MathUtil.Lerp(((float)n + 1.1f) / segments, pieStart, pieEnd);

						currentPieStart *= MathF.PI * 2f;
						currentPieEnd *= MathF.PI * 2f;

						ImGui.GetWindowDrawList().AddTriangleFilled(circleCenter - Vector2.Angle(currentPieStart - MathF.PI / 2f, 0f), circleCenter + Vector2.Angle(currentPieStart - MathF.PI / 2f, circleRadius), circleCenter + Vector2.Angle(currentPieEnd - MathF.PI / 2f, circleRadius), currentCol);
					}
					pieStart = pieEnd;

				}
				ImGui.GetWindowDrawList().AddCircle(circleCenter, circleRadius, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)));


				ImGui.EndChild();


				#endregion

				ImGui.SameLine();

				ImGuiTableFlags tableFlags = ImGuiTableFlags.Sortable | ImGuiTableFlags.SortMulti | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersV | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoBordersInBody;
				if (ImGui.BeginTable("Profiler metrics", 4, tableFlags) == true) {
					ImGui.TableSetupColumn("ID", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthFixed);
					ImGui.TableSetupColumn("Col", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.WidthFixed);
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.NoSort);
					ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.DefaultSort);
					ImGui.TableSetupScrollFreeze(0, 1);
					ImGui.TableHeadersRow();

					ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();

					if (prevSortColumn != sortSpecs.Specs.ColumnIndex || prevSortDirection != sortSpecs.Specs.SortDirection) {
						metricDrawOrder.Clear();
						if (sortSpecs.Specs.ColumnIndex == 0) {
							metricDrawOrder.AddRange(naturalOrder);
							if (sortSpecs.Specs.SortDirection == ImGuiSortDirection.Descending) {
								metricDrawOrder.Reverse();
							}
						}
						if (sortSpecs.Specs.ColumnIndex == 3) {
							metricDrawOrder.AddRange(timeOrder);
							if (sortSpecs.Specs.SortDirection == ImGuiSortDirection.Descending) {
								metricDrawOrder.Reverse();
							}
						}
					}
					prevSortColumn = sortSpecs.Specs.ColumnIndex;
					prevSortDirection = sortSpecs.Specs.SortDirection;

					

					for (int i = 0; i < metricDrawOrder.Count; i++) {

						int currentMetricIndex = metricDrawOrder[i];
						
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						ImGui.Text(currentMetricIndex.ToString("00"));
						ImGui.TableNextColumn();

						ImGui.ColorButton(metrics[currentMetricIndex].name + " plot colour", (Vector4)metrics[currentMetricIndex].col, ImGuiColorEditFlags.NoTooltip, new Vector2(15f, 15f));
						ImGui.TableNextColumn();
						ImGui.Text(metrics[currentMetricIndex].name);
						ImGui.TableNextColumn();
						ImGui.Text(metrics[currentMetricIndex].GetLast().ToString("F3") + " ms");

					}

					ImGui.EndTable();
				}
				



				ImGui.PopID();

			}

		}

	}

	
}
