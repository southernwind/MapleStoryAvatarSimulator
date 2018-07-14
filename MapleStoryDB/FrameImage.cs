namespace MapleStoryDB {
	public class FrameImage {
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

		/// <summary>
		/// 自身のイメージノードのパス
		/// </summary>
		public string FullPath {
			get;
			set;
		}

		/// <summary>
		/// 参照先イメージノードのパス
		/// Wz_Pngの場合は自身のイメージノードのパスと同一
		/// Wz_Uolの場合は参照先のものに変わる
		/// </summary>
		public string ImageLogicalPath {
			get;
			set;
		}

		public Frame Frame {
			get;
			set;
		}

		public Image Image {
			get;
			set;
		}
	}
}
