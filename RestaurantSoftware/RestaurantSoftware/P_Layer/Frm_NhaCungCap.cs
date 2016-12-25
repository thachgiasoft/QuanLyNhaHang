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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace RestaurantSoftware.P_Layer
{
    public partial class Frm_NhaCungCap : DevExpress.XtraEditors.XtraForm
    {
        DataTable dt = null;
        private NhaCungCap_BLL _nccBLL = null;
        private List<int> _listUpdate = null;
        public Frm_NhaCungCap()
        {
            InitializeComponent();
            RestaurantDBDataContext dbContext = new RestaurantDBDataContext();
            dt = new DataTable();
            _nccBLL = new NhaCungCap_BLL();
            _listUpdate = new List<int>();
            lue_TrangThai.DataSource = dbContext.TrangThais.Where(trangthai => trangthai.lienket == "nhacungcap");
        }
        //Xử lý sự kiện load của form nhà cung cấp
        private void Frm_NhaCungCap_Load(object sender, EventArgs e)
        {
            LoadNhaCungCap();
        }
        // load nhà cung cấp
        private void LoadNhaCungCap()
        {
            dt = RestaurantSoftware.Utils.Utils.ConvertToDataTable<NhaCungCap>(_nccBLL.LayDanhSachNhaCungCap());
            grc_NhaCungCap.DataSource = dt;
        }
        // xử lý thêm nhà cung cap
        private void btn_Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[0];
            gridView1.ShowEditor();
            gridView1.PostEditor();
            if (KiemTraHang())
            {
                if (!_nccBLL.KiemTraNhaCungCapTonTai(gridView1.GetFocusedRowCellValue(col_TenNhaCungCap).ToString()))
                {
                    NhaCungCap ncc = new NhaCungCap();
                    ncc.tennhacungcap = gridView1.GetFocusedRowCellValue(col_TenNhaCungCap).ToString();
                    ncc.diachi = gridView1.GetFocusedRowCellValue(col_SoDienThoai).ToString();
                    ncc.sdt = gridView1.GetFocusedRowCellValue(col_SoDienThoai).ToString();
                    ncc.ghichu = gridView1.GetFocusedRowCellValue(col_GhiChu).ToString();
                    ncc.trangthai = gridView1.GetFocusedRowCellValue(col_TrangThai).ToString();
                    _nccBLL.ThemNhaCungCapMoi(ncc);
                    Notifications.Success("Thêm nhà cung cấp thành công");
                    LoadNhaCungCap();
                }
                else
                {
                    Notifications.Error("Tên nhà cung cấp đã tồn tại. Vui lòng nhập tên nhà cung cấp lại.");
                }
            }
            else
            {
                Notifications.Error("Bạn chưa nhập đầy đủ thông tin nhà cung cấp. Vui lòng nhập lại");
            }
        }
        //hàm kiểm tra một hàng trong gridview
        private bool KiemTraHang()
        {
            if (gridView1.GetFocusedRowCellValue(col_TenNhaCungCap) != null || gridView1.GetFocusedRowCellValue(col_DiaChi) != null
                || gridView1.GetFocusedRowCellValue(col_SoDienThoai) != null || gridView1.GetFocusedRowCellValue(col_TrangThai) != null )
            {
                return true;
            }
            return false;
        }
        // xử lý xóa nhà cung cấp 
        private void btn_Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int i = 0; i < gridView1.SelectedRowsCount; i++)
            {
                int _ID_NhaCungCap = int.Parse(gridView1.GetRowCellValue(gridView1.GetSelectedRows()[i], "id_nhacungcap").ToString());
                if (_nccBLL.KiemTraThongTin(_ID_NhaCungCap))
                {
                    _nccBLL.XoaTam(_ID_NhaCungCap); // xóa tạm
                }
                else
                {
                    _nccBLL.XoaNhaCungCap(_ID_NhaCungCap);
                }
            }
            Notifications.Success("Xóa dữ liệu thành công!");
            LoadNhaCungCap();
        }
        // xử lý làm mới nhà cung cấp
        private void btn_LamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadNhaCungCap();
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[0];
            gridView1.ShowEditor();
        }
        // xử lý in nhà cung cấp
        private void btn_In_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "File PDF|*.pdf|Excel|*.xls|Text rtf|*.rtf";
            saveFileDialog1.Title = "Xuất danh sách nhà cung cấp";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FilterIndex == 1)
                    gridView1.ExportToPdf(saveFileDialog1.FileName);
                if (saveFileDialog1.FilterIndex == 2)
                    grc_NhaCungCap.ExportToXls(saveFileDialog1.FileName);
                if (saveFileDialog1.FilterIndex == 3)
                    grc_NhaCungCap.ExportToRtf(saveFileDialog1.FileName);
            }
        }
        // xử lý lưu nhà cung cấp
        private void btn_Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string error = "";
            bool isUpdate = false;
            if (_listUpdate.Count > 1)
            {
                foreach (int id in _listUpdate)
                {
                    NhaCungCap ncc = new NhaCungCap();
                    ncc.id_nhacungcap = int.Parse(gridView1.GetRowCellValue(id, "id_nhacungcap").ToString());
                    ncc.tennhacungcap = gridView1.GetRowCellValue(id, "tennhacungcap").ToString();
                    ncc.diachi = gridView1.GetRowCellValue(id, "diachi").ToString();
                    ncc.sdt = gridView1.GetRowCellValue(id, "sdt").ToString();
                    ncc.ghichu = gridView1.GetRowCellValue(id, "ghichu").ToString();
                    ncc.trangthai = gridView1.GetRowCellValue(id,"trangthai").ToString();
                    if (!_nccBLL.KiemTraNhaCungCapTonTai(ncc.tennhacungcap, ncc.id_nhacungcap))
                    {
                        _nccBLL.CapNhatNhaCungCap(ncc);
                        isUpdate = true;
                    }
                    else
                    {
                        if (error == "")
                        {
                            error = ncc.tennhacungcap;
                        }
                        else
                        {
                            error += "|" + ncc.tennhacungcap;
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
                    Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Các nhà cung cấp chưa được cập nhật (" + error + "). Lỗi: Tên nhà cung cấp đã tồn tại.");
                }
            }
            else
            {
                Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Lỗi: Tên nhà cung cấp đã tồn tại.");
            }
            btn_Luu.Enabled = true;
            LoadNhaCungCap();
        }
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            btn_Xoa.Enabled = false;
            if (gridView1.SelectedRowsCount > 0 && this.gridView1.FocusedRowHandle != GridControl.NewItemRowHandle)
            {
                btn_Xoa.Enabled = true;
            }
        }
        
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

        private void grc_NhaCungCap_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.gridView1.FocusedRowHandle == GridControl.NewItemRowHandle
               && gridView1.FocusedColumn == gridView1.Columns["trangthai"])
            {
                btn_Them.PerformClick();
            }
            if (e.KeyCode == Keys.Tab && this.gridView1.FocusedRowHandle == GridControl.NewItemRowHandle
                && gridView1.FocusedColumn == gridView1.Columns["trangthai"])
            {
                gridView1.SelectRow(gridView1.FocusedRowHandle);
                gridView1.FocusedColumn = gridView1.VisibleColumns[0];
            }
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            Point p = view.GridControl.PointToClient(MousePosition);
            GridHitInfo info = view.CalcHitInfo(p);
            if (info.HitTest == GridHitTest.Column)
            {
                LoadNhaCungCap();
            }
        }

       
    }
}