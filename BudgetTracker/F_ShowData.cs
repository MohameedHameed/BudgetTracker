using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BudgetTracker
{
    public partial class F_ShowData : Form
    {
        string filePath = "budget_data.csv";

        public F_ShowData()
        {
            InitializeComponent();
            monthly_budget_limittext.Text = Properties.Settings.Default.monthly_budget_limit.ToString();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void F_ShowData_Load(object sender, EventArgs e)
        {
         // تأكد من أن هذا هو مسار الملف الصحيح
            if (File.Exists(filePath))
            {
                dataGridView1.DataSource = LoadCSV(filePath);
            }
            else
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private DataTable LoadCSV(string filePath)
        {
            DataTable dt = new DataTable();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string[] headers = sr.ReadLine().Split(','); // قراءة العناوين
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header); // إضافة الأعمدة إلى الجدول
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(','); // قراءة الصفوف
                        dt.Rows.Add(rows);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CSV file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePath))
            {
                DateTime startDate = dateTimePickerStart.Value;
                DateTime endDate = dateTimePickerEnd.Value;

                DataTable dt = LoadCSV(filePath); // تحميل البيانات بالكامل
                DataTable filteredData = FilterByDateRange(dt, startDate, endDate); // تصفية البيانات

                dataGridView1.DataSource = filteredData; // عرض البيانات في DataGridView
            }
            else
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private DataTable FilterByDateRange(DataTable dt, DateTime startDate, DateTime endDate)
        {
            DataTable filteredTable = dt.Clone(); // إنشاء نسخة من الجدول الأصلي بنفس الأعمدة

            foreach (DataRow row in dt.Rows)
            {
                DateTime rowDate;
                if (DateTime.TryParseExact(row["Date"].ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out rowDate))
                {
                    if (rowDate >= startDate && rowDate <= endDate)
                    {
                        filteredTable.ImportRow(row);
                    }
                }
            }

            return filteredTable;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int selectedMonth = dateTimePicker1.Value.Month;
            int selectedYear = dateTimePicker1.Value.Year;

            CalculateMonthlySummary(selectedMonth, selectedYear);
        }
        private void CalculateMonthlySummary(int month, int year)
        {

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalIncome = 0;
            decimal totalExpenses = 0;

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string headerLine = sr.ReadLine(); // تخطي سطر العناوين
                    while (!sr.EndOfStream)
                    {
                        string[] row = sr.ReadLine().Split(',');

                        if (row.Length < 4) continue; // التأكد من أن كل البيانات موجودة

                        // استخراج القيم
                        DateTime transactionDate;
                        if (DateTime.TryParseExact(row[0], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out transactionDate))
                        {
                            if (transactionDate.Month == month && transactionDate.Year == year)
                            {
                                decimal amount;
                                if (decimal.TryParse(row[1], out amount))
                                {
                                    if (row[3] == "Income")
                                        totalIncome += amount;
                                    else if (row[3] == "Expense")
                                        totalExpenses += amount;
                                }
                            }
                        }
                    }
                }

                // عرض النتائج في رسالة
                MessageBox.Show($"Summary for {month}/{year}\n\nTotal Income: {totalIncome:C}\nTotal Expenses: {totalExpenses:C}",
                                "Monthly Summary",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) {
                monthly_budget_limittext.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                monthly_budget_limittext.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.monthly_budget_limit =Convert.ToInt32( monthly_budget_limittext.Text);
            Properties.Settings.Default.Save();
            monthly_budget_limittext.Text = Properties.Settings.Default.monthly_budget_limit.ToString();
            MessageBox.Show("changed Sucessfuly");
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
