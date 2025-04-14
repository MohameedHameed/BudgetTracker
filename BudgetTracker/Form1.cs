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
    public partial class Form1 : Form
    {
        private string filePath = "budget_data.csv";
        public Form1()
        {
            InitializeComponent();
            LoadCategories();
            CheckFileExists();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LoadCategories()
        {
            categorycombobox.Items.AddRange(new string[] { "Food", "Transport", "Rent", "Bills", "Entertainment", "Other" });
            categorycombobox.SelectedIndex = 0;
            operationTypeComboBox.Items.AddRange(new string[] { "Income", "Expense" });
            operationTypeComboBox.SelectedIndex = 1;

        }
        private void CheckFileExists()
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("Date,Amount,Category,operationType");
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (operationTypeComboBox.SelectedIndex == 1)
            {
                if (Properties.Settings.Default.monthly_budget_limit != 0)
                {
                    int total = CalculateMonthlySummary(Convert.ToInt32(DateTime.Now.Month), Convert.ToInt32(DateTime.Now.Year));

                    if (total != 0)
                    {
                        total += Convert.ToInt32(amounttext.Text.Trim());
                        if (total > Properties.Settings.Default.monthly_budget_limit)
                        {
                            MessageBox.Show("You Have exceeded the monthly spending limit", "warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
                            return;
                        }
                    }

                }
            }
          
            string date = datetimtranscation.Value.ToString("dd/MM/yyyy");
            string amount = amounttext.Text.Trim();
            string category = categorycombobox.SelectedItem?.ToString();
            string operationType = operationTypeComboBox.SelectedItem.ToString();
            // التحقق من صحة الإدخالات
            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(category))
            {
                MessageBox.Show("يرجى ملء جميع الحقول", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(amount, out decimal parsedAmount) || parsedAmount < 0)
            {
                MessageBox.Show("المبلغ يجب أن يكون رقمًا موجبًا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // حفظ البيانات في الملف
            SaveToCSV(date, parsedAmount, category, operationType);
        }
        private void SaveToCSV(string date, decimal amount, string category, string operationType)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true)) // فتح الملف بوضع الإلحاق
                {
                    sw.WriteLine($"{date},{amount},{category},{operationType}");
                }
                MessageBox.Show("تمت إضافة السجل بنجاح!", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                amounttext.Clear(); // مسح حقل الإدخال بعد الحفظ
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء حفظ البيانات: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            F_ShowData f_Show = new F_ShowData();
            f_Show.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private int CalculateMonthlySummary(int month, int year)
        {

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }

            int totalExpenses = 0;

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
                                int amount;
                                if (int.TryParse(row[1], out amount))
                                {
                                    if (row[3] == "Expense")
                                        totalExpenses += amount;
                                   
                                       
                                }
                            }
                        }
                    }
                }

                return totalExpenses;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
    } 