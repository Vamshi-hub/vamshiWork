using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorTrackP
{
	#region Exceptions

	public class ReaderNotInitializedException : Exception
	{
		public ReaderNotInitializedException() { }

		public ReaderNotInitializedException(string message) : base(message) { }

		public ReaderNotInitializedException(string message, Exception inner) : base(message, inner) { }
	}

	public class ReaderBusyException : Exception
	{
		public ReaderBusyException() { }

		public ReaderBusyException(string message) : base(message) { }

		public ReaderBusyException(string message, Exception inner) : base(message, inner) { }
	}

	public class OperationCancelledException : Exception
	{
		public OperationCancelledException() { }

		public OperationCancelledException(string message) : base(message) { }

		public OperationCancelledException(string message, Exception inner) : base(message, inner) { }
	}

	public class ReadErrorException : Exception
	{
		public ReadErrorException() { }

		public ReadErrorException(string message) : base(message) { }

		public ReadErrorException(string message, Exception inner) : base(message, inner) { }
	}

	public class TagNotFoundException : Exception
	{
		public TagNotFoundException() { }

		public TagNotFoundException(string message) : base(message) { }

		public TagNotFoundException(string message, Exception inner) : base(message, inner) { }
	}

	public class ReaderFaultException : Exception
	{
		public ReaderFaultException() { }

		public ReaderFaultException(string message) : base(message) { }

		public ReaderFaultException(string message, Exception inner) : base(message, inner) { }
	}

	public class NoMatchFoundException : Exception
	{
		public NoMatchFoundException() { }

		public NoMatchFoundException(string message) : base(message) { }

		public NoMatchFoundException(string message, Exception inner) : base(message, inner) { }
	}

	#endregion

	public interface IBarcodeReader
	{

		/// <summary>
		/// Initializes the RFIDUHFReeader if present.
		/// </summary>
		/// <returns>True if initialization was successful, False if not</returns>
		Task<bool> InitializeReaderAsync();

		/// <summary>
		/// Retrieves the Tag EPC from the RFID Tag.
		/// </summary>
		/// <returns>Characteristics string</returns>
		string ReadSingleTagAsync();

		void CloseReader();

		void StopTagRead();

        bool ReaderInitialized();
    }
}

