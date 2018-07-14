using System.Collections.Generic;

namespace MapleStoryDB {
	public class Image {
		public string ImageLogicalPath {
			get;
			set;
		}

		public int? OriginX {
			get;
			set;
		}

		public int? OriginY {
			get;
			set;
		}

		public string Z {
			get;
			set;
		}

		public string Group {
			get;
			set;
		}

		public string Hash {
			get;
			set;
		}

		public string ImageActualPath {
			get;
			set;
		}

		public ICollection<FrameImage> FrameImages {
			get;
			set;
		}

		public ICollection<Map> Maps {
			get;
			set;
		}
	}
}
