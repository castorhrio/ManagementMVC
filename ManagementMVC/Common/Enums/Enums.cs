using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public class Enums
    {
        /// <summary>
        /// 枚举value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 枚举显示值
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 枚举说明
        /// </summary>
        public string Text { get; set; }
    }

    #region 系统管理相关

    public enum enumOperator
    {
        [Description("无")]
        None,

        [Description("查询")]
        Select,

        [Description("添加")]
        Add,

        [Description("修改")]
        Edit,

        [Description("移除")]
        Remove,

        [Description("登录")]
        Login,

        [Description("登出")]
        LogOut,

        [Description("导出")]
        Export,

        [Description("导入")]
        Import,

        [Description("审核")]
        Audit,

        [Description("回复")]
        Reply,

        [Description("下载")]
        Download,

        [Description("上传")]
        Upload,

        [Description("分配")]
        Allocation,

        [Description("文件")]
        Files,

        [Description("流程")]
        Flow
    }

    public enum EnumLog4Net
    {
        [Description("普通")]
        INFO,
        [Description("警告")]
        WARN,
        [Description("错误")]
        ERROR,
        [Description("异常")]
        FATAL,
    }

    public enum EnumModuleType
    {
        无页面 = 1,
        列表页 = 2,
        弹出页 = 3
    }

    public enum EnumDepartmentType
    {
        情报管理局 = 1,
        施工队 = 2,
        工程部 = 3,
        计划科 = 4,
        其他部门 = 5
    }
    #endregion

    #region 流程枚举

    public enum FlowEnum
    {
        [Description("空白")]
        Black = 0,

        [Description("草稿")]
        Draft = 1,

        [Description("运行中")]
        Running = 2,

        [Description("已完成")]
        Complete = 3,

        [Description("挂起")]
        HungUp = 4,

        [Description("退回")]
        ReturnSta = 5,

        [Description("移交")]
        Shift = 6,

        [Description("删除")]
        Delete = 7,

        [Description("加签")]
        AskFor = 8,

        [Description("冻结")]
        Fix = 9,

        [Description("批处理")]
        Batch = 10,

        [Description("加签回复")]
        AskForReply = 11,
    }
    #endregion

    #region 系统字典

    public class SysDic
    {
        /// <summary>
        /// 根据DicKey值获取value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetDicValueByKey(string key, Dictionary<string, string> p)
        {
            if (p == null || p.Count == 0)
                return "";
            var dic = p.GetEnumerator();
            while (dic.MoveNext())
            {
                var obj = dic.Current;
                if (key == obj.Key)
                    return obj.Value;
            }

            return "";
        }

        /// <summary>
        /// 根据DICValue获取Key
        /// </summary>
        /// <param name="value"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetDicKeyByValue(string value, Dictionary<string, string> p)
        {
            if (p == null || p.Count == 0)
                return "";
            var dic = p.GetEnumerator();
            while (dic.MoveNext())
            {
                var obj = dic.Current;
                if (obj.Value == value)
                    return obj.Key;
            }

            return "";
        }

        /// <summary>
        /// 实体与编码对应字典,在验证数据权限时,通过此处字典来枚举实体编号
        /// </summary>
        public static Dictionary<string, string> DicEntity
        {
            get
            {
                Dictionary<string,string> _dic = new Dictionary<string, string>();
                _dic.Add("日志","");
                _dic.Add("用户", "18da4207-3bfc-49ea-90f7-16867721805c");      //用户Guid
                return _dic;
            }
        }

        /// <summary>
        /// 描述:存放特别的角色编号字典,在验证操作权限时用到
        /// </summary>
        public static Dictionary<string, int> DicRole
        {
            get
            {
                Dictionary<string,int> _dic = new Dictionary<string, int>();
                _dic.Add("超级管理员",1);
                return _dic;
            }
        }

        public static Dictionary<string, string> DicCodeType
        {
            get
            {
                Dictionary<string,string> _dic = new Dictionary<string, string>();
                try
                {
                    string dicStr = 
                }
            }
        } 
    }
    #endregion
}
