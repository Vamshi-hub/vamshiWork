using System;
using SQLite;

namespace astorTrackP
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}

