using Microsoft.EntityFrameworkCore.Storage;
using QuanLyBanHang.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;
using QuanLyBanHang;

namespace QuanLyBanHang.Forms
{
    public partial class frmSanPham : Form
    {

        QLBHDbContext context = new QLBHDbContext();
        bool xuLyThem = false;
        int id;
        string imagesFolder = Application.StartupPath.Replace("bin\\Debug\\net5.0-windows", "Images");
        public frmSanPham()
        {
            InitializeComponent();
        }

        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuyBo.Enabled = giaTri;
            cboHangSanXuat.Enabled = giaTri;
            cboLoaiSanPham.Enabled = giaTri;
            txtTenSanPham.Enabled = giaTri;
            numSoLuong.Enabled = giaTri;
            numDonGia.Enabled = giaTri;
            txtMoTa.Enabled = giaTri;
            picHinhAnh.Enabled = giaTri;

            btnThem.Enabled = !giaTri;
            btnDoiAnh.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
            btnTimKiem.Enabled = !giaTri;
            btnNhap.Enabled = !giaTri;
            btnXuat.Enabled = !giaTri;
        }

        public void LayLoaiSanPhamVaoComboBox()
        {
            cboLoaiSanPham.DataSource = context.LoaiSanPhams.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";

        }

        public void LayHangSanXuatVaoComboBox()
        {
            cboHangSanXuat.DataSource = context.HangSanXuats.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";

        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LayLoaiSanPhamVaoComboBox();
            LayHangSanXuatVaoComboBox();

            dataGridView.AutoGenerateColumns = false;
            //dataGridView.Columns["HinhAnh"].ValueType = typeof(Image);

            dataGridView.CellFormatting += dataGridView_CellFormatting;

            ((DataGridViewImageColumn)dataGridView.Columns["HinhAnh"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

            List<DanhSachSanPham> sp = new List<DanhSachSanPham>();
            sp = context.SanPhams.Select(r => new DanhSachSanPham
            {
                Id = r.Id,
                LoaiSanPhamId = r.LoaiSanPhamId,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatId = r.HangSanXuatId,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                SoLuong = r.SoLuong,
                DonGia = r.DonGia,
                HinhAnh = r.HinhAnh

            }).ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = sp;

            cboLoaiSanPham.DataBindings.Clear();
            cboLoaiSanPham.DataBindings.Add("SelectedValue", bindingSource, "LoaiSanPhamId", false, DataSourceUpdateMode.Never);

            cboHangSanXuat.DataBindings.Clear();
            cboHangSanXuat.DataBindings.Add("SelectedValue", bindingSource, "HangSanXuatId", false, DataSourceUpdateMode.Never);

            txtTenSanPham.DataBindings.Clear();
            txtTenSanPham.DataBindings.Add("Text", bindingSource, "TenSanPham", false, DataSourceUpdateMode.Never);

            txtMoTa.DataBindings.Clear();
            txtMoTa.DataBindings.Add("Text", bindingSource, "MoTa", false, DataSourceUpdateMode.Never);

            numSoLuong.DataBindings.Clear();
            numSoLuong.DataBindings.Add("Value", bindingSource, "SoLuong", false, DataSourceUpdateMode.Never);

            numDonGia.DataBindings.Clear();
            numDonGia.DataBindings.Add("Value", bindingSource, "DonGia", false, DataSourceUpdateMode.Never);

            picHinhAnh.DataBindings.Clear();
            Binding hinhAnh = new Binding("ImageLocation", bindingSource, "HinhAnh");
            hinhAnh.Format += (s, e) =>
            {
                e.Value = Path.Combine(imagesFolder, e.Value.ToString());
            };
            picHinhAnh.DataBindings.Add(hinhAnh);

            dataGridView.DataSource = bindingSource;


        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (dataGridView.Columns[e.ColumnIndex].Name == "HinhAnh") ;
            //Image image = Image.FromFile(Path.Combine(imagesFolder, e.ColumnIndex.ToString()));
            //image = new Bitmap(image, 24, 24);

            // e.Value = image;

            if (dataGridView.Columns[e.ColumnIndex].Name == "HinhAnh")
            {
                if (e.Value != null)
                {
                    string filePath = Path.Combine(imagesFolder, e.Value.ToString());

                    if (File.Exists(filePath))
                    {
                        Image img = Image.FromFile(filePath);
                        e.Value = new Bitmap(img, 60, 60);
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }



        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            cboHangSanXuat.Text = "";
            cboLoaiSanPham.Text = "";
            txtTenSanPham.Clear();
            txtMoTa.Clear();
            numSoLuong.Value = 0;
            numDonGia.Value = 0;
            picHinhAnh.Image = null;

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(true);
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboLoaiSanPham.Text))
                MessageBox.Show("Vui lòng chọn loại sản phẩm.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            else if (string.IsNullOrWhiteSpace(cboHangSanXuat.Text))
                MessageBox.Show("Vui lòng chọn hãng sản xuất.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            else if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
                MessageBox.Show("Vui lòng nhập tên sản phẩm.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            else if (numSoLuong.Value <= 0)
                MessageBox.Show("Số lượng phải lớn hơn 0.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            else if (numDonGia.Value <= 0)
                MessageBox.Show("Đơn giá phải lớn hơn 0.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            else
            {
                if (xuLyThem) // Thêm mới
                {
                    SanPham sp = new SanPham();

                    sp.LoaiSanPhamId =
                    Convert.ToInt32(cboLoaiSanPham.SelectedValue);

                    sp.HangSanXuatId =
                    Convert.ToInt32(cboHangSanXuat.SelectedValue);

                    sp.TenSanPham = txtTenSanPham.Text;

                    sp.SoLuong = Convert.ToInt32(numSoLuong.Value);

                    sp.DonGia = Convert.ToInt32(numDonGia.Value);

                    sp.MoTa = txtMoTa.Text;

                    if (picHinhAnh.ImageLocation != null)
                        sp.HinhAnh =
                        Path.GetFileName(picHinhAnh.ImageLocation);

                    context.SanPhams.Add(sp);
                    context.SaveChanges();
                }
                else // Sửa
                {
                    SanPham sp = context.SanPhams.Find(id);

                    if (sp != null)
                    {
                        sp.LoaiSanPhamId =
                        Convert.ToInt32(cboLoaiSanPham.SelectedValue);

                        sp.HangSanXuatId =
                        Convert.ToInt32(cboHangSanXuat.SelectedValue);

                        sp.TenSanPham = txtTenSanPham.Text;

                        sp.SoLuong =
                        Convert.ToInt32(numSoLuong.Value);

                        sp.DonGia =
                        Convert.ToInt32(numDonGia.Value);

                        sp.MoTa = txtMoTa.Text;

                        if (picHinhAnh.ImageLocation != null)
                            sp.HinhAnh =
                            Path.GetFileName(picHinhAnh.ImageLocation);

                        context.SaveChanges();
                    }
                }

                MessageBox.Show("Đã lưu thành công!");

                frmSanPham_Load(sender, e);
            }

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận xóa sản phẩm " + txtTenSanPham.Text + "?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
DialogResult.Yes)
            {
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                SanPham sp = context.SanPhams.Find(id);
                if (sp != null)
                {
                    context.SanPhams.Remove(sp);
                }
                context.SaveChanges();

                frmSanPham_Load(sender, e);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmSanPham_Load(sender, e);
        }

        private void btnDoiAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Cập nhật hình ảnh sản phẩm";
            openFileDialog.Filter = "Tập tin hình ảnh|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                string ext = Path.GetExtension(openFileDialog.FileName);
                string fileSavePath = Path.Combine(imagesFolder, fileName.GenerateSlug() + ext);
                File.Copy(openFileDialog.FileName, fileSavePath, true);

                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                SanPham sp = context.SanPhams.Find(id);
                sp.HinhAnh = fileName.GenerateSlug() + ext;
                context.SanPhams.Update(sp);

                context.SaveChanges();
                frmSanPham_Load(sender, e);
            }

        }

            private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            var sp = context.SanPhams.AsQueryable();

            // Tìm theo loại sản phẩm
            if (!string.IsNullOrEmpty(cboLoaiSanPham.Text))
            {
                int loaiId = Convert.ToInt32(cboLoaiSanPham.SelectedValue);
                sp = sp.Where(r => r.LoaiSanPhamId == loaiId);
            }

            // Tìm theo hãng sản xuất
            if (!string.IsNullOrEmpty(cboHangSanXuat.Text))
            {
                int hangId = Convert.ToInt32(cboHangSanXuat.SelectedValue);
                sp = sp.Where(r => r.HangSanXuatId == hangId);
            }

            // Tìm theo tên sản phẩm
            if (!string.IsNullOrEmpty(txtTenSanPham.Text))
            {
                sp = sp.Where(r => r.TenSanPham.Contains(txtTenSanPham.Text));
            }

            var ketQua = sp.Select(r => new DanhSachSanPham
            {
                Id = r.Id,
                LoaiSanPhamId = r.LoaiSanPhamId,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatId = r.HangSanXuatId,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                SoLuong = r.SoLuong,
                DonGia = r.DonGia,
                HinhAnh = r.HinhAnh
            }).ToList();

            dataGridView.DataSource = ketQua;
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xlsx";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook workbook;
                Microsoft.Office.Interop.Excel.Worksheet worksheet;
                Microsoft.Office.Interop.Excel.Range range;

                excel = new Microsoft.Office.Interop.Excel.Application();
                workbook = excel.Workbooks.Open(ofd.FileName);
                worksheet = workbook.ActiveSheet;
                range = worksheet.UsedRange;

                int rowCount = range.Rows.Count;

                for (int i = 2; i <= rowCount; i++)
                {
                    SanPham sp = new SanPham();

                    // Phân loại
                    string tenLoai = range.Cells[i, 2].Value.ToString();
                    var loai = context.LoaiSanPhams
                              .FirstOrDefault(x => x.TenLoai == tenLoai);
                    if (loai != null)
                        sp.LoaiSanPhamId = loai.Id;

                    // Hãng sản xuất
                    string tenHang = range.Cells[i, 3].Value.ToString();
                    var hang = context.HangSanXuats
                              .FirstOrDefault(x => x.TenHangSanXuat == tenHang);
                    if (hang != null)
                        sp.HangSanXuatId = hang.Id;

                    // Tên sản phẩm
                    sp.TenSanPham = range.Cells[i, 4].Value.ToString();

                    // Số lượng
                    sp.SoLuong = Convert.ToInt32(range.Cells[i, 5].Value);

                    // Đơn giá
                    sp.DonGia = Convert.ToDecimal(range.Cells[i, 6].Value);

                    // Hình ảnh
                    if (range.Cells[i, 7].Value != null)
                        sp.HinhAnh = Path.GetFileName(range.Cells[i, 7].Value.ToString());

                    context.SanPhams.Add(sp);
                }

                context.SaveChanges();

                workbook.Close();
                excel.Quit();

                MessageBox.Show("Nhập Excel thành công!");

                frmSanPham_Load(sender, e);
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.FileName = "DanhSachSanPham.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook workbook;
                Microsoft.Office.Interop.Excel.Worksheet worksheet;

                excel = new Microsoft.Office.Interop.Excel.Application();
                workbook = excel.Workbooks.Add(Type.Missing);
                worksheet = workbook.ActiveSheet;

                worksheet.Name = "SanPham";

                // Header
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] =
                    dataGridView.Columns[i].HeaderText;
                }

                // Data
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] =
                        dataGridView.Rows[i].Cells[j].Value;
                    }
                }

                workbook.SaveAs(sfd.FileName);
                workbook.Close();
                excel.Quit();

                MessageBox.Show("Xuất Excel thành công!");
            }
        }
    }
}
