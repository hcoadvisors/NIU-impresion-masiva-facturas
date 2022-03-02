using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HCO.Wizard.Common
{
    public class JSerializer
    {


        public static string Serialize(object value)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            return JsonConvert.SerializeObject(value, settings);
        }

        public static T Deserialize<T>(string json)
        {
            var o = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd" });

            return o;
        }

        public static JObject ParseStrToJObj(string str)
        {
            return JObject.Parse(str);
        }

        public static T DeserializeFromFile<T>(string jsonFilePath, string parentProperty = null)
        {
            string jsonString = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF7);

            if (parentProperty != null)
            {
                JObject jObject = null;
                List<JToken> jTokenList = null;

                jObject = ParseStrToJObj(jsonString);


                jTokenList = jObject["value"].Select(t => t).ToList();
                string json = Serialize(jTokenList);

                var r = Deserialize<T>(json);

                return r;
            }
            else
            {
                var r = Deserialize<T>(jsonString);

                return r;
            }
        }


        public static void SerializeToFile(object obj, string jsonFilePath)
        {
            string json = Serialize(obj);

            File.WriteAllText(jsonFilePath, json);
        }

        //public static string Serialize(object obj)
        //{

        //    var options = new JsonSerializerOptions
        //    {
        //        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        //        WriteIndented = true
        //    };


        //    string jsonString = JsonSerializer.Serialize(obj, options);

        //    return jsonString;
        //}

        //public static TValue Deserialize<TValue>(string json)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        //        WriteIndented = true
        //    };

        //    return JsonSerializer.Deserialize<TValue>(json, options);
        //}

        //public static TValue DeserializeFromFile<TValue>(string jsonFilePath)
        //{
        //    string jsonString = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF7);

        //    var r = Deserialize<TValue>(jsonString);

        //    return r;
        //}


        //public static void SerializeToFile(object obj, string jsonFilePath)
        //{
        //    string json = Serialize(obj);

        //    File.WriteAllText(jsonFilePath, json);
        //}
    }
}
