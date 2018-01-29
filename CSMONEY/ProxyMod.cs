using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CSMONEY
{
    public partial class ProxyMod : Form
    {
        int idBot = 0;
        public ProxyMod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.ListBotMod.Add(textBox1.Text);
            refreshListBox();

        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ListBotMod.RemoveAt(listBox1.SelectedIndex);
            refreshListBox();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        void refreshListBox()
        {
            listBox1.Items.Clear();
            Properties.Settings.Default.ListBots = "";
            string ss = "";
            foreach (var item in Program.ListBotMod)
            {
                ss += item + "\n";
                listBox1.Items.Add(item);
            }
            
            Properties.Settings.Default.ListBots = ss;
            Properties.Settings.Default.Save();
        }

        private void ProxyMod_Load(object sender, EventArgs e)
        {
            string[] tmp = Properties.Settings.Default.ListBots.Split('\n');
            Program.ListBotMod.Clear();
            foreach (var item in tmp)
            {
                if (item != "")
                {
                    Program.ListBotMod.Add(item);
                }
            }
            refreshListBox();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = $"Количество Предметов в Базу Cs.Money: {Program.DataMoney.Count}";
            label3.Text = $"Количество прокси: {Program.ProxyListFix.Count}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string[] fileText = System.IO.File.ReadAllLines(filename);
            foreach (var item in fileText)
            {
                Program.ProxyList.Enqueue(item);
                Program.ProxyListFix.Add(item);
            }
            MessageBox.Show(fileText.Length.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Money money = new Money();
            money.INI();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BotStarter BS = new BotStarter();
            BS.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var ItemFind = Program.DataMoney.Find(N => (N.Name.Replace(" ", "") == textBox2.Text.Replace(" ", "")));
            textBox3.Text = "";
            if (ItemFind != null)
            {
                textBox3.Text += $"Дата:{ItemFind.Dt}"+Environment.NewLine;
                textBox3.Text += $"Bot:{ItemFind.Bot}" + Environment.NewLine;
                textBox3.Text += $"Id:{ItemFind.Id}" + Environment.NewLine;
                textBox3.Text += $"Name:{ItemFind.Name}" + Environment.NewLine;
                textBox3.Text += $"Price:{ItemFind.Price}" + Environment.NewLine;
            }
            else { textBox3.Text += $"Предмет не найден!"; }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var wssv = new WebSocketServer(4649);

            wssv.AddWebSocketService<Laputa>("/Laputa");
            wssv.Start();
        }

        private void button7_Click(object sender1, EventArgs e1)
        {
            new System.Threading.Thread(delegate () {
                try
                {
                    var ws = new WebSocket("ws://localhost:4649/Laputa");
                     ws.OnMessage += (sender, e) =>
                     MessageBox.Show("Laputa says: " + e.Data);

                     ws.Connect();
                     ws.Send("BALUS");
                }
                catch (Exception ex) {
                    string eee = ex.Message;
                }
            }).Start();
           
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new System.Threading.Thread(delegate () {
                try
                {
                    MoneyGet MG = new MoneyGet(idBot);
                    MG.INI();
                    idBot++;
                }
                catch (Exception ex)
                {
                    string eee = ex.Message;
                }
            }).Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ArgumentsChek.Value = "214124";
        }
    }
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            MessageBox.Show(e.Data);
            ArgumentsChek.ValueChanged += Data_ValueChanged; //подписываем метод к событию
        }
        private void Data_ValueChanged(object sender, EventArgs e) //этот метод будет вызываться при изменении переменной в классе SomeClass
        {
            Send(ArgumentsChek.Value);
        }
    }
}
