using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {

	public interface IModelMemberDragDrop:IModelNode {
		[ModelReadOnly(typeof(ModelPropertyEditorDragDropReadonlyCalculator))]
		[Category(AttributeCategoryNameProvider.Xpand)]
		bool EnableDragDrop { get; set; }
	}
	[ModelInterfaceImplementor(typeof(IModelMemberDragDrop), "ModelMember")]
	public interface IModelPropertyEditorDragDrop: IModelMemberDragDrop {
		
	}

	public class ModelPropertyEditorDragDropReadonlyCalculator:IModelIsReadOnly{
		public bool IsReadOnly(IModelNode node, string propertyName){
			return !typeof(ImagePropertyEditor).IsAssignableFrom(((IModelCommonMemberViewItem)node).PropertyEditorType) ;
		}

		public bool IsReadOnly(IModelNode node, IModelNode childNode){
			return IsReadOnly(node, "");
		}
	}

	public class DragNDropImageController:ViewController<DetailView>,IModelExtender{
		protected override void OnActivated(){
			base.OnActivated();
			foreach (var imagePropertyEditor in View.GetItems<ImagePropertyEditor>().Where(editor => ((IModelPropertyEditorDragDrop) editor.Model).EnableDragDrop)){
				imagePropertyEditor.ControlCreated+=ImagePropertyEditorOnControlCreated;
			}
		}

		private void ImagePropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
			var baseEdit = ( ((ImagePropertyEditor) sender).Control);
			new DragDropProvider(baseEdit, image => {
				var xafImageEdit = baseEdit as XafImageEdit;
				if (xafImageEdit != null){
					xafImageEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
					xafImageEdit.Image = image;
				}
				else{
					var xafPictureEdit = ((XafPictureEdit) baseEdit);
					xafPictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
					xafPictureEdit.Image = image;
				}
			}).EnableDragDrop();
			
		}

		public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
			extenders.Add<IModelPropertyEditor,IModelPropertyEditorDragDrop>();
			extenders.Add<IModelMember, IModelMemberDragDrop>();
		}
	}

	public class DragDropProvider {
		Image _draggedImage;
		readonly BaseEdit _edit;
		private readonly Action<Image> _assignImageAction;

		[DllImport("GdiPlus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		private static extern int GdipCreateBitmapFromGdiDib(IntPtr pBih, IntPtr pPix, out IntPtr pBitmap);

		public DragDropProvider(BaseEdit edit,Action<Image> assignImageAction){
			_edit = edit;
			_assignImageAction = assignImageAction;
		}

		public void EnableDragDrop() {
			_edit.AllowDrop = true;
			_edit.DragEnter += OnDragEnter;
			_edit.DragDrop += OnDragDrop;
		}


		private void OnDragEnter(object sender, DragEventArgs e) {
			SetDragDropEffects(e);
		}

		private void OnDragDrop(object sender, DragEventArgs e) {
			if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
				_assignImageAction(_draggedImage);
				_draggedImage = null;
			}
		}

		private void SetDragDropEffects(DragEventArgs e) {
			e.Effect = DragDropEffects.None;

			MemoryStream str = e.Data.GetData(DataFormats.Dib) as MemoryStream;
			if (str != null) {
				try {
					GCHandle handle = GCHandle.Alloc(str.ToArray(), GCHandleType.Pinned);
					_draggedImage = BitmapFromDIB(handle.AddrOfPinnedObject());
					if (_draggedImage != null)
						e.Effect = DragDropEffects.Copy;
				}
				catch {
					_draggedImage = null;
				}
				return;
			}
			string[] fileName = e.Data.GetData("FileName") as string[];
			if (fileName != null) {
				try {
					_draggedImage = Image.FromFile(fileName[0]);
					e.Effect = DragDropEffects.Copy;
				}
				catch {
					_draggedImage = null;
				}
				return;
			}
			object bmp = e.Data.GetData("Bitmap");
			if (bmp != null) {
				_draggedImage = (Image)bmp;
				e.Effect = DragDropEffects.Copy;
				return;
			}
			var data = e.Data.GetData("HTML Format");
			if (data!=null){
				try{
					Match match = Regex.Match(data.ToString(), "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
					if (match.Success){
						var uri = new Uri(match.Groups[1].Value);
                        using var client = new HttpClient();
                        using var memoryStream = new MemoryStream(client.GetByteArrayAsync(uri).Result);
                        _draggedImage = Image.FromStream(memoryStream);
                        e.Effect = DragDropEffects.Copy;
                    }
				}
				catch (Exception){
					_draggedImage = null;
				}
			}
		}

		public Bitmap BitmapFromDIB(IntPtr pDIB) {

			IntPtr pPix = GetPixelInfo(pDIB);
			MethodInfo mi = typeof(Bitmap).GetMethod("FromGDIplus",
				BindingFlags.Static | BindingFlags.NonPublic);
			if (mi == null) return null;
			IntPtr pBmp;
			int status = GdipCreateBitmapFromGdiDib(pDIB, pPix, out pBmp);
			if ((status == 0) && (pBmp != IntPtr.Zero))
				return (Bitmap)mi.Invoke(null, new object[] { pBmp });
			else
				return null;
		}


		private IntPtr GetPixelInfo(IntPtr bmpPtr) {
			BITMAPINFOHEADER bmi = (BITMAPINFOHEADER)Marshal.PtrToStructure(bmpPtr, typeof(BITMAPINFOHEADER));
			if (bmi.biSizeImage == 0)
				bmi.biSizeImage = (uint)(((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * bmi.biHeight);
			int p = (int)bmi.biClrUsed;
			if ((p == 0) && (bmi.biBitCount <= 8))
				p = 1 << bmi.biBitCount; p = (p * 4) + (int)bmi.biSize + (int)bmpPtr;
			return (IntPtr)p;
		}

		public void DisableDragDrop() {
			_edit.DragEnter -= OnDragEnter;
			_edit.DragDrop -= OnDragDrop;
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BITMAPINFOHEADER {
		public uint biSize;
		public int biWidth;
		public int biHeight;
		public ushort biPlanes;
		public ushort biBitCount;
		public uint biCompression;
		public uint biSizeImage;
		public int biXPelsPerMeter;
		public int biYPelsPerMeter;
		public uint biClrUsed;
		public uint biClrImportant;

		public void Init() {
			biSize = (uint)Marshal.SizeOf(this);
		}
	}

}
