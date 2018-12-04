using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Collections;

//create by wcy
//DataBase Operation 
//select table_schema,table_name,table_type FROM information_schema.tables where TABLE_SCHEMA='db0808' and table_type='base table'

//select table_schema,table_name,column_name,data_type,is_nullable from information_schema.columns WHERE TABLE_SCHEMA ='db0808' and TABLE_NAME='score'
namespace DbAccess
{
    public class MySqlHelper
    {
        private static string strcn = "server=localhost;user id=root;password=123456;database=mysql1204";

        #region 根据SQL查出DataTable对象
        public static System.Data.DataTable getDataTable(string Sql)
        {
           // string strcn = ConfigurationManager.AppSettings["SqlConnString"];
            MySqlConnection connect = new MySqlConnection(strcn);
            DataTable dataTable = new DataTable();

            try
            {
                connect.Open();
                MySqlDataAdapter dap = new MySqlDataAdapter(Sql, strcn);
                dap.Fill(dataTable);
               
            }
            catch (System.Exception e)
            {
                dataTable = null;
                throw e;
            }
            finally
            {
                connect.Close();
                connect.Dispose();
                connect = null;            
            }
            return dataTable;
        }

        //执行一个Sql语句[如插入、更新、删除]
        //Create By Wcy
        //Create Date:2017-08-24    
        public static int ExecuteNonQuery(string Sql)
        {
           // string strcn = ConfigurationManager.AppSettings["SqlConnString"];                  
            MySqlConnection gCn = new MySqlConnection(strcn);

            try
            {
                gCn.Open();
            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }
            MySqlCommand Cmd = new MySqlCommand(Sql, gCn);
            try
            {
                Cmd.Prepare();
                int iResult = Cmd.ExecuteNonQuery();
                return iResult;
            }
            catch (System.Exception Ex)
            {
                throw (Ex);
            }
            finally
            {
                gCn.Close();
                gCn.Dispose();
                gCn = null;
                Cmd.Dispose();
                Cmd = null;
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="strSqlList">多条SQL语句</param>		
        public static void ExecuteSqlTran(ArrayList strSqlList)
        {
            //string strcn = ConfigurationManager.AppSettings["SqlConnString"];
            using (MySqlConnection conn = new MySqlConnection(strcn))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();

             

                cmd.Connection = conn;
                MySqlTransaction trans = conn.BeginTransaction();
                cmd.Transaction = trans;
                try
                {
                    for (int n = 0; n < strSqlList.Count; n++)
                    {
                        string strsql = strSqlList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    trans.Rollback();
                    throw e;
                }
                finally
                {
                    conn.Close();
                    cmd.Dispose();
                    trans.Dispose();
                }
            }
        }


        //执行一个Sql语句返回一个值
        //Create By Wcy
        //Create Date:2017-08-24    
        public static object ExecuteScalar(string Sql)
        {
            //string strcn = ConfigurationManager.AppSettings["SqlConnString"];
            MySqlConnection gCn = new MySqlConnection(strcn);

            try
            {
                gCn.Open();
            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }
            MySqlCommand Cmd = new MySqlCommand(Sql, gCn);
            try
            {
                Cmd.Prepare();             
                return Cmd.ExecuteScalar();
            }
            catch (System.Exception Ex)
            {
                throw (Ex);
            }
            finally
            {
                gCn.Close();
                gCn.Dispose();
                gCn = null;
                Cmd.Dispose();
                Cmd = null;
            }
        }

//sqlText VARCHAR(4000),in currentpage int,in pagesize int,OUT rowcount
        //分页存储过程   
        public static DataTable sp_page(string sql, int curIndex,int page_Size,out int rowCount)
        {
            MySqlConnection gCn = new MySqlConnection(strcn);

            try
            {
                gCn.Open();
            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }

            try
            {
                MySqlCommand cmd = new MySqlCommand(); //调用指定的存储过程
                cmd.CommandText = "Paging_Stduents";
                cmd.CommandType = CommandType.StoredProcedure;  //指明类型
                cmd.Connection = gCn;

                cmd.Parameters.Clear();
                MySqlParameter[] parameters = {
                    new MySqlParameter("sqlText", MySqlDbType.VarChar, 4000),
                    new MySqlParameter("currentpage", MySqlDbType.Int32),
                    new MySqlParameter("pagesize", MySqlDbType.Int32),               
                    new MySqlParameter("rowcount", MySqlDbType.Int32),
                    };

                parameters[0].Value = sql;
                parameters[1].Value = curIndex;
                parameters[2].Value = page_Size;              
                parameters[3].Direction = ParameterDirection.Output;

                cmd.Parameters.AddRange(parameters);   

                MySqlDataAdapter dap = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dap.Fill(dt);
                rowCount = Convert.ToInt32(parameters[3].Value);
                dap.Dispose();
                return dt;
            }
            catch (System.Exception ex)
            {
                throw (ex);
            }
            finally
            {
                gCn.Close();
                gCn.Dispose();
                gCn = null;            
            }

        }
     
        public static DataTable getStudent()
        {
           // string strcn = ConfigurationManager.AppSettings["SqlConnString"];
            MySqlConnection gCn = new MySqlConnection(strcn);
            string sql = "sp_page2('select * from student',1,2,@tt);";
            MySqlCommand Cmd = new MySqlCommand(sql, gCn);

            try
            {
                gCn.Open();
                Cmd.ExecuteNonQuery(); //执行存储过程

                //显示数据
                MySqlDataAdapter dap = new MySqlDataAdapter(Cmd);
                DataTable dt = new DataTable();
                dap.Fill(dt);
                dap.Dispose();
                return dt;

            }
            catch (MySqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                gCn.Close();
                gCn.Dispose();
                gCn = null;

            }


        }

        #endregion
    }
}
