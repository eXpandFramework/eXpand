namespace XVideoRental.Module.Win.Reports {
    partial class MostProfitableGenres {
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRGroupSortingSummary xrGroupSortingSummary1 = new DevExpress.XtraReports.UI.XRGroupSortingSummary();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MostProfitableGenres));
            this.xrTableCellPlot = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabelTotalRevenueLabel = new DevExpress.XtraReports.UI.XRLabel();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.paramStartDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabelHeader = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableCellReleaseDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.paramEndDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabelTotalRevenue = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTableFilmInfo = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellMovieTitle = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCellRevenue = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRowFilmInfo = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellCover = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPictureBoxCover = new DevExpress.XtraReports.UI.XRPictureBox();
            this.xrTableCellMovie = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableFilmFullInfo = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRowFilmTitleAndReleaseDate = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellReleaseDateLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRowTagLine = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCellPlotLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.GroupHeaderGenreGroup = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrRichText1 = new DevExpress.XtraReports.UI.XRRichText();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.reportHeaderBand1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.pageFooterBand1 = new DevExpress.XtraReports.UI.PageFooterBand();
            this.collectionDataSource1 = new DevExpress.Persistent.Base.ReportsV2.CollectionDataSource();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmFullInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // xrTableCellPlot
            // 
            this.xrTableCellPlot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(235)))), ((int)(((byte)(220)))));
            this.xrTableCellPlot.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCellPlot.BorderWidth = 1F;
            this.xrTableCellPlot.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Plot")});
            this.xrTableCellPlot.Dpi = 100F;
            this.xrTableCellPlot.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.xrTableCellPlot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(74)))), ((int)(((byte)(68)))));
            this.xrTableCellPlot.Name = "xrTableCellPlot";
            this.xrTableCellPlot.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 5, 100F);
            this.xrTableCellPlot.StylePriority.UseBackColor = false;
            this.xrTableCellPlot.StylePriority.UseBorders = false;
            this.xrTableCellPlot.StylePriority.UseBorderWidth = false;
            this.xrTableCellPlot.StylePriority.UseFont = false;
            this.xrTableCellPlot.StylePriority.UseForeColor = false;
            this.xrTableCellPlot.StylePriority.UsePadding = false;
            this.xrTableCellPlot.StylePriority.UseTextAlignment = false;
            this.xrTableCellPlot.Text = "xrTableCellPlot";
            this.xrTableCellPlot.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCellPlot.Weight = 2.5917061911089045D;
            // 
            // xrLabelTotalRevenueLabel
            // 
            this.xrLabelTotalRevenueLabel.Dpi = 100F;
            this.xrLabelTotalRevenueLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabelTotalRevenueLabel.LocationFloat = new DevExpress.Utils.PointFloat(225F, 20.00001F);
            this.xrLabelTotalRevenueLabel.Name = "xrLabelTotalRevenueLabel";
            this.xrLabelTotalRevenueLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelTotalRevenueLabel.SizeF = new System.Drawing.SizeF(105.6952F, 20F);
            this.xrLabelTotalRevenueLabel.StylePriority.UseFont = false;
            this.xrLabelTotalRevenueLabel.StylePriority.UseTextAlignment = false;
            this.xrLabelTotalRevenueLabel.Text = "Total Revenue:";
            this.xrLabelTotalRevenueLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
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
            // pageInfo1
            // 
            this.pageInfo1.Dpi = 100F;
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(6F, 6F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(313F, 23F);
            this.pageInfo1.StyleName = "PageInfo";
            // 
            // paramStartDate
            // 
            this.paramStartDate.Description = "StartDate:";
            this.paramStartDate.Name = "paramStartDate";
            this.paramStartDate.Type = typeof(System.DateTime);
            this.paramStartDate.ValueInfo = "10/10/2012 01:35:50";
            // 
            // xrLabelHeader
            // 
            this.xrLabelHeader.Dpi = 100F;
            this.xrLabelHeader.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.xrLabelHeader.LocationFloat = new DevExpress.Utils.PointFloat(100F, 0F);
            this.xrLabelHeader.Name = "xrLabelHeader";
            this.xrLabelHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelHeader.SizeF = new System.Drawing.SizeF(452.0833F, 35F);
            this.xrLabelHeader.StylePriority.UseFont = false;
            this.xrLabelHeader.StylePriority.UseTextAlignment = false;
            this.xrLabelHeader.Text = "Most Profitable Genres";
            this.xrLabelHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableCellReleaseDate
            // 
            this.xrTableCellReleaseDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(235)))), ((int)(((byte)(220)))));
            this.xrTableCellReleaseDate.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCellReleaseDate.BorderWidth = 1F;
            this.xrTableCellReleaseDate.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "ReleaseDate", "{0:d}")});
            this.xrTableCellReleaseDate.Dpi = 100F;
            this.xrTableCellReleaseDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.xrTableCellReleaseDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(74)))), ((int)(((byte)(68)))));
            this.xrTableCellReleaseDate.Name = "xrTableCellReleaseDate";
            this.xrTableCellReleaseDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 100F);
            this.xrTableCellReleaseDate.StylePriority.UseBackColor = false;
            this.xrTableCellReleaseDate.StylePriority.UseBorders = false;
            this.xrTableCellReleaseDate.StylePriority.UseBorderWidth = false;
            this.xrTableCellReleaseDate.StylePriority.UseFont = false;
            this.xrTableCellReleaseDate.StylePriority.UseForeColor = false;
            this.xrTableCellReleaseDate.StylePriority.UsePadding = false;
            this.xrTableCellReleaseDate.StylePriority.UseTextAlignment = false;
            this.xrTableCellReleaseDate.Text = "xrTableCellReleaseDate";
            this.xrTableCellReleaseDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
            this.xrTableCellReleaseDate.Weight = 2.5917063826496238D;
            // 
            // paramEndDate
            // 
            this.paramEndDate.Description = "End Date:";
            this.paramEndDate.Name = "paramEndDate";
            this.paramEndDate.Type = typeof(System.DateTime);
            this.paramEndDate.ValueInfo = "10/10/2012 01:36:54";
            // 
            // xrLabelTotalRevenue
            // 
            this.xrLabelTotalRevenue.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "MovieId")});
            this.xrLabelTotalRevenue.Dpi = 100F;
            this.xrLabelTotalRevenue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.xrLabelTotalRevenue.LocationFloat = new DevExpress.Utils.PointFloat(337.5F, 20.00001F);
            this.xrLabelTotalRevenue.Name = "xrLabelTotalRevenue";
            this.xrLabelTotalRevenue.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabelTotalRevenue.Scripts.OnSummaryGetResult = "xrLabelTotalRevenue_SummaryGetResult";
            this.xrLabelTotalRevenue.Scripts.OnSummaryReset = "xrLabelTotalRevenue_SummaryReset";
            this.xrLabelTotalRevenue.Scripts.OnSummaryRowChanged = "xrLabelTotalRevenue_SummaryRowChanged";
            this.xrLabelTotalRevenue.SizeF = new System.Drawing.SizeF(82.29166F, 20.00002F);
            this.xrLabelTotalRevenue.StylePriority.UseFont = false;
            this.xrLabelTotalRevenue.StylePriority.UseTextAlignment = false;
            xrSummary1.FormatString = "{0:$0.00}";
            xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.Custom;
            xrSummary1.IgnoreNullValues = true;
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.xrLabelTotalRevenue.Summary = xrSummary1;
            this.xrLabelTotalRevenue.Text = "[MovieId]";
            this.xrLabelTotalRevenue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrLabelTotalRevenue.WordWrap = false;
            // 
            // xrTableFilmInfo
            // 
            this.xrTableFilmInfo.BorderColor = System.Drawing.Color.White;
            this.xrTableFilmInfo.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableFilmInfo.BorderWidth = 1F;
            this.xrTableFilmInfo.Dpi = 100F;
            this.xrTableFilmInfo.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.xrTableFilmInfo.KeepTogether = true;
            this.xrTableFilmInfo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTableFilmInfo.Name = "xrTableFilmInfo";
            this.xrTableFilmInfo.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1,
            this.xrTableRowFilmInfo});
            this.xrTableFilmInfo.SizeF = new System.Drawing.SizeF(650F, 175F);
            this.xrTableFilmInfo.StylePriority.UseBorderColor = false;
            this.xrTableFilmInfo.StylePriority.UseBorders = false;
            this.xrTableFilmInfo.StylePriority.UseBorderWidth = false;
            this.xrTableFilmInfo.StylePriority.UseFont = false;
            this.xrTableFilmInfo.StylePriority.UseTextAlignment = false;
            this.xrTableFilmInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellMovieTitle,
            this.xrTableCellRevenue});
            this.xrTableRow1.Dpi = 100F;
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 0.27777777353922528D;
            // 
            // xrTableCellMovieTitle
            // 
            this.xrTableCellMovieTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(199)))), ((int)(((byte)(193)))));
            this.xrTableCellMovieTitle.BorderColor = System.Drawing.Color.White;
            this.xrTableCellMovieTitle.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellMovieTitle.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Title")});
            this.xrTableCellMovieTitle.Dpi = 100F;
            this.xrTableCellMovieTitle.Font = new System.Drawing.Font("Verdana", 9F);
            this.xrTableCellMovieTitle.Name = "xrTableCellMovieTitle";
            this.xrTableCellMovieTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.xrTableCellMovieTitle.StylePriority.UseBackColor = false;
            this.xrTableCellMovieTitle.StylePriority.UseBorderColor = false;
            this.xrTableCellMovieTitle.StylePriority.UseBorders = false;
            this.xrTableCellMovieTitle.StylePriority.UseFont = false;
            this.xrTableCellMovieTitle.StylePriority.UseForeColor = false;
            this.xrTableCellMovieTitle.StylePriority.UsePadding = false;
            this.xrTableCellMovieTitle.StylePriority.UseTextAlignment = false;
            this.xrTableCellMovieTitle.Text = "xrTableCellMovieTitle";
            this.xrTableCellMovieTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableCellMovieTitle.Weight = 3.0486482764362184D;
            // 
            // xrTableCellRevenue
            // 
            this.xrTableCellRevenue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(199)))), ((int)(((byte)(193)))));
            this.xrTableCellRevenue.BorderColor = System.Drawing.Color.White;
            this.xrTableCellRevenue.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableCellRevenue.Dpi = 100F;
            this.xrTableCellRevenue.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.xrTableCellRevenue.Name = "xrTableCellRevenue";
            this.xrTableCellRevenue.StylePriority.UseBackColor = false;
            this.xrTableCellRevenue.StylePriority.UseBorderColor = false;
            this.xrTableCellRevenue.StylePriority.UseBorders = false;
            this.xrTableCellRevenue.StylePriority.UseFont = false;
            this.xrTableCellRevenue.StylePriority.UseForeColor = false;
            this.xrTableCellRevenue.Text = "Revenue: ";
            this.xrTableCellRevenue.Weight = 1.1675674250181261D;
            // 
            // xrTableRowFilmInfo
            // 
            this.xrTableRowFilmInfo.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellCover,
            this.xrTableCellMovie});
            this.xrTableRowFilmInfo.Dpi = 100F;
            this.xrTableRowFilmInfo.Name = "xrTableRowFilmInfo";
            this.xrTableRowFilmInfo.Weight = 1.7222222264607747D;
            // 
            // xrTableCellCover
            // 
            this.xrTableCellCover.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCellCover.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPictureBoxCover});
            this.xrTableCellCover.Dpi = 100F;
            this.xrTableCellCover.Name = "xrTableCellCover";
            this.xrTableCellCover.StylePriority.UseBorders = false;
            this.xrTableCellCover.Text = "xrTableCellPhoto";
            this.xrTableCellCover.Weight = 0.77837832493913406D;
            // 
            // xrPictureBoxCover
            // 
            this.xrPictureBoxCover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(224)))), ((int)(((byte)(213)))));
            this.xrPictureBoxCover.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrPictureBoxCover.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Image", null, "Photo")});
            this.xrPictureBoxCover.Dpi = 100F;
            this.xrPictureBoxCover.LocationFloat = new DevExpress.Utils.PointFloat(1.818989E-12F, 1.144409E-05F);
            this.xrPictureBoxCover.Name = "xrPictureBoxCover";
            this.xrPictureBoxCover.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 10, 10, 100F);
            this.xrPictureBoxCover.SizeF = new System.Drawing.SizeF(120F, 150.6944F);
            this.xrPictureBoxCover.Sizing = DevExpress.XtraPrinting.ImageSizeMode.Squeeze;
            this.xrPictureBoxCover.StylePriority.UseBackColor = false;
            this.xrPictureBoxCover.StylePriority.UseBorders = false;
            this.xrPictureBoxCover.StylePriority.UsePadding = false;
            // 
            // xrTableCellMovie
            // 
            this.xrTableCellMovie.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCellMovie.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTableFilmFullInfo});
            this.xrTableCellMovie.Dpi = 100F;
            this.xrTableCellMovie.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTableCellMovie.Multiline = true;
            this.xrTableCellMovie.Name = "xrTableCellMovie";
            this.xrTableCellMovie.StylePriority.UseBorders = false;
            this.xrTableCellMovie.StylePriority.UseFont = false;
            this.xrTableCellMovie.Text = "xrTableCellMovie";
            this.xrTableCellMovie.Weight = 3.43783737651521D;
            // 
            // xrTableFilmFullInfo
            // 
            this.xrTableFilmFullInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableFilmFullInfo.Dpi = 100F;
            this.xrTableFilmFullInfo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 1.144409E-05F);
            this.xrTableFilmFullInfo.Name = "xrTableFilmFullInfo";
            this.xrTableFilmFullInfo.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRowFilmTitleAndReleaseDate,
            this.xrTableRowTagLine});
            this.xrTableFilmFullInfo.SizeF = new System.Drawing.SizeF(530.0001F, 150.6945F);
            this.xrTableFilmFullInfo.StylePriority.UseBorders = false;
            this.xrTableFilmFullInfo.StylePriority.UseTextAlignment = false;
            this.xrTableFilmFullInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleJustify;
            // 
            // xrTableRowFilmTitleAndReleaseDate
            // 
            this.xrTableRowFilmTitleAndReleaseDate.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableRowFilmTitleAndReleaseDate.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellReleaseDateLabel,
            this.xrTableCellReleaseDate});
            this.xrTableRowFilmTitleAndReleaseDate.Dpi = 100F;
            this.xrTableRowFilmTitleAndReleaseDate.Name = "xrTableRowFilmTitleAndReleaseDate";
            this.xrTableRowFilmTitleAndReleaseDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 0, 0, 0, 100F);
            this.xrTableRowFilmTitleAndReleaseDate.StylePriority.UseBorders = false;
            this.xrTableRowFilmTitleAndReleaseDate.StylePriority.UsePadding = false;
            this.xrTableRowFilmTitleAndReleaseDate.StylePriority.UseTextAlignment = false;
            this.xrTableRowFilmTitleAndReleaseDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrTableRowFilmTitleAndReleaseDate.Weight = 0.11788778790443251D;
            // 
            // xrTableCellReleaseDateLabel
            // 
            this.xrTableCellReleaseDateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(235)))), ((int)(((byte)(220)))));
            this.xrTableCellReleaseDateLabel.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCellReleaseDateLabel.BorderWidth = 1F;
            this.xrTableCellReleaseDateLabel.Dpi = 100F;
            this.xrTableCellReleaseDateLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.xrTableCellReleaseDateLabel.Name = "xrTableCellReleaseDateLabel";
            this.xrTableCellReleaseDateLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.xrTableCellReleaseDateLabel.StylePriority.UseBackColor = false;
            this.xrTableCellReleaseDateLabel.StylePriority.UseBorders = false;
            this.xrTableCellReleaseDateLabel.StylePriority.UseBorderWidth = false;
            this.xrTableCellReleaseDateLabel.StylePriority.UseFont = false;
            this.xrTableCellReleaseDateLabel.StylePriority.UsePadding = false;
            this.xrTableCellReleaseDateLabel.StylePriority.UseTextAlignment = false;
            this.xrTableCellReleaseDateLabel.Text = "Date:";
            this.xrTableCellReleaseDateLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft;
            this.xrTableCellReleaseDateLabel.Weight = 0.40829361735037595D;
            // 
            // xrTableRowTagLine
            // 
            this.xrTableRowTagLine.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCellPlotLabel,
            this.xrTableCellPlot});
            this.xrTableRowTagLine.Dpi = 100F;
            this.xrTableRowTagLine.Name = "xrTableRowTagLine";
            this.xrTableRowTagLine.Weight = 0.47428028811004519D;
            // 
            // xrTableCellPlotLabel
            // 
            this.xrTableCellPlotLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(235)))), ((int)(((byte)(220)))));
            this.xrTableCellPlotLabel.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCellPlotLabel.Dpi = 100F;
            this.xrTableCellPlotLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.xrTableCellPlotLabel.Name = "xrTableCellPlotLabel";
            this.xrTableCellPlotLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 10, 100F);
            this.xrTableCellPlotLabel.StylePriority.UseBackColor = false;
            this.xrTableCellPlotLabel.StylePriority.UseBorders = false;
            this.xrTableCellPlotLabel.StylePriority.UseFont = false;
            this.xrTableCellPlotLabel.StylePriority.UsePadding = false;
            this.xrTableCellPlotLabel.StylePriority.UseTextAlignment = false;
            this.xrTableCellPlotLabel.Text = "Info:";
            this.xrTableCellPlotLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCellPlotLabel.Weight = 0.40829380889109568D;
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.Dpi = 100F;
            this.topMarginBand1.HeightF = 100F;
            this.topMarginBand1.Name = "topMarginBand1";
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
            // GroupHeaderGenreGroup
            // 
            this.GroupHeaderGenreGroup.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrRichText1,
            this.xrLabelTotalRevenueLabel,
            this.xrLabelTotalRevenue});
            this.GroupHeaderGenreGroup.Dpi = 100F;
            this.GroupHeaderGenreGroup.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Genre", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeaderGenreGroup.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeaderGenreGroup.HeightF = 40.00003F;
            this.GroupHeaderGenreGroup.KeepTogether = true;
            this.GroupHeaderGenreGroup.Name = "GroupHeaderGenreGroup";
            this.GroupHeaderGenreGroup.Scripts.OnAfterPrint = "GroupHeaderGenreGroup_AfterPrint";
            this.GroupHeaderGenreGroup.Scripts.OnBandLevelChanged = "GroupHeaderGenreGroup_BandLevelChanged";
            this.GroupHeaderGenreGroup.Scripts.OnBeforePrint = "GroupHeaderGenreGroup_BeforePrint";
            this.GroupHeaderGenreGroup.Scripts.OnSortingSummaryGetResult = "GroupHeaderGenreGroup_SortingSummaryGetResult";
            this.GroupHeaderGenreGroup.Scripts.OnSortingSummaryReset = "GroupHeaderGenreGroup_SortingSummaryReset";
            this.GroupHeaderGenreGroup.Scripts.OnSortingSummaryRowChanged = "GroupHeaderGenreGroup_SortingSummaryRowChanged";
            xrGroupSortingSummary1.Enabled = true;
            xrGroupSortingSummary1.FieldName = "MovieId";
            xrGroupSortingSummary1.Function = DevExpress.XtraReports.UI.SortingSummaryFunction.Custom;
            xrGroupSortingSummary1.SortOrder = DevExpress.XtraReports.UI.XRColumnSortOrder.Descending;
            this.GroupHeaderGenreGroup.SortingSummary = xrGroupSortingSummary1;
            // 
            // xrRichText1
            // 
            this.xrRichText1.Bookmark = "Genres";
            this.xrRichText1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Bookmark", null, "Genre")});
            this.xrRichText1.Dpi = 100F;
            this.xrRichText1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.xrRichText1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0F);
            this.xrRichText1.Name = "xrRichText1";
            this.xrRichText1.SerializableRtfString = resources.GetString("xrRichText1.SerializableRtfString");
            this.xrRichText1.SizeF = new System.Drawing.SizeF(630F, 20F);
            this.xrRichText1.StylePriority.UseFont = false;
            // 
            // pageInfo2
            // 
            this.pageInfo2.Dpi = 100F;
            this.pageInfo2.Format = "Page {0} of {1}";
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(331F, 6F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.pageInfo2.SizeF = new System.Drawing.SizeF(313F, 23F);
            this.pageInfo2.StyleName = "PageInfo";
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // detailBand1
            // 
            this.detailBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTableFilmInfo});
            this.detailBand1.Dpi = 100F;
            this.detailBand1.HeightF = 175F;
            this.detailBand1.Name = "detailBand1";
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
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Dpi = 100F;
            this.bottomMarginBand1.HeightF = 100F;
            this.bottomMarginBand1.Name = "bottomMarginBand1";
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
            // reportHeaderBand1
            // 
            this.reportHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabelHeader});
            this.reportHeaderBand1.Dpi = 100F;
            this.reportHeaderBand1.HeightF = 35F;
            this.reportHeaderBand1.Name = "reportHeaderBand1";
            // 
            // pageFooterBand1
            // 
            this.pageFooterBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1,
            this.pageInfo2});
            this.pageFooterBand1.Dpi = 100F;
            this.pageFooterBand1.HeightF = 29F;
            this.pageFooterBand1.Name = "pageFooterBand1";
            // 
            // collectionDataSource1
            // 
            this.collectionDataSource1.Name = "collectionDataSource1";
            this.collectionDataSource1.ObjectTypeName = "XVideoRental.Module.Win.BusinessObjects.Movie.Movie";
            this.collectionDataSource1.TopReturnedRecords = 0;
            // 
            // MostProfitableGenres
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.detailBand1,
            this.pageFooterBand1,
            this.reportHeaderBand1,
            this.topMarginBand1,
            this.bottomMarginBand1,
            this.GroupHeaderGenreGroup});
            this.Bookmark = "Genres";
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.collectionDataSource1});
            this.DataSource = this.collectionDataSource1;
            this.DisplayName = "Most Profitable Genres";
            this.Extensions.Add("DataSerializationExtension", "XafReport");
            this.Extensions.Add("DataEditorExtension", "XafReport");
            this.Extensions.Add("ParameterEditorExtension", "XafReport");
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.paramStartDate,
            this.paramEndDate});
            this.ScriptsSource = resources.GetString("$this.ScriptsSource");
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "16.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableFilmFullInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrRichText1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.XRTableCell xrTableCellPlot;
        private DevExpress.XtraReports.UI.XRLabel xrLabelTotalRevenueLabel;
        private DevExpress.XtraReports.UI.XRControlStyle DataField;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.Parameters.Parameter paramStartDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabelHeader;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellReleaseDate;
        private DevExpress.XtraReports.Parameters.Parameter paramEndDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabelTotalRevenue;
        private DevExpress.XtraReports.UI.XRTable xrTableFilmInfo;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellMovieTitle;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellRevenue;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowFilmInfo;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellCover;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBoxCover;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellMovie;
        private DevExpress.XtraReports.UI.XRTable xrTableFilmFullInfo;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowFilmTitleAndReleaseDate;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellReleaseDateLabel;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRowTagLine;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCellPlotLabel;
        private DevExpress.XtraReports.UI.TopMarginBand topMarginBand1;
        private DevExpress.XtraReports.UI.XRControlStyle FieldCaption;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeaderGenreGroup;
        private DevExpress.XtraReports.UI.XRRichText xrRichText1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.DetailBand detailBand1;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.BottomMarginBand bottomMarginBand1;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.UI.ReportHeaderBand reportHeaderBand1;
        private DevExpress.XtraReports.UI.PageFooterBand pageFooterBand1;
        protected DevExpress.Persistent.Base.ReportsV2.CollectionDataSource collectionDataSource1;
    }
}
