using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock
{
    public partial class StockMain : Form
    {
        public StockMain()
        {
            InitializeComponent();
        }

        private void StockMain_Load(object sender, EventArgs e)
        {
            //button1.PerformClick();
        }

        private void closeMdi()
        {
            foreach (Form c in this.MdiChildren)
            {
                c.Close();
            }
        }

        private void StockMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Exit Button
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Home Button
        private void button1_Click(object sender, EventArgs e)
        {
            Home depan = new Home();
            depan.TopLevel = false;
            mainpanel.Controls.Add(depan);
            depan.Show();
        }

        // Order Button
        private void button2_Click(object sender, EventArgs e)
        {
        }

        // Stock Button
        private void button3_Click(object sender, EventArgs e)
        {
            Stock stk = new Stock();
            stk.TopLevel = false;
            mainpanel.Controls.Add(stk);
            stk.Show();
        }
    }
}
