using System.Collections.Generic;

namespace ViELearn.Models.ViELearnModels
{
    public class JsTreeModel
    {
        public string text;
        public string id;
        public List<JsTreeModel> children;
        public object data = new  { quyenXem = false, quyenThemMoi = false, quyenSua = false, quyenXoa = false };
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
        public bool quyenSua { get; set; }
        public bool quyenXoa { get; set; }
        public bool quyenThemMoi { get; set; }
    }

    public class JsonNodeState
    {
        public bool loaded { get; set; }
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }
}