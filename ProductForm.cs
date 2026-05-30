using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1;

namespace ExamWinFormsApp1
{
    public partial class ProductForm : Form
    {
        private TextBox txtArticle = new TextBox();
        private TextBox txtName = new TextBox();
        private TextBox txtPrice = new TextBox();
        private TextBox txtManufacturer = new TextBox();
        private TextBox txtSupplier = new TextBox();
        private TextBox txtDiscount = new TextBox();
        private TextBox txtStock = new TextBox();
        private ComboBox cmbCategory = new ComboBox();
        private TextBox txtUnit = new TextBox();
        private TextBox txtDescription = new TextBox();
        private TextBox txtPhoto = new TextBox();

        private Button btnSave = new Button();
        private Button btnDelete = new Button();
        
        private bool isNewProject = true;
        private MainForm mainFormReference;

        public ProductForm(Product currentProduct, MainForm mainForm)
        {
            mainFormReference = mainForm;
            Size = new Size(400, 550);
            Font = new Font("Times New Roman", 11);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            BuildInterface();

            if (currentProduct != null)
            {
                isNewProject = false;
                Text = "Редактирование товара";
                txtArticle.Text = currentProduct.Article;
                txtArticle.ReadOnly = true;
                txtName.Text = currentProduct.ProductName;
                txtPrice.Text = currentProduct.Price.ToString();
                txtManufacturer.Text = currentProduct.Manufacturer;
                txtSupplier.Text = currentProduct.Supplier;
                txtDiscount.Text = currentProduct.Discount.ToString();
                txtStock.Text = currentProduct.StockQuantity.ToString();
                cmbCategory.SelectedItem = currentProduct.Category;
                txtUnit.Text = currentProduct.Unit;
                txtDescription.Text = currentProduct.Description;
                txtPhoto.Text = currentProduct.Photo;
                btnDelete.Enabled = true;
            }
            else
            {
                Text = "Добавление товара";
                txtUnit.Text = "пара";
                btnDelete.Enabled = false;
            }
        }

        private void BuildInterface()
        {
            int top = 15;
            Controls.Add(new Label { Text = "Артикул:", Location = new Point(15, top), AutoSize = true });
            txtArticle = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtArticle);

            top += 35;
            Controls.Add(new Label { Text = "Название:", Location = new Point(15, top), AutoSize = true });
            txtName = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtName);

            top += 35;
            Controls.Add(new Label { Text = "Цена:", Location = new Point(15, top), AutoSize = true });
            txtPrice = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtPrice);

            top += 35;
            Controls.Add(new Label { Text = "Категория:", Location = new Point(15, top), AutoSize = true });
            cmbCategory = new ComboBox { Location = new Point(140, top), Size = new Size(220, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategory.Items.AddRange(new string[] { "Мужская", "Женская", "Детская" });
            cmbCategory.SelectedIndex = 0;
            Controls.Add(cmbCategory);

            top += 35;
            Controls.Add(new Label { Text = "Производитель:", Location = new Point(15, top), AutoSize = true });
            txtManufacturer = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtManufacturer);

            top += 35;
            Controls.Add(new Point(15, top) == Point.Empty ? null : new Label { Text = "Поставщик:", Location = new Point(15, top), AutoSize = true });
            txtSupplier = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtSupplier);

            top += 35;
            Controls.Add(new Label { Text = "Скидка (%):", Location = new Point(15, top), AutoSize = true });
            txtDiscount = new TextBox { Location = new Point(140, top), Size = new Size(220, 25), Text = "0" };
            Controls.Add(txtDiscount);

            top += 35;
            Controls.Add(new Label { Text = "Кол-во на складе:", Location = new Point(15, top), AutoSize = true });
            txtStock = new TextBox { Location = new Point(140, top), Size = new Size(220, 25), Text = "0" };
            Controls.Add(txtStock);

            top += 35;
            Controls.Add(new Label { Text = "Ед. измерения:", Location = new Point(15, top), AutoSize = true });
            txtUnit = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtUnit);

            top += 35;
            Controls.Add(new Label { Text = "Описание:", Location = new Point(15, top), AutoSize = true });
            txtDescription = new TextBox { Location = new Point(140, top), Size = new Size(220, 25) };
            Controls.Add(txtDescription);

            top += 35;
            Controls.Add(new Label { Text = "Имя файла фото:", Location = new Point(15, top), AutoSize = true });
            txtPhoto = new TextBox { Location = new Point(140, top), Size = new Size(220, 25), Text = "picture.png" };
            Controls.Add(txtPhoto);

            top += 45;
            btnSave = new Button { Text = "Сохранить", Location = new Point(40, top), Size = new Size(130, 35), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;

            btnDelete = new Button { Text = "Удалить", Location = new Point(210, top), Size = new Size(130, 35), BackColor = Color.LightCoral, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;

            Controls.AddRange(new Control[] { btnSave, btnDelete });
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtArticle.Text) || string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Заполните артикул, наименование и цену!");
                return;
            }

            try
            {
                Product p = new Product();
                p.Article = txtArticle.Text.Trim();
                p.ProductName = txtName.Text.Trim();
                p.Price = Convert.ToDecimal(txtPrice.Text);
                p.Category = cmbCategory.SelectedItem.ToString();
                p.Manufacturer = txtManufacturer.Text.Trim();
                p.Supplier = txtSupplier.Text.Trim();
                p.Discount = Convert.ToDecimal(txtDiscount.Text);
                p.StockQuantity = Convert.ToInt32(txtStock.Text);
                p.Unit = txtUnit.Text.Trim();
                p.Description = txtDescription.Text.Trim();
                p.Photo = txtPhoto.Text.Trim();

                bool success = await DbHelper.SaveProductAsync(p, isNewProject);
                if (success)
                {
                    MessageBox.Show("Данные успешно сохранены!");
                    mainFormReference.LoadProducts();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка валидации/сохранения: " + ex.Message);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                bool success = await DbHelper.DeleteProductAsync(txtArticle.Text);
                if (success)
                {
                    MessageBox.Show("Товар удален!");
                    mainFormReference.LoadProducts();
                    Close();
                }
            }
        }
    }
}