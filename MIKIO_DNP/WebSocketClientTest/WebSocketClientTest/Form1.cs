using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apache.NMS;
using Apache.NMS.Util;

namespace WebSocketClientTest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            string urlString = "stomp:tcp://localhost:8080/ximplweb/chat";
            Uri connecturi = new Uri(urlString);
            IConnectionFactory factory = new NMSConnectionFactory(urlString, "12345");
 
            using(IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession()) {
                IDestination destination = SessionUtil.GetDestination(session, "/topic/command");
                 using(IMessageConsumer consumer = session.CreateConsumer(destination))
                 using (IMessageProducer producer = session.CreateProducer(destination)) {
                     connection.Start();
                 }
            }
        }
    }
}
