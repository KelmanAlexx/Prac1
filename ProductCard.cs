using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ExamWinFormsApp1
{
    public partial class ProductCard : UserControl
    {
        private PictureBox picPhoto = new PictureBox();
        private Label lblTitle = new Label();
        private Label lblDescription = new Label();
        private Label lblManufacturer = new Label();
        private Label lblSupplier = new Label();
        private Label lblPrice = new Label();
        private Label lblUnit = new Label();
        private Label lblStock = new Label();
        private Label lblDiscount = new Label();

        public ProductCard(Product product)
        {
            InitializeComponent();
            InitializeObjects();
            LoadProductData(product);
        }

        private void InitializeObjects()
        {
            Size = new Size(740, 150);
            BorderStyle = BorderStyle.FixedSingle;
            Margin = new Padding(10);
            BackColor = Color.White;

            picPhoto = new PictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(140, 140),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTitle.Location = new Point(155, 5); lblTitle.Size = new Size(450, 22); lblTitle.Font = new Font("Times New Roman", 11, FontStyle.Bold);
            lblDescription.Location = new Point(155, 27); lblDescription.Size = new Size(450, 18); lblDescription.Font = new Font("Times New Roman", 9, FontStyle.Regular);
            lblManufacturer.Location = new Point(155, 45); lblManufacturer.Size = new Size(250, 18); lblManufacturer.Font = new Font("Times New Roman", 9, FontStyle.Regular);
            lblSupplier.Location = new Point(155, 63); lblSupplier.Size = new Size(250, 18); lblSupplier.Font = new Font("Times New Roman", 9, FontStyle.Regular);
            lblPrice.Location = new Point(155, 81); lblPrice.Size = new Size(400, 22); lblPrice.Font = new Font("Times New Roman", 10, FontStyle.Regular);
            lblUnit.Location = new Point(155, 105); lblUnit.Size = new Size(200, 18); lblUnit.Font = new Font("Times New Roman", 9, FontStyle.Regular);
            lblStock.Location = new Point(155, 123); lblStock.Size = new Size(200, 18); lblStock.Font = new Font("Times New Roman", 9, FontStyle.Regular);

            lblDiscount = new Label
            {
                Location = new Point(610, 45),
                Size = new Size(110, 60),
                Font = new Font("Times New Roman", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.AddRange(new Control[] {
                picPhoto, lblTitle, lblDescription, lblManufacturer,
                lblSupplier, lblPrice, lblUnit, lblStock, lblDiscount
            });
        }

        private void LoadProductData(Product p)
        {
            if (p.StockQuantity <= 0)
            {
                BackColor = Color.LightBlue;
                ApplyTextColor(Color.Black);
            }
            else if (p.Discount > 15)
            {
                BackColor = ColorTranslator.FromHtml("#2E8B57");
                ApplyTextColor(Color.White);
            }
            else
            {
                BackColor = Color.White;
                ApplyTextColor(Color.Black);
            }

            string path = !string.IsNullOrEmpty(p.Photo) && File.Exists("Images/" + p.Photo)
                ? "Images/" + p.Photo : "Images/picture.png";
            try { picPhoto.Image = Image.FromFile(path); } catch { picPhoto.BackColor = Color.LightGray; }

            lblTitle.Text = p.Category + " | " + p.ProductName + " (" + p.Article + ")";
            lblDescription.Text = "Описание: " + p.Description;
            lblManufacturer.Text = "Производитель: " + p.Manufacturer;
            lblSupplier.Text = "Поставщик: " + p.Supplier;
            lblUnit.Text = "Ед. измерения: " + p.Unit;
            lblStock.Text = "Остаток на складе: " + p.StockQuantity;

            if (p.StockQuantity <= 0)
            {
                lblStock.ForeColor = Color.Red;
            }

            if (p.Discount > 0)
            {
                decimal newPrice = p.Price * (1 - p.Discount / 100);
                lblPrice.Text = "Цена: " + p.Price.ToString("N2") + " ₽  →  " + newPrice.ToString("N2") + " ₽";
                
                lblPrice.Font = new Font("Times New Roman", 10, FontStyle.Strikeout);
                lblPrice.ForeColor = Color.Red;
                
                Label lblNewPrice = new Label();
                lblNewPrice.Text = "Итого: " + newPrice.ToString("N2") + " ₽";
                lblNewPrice.Font = new Font("Times New Roman", 11, FontStyle.Bold);
                lblNewPrice.ForeColor = (p.Discount > 15 && p.StockQuantity > 0) ? Color.White : Color.Black;
                lblNewPrice.Location = new Point(320, 81);
                lblNewPrice.Size = new Size(180, 22);
                Controls.Add(lblNewPrice);
            }
            else
            {
                lblPrice.Text = "Цена: " + p.Price.ToString("N2") + " ₽";
                lblPrice.Font = new Font("Times New Roman", 10, FontStyle.Regular);
            }

            lblDiscount.Text = "Скидка:\n" + p.Discount + "%";
            if (p.Discount > 15 && p.StockQuantity > 0)
                lblDiscount.ForeColor = Color.Yellow;
            else
                lblDiscount.ForeColor = p.Discount > 0 ? Color.Green : Color.Gray;
        }

        private void ApplyTextColor(Color color)
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl is Label)
                {
                    ctrl.ForeColor = color;
                }
            }
        }
    }
}