using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock
{
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            // Insert Logic
            con.Open();

            bool status = false;
            // 0==Active 1==Deactive
            if(comboBox1.SelectedIndex==0)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            var SqlQuery = "";

            if (IfProductExist(con, textBox1.Text))
            {
                SqlQuery = @"UPDATE [Products] SET [ProductName] = '" + textBox2.Text + "' , [ProductStatus] = '" + status + "' WHERE [ProductCode] = '" + textBox1.Text + "'";
            }
            else
            {
                SqlQuery = @"INSERT INTO [Stock].[dbo].[Products] ([ProductCode],[ProductName],[ProductStatus])
                            VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + status + "')";
            }


            SqlCommand cmd = new SqlCommand(SqlQuery, con);
            cmd.ExecuteNonQuery();
            con.Close();

            // Reading Data
            LoadData();

        }

        private bool IfProductExist(SqlConnection con, string productcode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("Select 1 From [Products] WHERE ProductCode='" + productcode + "' ", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if(dt.Rows.Count >0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoadData()
        {
            // Reading Data
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            SqlDataAdapter sda = new SqlDataAdapter("Select * From [Stock].[dbo].[Products]", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = item["ProductCode"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item["ProductName"].ToString();

                if ((bool)item["ProductStatus"])
                {
                    dataGridView1.Rows[n].Cells[2].Value = "Active";
                }
                else
                {
                    dataGridView1.Rows[n].Cells[2].Value = "Deactive";
                }

            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            if (dataGridView1.SelectedRows[0].Cells[2].Value.ToString() == "Active")
            {
                comboBox1.SelectedIndex = 0;    // 0==Active
            }
            else
            {
                comboBox1.SelectedIndex = 1;    // 1==Deactive
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            
            
            var SqlQuery = "";

            if (IfProductExist(con, textBox1.Text))
            {
                con.Open();
                SqlQuery = @"DELETE FROM [Products] WHERE [ProductCode] = '" + textBox1.Text + "'";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            else
            {
                MessageBox.Show("Product not exist.");
            }


            // Reading Data
            LoadData();
        }



    }
}
