using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace XiMPLib.xIO {
    public class xcFile {
        public Uri Uri {
            get;
            set;
        }

        public FileInfo FileInfo {
            get {
                return new System.IO.FileInfo(Path);
            }
        }

        public String Path {
            get;
            set;
        }

        public String MimeType {
            get {
                return MimeMapping.GetMimeMapping(Path); 
            }
        }

        public long Length {
            get {
                return isFile() ? this.FileInfo.Length : 0;
            }
        }

        public String Text {
            get {
                return System.IO.File.Exists(Path) ? System.IO.File.ReadAllText(Path) : null;
            }
        }

        public byte[] Data {
            get {
                return System.IO.File.ReadAllBytes(Path);
            }
        }

        public xcFile(String path) {
            this.Path = path;
            this.Uri = new Uri(path);
        }

        public xcFile(Uri uri) {
            Uri = uri;
            Path = uri.AbsolutePath;
        }

        public Boolean isFile() {
            return Path != null && System.IO.File.Exists(Path);
        }

        public Boolean isDirectory() {
            return System.IO.Directory.Exists(Path);
        }

        public Boolean exists() {
            return isFile() || isDirectory();
        }

        public Boolean createDir() {
            try {
                System.IO.Directory.CreateDirectory(Path);
                return true;
            } catch (IOException ex) {
                return false;
            } catch (ArgumentException ex) {
                return false;
            } catch (UnauthorizedAccessException ex) {
                return false;
            }
        }
    }
}
