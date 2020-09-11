namespace Snake.Online
{
    public enum ClintStaet
    {
        /// <summary>
        /// 请求连接
        /// </summary>
        Connect,
        /// <summary>
        /// 连接成功
        /// </summary>
        Success,
        /// <summary>
        /// 超出连接人数
        /// </summary>
        Out_Of_Num,
        /// <summary>
        /// 断开
        /// </summary>
        Break
    }
    
    public class SeverMsg
    {
        public string name;
        public string ip;
        public int point;
    }
    
    public class ClineMsg
    {
        public string name;
        public string ip;
        public int point;
        public ClintStaet clintStaet;
    }
}