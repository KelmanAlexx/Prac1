using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1;

namespace ExamWinFormsApp1
{
    public partial class Loginform : Form
    {
        private TextBox txtLogin = new TextBox();
        private TextBox txtPassword = new TextBox();

        public Loginform()
        {
            InitializeComponent();
            SetupForm();
            InitializeObjects();
        }

        private void SetupForm()
        {
            Text = "Авторизация";
            Size = new Size(350, 280);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Times New Roman", 14);
            MaximizeBox = false;
        }

        private void InitializeObjects()
        {
            var picLogo = new PictureBox { Location = new Point(100, 10), Size = new Size(150, 50), SizeMode = PictureBoxSizeMode.Zoom };
            try { picLogo.Image = Image.FromFile("Images/Icon.png"); } catch { }

            txtLogin = new TextBox { Location = new Point(40, 95), Size = new Size(250, 25) };
            txtPassword = new TextBox { Location = new Point(40, 150), Size = new Size(250, 25), PasswordChar = '*' };

            var btnLogin = CreateButton("Войти", new Point(40, 190), "#00FA9A", BtnLogin_Click);
            var btnGuest = CreateButton("Гость", new Point(170, 190), "#7FFF00", BtnGuest_Click);

            Controls.AddRange(new Control[] {
                picLogo,
                new Label { Text = "Логин:", Location = new Point(40, 70), Size = new Size(80, 25) },
                txtLogin,
                new Label { Text = "Пароль:", Location = new Point(40, 125), Size = new Size(80, 25) },
                txtPassword,
                btnLogin, btnGuest
            });
        }

        private Button CreateButton(string text, Point loc, string color, EventHandler click)
        {
            Button btn = new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(120, 35),
                BackColor = ColorTranslator.FromHtml(color),
                FlatStyle = FlatStyle.Flat
            };
            btn.Click += click;
            return btn;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Text))
            { 
                MessageBox.Show("Введите логин и пароль!"); 
                return; 
            }

            try
            {
                DataTable table = await DbHelper.CheckUserAsync(txtLogin.Text, txtPassword.Text);

                if (table.Rows.Count > 0)
                {
                    string name = table.Rows[0]["full_name"].ToString();
                    string role = table.Rows[0]["role"].ToString();
                    OpenMain(name, role);
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Ошибка подключения: " + ex.Message); 
            }
        }

        private void BtnGuest_Click(object sender, EventArgs e)
        {
            OpenMain("Гость", "Гость");
        }

        private void OpenMain(string name, string role)
        {
            Hide();
            MainForm main = new MainForm(name, role);
            main.FormClosed += delegate (object s, FormClosedEventArgs args) { Close(); };
            main.Show();
        }
    }
}