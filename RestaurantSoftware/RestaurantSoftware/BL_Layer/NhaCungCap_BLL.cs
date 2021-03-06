﻿using RestaurantSoftware.DA_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSoftware.BL_Layer
{
    public class NhaCungCap_BLL
    {
        RestaurantDBDataContext dbContext = null;
        HangHoa_BLL _hanghoaBll;
        // hàm khởi tạo lớp NhaCungCap_BLL
        public NhaCungCap_BLL()
        {
            dbContext = new RestaurantDBDataContext();
            _hanghoaBll = new HangHoa_BLL();
        }
        // hàm lấy danh sách nhà cung cấp
        public IEnumerable<NhaCungCap> LayDanhSachNhaCungCap()
        {
            IEnumerable<NhaCungCap> query = from ncc in dbContext.NhaCungCaps where ncc.trangthai != "Xoa" select ncc;
            return query;
        }
        // hàm thêm nhà cung cấp
        public void ThemNhaCungCapMoi(NhaCungCap ncc)
        {
            dbContext.NhaCungCaps.InsertOnSubmit(ncc);
            dbContext.SubmitChanges();
        }
        // hàm cập nhật nhà cung cấp
        public void CapNhatNhaCungCap(NhaCungCap ncc)
        {
            NhaCungCap _nhacungcap = dbContext.NhaCungCaps.Single<NhaCungCap>(x => x.id_nhacungcap == ncc.id_nhacungcap);
            _nhacungcap.tennhacungcap = ncc.tennhacungcap;
            _nhacungcap.sdt = ncc.sdt;
            _nhacungcap.diachi = ncc.diachi;
            _nhacungcap.ghichu = ncc.ghichu;
            _nhacungcap.trangthai = ncc.trangthai;
            dbContext.SubmitChanges();
        }
        // lay ID nhà cung cấp
        public int LayIdNhaCungCap(string TenNhaCungCap)
        {
            IEnumerable<NhaCungCap> query = from ncc in dbContext.NhaCungCaps where ncc.tennhacungcap == TenNhaCungCap select ncc;
            return query.First().id_nhacungcap;
        }
        //Kiểm tra nhà cung cấp có tồn tại hay không
        public int KiemTraNhaCungCapTonTai(string _TenNhaCungCap, string _SDT ,int id = -1)
        {
            IEnumerable<NhaCungCap> query = from ncc in dbContext.NhaCungCaps
                                            where ncc.tennhacungcap == _TenNhaCungCap || ncc.sdt == _SDT
                                            select ncc;
            if (0 < query.Count() && query.Count() <= 2)
            {
                if (id != -1)
                {
                    query = query.Where(m => m.id_nhacungcap == id);
                    if (query.Count() == 1)
                    {
                        return 1;
                    }
                    if (query.Where(y => y.trangthai.Equals("Xoa")).Count() > 0)
                    {
                        return 0;
                    }
                }
                return -1;
            }
            return 1;
        }
        //Xóa nhà cung cấp
        public void XoaNhaCungCap(int _NhaCungCapID)
        {
            NhaCungCap _NhaCungCap = dbContext.NhaCungCaps.Single<NhaCungCap>(x => x.id_nhacungcap == _NhaCungCapID);
            dbContext.NhaCungCaps.DeleteOnSubmit(_NhaCungCap);
            dbContext.SubmitChanges();
        }
        // xoa tam
        public void XoaTam(int _NhaCungCapID)
        {
            NhaCungCap _NhaCungCap = dbContext.NhaCungCaps.Single<NhaCungCap>(x => x.id_nhacungcap == _NhaCungCapID);
            _NhaCungCap.trangthai = "Xoa";
            // update 
            dbContext.SubmitChanges();
        }
        // kiểm tra ncc khi lưu 
        public bool KiemTraNCC(NhaCungCap ncc)
        {
            if (ncc.tennhacungcap != null && ncc.tennhacungcap != "" 
                && ncc.sdt != null && ncc.sdt != "")
                return true;
            return false;
        }
        // hàm kiểm tra thông tin ncc khi xóa tạm
        public bool KiemTraThongTin(int _NhaCungCapID)
        {
            IEnumerable<HoaDonNhapHang> _KiemTraHoaDon = from hd in dbContext.HoaDonNhapHangs
                                                         where hd.id_nhacungcap == _NhaCungCapID
                                                         select hd;
            if (_KiemTraHoaDon.Count() > 0)
            {
                return true;
            }
            return false;
        }

    }
}
