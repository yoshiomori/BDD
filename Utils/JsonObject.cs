using System;

namespace Utils
{
    public class JsonObject
    {
        public dynamic Value { get; set; }

        public JsonObject(string json)
        {
            try
            {
                Value = JsonConvert.DeserializeObject<ExpandoObject>(json);
            }
            catch (System.InvalidCastException)
            {
                Value = JsonConvert.DeserializeObject(json);
            }
        }



        public bool IsPropertyExist(string name)
        {
            if (Value is ExpandoObject)
                return ((IDictionary<string, object>)Value).ContainsKey(name);

            return Value.GetType().GetProperty(name) != null;
        }
    }
}
