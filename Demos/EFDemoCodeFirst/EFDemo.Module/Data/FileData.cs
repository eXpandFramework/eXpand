using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace EFDemo.Module.Data {
	[DefaultProperty("FileName")]
	public class FileData : IFileData, IEmptyCheckable {
		private Byte[] content;
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public Int32 Size { get; set; }
		public String FileName { get; set; }
		public Byte[] Content {
			get { return content; }
			set {
				if(content != value) {
					content = value;
					if(content != null) {
						Size = content.Length;
					}
					else {
						Size = 0;
					}
				}
			}
		}

		[NotMapped, Browsable(false)]
		public Boolean IsEmpty {
			get { return String.IsNullOrEmpty(FileName); }
		}
	
		public void LoadFromStream(String fileName, Stream stream) {
			FileName = fileName;
			Byte[] bytes = new Byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			Content = bytes;
		}
		public void SaveToStream(Stream stream) {
			if(String.IsNullOrEmpty(FileName)) {
				throw new InvalidOperationException();
			}
			stream.Write(Content, 0, Size);
			stream.Flush();
		}
		public void Clear() {
			Content = null;
			FileName = "";
		}
		public override String ToString() {
			return FileName;
		}
	}
}
