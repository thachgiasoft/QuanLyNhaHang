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
using RestaurantSoftware.Utils;

namespace RestaurantSoftware.P_Layer
{
    public partial class Frm_ThanhToan : DevExpress.XtraEditors.XtraForm
    {
        DataTable dt = new DataTable();
        private HoaDon_BLL _hoadonbll = new HoaDon_BLL();
        private Ban_BLL _banBll = new Ban_BLL();
        private ThanhToan_BLL _thanhToanBll = new ThanhToan_BLL();
        private List<int> _listUpdate = new List<int>();
        public bool kt = false;
        int ID_NHANVIEN = 0;
        public Frm_ThanhToan(int idnv)
        {
            InitializeComponent();
            dt_NgayLap.Text = DateTime.Now.ToShortDateString();
            // This line of code is generated by Data Source Configuration Wizard
            cmb_NhanVien.Properties.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().NhanViens;
            ID_NHANVIEN = idnv;
            cmb_NhanVien.EditValue = ID_NHANVIEN;
            // This line of code is generated by Data Source Configuration Wizard
            txt_Tenkh.Properties.DataSource = new RestaurantSoftware.DA_Layer.RestaurantDBDataContext().KhachHangs;
        }


        private void Frm_ThanhToan_Load(object sender, EventArgs e)
        {
            LoadDataSource();
            btn_in.Enabled = false;
            btn_ThanhToan.Enabled = false;
            LoadDsKhachHang();
            txt_Tenkh.EditValue = 1;

        }
        private void LoadDataSource()
        {
            LoadDsBan();
            _thanhToanBll.LoadHoaDon(grv_HoaDon);
        }
        public void LoadDsKhachHang()
        {
            DataTable dt = Utils.Utils.ConvertToDataTable<KhachHang>(_thanhToanBll.LayDsKhachHang());
            txt_Tenkh.Properties.DataSource = dt;
            txt_Tenkh.Properties.DisplayMember = "tenkh";
            txt_Tenkh.Properties.ValueMember = "id_khachhang";
            
        }
        public void LoadDsBan()
        {
            lvDsBan.Clear();
            DataTable ban = Utils.Utils.ConvertToDataTable<ChiTiet_ThanhToan>(_thanhToanBll.LayDanhSachBan("Chưa thanh toán"));

            lvDsBan.LargeImageList = imageList1;

            foreach (DataRow dr in ban.Rows)
            {
                bool exsistGroup = false;
                ListViewItem lvItem = new ListViewItem();
                ListViewGroup lvGroup = new ListViewGroup();
                lvItem.Text = dr["tenban"].ToString();
                lvItem.ImageIndex = 0;
                lvItem.Name = dr["Idban"].ToString();
                lvGroup.Header = dr["Tenloaiban"].ToString();
                lvItem.Group = lvGroup;

                if (lvDsBan.Groups.Count != 0)
                {
                    foreach (ListViewGroup gr in lvDsBan.Groups)
                    {
                        if (gr.Header == lvGroup.Header)
                        {
                            exsistGroup = true;
                            lvItem.Group = gr;
                            break;
                        }
                    }
                    if (exsistGroup == false)
                    {
                        lvDsBan.Groups.Add(lvGroup);
                    }

                }
                else lvDsBan.Groups.Add(lvGroup);
                lvDsBan.Items.Add(lvItem);

            }
        }

        private void lvDsBan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDsBan.SelectedItems.Count > 0)
            {
                txt_SDT.Text = "";
                txt_KhachDua.Text = "";
                txt_TraLai.Text = "";
                cmb_NhanVien.EditValue = ID_NHANVIEN;
                txt_Ban.Text = lvDsBan.SelectedItems[0].Text;
                _thanhToanBll.loadid(int.Parse(lvDsBan.SelectedItems[0].Name), "Chưa thanh toán", txt_MaHoaDon,txt_DaTra);
                _thanhToanBll.LayDsThamSo(txt_VAT, txt_KhuyenMai);
                LoadChiTietHoaDon();
                //chuyenvetiente(txt_KhachDua);
                string a = txt_DaTra.Text;
                string[] split = a.Split('.');
                txt_DaTra.Text = split[0];
                chuyenvetiente(txt_DaTra);
                btn_ThanhToan.Enabled = true;
                btn_in.Enabled = true;
                checkBoxKhuyenmai.Enabled = true;
                txt_KhachDua.Enabled = true;
                kt = true;


            }
        }
        public void LoadChiTietHoaDon()
        {
            try
            {
                int a = int.Parse(txt_MaHoaDon.Text);
                _thanhToanBll.LoadChiTietHoaDon(a, grd_DanhSachMon);
                TongTien();
                TongHoaDon();
                chuyenvetiente(txt_TongTien);
                chuyenvetiente(txt_TongHoaDon);



            }
            catch (Exception)
            {
                Notifications.Answers("Lỗi load chi tiết");

            }

        }
        public void KiemtraTextBox()
        {
            if (txt_TongTien.Text == "")
            {
                txt_TongTien.Text = "0";
            }
            if (txt_KhuyenMai.Text == "")
            {
                txt_KhuyenMai.Text = "0";
            }
            if (txt_VAT.Text == "")
            {
                txt_VAT.Text = "0";
            }
            if (txt_TongHoaDon.Text == "")
            {
                txt_TongHoaDon.Text = "0";
            }

        }

        private void gv_HoaDon_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            txt_SDT.Text = "";
            txt_KhachDua.Text = "";
            txt_TraLai.Text = "";
            txt_Tenkh.EditValue = Convert.ToInt32(gv_HoaDon.GetFocusedRowCellDisplayText(col_khachHang));
            txt_MaHoaDon.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_MaHoaDon);
            txt_SDT.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_sdt);
            dt_NgayLap.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_ThoiGian);
            txt_Ban.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_TenBan);
            txt_DaTra.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_DaTra);
            chuyenvetiente(txt_DaTra);
            cmb_NhanVien.EditValue = Convert.ToInt32(gv_HoaDon.GetFocusedRowCellDisplayText(col_NhanVien));
            kt = false;
           
            if (gv_HoaDon.GetFocusedRowCellDisplayText(col_TrangThai) == "Đã thanh toán")
            {
                txt_VAT.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_Vat);
                txt_KhuyenMai.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_KhuyenMai);
                btn_ThanhToan.Enabled = false;
                checkBoxKhuyenmai.Enabled = false;
                txt_KhachDua.Enabled = false;

            }
            KiemtraTextBox();
            LoadChiTietHoaDon();
        }

        public void TinhToan()
        {
            try
            {
                TongTien();
                TongHoaDon();
                if (txt_KhachDua.Text != "")
                {
                    int a = chuyenvekieuint(txt_KhachDua);
                    int b = chuyenvekieuint(txt_TongHoaDon);
                    int c = chuyenvekieuint(txt_DaTra);
                    int d = a+c-b;
                    txt_TraLai.Text = d.ToString();

                }
                chuyenvetiente(txt_TraLai);
            }
            catch (Exception)
            {
                
                
            }
            
        }
        public void chuyenvetiente(TextEdit txt)
        {
            try
            {
                if (txt.Text == "0")
                {
                    int c = int.Parse(txt.Text);
                    txt.Text = c.ToString("0");
                }
                else
                {
                    int c = int.Parse(txt.Text);
                    txt.Text = c.ToString("#,###");
                }
            }
            catch (Exception)
            {


            }
            
            
        }
        public int chuyenvekieuint(TextEdit txt)
        {
            int temp = 0;
            try
            {
                if (txt.Text == "0")
                {
                    int c = int.Parse(txt.Text);
                    temp = int.Parse(c.ToString("0"));
                    return temp;
                }
                else
                {
                    temp = int.Parse((txt.Text).Replace(",", ""));
                    return temp;
                }
            }
            catch (Exception)
            {

                return temp;
            }
        }
        public void TongTien()
        {
            try
            {
                var a = gv_DanhSachMon.Columns["thanhtien"].SummaryItem.SummaryValue;
                txt_TongTien.EditValue = a;   

            }
            catch (Exception)
            {

                Notifications.Answers("Lỗi tổng tiền");
            }
           
        }

        private void checkBoxKhuyenmai_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxKhuyenmai.Checked == true)
            {
                txt_KhuyenMai.Properties.ReadOnly = false;

            }
            else
            {

                txt_KhuyenMai.Properties.ReadOnly = true;
                if (kt == false)
                {
                    if (gv_HoaDon.GetFocusedRowCellDisplayText(col_TrangThai) == "Đã thanh toán")
                    {
                        txt_KhuyenMai.Text = gv_HoaDon.GetFocusedRowCellDisplayText(col_KhuyenMai);

                    }
                    else
                        if (gv_HoaDon.GetFocusedRowCellDisplayText(col_TrangThai) == "Chưa thanh toán")
                        {
                            TextEdit a = new TextEdit();
                            _thanhToanBll.LayDsThamSo(a, txt_KhuyenMai);
                        }
                    TinhToan();
                }
                else
                {
                    TextEdit b = new TextEdit();
                    _thanhToanBll.LayDsThamSo(b, txt_KhuyenMai);
                    TinhToan();
                }
            }
         
            
        }

       
        public void TongHoaDon()
        {
            try
            {
                KiemtraTextBox();

                var a = gv_DanhSachMon.Columns["thanhtien"].SummaryItem.SummaryValue;
                int d = Convert.ToInt32(a);
                int b = (d * (int.Parse(txt_VAT.Text))) / 100;
                int c = (d* (int.Parse(txt_KhuyenMai.Text))) / 100;
                txt_TongHoaDon.Text = (d + b - c).ToString();
                chuyenvetiente(txt_TongHoaDon);
                
                
            }
            catch (Exception)
            {

            }

        }

        private void txt_KhuyenMai_EditValueChanged(object sender, EventArgs e)
        {
            if(checkBoxKhuyenmai.Checked ==true)
            {
                //chuyenvekieuint(txt_KhachDua);
                TinhToan();
                //chuyenvetiente(txt_KhachDua);

            }
           

        }

        private void txt_VAT_EditValueChanged(object sender, EventArgs e)
        {
           

        }

        private void txt_KhachDua_EditValueChanged(object sender, EventArgs e)
        {
            TinhToan();
        }

        private void txt_KhachDua_Leave(object sender, EventArgs e)
        {
            chuyenvetiente(txt_KhachDua);
          
            chuyenvetiente(txt_TongHoaDon);
        }

        private void txt_KhachDua_Click(object sender, EventArgs e)
        {
            txt_KhachDua.Text = "";
            txt_TraLai.Text = "";
        }

        private void btn_ThanhToan_Click(object sender, EventArgs e)
        {
            if (txt_KhachDua.Text!="")
            {
                string a = (txt_TongHoaDon.Text).Replace(",", "");
                string b = (txt_KhachDua.Text).Replace(",", "");
                if (int.Parse(a) < int.Parse(b))
                {
                    try
                    {

                        HoaDonThanhToan hd = new HoaDonThanhToan();
                        hd.id_hoadon = int.Parse(txt_MaHoaDon.Text);
                        hd.khuyenmai = int.Parse(txt_KhuyenMai.Text);
                        hd.vat = int.Parse(txt_VAT.Text);
                        hd.id_khachhang = (int)txt_Tenkh.EditValue;
                        hd.tongtien = int.Parse(a);
                        hd.trangthai = "Đã thanh toán";
                        hd.datra = int.Parse(a);
                        Ban bn = new Ban();
                        if (kt == true)
                        {
                            bn.id_ban = int.Parse(lvDsBan.SelectedItems[0].Name);

                        }
                        else
                        {
                            bn.id_ban = Convert.ToInt32(gv_HoaDon.GetFocusedRowCellDisplayText(col_MaBan));
                        }
                        bn.trangthai = "Trống";
                        _thanhToanBll.ThanhToan(hd);
                        _banBll.CapNhatBanThanhToan(bn);
                        Notifications.Answers("Thanh toán thành công!");
                        LoadDataSource();
                    }
                    catch (Exception)
                    {

                        Notifications.Answers("Hóa đơn đã thanh toán rồi!");
                    }

                }
                else
                    Notifications.Answers("Khách hàng chưa thanh toán đủ tiền!");

            }
            
                
        }

        private void btn_in_Click(object sender, EventArgs e)
        {
            HoaDonThanhToan m = new HoaDonThanhToan();
            m.id_hoadon = int.Parse(txt_MaHoaDon.Text);
            m.vat = int.Parse(txt_VAT.Text);
            m.khuyenmai = int.Parse(txt_KhuyenMai.Text);
            _hoadonbll.CapNhatVatKhuyenMai(m);
            Frm_PrintfHoaDon hd = new Frm_PrintfHoaDon(int.Parse(txt_MaHoaDon.Text));
            hd.Show();
        }

        private void txt_Tenkh_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.LookUpEdit editor = (sender as DevExpress.XtraEditors.LookUpEdit);
            DataRowView row = editor.Properties.GetDataSourceRowByKeyValue(editor.EditValue) as DataRowView;
            txt_SDT.Text = row["sdt"].ToString();
        }


    }
}