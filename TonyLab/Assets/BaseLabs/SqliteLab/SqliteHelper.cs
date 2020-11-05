using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NonsensicalKit
{
    public class SQLManagerHelper
    {
        // 数据库连接对象
        private SqliteConnection _Connection;
        // 数据库命令
        private SqliteCommand _Command;
        // 数据读取定义
        private SqliteDataReader _Reader;
        // 本地数据库名字
        private string _SqlName;
        // 数据库存放位置
        private string _Path;
        // 同步锁
        private static readonly object sqlservice = new object();

        /// <summary>
        /// 执行SQL命令,并返回一个SqliteDataReader对象
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public SqliteDataReader ExecuteSQLCommand(string queryString)
        {
            try
            {
                lock (sqlservice)
                {
                    this._Command = this._Connection.CreateCommand();
                    this._Command.CommandText = queryString;
                    this._Reader = this._Command.ExecuteReader();
                    return this._Reader;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 重置数据库属性
        /// </summary>
        public void Reset()
        {
            //CREATE TABLE TABLEACCOUNT(ACCOUNT TEXT  PRIMARY KEY NOT NULL, PSSSWARD TEXT, STATE INT)
            //账号表：账户,密码,登录状态(0为未登录，1为登陆)
            ExecuteSQLCommand("INSERT INTO TABLEACCOUNT VALUES('admin','123456',0);");


            //    CREATE TABLE "TABLEQUESTIONBANK"(
            //"QUESTIONID"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
            //"TITLETEXT" TEXT NOT NULL DEFAULT 'nothing',
            //"ATEXT" TEXT NOT NULL DEFAULT 'nothing',
            //"BTEXT" TEXT NOT NULL DEFAULT 'nothing',
            //"CTEXT" TEXT DEFAULT 'nothing',
            //"DTEXT" TEXT DEFAULT 'nothing',
            //"RIGHTANSWERTEXT"   TEXT NOT NULL DEFAULT 'nothing',
            //"QUESTIONTYPE"  INTEGER NOT NULL,
            //"TITLETYPE" INTEGER,
            //"OPTIONTYPE"    INTEGER
            //  )
            /* "INSERT INTO TABLEQUESTIONBANK (TITLETEXT,ATEXT,BTEXT,CTEXT,DTEXT,RIGHTANSWERTEXT,QUESTIONTYPE,TITLETYPE,OPTIONTYPE) VALUES('TEXT','TEXT','TEXT','TEXT','TEXT','TEXT',0,0,0);"
             * QUESTIONTYPE:0.判断，1.单选，2.多选
             * TITLETYPE & OPTIONTYPE:0.文字，1.图片，2.视频
             */

            //生成判断题
            for (int i = 1; i <= 200; i++)
            {
                int a = GetRandomInt(50) + 50;
                int b = GetRandomInt(50) + 50;
                int isTrue = Math.Abs(GetRandomInt(2));
                int d = 0;
                if (isTrue == 0)
                {
                    int r = 0;
                    while ((r = GetRandomInt(10)) == 0) ;
                    d = a + b + r;
                }
                else
                {
                    d = a + b;
                }
                ExecuteSQLCommand("INSERT INTO TABLEQUESTIONBANK (TITLETEXT,ATEXT,BTEXT,CTEXT,DTEXT,RIGHTANSWERTEXT,QUESTIONTYPE,TITLETYPE,OPTIONTYPE) " +
                                    "VALUES('" + a + "+" + b + "=" + d + "?','Yes','nothing','No','nothing','" + (isTrue == 1 ? "A" : "C") + "',0,0,0);");
            }

            //生成单选题
            for (int i = 0; i < 200; i++)
            {

                int a = GetRandomInt(50) + 50;
                int b = GetRandomInt(50) + 50;
                int option = Math.Abs(GetRandomInt(4));
                int r = 0;
                while ((r = GetRandomInt(10)) == 0) ;
                int d1 = (option == 0 ? (a + b) : (a + b + r));
                while ((r = GetRandomInt(10)) == 0) ;
                int d2 = (option == 1 ? (a + b) : (a + b + r));
                while ((r = GetRandomInt(10)) == 0) ;
                int d3 = (option == 2 ? (a + b) : (a + b + r));
                while ((r = GetRandomInt(10)) == 0) ;
                int d4 = (option == 3 ? (a + b) : (a + b + r));
                ExecuteSQLCommand("INSERT INTO TABLEQUESTIONBANK (TITLETEXT,ATEXT,BTEXT,CTEXT,DTEXT,RIGHTANSWERTEXT,QUESTIONTYPE,TITLETYPE,OPTIONTYPE) " +
                                    "VALUES('" + a + "+" + b + "=?','" + d1 + "','" + d2 + "','" + d3 + "','" + d4 + "','" + ((option == 0) ? "A" : ((option == 1) ? "B" : ((option == 2) ? "C" : "D"))) + "',1,0,0);");
            }

            //生成多选题
            for (int i = 0; i < 100; i++)
            {
                int a = GetRandomInt(50) + 50;
                int dx = Math.Abs(GetRandomInt(2));
                int num = 0, d1 = 0, d2 = 0, d3 = 0, d4 = 0;
                bool[] isRight = new bool[4];
                while (num < 2)
                {
                    num = 0;
                    int r = 0;
                    while ((r = GetRandomInt(20)) == 0) ;
                    d1 = a + r;
                    if (dx == 0 ? d1 < a : d1 > a)
                    {
                        num++;
                        isRight[0] = true;
                    }
                    else
                    {
                        isRight[0] = false;
                    }
                    while ((r = GetRandomInt(20)) == 0) ;
                    d2 = a + r;
                    if (dx == 0 ? d2 < a : d2 > a)
                    {
                        num++;
                        isRight[1] = true;
                    }
                    else
                    {
                        isRight[1] = false;
                    }
                    while ((r = GetRandomInt(20)) == 0) ;
                    d3 = a + r;
                    if (dx == 0 ? d3 < a : d3 > a)
                    {
                        num++;
                        isRight[2] = true;
                    }
                    else
                    {
                        isRight[2] = false;
                    }
                    while ((r = GetRandomInt(20)) == 0) ;
                    d4 = a + r;
                    if (dx == 0 ? d1 < a : d4 > a)
                    {
                        num++;
                        isRight[3] = true;
                    }
                    else
                    {
                        isRight[3] = false;
                    }
                }
                ExecuteSQLCommand("INSERT INTO TABLEQUESTIONBANK (TITLETEXT,ATEXT,BTEXT,CTEXT,DTEXT,RIGHTANSWERTEXT,QUESTIONTYPE,TITLETYPE,OPTIONTYPE)" +
                                " VALUES('哪些值" + (dx == 0 ? "小于" : "大于") + a + "','" + d1 + "','" + d2 + "','" + d3 + "','" + d4 + "','" + (isRight[0] ? "A" : "") + (isRight[1] ? "B" : "") + (isRight[2] ? "C" : "") + (isRight[3] ? "D" : "") + "',2,0,0);");

            }
            //  CREATE TABLE "TABLEOPERATION"(
            //    "OPERATIONTIMES"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
            //    "ACCOUNT"   TEXT,
            //    "QUESTIONID"    TEXT,
            //    "ANSWER"    TEXT,
            //    "RIGHTANSWER"   TEXT
            //    )

            // CREATE TABLE "TABLETEST"(
            //"TESTID"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
            //"ACCOUNT"   TEXT,
            //"BEGINTIME" TEXT,
            //"RIGHTNUM"  INTEGER,
            //"COMPLETEDNUM"  INTEGER,
            //"FINISHTIME"    TEXT
            //)
        }

        /// <summary>
        /// 获取使用Guid作为种子返回的随机数
        /// </summary>
        /// <param name="max">返回值的绝对值小于max</param>
        /// <returns></returns>
        private int GetRandomInt(int max)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            System.Random random = new System.Random(iSeed);
            int temp = random.Next(max * 2 - 1);
            temp = temp - max + 1;

            return temp;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">数据库存储路径</param>
        /// <param name="sqlName">数据库名称</param>
        public SQLManagerHelper(string path, string sqlName)
        {

            this._Path = path;
            this._SqlName = sqlName;
            this.CreateSQL();
        }

        /// <summary>
        /// 创建数据库文件，如果已有数据库则会跳过
        /// </summary>
        private void CreateSQL()
        {
            bool flag = false;
            if (!File.Exists(this._Path + "/" + this._SqlName))
            {
                flag = true;
                if (!Directory.Exists(this._Path))
                {
                    Directory.CreateDirectory(this._Path);
                }
                //File.Create(this._Path + "/" + this._SqlName);
            }
            this._Connection = new SqliteConnection("data source=" + this._Path + "/" + this._SqlName);
            this._Connection.Open();

            if (flag)
            {
                Reset();
            }
        }

        /// <summary>
        /// 示例
        /// </summary>
        private void Example()
        {
            //-----示例-------
            SQLManagerHelper sqlmg = new SQLManagerHelper("D:/", "testDB.db");

            //创建表
            ExecuteSQLCommand(" CREATE TABLE COMPANY(ID INT , NAME TEXT  ,AGE  INT ); ");

            //插入数据
            ExecuteSQLCommand("INSERT INTO COMPANY VALUES (1, 'James', 24);");
            ExecuteSQLCommand("INSERT INTO COMPANY VALUES (2, 'James2', 25);");
            ExecuteSQLCommand("INSERT INTO COMPANY VALUES (3, 'James3', 26);");
            ExecuteSQLCommand("INSERT INTO COMPANY VALUES (4, 'James4', 27);");

            //查询表

            using (SqliteDataReader sdr = ExecuteSQLCommand("select * from COMPANY"))
            {
                //按行往record插入数据
                List<string> Record = new List<string>();   //新建链表
                int fc = sdr.FieldCount;                    //获取数据列数
                string temp = string.Empty;
                while (sdr.Read())                          //读取下一条记录
                {
                    if (sdr.HasRows)                        //是否还有记录
                    {
                        temp = "";
                        for (int i = 0; i < fc; i++)
                        {
                            temp += sdr[i] + "  ";
                        }
                        Record.Add(temp);
                    }
                }
            }
            //-----示例-------
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~SQLManagerHelper()
        {
            CloseSQLConnection();
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseSQLConnection()
        {
            if (this._Command != null)
            {
                this._Command.Cancel();
            }

            if (this._Reader != null)
            {
                this._Reader.Close();
            }

            if (this._Connection != null)
            {
                this._Connection.Close();

            }
            this._Command = null;
            this._Reader = null;
            this._Connection = null;
        }

        /// <summary>
        /// 获取对应表的行数
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetRow(string tableName)
        {
            using (SqliteDataReader sdr = ExecuteSQLCommand("select * from " + tableName))
            {
                int count = 0;
                while (sdr.Read())
                {
                    if (sdr.HasRows)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}
