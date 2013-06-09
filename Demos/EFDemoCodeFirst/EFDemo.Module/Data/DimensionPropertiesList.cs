using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EFDemo.Module.Data {
	public class DimensionPropertiesList : Collection<String> {
		protected override void ClearItems() {
			base.ClearItems();
			OnListChanged();
		}
		protected override void InsertItem(Int32 index, String item) {
			base.InsertItem(index, item);
			OnListChanged();
		}
		protected override void RemoveItem(Int32 index) {
			base.RemoveItem(index);
			OnListChanged();
		}
		protected override void SetItem(Int32 index, String item) {
			base.SetItem(index, item);
			OnListChanged();
		}
		protected virtual void OnListChanged() {
			if(ListChanged != null) {
				ListChanged(this, EventArgs.Empty);
			}
		}
		public void ResetList() {
			OnListChanged();
		}
		public event EventHandler ListChanged;
	}
}
