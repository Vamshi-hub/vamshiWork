using System;
using Com.Rscja.Deviceapi;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;
using Android.Media;

namespace astorTrackP.Droid
{
	public class ChainwayBarcodeReader : IBarcodeReader
	{
		#region Declarations

		private object GlobalLock = new object();
		private bool BarcodeOperationIsActive = false;
		private bool isInitialized = false;
		private CancellationTokenSource cancellationTS = null;
		private Barcode1D barcode = null;

		#endregion

		private void StandardStart()
		{
			lock (GlobalLock)
			{
				if (BarcodeOperationIsActive) throw new ReaderBusyException();
				if (!isInitialized) throw new ReaderNotInitializedException();
				BarcodeOperationIsActive = true;
				cancellationTS = new CancellationTokenSource();
			}
		}

		private void StandardStop()
		{
			lock (GlobalLock)
			{
				cancellationTS = null;
				BarcodeOperationIsActive = false;
				
			}
		}

		/// <summary>
		/// Initializes the BarcodeReader if present.
		/// </summary>
		/// <returns>True if initialization was successful, False if not</returns>
		public Task<bool> InitializeReaderAsync()
		{
			if (BarcodeOperationIsActive)
			{
				CancelOperation();
				Thread.Sleep(4000);
			}
			lock (GlobalLock)
			{
				
				if (BarcodeOperationIsActive) throw new ReaderBusyException();
				if (isInitialized) return Task.FromResult(true);
				BarcodeOperationIsActive = true;
				cancellationTS = new CancellationTokenSource();
			}
			CancellationToken token = cancellationTS.Token;
			TaskFactory factory = new TaskFactory(token);
			var tsk = factory.StartNew<bool>(() =>
				{
					try
					{
						barcode = Barcode1D.Instance;
                        if (barcode.Open())
                        {
                            isInitialized = true;
                            return true;
                        }
                        barcode.Close();
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



		public string ReadSingleTagAsync()
		{
			try
			{
                barcode.Open();                        
                string value = barcode.Scan();
                PlayAlert();
                return value;
            }
			catch
			{
				throw;
			}
			finally
			{
				StopTagRead();                
            }			
		}


		/// <summary>
		/// Cancels the current RFID operation
		/// </summary>
		public void CancelOperation()
		{
			if (!isInitialized) throw new ReaderNotInitializedException();
			lock (GlobalLock)
			{
				if (cancellationTS != null) {
					cancellationTS.Cancel ();
				}

			}
		}

		/// <summary>
		/// Stop tag read
		/// </summary>
		public void StopTagRead()
		{
			if (!isInitialized) throw new ReaderNotInitializedException();

			lock (GlobalLock)
			{
				if (BarcodeOperationIsActive)
				{					
					StandardStop();
				}
			}
            barcode.StopScan();
        }

		public void CloseReader()
		{			
			if (barcode != null) {
				CancelOperation ();
				barcode.Close ();
				barcode.Dispose ();
				barcode = null;
				isInitialized = false;
				BarcodeOperationIsActive = false;
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

