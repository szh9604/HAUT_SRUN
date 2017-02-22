using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 校园网客户端
{
    [Serializable]
    class user
    {
        private String userCode;
        private String userPasswd;

        public user(String code,String passwd)
        {
            userCode = code;
            userPasswd = passwd;
        }

        public String getUserCode()
        {
            return userCode;
        }

        public String getUserPasswd()
        {
            return userPasswd;
        }

        public void setUserPasswd(String passwd)
        {
            userPasswd = passwd;
        }
    }
}
