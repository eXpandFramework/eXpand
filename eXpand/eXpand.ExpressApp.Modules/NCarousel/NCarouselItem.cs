using System;

namespace eXpand.NCarousel {
    public class NCarouselItem {
        public NCarouselItem(Uri url, string text, string alt) {
            Url = url;
            Text = text;
            Alt = alt;
        }

        public NCarouselItem(Uri url, string text) {
            Url = url;
            Text = text;
        }

        public Uri Url { get; set; }
        public string Text { get; set; }
        public string Alt { get; set; }
    }
}