using System.Collections.Generic;

namespace MapleStoryDB {
	public class Frame {
		public int EquipmentId {
			get;
			set;
		}

		public string Motion {
			get;
			set;
		}

		public string Pattern {
			get;
			set;
		}

		public int FrameNo {
			get;
			set;
		}

		public int? Face {
			get;
			set;
		}

		public int? Delay {
			get;
			set;
		}

		public int? MoveX {
			get;
			set;
		}

		public int? MoveY {
			get;
			set;
		}

		public int? Rotate {
			get;
			set;
		}

		public int? Flip {
			get;
			set;
		}

		public int? HideName {
			get;
			set;
		}

		/// <summary>
		/// 参照フレーム外部キー[装備ID]
		/// </summary>
		public int? ReferenceFrameEquipmentId {
			get;
			set;
		}

		/// <summary>
		/// 参照フレーム外部キー[動作]
		/// </summary>
		public string ReferenceFrameMotion {
			get;
			set;
		}

		/// <summary>
		/// 参照フレーム外部キー[パターン]
		/// </summary>
		public string ReferenceFramePattern {
			get;
			set;
		}

		/// <summary>
		/// 参照フレーム外部キー[フレーム番号]
		/// </summary>
		public int? ReferenceFrameFrameNo {
			get;
			set;
		}

		public Equipment Equipment {
			get;
			set;
		}

		public ICollection<FrameImage> FrameImages {
			get;
			set;
		}

		/// <summary>
		/// 参照フレーム
		/// </summary>
		public Frame ReferenceFrame {
			get;
			set;
		}
	}
}
