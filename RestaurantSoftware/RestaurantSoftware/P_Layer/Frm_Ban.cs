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
using RestaurantSoftware.P_Layer;
using RestaurantSoftware.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace RestaurantSoftware.P_Layer
{
    public partial class Frm_Ban : DevExpress.XtraEditors.XtraForm
    {
        DataTable dt = new DataTable();
        private Ban_BLL _ban_Bll = new Ban_BLL();
        private List<int> _listUpdate = new List<int>();
        public Frm_Ban()
        {
            InitializeComponent();
            RestaurantDBDataContext db = new RestaurantDBDataContext();
            // This line of code is generated by Data Source Configuration Wizard
            lue_LoaiBan.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().LoaiBans;
            // This line of code is generated by Data Source Configuration Wizard
            lue_TrangThai.DataSource = db.TrangThais.Where(TrangThai => TrangThai.lienket == "ban");
        }

        private void Frm_Ban_Load(object sender, EventArgs e)
        {
            LoadDataSource();
        }

        private void LoadDataSource()
        {
            dt = Utils.Utils.ConvertToDataTable<Ban>(_ban_Bll.LayDanhSachBan());
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
                string TenBan = gridView1.GetFocusedRowCellValue(col_TenBan).ToString();
                if (_ban_Bll.KiemTraBanTonTai(TenBan) != -1)
                {
                    try
                    {
                        Ban ban = new Ban();
                        ban.tenban = gridView1.GetFocusedRowCellValue(col_TenBan).ToString();
                        ban.id_loaiban = int.Parse(gridView1.GetFocusedRowCellValue(col_LoaiBan).ToString());
                        ban.trangthai = gridView1.GetFocusedRowCellValue(col_TrangThai).ToString();
                        if (_ban_Bll.KiemTraBanTonTai(TenBan) == 1)
                        {
                            _ban_Bll.ThemBanMoi(ban);
                        }
                        else
                        {
                            ban.id_ban = _ban_Bll.LayIdBan(TenBan);
                            _ban_Bll.CapNhatBan(ban);
                        }
                        Notifications.Success("Thêm bàn mới thành công!");
                        LoadDataSource();
                        btn_LuuLai.Enabled = false;
                        _listUpdate.Clear();
                    }
                    catch (Exception)
                    {
                        Notifications.Error("Bạn chưa nhập đầy đủ thông tin bàn. Vui lòng nhập lại!");
                    }
                }
                else
                {
                    Notifications.Error("Tên bàn đã tồn tại. Vui lòng nhập tên bàn lại.");
                }
            }
            else
            {
                Notifications.Error("Bạn chưa nhập đầy đủ thông tin bàn. Vui lòng nhập lại!");
            }
        }

        private bool KiemTraHang()
        {
            if (gridView1.GetFocusedRowCellValue(col_TenBan) != null || gridView1.GetFocusedRowCellValue(col_TrangThai) != null)
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
                int _ID_Ban = int.Parse(gridView1.GetRowCellValue(gridView1.GetSelectedRows()[i], "id_ban").ToString());
                if (_ban_Bll.KiemTraThongTin(_ID_Ban))
                {
                    _ban_Bll.XoaTam(_ID_Ban);
                }
                else
                {
                    _ban_Bll.XoaBan(_ID_Ban);
                }
            }
            Notifications.Success("Xóa dữ liệu thành công!");
            LoadDataSource();
        }

        private void btn_LuuLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string error = "";
            bool isUpdate = false;
            if (_listUpdate.Count > 0)
                foreach (int id in _listUpdate)
                {
                    Ban ban = new Ban();
                    ban.id_ban = int.Parse(gridView1.GetRowCellValue(id, "id_ban").ToString());
                    ban.tenban = gridView1.GetRowCellValue(id, "tenban").ToString();
                    ban.id_loaiban = int.Parse(gridView1.GetRowCellValue(id, "id_loaiban").ToString());
                    ban.trangthai = gridView1.GetRowCellValue(id, "trangthai").ToString();

                    if (_ban_Bll.KiemTraBanTonTai(ban.tenban,ban.id_ban) == 1)
                    {
                        _ban_Bll.CapNhatBan(ban);
                        isUpdate = true;
                    }
                    else
                    {
                        if (error == "")
                        {
                            error = ban.tenban;
                        }
                        else
                        {
                            error += " | " + ban.tenban;
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
                    Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Các bàn chưa được cập nhật (" + error + "). Lỗi: Tên bàn đã tồn tại.");
                }
            }
            else
            {
                Notifications.Error("Có lỗi xảy ra khi cập nhật dữ liệu. Lỗi: Tên bàn đã tồn tại.");
            }
            btn_LuuLai.Enabled = false;
            LoadDataSource();
        }

        private void btn_LamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDataSource();
            this.gridView1.FocusedRowHandle = GridControl.NewItemRowHandle;
            gridView1.SelectRow(gridView1.FocusedRowHandle);
            gridView1.FocusedColumn = gridView1.VisibleColumns[1];
            gridView1.ShowEditor();
        }

        private void btn_In_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "File PDF|*.pdf|Excel|*.xls|Text rtf|*.rtf";
            saveFileDialog1.Title = "Xuất danh sách bàn";
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

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            btn_Xoa.Enabled = false;
            if (gridView1.SelectedRowsCount > 0 && this.gridView1.FocusedRowHandle != GridControl.NewItemRowHandle)
            {
                btn_Xoa.Enabled = true;
            }

            if (this.gridView1.FocusedRowHandle == GridControl.NewItemRowHandle)
            {
                btn_LuuLai.Enabled = false;
            }
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            if (this.gridView1.FocusedRowHandle != GridControl.NewItemRowHandle)
            {
                btn_LuuLai.Enabled = true;
                _listUpdate.Add(e.RowHandle);
            }
            else
            {
                btn_LuuLai.Enabled = false;
            }
        }

        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
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
                LoadDataSource();
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
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
          if (e.PrevFocusedRowHandle == GridControl.NewItemRowHandle)
              LoadDataSource();
        }
    }
}