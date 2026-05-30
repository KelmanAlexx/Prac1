using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1;

namespace ExamWinFormsApp1
{
    public partial class MainForm : Form
    {
        private FlowLayoutPanel productsPanel = new FlowLayoutPanel();
        private Panel header = new Panel();
        private Panel filterPanel = new Panel();
        
        private TextBox txtSearch = new TextBox();
        private ComboBox cmbSort = new ComboBox();
        private Button btnOrderAction = new Button(); 
        private Button btnProductAction = new Button(); 

        private string userName;
        private string userRole;

        public MainForm(string name, string role)
        {
            userName = name;
            userRole = role;
            InitializeComponent();
            SetupForm();
            InitializeObjects();
            ApplyRolePermissions();
            LoadProducts();
        }

        private void SetupForm()
        {
            Text = "Магазин обуви - Панель управления";
            Size = new Size(900, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Times New Roman", 10);
        }

        private void InitializeObjects()
        {
            header = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = ColorTranslator.FromHtml("#7FFF00") };
            Label lblUser = new Label { Text = "Пользователь: " + userName + " (" + userRole + ")", AutoSize = true, Location = new Point(10, 15), Font = new Font("Times New Roman", 11, FontStyle.Bold) };
            Button btnExit = new Button { Text = "Выйти", Location = new Point(780, 10), Size = new Size(90, 30), BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExit.Click += BtnExit_Click;
            header.Controls.AddRange(new Control[] { lblUser, btnExit });

            filterPanel = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.LightGray };
            
            Label lblSearch = new Label { Text = "Поиск:", Location = new Point(10, 18), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(60, 15), Size = new Size(200, 23) };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            Label lblSort = new Label { Text = "Сортировка:", Location = new Point(280, 18), AutoSize = true };
            cmbSort = new ComboBox { Location = new Point(360, 15), Size = new Size(180, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSort.Items.AddRange(new string[] { "Без сортировки", "Стоимость (возрастание)", "Стоимость (убывание)", "Размер скидки" });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += CmbSort_SelectedIndexChanged;

            btnProductAction = new Button { Text = "Добавить товар", Location = new Point(560, 12), Size = new Size(140, 28), BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnProductAction.Click += BtnProductAction_Click;

            btnOrderAction = new Button { Text = "Заказы", Location = new Point(710, 12), Size = new Size(160, 28), BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOrderAction.Click += BtnOrderAction_Click;

            filterPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblSort, cmbSort, btnProductAction, btnOrderAction });

            productsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };

            Controls.AddRange(new Control[] { productsPanel, filterPanel, header });
        }

        private void ApplyRolePermissions()
        {
            if (userRole == "Гость")
            {
                filterPanel.Visible = false;
                btnProductAction.Visible = false;
                btnOrderAction.Visible = false;
            }
            else if (userRole == "Авторизированный клиент")
            {
                filterPanel.Visible = true;
                btnProductAction.Visible = false;
                btnOrderAction.Visible = false;
            }
            else if (userRole == "Менеджер")
            {
                filterPanel.Visible = true;
                btnProductAction.Visible = false;
                btnOrderAction.Text = "Просмотр заказов";
            }
            else if (userRole == "Администратор")
            {
                filterPanel.Visible = true;
                btnProductAction.Visible = true;
                btnOrderAction.Text = "Управление заказами";
            }
            else
            {
                filterPanel.Visible = false;
                btnProductAction.Visible = false;
                btnOrderAction.Visible = false;
            }
        }

        public async void LoadProducts()
        {
            try
            {
                productsPanel.Controls.Clear();
                string searchPattern = txtSearch.Text.Trim();
                string sortOrder = cmbSort.SelectedItem.ToString();

                List<Product> products = await DbHelper.GetProductsAsync(searchPattern, sortOrder);

                foreach (Product prod in products)
                {
                    ProductCard card = new ProductCard(prod);
                    
                    if (userRole == "Администратор")
                    {
                        card.DoubleClick += delegate (object s, EventArgs e)
                        {
                            ProductForm pf = new ProductForm(prod, this);
                            pf.ShowDialog();
                        };
                    }
                    productsPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отображения каталога: " + ex.Message);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void CmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void BtnProductAction_Click(object sender, EventArgs e)
        {
            ProductForm pf = new ProductForm(null, this);
            pf.ShowDialog();
        }

        private void BtnOrderAction_Click(object sender, EventArgs e)
        {
            OrdersForm of = new OrdersForm(userRole);
            of.ShowDialog();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Hide();
            Loginform l = new Loginform();
            l.FormClosed += delegate (object s, FormClosedEventArgs args) { Close(); };
            l.Show();
        }
    }
}