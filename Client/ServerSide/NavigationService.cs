using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ServerSide
{
    public static class NavigationService
    {
        public static event EventHandler<Object> RaiseGotoSignInPage = null;
        public static event EventHandler<Object> RaiseGotoRoomPage = null;
        public static event EventHandler<Object> RaiseGotoMainPage = null;

        public static void GotoMainPage()
        {
            if (RaiseGotoMainPage != null) RaiseGotoMainPage(null, null);
        }

        public static void GotoSignInPage()
        {
            if (RaiseGotoSignInPage != null) RaiseGotoSignInPage(null, null);
        }

        public static void GotoRoomPage()
        {
            if (RaiseGotoMainPage != null) RaiseGotoRoomPage(null, null);
        }
    }
}
