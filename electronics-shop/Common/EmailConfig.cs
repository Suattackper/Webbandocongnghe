using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace electronics_shop.Common
{
    // Huỳnh Như : 21/12/23 3:18 PM
    public class EmailConfig
    {
        
            /*
               Để dùng gmail gửi email rs cho người khác bạn cần phải vô đây "https://www.google.com/settings/security/lesssecureapps" Cho phép ứng dụng kém an toàn: Bật
             */
            public const string emailID = "lytuthat1234@gmail.com";
            public const string emailPassword = "Kikiga18123";
            public const string emailHost = "smtp.gmail.com"; //nếu dùng gmail thì đổi thành  "Host = "smtp.gmail.com"
            public const string emailName = "Besnik_Electronics shop";//Tên email hiển thị khi gửi

    }
}