using System.Collections.Generic;

namespace ViELearn.Models.SystemModels
{
    public class JsTreeModel
    {
        public string text { get; set; }
        public string id { get; set; }
        public int idParrent { get; set; }
        public List<JsTreeModel> children { get; set; }

        public object data = new { quyenXem = false, quyenThemMoi = false, quyenSua = false, quyenXoa = false, quyenChuyen = false, quyenDuyet = false, quyenXuatBan = false };

        public object state = new { opened = false };
    }
    public class JsonTreeData
    {
        public string NhomNguoiDung { get; set; }
        public List<JsonNodeList> DanhSachQuyen { get; set; }
        public List<int> DanhSachUndetermineds { get; set; }
    }
    public class JsonNodeList
    {
        public string icon { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public JsonNodeState state { get; set; }
        public JsonNodeData data { get; set; }
    }
    public class JsonNodeData
    {
        public bool quyenXem { get; set; }
        public bool quyenThemMoi { get; set; }
        public bool quyenSua { get; set; }
        public bool quyenXoa { get; set; }
        public bool quyenChuyen { get; set; }
        public bool quyenDuyet { get; set; }
        public bool quyenXuatBan { get; set; }

    }

    public class JsonNodeState
    {
        public bool? loaded { get; set; }
        public bool? opened { get; set; }
        public bool? selected { get; set; }
        public bool? disabled { get; set; }
        public bool? @checked { get; set; }
    }
}