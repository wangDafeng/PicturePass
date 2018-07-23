using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace PicturePass
{
    public partial class Form1 : Form
    {
        private Form2 f;

        private delegate void TextDelegate(string data);
        private  DataSet WorkPlanDataSet = new DataSet();



        public Form1()
        {
            InitializeComponent();


        }

        private void ComboBox1Selected(object sender, EventArgs e)
        {
            SelectComboBox(comboBox1.Text);
        }
        private void SelectComboBox(string data)
        {
            if (WorkPlanDataSet.Tables.Count < 1 || WorkPlanDataSet.Tables[0].Rows.Count < 1) return;
            DataRowCollection rows = WorkPlanDataSet.Tables[0].Rows;

            comboBox2.Items.Clear();
            for (int i = 0; i < rows.Count; i++)
            {
                string p = rows[i]["projectname"].ToString().Trim();

                if (p.Equals(data))
                {
                    string[] u = rows[i]["unitname"].ToString().Trim().Split('&');
                    for (int j = 0; j < u.Length; j++)
                    {
                        if (comboBox2.Items.Contains(u[j])) continue;
                        comboBox2.Items.Add(u[j]);
                        comboBox2.Text = u[j];
                    }

                }
            }

        }

        private void SetcomboBox(string data)
        {
            if (WorkPlanDataSet.Tables.Count < 1||WorkPlanDataSet.Tables[0].Rows.Count<1) return;
            DataRowCollection rows = WorkPlanDataSet.Tables[0].Rows;


            comboBox1.Items.Clear();
            comboBox1.Text = data;
            comboBox2.Items.Clear();
            for (int i = 0; i < rows.Count; i++)
            {
                string p = rows[i]["projectname"].ToString().Trim();

                if (p.Equals(data))
                {
                    string[] u = rows[i]["unitname"].ToString().Trim().Split('&');
                    for(int j = 0; j < u.Length; j++)
                    {
                        if (comboBox2.Items.Contains(u[j])) continue;
                        comboBox2.Items.Add(u[j]);
                        comboBox2.Text = u[j];
                    }
                    
                }

                if (comboBox1.Items.Contains(p)) continue;
                comboBox1.Items.Add(p);
            }

           
        }


        private void ReadProjectName(object state)
        {
            try
            {

                SqlConnection con = new SqlConnection();
                con.ConnectionString = "server="+Program.Server+";database=PicturePass;uid=sa;pwd=sa";

                SqlCommand ProjectNameCommand = new SqlCommand("SELECT * FROM [PicturePass].[dbo].[projectname]", con); //项目名命令
                SqlDataAdapter SelectAdapter = new SqlDataAdapter();//定义一个数据适配器
                SelectAdapter.SelectCommand = ProjectNameCommand;
                DataSet ProjectNameDataSet = new DataSet();//定义项目名数据集
                con.Open();//打开数据库连接
                SelectAdapter.SelectCommand.ExecuteNonQuery();//执行数据库查询指令
                SelectAdapter.Fill(ProjectNameDataSet);//填充数据集
                string ProjectName = ProjectNameDataSet.Tables[0].Rows[0]["name"].ToString().Trim();

                SqlCommand WorkPlanCommand= new SqlCommand();
                WorkPlanCommand.Connection = con;
                if (Program.UserName.IndexOf("检查") < 0)
                {
                    WorkPlanCommand.CommandText = "SELECT * FROM [PicturePass].[dbo].[WorkPlan] where username='" + Program.UserName + "'"; //用户登录
                }
                else
                {
                    WorkPlanCommand.CommandText = "SELECT * FROM [PicturePass].[dbo].[WorkPlan]"; //用户登录
                }
               
                SelectAdapter.SelectCommand = WorkPlanCommand;
                SelectAdapter.SelectCommand.ExecuteNonQuery();
                WorkPlanDataSet.Clear();
                SelectAdapter.Fill(WorkPlanDataSet);
                con.Close();//关闭数据库


                comboBox1.Invoke(new TextDelegate(SetcomboBox), ProjectName);


             

            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.workType = Program.Mode.view;
            showForm2();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.workType = Program.Mode.review;
            showForm2();


        }

        private void showForm2()
        {

            if (comboBox1.Text.Length < 3)
            {
                MessageBox.Show("项目名过短");
                return;
            }
            if (comboBox2.Text.Length < 3)
            {
                MessageBox.Show("工程名过短");
                return;
            }
            if (textBox4.Text.Length < 3)
            {
                MessageBox.Show("填写服务器地址");
                return;
            }
            if (textBox5.Text.Length <1)
            {
                MessageBox.Show("填写用户名");
                return;
            }



            Program.ProjectName = comboBox1.Text;
            Program.UnitName = comboBox2.Text;

  


            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "图片文件(*.jpg;*.bmp;*.png)|*.jpg;*.bmp;*.png|(All file(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {

                    if (f == null || f.IsDisposed)
                    {
                        f = new Form2(fileDialog.FileName);
                    }
                    f.ShowDialog();

                }
            }

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openFileDialog1.FileName;
            Program.filePath = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Program.workType = Program.Mode.quickCheck;
            showForm2();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Length < 3)
            {
                MessageBox.Show("填写服务器地址");
                return;
            }
            if (textBox5.Text.Length < 1)
            {
                MessageBox.Show("填写用户名");
                return;
            }

            Program.Server = textBox4.Text;
            Program.UserName = textBox5.Text;

            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadProjectName));
        }
    }

}
