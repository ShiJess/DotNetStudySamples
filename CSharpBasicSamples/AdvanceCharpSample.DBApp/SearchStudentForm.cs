using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MySchool
{
    /// <summary>
    /// 查询学员用户窗体
    /// </summary>
    public partial class SearchStudentForm : Form
    {
        public SearchStudentForm()
        {
            InitializeComponent();
        }

        // 单击“取消”按钮时，关闭窗体
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 查找用户
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtLoginId.Text == "")  // 必须输入用户名才能查找
            {
                MessageBox.Show("请输入用户名", "输入提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLoginId.Focus();
            }            
            else  // 查找用户
            {
                FillListView();  // 填充列表视图
            }
        }     

        // 删除用户
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            // 确保用户选择了一个学员才执行删除操作
            if (lvStudent.SelectedItems.Count == 0)
            {
                MessageBox.Show("您没有选择任何用户", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            { 
                // 为防止误删除，要先询问
                DialogResult choice = MessageBox.Show("确定要删除该用户吗？","操作警告",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);

                // 如果确定删除，则执行删除操作
                if (choice == DialogResult.Yes)
                {
                    // 删除用sql语句
                    string sql = string.Format("DELETE FROM Student WHERE StudentID={0}",(int)lvStudent.SelectedItems[0].Tag);

                    // 创建Command对象
                    SqlCommand command = new SqlCommand(sql, DBHelper.connection);

                    int result = 0;  // 操作结果

                    try
                    {
                        DBHelper.connection.Open();  // 打开数据库连接
                        result = command.ExecuteNonQuery();  // 执行命令                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        DBHelper.connection.Close();  // 关闭数据库连接
                    }

                    if (result < 1)  // 操作失败
                    {
                        MessageBox.Show("删除失败！", "操作结果", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else  // 操作成功
                    {
                        MessageBox.Show("删除成功！", "操作结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillListView();  // 重新查询信息填充列表视图
                    }
                }
            }
        }

        // 将用户状态修改为活动的
        private void tsmiActive_Click(object sender, EventArgs e)
        {
            // 确保用户选择了一个学员才执行修改操作
            if (lvStudent.SelectedItems.Count == 0)
            {
                MessageBox.Show("您没有选择任何用户", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // 删除用sql语句
                string sql = string.Format("Update Student SET UserStateId=1 WHERE StudentID={0}", (int)lvStudent.SelectedItems[0].Tag);
                int result = 0;  // 操作结果

                try
                {
                    // 创建Command对象
                    SqlCommand command = new SqlCommand(sql, DBHelper.connection);
                    DBHelper.connection.Open();          // 打开数据库连接
                    result = command.ExecuteNonQuery();  // 执行命令                        
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    DBHelper.connection.Close();  // 关闭数据库连接
                }
                if (result < 1)  // 操作失败
                {
                    MessageBox.Show("修改失败！", "操作结果", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else  // 操作成功
                {
                    MessageBox.Show("修改成功！", "操作结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillListView();  // 重新查询信息填充列表视图
                }
            }
        }

        // 将用户状态修改为非活动的
        private void tsmiInActive_Click(object sender, EventArgs e)
        {
            // 确保用户选择了一个学员才执行修改操作
            if (lvStudent.SelectedItems.Count == 0)
            {
                MessageBox.Show("您没有选择任何用户", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // 删除用sql语句
                string sql = string.Format("Update Student SET UserStateId=0 WHERE StudentID={0}", (int)lvStudent.SelectedItems[0].Tag);
                int result = 0;  // 操作结果

                try
                {
                    // 创建Command对象
                    SqlCommand command = new SqlCommand(sql, DBHelper.connection);
                    DBHelper.connection.Open();          // 打开数据库连接
                    result = command.ExecuteNonQuery();  // 执行命令                        
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    DBHelper.connection.Close();  // 关闭数据库连接
                }
                if (result < 1)  // 操作失败
                {
                    MessageBox.Show("修改失败！", "操作结果", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else  // 操作成功
                {
                    MessageBox.Show("修改成功！", "操作结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FillListView();  // 重新查询信息填充列表视图
                }
            }
        }

        /// <summary>
        /// 根据查询条件，从数据库中读取信息，填充列表视图
        /// </summary>
        private void FillListView()
        {
            string loginId;      // 用户名
            string studentName;  // 姓名
            string studentNO;    // 学号
            int userStateId;     // 用户状态Id 
            string userState;    // 用户状态

            // 查找学员用户的sql语句
            string sql = string.Format(
                "SELECT StudentID,LoginId,StudentNO,StudentName,UserStateId FROM Student WHERE LoginId like '%{0}%'", txtLoginId.Text
                );
            try
            {
                SqlCommand command = new SqlCommand(sql, DBHelper.connection); // 构造Command对象                
                DBHelper.connection.Open();  // 打开数据库连接

                SqlDataReader dataReader = command.ExecuteReader();  // 执行查询用户命令

                lvStudent.Items.Clear();    // 清除ListView中的所有项

                // 如果结果中没有数据行，就弹出提示，
                if (!dataReader.HasRows)
                {
                    MessageBox.Show("抱歉，没有您要找的用户！", "结果提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 将查到的结果循环写到ListView中
                    while (dataReader.Read())
                    {
                        // 将从数据库中读取到的用户名、姓名、学号、用户状态赋给相应的变量
                        loginId = (string)dataReader["LoginId"];
                        studentName = (string)dataReader["StudentName"];
                        studentNO = (string)dataReader["StudentNO"];
                        userStateId = (int)dataReader["UserStateId"];
                        userState = (userStateId == 1) ? "活动" : "非活动";

                        ListViewItem lviStudent = new ListViewItem(loginId);//创建一个ListView项
                        lviStudent.Tag = (int)dataReader["StudentID"];      // 将ID放在Tag中
                        lvStudent.Items.Add(lviStudent); // 向ListView中添加一个新项
                        lviStudent.SubItems.AddRange(new string[] { studentName, studentNO, userState });//向当前项中添加子项
                    }
                }
                dataReader.Close();  //关闭dataReader
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询数据库出错！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DBHelper.connection.Close();  // 关闭数据库连接
            }
        }                 
    }
}