using System;

namespace RemoteCam.DTO
{
    public class Capture
    {
        //To avoid client side date formatting
        public string TimeStamp { get; set; }
        public string Image64 { get; set; }
    }
}
