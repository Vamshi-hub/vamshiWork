using System;
using Com.Rscja.Deviceapi;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;


namespace astorTrackP.Droid
{
	public class ChainwayRFIDReader1
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
				if (RFOperationIsActive) throw new ReaderBusyException ();				
				if (!isInitialized) throw new ReaderNotInitializedException();
				RFOperationIsActive = true;
				cancellationTS = new CancellationTokenSource();
			}
		}

		/// <summary>
		/// Initializes the RFIDUHFReader if present.
		/// </summary>
		/// <returns>True if initialization was successful, False if not</returns>
		public Task<bool> InitializeReaderAsync()
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
			//TaskFactory 
			TaskFactory factory = new TaskFactory(token);
			var tsk = factory.StartNew<bool>(() =>
				{
					try
					{						
						rf = RFIDWithUHF.GetInstance();
						rf.SetPower(App.RFIDPower);
						if (rf.Init())
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
//						lock (GlobalLock)
//						{
//							cancellationTS = null;
//							RFOperationIsActive = false;
//						}
						StandardStop();
					}
				});

			return tsk;
		}

		public void FreeReader()
		{
			if (cancellationTS != null)
				cancellationTS = null;
		}

		public Task<string> ReadSingleTagAsync()
		{
			if (RFOperationIsActive)
			{
//				if (cancellationTS != null)
//					cancellationTS = null;
//				
//				Thread.Sleep(100);

			}
			//CancelOperation();
			StandardStart();
			CancellationToken token = cancellationTS.Token;
			TaskFactory factory = new TaskFactory(token);
			var tsk = factory.StartNew<string>(() =>
				{
					try
					{
						return rf.InventorySingleTag();
					}
					catch
					{
						throw;
					}
					finally
					{
						StopTagRead();
					}
				});
			return tsk;
		}

		public Task<bool> SetRFIDPowerAsync(int RFIDpower)
		{
			StandardStart();
			CancellationToken token = cancellationTS.Token;
			TaskFactory factory = new TaskFactory(token);
			var tsk = factory.StartNew<bool>(() =>
				{
					try
					{
						bool blnResult = false;
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
				if (cancellationTS != null) {
					cancellationTS.Cancel ();	
					RFOperationIsActive = false;
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
				if (RFOperationIsActive)
				{
					rf.StopInventory();
					RFOperationIsActive = false;
					StandardStop();
				}
			}
		}

		private void StandardStop()
		{			
			lock (GlobalLock)
			{
				cancellationTS = null;	
				RFOperationIsActive = false;
			}
		}
	}
}

