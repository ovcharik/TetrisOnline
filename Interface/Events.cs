using System;

namespace Interface
{
    public static class Events
    {
        // Client
        public const Int32 SIGN_IN = 0x001;
        public const Int32 SIGN_OUT = 0x002;
        public const Int32 SEND_MSG = 0x003;

        // Server
        public const Int32 SIGNED_IN = 0x101;
        public const Int32 SIGNED_OUT = 0x102;
        public const Int32 UPDATE_ID = 0x103;
        public const Int32 UPDATE_USER_LIST = 0x104;
        public const Int32 SENDED_MSG = 0x105;

        static public String EventToString(Int32 e)
        {
            switch (e)
            {
                case SIGN_IN:
                    return "Sign In";
                case SIGN_OUT:
                    return "Sign Out";
                case SEND_MSG:
                    return "Send Message";

                case SIGNED_IN:
                    return "Signed In";
                case SIGNED_OUT:
                    return "Signed Out";
                case UPDATE_ID:
                    return "Update ID";
                case UPDATE_USER_LIST:
                    return "Update Users List";
                case SENDED_MSG:
                    return "Sended Message";

                default:
                    return "Undefined Event";
            }
        }
    }
}
