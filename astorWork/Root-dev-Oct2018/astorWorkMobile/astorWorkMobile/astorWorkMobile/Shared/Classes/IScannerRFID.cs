using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.Shared.Classes
{
    public interface IScannerRFID
    {
        bool SetPower(int power);
        bool StartContinousScan(byte anti, byte q);
        bool StopContinousScan();
        bool InitSuccess();
        string GetStatus();
        string GetSingleTag();
        string GetTagFromBuffer();
        void SubscribeKeyEvent(EventHandler handler);
        void UnsubscribeKeyEvent(EventHandler handler);

        void Dispose();
        bool Init();
    }

    public class AndroidKeyEventArgs : EventArgs
    {
        public string Key { get; set; }
        public int KeyCode { get; set; }
        public int RepeatCount { get; set; }
    }
}
