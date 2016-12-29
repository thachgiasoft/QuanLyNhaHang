﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using RestaurantSoftware.BL_Layer;
using RestaurantSoftware.DA_Layer;
using RestaurantSoftware.P_Layer;
using RestaurantSoftware.Utils;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;

namespace RestaurantSoftware.P_Layer
{
    public partial class Frm_Mon : DevExpress.XtraEditors.XtraForm
    {
        DataTable dt = new DataTable();
        private Mon_BLL _monBll = new Mon_BLL();
        private LoaiMon_BLL _loaimonBll = new LoaiMon_BLL();
        private List<int> _listUpdate = new List<int>();
        public Frm_Mon()
        {
            InitializeComponent();
            RestaurantDBDataContext db = new RestaurantDBDataContext();
            // This line of code is generated by Data Source Configuration Wizard
            lue_LoaiMon.DataSource = db.LoaiMons;
            // This line of code is generated by Data Source Configuration Wizard
            lue_TrangThai.DataSource = db.TrangThais.Where(TrangThai => TrangThai.lienket == "mon");
            lue_DonVi.DataSource = db.DonVis.Where(DonVi => DonVi.lienket == "mon");
        }

        private void Frm_Mon_Load(object sender, EventArgs e)
        {
            LoadDataSource();
        }
        private void LoadDataSource()
        {
            dt = Utils.Utils.ConvertToDataTable<Mon>(_monBll.LayDanhSachMon());
            gridControl1.DataSource = dt;
        }

        private void btn_Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[1];
            gridView1.ShowEditor();
            gridView1.PostEditor();
            if (KiemTraHang())
            {
                string tenmon = gridView1.GetFocusedRowCellValue(col_TenMon).ToString();
                if (_monBll.KiemTraTenMonTonTai(tenmon) != -1)
                {
                    try
                    {
                        Mon mon = new Mon();
                        mon.tenmon = gridView1.GetFocusedRowCellValue(col_TenMon).ToString();
                        mon.id_loaimon = int.Parse(gridView1.GetFocusedRowCellValue(col_LoaiMon).ToString());
                        mon.tenviettat = gridView1.GetFocusedRowCellValue(col_TenVietTat).ToString();
                        mon.trangthai = gridView1.GetFocusedRowCellValue(col_TrangThai).ToString();
                        mon.gia = decimal.Parse(gridView1.GetFocusedRowCellValue(col_Gia).ToString());
                        mon.id_donvi = int.Parse(gridView1.GetFocusedRowCellValue(col_DonVi).ToString());
                        if(_monBll.KiemTraTenMonTonTai(tenmon) == 1)
                        {
                            _monBll.ThemMonMoi(mon);
                        }
                        else
                        {
                            mon.id_mon = _monBll.LayIdMon(tenmon);
                            _monBll.CapNhatMon(mon);
                        }
                        Notifications.Success("Thêm món mới thành công!");
                        LoadDataSource();
                        btn_Luu.Enabled = false;
                        _listUpdate.Clear();
                    }
                    catch (Exception)
                    {
                        Notifications.Error("Bạn chưa nhập đầy đủ thông tin món. Vui lòng nhập lại!");
                    }
                }
                else
                {
                    Notifications.Error("Tên món đã tồn tại. Vui lòng nhập tên món lại.");
                }
            }
            else
            {
                Notifications.Error("Bạn chưa nhập đầy đủ thông tin món. Vui lòng nhập lại!");
            }
        }

        private bool KiemTraHang()
        {
            if (gridView1.GetFocusedRowCellValue(col_TenMon) != null && gridView1.GetFocusedRowCellValue(col_LoaiMon) != null
                && gridView1.GetFocusedRowCellValue(col_TrangThai) != null && gridView1.GetFocusedRowCellValue(col_Gia) != null 
                && gridView1.GetFocusedRowCellValue(col_DonVi) != null && gridView1.GetFocusedRowCellValue(col_TenMon).ToString() != ""
                && gridView1.GetFocusedRowCellValue(col_LoaiMon).ToString() != "" && gridView1.GetFocusedRowCellValue(col_TrangThai).ToString() != "" 
                && gridView1.GetFocusedRowCellValue(col_Gia).ToString() != "" && gridView1.GetFocusedRowCellValue(col_DonVi).ToString() != "")
                return true;
            return false;
        }

        private void btn_Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Notifications.Answers("Bạn thật sự muốn xóa dữ liệu?") == DialogResult.Cancel)
            {
                return;
            }
            for (int i = 0; i < gridView1.SelectedRowsCount; i++)
            {
                int _ID_Mon = int.Parse(gridView1.GetRowCellValue(gridView1.GetSelectedRows()[i], "id_mon").ToString());
                if (_monBll.KiemTraThongTin(_ID_Mon))
                {
                    _monBll.XoaTam(_ID_Mon);
                }
                else
                {
                    _monBll.XoaMon(_ID_Mon);
                }
            }
            Notifications.Success("Xóa dữ liệu thành công!");
            LoadDataSource();
        }

        private void btn_Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string error = "";
            bool isUpdate = false;
            bool KiemTra = false;
            if (_listUpdate.Count > 0)
                foreach (int id in _listUpdate)
                {
                    Mon mon = new Mon();
                    mon.id_mon = int.Parse(gridView1.GetRowCellValue(id, "id_mon").ToString());
                    mon.tenmon = gridView1.GetRowCellValue(id, "tenmon").ToString();
                    mon.id_loaimon = int.Parse(gridView1.GetRowCellValue(id, "id_loaimon").ToString());
                    mon.tenviettat = gridView1.GetRowCellValue(id, "tenviettat").ToString();
                    mon.gia = (gridView1.GetRowCellValue(id, "gia").ToString() == "") ? decimal.Parse(gridView1.GetRowCellValue(id, "gia").ToString()) : 0;
                    mon.trangthai = gridView1.GetRowCellValue(id, "trangthai").ToString();
                    mon.id_donvi = int.Parse(gridView1.GetRowCellValue(id, "id_donvi").ToString());
                    if(_monBll.KiemTraMon(mon))
                    {
                        if (_monBll.KiemTraTenMonTonTai(mon.tenmon, mon.id_mon) == 1)
                        {
                            _monBll.CapNhatMon(mon);
                            isUpdate = true;
                            btn_Luu.Enabled = false;
                        }
                        else
                        {
                            if (error == "")
                            {
                                error = mon.tenmon;
                            }
                            else
                            {
                                error += " | " + mon.tenmon;
                            }
                        }
                    }
                    else
                    {
                        KiemTra = true;
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
                    Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Các món chưa được cập nhật (" + error + "). Lỗi: Tên món đã tồn tại.");
                }
            }
            else if(KiemTra == true)
            {
                Notifications.Error("Lỗi xảy ra khi cập nhật dữ liệu. Lỗi: Dữ liệu không được rỗng, giá khác 0.");
            }
            else
            {
                Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Lỗi: Tên món đã tồn tại.");
            }
            LoadDataSource();
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

        private void btn_LamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDataSource();
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[1];
            gridView1.ShowEditor();
            btn_Luu.Enabled = false;
        }
        private void btn_In_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "File PDF|*.pdf|Excel|*.xls|Text rtf|*.rtf";
            saveFileDialog1.Title = "Xuất danh sách món";
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

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == "col_STT" && e.RowHandle != GridControl.NewItemRowHandle)
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
            if (e.Column.Name == "col_STT" && e.RowHandle == GridControl.NewItemRowHandle)
            {
                e.DisplayText = (gridView1.RowCount + 1).ToString();
            }
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            btn_Xoa.Enabled = false;
            if (gridView1.SelectedRowsCount > 0 && this.gridView1.FocusedRowHandle != GridControl.NewItemRowHandle)
            {
                btn_Xoa.Enabled = true;
            }

            if(this.gridView1.FocusedRowHandle == GridControl.NewItemRowHandle)
            {
                btn_Luu.Enabled = false;
            }
        }

        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.gridView1.FocusedRowHandle == GridControl.NewItemRowHandle
                && gridView1.FocusedColumn == gridView1.Columns["id_donvi"])
            {
                btn_Them.PerformClick();
            }
            if (e.KeyCode == Keys.Tab && this.gridView1.FocusedRowHandle == GridControl.NewItemRowHandle
                && gridView1.FocusedColumn == gridView1.Columns["id_donvi"])
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
                LoadDataSource();
                btn_Luu.Enabled = false;
            }
        }

        private void txt_Gia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '-')
            {
                e.Handled = true;
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.PrevFocusedRowHandle == GridControl.NewItemRowHandle)
                LoadDataSource();
        }
    }
}