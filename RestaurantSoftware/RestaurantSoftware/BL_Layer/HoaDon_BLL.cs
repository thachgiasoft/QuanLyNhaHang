﻿using DevExpress.XtraGrid;
using RestaurantSoftware.DA_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSoftware.BL_Layer
{
    class HoaDon_BLL
    {
        ChiTiet_ThanhToan ctth = new ChiTiet_ThanhToan();
        RestaurantDBDataContext dbContext = new RestaurantDBDataContext();
        
        public void LoadHoaDon(GridControl gr)
        {
            var query = from db in dbContext.HoaDonThanhToans
                        join kh in dbContext.KhachHangs on db.id_khachhang equals kh.id_khachhang
                        join bn in dbContext.Bans on db.id_ban equals bn.id_ban
                        where !(db.trangthai == "Chưa thanh toán")
                        select new
                        {
                            db.id_hoadon,
                            db.id_ban,
                            bn.tenban,
                            db.trangthai,
                            db.thoigian,
                            db.id_khachhang,
                            kh.tenkh,
                            kh.sdt,
                            db.vat,
                            db.khuyenmai,
                            datra = (decimal?)db.datra,
                            db.id_nhanvien,
                            tongtien = (int?)db.tongtien
                        };
            gr.DataSource = query;
        }
        public void CapNhatHoaDon(HoaDonThanhToan m)
        {
            HoaDonThanhToan hd = dbContext.HoaDonThanhToans.Single<HoaDonThanhToan>(x => x.id_hoadon == m.id_hoadon);
            hd.trangthai = m.trangthai;
            // update 
            dbContext.SubmitChanges();
        }
        public void CapNhatVatKhuyenMai(HoaDonThanhToan m)
        {
            try
            {
                HoaDonThanhToan hd = dbContext.HoaDonThanhToans.Single<HoaDonThanhToan>(x => x.id_hoadon == m.id_hoadon && x.trangthai == "Chưa thanh toán");
                hd.vat = m.vat;
                hd.khuyenmai = m.khuyenmai;
                // update 
                dbContext.SubmitChanges();
            }
            catch (Exception)
            {
                
               
            }
           
        }
    }
}
