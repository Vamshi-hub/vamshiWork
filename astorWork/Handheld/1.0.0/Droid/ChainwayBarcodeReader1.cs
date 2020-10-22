using System;
using Com.Rscja.Deviceapi;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;


namespace astorTrackP.Droid
{
	public class ChainwayBarcodeReader1 
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
				barcode.Close ();
			}
		}

		/// <summary>
		/// Initializes the RFIDUHFReader if present.
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
						return false;
					}
					catch
					{
						return false;
					}
					finally
					{
						lock (GlobalLock)
						{
							cancellationTS = null;
							BarcodeOperationIsActive = false;
						}
					}
				});
			return tsk;
		}

		public void CloseReader()
		{
			if (BarcodeOperationIsActive)
			{
				CancelOperation();
				Thread.Sleep(4000);
			}
		}

		public Task<string> ReadSingleTagAsync()
		{
			StandardStart();

			CancellationToken token = cancellationTS.Token;
			TaskFactory factory = new TaskFactory(token);
			var tsk = factory.StartNew<string>(() =>
				{
					try
					{
						barcode.Open();
						return barcode.Scan();
					}
					catch
					{
						throw;
					}
					finally
					{
						StandardStop();
					}
				});
			return tsk;
		}


		/// <summary>
		/// Cancels the current RFID operation
		/// </summary>
		public void CancelOperation()
		{
			if (!isInitialized) throw new ReaderNotInitializedException();
			lock (GlobalLock)
			{
				if (cancellationTS != null) cancellationTS.Cancel();
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
					barcode.StopScan ();
					StandardStop();

				}
			}
		}


	}
}

