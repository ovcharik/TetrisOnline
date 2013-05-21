using System;

namespace Interface
{
    public static class Events
    {
        // Client
        public const Int32 SIGN_IN = 0x001;
        public const Int32 SIGN_OUT = 0x002;
        public const Int32 SEND_MSG = 0x003;
        public const Int32 CREATE_ROOM = 0x004;
        public const Int32 WATCH_ROOM = 0x005;
        public const Int32 NOTWATCH_ROOM = 0x006;
        public const Int32 ENTER_ROOM = 0x007;
        public const Int32 LEAVE_ROOM = 0x008;
        public const Int32 SEND_ROOM_MSG = 0x009;

        // Server
        public const Int32 SIGNED_IN = 0x101;
        public const Int32 SIGNED_OUT = 0x102;
        public const Int32 UPDATE_ID = 0x103;
        public const Int32 UPDATE_USER_LIST = 0x104;
        public const Int32 SENDED_MSG = 0x105;
        public const Int32 CREATED_ROOM = 0x106;
        public const Int32 CLOSED_ROOM = 0x107;
        public const Int32 REMOVED_ROOM = 0x108;
        public const Int32 WATCHED_ROOM = 0x109;
        public const Int32 NOTWATCHED_ROOM = 0x10A;
        public const Int32 ENTERED_ROOM = 0x10B;
        public const Int32 LEAVED_ROOM = 0x10C;
        public const Int32 GAME_STARTED_ROOM = 0x10D;
        public const Int32 SENDED_MSG_ROOM = 0x00E;
        public const Int32 UPDATE_ROOM_LIST = 0x10F;

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
                case CREATE_ROOM:
                    return "Create Room";
                case WATCH_ROOM:
                    return "Watch Room";
                case NOTWATCH_ROOM:
                    return "Not Watch Room";
                case ENTER_ROOM:
                    return "Enter Room";
                case LEAVE_ROOM:
                    return "Leave Room";
                case SEND_ROOM_MSG:
                    return "Room Send Message";

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
                case CREATED_ROOM:
                    return "Created Room";
                case CLOSED_ROOM:
                    return "Closed Room";
                case REMOVED_ROOM:
                    return "Removed Room";
                case WATCHED_ROOM:
                    return "Watched Room";
                case NOTWATCHED_ROOM:
                    return "Not Watched Room";
                case ENTERED_ROOM:
                    return "Entered Room";
                case LEAVED_ROOM:
                    return "Leaved Room";
                case GAME_STARTED_ROOM:
                    return "Game Started Room";
                case SENDED_MSG_ROOM:
                    return "Sended Message Room";
                case UPDATE_ROOM_LIST:
                    return "Update Room List";

                default:
                    return "Undefined Event";
            }
        }
    }
}
