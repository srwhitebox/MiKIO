using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XiMPLib.MiHIS {
    public class BillInfo {
        public string CaseNo {
            get {
                return Items["案件編號"];
            }
        }

        public string UnpaidRegistraion {
            get {
                return Items["掛號部分負擔未繳"];
            }
        }

        public string IdType {
            get {
                return Items["身分別"];
            }
        }

        public string PaymentType {
            get {
                return Items["部分負擔代碼"];
            }
        }

        public string DiscountType {
            get {
                return Items["折扣別"];
            }
        }

        public string DeptName {
            get {
                return Items["科別"];
            }
        }

        public string DoctorName {
            get {
                return Items["醫師"];
            }
        }


        public int RegistrationFee {
            get {
                return getExpense("掛號費用");
            }
        }

        public int PartialPayment {
            get {
                return getExpense("部分負擔");
            }
        }

        public string DrugWeight {
            get {
                return Items["藥費加重"];
            }
        }

        public int SelfExpense {
            get {
                return getExpense("自費金額");
            }
        }

        public int InsuranceExpense {
            get {
                return getExpense("健保差價");
            }
        }

        public int OpdExpense {
            get {
                return getExpense("醫療費用");
            }
        }

        public int DrugExpense {
            get {
                return getExpense("藥品費用");
            }
        }

        public int EtcExpense {
            get {
                return getExpense("其他費用");
            }
        }

        Dictionary<string, string> Items = new Dictionary<string, string>();



        public void dispatchXml(string xmlValue) {
            XDocument xdoc = XDocument.Parse(xmlValue, LoadOptions.None);

            // 案件編號=AC150602101001/掛號部分負擔未繳=N/身分別=健保/折扣別=/部分負擔代碼=D10/科別='一般內科/醫師=趙醫生/應收金額=0/掛號費用=0/部分負擔=0/藥費加重=0/自費金額=0/健保差價=0/醫療費用=320/藥品費用=0/其他費用=0
            string body = xdoc.FirstNode.Document.Root.FirstNode.ToString().Substring(2);
            string[] token = body.Split('/');
            foreach (string pair in token) {
                string[] keyValue = pair.Split('=');
                string key = keyValue[0];
                if (string.IsNullOrEmpty(key))
                    return;
                key = keyValue[0].Trim();
                if (string.IsNullOrEmpty(key))
                    return;
                string value = keyValue[1];
                if (!string.IsNullOrEmpty(value)) {
                    value = value.Trim();
                }

                Items.Add(key, value);
            }

        }

        private int getExpense(string item) {
            string value = Items[item];
            return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        }
    }
}
