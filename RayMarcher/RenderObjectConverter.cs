using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RayMarcher.Renderable;

namespace RayMarcher
{
  public class RenderObjectConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(IRenderObject);
    }


    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var jar = JArray.Load(reader);

      List<IRenderObject> renderObjects = new List<IRenderObject>();

      foreach(var jsonObject in jar) { 
        RenderObjectType type = jsonObject["Type"].ToObject<RenderObjectType>();
        switch (type)
        {
          case RenderObjectType.Sphere:
            renderObjects.Add(jsonObject.ToObject<Sphere>(serializer));
            break;
          case RenderObjectType.Plane:
            renderObjects.Add(jsonObject.ToObject<Plane>(serializer));
            break;
          case RenderObjectType.Triangle:
            renderObjects.Add(jsonObject.ToObject<Tri>(serializer));
            break;
          default:
            throw new JsonSerializationException("Unknown RenderObjectType");
        }
      }
      return renderObjects;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}
