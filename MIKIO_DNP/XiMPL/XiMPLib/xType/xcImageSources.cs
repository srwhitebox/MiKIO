using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using XiMPLib.xDocument;
using XiMPLib.xUI;

namespace XiMPLib.xType {
    public class xcImageSources : List<ImageSource>{
        public ImageSource Normal {
            get {
                return this.Count > 0 ? this[0] : null;
            }
        }
        public ImageSource Released {
            get {
                return Normal;
            }
        }

        public ImageSource Pressed {
            get {
                return this.Count > 1 ? (this[1]!=null ? this[1] : Normal) : Normal;
            }
        }

        public ImageSource Disabled {
            get {
                return this.Count > 2 ? (this[2] != null ? this[2] : Normal) : Normal;
            }
        }
        public ImageSource Hover {
            get {
                return this.Count > 3 ? (this[3] != null ? this[3] : Normal) : Normal;
            }
        }

        private List<bool> IsGifList = new List<bool>();

        public xcImageSources(string brushesDef, Uri parentUri = null) {
            dispatch(brushesDef, parentUri);
        }

        private void dispatch(string sourcesDefine, Uri parentUri=null) {
            if (string.IsNullOrWhiteSpace(sourcesDefine))
                return;
            if (sourcesDefine.ToLower().Equals("none") || sourcesDefine.ToLower().Equals("null"))
                return;
            string[] sourceTokens = sourcesDefine.Split('|');
            for (int i = 0; i < sourceTokens.Length; i++) {
                Uri uri = new Uri(sourceTokens[i], UriKind.RelativeOrAbsolute);
                ImageSource source = null;
                if (!string.IsNullOrEmpty(sourceTokens[i].Trim())){
                    source = toImageSource(uri, parentUri);
                }
                this.Add(source);
                IsGifList.Add(source != null ? sourceTokens[i].ToLower().EndsWith("gid") : false);
            }
        }

        private ImageSource toImageSource(Uri uri, Uri ParentUri) {
            Uri absUri = ParentUri == null ? uri : new System.Uri(ParentUri, uri);
            return new BitmapImage(absUri);
        }

        public bool isGif(ImageSource source) {
            int index = this.IndexOf(source);
            return index > 0 ? IsGifList[index] : false;
        }

        public static ImageSource getFromIcon(System.Drawing.Icon icon)
        {
            return icon == null ? null : Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static ImageSource getApplicationIconSource()
        {
            return getFromIcon(xcImage.getApplicationIcon());
        }
    }
}
