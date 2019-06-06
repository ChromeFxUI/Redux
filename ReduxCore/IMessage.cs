using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReduxCore
{
    public interface IMessage
    {
        Message Message { get; set; }
    }

    public class Message
    {
        public MessageType MsgType { get; set; } = MessageType.Info;
        public string Msg { get; set; } = "";
        public int Duration { set; get; } = 3000;

        public Showtype Showtype { get; set; } = Showtype.Message;
    }

    public enum MessageType
    {
        Undefine = -1,
        Waring = 0,
        Success = 1,
        Info = 2,
        Error = 3,
    }

    public enum Showtype
    {
        Message = 0,
        Alert = 1,
        Loading = 2,
        MessageBox = 3,
        Notification = 4,
    }
}
