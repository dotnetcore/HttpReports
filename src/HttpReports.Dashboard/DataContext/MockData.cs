using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.DataContext
{
    public class MockData
    {
        public void MockSqlServer(string Constr)
        { 
            using (SqlConnection con = new SqlConnection(Constr))
            {
                for (int i = 0; i < 100; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/aaa','/api/user/aaa','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 98; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/bbb','/api/user/bbb','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 92; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/ccc','/api/user/ccc','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 90; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/ddd','/api/user/ddd','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 82; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/eee','/api/user/eee','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 80; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/fff','/api/user/fff','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 76; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/ggg','/api/user/ggg','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 72; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/iii','/api/user/iii','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 70; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/mmm','/api/user/mmm','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 66; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/ttt','/api/user/ttt','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 65; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/uuu','/api/user/uuu','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 62; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/rrr','/api/user/rrr','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 58; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/vvv','/api/user/vvv','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 30; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/ppp','/api/user/ppp','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 10; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/xxx','/api/user/xxx','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 55; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/lll','/api/user/lll','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 52; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/b1','/api/user/b1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 53; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/c2','/api/user/c2','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 35; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/h1','/api/user/h1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 13; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/r1','/api/user/r1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 31; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/pc','/api/user/pc','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }
                 

                for (int i = 0; i < 30; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/k23','/api/user/k23','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 10; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/mk1','/api/user/mk1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 55; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/plcm','/api/user/plcm','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 52; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/zhu','/api/user/zhu','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 53; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/umw','/api/user/umw','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 35; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Auth','/user/apcod','/api/user/apcod','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 13; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Payment','/user/mdfdfd','/api/user/mdfdfd','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 31; i++)
                {
                    con.Execute($"Insert Into RequestInfo Values ('Log','/user/ddmmm','/api/user/ddmmm','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

            }
        } 

        public void MockMySql(string Constr)
        {
            using (MySqlConnection con = new MySqlConnection(Constr))
            {
                //for (int i = 0; i < 100; i++)
                //{
                //    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime) Values ('auth','/user/checklogin','/api/user/checklogin','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                //}

                for (int i = 0; i < 100; i++)
                {
                    con.Execute($"Insert Into RequestInfo (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/aaa','/api/user/aaa','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 98; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/bbb','/api/user/bbb','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 92; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/ccc','/api/user/ccc','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 90; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/ddd','/api/user/ddd','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 82; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/eee','/api/user/eee','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 80; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/fff','/api/user/fff','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 76; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/ggg','/api/user/ggg','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 72; i++)
                {
                    con.Execute($"Insert Into RequestInfo   (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime) Values ('Auth','/user/iii','/api/user/iii','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 70; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/mmm','/api/user/mmm','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 66; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/ttt','/api/user/ttt','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 65; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/uuu','/api/user/uuu','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 62; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/rrr','/api/user/rrr','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 58; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/vvv','/api/user/vvv','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 30; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/ppp','/api/user/ppp','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 10; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/xxx','/api/user/xxx','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 55; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/lll','/api/user/lll','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 52; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/b1','/api/user/b1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 53; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/c2','/api/user/c2','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 35; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/h1','/api/user/h1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 13; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/r1','/api/user/r1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 31; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/pc','/api/user/pc','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }


                for (int i = 0; i < 30; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/k23','/api/user/k23','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 10; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/mk1','/api/user/mk1','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 55; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/plcm','/api/user/plcm','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 52; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/zhu','/api/user/zhu','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 53; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/umw','/api/user/umw','GET',{new Random().Next(1, 999)},'200','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 35; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Auth','/user/apcod','/api/user/apcod','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 13; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Payment','/user/mdfdfd','/api/user/mdfdfd','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }

                for (int i = 0; i < 31; i++)
                {
                    con.Execute($"Insert Into RequestInfo  (Node,Route,Url,Method,Milliseconds,StatusCode,IP,CreateTime)  Values ('Log','/user/ddmmm','/api/user/ddmmm','GET',{new Random().Next(1, 999)},'500','192.168.1.1','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                }





            }
        }

    }
}
