using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public static class Events
    {
        // Client
        public const Int32 SIGN_IN = 0x001;
        public const Int32 SIGN_OUT = 0x002;

        // Server
        public const Int32 SIGNED_IN = 0x101;
        public const Int32 SIGNED_OUT = 0x102;
        public const Int32 UPDATE_ID = 0x103;
        public const Int32 UPDATE_USER_LIST = 0x104;

        static public String EventToString(Int32 e)
        {
            switch (e)
            {
                case SIGN_IN:
                    return "Sign In";
                case SIGN_OUT:
                    return "Sign Out";

                case SIGNED_IN:
                    return "Signed In";
                case SIGNED_OUT:
                    return "Signed Out";
                case UPDATE_ID:
                    return "Update ID";
                case UPDATE_USER_LIST:
                    return "Update Users List";
                default:
                    return "Undefined Event";
            }
        }
    }
}
