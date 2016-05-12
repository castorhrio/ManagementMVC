﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 数据库连接字符串提取操作
    /// 字符串对应应用程序中配置文件
    /// 模型对应Domain中的数据库模型Context.cs构造函数
    /// </summary>
    public class MyConfig:ManagementMVCEntities
    {
        /// <summary>
        /// 封装EF实体模型，供Dao使用
        /// </summary>
        public System.Data.Entity.DbContext db { get; private set; }

        public MyConfig()
        {
            //实例化EF数据上下文
            db = new ManagementMVCEntities();
        }

        #region 连接数据库配置
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DefaultConnectionString = "";

        /// <summary>
        /// 通用数据库链接对象配置
        /// </summary>
        public static IDbConnection DefaultConnection
        {
            get
            {
                IDbConnection defaultConn = null;
                string action = ConfigurationManager.AppSettings["daoType"];

                //数据库类型
                switch (action)
                {
                    case "mssql":
                        defaultConn = new System.Data.SqlClient.SqlConnection();
                        DefaultConnectionString =
                            ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
                        break;
                    //case "oracle":
                    //    defaultConn = new Oracle.ManagedDataAccess.Client.OracleConnection();
                    //    DefaultConnectionString = ConfigurationManager.ConnectionStrings["OracleConnectionString"].ConnectionString;
                    //    break;
                }

                return defaultConn;
            }
        }

        /// <summary>
        /// 构造数据库连接字符串 注：数据库切换要修改
        /// </summary>
        /// <param name="EntityName"></param>
        /// <returns></returns>
        public static string DataBaseConnectionString(string EntityName)
        {
            IDbConnection con = DefaultConnection;
            return EFConnectionStringModle(EntityName, DefaultConnectionString);
        }

        /// <summary>
        /// 构造EF使用数据库连接字符串
        /// </summary>
        /// <param name="EntityName">数据上下文坏境</param>
        /// <param name="DBsoure">数据字符串</param>
        /// <returns></returns>
        static string EFConnectionStringModle(string EntityName, string DBsoure)
        {
            return string.Concat("metadate=res://*/",
                EntityName, ".csdl|res://*/",
                EntityName, ".ssdl|res://*/",
                EntityName, ".msl;provider=System.Data.SqlClient;provider connection string='",
                DBsoure, "");
        }
        #endregion
    }
}
