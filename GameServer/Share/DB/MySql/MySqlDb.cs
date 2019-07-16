using System;
using System.Diagnostics;

using Share.Logs;

using MySql.Data.MySqlClient;

namespace Share.DB.MySql
{
    public class MySqlDb
    {
        private MySqlConnection m_DBConection;
        private string m_ConnctionString;


        public MySqlDb(string connction_str)
        {
            Debug.Assert(string.Empty != connction_str);

            m_ConnctionString = connction_str;
            m_DBConection = new MySqlConnection(m_ConnctionString);
        }

        private void Connect()
        {
            try
            {
                m_DBConection.Open();
            }
            catch (MySqlException sql_ex)
            {
                LogManager.Error("MySql Connect error: ", sql_ex);
            }
            catch (Exception ex)
            {
                LogManager.Error("MySql Connect error: ", ex);
            }
            finally
            {
                m_DBConection.Close();
            }
        }

        public void Close()
        {
            m_DBConection.Close();
        }


        public MySqlCommand GetCommand(string sql_str)
        {
            return new MySqlCommand(sql_str, m_DBConection);
        }

        public MySqlCommand GetCommand(string sql_str, MySqlTransaction trans)
        {
            return new MySqlCommand(sql_str, m_DBConection, trans);            
        }

        public int ExecuteNonQuery(MySqlCommand command)
        {
            return command.ExecuteNonQuery();
        }

        public object ExecuteScalar(MySqlCommand command)
        {
            return command.ExecuteScalar();
        }

        public MySqlTransaction BeginTransaction()
        {
            return m_DBConection.BeginTransaction();
        }       
    }
}
