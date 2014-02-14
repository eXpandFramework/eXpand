using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems {
    [ModelAbstractClass]
    public interface IModelMemberViewItemRepositoryItem : IModelMemberViewItem {
        IModelRepositoryItems RepositoryItems { get; }
    }

    public interface IModelRepositoryItems : IModelNode, IModelList<IModelRepositoryItem> {
    }

    [DomainLogic(typeof(IModelRepositoryItemCheckEdit))]
    public class ModelRepositoryItemDomainLogic{
        public static string Get_Id(IModelRepositoryItemCheckEdit modelRepositoryItem) {
            return new FastModelEditorHelper().GetNodeDisplayName(modelRepositoryItem.GetType());
        }
    }
    [ModelDisplayName("Item")]
    [ModelAbstractClass]
    public interface IModelRepositoryItem : IModelNodeEnabled {

    }
    #region RepositoryItem Descenants
    [ModelDisplayName("TextEdit")]
    public interface IModelRepositoryItemTextEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ButtonEdit")]
    public interface IModelRepositoryItemButtonEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("FileDataEdit")]
    public interface IModelRepositoryItemFileDataEdit : IModelRepositoryItem {

    }
    [ModelDisplayName("PopupBase")]
    public interface IModelRepositoryItemPopupBase : IModelRepositoryItem {
    }

    [ModelDisplayName("PopupBaseAutoSearchEdit")]
    public interface IModelRepositoryItemPopupBaseAutoSearchEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ComboBox")]
    public interface IModelRepositoryItemComboBox : IModelRepositoryItem {
    }

    [ModelDisplayName("PopupContainerEdit")]
    public interface IModelRepositoryItemPopupContainerEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("IModelRepositoryFieldPicker")]
    public interface IModelRepositoryFieldPicker : IModelRepositoryItem {
    }

    [ModelDisplayName("PopupExpressionEdit")]
    public interface IModelRepositoryItemPopupExpressionEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("PopupCriteriaEdit")]
    public interface IModelRepositoryItemPopupCriteriaEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ImageComboBox")]
    public interface IModelRepositoryItemImageComboBox : IModelRepositoryItem {
    }

    [ModelDisplayName("CheckEdit")]
    public interface IModelRepositoryItemCheckEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("DateEdit")]
    public interface IModelRepositoryItemDateEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("BaseSpinEdit")]
    public interface IModelRepositoryItemBaseSpinEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("SpinEdit")]
    public interface IModelRepositoryItemSpinEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ObjectEdit")]
    public interface IModelRepositoryItemObjectEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("MemoEdit")]
    public interface IModelRepositoryItemMemoEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("LookupEdit")]
    public interface IModelRepositoryItemLookupEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ProtectedContentTextEdit")]
    public interface IModelRepositoryItemProtectedContentTextEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("BlobBaseEdit")]
    public interface IModelRepositoryItemBlobBaseEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("RtfEditEx")]
    public interface IModelRepositoryItemRtfEditEx : IModelRepositoryItem {
    }

    [ModelDisplayName("HyperLinkEdit")]
    public interface IModelRepositoryItemHyperLinkEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("PictureEdit")]
    public interface IModelRepositoryItemPictureEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("CalcEdit")]
    public interface IModelRepositoryItemCalcEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("CheckedComboBoxEdit")]
    public interface IModelRepositoryItemCheckedComboBoxEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ColorEdit")]
    public interface IModelRepositoryItemColorEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ColorPickEdit")]
    public interface IModelRepositoryItemColorPickEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("FontEdit")]
    public interface IModelRepositoryItemFontEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ImageEdit")]
    public interface IModelRepositoryItemImageEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("LookUpEditBase")]
    public interface IModelRepositoryItemLookUpEditBase : IModelRepositoryItem {
    }

    [ModelDisplayName("LookUpEdit")]
    public interface IModelRepositoryItemLookUpEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("MemoExEdit")]
    public interface IModelRepositoryItemMemoExEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("MRUEdit")]
    public interface IModelRepositoryItemMRUEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("BaseProgressBar")]
    public interface IModelRepositoryItemBaseProgressBar : IModelRepositoryItem {
    }

    [ModelDisplayName("MarqueeProgressBar")]
    public interface IModelRepositoryItemMarqueeProgressBar : IModelRepositoryItem {
    }

    [ModelDisplayName("ProgressBar")]
    public interface IModelRepositoryItemProgressBar : IModelRepositoryItem {
    }

    [ModelDisplayName("RadioGroup")]
    public interface IModelRepositoryItemRadioGroup : IModelRepositoryItem {
    }

    [ModelDisplayName("TrackBar")]
    public interface IModelRepositoryItemTrackBar : IModelRepositoryItem {
    }

    [ModelDisplayName("RangeTrackBar")]
    public interface IModelRepositoryItemRangeTrackBar : IModelRepositoryItem {
    }

    [ModelDisplayName("TimeEdit")]
    public interface IModelRepositoryItemTimeEdit : IModelRepositoryItem {
    }

    [ModelDisplayName("ZoomTrackBar")]
    public interface IModelRepositoryItemZoomTrackBar : IModelRepositoryItem {
    }
    #endregion
}