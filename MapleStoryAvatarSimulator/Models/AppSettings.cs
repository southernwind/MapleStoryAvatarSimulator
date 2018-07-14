using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapleStoryAvatarSimulator.Models
{
	public class AppSettings {
		public DbConnectionInformation DbConnectionInformation {
			get;
			set;
		}
	}

	public class DbConnectionInformation {
		public string Server {
			get;
			set;
		}

		public uint Port {
			get;
			set;
		}

		public string DataBase {
			get;
			set;
		}

		public string User {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}
	}
}
