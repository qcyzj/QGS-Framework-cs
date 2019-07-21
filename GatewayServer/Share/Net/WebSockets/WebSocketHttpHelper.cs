using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using Share.Logs;

namespace Share.Net.WebSockets
{
    public static class WebSocketHttpHelper
    {
        private const string HTTP_GET_HEADER_REGEX = @"^GET(.*)HTTP\/1\.1";

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
                    LogManager.Error("Http header message too large to fit buffer.");
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

        public static bool IsWebSocketUpgradeRequest(string http_header)
        {
            Regex head_regex = new Regex(HTTP_HEADER_REGEX, RegexOptions.IgnoreCase);
            Match head_match = head_regex.Match(http_header);

            if (head_match.Success)
            {
                Regex upgrade_regex = new Regex("Upgrade: websocket", RegexOptions.IgnoreCase)
            }

            return false;
        }


        public static bool ParseHttpHeaderUriField(string http_header, out string value)
        {
            Regex regex = new Regex(HTTP_GET_HEADER_REGEX, RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        public static bool ParseHttpHeaderHostField(string http_header, out string value)
        {
            Regex regex = new Regex(@"Host: (.*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        public static bool ParseHttpHeaderUpgradeField(string http_header, out string value)
        {
            Regex regex = new Regex(@"Upgrade: (.*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        public static bool ParseHttpHeaderConnectionField(string http_header, out string value)
        {
            Regex regex = new Regex(@"Connection: (.*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        public static bool ParseHttpHeaderSecWebSocketKeyField(string http_header, out string value)
        {
            Regex regex = new Regex(@"Sec-WebSocket-Key: (.*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        public static bool ParseHttpHeaderSecWebSocketVersionField(string http_header, out string value)
        {
            Regex regex = new Regex(@"Sec-WebSocket-Version: (.*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        public static bool ParseHttpHeaderSecWebSocketProtocolField(string http_header, out string value)
        {
            Regex regex = new Regex(@"Sec-WebSocket-Protocol: (.*)", RegexOptions.IgnoreCase);
            Match match = regex.Match(http_header);

            if (match.Success)
            {
                value = match.Groups[1].Value.Trim();
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }
    }
}
