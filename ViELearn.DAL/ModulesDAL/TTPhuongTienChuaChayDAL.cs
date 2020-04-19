using System;
using System.Collections.Generic;
using System.Linq;
using ViELearn.DAL.BaseDAL;
using ViELearn.Models.ProjectModels;
using ViELearn.Models.ReportModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class TTPhuongTienChuaChayDAL : DALBaseClass<TTPhuongTienChuaChay>
    {
        public TTPhuongTienChuaChayDAL()
        {
            ConnectionStringName = "DefaultConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public TTPhuongTienChuaChayDAL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public List<TTPhuongTienChuaChay> GetLstByOrderView()
        {
            try
            {
                var sql = string.Format(@"SELECT *
                                        FROM TTPhuongTienChuaChay
                                        Order By Id DESC");

                return _db.Fetch<TTPhuongTienChuaChay>(sql);
            }
            catch (Exception ex)
            {
                return new List<TTPhuongTienChuaChay>();
            }
        }

        public List<TTPhuongTienChuaChay> GetListPTCCReport01_Doi(int idDoi)
        {
            try
            {
                var sql = string.Format(@"SELECT * FROM dbo.TTPhuongTienChuaChay tttc WHERE tttc.DonVi = {0}", idDoi);
                return _db.Fetch<TTPhuongTienChuaChay>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TTPhuongTienChuaChay> GetListPTCCReport01_Phong(int idPhong)
        {
            try
            {
                var sql = string.Format(@"SELECT * FROM dbo.TTPhuongTienChuaChay tttc WHERE tttc.DonVi = {0}", idPhong);
                return _db.Fetch<TTPhuongTienChuaChay>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TTPhuongTienChuaChay> GetListPTCCReport01_So(int idSo)
        {
            try
            {
                var sql = string.Format(@"SELECT * FROM dbo.TTPhuongTienChuaChay tttc  WHERE tttc.DonVi = {0}", idSo);
                return _db.Fetch<TTPhuongTienChuaChay>(sql).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_02 GetListPTCCReport02_Doi(int idDoi)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS XCC_IVECO,
	                                               COUNT(lx2.id) AS XCC_CAMIVA,
	                                               COUNT(lx3.id) AS XCC_HINO,
	                                               COUNT(lx4.Id) AS XCC_MERCEDES,
	                                               COUNT(lx5.id) AS XCC_ISUZU,
	                                               COUNT(lx6.id) AS XCC_NISSAN,
	                                               COUNT(lx7.Id) AS XCC_MAN,
	                                               COUNT(lx8.id) AS XCC_ZIN130,
	                                               COUNT(lx9.id) AS XCC_Khac,
	                                               COUNT(lx11.Id) AS XT_52m,
	                                               COUNT(lx12.id) AS XT_32m,
	                                               COUNT(lx13.id) AS XT_Khac,
	                                               COUNT(lx14.Id) AS XCN_9m3,
	                                               COUNT(lx15.id) AS XCN_6m3,
                                                   COUNT(lx16.id) AS XCN_Khac,
                                                   COUNT(lx17.id) AS XeTramBom,
                                                   COUNT(lx18.id) AS XeCBL,
                                                   COUNT(lx19.id) AS XeChiHuy,
                                                   COUNT(lx20.id) AS XeTaiBanTai,
                                                   COUNT(lx21.id) AS XeKhac,
                                                   COUNT(lx22.id) AS MayBom,
                                                   COUNT(lx23.id) AS XeDangSuaChua
                                            FROM dbo.TTPhuongTienChuaChay tttc 
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4151 AND lx1.DTKT_LoaiXeNen = 4169 AND lx1.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4151 AND lx2.DTKT_LoaiXeNen = 4170 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4151 AND lx3.DTKT_LoaiXeNen = 4171 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4151 AND lx4.DTKT_LoaiXeNen = 4195 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4151 AND lx5.DTKT_LoaiXeNen = 4196 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4151 AND lx6.DTKT_LoaiXeNen = 4197 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4151 AND lx7.DTKT_LoaiXeNen = 4198 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4151 AND lx8.DTKT_LoaiXeNen = 4199 AND lx8.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4151 AND lx9.DTKT_LoaiXeNen NOT IN (4169,4170,4171,4195,4196,4197,4198,4199) AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4152 AND lx11.PhanLoaiChiTiet = 4186 AND lx11.DTKT_DoCaoThang = 52 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4152 AND lx12.PhanLoaiChiTiet = 4186 AND lx12.DTKT_DoCaoThang = 32 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx13 ON lx13.LoaiPhuongTien = 4152 AND lx13.PhanLoaiChiTiet = 4186 AND lx13.DTKT_DoCaoThang NOT IN (52,32) AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx14 ON lx14.LoaiPhuongTien = 4152 AND lx14.PhanLoaiChiTiet = 4184 AND lx14.DTKT_DungTichKetNuoc = 9000 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx15 ON lx15.LoaiPhuongTien = 4152 AND lx15.PhanLoaiChiTiet = 4184 AND lx15.DTKT_DungTichKetNuoc = 6000 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx16 ON lx16.LoaiPhuongTien = 4152 AND lx16.PhanLoaiChiTiet = 4184 AND lx16.DTKT_DungTichKetNuoc NOT IN (9000,6000) AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx17 ON lx17.LoaiPhuongTien = 4152 AND lx17.PhanLoaiChiTiet = 4185 AND lx17.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx18 ON lx18.LoaiPhuongTien = 4152 AND lx18.PhanLoaiChiTiet = 4189 AND lx18.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx19 ON lx19.LoaiPhuongTien = 4152 AND lx19.PhanLoaiChiTiet = 4182 AND lx19.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx20 ON lx20.LoaiPhuongTien = 4152 AND lx20.PhanLoaiChiTiet IN (4192,4187) AND lx20.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx21 ON lx21.LoaiPhuongTien = 4152 AND lx21.PhanLoaiChiTiet = 4194 AND lx21.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx22 ON lx22.LoaiPhuongTien = 4155 AND lx22.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx23 ON lx23.TinhTrangSuDung = 4163 AND lx23.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Doi = ttdv.id
                                            WHERE tttc.Doi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idDoi);
                return _db.Fetch<Report004_02>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_02 GetListPTCCReport02_Phong(int idPhong)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS XCC_IVECO,
	                                               COUNT(lx2.id) AS XCC_CAMIVA,
	                                               COUNT(lx3.id) AS XCC_HINO,
	                                               COUNT(lx4.Id) AS XCC_MERCEDES,
	                                               COUNT(lx5.id) AS XCC_ISUZU,
	                                               COUNT(lx6.id) AS XCC_NISSAN,
	                                               COUNT(lx7.Id) AS XCC_MAN,
	                                               COUNT(lx8.id) AS XCC_ZIN130,
	                                               COUNT(lx9.id) AS XCC_Khac,
	                                               COUNT(lx11.Id) AS XT_52m,
	                                               COUNT(lx12.id) AS XT_32m,
	                                               COUNT(lx13.id) AS XT_Khac,
	                                               COUNT(lx14.Id) AS XCN_9m3,
	                                               COUNT(lx15.id) AS XCN_6m3,
                                                   COUNT(lx16.id) AS XCN_Khac,
                                                   COUNT(lx17.id) AS XeTramBom,
                                                   COUNT(lx18.id) AS XeCBL,
                                                   COUNT(lx19.id) AS XeChiHuy,
                                                   COUNT(lx20.id) AS XeTaiBanTai,
                                                   COUNT(lx21.id) AS XeKhac,
                                                   COUNT(lx22.id) AS MayBom,
                                                   COUNT(lx23.id) AS XeDangSuaChua
                                            FROM dbo.TTPhuongTienChuaChay tttc 
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4151 AND lx1.DTKT_LoaiXeNen = 4169 AND lx1.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4151 AND lx2.DTKT_LoaiXeNen = 4170 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4151 AND lx3.DTKT_LoaiXeNen = 4171 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4151 AND lx4.DTKT_LoaiXeNen = 4195 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4151 AND lx5.DTKT_LoaiXeNen = 4196 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4151 AND lx6.DTKT_LoaiXeNen = 4197 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4151 AND lx7.DTKT_LoaiXeNen = 4198 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4151 AND lx8.DTKT_LoaiXeNen = 4199 AND lx8.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4151 AND lx9.DTKT_LoaiXeNen NOT IN (4169,4170,4171,4195,4196,4197,4198,4199) AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4152 AND lx11.PhanLoaiChiTiet = 4186 AND lx11.DTKT_DoCaoThang = 52 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4152 AND lx12.PhanLoaiChiTiet = 4186 AND lx12.DTKT_DoCaoThang = 32 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx13 ON lx13.LoaiPhuongTien = 4152 AND lx13.PhanLoaiChiTiet = 4186 AND lx13.DTKT_DoCaoThang NOT IN (52,32) AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx14 ON lx14.LoaiPhuongTien = 4152 AND lx14.PhanLoaiChiTiet = 4184 AND lx14.DTKT_DungTichKetNuoc = 9000 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx15 ON lx15.LoaiPhuongTien = 4152 AND lx15.PhanLoaiChiTiet = 4184 AND lx15.DTKT_DungTichKetNuoc = 6000 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx16 ON lx16.LoaiPhuongTien = 4152 AND lx16.PhanLoaiChiTiet = 4184 AND lx16.DTKT_DungTichKetNuoc NOT IN (9000,6000) AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx17 ON lx17.LoaiPhuongTien = 4152 AND lx17.PhanLoaiChiTiet = 4185 AND lx17.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx18 ON lx18.LoaiPhuongTien = 4152 AND lx18.PhanLoaiChiTiet = 4189 AND lx18.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx19 ON lx19.LoaiPhuongTien = 4152 AND lx19.PhanLoaiChiTiet = 4182 AND lx19.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx20 ON lx20.LoaiPhuongTien = 4152 AND lx20.PhanLoaiChiTiet IN (4192,4187) AND lx20.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx21 ON lx21.LoaiPhuongTien = 4152 AND lx21.PhanLoaiChiTiet = 4194 AND lx21.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx22 ON lx22.LoaiPhuongTien = 4155 AND lx22.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx23 ON lx23.TinhTrangSuDung = 4163 AND lx23.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Phong = ttdv.id
                                            WHERE tttc.Phong = {0}
                                            GROUP BY ttdv.TenDanhMuc", idPhong);
                return _db.Fetch<Report004_02>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_02 GetListPTCCReport02_So(int idSo)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS XCC_IVECO,
	                                               COUNT(lx2.id) AS XCC_CAMIVA,
	                                               COUNT(lx3.id) AS XCC_HINO,
	                                               COUNT(lx4.Id) AS XCC_MERCEDES,
	                                               COUNT(lx5.id) AS XCC_ISUZU,
	                                               COUNT(lx6.id) AS XCC_NISSAN,
	                                               COUNT(lx7.Id) AS XCC_MAN,
	                                               COUNT(lx8.id) AS XCC_ZIN130,
	                                               COUNT(lx9.id) AS XCC_Khac,
	                                               COUNT(lx11.Id) AS XT_52m,
	                                               COUNT(lx12.id) AS XT_32m,
	                                               COUNT(lx13.id) AS XT_Khac,
	                                               COUNT(lx14.Id) AS XCN_9m3,
	                                               COUNT(lx15.id) AS XCN_6m3,
                                                   COUNT(lx16.id) AS XCN_Khac,
                                                   COUNT(lx17.id) AS XeTramBom,
                                                   COUNT(lx18.id) AS XeCBL,
                                                   COUNT(lx19.id) AS XeChiHuy,
                                                   COUNT(lx20.id) AS XeTaiBanTai,
                                                   COUNT(lx21.id) AS XeKhac,
                                                   COUNT(lx22.id) AS MayBom,
                                                   COUNT(lx23.id) AS XeDangSuaChua
                                            FROM dbo.TTPhuongTienChuaChay tttc 
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4151 AND lx1.DTKT_LoaiXeNen = 4169 AND lx1.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4151 AND lx2.DTKT_LoaiXeNen = 4170 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4151 AND lx3.DTKT_LoaiXeNen = 4171 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4151 AND lx4.DTKT_LoaiXeNen = 4195 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4151 AND lx5.DTKT_LoaiXeNen = 4196 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4151 AND lx6.DTKT_LoaiXeNen = 4197 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4151 AND lx7.DTKT_LoaiXeNen = 4198 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4151 AND lx8.DTKT_LoaiXeNen = 4199 AND lx8.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4151 AND lx9.DTKT_LoaiXeNen NOT IN (4169,4170,4171,4195,4196,4197,4198,4199) AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4152 AND lx11.PhanLoaiChiTiet = 4186 AND lx11.DTKT_DoCaoThang = 52 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4152 AND lx12.PhanLoaiChiTiet = 4186 AND lx12.DTKT_DoCaoThang = 32 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx13 ON lx13.LoaiPhuongTien = 4152 AND lx13.PhanLoaiChiTiet = 4186 AND lx13.DTKT_DoCaoThang NOT IN (52,32) AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx14 ON lx14.LoaiPhuongTien = 4152 AND lx14.PhanLoaiChiTiet = 4184 AND lx14.DTKT_DungTichKetNuoc = 9000 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx15 ON lx15.LoaiPhuongTien = 4152 AND lx15.PhanLoaiChiTiet = 4184 AND lx15.DTKT_DungTichKetNuoc = 6000 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx16 ON lx16.LoaiPhuongTien = 4152 AND lx16.PhanLoaiChiTiet = 4184 AND lx16.DTKT_DungTichKetNuoc NOT IN (9000,6000) AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx17 ON lx17.LoaiPhuongTien = 4152 AND lx17.PhanLoaiChiTiet = 4185 AND lx17.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx18 ON lx18.LoaiPhuongTien = 4152 AND lx18.PhanLoaiChiTiet = 4189 AND lx18.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx19 ON lx19.LoaiPhuongTien = 4152 AND lx19.PhanLoaiChiTiet = 4182 AND lx19.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx20 ON lx20.LoaiPhuongTien = 4152 AND lx20.PhanLoaiChiTiet IN (4192,4187) AND lx20.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx21 ON lx21.LoaiPhuongTien = 4152 AND lx21.PhanLoaiChiTiet = 4194 AND lx21.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx22 ON lx22.LoaiPhuongTien = 4155 AND lx22.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx23 ON lx23.TinhTrangSuDung = 4163 AND lx23.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.DonVi = ttdv.id
                                            WHERE tttc.DonVi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idSo);
                return _db.Fetch<Report004_02>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_03 GetListPTCCReport03_Doi(int idDoi)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS SoLuong,
	                                               COUNT(lx2.id) AS CL_HoatDong,
	                                               COUNT(lx3.id) AS CL_HuHong,
	                                               COUNT(lx4.Id) AS LX_IVECO,
	                                               COUNT(lx5.id) AS LX_CAMIVA,
	                                               COUNT(lx6.id) AS LX_HINO,
	                                               COUNT(lx7.Id) AS LX_MERCEDES,
	                                               COUNT(lx8.id) AS LX_ISUZU,
	                                               COUNT(lx9.id) AS LX_NISSAN,
	                                               COUNT(lx10.Id) AS LX_MAN,
	                                               COUNT(lx11.id) AS LX_ZIN130,
	                                               COUNT(lx12.id) AS LX_Khac
                                            FROM dbo.TTPhuongTienChuaChay tttc 
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4151  AND lx1.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4151 AND lx2.TinhTrangSuDung = 4161 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4151 AND lx3.TinhTrangSuDung = 4163 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4151 AND lx4.DTKT_LoaiXeNen = 4169 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4151 AND lx5.DTKT_LoaiXeNen = 4170 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4151 AND lx6.DTKT_LoaiXeNen = 4171 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4151 AND lx7.DTKT_LoaiXeNen = 4195 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4151 AND lx8.DTKT_LoaiXeNen = 4196 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4151 AND lx9.DTKT_LoaiXeNen = 4197 AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 4151 AND lx10.DTKT_LoaiXeNen = 4198 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4151 AND lx11.DTKT_LoaiXeNen IN (4192,4187) AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4151 AND lx12.DTKT_LoaiXeNen NOT IN (4169,4170,4171,4195,4196,4197,4198,4192,4187) AND lx12.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Doi = ttdv.id
                                            WHERE tttc.Doi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idDoi);

                return _db.Fetch<Report004_03>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_03 GetListPTCCReport03_Phong(int idPhong)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS SoLuong,
	                                               COUNT(lx2.id) AS CL_HoatDong,
	                                               COUNT(lx3.id) AS CL_HuHong,
	                                               COUNT(lx4.Id) AS LX_IVECO,
	                                               COUNT(lx5.id) AS LX_CAMIVA,
	                                               COUNT(lx6.id) AS LX_HINO,
	                                               COUNT(lx7.Id) AS LX_MERCEDES,
	                                               COUNT(lx8.id) AS LX_ISUZU,
	                                               COUNT(lx9.id) AS LX_NISSAN,
	                                               COUNT(lx10.Id) AS LX_MAN,
	                                               COUNT(lx11.id) AS LX_ZIN130,
	                                               COUNT(lx12.id) AS LX_Khac
                                            FROM dbo.TTPhuongTienChuaChay tttc 
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4151  AND lx1.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4151 AND lx2.TinhTrangSuDung = 4161 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4151 AND lx3.TinhTrangSuDung = 4163 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4151 AND lx4.DTKT_LoaiXeNen = 4169 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4151 AND lx5.DTKT_LoaiXeNen = 4170 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4151 AND lx6.DTKT_LoaiXeNen = 4171 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4151 AND lx7.DTKT_LoaiXeNen = 4195 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4151 AND lx8.DTKT_LoaiXeNen = 4196 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4151 AND lx9.DTKT_LoaiXeNen = 4197 AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 4151 AND lx10.DTKT_LoaiXeNen = 4198 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4151 AND lx11.DTKT_LoaiXeNen IN (4192,4187) AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4151 AND lx12.DTKT_LoaiXeNen NOT IN (4169,4170,4171,4195,4196,4197,4198,4192,4187) AND lx12.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Phong = ttdv.id
                                            WHERE tttc.Phong = {0}
                                            GROUP BY ttdv.TenDanhMuc", idPhong);

                return _db.Fetch<Report004_03>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_03 GetListPTCCReport03_So(int idSo)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS SoLuong,
	                                               COUNT(lx2.id) AS CL_HoatDong,
	                                               COUNT(lx3.id) AS CL_HuHong,
	                                               COUNT(lx4.Id) AS LX_IVECO,
	                                               COUNT(lx5.id) AS LX_CAMIVA,
	                                               COUNT(lx6.id) AS LX_HINO,
	                                               COUNT(lx7.Id) AS LX_MERCEDES,
	                                               COUNT(lx8.id) AS LX_ISUZU,
	                                               COUNT(lx9.id) AS LX_NISSAN,
	                                               COUNT(lx10.Id) AS LX_MAN,
	                                               COUNT(lx11.id) AS LX_ZIN130,
	                                               COUNT(lx12.id) AS LX_Khac
                                            FROM dbo.TTPhuongTienChuaChay tttc 
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4151  AND lx1.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4151 AND lx2.TinhTrangSuDung = 4161 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4151 AND lx3.TinhTrangSuDung = 4163 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4151 AND lx4.DTKT_LoaiXeNen = 4169 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4151 AND lx5.DTKT_LoaiXeNen = 4170 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4151 AND lx6.DTKT_LoaiXeNen = 4171 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4151 AND lx7.DTKT_LoaiXeNen = 4195 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4151 AND lx8.DTKT_LoaiXeNen = 4196 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4151 AND lx9.DTKT_LoaiXeNen = 4197 AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 4151 AND lx10.DTKT_LoaiXeNen = 4198 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4151 AND lx11.DTKT_LoaiXeNen IN (4192,4187) AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4151 AND lx12.DTKT_LoaiXeNen NOT IN (4169,4170,4171,4195,4196,4197,4198,4192,4187) AND lx12.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Donvi = ttdv.id
                                            WHERE tttc.Donvi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idSo);

                return _db.Fetch<Report004_03>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_04 GetListPTCCReport04_Doi(int idDoi)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS SoLuong,
	                                               COUNT(lx2.id) AS CL_HoatDong,
	                                               COUNT(lx3.id) AS CL_HuHong,
	                                               COUNT(lx4.Id) AS LX_Thang,
	                                               COUNT(lx5.id) AS LX_ChoNuoc,
	                                               COUNT(lx6.id) AS LX_CuuHo,
	                                               COUNT(lx7.Id) AS LX_TramBom,
	                                               COUNT(lx8.id) AS LX_ChiHuy,
	                                               COUNT(lx9.id) AS LX_CuuThuong,
	                                               COUNT(lx10.Id) AS LX_XeCon,
	                                               COUNT(lx11.id) AS LX_XeTaiBanTai,
	                                               COUNT(lx12.id) AS LX_LoaiKhac
                                            FROM dbo.TTPhuongTienChuaChay tttc
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4152  AND lx1.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4152 AND lx2.TinhTrangSuDung = 4161 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4152 AND lx3.TinhTrangSuDung = 4163 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4152 AND lx4.PhanLoaiChiTiet = 4186 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4152 AND lx5.PhanLoaiChiTiet = 4184 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4152 AND lx6.PhanLoaiChiTiet = 4190 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4152 AND lx7.PhanLoaiChiTiet = 4185 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4152 AND lx8.PhanLoaiChiTiet = 4182 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4152 AND lx9.PhanLoaiChiTiet = 4183 AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 4152 AND lx10.PhanLoaiChiTiet = 4193 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4152 AND lx11.PhanLoaiChiTiet = 4199 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4152 AND lx12.PhanLoaiChiTiet NOT IN (4169,4170,4171,4195,4196,4197,4198,4199) AND lx12.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Doi = ttdv.id
                                            WHERE tttc.Doi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idDoi);

                return _db.Fetch<Report004_04>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_04 GetListPTCCReport04_Phong(int idPhong)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS SoLuong,
	                                               COUNT(lx2.id) AS CL_HoatDong,
	                                               COUNT(lx3.id) AS CL_HuHong,
	                                               COUNT(lx4.Id) AS LX_Thang,
	                                               COUNT(lx5.id) AS LX_ChoNuoc,
	                                               COUNT(lx6.id) AS LX_CuuHo,
	                                               COUNT(lx7.Id) AS LX_TramBom,
	                                               COUNT(lx8.id) AS LX_ChiHuy,
	                                               COUNT(lx9.id) AS LX_CuuThuong,
	                                               COUNT(lx10.Id) AS LX_XeCon,
	                                               COUNT(lx11.id) AS LX_XeTaiBanTai,
	                                               COUNT(lx12.id) AS LX_LoaiKhac
                                            FROM dbo.TTPhuongTienChuaChay tttc
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4152  AND lx1.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4152 AND lx2.TinhTrangSuDung = 4161 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4152 AND lx3.TinhTrangSuDung = 4163 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4152 AND lx4.PhanLoaiChiTiet = 4186 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4152 AND lx5.PhanLoaiChiTiet = 4184 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4152 AND lx6.PhanLoaiChiTiet = 4190 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4152 AND lx7.PhanLoaiChiTiet = 4185 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4152 AND lx8.PhanLoaiChiTiet = 4182 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4152 AND lx9.PhanLoaiChiTiet = 4183 AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 4152 AND lx10.PhanLoaiChiTiet = 4193 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4152 AND lx11.PhanLoaiChiTiet = 4199 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4152 AND lx12.PhanLoaiChiTiet NOT IN (4169,4170,4171,4195,4196,4197,4198,4199) AND lx12.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Phong = ttdv.id
                                            WHERE tttc.Phong = {0}
                                            GROUP BY ttdv.TenDanhMuc", idPhong);

                return _db.Fetch<Report004_04>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_04 GetListPTCCReport04_So(int idSo)
        {
            try
            {
                var sql = string.Format(@"SELECT   ttdv.TenDanhMuc as DonVi,
                                                   COUNT(lx1.Id) AS SoLuong,
	                                               COUNT(lx2.id) AS CL_HoatDong,
	                                               COUNT(lx3.id) AS CL_HuHong,
	                                               COUNT(lx4.Id) AS LX_Thang,
	                                               COUNT(lx5.id) AS LX_ChoNuoc,
	                                               COUNT(lx6.id) AS LX_CuuHo,
	                                               COUNT(lx7.Id) AS LX_TramBom,
	                                               COUNT(lx8.id) AS LX_ChiHuy,
	                                               COUNT(lx9.id) AS LX_CuuThuong,
	                                               COUNT(lx10.Id) AS LX_XeCon,
	                                               COUNT(lx11.id) AS LX_XeTaiBanTai,
	                                               COUNT(lx12.id) AS LX_LoaiKhac
                                            FROM dbo.TTPhuongTienChuaChay tttc
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 4152  AND lx1.id = tttc.id                                            
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 4152 AND lx2.TinhTrangSuDung = 4161 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 4152 AND lx3.TinhTrangSuDung = 4163 AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 4152 AND lx4.PhanLoaiChiTiet = 4186 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 4152 AND lx5.PhanLoaiChiTiet = 4184 AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 4152 AND lx6.PhanLoaiChiTiet = 4190 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 4152 AND lx7.PhanLoaiChiTiet = 4185 AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 4152 AND lx8.PhanLoaiChiTiet = 4182 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 4152 AND lx9.PhanLoaiChiTiet = 4183 AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 4152 AND lx10.PhanLoaiChiTiet = 4193 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 4152 AND lx11.PhanLoaiChiTiet = 4199 AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 4152 AND lx12.PhanLoaiChiTiet NOT IN (4169,4170,4171,4195,4196,4197,4198,4199) AND lx12.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.DonVi = ttdv.id
                                            WHERE tttc.DonVi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idSo);

                return _db.Fetch<Report004_04>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_05 GetListPTCCReport05_Doi(int idDoi)
        {
            try
            {
                var sql = string.Format(@"SELECT ttdv.TenDanhMuc as DonVi,   
                                                COUNT(lx1.Id) AS MBCC_SoLuong,
	                                            COUNT(lx2.id) AS MBCC_CL_HoatDong,
	                                            COUNT(lx4.id) AS MBCC_CL_HuHong,
	                                            COUNT(lx5.Id) AS MBCC_LPT_TohatsuV46,
	                                            COUNT(lx6.id) AS MBCC_LPT_TohatsuV75,
	                                            COUNT(lx7.id) AS MBCC_LPT_Bj25,
	                                            COUNT(lx8.Id) AS MBCC_LPT_Rong,
	                                            COUNT(lx9.id) AS MBCC_LPT_LoaiKhac,
	                                            COUNT(lx10.id) AS TCCC_SoLuong,
	                                            COUNT(lx11.Id) AS TCCC_CL_HoatDong,
	                                            COUNT(lx12.id) AS TCCC_CL_HuHong,
	                                            COUNT(lx13.id) AS TCCC_LPT_TauCCBien,
	                                            COUNT(lx14.Id) AS TCCC_LPT_TauCCSong,
	                                            COUNT(lx15.id) AS TCCC_LPT_CanoCC,
                                                COUNT(lx16.id) AS TCCC_LPT_XuongCH
                                            FROM dbo.TTPhuongTienChuaChay tttc
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 151  AND lx1.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 152 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 153  AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 154 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 155  AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 156 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 157  AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 158 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 159  AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 160 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 161  AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 162 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx13 ON lx13.LoaiPhuongTien = 163  AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx14 ON lx14.LoaiPhuongTien = 164 AND lx14.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx15 ON lx15.LoaiPhuongTien = 165  AND lx15.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx16 ON lx16.LoaiPhuongTien = 166 AND lx16.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Doi = ttdv.id
                                            WHERE tttc.Doi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idDoi);

                return _db.Fetch<Report004_05>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_05 GetListPTCCReport05_Phong(int idPhong)
        {
            try
            {
                var sql = string.Format(@"SELECT ttdv.TenDanhMuc as DonVi,   
                                                COUNT(lx1.Id) AS MBCC_SoLuong,
	                                            COUNT(lx2.id) AS MBCC_CL_HoatDong,
	                                            COUNT(lx4.id) AS MBCC_CL_HuHong,
	                                            COUNT(lx5.Id) AS MBCC_LPT_TohatsuV46,
	                                            COUNT(lx6.id) AS MBCC_LPT_TohatsuV75,
	                                            COUNT(lx7.id) AS MBCC_LPT_Bj25,
	                                            COUNT(lx8.Id) AS MBCC_LPT_Rong,
	                                            COUNT(lx9.id) AS MBCC_LPT_LoaiKhac,
	                                            COUNT(lx10.id) AS TCCC_SoLuong,
	                                            COUNT(lx11.Id) AS TCCC_CL_HoatDong,
	                                            COUNT(lx12.id) AS TCCC_CL_HuHong,
	                                            COUNT(lx13.id) AS TCCC_LPT_TauCCBien,
	                                            COUNT(lx14.Id) AS TCCC_LPT_TauCCSong,
	                                            COUNT(lx15.id) AS TCCC_LPT_CanoCC,
                                                COUNT(lx16.id) AS TCCC_LPT_XuongCH
                                            FROM dbo.TTPhuongTienChuaChay tttc
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 151  AND lx1.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 152 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 153  AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 154 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 155  AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 156 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 157  AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 158 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 159  AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 160 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 161  AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 162 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx13 ON lx13.LoaiPhuongTien = 163  AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx14 ON lx14.LoaiPhuongTien = 164 AND lx14.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx15 ON lx15.LoaiPhuongTien = 165  AND lx15.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx16 ON lx16.LoaiPhuongTien = 166 AND lx16.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.Phong = ttdv.id
                                            WHERE tttc.Phong = {0}
                                            GROUP BY ttdv.TenDanhMuc", idPhong);

                return _db.Fetch<Report004_05>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Report004_05 GetListPTCCReport05_So(int idSo)
        {
            try
            {
                var sql = string.Format(@"SELECT ttdv.TenDanhMuc as DonVi,   
                                                COUNT(lx1.Id) AS MBCC_SoLuong,
	                                            COUNT(lx2.id) AS MBCC_CL_HoatDong,
	                                            COUNT(lx4.id) AS MBCC_CL_HuHong,
	                                            COUNT(lx5.Id) AS MBCC_LPT_TohatsuV46,
	                                            COUNT(lx6.id) AS MBCC_LPT_TohatsuV75,
	                                            COUNT(lx7.id) AS MBCC_LPT_Bj25,
	                                            COUNT(lx8.Id) AS MBCC_LPT_Rong,
	                                            COUNT(lx9.id) AS MBCC_LPT_LoaiKhac,
	                                            COUNT(lx10.id) AS TCCC_SoLuong,
	                                            COUNT(lx11.Id) AS TCCC_CL_HoatDong,
	                                            COUNT(lx12.id) AS TCCC_CL_HuHong,
	                                            COUNT(lx13.id) AS TCCC_LPT_TauCCBien,
	                                            COUNT(lx14.Id) AS TCCC_LPT_TauCCSong,
	                                            COUNT(lx15.id) AS TCCC_LPT_CanoCC,
                                                COUNT(lx16.id) AS TCCC_LPT_XuongCH
                                            FROM dbo.TTPhuongTienChuaChay tttc
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx1 ON lx1.LoaiPhuongTien = 151  AND lx1.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx2 ON lx2.LoaiPhuongTien = 152 AND lx2.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx3 ON lx3.LoaiPhuongTien = 153  AND lx3.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx4 ON lx4.LoaiPhuongTien = 154 AND lx4.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx5 ON lx5.LoaiPhuongTien = 155  AND lx5.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx6 ON lx6.LoaiPhuongTien = 156 AND lx6.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx7 ON lx7.LoaiPhuongTien = 157  AND lx7.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx8 ON lx8.LoaiPhuongTien = 158 AND lx8.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx9 ON lx9.LoaiPhuongTien = 159  AND lx9.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx10 ON lx10.LoaiPhuongTien = 160 AND lx10.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx11 ON lx11.LoaiPhuongTien = 161  AND lx11.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx12 ON lx12.LoaiPhuongTien = 162 AND lx12.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx13 ON lx13.LoaiPhuongTien = 163  AND lx13.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx14 ON lx14.LoaiPhuongTien = 164 AND lx14.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx15 ON lx15.LoaiPhuongTien = 165  AND lx15.id = tttc.id
                                            LEFT JOIN dbo.TTPhuongTienChuaChay lx16 ON lx16.LoaiPhuongTien = 166 AND lx16.id = tttc.id
                                            INNER JOIN dbo.DanhMucChung ttdv ON tttc.DonVi = ttdv.id
                                            WHERE tttc.DonVi = {0}
                                            GROUP BY ttdv.TenDanhMuc", idSo);

                return _db.Fetch<Report004_05>(sql).ToList()[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int GetTotalTTPhuongTienChuaChay()
        {
            try
            {
                var sql = string.Format("SELECT Total_Rows= SUM(st.row_count) FROM sys.dm_db_partition_stats st WHERE object_name(object_id) = '{0}' AND (index_id < 2)", "SystemParameters");
                return _db.SingleOrDefault<int>(sql);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}