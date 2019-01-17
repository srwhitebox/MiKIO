using MitacHis.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using XiMPLib.xType;
using XiMPLib.xUI;

namespace XiMPLib.MiHIS {
    public class CalledInfoList : List<CalledInfo> {

        private WebClient client = new WebClient();
        private System.Timers.Timer OpdPrgoressRotateTimer;
        private const int RotateInterval = 5000;
        private const int RoomNumber = 3;
        public CalledInfoList() {
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            client.Encoding = Encoding.GetEncoding("UTF-8");
            init();
            //xcMessageBox.Show("test", "test", xcMessageBox.Buttons.YesNo, xcMessageBox.Icon.Exclamation);
            //client.DownloadStringAsync(new Uri("http://www.antifat.com.tw/"));
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
            if (e.Error != null || e.Cancelled) {
                setErrorMessage("請確認網路");
                //MessageBox.Show("Internet may have problem. Check the internet connection.\nApplication shall be shutdown.", "MiKIO");
                //System.Environment.Exit(0);
            } else {
                string htmlContents = e.Result;
                if (!patchHtml(htmlContents))
                {
                    setErrorMessage("請確認網路");
                }
            }
            OpdPrgoressRotateTimer.Start();
        }

        public void init() {
            for (int i = 1; i < RoomNumber+1; i++) {
                Add(new CalledInfo(i.ToString()));
            }
            try {
                string htmlContents = client.DownloadString("http://www.antifat.com.tw/");
                patchHtml(htmlContents);
            } catch {
                setErrorMessage("No connection");
                //MessageBox.Show("Internet may have problem. Check the internet connection.\nApplication shall be shutdown.", "MiKIO");
                //System.Environment.Exit(0);
            }
            OpdPrgoressRotateTimer = new System.Timers.Timer(RotateInterval);
            OpdPrgoressRotateTimer.Elapsed += OpdPrgoressRotateTimer_Elapsed;
            OpdPrgoressRotateTimer.Start();
        }

        private void setErrorMessage(string message) {
            foreach (CalledInfo info in this) {
                info.CalledNo = "";
                info.DoctorName = message;
                info.DoctorTitle = "";
                info.OpdTimeName = "";
                info.StateText = "";
                info.notifyChanged();
            }
        }

        private void OpdPrgoressRotateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            try {
                OpdPrgoressRotateTimer.Stop();
                client.DownloadStringAsync(new Uri("http://www.antifat.com.tw/"));
            } catch {
            }
        }

        private bool patchHtml(string htmlContents)
        {
            int startIndex=0, endIndex=0;
            string tag;

            int[] rooms = new int[3];

            const string key = "#FFFFDD;\">";
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                index = htmlContents.IndexOf(key, index) + key.Length;
                rooms[i] = index;
                index += key.Length;
            }

            for (int i = 0; i < 3; i++)
            {
                tag = "";
                // Date
                startIndex = rooms[i];
                endIndex = htmlContents.IndexOf("<br>", startIndex);
                tag = htmlContents.Substring(startIndex, endIndex-startIndex);
                tag = xcString.removeWhiteSpace(tag);
                try {
                    DateTime dateTime = DateTime.ParseExact(tag, "yyyy-MM-dd", null);
                    this[i].OpdTime = dateTime;
                }
                catch
                {
                    this[i].DoctorName = tag;
                    continue;
                }

                // opdTime
                startIndex = endIndex + 4;
                endIndex = htmlContents.IndexOf("<br>", startIndex);
                tag = htmlContents.Substring(startIndex, endIndex - startIndex);
                tag = xcString.removeWhiteSpace(tag);
                this[i].OpdTimeName = tag;

                // Waiting No
                startIndex = htmlContents.IndexOf("10px 0px 5px 0px;\">", endIndex);
                if (i < 2 && (startIndex < 0 || startIndex >= rooms[i + 1]))
                {
                    this[i].notifyChanged();
                    continue;
                }
                if (startIndex < 0)
                    continue;

                endIndex = htmlContents.IndexOf("</div>", startIndex);
                startIndex = htmlContents.LastIndexOf(">", endIndex);
                tag = htmlContents.Substring(startIndex+1, endIndex - startIndex-1);
                tag = xcString.removeWhiteSpace(tag);
                this[i].CalledNo = tag;

                // Doctor
                startIndex = endIndex + 6;
                endIndex = htmlContents.IndexOf("</div>", startIndex);
                tag = htmlContents.Substring(startIndex, endIndex - startIndex);
                tag = xcString.removeWhiteSpace(tag);
                this[i].DoctorName = tag.Substring(0, tag.Length-2);
                this[i].DoctorTitle = tag.Substring(tag.Length - 2);

                this[i].notifyChanged();
            }
            
            return true;
        }

        private bool patchHtmlOld(string htmlContents) {
            int startIndex = htmlContents.IndexOf("<div class=\"top_main_right_dis_bg\">");
            int endIndex = htmlContents.IndexOf("<div class=\"top_main_right_title1_clear\">");
            if (startIndex < 0 || endIndex < 0)
                return false;

            htmlContents = htmlContents.Substring(startIndex, endIndex - startIndex);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(htmlContents);
            CalledInfo calledInfo = null;
            foreach (XmlNode node in doc.FirstChild) {
                string className = node.Attributes["class"].Value;
                const string key = "top_main_right_dis";
                if (className.StartsWith("top_main_right_distail")) {
                    if (node.HasChildNodes) {

                        foreach (XmlNode valueNode in node.ChildNodes) {
                            if (valueNode.NodeType == XmlNodeType.Text) {
                                string state = valueNode.InnerText.Trim();
                                calledInfo.DoctorName = calledInfo.StateText = state;
                            } else {
                                if (valueNode.Attributes["class"] == null)
                                    continue;

                                className = valueNode.Attributes["class"].Value;
                                switch (className) {
                                    case "top_main_right_disnum":
                                        string calledNo = valueNode.InnerText;
                                        if (!string.IsNullOrEmpty(calledNo)) {
                                            calledNo = calledNo.Trim();
                                        }
                                        if (string.IsNullOrEmpty(calledInfo.CalledNo) || !calledInfo.CalledNo.Equals(calledNo)) {
                                            calledInfo.CalledNo = calledNo;
                                            calledInfo.notifyChanged();
                                        }
                                        break;
                                    case "top_main_right_distext":
                                        string doctorName = valueNode.ChildNodes[0].InnerText;
                                        if (!string.IsNullOrEmpty(doctorName)) {
                                            doctorName = doctorName.Trim();
                                            if (string.IsNullOrEmpty(calledInfo.DoctorName) || !calledInfo.DoctorName.Equals(doctorName)) {
                                                string[] token = doctorName.Split(' ');
                                                calledInfo.DoctorName = token[0];
                                                if (token.Length > 1)
                                                    calledInfo.DoctorTitle = token[1];
                                                calledInfo.notifyChanged();
                                            }
                                        }
                                        string opdTimeName = valueNode.ChildNodes[1].InnerText;
                                        if (!string.IsNullOrEmpty(opdTimeName))
                                            opdTimeName = opdTimeName.Trim();
                                        if (string.IsNullOrEmpty(calledInfo.OpdTimeName) || !calledInfo.OpdTimeName.Equals(opdTimeName)) {
                                            calledInfo.OpdTimeName = opdTimeName;
                                            calledInfo.notifyChanged();
                                        }
                                        break;
                                    case "top_main_right_distime":
                                        string time = valueNode.InnerText;
                                        if (!string.IsNullOrEmpty(time))
                                            time = time.Trim();
                                        if (!string.IsNullOrEmpty(time)) {
                                            DateTime dateTime = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", null);
                                            if (!calledInfo.OpdTime.Equals(dateTime)) {
                                                calledInfo.OpdTime = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", null);
                                                calledInfo.notifyChanged();
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    } else {

                    }
                } else {
                    string roomNum = className.Substring(key.Length);
                    calledInfo = null;
                    for (int i = 0; i < this.Count; i++) {
                        if (this[i].RoomNo.Equals(roomNum)) {
                            calledInfo = this[i];
                            break;
                        }
                    }
                    if (calledInfo == null) {
                        calledInfo = new CalledInfo(roomNum);
                        this.Add(calledInfo);
                    }
                    calledInfo.notifyChanged();
                }
            }
            return true;
        }
    }
}
