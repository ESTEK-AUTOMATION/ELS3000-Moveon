using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Machine;
using Common;

namespace Product
{
    public class ProductFormReport : Machine.FormReport
    {
        private ProductShareVariables m_ProductShareVariables;// = new ProductShareVariables();
        public new ProductAlarmList m_alarmData = new ProductAlarmList();

        private ProductReportProcess m_ProductReportProcess;
        private ProductReportEvent m_ProductReportEvent;

        public ProductShareVariables productShareVariables
        {
            set
            {
                m_ProductShareVariables = value;
                shareVariables = m_ProductShareVariables;
            }
        }

        public ProductReportProcess productReportProcess
        {
            set
            {
                m_ProductReportProcess = value;
                reportProcess = value;
            }
        }

        public ProductReportEvent productReportEvent
        {
            set
            {
                m_ProductReportEvent = value;
                
                // = value;
            }
        }

        override public void InitializeAlarmList()
        {
            try
            {
                m_alarmData = Tools.Deserialize<ProductAlarmList>(m_ProductShareVariables.strSystemPath + m_ProductShareVariables.strAlarmFile);
                ProductAlarmList alarmList = new ProductAlarmList();

                FieldInfo[] fields = typeof(ProductAlarmList).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo _fields in fields)
                {
                    m_alarmList.Add((Machine.Platform.Alarm)_fields.GetValue(alarmList));
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.",
                    DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
        }
        override public int GetUpDownSetting(int eventID, ReportEvent reportEvent)
        {
            int nSetting = 2;

            if (eventID == reportEvent.EventSoftwareInitialize.EventID)
            {
                nSetting = reportEvent.EventSoftwareInitialize.EventResult;
            }

            if (eventID == reportEvent.EventSoftwareDisable.EventID)
            {
                nSetting = reportEvent.EventSoftwareDisable.EventResult;
            }

            if (eventID == reportEvent.EventSoftwareShutDown.EventID)
            {
                nSetting = reportEvent.EventSoftwareShutDown.EventResult;
            }

            if (eventID == reportEvent.EventMachineIdle.EventID)
            {
                nSetting = reportEvent.EventMachineIdle.EventResult;
            }

            if (eventID == reportEvent.EventMachineHome.EventID)
            {
                nSetting = reportEvent.EventMachineHome.EventResult;
            }

            if (eventID == reportEvent.EventMachineEngineering.EventID)
            {
                nSetting = reportEvent.EventMachineEngineering.EventResult;
            }

            if (eventID == reportEvent.EventMachineConversion.EventID)
            {
                nSetting = reportEvent.EventMachineConversion.EventResult;
            }

            if (eventID == reportEvent.EventMachineMaintenance.EventID)
            {
                nSetting = reportEvent.EventMachineMaintenance.EventResult;
            }

            if (eventID == reportEvent.EventMachineMaterial.EventID)
            {
                nSetting = reportEvent.EventMachineMaterial.EventResult;
            }

            if (eventID == reportEvent.EventMachineResponse.EventID)
            {
                nSetting = reportEvent.EventMachineResponse.EventResult;
            }

            if (eventID == reportEvent.EventMachineProduction.EventID)
            {
                nSetting = reportEvent.EventMachineProduction.EventResult;
            }

            if (eventID == reportEvent.EventMachineFailure.EventID)
            {
                nSetting = reportEvent.EventMachineFailure.EventResult;
            }
            if (eventID == reportEvent.EventMachineMessage.EventID)
            {
                nSetting = reportEvent.EventMachineMessage.EventResult;
            }
            else
            {

            }
            return nSetting;
        }

        override public int CalculatePerformance(ReportProcess.Record[] record, List<int> alarmFilter, ReportEvent reportEvent, out MachinePerformance machinePerformance)
        {
            int m_nError = 0;

            machinePerformance = new MachinePerformance
            {
                UpTime = new TimeSpan(),
                DownTime = new TimeSpan(),
                FailureTime = new TimeSpan(),
                WaitTime = new TimeSpan(),
                AssistTime = new TimeSpan(),
                MaintenanceTime = new TimeSpan(),
                TotalTime = new TimeSpan(),
                MTBA = new TimeSpan(),
                MTBF = new TimeSpan(),
                MTTR = new TimeSpan(),
                PercentDownTime = 0,
                PercentUpTime = 0,
                PercentWaitTime = 0,
                PercentMaintanainceTime = 0,
                MCBA = 0,
                MCBF = 0,
                AssistCount = 0,
                FailureCount = 0,
                ShotCount = 0
            };

            ReportProcess.Record recordPrevious = new ReportProcess.Record();
            try
            {
                if (record == null)
                {
                    return 1;
                }
                if (record.Length == 0)
                {
                    return 2;
                }
                foreach (var _reportInfo in record)
                {
                    if (recordPrevious.Equals(new ReportProcess.Record()))
                    {
                        recordPrevious = _reportInfo;
                        continue;
                    }
                    if (alarmFilter.Contains(recordPrevious.alarmID))
                    {
                    }
                    else
                    {
                        if (recordPrevious.eventID == 8)  // maintainence 
                        {
                            machinePerformance.MaintenanceTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                            //nAssistCount++;
                        }
                        if (recordPrevious.eventID == 11)  // Production 
                        {
                            machinePerformance.ShotCount++;
                        }
                        if (recordPrevious.eventID == 10) // wait time
                        {
                            machinePerformance.WaitTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        if (recordPrevious.eventID == 12) // Assist time
                        {
                            machinePerformance.AssistCount++;
                            machinePerformance.AssistTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        if (recordPrevious.eventID == 13)  // alarm
                        {
                            machinePerformance.FailureCount++;
                            machinePerformance.FailureTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        if (GetUpDownSetting(recordPrevious.eventID, reportEvent) == 1)  // 
                        {
                            machinePerformance.UpTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                            machinePerformance.TotalTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                        else if (GetUpDownSetting(recordPrevious.eventID, reportEvent) == 0) //
                        {
                            machinePerformance.DownTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                            machinePerformance.TotalTime += _reportInfo.dateTime.Subtract(recordPrevious.dateTime);
                        }
                    }
                    recordPrevious = _reportInfo;
                }

                if (machinePerformance.TotalTime.Ticks != 0)
                {
                    machinePerformance.PercentDownTime = ((double)machinePerformance.DownTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                    machinePerformance.PercentUpTime = ((double)machinePerformance.UpTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                    machinePerformance.PercentMaintanainceTime = ((double)machinePerformance.MaintenanceTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                    machinePerformance.PercentWaitTime = ((double)machinePerformance.WaitTime.Ticks / (double)machinePerformance.TotalTime.Ticks) * 100;
                }
                else
                {
                    machinePerformance.PercentDownTime = 0;
                    machinePerformance.PercentUpTime = 0;
                    machinePerformance.PercentMaintanainceTime = 0;
                    machinePerformance.PercentWaitTime = 0;
                }
                machinePerformance.PercentDownTime = Math.Round(machinePerformance.PercentDownTime, 2);
                machinePerformance.PercentUpTime = Math.Round(machinePerformance.PercentUpTime, 2);
                machinePerformance.PercentMaintanainceTime = Math.Round(machinePerformance.PercentMaintanainceTime, 2);
                machinePerformance.PercentWaitTime = Math.Round(machinePerformance.PercentWaitTime, 2);

                if (machinePerformance.AssistCount == 0)
                {
                    //nAssistCount = 1;
                    machinePerformance.MTBA = new TimeSpan(machinePerformance.UpTime.Ticks);
                    machinePerformance.MCBA = (double)machinePerformance.ShotCount;
                }
                else
                {
                    machinePerformance.MTBA = new TimeSpan(machinePerformance.UpTime.Ticks / machinePerformance.AssistCount);
                    machinePerformance.MCBA = (double)machinePerformance.ShotCount / machinePerformance.AssistCount;
                }
                if (machinePerformance.FailureCount == 0)
                {
                    //nFailureCount = 1;
                    machinePerformance.MTBF = new TimeSpan(machinePerformance.UpTime.Ticks);
                    machinePerformance.MCBF = (double)machinePerformance.ShotCount;
                    machinePerformance.MTTR = new TimeSpan(machinePerformance.DownTime.Ticks);
                }
                else
                {
                    machinePerformance.MTBF = new TimeSpan(machinePerformance.UpTime.Ticks / machinePerformance.FailureCount);
                    machinePerformance.MCBF = (double)machinePerformance.ShotCount / machinePerformance.FailureCount;
                    machinePerformance.MTTR = new TimeSpan(machinePerformance.DownTime.Ticks / machinePerformance.FailureCount);
                }
                if ((machinePerformance.AssistCount + machinePerformance.FailureCount) == 0)
                    machinePerformance.MTTA = new TimeSpan(machinePerformance.WaitTime.Ticks);
                else
                    machinePerformance.MTTA = new TimeSpan(machinePerformance.WaitTime.Ticks / (machinePerformance.AssistCount + machinePerformance.FailureCount));
            }
            catch (Exception ex)
            {
                m_nError = -1;
                DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.",
                    DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return m_nError;
        }

        override public int GenerateReportToListString(DateTime startDate, DateTime endDate, MachinePerformance machinePerformance, ReportProcess.Record[] record, List<AlarmInfo2> listAlarmRecord, List<int> alarmFilter, out StringBuilder data)
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            int nError = 0;
            data = new StringBuilder();
            String X = ((endDate - startDate).TotalHours / 24).ToString();  //((startDate - endDate).ToString("dd"))
            try
            {
                #region Data_structure
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                //listStrData.Append(string.Format("{0,-70}  \r\n", "ESTEK AUTOMATION SDN BHD") + "\r\n");
                data.Append("                                       ESTEK AUTOMATION SDN BHD                               " + "\r\n");
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append(string.Format("{0,-10} {1} {2}", "Date", ":", DateTime.Now.ToString("dd/MM/yyyy") + "\r\n"));
                data.Append(string.Format("{0,-10} {1} {2}", "Period", ":", startDate.ToString("dd/MM/yyyy(HH:mm:ss)") + " - " + endDate.ToString("dd/MM/yyyy(HH:mm:ss)") + " (" + ((endDate - startDate).TotalHours / 24).ToString("n2") + " days)") + "\r\n");
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                //listStrData.Append(string.Format("{0,-80}  \r\n", "MACHINE PERFORMANCE"));
                data.Append("                                         MACHINE PERFORMANCE                                  " + "\r\n");
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "UP TIME", ":", machinePerformance.UpTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentUpTime + "%)", "", ""));
                //listStrData += (string.Format("{0,-10} {1,24} {2,-15} {3,-20} {4,-15}\r\n", "STOP TIME", ":", "<STOP TIME>", "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "DOWN TIME", ":", machinePerformance.DownTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentDownTime + "%)", "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "WAIT TIME", ":", machinePerformance.WaitTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentWaitTime + "%)", "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "MAINTENANCE TIME", ":", machinePerformance.MaintenanceTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentMaintanainceTime + "%)", "", ""));
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "MTBA", ":", machinePerformance.MTBA.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "MTBF", ":", machinePerformance.MTBF.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "MTTA", ":", machinePerformance.MTTA.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "MTTR", ":", machinePerformance.MTTR.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "ASSIST", ":", machinePerformance.AssistTime.ToString(@"dd\.hh\:mm\:ss") + "(" + machinePerformance.AssistCount.ToString() + " times)", "", ""));
                //listStrData += (string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "WAIT TIME", ":", "<ASSIST WAIT TIME>", "", ""));
                data.Append(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "FAILURES", ":", machinePerformance.FailureTime.ToString(@"dd\.hh\:mm\:ss") + "(" + machinePerformance.FailureCount.ToString() + " times)", "", ""));
                //listStrData += (string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "WAIT TIME", ":", "<FAILURES WAIT TIME>", "", ""));
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                //listStrData += ("");
                //listStrData += ("");
                data.Append("                                             History                                      " + "\r\n");
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append(string.Format("{0,-10} {1,-12} {2,-10} {3,-13} {4,-13} {5, -14}  {6, -10}\r\n", "No", "Date", "Time", "Duration", "Descriptions", "Lot Id", "Alarm Id"));
                data.Append("     " + "\r\n");
                //listStrData += (string.Format(""));
                //listStrData += ("");
                //MessageBox.Show("get report header time " + stopwatch.ElapsedMilliseconds.ToString());
                //stopwatch.Restart();
                int nNo = 1;
                int nZeroNo = 0;
                string strNo = "";
                string strAlarmid = "";

                ReportProcess.Record recordPrevious = new ReportProcess.Record();

                foreach (ReportProcess.Record _report in record)
                {
                    if (recordPrevious.Equals(new ReportProcess.Record()))
                    {
                        recordPrevious = _report;
                        continue;
                    }
                    if (_report.dateTime == null)
                    {

                    }
                    else
                    {
                        if (alarmFilter.Contains(_report.alarmID))
                        {


                        }
                        else
                        {
                            if (_report.alarmID == 0)
                            {
                                strAlarmid = "";
                            }
                            else
                            {
                                strAlarmid = _report.alarmID.ToString();
                            }
                            nZeroNo = Convert.ToInt32(Math.Floor(Math.Log10(record.Length) + 1));
                            strNo = string.Format("{0:D" + nZeroNo.ToString() + "}", nNo);
                            data.Append(string.Format("{0,-10} {1,-12} {2,-10} {3,-13} {4,-13} {5, -14}  {6, -10}\r\n", strNo, _report.dateTime.ToString("dd/MM/yyyy"), _report.dateTime.ToString(@"HH\:mm\:ss"), _report.dateTime.Subtract(recordPrevious.dateTime).ToString("G"), _report.eventName, _report.lotID, strAlarmid));
                            nNo++;
                        }
                    }
                    recordPrevious = _report;
                }
                //MessageBox.Show("get report data time " + stopwatch.ElapsedMilliseconds.ToString());
                //stopwatch.Restart();
                //foreach (var a in m_alarmList)
                //{
                //     listStrData.Add(string.Format("{0,-10} {1,-12} \r\n",a.AlarmID , a.AlarmMessageEnglish));
                //}
                data.Append(string.Format("\r\n"));
                data.Append(string.Format("\r\n"));
                data.Append(string.Format("\r\n"));
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append("                                            Alarm List                                      " + "\r\n");
                data.Append("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.Append(string.Format("\r\n"));

                //var query = m_alarmList.OrderBy(p => p.AlarmID).ToList();
                var query = listAlarmRecord.OrderBy(p => p.AlarmID).ToList();

                foreach (var item in query)
                {
                    if (item.AlarmID > 1000)
                    {
                        //listStrData.Add(string.Format("{0,-10} {1,-12} \r\n", item.AlarmID, item.AlarmMessageEnglish));
                        data.Append(string.Format("{0,-7} {1,-9} {2,-9} \r\n", item.AlarmID, item.AlarmType, item.AlarmName));
                    }
                }

                #endregion
                //MessageBox.Show("get report alarm time " + stopwatch.ElapsedMilliseconds.ToString());
                //stopwatch.Restart();
            }
            catch (Exception ex)
            {
                nError = -1;
                DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.",
                    DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return nError;
        }

        override public int GenerateReportToCSV(DateTime startDate, DateTime endDate, MachinePerformance machinePerformance, ReportProcess.Record[] report, List<AlarmInfo2> listAlarmRecord, List<int> alarmFilter, out StringBuilder data)
        {
            int m_nError = 0;
            data = new StringBuilder();
            try
            {
                #region Data_structure
                data.Clear();
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                //listStrData.Add(string.Format("{0,-70}  \r\n", "ESTEK AUTOMATION SDN BHD") + "\r\n");
                data.AppendLine("ESTEK AUTOMATION SDN BHD                               ");
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                data.AppendLine(string.Format("{0,-10}, {1} {2}", "Date", ":", DateTime.Now.ToString("dd/MM/yyyy")));
                data.AppendLine(string.Format("{0,-10}, {1} {2}", "Period", ":", startDate.ToString("dd/MM/yyyy (HH:mm:ss)") + " - " + endDate.ToString("dd/MM/yyyy (HH:mm:ss)") + " (" + ((startDate - endDate).ToString("dd")) + " days)"));
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                //listStrData.Add(string.Format("{0,-80}  \r\n", "MACHINE PERFORMANCE"));
                data.AppendLine("");
                data.AppendLine("MACHINE PERFORMANCE                                  ");
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "UP TIME", ":", machinePerformance.UpTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentUpTime + "%)", "", ""));
                //listStrData.Add(string.Format("{0,-10} {1,24} {2,-15} {3,-20} {4,-15}\r\n", "STOP TIME", ":", "<STOP TIME>", "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "DOWN TIME", ":", machinePerformance.DownTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentDownTime + "%)", "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "WAIT TIME", ":", machinePerformance.WaitTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentWaitTime + "%)", "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "MAINTENANCE TIME", ":", machinePerformance.MaintenanceTime.ToString(@"dd\.hh\:mm\:ss") + " (" + machinePerformance.PercentMaintanainceTime + "%)", "", ""));
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "MTBA", ":", machinePerformance.MTBA.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "MTBF", ":", machinePerformance.MTBF.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "MTTA", ":", machinePerformance.MTTA.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "MTTR", ":", machinePerformance.MTTR.ToString(@"dd\.hh\:mm\:ss"), "", ""));
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" + "\r\n");
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "ASSIST", ":", machinePerformance.AssistTime.ToString(@"dd\.hh\:mm\:ss") + "(" + machinePerformance.AssistCount.ToString() + " times)", "", ""));
                //listStrData.Add(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "WAIT TIME", ":", "<ASSIST WAIT TIME>", "", ""));
                data.AppendLine(string.Format("{0,-18}, {1,-1} {2,-15} {3,-20} {4,-15}", "FAILURES", ":", machinePerformance.FailureTime.ToString(@"dd\.hh\:mm\:ss") + "(" + machinePerformance.FailureCount.ToString() + " times)", "", ""));
                //listStrData.Add(string.Format("{0,-18} {1,-1} {2,-15} {3,-20} {4,-15}\r\n", "WAIT TIME", ":", "<FAILURES WAIT TIME>", "", ""));
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                //listStrData.Add("");
                //listStrData.Add("");
                data.AppendLine("");
                data.AppendLine("History                                      ");
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                data.AppendLine("No" + "," + "Date" + "," + "Time" + "," + "Duration" + "," + "Descriptions" + "," + "Lot Id" + "," + "Alarm Id");
                int nNo = 1;
                int nZeroNo = 0;
                string strNo = "";
                string strAlarmid = "";

                ReportProcess.Record recordPrevious = new ReportProcess.Record();

                foreach (ReportProcess.Record _record in report)
                {
                    if (recordPrevious.Equals(new ReportProcess.Record()))
                    {
                        recordPrevious = _record;
                        continue;
                    }
                    if (alarmFilter.Contains(_record.alarmID))
                    {


                    }

                    else
                    {
                        if (_record.dateTime == null)
                        {

                        }
                        else
                        {
                            if (_record.alarmID == 0)
                            {
                                strAlarmid = "";
                            }
                            else
                            {
                                strAlarmid = _record.alarmID.ToString();
                            }
                            nZeroNo = Convert.ToInt32(Math.Floor(Math.Log10(report.Length) + 1));
                            strNo = string.Format("{0:D" + nZeroNo.ToString() + "}", nNo);
                            //listStrData.Add(string.Format("{0,-10} {1,-12} {2,-10} {3,-13} {4,-13} {5, -14}  {6, -10}\r\n", strNo, _reportInfo.Date, _reportInfo.Time, _reportInfo.timeSpan, _reportInfo.PrevousEventName, _reportInfo.LotID, strAlarmid));
                            //data.AppendLine(strNo.ToString() + "," + _record.dateTime + "," + _record.Time + "," + _record.timeSpan + "," + _record.PrevousEventName + "," + _record.LotID + "," + strAlarmid);
                            //data.AppendLine(string.Format("{0,-10} {1,-12} {2,-10} {3,-13} {4,-13} {5, -14}  {6, -10}\r\n", strNo, _record.Date, _record.Time, _record.timeSpan, _record.PrevousEventName, _record.LotID, strAlarmid));
                            //data.AppendLine(string.Format("{0,-10} {1,-12} {2,-10} {3,-13} {4,-13} {5, -14}  {6, -10}\r\n", strNo, _record.dateTime.ToString("dd/MM/yyyy"), _record.dateTime.ToString(@"HH\:mm\:ss"), _record.timeSpan, _record.eventName, _record.lotID, strAlarmid));
                            data.AppendLine(strNo.ToString() + "," + _record.dateTime.ToString("dd/MM/yyyy") + "," + _record.dateTime.ToString(@"HH\:mm\:ss") + "," + _record.timeSpan + "," + _record.eventName + "," + _record.lotID + "," + strAlarmid);
                            nNo++;
                        }
                    }
                    recordPrevious = _record;
                }

                //foreach (var a in m_alarmList)
                //{
                //     listStrData.Add(string.Format("{0,-10} {1,-12} \r\n",a.AlarmID , a.AlarmMessageEnglish));
                //}
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );
                data.AppendLine("");
                data.AppendLine("Alarm List                                      ");
                data.AppendLine("");
                //listStrData.Add("+---------------------------------------------------------------------------------------------+" );

                //var query = m_alarmList.OrderBy(p => p.AlarmID).ToList();
                var query = listAlarmRecord.OrderBy(p => p.AlarmID).ToList();

                foreach (var item in query)
                {
                    if (item.AlarmID > 1000)
                    {
                        //listStrData.Add(string.Format("{0,-10} {1,-12} \r\n", item.AlarmID, item.AlarmMessageEnglish));
                        data.AppendLine(item.AlarmID + "," + item.AlarmType + "," + item.AlarmName);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                DebugLogger.WriteLog(string.Format("{0}  {1} at {2}.",
                    DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString(), m_strmode));
            }
            return m_nError;
        }

    }
}
