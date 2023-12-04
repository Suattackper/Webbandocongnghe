using electronics_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace electronics_shop.Common
{
    public class Notification
    {
        public static bool Has_flash()
        {
            if (HttpContext.Current != null && HttpContext.Current.Application["Notification"] != null)
            {
                // Kiểm tra nếu dữ liệu trong "Notification" không rỗng
                if (!string.IsNullOrEmpty(HttpContext.Current.Application["Notification"].ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public static void setNotification1_5s(string msg1_5s, string msgType1_5s)
        {
            var tb = new NotificationModels();
            tb.msg1_5s = msg1_5s;
            tb.msgType1_5s = msgType1_5s;
            HttpContext.Current.Application["Notification"] = tb;
        }

        public static void setNotification3s(string msg3s, string msgType3s)
        {
            var tb = new NotificationModels();
            tb.msg3s = msg3s;
            tb.msgType3s = msgType3s;
            HttpContext.Current.Application["Notification"] = tb;
        }

        public static void setNotification5s(string msg5s, string msgType5s)
        {
            var tb = new NotificationModels();
            tb.msg5s = msg5s;
            tb.msgType5s = msgType5s;
            HttpContext.Current.Application["Notification"] = tb;
        }

        public static NotificationModels Get_flash()
        {
            var Notifi = (NotificationModels)HttpContext.Current.Application["Notification"];
            HttpContext.Current.Application["Notification"] = "";
            return Notifi;
        }
    }
}