using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace TcpServer
{
    class HumanModel
    {
        public static Dictionary<string, JsonData> HumanModeDic = new Dictionary<string, JsonData>();

        public static void SetHumaModeData(JsonData humanData)
        {
            var userID = humanData[Constant.USER_ID].ToString();
            HumanModeDic[userID] = humanData;
        }
        public static Dictionary<string, JsonData> GetHumaModeData()
        {
            return HumanModeDic;
        }
        public static void RemoveHumaModeData(string id)
        {
            HumanModeDic.Remove(id);
        }
    }
}
