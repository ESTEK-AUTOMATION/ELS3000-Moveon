using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;

namespace Customer
{
    public static class RecipeParameterLogger
    {
        public static FileLogger m_RecipeParameterLogger = new FileLogger("RECIPE");
        private static Object m_locker = new Object();
        //1.0.0.0o Charles
        private static bool m_bKillThread = false;
        private static List<string> m_listMessage = new List<string>();
        private static StringBuilder sbMessage = new StringBuilder();
        private static Mutex m_mutexAccessListMessage = new Mutex();
        private static Mutex m_mutexAccessWriteLog = new Mutex();
        private static Thread m_thread;
        private static bool m_bEnableThreadPool = true;
        //--
        public static void WriteLog(string str_msg)
        {
            //1.0.0.0o Charles
            //ThreadPool.QueueUserWorkItem(WriteLogCallback, str_msg);
            if (m_bEnableThreadPool)
                ThreadPool.QueueUserWorkItem(WriteLogCallback, str_msg);
            else
            {
                m_mutexAccessListMessage.WaitOne();
                sbMessage.AppendLine(str_msg);
                m_mutexAccessListMessage.ReleaseMutex();
            }
            //--
        }

        private static void WriteLogCallback(Object msg)
        {
            try
            {
                lock (m_locker)
                {
                    //1.0.0.0o Charles
                    m_mutexAccessWriteLog.WaitOne();
                    //--
                    m_RecipeParameterLogger.CreateLog();
                    m_RecipeParameterLogger.AppendLogFile((string)msg);
                    //1.0.0.0o Charles
                    m_mutexAccessWriteLog.ReleaseMutex();
                    //--
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        //1.0.0.0o Charles
        public static void ThreadStart()
        {
            try
            {
                m_bEnableThreadPool = false;
                m_thread = new Thread(RunLogger);
                m_thread.Priority = ThreadPriority.Lowest;

                m_thread.Start();
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }

        public static void RunLogger()
        {
            try
            {
                string strLogMessage = "";
                while (!m_bKillThread)
                {

                    if (sbMessage.Length > 0)
                    {
                        m_mutexAccessListMessage.WaitOne();
                        strLogMessage = sbMessage.ToString();
                        sbMessage.Clear();
                        m_mutexAccessListMessage.ReleaseMutex();

                        if (strLogMessage.EndsWith("\r\n"))
                        {
                            if (strLogMessage.Length - 2 >= 0)
                                strLogMessage = strLogMessage.Remove(strLogMessage.Length - 2, 2);
                        }
                        m_mutexAccessWriteLog.WaitOne();
                        m_RecipeParameterLogger.CreateLog();
                        m_RecipeParameterLogger.AppendLogFile(strLogMessage);
                        m_mutexAccessWriteLog.ReleaseMutex();
                        strLogMessage = "";
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }

        public static void ExitThread()
        {
            try
            {
                m_bKillThread = true;
                m_thread.Join();
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), "Exit Recipe Parameter Logger thread.\n"));
            }
            catch (Exception ex)
            {
                Machine.DebugLogger.WriteLog(string.Format("{0} {1}.", DateTime.Now.ToString("yyyyMMdd HHmmss"), ex.ToString()));
            }
        }
        //--

    }
}
