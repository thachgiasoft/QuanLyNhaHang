﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using RestaurantSoftware.BL_Layer;
using RestaurantSoftware.DA_Layer;
using DevExpress.XtraGrid;
using RestaurantSoftware.Utils;

namespace RestaurantSoftware.P_Layer
{
    public partial class Frm_NguyenLieu : DevExpress.XtraEditors.XtraForm
    {
        DataTable dt = null;
        private HangHoa_BLL _hanghoaBLL=null;
        private List<int> _listUpdate = null;
        // khởi tạo lớp form nguyên liệu
        public Frm_NguyenLieu()
        {
            InitializeComponent();
            dt = new DataTable();
            _hanghoaBLL = new HangHoa_BLL();
            _listUpdate = new List<int>();

            // load dữ liệu loại hàng hóa
            lue_LoaiHang.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().LoaiHangHoas;

            // load dữ liệu đơn vị
            lue_DonVi.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().DonVis;

            //load dữ liệu nhà cung cấp
            //lue_NhaCungCap.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().NhaCungCaps;
            // This line of code is generated by Data Source Configuration Wizard
            gridControl1.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().NhaCungCaps;
        }

        private void Frm_NguyenLieu_Load(object sender, EventArgs e)
        {
            LoadHangHoa();
        }
        // load hàng hoá
        private void LoadHangHoa()
        {
            dt = RestaurantSoftware.Utils.Utils.ConvertToDataTable<HangHoa>(_hanghoaBLL.LayDanhSachHangHoa());
            gridControl1.DataSource = dt;
        }
        // xử lý thêm nguyên liệu
        private void btn_Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[0];
            gridView1.ShowEditor();
            gridView1.PostEditor();
            if (KiemTraHang())
            {
                if (!_hanghoaBLL.KiemTraHangHoaTonTai(gridView1.GetFocusedRowCellValue(col_TenHangHoa).ToString()))
                {
                    HangHoa hh = new HangHoa();
                    hh.tenhanghoa = gridView1.GetFocusedRowCellValue(col_TenHangHoa).ToString() ;
                    hh.id_loaihang = int.Parse(gridView1.GetFocusedRowCellValue(col_LoaiHang).ToString());
                    hh.soluong = int.Parse(gridView1.GetFocusedRowCellValue(col_SoLuong).ToString());
                    hh.dongia = decimal.Parse(gridView1.GetFocusedRowCellValue(col_DonGia).ToString());
                    hh.id_donvi = int.Parse(gridView1.GetFocusedRowCellValue(col_DonVi).ToString());
                    //hh.id_nhacungcap = int.Parse(gridView1.GetFocusedRowCellValue(col_NhaCungCap).ToString());
                    _hanghoaBLL.ThemHangHoaMoi(hh);
                    Notifications.Success("Thêm hàng hoá thành công");
                    LoadHangHoa();
                }
                else
                {
                    Notifications.Error("Tên hàng hoá đã tồn tại. Vui lòng nhập tên hàng hoá lại.");
                }
            }
            else
            {
                Notifications.Error("Bạn chưa nhập đầy đủ thông tin hàng hoá. Vui lòng nhập lại");
            }

        }
        //hàm kiểm tra một hàng trong gridview
        private bool KiemTraHang()
        {
            if (gridView1.GetFocusedRowCellValue(col_TenHangHoa) != null || gridView1.GetFocusedRowCellValue(col_LoaiHang) != null
                || int.Parse(gridView1.GetFocusedRowCellValue(col_SoLuong).ToString()) >= 0 || int.Parse(gridView1.GetFocusedRowCellValue(col_DonGia).ToString()) >=0)
            {
                return true;
            }
            return false;
        }
        // hàm xoá hàng hoá
        private void btn_Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int i = 0; i < gridView1.SelectedRowsCount; i++)
            {
                if (gridView1.GetRowCellValue(gridView1.GetSelectedRows()[i], "trangthai").Equals("Hết"))
                {
                    if (Notifications.Answers("Bạn thật sự có muốn xóa dữ liệu không") == DialogResult.Cancel)
                    {
                        return;
                    }
                    int _ID_HangHoa = int.Parse(gridView1.GetRowCellValue(gridView1.GetSelectedRows()[i], "id_hanghoa").ToString());
                    _hanghoaBLL.XoaHangHoa(_ID_HangHoa);
                    Notifications.Success("Bạn xóa thành công");
                }
                else
                {
                    Notifications.Success("Bạn không được phép xoá");
                }

            }
            LoadHangHoa();
        }
         // xử lý khi update dữ liệu trên hàng
        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            if (this.gridView1.FocusedRowHandle != GridControl.NewItemRowHandle)
            {
                btn_Luu.Enabled = true;
                _listUpdate.Add(e.RowHandle);
            }
            else
            {
                btn_Luu.Enabled = false;
            }
        }
        // hàm lưu hàng hoá
        private void btn_Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             string error = "";
            bool isUpdate = false;
            if (_listUpdate.Count > 1)
            {
                foreach (int id in _listUpdate)
                {
                    HangHoa hh =new HangHoa();
                    hh.id_hanghoa = int.Parse(gridView1.GetRowCellValue(id,"id_hanghoa").ToString());
                    hh.tenhanghoa =gridView1.GetRowCellValue(id,"tenhanghoa").ToString();
                    hh.id_loaihang =int.Parse(gridView1.GetRowCellValue(id,"id_loaihang").ToString());
                    hh.soluong =int.Parse(gridView1.GetRowCellValue(id,"soluong").ToString());
                    hh.dongia = decimal.Parse(gridView1.GetRowCellValue(id,"dongia").ToString());
                    hh.id_donvi =int.Parse(gridView1.GetRowCellValue(id,"id_donvi").ToString());
                    //hh.id_nhacungcap =int.Parse(gridView1.GetRowCellValue(id,"id_nhacungcap").ToString());
                    if (!_hanghoaBLL.KiemTraHangHoaTonTai(hh.tenhanghoa, hh.id_hanghoa))
                    {
                        _hanghoaBLL.CapNhatHangHoa(hh);
                        isUpdate = true;
                    }
                    else
                    {
                        if (error == "")
                        {
                            error = hh.tenhanghoa;
                        }
                        else
                        {
                            error += "|" + hh.tenhanghoa;
                        }
                    }
                }
            }
            if (isUpdate == true)
            {
                if (error.Length == 0)
                {
                    Notifications.Success("Cập dữ liệu thành công.");
                }
                else
                {
                    Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Các hàng hoá chưa được cập nhật (" + error + "). Lỗi: Tên hàng hoá đã tồn tại.");
                }
            }
            else
            {
                Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Lỗi: Tên hàng hoá đã tồn tại.");
            }
        }
        // xử lý làm mới 
        private void btn_LamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
             LoadHangHoa();
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[0];
            gridView1.ShowEditor();
        }
        // xử lý in
        private void btn_In_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "File PDF|*.pdf|Excel|*.xls|Text rtf|*.rtf";
            saveFileDialog1.Title = "Xuất danh sách hàng hoá";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FilterIndex == 1)
                    gridView1.ExportToPdf(saveFileDialog1.FileName);
                if (saveFileDialog1.FilterIndex == 2)
                    gridControl1.ExportToXls(saveFileDialog1.FileName);
                if (saveFileDialog1.FilterIndex == 3)
                    gridControl1.ExportToRtf(saveFileDialog1.FileName);
            }
        }

    }
}