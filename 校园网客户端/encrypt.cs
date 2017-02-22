using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

namespace 校园网客户端
{
    class encrypt
    {
        // UrlEncode编码转换
        public static string UrlEncode(string temp, Encoding encoding)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                string t = temp[i].ToString();
                string k = HttpUtility.UrlEncode(t, encoding);
                if (t == k)
                {
                    stringBuilder.Append(t);
                }
                else
                {
                    stringBuilder.Append(k.ToUpper());
                }
            }
            return stringBuilder.ToString();
        }

        // 密码加密
        public static String passwordEncode(String pwd)
        {
            String encoding_key = "1234567890";
            String pe = "";
            for (int i = 0; i < pwd.Length; i++)
            {
                char[] _a = encoding_key.Substring(encoding_key.Length - i % encoding_key.Length - 1, 1).ToCharArray();
                char[] _b = pwd.Substring(i, 1).ToCharArray();
                int ki = _a[0] ^ _b[0];
                char _l = (char)((ki & 0x0F) + 0x36);
                char _h = (char)((ki >> 4 & 0x0F) + 0x63);
                if (i % 2 == 0) pe += _l + "" + _h + "";
                else pe += _h + "" + _l + "";
            }
            //return pe;
            return UrlEncode(pe, Encoding.UTF8);
        }

        //账号加密
        public static String userNameEncode(String username)
        {

            String rtn = "{SRUN3}\r\n";
            char[] usr_arr = username.ToCharArray();
            for (int i = 0; i < usr_arr.Length; ++i)
            {
                rtn += (char)((int)usr_arr[i] + 4);
            }
            //return rtn;
            return UrlEncode(rtn, Encoding.UTF8);
        }


        public static String getUrlData(String name, String password)
        {
            name = userNameEncode(name);
            password = passwordEncode(password);
            String data =
                "action=login&username="
                        + name
                        + "&password="
                        + password
                        + "&drop=0&pop=1&type=2&n=117&mbytes=0&minutes=0&ac_id="
                        + "1"
                        + "&mac="
                        + "";
            return data;
        }
    }
}
