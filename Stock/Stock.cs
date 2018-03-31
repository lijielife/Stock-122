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
    public partial class Stock : Form
    {
        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;

            comboBox2.SelectedIndex = 0;

            LoadData();
            LoadCategory();
        }

        // Double Click on Selected Item List
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBox4.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox5.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            textBox6.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            if (dataGridView1.SelectedRows[0].Cells[5].Value.ToString() == "Active")
            {
                comboBox2.SelectedIndex = 0;    // 0==Active
            }
            else
            {
                comboBox2.SelectedIndex = 1;    // 1==Deactive
            }
            tabControl1.SelectedIndex = 1;
        }

        //-----Product Section-----//
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
                dataGridView1.Rows[n].Cells[2].Value = item["ProductCategory"].ToString();
                dataGridView1.Rows[n].Cells[3].Value = item["ProductDescription"].ToString();
                dataGridView1.Rows[n].Cells[4].Value = item["ProductPrice"].ToString();

                if ((bool)item["ProductStatus"])
                {
                    dataGridView1.Rows[n].Cells[5].Value = "Active";
                }
                else
                {
                    dataGridView1.Rows[n].Cells[5].Value = "Deactive";
                }
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
        }

        // Boolean flag used to determine when a character other than a number is entered.
        private bool nonNumberEntered = false;

        // Handle the KeyDown event to determine the type of character entered into the control.
        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }
        }

        // Product Add Button
        private void button6_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            // Insert Logic

            bool status = false;
            // 0==Active 1==Deactive
            if (comboBox2.SelectedIndex == 0)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            var SqlQuery = "";

            if (IfProductExist(con, textBox3.Text))
            {
                MessageBox.Show("Product already exist.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                con.Open();
                SqlQuery = @"INSERT INTO [Stock].[dbo].[Products] ([ProductCode],[ProductName],[ProductCategory],[ProductDescription],[ProductPrice],[ProductStatus])
                            VALUES ('" + textBox3.Text + "', '" + textBox4.Text + "', '" + comboBox1.Text + "', '" + textBox5.Text + "', '" + textBox6.Text + "', '" + status + "')";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Product Successfully Added.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
            }

            // Reading Data
            LoadData();
        }

        // Product Delete Button
        private void button5_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");

            var SqlQuery = "";

            if (IfProductExist(con, textBox3.Text))
            {
                var result=MessageBox.Show("Are you sure you want to delete this product?","Confirmation",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
                if(result==DialogResult.OK)
                {
                    con.Open();
                    SqlQuery = @"DELETE FROM [Products] WHERE [ProductCode] = '" + textBox3.Text + "'";
                    SqlCommand cmd = new SqlCommand(SqlQuery, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                }
            }
            else
            {
                MessageBox.Show("Product does not exist.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            // Reading Data
            LoadData();
        }

        // Product Update Button
        private void button4_Click(object sender, EventArgs e)
        {

            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            // Insert Logic

            bool status = false;
            // 0==Active 1==Deactive
            if (comboBox1.SelectedIndex == 0)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            var SqlQuery = "";

            if (IfProductExist(con, textBox3.Text))
            {
                var result=MessageBox.Show("Are you sure you want to update this product?","Confirmation",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    con.Open();
                    SqlQuery = @"UPDATE [Products] SET [ProductName]='" + textBox4.Text + "', [ProductStatus]='" + status + "', [ProductCategory]='" + comboBox1.Text + "', [ProductDescription]='" + textBox5.Text + "', [ProductPrice]='" + textBox6.Text + "' WHERE [ProductCode] = '" + textBox3.Text + "'";
                    SqlCommand cmd = new SqlCommand(SqlQuery, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                }
            }
            else
            {
                MessageBox.Show("Product does not exist.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Reading Data
            LoadData();
        }

        // Product Browse Image Button
        private void button7_Click(object sender, EventArgs e)
        {

        }

        // Check If Product Exists
        private bool IfProductExist(SqlConnection con, string productcode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("Select 1 From [Products] WHERE ProductCode='" + productcode + "' ", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //-----End Product Section-----//

        //-----Category Section-----//
        // Load Category Data onto DataGridView
        public void LoadCategory()
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            SqlDataAdapter sda = new SqlDataAdapter("Select * From [Stock].[dbo].[Category]", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView2.Rows.Clear();

            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView2.Rows.Add();
                dataGridView2.Rows[n].Cells[0].Value = item["CategoryCode"].ToString();
                dataGridView2.Rows[n].Cells[1].Value = item["CategoryName"].ToString();
            }

            int x = dataGridView2.Rows.Count;
            int[] catcode = new int[x];

            for (int i = 0; i < x; i++)
            {
                catcode[i] = Convert.ToInt16(dataGridView2.Rows[i].Cells[0].Value.ToString());
                if (catcode[i] != i)
                {
                    textBox1.Text = "" + i;
                    break;
                }
                else if (catcode[i] == x - 1)
                {
                    textBox1.Text = "" + x;
                }
            }
            getCatCombo();
        }

        // Fill Category ComboBox on Product tab
        private void getCatCombo()
        {
            comboBox1.Items.Clear();
            foreach(DataGridViewRow row in dataGridView2.Rows)
            {
                comboBox1.Items.Add(row.Cells[1].Value.ToString());
            }
            comboBox1.SelectedIndex = 0;
        }

        // Category Add Button
        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            // Insert Logic
            con.Open();

            var SqlQuery = "";

            if (IfCategoryExist(con, textBox1.Text))
            {
                MessageBox.Show("Product already exist.");
            }
            else
            {
                SqlQuery = @"INSERT INTO [Stock].[dbo].[Category] ([CategoryCode],[CategoryName])
                            VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "')";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();

                textBox1.Clear();
                textBox2.Clear();
            }

            LoadCategory();
        }

        // Category Delete Button
        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");

            var SqlQuery = "";
            int x = Convert.ToInt16(textBox1.Text);

            if (IfCategoryExist(con, textBox1.Text) && x!=0)
            {
                con.Open();
                SqlQuery = @"DELETE FROM [Category] WHERE [CategoryCode] = '" + textBox1.Text + "'";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();

                textBox1.Clear();
                textBox2.Clear();
            }
            else
            {
                if (x==0)
                {
                    MessageBox.Show("Cannot delete this category.");
                }
                else
                {
                    MessageBox.Show("Category does not exist.");
                }
            }

            // Reading Data
            LoadCategory();
        }

        // Category Update Button
        private void button3_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.\\MSSQLEXPRESS;Initial Catalog=Stock;Integrated Security=True");
            // Insert Logic
            con.Open();

            var SqlQuery = "";
            int x = Convert.ToInt16(textBox1.Text);

            if (IfCategoryExist(con, textBox1.Text) && x != 0)
            {
                SqlQuery = @"UPDATE [Category] SET [CategoryName] = '" + textBox2.Text + "' WHERE [CategoryCode] = '" + textBox1.Text + "'";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.ExecuteNonQuery();
                con.Close();

                textBox1.Clear();
                textBox2.Clear();
            }
            else
            {
                if (x == 0)
                {
                    MessageBox.Show("Cannot update this category.");
                }
                else
                {
                    MessageBox.Show("Category does not exist.");
                }
            }

            // Reading Data
            LoadCategory();
        }

        // Double Click on Category to Select
        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            textBox1.Text = dataGridView2.SelectedRows[0].Cells[0].Value.ToString();
            textBox2.Text = dataGridView2.SelectedRows[0].Cells[1].Value.ToString();
        }

        // Check If Category Exists
        private bool IfCategoryExist(SqlConnection con, string categorycode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("Select 1 From [Category] WHERE CategoryCode='" + categorycode + "' ", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //-----End Category Section-----//
    }
}