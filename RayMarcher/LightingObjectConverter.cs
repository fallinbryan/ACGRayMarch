using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RayMarcher
{
  internal class LightingObjectConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      bool debughere = true;
      return objectType == typeof(Lighting.ILightSource);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      JArray jArray = JArray.Load(reader);
      List<Lighting.ILightSource> lightSources = new List<Lighting.ILightSource>();

      foreach(JObject jObject in jArray)
      {
        Lighting.LightType type = jObject["Type"].ToObject<Lighting.LightType>();
        switch (type)
        {
          case Lighting.LightType.Directional:
            lightSources.Add(jObject.ToObject<Lighting.SunLamp>(serializer));
            break;
          default:
            throw new JsonSerializationException("Unknown LightType");
        }
      }


     return lightSources;
      
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
    }

    public override bool CanWrite => false;
  }
}
