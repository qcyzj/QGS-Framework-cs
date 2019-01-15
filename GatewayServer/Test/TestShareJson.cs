using Newtonsoft.Json.Linq;

using Share.Json;

namespace GatewayServer.Test
{
    public class TestShareJson
    {
        private class Product
        {
            public string Name;
            public int Price;
        }


        private Product m_Product;


        public TestShareJson()
        {
            m_Product = new Product();
            m_Product.Name = "Apple";
            m_Product.Price = 100;
        }


        public void RunAllTest()
        {
            Test_Share_JsonData();

            Test_Share_JsonHelper();
        }


        private void Test_Share_JsonData()
        {
            string Key_Name = "Name";
            string Key_Price = "Price";
            string Key_List = "TL";

            string Value_Name = "Apple";
            int Value_Price = 100;
            string List_Value_1 = "1";
            string List_Value_2 = "2";
            string List_Value_3 = "3";

            JsonData test_json = new JsonData();
            test_json[Key_Name] = Value_Name;
            test_json[Key_Price] = Value_Price;

            JsonArray test_array = new JsonArray();
            test_array.Add(List_Value_1);
            test_array.Add(List_Value_2);
            test_array.Add(List_Value_3);
            test_json[Key_List] = test_array.Root;

            CAssert.AreEqual((string)test_json[Key_Name], Value_Name);
            CAssert.AreEqual((int)test_json[Key_Price], Value_Price);

            CAssert.AreEqual(JTokenType.Array, test_json[Key_List].Type);

            JsonArray json_array = new JsonArray(test_json[Key_List]);
            CAssert.AreEqual((string)json_array[0], List_Value_1);
            CAssert.AreEqual((string)json_array[1], List_Value_2);
            CAssert.AreEqual((string)json_array[2], List_Value_3);

            string json_string = test_json.ToJsonString();
            JsonData test_two_json = new JsonData(json_string);
            CAssert.AreEqual(test_two_json[Key_Name], test_json[Key_Name]);
            CAssert.AreEqual(test_two_json[Key_Price], test_json[Key_Price]);
            CAssert.AreEqual(JTokenType.Array, test_two_json[Key_List].Type);

            JsonArray test_two_array = new JsonArray(test_two_json[Key_List]);
            CAssert.AreEqual((string)test_two_array[0], List_Value_1);
            CAssert.AreEqual((string)test_two_array[1], List_Value_2);
            CAssert.AreEqual((string)test_two_array[2], List_Value_3);
        }

        private void Test_Share_JsonHelper()
        {
            string product_json_string = JsonHelper.SerializeObjectToJson(m_Product);
            Product product_two = JsonHelper.DeserializeJsonToObject<Product>(product_json_string);
            CAssert.AreEqual(m_Product.Name, product_two.Name);
            CAssert.AreEqual(m_Product.Price, product_two.Price);

            JsonData product_jsondata = JsonHelper.SerializeObjectToJsonData(m_Product);
            Product product_three = JsonHelper.DeserializeJsonDataToObject<Product>(product_jsondata);
            CAssert.AreEqual(m_Product.Name, product_three.Name);
            CAssert.AreEqual(m_Product.Price, product_three.Price);
        }
    }
}
