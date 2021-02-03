//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;

//namespace SqlServerTest
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Test();
//            Console.Read();
//        }

//        static void Test()
//        {
//            string connectString = "Data Source=.;Initial Catalog=TonyLab;Integrated Security=True";
//            SqlConnection sqlCnt = new SqlConnection(connectString);
       
//            string sql =
//               "create table Equipment(news_id int primary key identity(1, 1),news_title varchar(50) not null,news_author varchar(20),news_summary varchar(50),news_content text not null,news_pic varchar(50))";
//            SqlCommand cmd = new SqlCommand(sql, sqlCnt);
//            try
//            {
//                cmd.ExecuteNonQuery();
//                Console.WriteLine(0);
//            }
//            catch (Exception)
//            {
//                Console.WriteLine(-1);
//            }
//        }
//    }

//    public class mySqlserver
//    {
//        /// <summary>
//        /// 验证方式
//        /// </summary>
//        public enum Verification_Method
//        {
//            Windows,
//            Sqlserver,
//        }

//        /// <summary>
//        /// sql链接变量
//        /// </summary>
//        private SqlConnection con = new SqlConnection();

//        /// <summary>
//        /// 链接字符串
//        /// </summary>
//        private string constr;

//        /// <summary>
//        /// 当前操作的数据库名称
//        /// </summary>
//        private string database;

//        public mySqlserver()
//        {

//        }

//        /// <summary>
//        /// 不用用户名的链接方式
//        /// </summary>
//        /// <param name="_data_source">数据源</param>
//        /// <param name="_initial_catalog">数据库名称</param>
//        /// <param name="_integrated_security">综合安全</param>
//        public mySqlserver(string _data_source,string _initial_catalog,bool _integrated_security)
//        {
//           constr = "Data Source="+ _data_source + ";Initial Catalog="+ _initial_catalog + ";Integrated Security="+( _integrated_security==true?"True":"False");
//            con.ConnectionString = constr;
//        }

//        /// <summary>
//        /// 以constr为ConnectionString实例化并打开con
//        /// </summary>
//        /// <returns></returns>
//        public int Open()
//        {
//            try
//            {
//                if (con.State==ConnectionState.Open)
//                {
//                    con.Close();
//                    con.Open();
//                }
//                else
//                {
//                    con.Open();
//                }
//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }
//        }

//        public int Close()
//        {
//            try
//            {
//                con.Close();
//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }
//        }

//        public int Create_Datebase(string _database)
//        {
//            // 打开数据库连接
//            if (con.State != ConnectionState.Open)
//                con.Open();
//            string sql = "CREATE DATABASE " + _database + " ON PRIMARY"
//                               + "(name=test_data, filename = 'C:\\mysql\\mydb_data.mdf', size=3,"
//                               + "maxsize=5, filegrowth=10%) log on"
//                               + "(name=mydbb_log, filename='C:\\mysql\\mydb_log.ldf',size=3,"
//                               + "maxsize=20,filegrowth=1)";
//            SqlCommand cmd = new SqlCommand(sql, con);
//            try
//            {
//                cmd.ExecuteNonQuery();
//                if (con.State == ConnectionState.Open)
//                    con.Close();
//                string ConnectionString = "Integrated Security=SSPI;" + "Initial Catalog=mydb;" + "Data Source=localhost;";
//                con.ConnectionString = ConnectionString;
//                con.Open();

//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }

//        }

//        public int Create_Datebase(string _database, string _path)
//        {
//            // 打开数据库连接
//            if (con.State != ConnectionState.Open)
//                con.Open();
//            string sql = "CREATE DATABASE " + _database + " ON PRIMARY"
//                               + "(name=test_data, filename = 'C:\\mysql\\mydb_data.mdf', size=3,"
//                               + "maxsize=5, filegrowth=10%) log on"
//                               + "(name=mydbb_log, filename='C:\\mysql\\mydb_log.ldf',size=3,"
//                               + "maxsize=20,filegrowth=1)";
//            SqlCommand cmd = new SqlCommand(sql, con);
//            try
//            {
//                cmd.ExecuteNonQuery();
//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }

//        }

//        public int Create_Datebase(string _database, string _mdfpath, string _logpath)
//        {
//            // 打开数据库连接
//            if (con.State != ConnectionState.Open)
//                con.Open();
//            string sql = "CREATE DATABASE " + _database + " ON PRIMARY"
//                               + "(name=test_data, filename = '" + _mdfpath + "', size=3,"
//                               + "maxsize=5, filegrowth=10%) log on"
//                               + "(name=mydbb_log, filename='" + _logpath + "',size=3,"
//                               + "maxsize=20,filegrowth=1)";
//            SqlCommand cmd = new SqlCommand(sql, con);
//            try
//            {
//                cmd.ExecuteNonQuery();
//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }

//        }

//        public void Create_Table(string _database)
//        {
//            string sql = "USE" +
//                "create table Equipment(news_id int primary key identity(1, 1),news_title varchar(50) not null,news_author varchar(20),news_summary varchar(50),news_content text not null,news_pic varchar(50))";
//        }

//        public int Test()
//        {
//            string sql =
//                "create table Equipment(news_id int primary key identity(1, 1),news_title varchar(50) not null,news_author varchar(20),news_summary varchar(50),news_content text not null,news_pic varchar(50))";
//            SqlCommand cmd = new SqlCommand(sql, con);
//            try
//            {
//                cmd.ExecuteNonQuery();
//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }
//        }

//        public int Inset()
//        {
//            string sql = "insert into tabel(Address,SerialNumber,Switch)value (192.168.101.1,040001800,0)";
//            try
//            {
//                SqlCommand com = new SqlCommand(sql, con);
//                return 0;
//            }
//            catch (Exception)
//            {
//                return -1;
//            }
//        }
//    }
//}
