using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DabSquadBot
{
    //Hidden data class, used for Global vars
    public class Data
    {
        public List<string> DabImages { get; set; }
    }

    public class JSONHelper
    {
        //Write vars to json file
        public void JsonSerialize(Data hiddenData)
        {
            File.WriteAllText(Global.filePath + "Data.json", JsonConvert.SerializeObject(hiddenData, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter()));
        }

        //Read vars from Json file
        public Data JsonDeserialize()
        {
            return JsonConvert.DeserializeObject<Data>(File.ReadAllText(Global.filePath + "Data.json"), new Newtonsoft.Json.Converters.StringEnumConverter());
        }
    }
}
