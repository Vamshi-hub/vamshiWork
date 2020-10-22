using Android.App;
using Android.Hardware.Input;
using astorWorkMobile.Droid;
using astorWorkMobile.Shared.Classes;
using Com.Rscja.Deviceapi;
using System;
using System.Diagnostics;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(ScannerC72))]
public class ScannerC72 : IScannerRFID
{
    public RFIDWithUHF uhfAPI;
    private bool _initSuccess = false;

    public string GetSingleTag()
    {
        string result = string.Empty;
        if (_initSuccess)
        {
            try
            {
                uhfAPI.StopInventory();
                string strUII = uhfAPI.InventorySingleTag();
                if (!string.IsNullOrEmpty(strUII))
                {
                    result = uhfAPI.ConvertUiiToEPC(strUII);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        return result;
    }

    public string GetStatus()
    {
        throw new System.NotImplementedException();
    }

    public void SubscribeKeyEvent(EventHandler handler)
    {
        if(MainActivity.Instance.scanKeyHandler == null)
            MainActivity.Instance.scanKeyHandler += handler;
    }

    public void UnsubscribeKeyEvent(EventHandler handler)
    {
        if (MainActivity.Instance.scanKeyHandler != null)
            MainActivity.Instance.scanKeyHandler -= handler;
    }

    public bool SetPower(int power)
    {
        if (_initSuccess)
        {
            try
            {
                return uhfAPI.SetPower(power);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        return false;
    }

    public bool StartContinousScan(byte anti, byte q)
    {
        if (_initSuccess)
        {
            try
            {
                uhfAPI.StopInventory();
                if (uhfAPI.SetEPCTIDMode(true))
                    return uhfAPI.StartInventoryTag(anti, q);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        return false;
    }

    public bool StopContinousScan()
    {
        if (_initSuccess)
        {
            try
            {
                return uhfAPI.StopInventory();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        return false;
    }

    public string GetTagFromBuffer()
    {
        var result = string.Empty;
        if (_initSuccess)
        {
            try
            {
                string[] res = uhfAPI.ReadTagFromBuffer();
                if (res != null && res.Length >= 2)
                {
                    /*
                    string strEPC;
                    string strTid = "";
                    StringBuilder sb = new StringBuilder();
                    if (res[0] != "0000000000000000")
                    {
                        strTid = "TID:" + res[0] + "\r\n";
                    }
                    strEPC = "EPC:" + uhfAPI.ConvertUiiToEPC(res[1]) + "@";
                    sb.Append(strTid);
                    sb.Append(strEPC);
                    sb.Append(res[2]);
                    */

                    result = uhfAPI.ConvertUiiToEPC(res[1]);
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
        return result;
    }

    public bool InitSuccess()
    {
        return _initSuccess;
    }

    public void Dispose()
    {
        if (uhfAPI != null)
        {
            try
            {
                _initSuccess = !uhfAPI.Free();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }

    public ScannerC72()
    {
        Init();
    }

    public bool Init()
    {
        try
        {
            uhfAPI = RFIDWithUHF.Instance;
            uhfAPI.StopInventory();
            _initSuccess = uhfAPI.Init();
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
        }

        return _initSuccess;
    }
}