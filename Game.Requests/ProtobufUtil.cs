using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;
using AsyncPrimitives;

namespace Game.Requests
{
    public class ProtobufUtil
    {
        public static TypeModel SerializationModel { get; set; }

        static readonly KeyedLazyInitializer<Type, bool> _isProtoContract
            = new KeyedLazyInitializer<Type, bool>(type =>
                Attribute.GetCustomAttributes(type, typeof(ProtoContractAttribute)).Any());
        public static bool IsProtoContract(Type type) => _isProtoContract[type];

        public static void ToProtobuf(Stream stream, object obj)
        {
            if (obj == null)
            {
                return;
            }
            var model = SerializationModel ?? RuntimeTypeModel.Default;
            model.Serialize(stream, obj);
        }

        public static object ObjectFromProtobuf(Stream stream, Type type)
        {
            var model = SerializationModel ?? RuntimeTypeModel.Default;
            return model.Deserialize(stream, null, type);
        }

        public static byte[] ToProtobuf(object obj)
        {
            using (var memStream = new MemoryStream())
            {
                ToProtobuf(memStream, obj);
                return memStream.ToArray();
            }
        }

        public static T ObjectFromProtobuf<T>(byte[] pbArr)
        {
            using (var memStream = new MemoryStream(pbArr))
            {
                return (T)ObjectFromProtobuf(memStream, typeof(T));
            }
        }
    }
}
