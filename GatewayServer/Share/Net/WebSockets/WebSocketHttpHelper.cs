using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Share.Net.WebSockets
{
    public static class WebSocketHttpHelper
    {
        public static string ReadHttpHeader(NetworkStream stream)
        {
            while (!stream.DataAvailable)
            { }

            int read_bytes = 0;
            byte[] buffer = new byte[8192];
            int offset = 0;

            do
            {
                if (offset >= 8192)
                {
                    break;
                }

                read_bytes = stream.Read(buffer, offset, buffer.Length - offset);
                offset += read_bytes;

                string header = Encoding.UTF8.GetString(buffer);

                if (Regex.IsMatch(header, "\r\n\r\n"))
                {
                    return header;
                }
            }
            while (read_bytes > 0);

            return string.Empty;
        }
    }
}
