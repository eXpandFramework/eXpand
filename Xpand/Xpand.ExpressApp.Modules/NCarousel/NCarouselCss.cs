namespace Xpand.NCarousel {
    public class NCarouselCss {
        public NCarouselCss() {
            SetDefaultValues(Alignment.Horizontal,false);
        }


        public string Container { get; set; }

        public bool AllowOverride { get; set; }

        public string Iτem { get; set; }
        public string Next { get; set; }

        public string Clip { get; set; }

        public string ClassName { get; set; }

        public string Previous { get; set; }

        public void SetDefaultValues(Alignment alignment, bool hideImages) {
            ClassName = "jcarousel-skin-tango";
            if (hideImages)
                SetDefaultValuesForHiddenImages(alignment);
            else {
                SetDefaultValuesWithImages(alignment);
            }
        }

        void SetDefaultValuesWithImages(Alignment alignment) {
            if (alignment == Alignment.Horizontal){
                Container = "-moz-border-radius: 10px;background: #F0F6F9;border: 2px solid #346F97;width: 300px;height:130px;padding: 20px 40px;";
                Clip = "width:  280px;height: 135px;";
                Iτem = "width: 100px;height: 120px;";
                Next = "top: 60px;";
                Previous = "top: 60px;";
            }
            else{
                Container =
                    "-moz-border-radius: 10px;background: #F0F6F9;border: 1px solid #346F97;width: 95px;height: 355px;padding:30px;";
                Clip = "width:  80px;height: 350px;";
                Iτem = "width: 100px;height: 120px;";
                Next = "left: 56px;";
                Previous = "left: 56px;";
            }
        }

        void SetDefaultValuesForHiddenImages(Alignment alignment) {
            if (alignment == Alignment.Horizontal){
                Container = "-moz-border-radius: 10px;background: #F0F6F9;border: 2px solid #346F97;width: 300px;height:50px;padding: 20px 40px;";
                Clip = "width:  280px;height: 75px;";
                Iτem = "width: 100px;height: 40px;";
                Next = "top: 28px;";
                Previous = "top: 28px;";
            }
            else{
                Container =
                    "-moz-border-radius: 10px;background: #F0F6F9;border: 1px solid #346F97;width: 90px;height: 220px;padding:30px;";
                Clip = "width:90px;height:215px;";
                Iτem = "width: auto;height: 40px;margin-bottom: 0px;";
                Next = "left: 53px;";
                Previous = "left: 53px;";
            }
        }
    }
}