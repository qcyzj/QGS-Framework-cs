using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Share.Json
{
    public class JsonData
    {
        private JObject m_Root;


        public JsonData()
        {
            m_Root = new JObject();
        }

        public JsonData(string json_string)
        {
            m_Root = JObject.Parse(json_string);
        }

        public JToken this[string key]
        {
            get { return m_Root[key]; }
            set { m_Root[key] = value; }
        }

        public string ToJsonString()
        {
            return m_Root.ToString();
        }            
    }

    public class JsonArray
    {
        private JArray m_Root;


        public JArray Root { get { return m_Root; } }


        public JsonArray()
        {
            m_Root = new JArray();
            Debug.Assert(JTokenType.Array == m_Root.Type);
        }

        public JsonArray(JToken token)
        {
            m_Root = (JArray)token;
            Debug.Assert(JTokenType.Array == m_Root.Type);
        }

        public JToken this[int index]
        {
            get { return m_Root[index]; }
            set { m_Root[index] = value; }
        }

        public void Add(JToken value)
        {
            m_Root.Add(value);
        }
    }

    public static class JsonHelper
    {
        public static T DeserializeJsonToObject<T>(string json_string) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json_string);
        }

        public static T DeserializeJsonDataToObject<T>(JsonData json_string) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json_string.ToJsonString());
        }

        public static string SerializeObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static JsonData SerializeObjectToJsonData(object obj)
        {
            return new JsonData(SerializeObjectToJson(obj));
        }
    }
}
