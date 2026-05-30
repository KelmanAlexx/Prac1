using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1;

namespace ExamWinFormsApp1
{
    public partial class OrdersForm : Form
    {
        private DataGridView dgvOrders = new DataGridView();
        private ComboBox cmbStatus = new ComboBox();
        private Button btnUpdateStatus = new Button();
        private string currentRole;

        public OrdersForm(string role)
        {
            currentRole = role;
            Text = "Реестр заказов магазина";
            Size = new Size(800, 450);
            Font = new Font("Times New Roman", 11);
            StartPosition = FormStartPosition.CenterParent;

            InitializeFormComponents();
            LoadOrdersData();
        }

        private void InitializeFormComponents()
        {
            dgvOrders = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(750, 320),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Panel statusPanel = new Panel { Location = new Point(15, 350), Size = new Size(750, 50), BackColor = Color.WhiteSmoke };

            Label lblStatus = new Label { Text = "Новый статус:", Location = new Point(10, 15), AutoSize = true };
            cmbStatus = new ComboBox { Location = new Point(120, 12), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new string[] { "Новый", "В пути", "Готов к выдаче", "Завершен", "Отменен" });
            cmbStatus.SelectedIndex = 0;

            btnUpdateStatus = new Button { Text = "Изменить статус", Location = new Point(340, 8), Size = new Size(160, 32), BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat };
            btnUpdateStatus.Click += BtnUpdateStatus_Click;

            statusPanel.Controls.AddRange(new Control[] { lblStatus, cmbStatus, btnUpdateStatus });
            Controls.AddRange(new Control[] { dgvOrders, statusPanel });

            if (currentRole == "Менеджер")
            {
                statusPanel.Visible = false;
                dgvOrders.Height = 380;
            }
        }

        private async void LoadOrdersData()
        {
            try
            {
                DataTable data = await DbHelper.GetOrdersAsync();
                dgvOrders.DataSource = data;
                
                if (dgvOrders.Columns["order_id"] != null) dgvOrders.Columns["order_id"].HeaderText = "ID Заказа";
                if (dgvOrders.Columns["customer_name"] != null) dgvOrders.Columns["customer_name"].HeaderText = "ФИО Клиента";
                if (dgvOrders.Columns["order_date"] != null) dgvOrders.Columns["order_date"].HeaderText = "Дата заказа";
                if (dgvOrders.Columns["delivery_date"] != null) dgvOrders.Columns["delivery_date"].HeaderText = "Дата доставки";
                if (dgvOrders.Columns["order_status"] != null) dgvOrders.Columns["order_status"].HeaderText = "Статус";
                if (dgvOrders.Columns["pickup_code"] != null) dgvOrders.Columns["pickup_code"].HeaderText = "Код выдачи";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения данных заказов: " + ex.Message);
            }
        }

        private async void BtnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите заказ из таблицы!");
                return;
            }

            try
            {
                int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["order_id"].Value);
                string newStatus = cmbStatus.SelectedItem.ToString();

                bool res = await DbHelper.UpdateOrderStatusAsync(orderId, newStatus);
                if (res)
                {
                    MessageBox.Show("Статус заказа успешно изменен!");
                    LoadOrdersData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса: " + ex.Message);
            }
        }
    }
}