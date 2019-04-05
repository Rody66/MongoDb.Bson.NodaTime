using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NodaTime;
using NodaTime.Text;

namespace MongoDb.Bson.NodaTime
{
    public class LocalDateTimeSerializer : SerializerBase<LocalDateTime>
    {
        public override LocalDateTime Deserialize(
            BsonDeserializationContext context,
            BsonDeserializationArgs args)
        {
            BsonType currentBsonType = context.Reader.GetCurrentBsonType();
            switch (currentBsonType)
            {
                case BsonType.String:
                    return LocalDateTimePattern.ExtendedIso.Parse(context.Reader.ReadString()).Value;
                case BsonType.DateTime:
                    var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(context.Reader.ReadDateTime());
                    return LocalDateTime.FromDateTime(dateTime.DateTime);
                case BsonType.Null:
                    throw new InvalidOperationException("LocalDateTime is a value type, but the BsonValue is null.");
                default:
                    throw new NotSupportedException($"Cannot convert a {(object) currentBsonType} to an LocalDateTime.");
            }
        }

        public override void Serialize(
            BsonSerializationContext context,
            BsonSerializationArgs args,
            LocalDateTime value)
        {
            var instant = Instant.FromUtc(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            context.Writer.WriteDateTime(instant.ToUnixTimeMilliseconds());
        }
    }
}