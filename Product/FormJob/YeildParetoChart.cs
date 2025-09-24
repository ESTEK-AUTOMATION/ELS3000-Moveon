using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace Product
{
    public partial class YeildParetoChart : UserControl
    {
        public YeildParetoChart()
        {
            InitializeComponent();
        }

        public void LoadDataToDisplay(string FolderName, string FileName)
        {
            try
            {
                Dictionary<string, int> defectData = ParseDefectData(FolderName + "\\" + FileName);

                if (string.IsNullOrWhiteSpace(FolderName + "\\" + FileName) || !File.Exists(FolderName + "\\" + FileName))
                {
                    MessageBox.Show("File path is invalid or file does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (defectData.Count == 0)
                {
                    MessageBox.Show("No defect data found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Sort defects in descending order
                var sortedDefects = defectData.OrderByDescending(x => x.Value).ToList();
                int totalDefects = sortedDefects.Sum(x => x.Value);

                int totalInputQuantity = CalculateTotalInputQuantity(FolderName + "\\" + FileName);
                double totalYeild = CalculateTotalYeild(FolderName + "\\" + FileName);

                // Calculate cumulative percentage
                List<double> cumulativePercentages = new List<double>();
                double cumulativeSum = 0;

                foreach (var item in sortedDefects)
                {
                    cumulativeSum += item.Value;
                    //cumulativePercentages.Add((cumulativeSum / totalDefects) * 100);
                    cumulativePercentages.Add(((totalInputQuantity - totalDefects + cumulativeSum) / totalInputQuantity) * 100);
                }

                // Clear previous chart data
                chartPareto.Series.Clear();
                chartPareto.ChartAreas.Clear();
                chartPareto.ChartAreas.Add(new ChartArea("Main"));

                var barSeries = new Series("Defect Count")
                {
                    ChartType = SeriesChartType.Column,
                    YAxisType = AxisType.Primary,
                    Color = System.Drawing.Color.Blue,
                    IsValueShownAsLabel = true
                };

                // Create Line Series for Cumulative Percentage
                var lineSeries = new Series("Cumulative %")
                {
                    ChartType = SeriesChartType.Line,
                    YAxisType = AxisType.Secondary,
                    Color = System.Drawing.Color.Red,
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle
                };

                var totalInputSeries = new Series($"Total Input: {totalInputQuantity}")
                {
                    ChartType = SeriesChartType.Line, // Hidden line series
                    Color = System.Drawing.Color.Transparent, // Hide in chart
                    IsVisibleInLegend = true, // Show in legend
                    BorderWidth = 0
                };
                var totalYeildSeries = new Series($"Total Yeild: {totalYeild}%")
                {
                    ChartType = SeriesChartType.Line, // Hidden line series
                    Color = System.Drawing.Color.Transparent, // Hide in chart
                    IsVisibleInLegend = true, // Show in legend
                    BorderWidth = 0
                };

                // Add data points
                for (int i = 0; i < sortedDefects.Count; i++)
                {
                    barSeries.Points.AddXY(sortedDefects[i].Key, sortedDefects[i].Value);
                    if (i > 0)
                    {
                        lineSeries.Points.AddXY(sortedDefects[i].Key, cumulativePercentages[i]);
                    }
                    else
                    {
                        // Add an empty point for the first bar so the line starts from the second bar
                        lineSeries.Points.AddXY(sortedDefects[i].Key, double.NaN);
                    }
                }

                // Add series to chart
                chartPareto.Series.Add(barSeries);
                chartPareto.Series.Add(lineSeries);
                chartPareto.Series.Add(totalInputSeries);
                chartPareto.Series.Add(totalYeildSeries);

                // Configure chart axes
                chartPareto.ChartAreas["Main"].AxisX.Title = "Defect Codes";
                chartPareto.ChartAreas["Main"].AxisY.Title = "Defect Count";
                chartPareto.ChartAreas["Main"].AxisY2.Title = "Cumulative Percentage (%)";
                chartPareto.ChartAreas["Main"].AxisY2.Enabled = AxisEnabled.True;

                // Ensure all defect codes are visible
                chartPareto.ChartAreas["Main"].AxisX.Interval = 1; // Display all labels
                chartPareto.ChartAreas["Main"].AxisX.LabelStyle.Angle = -45; // Rotate labels for better visibility
                chartPareto.ChartAreas["Main"].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                chartPareto.ChartAreas["Main"].AxisX.LabelStyle.IsStaggered = false; // Avoid overlapping

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private Dictionary<string, int> ParseDefectData(string filePath)
        {
            try
            {
                Dictionary<string, int> defectCounts = new Dictionary<string, int>();
                string[] lines = File.ReadAllLines(filePath);
                int startColumn = -1, endColumn = -1;
                List<String> defectCodeList = new List<String>();

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Detect column positions from header row
                    if (line.Contains("InputQty") && line.Contains("TotalReject"))
                    {

                        string exampleInput = "D_Cut_GlueNOTCH";
                        foreach (string defectCode in parts)
                        {
                            if (defectCode.Contains(exampleInput))
                            {
                                defectCodeList.Add("D_Cut_Glue");
                                defectCodeList.Add("NOTCH");
                            }
                            else
                            {
                                defectCodeList.Add(defectCode);
                            }
                        }
                        startColumn = Array.IndexOf(parts, "EP") + 1;
                        endColumn = Array.IndexOf(parts, "TotalReject") - 1;
                        continue; // Skip header row
                    }

                    if (startColumn > 0 && endColumn >= startColumn && parts.Count() > endColumn)
                    {
                        // Read defect codes dynamically between InputQuantity and TotalReject
                        for (int i = startColumn; i <= endColumn; i++)
                        {
                            if (int.TryParse(parts[i], out int defectCount) && defectCount >= 0)
                            {
                                //string defectCode = $"Defect-{i - startColumn + 1}"; // Generate unique defect code name
                                string defectCode = defectCodeList[i]; // Generate unique defect code name
                                if (defectCounts.ContainsKey(defectCode))
                                    defectCounts[defectCode] += defectCount;
                                else
                                    defectCounts[defectCode] = defectCount;
                            }
                        }
                    }
                }
                return defectCounts;
            }
            catch (Exception ex)
            {
                Dictionary<string, int> defectCounts = new Dictionary<string, int>();
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return defectCounts;
            }
        }

        private int CalculateTotalInputQuantity(string filePath)
        {
            int totalInputQuantity = 0;
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                MessageBox.Show("File does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
            {
                MessageBox.Show("File is read-only!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {

                // Trim spaces and match the exact phrase "TOTAL INPUT" followed by a number
                if (line.Trim().StartsWith("TOTAL INPUT"))
                {
                    string[] parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Ensure the line has only 3 parts: "TOTAL INPUT" and the number
                    if (parts.Length == 3 && int.TryParse(parts[2], out int total))
                    {
                        totalInputQuantity = total;
                        break; // Stop once found
                    }
                }

            }
            return totalInputQuantity;

        }

        private double CalculateTotalYeild(string filePath)
        {
            double totalInputQuantity = 0;
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                MessageBox.Show("File does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
            {
                MessageBox.Show("File is read-only!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {

                // Trim spaces and match the exact phrase "TOTAL INPUT" followed by a number
                if (line.Trim().StartsWith("PASS YIELD"))
                {
                    string[] parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Ensure the line has only 3 parts: "TOTAL INPUT" and the number
                    parts[2] = parts[2].Replace("%", "");
                    if (parts.Length == 3 && double.TryParse(parts[2], out double total))
                    {
                        totalInputQuantity = total;
                        break; // Stop once found
                    }
                }

            }
            return totalInputQuantity;

        }


    }

}
