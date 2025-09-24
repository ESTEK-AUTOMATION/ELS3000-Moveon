using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Machine;
using System.Windows.Forms.DataVisualization.Charting;
using Machine.Platform;

namespace Product
{
    public partial class AlarmParetoChart : UserControl
    {
        private string m_strIOPathDatabase = "..\\Record\\";
        private string m_strIOFileName = ".db";
        private string m_strIOSetting = ";Version=3;New=False;Compress=True;";
        private string m_strIODatabaseFile;
        DateTime dt = DateTime.Now;

        private static SQLLite m_database = new SQLLite();
        public AlarmParetoChart()
        {
            InitializeComponent();
        }

        public void LoadDataToDisplay(string LotID, List<Alarm> Alarms)
        {
            m_strIODatabaseFile = "Data Source=" + m_strIOPathDatabase + string.Format("{0:yyyy}\\{1:MM}\\{2:yyyy-MM-dd}.db", dt, dt, dt) + m_strIOSetting;
            m_database.Configure(m_strIODatabaseFile, ";", "/");
            var alarmCounts = GetAlarmCountsBasedOnLotID(LotID);
            // Draw Pareto chart
            DrawParetoChart(alarmCounts);

            // Update ListView with AlarmID and AlarmMessageEnglish
            AddAlarmsToListView(alarmCounts, Alarms);
        }

        public void DrawParetoChart(Dictionary<int, int> alarmCounts)
        {
            AlarmChart.Series.Clear();
            AlarmChart.ChartAreas.Clear();

            var chartArea = new ChartArea("ChartArea1");
            AlarmChart.ChartAreas.Add(chartArea);

            var ordered = alarmCounts.OrderByDescending(kv => kv.Value).ToList();
            int total = ordered.Sum(x => x.Value);
            double cumulative = 0;

            var barSeries = new Series("Alarm Count")
            {
                ChartType = SeriesChartType.Column,
                Color = System.Drawing.Color.SteelBlue,
                YAxisType = AxisType.Primary,
                IsValueShownAsLabel = true
            };

            //var lineSeries = new Series("Cumulative %")
            //{
            //    ChartType = SeriesChartType.Line,
            //    Color = System.Drawing.Color.Red,
            //    BorderWidth = 2,
            //    YAxisType = AxisType.Secondary
            //};

            int index = 0;
            foreach (var kv in ordered)
            {
                string label = kv.Key.ToString();
                barSeries.Points.AddXY(label, kv.Value);

                cumulative += (double)kv.Value / total * 100;
                //lineSeries.Points.AddXY(label, cumulative);

                index++;
            }

            AlarmChart.Series.Add(barSeries);
            //AlarmChart.Series.Add(lineSeries);

            AlarmChart.ChartAreas[0].AxisX.Interval = 1;
            AlarmChart.ChartAreas[0].RecalculateAxesScale();
        }

        private Dictionary<int, int> GetAlarmCountsBasedOnLotID(string lotId)
        {
            //var result = new Dictionary<int, int>();

            string result = m_database.Read("Record", "ALARMID", $"LOTID = '{lotId}' AND ALARMID != 0 AND ALARMID != 2001 AND ALARMID !=  2002");
            string[] alarmCounting = result.Split('/');

            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (string str in alarmCounting)
            {
                if (int.TryParse(str, out int number))
                {
                    if (!counts.ContainsKey(number))
                        counts[number] = 0;

                    counts[number]++;
                }
            }

            return counts;
        }

        public void AddAlarmsToListView(Dictionary<int, int> alarmCounts, List<Alarm> alarmList)
        {
            // Deserialize the CustomerAlarmList from XML
            //CustomerAlarmList alarmList = Tools.Deserialize<CustomerAlarmList>(filepath);

            // Loop through the dictionary of alarm counts, which contains AlarmID and their occurrence counts
            foreach (var alarmEntry in alarmCounts)
            {
                int alarmID = alarmEntry.Key;
                int count = alarmEntry.Value;  // Count of occurrences (if needed, you can display this as well)

                // Exclude AlarmID 2001 and 2002
                if (alarmID == 2001 || alarmID == 2002)
                {
                    continue;  // Skip this iteration and don't add it to the ListView
                }

                // Search for the AlarmMessageEnglish corresponding to the AlarmID
                var alarmMessage = FindAlarmMessageById(alarmList, alarmID);

                // If a matching AlarmID is found, add it to the ListView
                if (alarmMessage != null)
                {
                    // Add AlarmID to first column
                    ListViewItem item = new ListViewItem(alarmID.ToString());

                    // Add AlarmMessageEnglish to second column
                    item.SubItems.Add(alarmMessage);

                    item.SubItems.Add(count.ToString()); // Optionally display the count of the alarm occurrences
                    listViewAlarmList.Items.Add(item); // Add item to the ListView
                }
            }
        }

        private string FindAlarmMessageById(List<Alarm> alarmList, int alarmID)
        {
            // Iterate through all the fields in the CustomerAlarmList class
            var fields = alarmList.GetType().GetFields();

            foreach (var field in fields)
            {
                // Check if the field is of type 'Alarm' and if its AlarmID matches the input
                if (field.FieldType == typeof(Alarm))
                {
                    Alarm alarm = (Alarm)field.GetValue(alarmList);
                    if (alarm.AlarmID == alarmID)
                    {
                        // Return the AlarmMessageEnglish if found
                        return alarm.AlarmMessageEnglish;
                    }
                }
            }

            // Return null if no matching AlarmID is found
            return null;
        }

    }























}
