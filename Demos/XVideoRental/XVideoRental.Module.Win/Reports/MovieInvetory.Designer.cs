namespace XVideoRental.Module.Win.Reports {
    partial class MovieInvetory {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            this.xrTableRowGenreInfo = new DevExpress.XtraReports.UI.XRTableRow();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrTableCellRatingLable = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableFilmInfo = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableCellYear = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellRating = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabelMovieTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanelFilmInfoCard = new DevExpress.XtraReports.UI.XRPanel();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrTableCellGenreLable = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellYearLable = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellLocationLable = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRowRatingInfo = new DevExpress.XtraReports.UI.XRTableRow();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrBarCodeMoviItemId = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrTableRowYearInfo = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableRowLocationInfo = new DevExpress.XtraReports.UI.XRTableRow();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrTableCellLocation = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellGenre = new DevExpress.XtraReports.UI.XRTableCell();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.collectionDataSource1 = new DevExpress.Persistent.Base.ReportsV2.CollectionDataSource();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // xrTableRowGenreInfo
            // 
            this.xrTableRowGenreInfo.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellGenreLable,
            this.xrTableCellGenre});
            this.xrTableRowGenreInfo.Dpi = 100F;
            this.xrTableRowGenreInfo.Name = "xrTableRowGenreInfo";
            this.xrTableRowGenreInfo.Weight = 0.17450354620551689D;
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.BorderColor = System.Drawing.Color.Black;
            this.Title.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.Title.BorderWidth = 1F;
            this.Title.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold);
            this.Title.ForeColor = System.Drawing.Color.Maroon;
            this.Title.Name = "Title";
            // 
            // detailBand1
            // 
            this.detailBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanelFilmInfoCard});
            this.detailBand1.Dpi = 100F;
            this.detailBand1.HeightF = 169.375F;
            this.detailBand1.MultiColumn.ColumnCount = 2;
            this.detailBand1.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount;
            this.detailBand1.Name = "detailBand1";
            this.detailBand1.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Movie.MovieTitle", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            // 
            // PageInfo
            // 
            this.PageInfo.BackColor = System.Drawing.Color.Transparent;
            this.PageInfo.BorderColor = System.Drawing.Color.Black;
            this.PageInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.PageInfo.BorderWidth = 1F;
            this.PageInfo.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.PageInfo.ForeColor = System.Drawing.Color.Black;
            this.PageInfo.Name = "PageInfo";
            // 
            // xrTableCellRatingLable
            // 
            this.xrTableCellRatingLable.CanGrow = false;
            this.xrTableCellRatingLable.Dpi = 100F;
            this.xrTableCellRatingLable.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellRatingLable.Name = "xrTableCellRatingLable";
            this.xrTableCellRatingLable.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100F);
            this.xrTableCellRatingLable.StylePriority.UseFont = false;
            this.xrTableCellRatingLable.StylePriority.UsePadding = false;
            this.xrTableCellRatingLable.StylePriority.UseTextAlignment = false;
            this.xrTableCellRatingLable.Text = "RATING:";
            this.xrTableCellRatingLable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellRatingLable.Weight = 0.875170230911391D;
            // 
            // xrTableFilmInfo
            // 
            this.xrTableFilmInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableFilmInfo.BorderWidth = 1F;
            this.xrTableFilmInfo.Dpi = 100F;
            this.xrTableFilmInfo.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.xrTableFilmInfo.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 51.87505F);
            this.xrTableFilmInfo.Name = "xrTableFilmInfo";
            this.xrTableFilmInfo.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRowGenreInfo,
            this.xrTableRowYearInfo,
            this.xrTableRowRatingInfo,
            this.xrTableRowLocationInfo});
            this.xrTableFilmInfo.SizeF = new System.Drawing.SizeF(255.097F, 80F);
            this.xrTableFilmInfo.StylePriority.UseBorders = false;
            this.xrTableFilmInfo.StylePriority.UseBorderWidth = false;
            this.xrTableFilmInfo.StylePriority.UseFont = false;
            this.xrTableFilmInfo.StylePriority.UseTextAlignment = false;
            this.xrTableFilmInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTableCellYear
            // 
            this.xrTableCellYear.CanGrow = false;
            this.xrTableCellYear.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Movie.ReleaseDate", "{0:yyyy}")});
            this.xrTableCellYear.Dpi = 100F;
            this.xrTableCellYear.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellYear.Multiline = true;
            this.xrTableCellYear.Name = "xrTableCellYear";
            this.xrTableCellYear.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 0, 0, 0, 100F);
            this.xrTableCellYear.StylePriority.UseFont = false;
            this.xrTableCellYear.StylePriority.UsePadding = false;
            this.xrTableCellYear.Text = "xrTableCellYear";
            this.xrTableCellYear.Weight = 2.1248297690886089D;
            // 
            // xrTableCellRating
            // 
            this.xrTableCellRating.CanGrow = false;
            this.xrTableCellRating.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Movie.Rating")});
            this.xrTableCellRating.Dpi = 100F;
            this.xrTableCellRating.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellRating.Multiline = true;
            this.xrTableCellRating.Name = "xrTableCellRating";
            this.xrTableCellRating.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 0, 0, 0, 100F);
            this.xrTableCellRating.StylePriority.UseFont = false;
            this.xrTableCellRating.StylePriority.UsePadding = false;
            this.xrTableCellRating.Text = "xrTableCellRating";
            this.xrTableCellRating.Weight = 2.1248297690886089D;
            // 
            // xrLabelMovieTitle
            // 
            this.xrLabelMovieTitle.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabelMovieTitle.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Movie.Title")});
            this.xrLabelMovieTitle.Dpi = 100F;
            this.xrLabelMovieTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrLabelMovieTitle.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 10.00001F);
            this.xrLabelMovieTitle.Name = "xrLabelMovieTitle";
            this.xrLabelMovieTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 100F);
            this.xrLabelMovieTitle.SizeF = new System.Drawing.SizeF(255.097F, 31.33333F);
            this.xrLabelMovieTitle.StylePriority.UseBorders = false;
            this.xrLabelMovieTitle.StylePriority.UseFont = false;
            this.xrLabelMovieTitle.StylePriority.UsePadding = false;
            this.xrLabelMovieTitle.StylePriority.UseTextAlignment = false;
            this.xrLabelMovieTitle.Text = "xrLabelMovieTitle";
            this.xrLabelMovieTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrPanelFilmInfoCard
            // 
            this.xrPanelFilmInfoCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(224)))), ((int)(((byte)(204)))));
            this.xrPanelFilmInfoCard.BorderColor = System.Drawing.Color.White;
            this.xrPanelFilmInfoCard.Borders = DevExpress.XtraPrinting.BorderSide.Left;
            this.xrPanelFilmInfoCard.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabelMovieTitle,
            this.xrTableFilmInfo,
            this.xrBarCodeMoviItemId});
            this.xrPanelFilmInfoCard.Dpi = 100F;
            this.xrPanelFilmInfoCard.LocationFloat = new DevExpress.Utils.PointFloat(10F, 0F);
            this.xrPanelFilmInfoCard.Name = "xrPanelFilmInfoCard";
            this.xrPanelFilmInfoCard.SizeF = new System.Drawing.SizeF(354.3334F, 160F);
            this.xrPanelFilmInfoCard.StylePriority.UseBackColor = false;
            this.xrPanelFilmInfoCard.StylePriority.UseBorderColor = false;
            this.xrPanelFilmInfoCard.StylePriority.UseBorders = false;
            // 
            // DataField
            // 
            this.DataField.BackColor = System.Drawing.Color.Transparent;
            this.DataField.BorderColor = System.Drawing.Color.Black;
            this.DataField.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.DataField.BorderWidth = 1F;
            this.DataField.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.DataField.ForeColor = System.Drawing.Color.Black;
            this.DataField.Name = "DataField";
            this.DataField.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // xrTableCellGenreLable
            // 
            this.xrTableCellGenreLable.Dpi = 100F;
            this.xrTableCellGenreLable.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellGenreLable.Name = "xrTableCellGenreLable";
            this.xrTableCellGenreLable.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100F);
            this.xrTableCellGenreLable.StylePriority.UseFont = false;
            this.xrTableCellGenreLable.StylePriority.UsePadding = false;
            this.xrTableCellGenreLable.StylePriority.UseTextAlignment = false;
            this.xrTableCellGenreLable.Text = "GENRE:";
            this.xrTableCellGenreLable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellGenreLable.Weight = 0.875170230911391D;
            // 
            // xrTableCellYearLable
            // 
            this.xrTableCellYearLable.CanGrow = false;
            this.xrTableCellYearLable.Dpi = 100F;
            this.xrTableCellYearLable.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellYearLable.Name = "xrTableCellYearLable";
            this.xrTableCellYearLable.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100F);
            this.xrTableCellYearLable.StylePriority.UseFont = false;
            this.xrTableCellYearLable.StylePriority.UsePadding = false;
            this.xrTableCellYearLable.StylePriority.UseTextAlignment = false;
            this.xrTableCellYearLable.Text = "YEAR:";
            this.xrTableCellYearLable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellYearLable.Weight = 0.875170230911391D;
            // 
            // xrTableCellLocationLable
            // 
            this.xrTableCellLocationLable.CanGrow = false;
            this.xrTableCellLocationLable.Dpi = 100F;
            this.xrTableCellLocationLable.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellLocationLable.Name = "xrTableCellLocationLable";
            this.xrTableCellLocationLable.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100F);
            this.xrTableCellLocationLable.StylePriority.UseFont = false;
            this.xrTableCellLocationLable.StylePriority.UsePadding = false;
            this.xrTableCellLocationLable.StylePriority.UseTextAlignment = false;
            this.xrTableCellLocationLable.Text = "LOCATION:";
            this.xrTableCellLocationLable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellLocationLable.Weight = 0.875170230911391D;
            // 
            // xrTableRowRatingInfo
            // 
            this.xrTableRowRatingInfo.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellRatingLable,
            this.xrTableCellRating});
            this.xrTableRowRatingInfo.Dpi = 100F;
            this.xrTableRowRatingInfo.Name = "xrTableRowRatingInfo";
            this.xrTableRowRatingInfo.Weight = 0.17450354098189264D;
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.Dpi = 100F;
            this.topMarginBand1.HeightF = 100F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // xrBarCodeMoviItemId
            // 
            this.xrBarCodeMoviItemId.AutoModule = true;
            this.xrBarCodeMoviItemId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(223)))), ((int)(((byte)(203)))));
            this.xrBarCodeMoviItemId.BarCodeOrientation = DevExpress.XtraPrinting.BarCode.BarCodeOrientation.RotateLeft;
            this.xrBarCodeMoviItemId.BorderColor = System.Drawing.Color.White;
            this.xrBarCodeMoviItemId.Borders = DevExpress.XtraPrinting.BorderSide.Left;
            this.xrBarCodeMoviItemId.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "MovieItemId", "{0:000000}")});
            this.xrBarCodeMoviItemId.Dpi = 100F;
            this.xrBarCodeMoviItemId.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrBarCodeMoviItemId.LocationFloat = new DevExpress.Utils.PointFloat(275.5136F, 3.33786E-06F);
            this.xrBarCodeMoviItemId.Name = "xrBarCodeMoviItemId";
            this.xrBarCodeMoviItemId.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 5, 10, 10, 100F);
            this.xrBarCodeMoviItemId.SizeF = new System.Drawing.SizeF(78.81982F, 160F);
            this.xrBarCodeMoviItemId.StylePriority.UseBackColor = false;
            this.xrBarCodeMoviItemId.StylePriority.UseBorderColor = false;
            this.xrBarCodeMoviItemId.StylePriority.UseBorders = false;
            this.xrBarCodeMoviItemId.StylePriority.UseFont = false;
            this.xrBarCodeMoviItemId.StylePriority.UsePadding = false;
            this.xrBarCodeMoviItemId.StylePriority.UseTextAlignment = false;
            this.xrBarCodeMoviItemId.Symbology = code128Generator1;
            this.xrBarCodeMoviItemId.Text = "xrBarCodeMoviItemId";
            this.xrBarCodeMoviItemId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTableRowYearInfo
            // 
            this.xrTableRowYearInfo.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellYearLable,
            this.xrTableCellYear});
            this.xrTableRowYearInfo.Dpi = 100F;
            this.xrTableRowYearInfo.Name = "xrTableRowYearInfo";
            this.xrTableRowYearInfo.Weight = 0.17450354408026392D;
            // 
            // xrTableRowLocationInfo
            // 
            this.xrTableRowLocationInfo.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellLocationLable,
            this.xrTableCellLocation});
            this.xrTableRowLocationInfo.Dpi = 100F;
            this.xrTableRowLocationInfo.Name = "xrTableRowLocationInfo";
            this.xrTableRowLocationInfo.Weight = 0.17450354098189264D;
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Dpi = 100F;
            this.bottomMarginBand1.HeightF = 100F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // xrTableCellLocation
            // 
            this.xrTableCellLocation.CanGrow = false;
            this.xrTableCellLocation.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Location")});
            this.xrTableCellLocation.Dpi = 100F;
            this.xrTableCellLocation.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellLocation.Multiline = true;
            this.xrTableCellLocation.Name = "xrTableCellLocation";
            this.xrTableCellLocation.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 0, 0, 0, 100F);
            this.xrTableCellLocation.StylePriority.UseFont = false;
            this.xrTableCellLocation.StylePriority.UsePadding = false;
            this.xrTableCellLocation.Text = "xrTableCellLocation";
            this.xrTableCellLocation.Weight = 2.1248297690886089D;
            // 
            // xrTableCellGenre
            // 
            this.xrTableCellGenre.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Movie.Genre")});
            this.xrTableCellGenre.Dpi = 100F;
            this.xrTableCellGenre.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellGenre.Multiline = true;
            this.xrTableCellGenre.Name = "xrTableCellGenre";
            this.xrTableCellGenre.Padding = new DevExpress.XtraPrinting.PaddingInfo(3, 0, 0, 0, 100F);
            this.xrTableCellGenre.StylePriority.UseFont = false;
            this.xrTableCellGenre.StylePriority.UsePadding = false;
            this.xrTableCellGenre.Text = "xrTableCellGenre";
            this.xrTableCellGenre.Weight = 2.1248297690886089D;
            // 
            // FieldCaption
            // 
            this.FieldCaption.BackColor = System.Drawing.Color.Transparent;
            this.FieldCaption.BorderColor = System.Drawing.Color.Black;
            this.FieldCaption.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.FieldCaption.BorderWidth = 1F;
            this.FieldCaption.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FieldCaption.ForeColor = System.Drawing.Color.Maroon;
            this.FieldCaption.Name = "FieldCaption";
            // 
            // collectionDataSource1
            // 
            this.collectionDataSource1.Name = "collectionDataSource1";
            this.collectionDataSource1.ObjectTypeName = "XVideoRental.Module.Win.BusinessObjects.Movie.MovieItem";
            this.collectionDataSource1.TopReturnedRecords = 0;
            // 
            // MovieInvetory
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.detailBand1,
            this.topMarginBand1,
            this.bottomMarginBand1});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.collectionDataSource1});
            this.DataSource = this.collectionDataSource1;
            this.DisplayName = "Movie Invetory";
            this.Extensions.Add("DataSerializationExtension", "XafReport");
            this.Extensions.Add("DataEditorExtension", "XafReport");
            this.Extensions.Add("ParameterEditorExtension", "XafReport");
            this.FilterString = "[Status] <> \'Sold\' And [Status] <> \'Damaged\' And [Status] <> \'Lost\'";
            this.Margins = new System.Drawing.Printing.Margins(36, 41, 100, 100);
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "16.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.XRTableRow xrTableRowGenreInfo;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellGenreLable;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellGenre;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.XRPanel xrPanelFilmInfoCard;
        private DevExpress.XtraReports.UI.XRLabel xrLabelMovieTitle;
        private DevExpress.XtraReports.UI.XRTable xrTableFilmInfo;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowYearInfo;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellYearLable;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellYear;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowRatingInfo;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellRatingLable;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellRating;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowLocationInfo;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellLocationLable;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellLocation;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCodeMoviItemId;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.UI.XRControlStyle DataField;
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRControlStyle FieldCaption;
        protected DevExpress.Persistent.Base.ReportsV2.CollectionDataSource collectionDataSource1;
    }
}
