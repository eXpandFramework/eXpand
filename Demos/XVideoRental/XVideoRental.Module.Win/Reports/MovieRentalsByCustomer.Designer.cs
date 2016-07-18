namespace XVideoRental.Module.Win.Reports {
    partial class MovieRentalsByCustomer {
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
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovieRentalsByCustomer));
            this.xrTableRowMovieTitle = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellMovieTitle = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellPhotoColumnHeader = new DevExpress.XtraReports.UI.XRTableCell();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrTableCellReturnedOn = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabelHeader = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabelCustomerFullName = new DevExpress.XtraReports.UI.XRLabel();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrPageInfoPageCount = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrTableCellRentStartDateColumnHeader = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableFilms = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRowFilmInfo = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellPhoto = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPictureBoxPhoto = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrTableCellTitle = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableMovieInfo = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRowMovieFormat = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellMovieFormat = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRowPlot = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellPlot = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellRentedOn = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellExpectedOn = new DevExpress.XtraReports.UI.XRTableCell();
            this.reportHeaderBand1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrPictureBoxCustomerPhoto = new DevExpress.XtraReports.UI.XRPictureBox();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrTableCellExpectedOnColumnHeader = new DevExpress.XtraReports.UI.XRTableCell();
            this.DetailReport1 = new DevExpress.XtraReports.UI.DetailReportBand();
            this.DetailRents = new DevExpress.XtraReports.UI.DetailBand();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.paramShowMode = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrTableCellRentEndDateColumnHeader = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRowColumnHeaders = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellFilmInfoColumnHeader = new DevExpress.XtraReports.UI.XRTableCell();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTableFilmsColumnHeaders = new DevExpress.XtraReports.UI.XRTable();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.collectionDataSource1 = new DevExpress.Persistent.Base.ReportsV2.CollectionDataSource();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableMovieInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmsColumnHeaders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // xrTableRowMovieTitle
            // 
            this.xrTableRowMovieTitle.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellMovieTitle});
            this.xrTableRowMovieTitle.Dpi = 100F;
            this.xrTableRowMovieTitle.Name = "xrTableRowMovieTitle";
            this.xrTableRowMovieTitle.Weight = 0.43916619757478564D;
            // 
            // xrTableCellMovieTitle
            // 
            this.xrTableCellMovieTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.xrTableCellMovieTitle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrTableCellMovieTitle.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellMovieTitle.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Receipts.Rents.Item.Movie.MovieTitle")});
            this.xrTableCellMovieTitle.Dpi = 100F;
            this.xrTableCellMovieTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.xrTableCellMovieTitle.Name = "xrTableCellMovieTitle";
            this.xrTableCellMovieTitle.StylePriority.UseBackColor = false;
            this.xrTableCellMovieTitle.StylePriority.UseBorderColor = false;
            this.xrTableCellMovieTitle.StylePriority.UseBorders = false;
            this.xrTableCellMovieTitle.StylePriority.UseFont = false;
            this.xrTableCellMovieTitle.Text = "xrTableCellMovieTitle";
            this.xrTableCellMovieTitle.Weight = 2.5568376159667969D;
            // 
            // xrTableCellPhotoColumnHeader
            // 
            this.xrTableCellPhotoColumnHeader.CanGrow = false;
            this.xrTableCellPhotoColumnHeader.Dpi = 100F;
            this.xrTableCellPhotoColumnHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrTableCellPhotoColumnHeader.Name = "xrTableCellPhotoColumnHeader";
            this.xrTableCellPhotoColumnHeader.StylePriority.UseFont = false;
            this.xrTableCellPhotoColumnHeader.StylePriority.UseTextAlignment = false;
            this.xrTableCellPhotoColumnHeader.Text = "Cover";
            this.xrTableCellPhotoColumnHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellPhotoColumnHeader.Weight = 0.52294987794682846D;
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
            // xrTableCellReturnedOn
            // 
            this.xrTableCellReturnedOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.xrTableCellReturnedOn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrTableCellReturnedOn.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Receipts.Rents.ReturnedOn", "{0:d}")});
            this.xrTableCellReturnedOn.Dpi = 100F;
            this.xrTableCellReturnedOn.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellReturnedOn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(97)))), ((int)(((byte)(98)))));
            this.xrTableCellReturnedOn.Multiline = true;
            this.xrTableCellReturnedOn.Name = "xrTableCellReturnedOn";
            this.xrTableCellReturnedOn.StylePriority.UseBackColor = false;
            this.xrTableCellReturnedOn.StylePriority.UseBorderColor = false;
            this.xrTableCellReturnedOn.StylePriority.UseFont = false;
            this.xrTableCellReturnedOn.StylePriority.UseForeColor = false;
            this.xrTableCellReturnedOn.StylePriority.UseTextAlignment = false;
            this.xrTableCellReturnedOn.Text = "xrTableCellRentEndDate";
            this.xrTableCellReturnedOn.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellReturnedOn.Weight = 0.42895777772454391D;
            // 
            // xrLabelHeader
            // 
            this.xrLabelHeader.Dpi = 100F;
            this.xrLabelHeader.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.xrLabelHeader.LocationFloat = new DevExpress.Utils.PointFloat(75F, 0F);
            this.xrLabelHeader.Name = "xrLabelHeader";
            this.xrLabelHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelHeader.SizeF = new System.Drawing.SizeF(500.0001F, 35F);
            this.xrLabelHeader.StylePriority.UseFont = false;
            this.xrLabelHeader.StylePriority.UseTextAlignment = false;
            this.xrLabelHeader.Text = "Movie Rentals by Customer";
            this.xrLabelHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabelCustomerFullName
            // 
            this.xrLabelCustomerFullName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "FullName"),
            new DevExpress.XtraReports.UI.XRBinding("Bookmark", null, "FullName")});
            this.xrLabelCustomerFullName.Dpi = 100F;
            this.xrLabelCustomerFullName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.xrLabelCustomerFullName.LocationFloat = new DevExpress.Utils.PointFloat(62.5F, 127.5F);
            this.xrLabelCustomerFullName.Name = "xrLabelCustomerFullName";
            this.xrLabelCustomerFullName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelCustomerFullName.SizeF = new System.Drawing.SizeF(515.5834F, 20.00003F);
            this.xrLabelCustomerFullName.StylePriority.UseFont = false;
            this.xrLabelCustomerFullName.StylePriority.UseTextAlignment = false;
            this.xrLabelCustomerFullName.Text = "xrLabelCustomerFullName";
            this.xrLabelCustomerFullName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfoPageCount});
            this.bottomMarginBand1.Dpi = 100F;
            this.bottomMarginBand1.HeightF = 100F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // xrPageInfoPageCount
            // 
            this.xrPageInfoPageCount.Dpi = 100F;
            this.xrPageInfoPageCount.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrPageInfoPageCount.LocationFloat = new DevExpress.Utils.PointFloat(601.0001F, 0F);
            this.xrPageInfoPageCount.Name = "xrPageInfoPageCount";
            this.xrPageInfoPageCount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfoPageCount.SizeF = new System.Drawing.SizeF(99.99994F, 23.00002F);
            this.xrPageInfoPageCount.StylePriority.UseFont = false;
            this.xrPageInfoPageCount.StylePriority.UseTextAlignment = false;
            this.xrPageInfoPageCount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrTableCellRentStartDateColumnHeader
            // 
            this.xrTableCellRentStartDateColumnHeader.CanGrow = false;
            this.xrTableCellRentStartDateColumnHeader.Dpi = 100F;
            this.xrTableCellRentStartDateColumnHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrTableCellRentStartDateColumnHeader.Name = "xrTableCellRentStartDateColumnHeader";
            this.xrTableCellRentStartDateColumnHeader.StylePriority.UseFont = false;
            this.xrTableCellRentStartDateColumnHeader.StylePriority.UseTextAlignment = false;
            this.xrTableCellRentStartDateColumnHeader.Text = "Rented on";
            this.xrTableCellRentStartDateColumnHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellRentStartDateColumnHeader.Weight = 0.52294970561954968D;
            // 
            // xrTableFilms
            // 
            this.xrTableFilms.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableFilms.Dpi = 100F;
            this.xrTableFilms.KeepTogether = true;
            this.xrTableFilms.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTableFilms.Name = "xrTableFilms";
            this.xrTableFilms.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRowFilmInfo});
            this.xrTableFilms.SizeF = new System.Drawing.SizeF(700F, 140F);
            this.xrTableFilms.StylePriority.UseBorders = false;
            // 
            // xrTableRowFilmInfo
            // 
            this.xrTableRowFilmInfo.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellPhoto,
            this.xrTableCellTitle,
            this.xrTableCellRentedOn,
            this.xrTableCellExpectedOn,
            this.xrTableCellReturnedOn});
            this.xrTableRowFilmInfo.Dpi = 100F;
            this.xrTableRowFilmInfo.Name = "xrTableRowFilmInfo";
            this.xrTableRowFilmInfo.Weight = 1D;
            // 
            // xrTableCellPhoto
            // 
            this.xrTableCellPhoto.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCellPhoto.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBoxPhoto});
            this.xrTableCellPhoto.Dpi = 100F;
            this.xrTableCellPhoto.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTableCellPhoto.Name = "xrTableCellPhoto";
            this.xrTableCellPhoto.StylePriority.UseBorders = false;
            this.xrTableCellPhoto.StylePriority.UseFont = false;
            this.xrTableCellPhoto.StylePriority.UseTextAlignment = false;
            this.xrTableCellPhoto.Text = "xrTableCellPhoto";
            this.xrTableCellPhoto.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellPhoto.Weight = 0.42895799503931054D;
            // 
            // xrPictureBoxPhoto
            // 
            this.xrPictureBoxPhoto.BackColor = System.Drawing.Color.White;
            this.xrPictureBoxPhoto.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrPictureBoxPhoto.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPictureBoxPhoto.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Image", null, "Receipts.Rents.Item.Movie.Photo")});
            this.xrPictureBoxPhoto.Dpi = 100F;
            this.xrPictureBoxPhoto.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrPictureBoxPhoto.Name = "xrPictureBoxPhoto";
            this.xrPictureBoxPhoto.Padding = new DevExpress.XtraPrinting.PaddingInfo(1, 1, 1, 1, 100F);
            this.xrPictureBoxPhoto.SizeF = new System.Drawing.SizeF(100F, 140F);
            this.xrPictureBoxPhoto.Sizing = DevExpress.XtraPrinting.ImageSizeMode.Squeeze;
            this.xrPictureBoxPhoto.StylePriority.UseBackColor = false;
            this.xrPictureBoxPhoto.StylePriority.UseBorderColor = false;
            this.xrPictureBoxPhoto.StylePriority.UseBorders = false;
            this.xrPictureBoxPhoto.StylePriority.UsePadding = false;
            // 
            // xrTableCellTitle
            // 
            this.xrTableCellTitle.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCellTitle.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTableMovieInfo});
            this.xrTableCellTitle.Dpi = 100F;
            this.xrTableCellTitle.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.xrTableCellTitle.Multiline = true;
            this.xrTableCellTitle.Name = "xrTableCellTitle";
            this.xrTableCellTitle.StylePriority.UseBorders = false;
            this.xrTableCellTitle.StylePriority.UseFont = false;
            this.xrTableCellTitle.StylePriority.UseTextAlignment = false;
            this.xrTableCellTitle.Text = "xrTableCellTitle";
            this.xrTableCellTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellTitle.Weight = 1.2868742439987364D;
            // 
            // xrTableMovieInfo
            // 
            this.xrTableMovieInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableMovieInfo.Dpi = 100F;
            this.xrTableMovieInfo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTableMovieInfo.Name = "xrTableMovieInfo";
            this.xrTableMovieInfo.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRowMovieTitle,
            this.xrTableRowMovieFormat,
            this.xrTableRowPlot});
            this.xrTableMovieInfo.SizeF = new System.Drawing.SizeF(300.0001F, 140F);
            this.xrTableMovieInfo.StylePriority.UseBorders = false;
            // 
            // xrTableRowMovieFormat
            // 
            this.xrTableRowMovieFormat.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellMovieFormat});
            this.xrTableRowMovieFormat.Dpi = 100F;
            this.xrTableRowMovieFormat.Name = "xrTableRowMovieFormat";
            this.xrTableRowMovieFormat.Weight = 0.4391661975747857D;
            // 
            // xrTableCellMovieFormat
            // 
            this.xrTableCellMovieFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.xrTableCellMovieFormat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrTableCellMovieFormat.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellMovieFormat.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Receipts.Rents.Item.Format")});
            this.xrTableCellMovieFormat.Dpi = 100F;
            this.xrTableCellMovieFormat.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.xrTableCellMovieFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(97)))), ((int)(((byte)(98)))));
            this.xrTableCellMovieFormat.Name = "xrTableCellMovieFormat";
            this.xrTableCellMovieFormat.StylePriority.UseBackColor = false;
            this.xrTableCellMovieFormat.StylePriority.UseBorderColor = false;
            this.xrTableCellMovieFormat.StylePriority.UseBorders = false;
            this.xrTableCellMovieFormat.StylePriority.UseFont = false;
            this.xrTableCellMovieFormat.StylePriority.UseForeColor = false;
            this.xrTableCellMovieFormat.Text = "xrTableCellMovieFormat";
            this.xrTableCellMovieFormat.Weight = 2.5568376159667969D;
            // 
            // xrTableRowPlot
            // 
            this.xrTableRowPlot.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellPlot});
            this.xrTableRowPlot.Dpi = 100F;
            this.xrTableRowPlot.Name = "xrTableRowPlot";
            this.xrTableRowPlot.Weight = 1.5809977466414493D;
            // 
            // xrTableCellPlot
            // 
            this.xrTableCellPlot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.xrTableCellPlot.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrTableCellPlot.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellPlot.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Receipts.Rents.Item.Movie.Plot")});
            this.xrTableCellPlot.Dpi = 100F;
            this.xrTableCellPlot.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellPlot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(97)))), ((int)(((byte)(98)))));
            this.xrTableCellPlot.Name = "xrTableCellPlot";
            this.xrTableCellPlot.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.xrTableCellPlot.StylePriority.UseBackColor = false;
            this.xrTableCellPlot.StylePriority.UseBorderColor = false;
            this.xrTableCellPlot.StylePriority.UseBorders = false;
            this.xrTableCellPlot.StylePriority.UseFont = false;
            this.xrTableCellPlot.StylePriority.UseForeColor = false;
            this.xrTableCellPlot.StylePriority.UsePadding = false;
            this.xrTableCellPlot.StylePriority.UseTextAlignment = false;
            this.xrTableCellPlot.Text = "xrTableCellPlot";
            this.xrTableCellPlot.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopJustify;
            this.xrTableCellPlot.Weight = 2.5568376159667969D;
            // 
            // xrTableCellRentedOn
            // 
            this.xrTableCellRentedOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.xrTableCellRentedOn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrTableCellRentedOn.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellRentedOn.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Receipts.Rents.RentedOn", "{0:d}")});
            this.xrTableCellRentedOn.Dpi = 100F;
            this.xrTableCellRentedOn.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellRentedOn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(97)))), ((int)(((byte)(98)))));
            this.xrTableCellRentedOn.Multiline = true;
            this.xrTableCellRentedOn.Name = "xrTableCellRentedOn";
            this.xrTableCellRentedOn.StylePriority.UseBackColor = false;
            this.xrTableCellRentedOn.StylePriority.UseBorderColor = false;
            this.xrTableCellRentedOn.StylePriority.UseBorders = false;
            this.xrTableCellRentedOn.StylePriority.UseFont = false;
            this.xrTableCellRentedOn.StylePriority.UseForeColor = false;
            this.xrTableCellRentedOn.StylePriority.UseTextAlignment = false;
            this.xrTableCellRentedOn.Text = "xrTableCellRentedOn";
            this.xrTableCellRentedOn.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellRentedOn.Weight = 0.42895799100693155D;
            // 
            // xrTableCellExpectedOn
            // 
            this.xrTableCellExpectedOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(246)))));
            this.xrTableCellExpectedOn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(217)))), ((int)(((byte)(219)))));
            this.xrTableCellExpectedOn.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Receipts.Rents.ClearedReturnedOnDate", "{0:d}")});
            this.xrTableCellExpectedOn.Dpi = 100F;
            this.xrTableCellExpectedOn.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.xrTableCellExpectedOn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(97)))), ((int)(((byte)(98)))));
            this.xrTableCellExpectedOn.Multiline = true;
            this.xrTableCellExpectedOn.Name = "xrTableCellExpectedOn";
            this.xrTableCellExpectedOn.StylePriority.UseBackColor = false;
            this.xrTableCellExpectedOn.StylePriority.UseBorderColor = false;
            this.xrTableCellExpectedOn.StylePriority.UseFont = false;
            this.xrTableCellExpectedOn.StylePriority.UseForeColor = false;
            this.xrTableCellExpectedOn.StylePriority.UseTextAlignment = false;
            this.xrTableCellExpectedOn.Text = "[Rents.ClearedReturnedOnDate]";
            this.xrTableCellExpectedOn.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellExpectedOn.Weight = 0.42895800935788148D;
            // 
            // reportHeaderBand1
            // 
            this.reportHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabelHeader});
            this.reportHeaderBand1.Dpi = 100F;
            this.reportHeaderBand1.HeightF = 36.04167F;
            this.reportHeaderBand1.Name = "reportHeaderBand1";
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
            // xrPictureBoxCustomerPhoto
            // 
            this.xrPictureBoxCustomerPhoto.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Image", null, "Photo")});
            this.xrPictureBoxCustomerPhoto.Dpi = 100F;
            this.xrPictureBoxCustomerPhoto.LocationFloat = new DevExpress.Utils.PointFloat(275F, 0F);
            this.xrPictureBoxCustomerPhoto.Name = "xrPictureBoxCustomerPhoto";
            this.xrPictureBoxCustomerPhoto.SizeF = new System.Drawing.SizeF(105F, 125F);
            this.xrPictureBoxCustomerPhoto.Sizing = DevExpress.XtraPrinting.ImageSizeMode.Squeeze;
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
            // xrTableCellExpectedOnColumnHeader
            // 
            this.xrTableCellExpectedOnColumnHeader.CanGrow = false;
            this.xrTableCellExpectedOnColumnHeader.Dpi = 100F;
            this.xrTableCellExpectedOnColumnHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrTableCellExpectedOnColumnHeader.Name = "xrTableCellExpectedOnColumnHeader";
            this.xrTableCellExpectedOnColumnHeader.StylePriority.UseFont = false;
            this.xrTableCellExpectedOnColumnHeader.StylePriority.UseTextAlignment = false;
            this.xrTableCellExpectedOnColumnHeader.Text = "Expected on";
            this.xrTableCellExpectedOnColumnHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellExpectedOnColumnHeader.Weight = 0.52294986247842823D;
            // 
            // DetailReport1
            // 
            this.DetailReport1.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.DetailRents});
            this.DetailReport1.DataMember = "Receipts.OverdueRents";
            this.DetailReport1.DataSource = this.collectionDataSource1;
            this.DetailReport1.Dpi = 100F;
            this.DetailReport1.FilterString = "[Active] = True";
            this.DetailReport1.Level = 0;
            this.DetailReport1.Name = "DetailReport1";
            // 
            // DetailRents
            // 
            this.DetailRents.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTableFilms});
            this.DetailRents.Dpi = 100F;
            this.DetailRents.HeightF = 140F;
            this.DetailRents.Name = "DetailRents";
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.DetailReport1});
            this.DetailReport.DataMember = "Receipts";
            this.DetailReport.DataSource = this.collectionDataSource1;
            this.DetailReport.Dpi = 100F;
            this.DetailReport.FilterString = "[Rents][[Active] = True]";
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            // 
            // Detail
            // 
            this.Detail.Dpi = 100F;
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
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
            // paramShowMode
            // 
            this.paramShowMode.Description = "Details:";
            staticListLookUpSettings1.LookUpValues.Add(new DevExpress.XtraReports.Parameters.LookUpValue(0, "Complete History"));
            staticListLookUpSettings1.LookUpValues.Add(new DevExpress.XtraReports.Parameters.LookUpValue(1, "Non-Return Rentals Only"));
            staticListLookUpSettings1.LookUpValues.Add(new DevExpress.XtraReports.Parameters.LookUpValue(2, "Expired Rentals Only"));
            this.paramShowMode.LookUpSettings = staticListLookUpSettings1;
            this.paramShowMode.Name = "paramShowMode";
            this.paramShowMode.Type = typeof(int);
            this.paramShowMode.ValueInfo = "0";
            // 
            // xrTableCellRentEndDateColumnHeader
            // 
            this.xrTableCellRentEndDateColumnHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellRentEndDateColumnHeader.CanGrow = false;
            this.xrTableCellRentEndDateColumnHeader.Dpi = 100F;
            this.xrTableCellRentEndDateColumnHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrTableCellRentEndDateColumnHeader.Name = "xrTableCellRentEndDateColumnHeader";
            this.xrTableCellRentEndDateColumnHeader.StylePriority.UseBorders = false;
            this.xrTableCellRentEndDateColumnHeader.StylePriority.UseFont = false;
            this.xrTableCellRentEndDateColumnHeader.StylePriority.UseTextAlignment = false;
            this.xrTableCellRentEndDateColumnHeader.Text = "Returned on";
            this.xrTableCellRentEndDateColumnHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellRentEndDateColumnHeader.Weight = 0.52294914954506977D;
            // 
            // xrTableRowColumnHeaders
            // 
            this.xrTableRowColumnHeaders.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellPhotoColumnHeader,
            this.xrTableCellFilmInfoColumnHeader,
            this.xrTableCellRentStartDateColumnHeader,
            this.xrTableCellExpectedOnColumnHeader,
            this.xrTableCellRentEndDateColumnHeader});
            this.xrTableRowColumnHeaders.Dpi = 100F;
            this.xrTableRowColumnHeaders.Name = "xrTableRowColumnHeaders";
            this.xrTableRowColumnHeaders.Weight = 1D;
            // 
            // xrTableCellFilmInfoColumnHeader
            // 
            this.xrTableCellFilmInfoColumnHeader.CanGrow = false;
            this.xrTableCellFilmInfoColumnHeader.Dpi = 100F;
            this.xrTableCellFilmInfoColumnHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.xrTableCellFilmInfoColumnHeader.Name = "xrTableCellFilmInfoColumnHeader";
            this.xrTableCellFilmInfoColumnHeader.StylePriority.UseFont = false;
            this.xrTableCellFilmInfoColumnHeader.StylePriority.UseTextAlignment = false;
            this.xrTableCellFilmInfoColumnHeader.Text = "Film Info";
            this.xrTableCellFilmInfoColumnHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCellFilmInfoColumnHeader.Weight = 1.5688500924089377D;
            // 
            // detailBand1
            // 
            this.detailBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabelCustomerFullName,
            this.xrTableFilmsColumnHeaders,
            this.xrPictureBoxCustomerPhoto});
            this.detailBand1.Dpi = 100F;
            this.detailBand1.HeightF = 192.4999F;
            this.detailBand1.Name = "detailBand1";
            // 
            // xrTableFilmsColumnHeaders
            // 
            this.xrTableFilmsColumnHeaders.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Bottom;
            this.xrTableFilmsColumnHeaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(219)))), ((int)(((byte)(225)))));
            this.xrTableFilmsColumnHeaders.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(172)))), ((int)(((byte)(182)))));
            this.xrTableFilmsColumnHeaders.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableFilmsColumnHeaders.Dpi = 100F;
            this.xrTableFilmsColumnHeaders.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrTableFilmsColumnHeaders.KeepTogether = true;
            this.xrTableFilmsColumnHeaders.LocationFloat = new DevExpress.Utils.PointFloat(0F, 162.4999F);
            this.xrTableFilmsColumnHeaders.Name = "xrTableFilmsColumnHeaders";
            this.xrTableFilmsColumnHeaders.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRowColumnHeaders});
            this.xrTableFilmsColumnHeaders.SizeF = new System.Drawing.SizeF(700F, 30F);
            this.xrTableFilmsColumnHeaders.StylePriority.UseBackColor = false;
            this.xrTableFilmsColumnHeaders.StylePriority.UseBorderColor = false;
            this.xrTableFilmsColumnHeaders.StylePriority.UseBorders = false;
            this.xrTableFilmsColumnHeaders.StylePriority.UseFont = false;
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.Dpi = 100F;
            this.topMarginBand1.HeightF = 100F;
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // collectionDataSource1
            // 
            this.collectionDataSource1.Name = "collectionDataSource1";
            this.collectionDataSource1.ObjectTypeName = "XVideoRental.Module.Win.BusinessObjects.Customer";
            this.collectionDataSource1.TopReturnedRecords = 0;
            // 
            // MovieRentalsByCustomer
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.detailBand1,
            this.reportHeaderBand1,
            this.topMarginBand1,
            this.bottomMarginBand1,
            this.DetailReport});
            this.Bookmark = "Customers";
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.collectionDataSource1});
            this.DataSource = this.collectionDataSource1;
            this.DisplayName = "Movie Rentals By Customer";
            this.Extensions.Add("DataSerializationExtension", "XafReport");
            this.Extensions.Add("DataEditorExtension", "XafReport");
            this.Extensions.Add("ParameterEditorExtension", "XafReport");
            this.Margins = new System.Drawing.Printing.Margins(100, 49, 100, 100);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.paramShowMode});
            this.Scripts.OnParametersRequestSubmit = "MovieRentalsByCustomerReport_ParametersRequestSubmit";
            this.ScriptsSource = resources.GetString("$this.ScriptsSource");
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "16.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableMovieInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmsColumnHeaders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.XRTableRow xrTableRowMovieTitle;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellMovieTitle;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellPhotoColumnHeader;
        private DevExpress.XtraReports.UI.XRControlStyle DataField;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellReturnedOn;
        private DevExpress.XtraReports.UI.XRLabel xrLabelHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabelCustomerFullName;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfoPageCount;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellRentStartDateColumnHeader;
        private DevExpress.XtraReports.UI.XRTable xrTableFilms;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowFilmInfo;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellPhoto;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxPhoto;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellTitle;
        private DevExpress.XtraReports.UI.XRTable xrTableMovieInfo;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowMovieFormat;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellMovieFormat;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowPlot;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellPlot;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellRentedOn;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellExpectedOn;
        private DevExpress.XtraReports.UI.ReportHeaderBand reportHeaderBand1;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxCustomerPhoto;
        private DevExpress.XtraReports.UI.XRControlStyle FieldCaption;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellExpectedOnColumnHeader;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport1;
        private DevExpress.XtraReports.UI.DetailBand DetailRents;
        protected DevExpress.Persistent.Base.ReportsV2.CollectionDataSource collectionDataSource1;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.Parameters.Parameter paramShowMode;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellRentEndDateColumnHeader;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowColumnHeaders;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellFilmInfoColumnHeader;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.XRTable xrTableFilmsColumnHeaders;
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
    }
}
