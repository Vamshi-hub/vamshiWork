using System;
using Com.Rscja.Deviceapi;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;
using Android.Media;

namespace astorTrackP.Droid
{
    public class ChainwayRFIDReader : IRFIDUHFReader
    {
        #region Declarations

        private object GlobalLock = new object();
        private bool RFOperationIsActive = false;
        private bool isInitialized = false;
        private CancellationTokenSource cancellationTS = null;
        private RFIDWithUHF rf = null;

        #endregion

        private void StandardStart()
        {
            lock (GlobalLock)
            {
                if (RFOperationIsActive) throw new ReaderBusyException();
                if (!isInitialized) throw new ReaderNotInitializedException();
                RFOperationIsActive = true;
                cancellationTS = new CancellationTokenSource();
            }
        }

        private void StandardStop()
        {
            lock (GlobalLock)
            {
                if (cancellationTS != null)
                    cancellationTS.Cancel();
                cancellationTS = null;
                RFOperationIsActive = false;
            }
        }

        public Task<bool> InitializeReaderAsync()  //<-- run one time when application load
        {
            

            if (RFOperationIsActive)
            {
                CancelOperation();
                Thread.Sleep(100);
            }
            lock (GlobalLock)
            {
                if (RFOperationIsActive) throw new ReaderBusyException();
                if (isInitialized) return Task.FromResult(true);
                RFOperationIsActive = true;
                cancellationTS = new CancellationTokenSource();
            }
            CancellationToken token = cancellationTS.Token;
            TaskFactory factory = new TaskFactory(token);
            var tsk = factory.StartNew<bool>(() =>
                {
                    try
                    {
                        if (cancellationTS.IsCancellationRequested)
                            return false;

                        if (rf != null)
                            if (!rf.IsPowerOn)
                                CloseReader();

                        rf = RFIDWithUHF.GetInstance();
                        
                        if (rf != null)
                        {
                            isInitialized = rf.Init();
                            rf.SetPower(App.RFIDPower);
                            return isInitialized;
                        }
                        return false;
                    }
                    catch
                    {
                        return false;
                    }
                    finally
                    {
                        StandardStop();
                    }
                });

            return tsk;
        }

        public bool IsPowerOn()
        {
            return rf.IsPowerOn;
        }

        public bool StartInventoryTag()
        {
            bool startInventory = false;
            try
            {
                if (rf.IsPowerOn)
                    if (rf.StartInventoryTag((byte)0, (byte)0))
                    {
                        startInventory = true;
                    }
                    else
                    {
                        rf.StopInventory();
                        startInventory = false;
                    }


            }
            catch
            {
                throw;
            }
            finally
            {
                StandardStop();
            }
            return startInventory;
        }

        public void StopInventory()
        {
            try
            {                
                rf.StopInventory();
                StandardStop();
            }
            catch
            {
                throw;
            }
        }

        public Task<string> ReadSingleTagAsync()
        {
            
            string[] res = null;
            cancellationTS = new CancellationTokenSource();
            CancellationToken token = cancellationTS.Token;
            TaskFactory factory = new TaskFactory(token);
            var tsk = factory.StartNew<string>(() =>
                {
                    string tagID = "";
                    try
                    {
                        res = rf.ReadTagFormBuffer();
                        //res = rf.ReadUidFormBuffer();

                        if (res != null)
                        {
                            tagID = res[1].ToString();
                            PlayAlert();
                        }
                        else
                            tagID = "";

                        return tagID;
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    { 
                        StandardStop();
                        //Thread.Sleep(80);
                    }
                });
            return tsk;
        }
                
        public bool SetRFIDPowerAsync(int RFIDpower)
        {
            //         StandardStart();
            //         CancellationToken token = cancellationTS.Token;

            //TaskFactory factory = new TaskFactory(token);
            //var tsk = factory.StartNew<bool>(() =>
            //	{
            bool blnResult = false;
            try
            {

                if (isInitialized)
                    blnResult = rf.SetPower(RFIDpower);
                return blnResult;
            }
            catch
            {
                throw;
            }
            finally
            {
                StandardStop();
            }
            //});
            return blnResult;
        }

        public string ReadSingleTag()
        {
            string[] res = null;

            string tagID = "";
            try
            {
                res = rf.ReadTagFormBuffer();
                if (res != null)
                {
                    tagID = res[1].ToString();
                    PlayAlert();
                }
                else
                    tagID = "";
            }
            catch
            {
                throw;
            }
            return tagID;
        }
                
        public void CancelOperation()
		{
			lock (GlobalLock)
			{
				if (cancellationTS != null) {
					cancellationTS.Cancel ();	
					RFOperationIsActive = false;
                    isInitialized = false;
                }
			}
		}

		public void StopTagRead()
		{
			lock (GlobalLock)
			{
				if (RFOperationIsActive)
				{
					rf.StopInventory();
					StandardStop();
				}
			}
		}

		public void CloseReader()
		{
			if (rf != null) {
				CancelOperation ();
				rf.Free ();
				isInitialized = false;
                StandardStop();
            }
		}

        public bool ReaderInitialized()
        {   
            return isInitialized;
        }

        private MediaPlayer player = null;

        public void PlayAlert()
        {
            try
            {
                if (player != null) player.Release();
                

                player = new Android.Media.MediaPlayer();

                if (player.IsPlaying)
                {
                    player.Stop();
                    player.Release();
                    player = new MediaPlayer();
                }

                var fd = global::Android.App.Application.Context.Assets.OpenFd("beep.mp3");
                player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.Prepare();                
                player.SetVolume(0.3f, 0.3f);                
                player.Start();
            }
            catch (Exception ex)
            {
                var x = ex.StackTrace;
            }
        }
    }
}

