using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace JuristicMonitor
{
    class Logger
    {
        public enum MSG_LEVEL { VERBOSE, WARNING, ERROR};

        public class LogItem
        {
            DateTime time;
            String msg;
            MSG_LEVEL level;

            public LogItem(MSG_LEVEL l, DateTime d, String m)
            {
                level = l;
                time = d;
                msg = m;
            }
            public DateTime gettime() { return time;  }
            public String getmessage() { return msg; }
            public String getlevel() { 
                switch(level)
                {
                    case MSG_LEVEL.VERBOSE: return "VERBOSE";
                    case MSG_LEVEL.WARNING: return "WARNING";
                    case MSG_LEVEL.ERROR :  return "ERROR";
                }
                return "";
            }
        }

        static ArrayList messeges = new ArrayList();

        static public void v(String msg)
        {
            messeges.Add(new LogItem(MSG_LEVEL.VERBOSE, DateTime.Now, msg) );
        }

        static public void w(String msg)
        {
            messeges.Add(new LogItem(MSG_LEVEL.WARNING, DateTime.Now, msg));
        }
        
        static public void e(String msg)
        {
            messeges.Add(new LogItem(MSG_LEVEL.ERROR, DateTime.Now, msg));
        }

        public static int get_msg_count()
        {
            return messeges.Count;
        }

        public static LogItem get_message(int index)
        {
            return (LogItem)messeges[index];
        }

        //debug message on/off

        static Boolean debug_message = true;
        public static void setDebugMessageOnOff(Boolean dm)
        {
            debug_message = dm;
        }

        public static void DbgMsg(String s)
        {
            if(debug_message)
                Debug.Write(s);
        }

        public static void DbgMsgLine(String s)
        {
            if (debug_message)
                Debug.WriteLine(s);
        }
    }
}
