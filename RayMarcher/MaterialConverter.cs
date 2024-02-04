using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RayMarcher.Shading;

namespace RayMarcher
{
  internal class MaterialConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Material);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var jsonObject = JObject.Load(reader);
      MaterialType type = jsonObject["Type"].ToObject<MaterialType>();
      switch (type)
      {
        case MaterialType.Phong:
          return jsonObject.ToObject<PhongMaterial>(serializer);
        case MaterialType.PBR:
          return jsonObject.ToObject<PBRMaterial>(serializer);
        default:
          throw new JsonSerializationException("Unknown MaterialType");
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
    }

    public override bool CanWrite => false;
  }
}
