namespace Xpand.ExpressApp.PivotGrid.Win.NetIncome {
    partial class NetIncomeGaugeControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            DevExpress.XtraGauges.Core.Model.ScaleLabel scaleLabel2 = new DevExpress.XtraGauges.Core.Model.ScaleLabel();
            DevExpress.XtraGauges.Core.Model.ArcScaleRange arcScaleRange3 = new DevExpress.XtraGauges.Core.Model.ArcScaleRange();
            DevExpress.XtraGauges.Core.Model.ArcScaleRange arcScaleRange4 = new DevExpress.XtraGauges.Core.Model.ArcScaleRange();
            this.gaugeControl2 = new DevExpress.XtraGauges.Win.GaugeControl();
            this.circularGauge1 = new DevExpress.XtraGauges.Win.Gauges.Circular.CircularGauge();
            this.arcScaleBackgroundLayerComponent1 = new DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleBackgroundLayerComponent();
            this.arcScaleComponent1 = new DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleComponent();
            this.labelComponent1 = new DevExpress.XtraGauges.Win.Base.LabelComponent();
            this.arcScaleNeedleComponent1 = new DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleNeedleComponent();
            ((System.ComponentModel.ISupportInitialize)(this.circularGauge1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arcScaleBackgroundLayerComponent1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arcScaleComponent1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelComponent1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arcScaleNeedleComponent1)).BeginInit();
            this.SuspendLayout();
            // 
            // gaugeControl2
            // 
            this.gaugeControl2.BackColor = System.Drawing.Color.Transparent;
            this.gaugeControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gaugeControl2.Gauges.AddRange(new DevExpress.XtraGauges.Base.IGauge[] {
            this.circularGauge1});
            this.gaugeControl2.Location = new System.Drawing.Point(4, 8);
            this.gaugeControl2.Name = "gaugeControl2";
            this.gaugeControl2.Size = new System.Drawing.Size(219, 219);
            this.gaugeControl2.TabIndex = 9;
            // 
            // circularGauge1
            // 
            this.circularGauge1.BackgroundLayers.AddRange(new DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleBackgroundLayerComponent[] {
            this.arcScaleBackgroundLayerComponent1});
            this.circularGauge1.Bounds = new System.Drawing.Rectangle(6, 6, 207, 207);
            this.circularGauge1.Labels.AddRange(new DevExpress.XtraGauges.Win.Base.LabelComponent[] {
            this.labelComponent1});
            this.circularGauge1.Name = "circularGauge1";
            this.circularGauge1.Needles.AddRange(new DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleNeedleComponent[] {
            this.arcScaleNeedleComponent1});
            this.circularGauge1.Scales.AddRange(new DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleComponent[] {
            this.arcScaleComponent1});
            // 
            // arcScaleBackgroundLayerComponent1
            // 
            this.arcScaleBackgroundLayerComponent1.ArcScale = this.arcScaleComponent1;
            this.arcScaleBackgroundLayerComponent1.Name = "bg";
            this.arcScaleBackgroundLayerComponent1.ShapeType = DevExpress.XtraGauges.Core.Model.BackgroundLayerShapeType.CircularFull_Style1;
            this.arcScaleBackgroundLayerComponent1.ZOrder = 1000;
            // 
            // arcScaleComponent1
            // 
            this.arcScaleComponent1.AppearanceTickmarkText.Font = new System.Drawing.Font("Tahoma", 11F);
            this.arcScaleComponent1.AppearanceTickmarkText.TextBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:Black");
            this.arcScaleComponent1.Center = new DevExpress.XtraGauges.Core.Base.PointF2D(125F, 125F);
            this.arcScaleComponent1.EndAngle = 60F;
            scaleLabel2.AppearanceText.Font = new System.Drawing.Font("Tahoma", 11F);
            scaleLabel2.AppearanceText.TextBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:Maroon");
            scaleLabel2.FormatString = "${1:F2}";
            scaleLabel2.Name = "Label0";
            scaleLabel2.Position = new DevExpress.XtraGauges.Core.Base.PointF2D(125F, 180F);
            scaleLabel2.Size = new System.Drawing.SizeF(150F, 30F);
            this.arcScaleComponent1.Labels.AddRange(new DevExpress.XtraGauges.Core.Model.ILabel[] {
            scaleLabel2});
            this.arcScaleComponent1.MajorTickmark.FormatString = "${0:F2}K";
            this.arcScaleComponent1.MajorTickmark.Multiplier = 0.001F;
            this.arcScaleComponent1.MajorTickmark.ShapeOffset = -3.5F;
            this.arcScaleComponent1.MajorTickmark.ShapeType = DevExpress.XtraGauges.Core.Model.TickmarkShapeType.Circular_Style1_4;
            this.arcScaleComponent1.MajorTickmark.ShowText = false;
            this.arcScaleComponent1.MajorTickmark.TextOffset = -15F;
            this.arcScaleComponent1.MajorTickmark.TextOrientation = DevExpress.XtraGauges.Core.Model.LabelOrientation.LeftToRight;
            this.arcScaleComponent1.MaxValue = 4000F;
            this.arcScaleComponent1.MinorTickCount = 4;
            this.arcScaleComponent1.MinorTickmark.ShapeType = DevExpress.XtraGauges.Core.Model.TickmarkShapeType.Circular_Style1_3;
            this.arcScaleComponent1.MinValue = 2500F;
            this.arcScaleComponent1.Name = "scale1";
            arcScaleRange3.AppearanceRange.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#90FF8080");
            arcScaleRange3.EndThickness = 20F;
            arcScaleRange3.EndValue = 3000F;
            arcScaleRange3.Name = "Range0";
            arcScaleRange3.ShapeOffset = -10F;
            arcScaleRange3.StartThickness = 25F;
            arcScaleRange4.AppearanceRange.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#9080FF80");
            arcScaleRange4.EndThickness = 25F;
            arcScaleRange4.EndValue = 3500F;
            arcScaleRange4.Name = "Range2";
            arcScaleRange4.ShapeOffset = -10F;
            arcScaleRange4.StartThickness = 20F;
            arcScaleRange4.StartValue = 3000F;
            this.arcScaleComponent1.Ranges.AddRange(new DevExpress.XtraGauges.Core.Model.IRange[] {
            arcScaleRange3,
            arcScaleRange4});
            this.arcScaleComponent1.StartAngle = -240F;
            this.arcScaleComponent1.Value = 3000F;
            // 
            // labelComponent1
            // 
            this.labelComponent1.AppearanceText.Font = new System.Drawing.Font("Tahoma", 11F);
            this.labelComponent1.AppearanceText.TextBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:Black");
            this.labelComponent1.Name = "circularGauge2_Label1";
            this.labelComponent1.Position = new DevExpress.XtraGauges.Core.Base.PointF2D(125F, 160F);
            this.labelComponent1.Text = "Month";
            this.labelComponent1.ZOrder = -1001;
            // 
            // arcScaleNeedleComponent1
            // 
            this.arcScaleNeedleComponent1.ArcScale = this.arcScaleComponent1;
            this.arcScaleNeedleComponent1.Name = "needle";
            this.arcScaleNeedleComponent1.ShapeType = DevExpress.XtraGauges.Core.Model.NeedleShapeType.CircularFull_Style6;
            this.arcScaleNeedleComponent1.StartOffset = -20F;
            this.arcScaleNeedleComponent1.ZOrder = -50;
            // 
            // NetIncomeGaugeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gaugeControl2);
            this.Name = "NetIncomeGaugeControl";
            this.Size = new System.Drawing.Size(226, 228);
            ((System.ComponentModel.ISupportInitialize)(this.circularGauge1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arcScaleBackgroundLayerComponent1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arcScaleComponent1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelComponent1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arcScaleNeedleComponent1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGauges.Win.GaugeControl gaugeControl2;
        private DevExpress.XtraGauges.Win.Gauges.Circular.CircularGauge circularGauge1;
        private DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleBackgroundLayerComponent arcScaleBackgroundLayerComponent1;
        private DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleComponent arcScaleComponent1;
        private DevExpress.XtraGauges.Win.Base.LabelComponent labelComponent1;
        private DevExpress.XtraGauges.Win.Gauges.Circular.ArcScaleNeedleComponent arcScaleNeedleComponent1;
    }
}
