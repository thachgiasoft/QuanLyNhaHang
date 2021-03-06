﻿using RestaurantSoftware.DA_Layer;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantSoftware.BL_Layer
{
    class LoaiMon_BLL
    {
        RestaurantDBDataContext dbContext = new RestaurantDBDataContext();
        Mon_BLL _monBll = new Mon_BLL();
        public IEnumerable<LoaiMon> LayDanhSachLoaiMon()
        {
            IEnumerable<LoaiMon> query = from lm in dbContext.LoaiMons where lm.trangthai != false select lm;
            return query;
        }

        public void ThemLoaiMon(LoaiMon loaimon)
        {
            dbContext.LoaiMons.InsertOnSubmit(loaimon);
            dbContext.SubmitChanges();
        }

        public int LayIdLoaiMon(string TenLoaiMon)
        {
            IEnumerable<LoaiMon> query = from lm in dbContext.LoaiMons where lm.tenloaimon == TenLoaiMon select lm;
            return query.First().id_loaimon;
        }

        public int KiemTraLoaiMonTonTai(string _LoaiMon, int id = -1)
        {
            IEnumerable<LoaiMon> query = from lm in dbContext.LoaiMons
                                         where lm.tenloaimon == _LoaiMon
                                     select lm;
            if (0 < query.Count() && query.Count() <= 2)
            {
                if (id != -1)
                {
                    query = query.Where(lm => lm.id_loaimon == id);
                    if (query.Count() == 1)
                    {
                        return 1;
                    }
                }

                if (query.Where(y => y.trangthai.Equals(false)).Count() > 0)
                {
                    return 0;
                }

                return -1;
            }
            return 1;
        }

        public void CapNhatLoaiMon(LoaiMon lm)
        {
            LoaiMon _loaimon = dbContext.LoaiMons.Single<LoaiMon>(x => x.id_loaimon == lm.id_loaimon);
            _loaimon.tenloaimon = lm.tenloaimon;
            // update 
            dbContext.SubmitChanges();
        }

        public bool KiemTraThongTin(int id_loaimon)
        {
            IEnumerable<int> mons = _monBll.getIDMon(id_loaimon);
            if(mons != null)
            {
                foreach(int i in mons)
                {
                    if (_monBll.KiemTraThongTin(i))
                        return true;
                }
                    
            }
            return false;
        }

        public bool KiemTraLoaiMon(LoaiMon loaimon)
        {
            if (loaimon.tenloaimon != null && loaimon.tenloaimon.ToString() != "")
                return true;
            return false;
        }

        public void XoaTam(int id_loaimon)
        {
            LoaiMon _loaimon = dbContext.LoaiMons.Single<LoaiMon>(x => x.id_loaimon == id_loaimon);
            _loaimon.trangthai = false;
            // update 
            dbContext.SubmitChanges();
        }

        public void XoaLoaiMon(int _LoaiMonID)
        {
            LoaiMon _loaimon = dbContext.LoaiMons.Single<LoaiMon>(x => x.id_loaimon == _LoaiMonID);
            dbContext.LoaiMons.DeleteOnSubmit(_loaimon);
            dbContext.SubmitChanges();
        }
    }
}
