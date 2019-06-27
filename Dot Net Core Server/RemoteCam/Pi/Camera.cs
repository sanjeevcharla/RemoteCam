using RemoteCam.DTO;
using System.IO;

namespace RemoteCam.Pi
{
    public static class Camera
    {
        public static void TakePicture()
        {
            $" -r 1280x720 --no-timestamp image.jpg".RunAsTerminalCommand();
        }

        public static Capture GetLatestCapture()
        {
            FileInfo imageFileInfo = new FileInfo("image.jpg");
            Capture capture = new Capture()
            {
                Image64 = imageFileInfo.ToBase64(),
                TimeStamp = imageFileInfo.LastWriteTime.ToString("dd-MM-yyyy hh:mm:ss")
            };
            return capture;
        }

        public static string GetLatestCaptureFile()
        {
            return new FileInfo("image.jpg").FullName;
        }
    }
}
