using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    public partial class LogView : Form
    {
        public LogView()
        {
            InitializeComponent();
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshTextBox();
        }

        private void LogView_Load(object sender, EventArgs e)
        {
            RefreshTextBox();
        }
        private void RefreshTextBox()
        {
            textBox1.Text = Program.sMessageForSendItems;
            textBox2.Text = Program.sMessageForEditData;
            textBox3.Text = Program.sMessageFailed;
            textBox4.Text = Program.sMessageDateBase;
            label1.Text = $"Отправлено обменов:{Program.OfferSend}";
            label2.Text = $"Не отправлено обменов:{Program.DontOfferSend}";

        }
    }
}
