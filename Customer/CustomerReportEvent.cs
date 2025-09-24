using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine;
using Product;

namespace Customer
{
    public class CustomerReportEvent : ProductReportEvent
    {
        public EventSetting EventLoadMaterialTime = new EventSetting { EventID = 15, EventName = "Load Time", EventResult = DownTime };
        public EventSetting EventUnloadMaterialTime = new EventSetting { EventID = 16, EventName = "Unload Time", EventResult = DownTime };
        public EventSetting EventRejectTrayExchangeTime = new EventSetting { EventID = 17, EventName = "Reject Tray Exchange Time", EventResult = UpTime };
        public EventSetting EventCleaningTime = new EventSetting { EventID = 18, EventName = "Cleaning Time", EventResult = DownTime };
    }
}
