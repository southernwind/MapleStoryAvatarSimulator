using System.Collections.Generic;

namespace MapleStoryDB {
	public class Equipment {
		public int EquipmentId {
			get;
			set;
		}

		public string EquipmentName {
			get;
			set;
		}

		public string IconImageLogicalPath {
			get;
			set;
		}
		public Image Icon {
			get;
			set;
		}

		public string IconRawImageLogicalPath {
			get;
			set;
		}

		public Image IconRaw {
			get;
			set;
		}

		public string Islot {
			get;
			set;
		}

		public string Vslot {
			get;
			set;
		}

		public int? ReqJob {
			get;
			set;
		}

		public int? ReqLevel {
			get;
			set;
		}

		public int? ReqStr {
			get;
			set;
		}

		public int? ReqDex {
			get;
			set;
		}

		public int? ReqInt {
			get;
			set;
		}

		public int? ReqLuk {
			get;
			set;
		}

		public int? IncPdd {
			get;
			set;
		}

		public int? IncStr {
			get;
			set;
		}

		public int? IncDex {
			get;
			set;
		}

		public int? IncInt {
			get;
			set;
		}

		public int? IncLuk {
			get;
			set;
		}

		public int? Tuc {
			get;
			set;
		}

		public int? Price {
			get;
			set;
		}

		public int? Cash {
			get;
			set;
		}

		public ICollection<Frame> Frames {
			get;
			set;
		}
	}
}
