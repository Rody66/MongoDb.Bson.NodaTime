using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NodaTime;
using NodaTime.Text;
using System;

namespace MongoDb.Bson.NodaTime
{
	public class LocalDateSerializer : SerializerBase<LocalDate>
	{
		public override LocalDate Deserialize(
			BsonDeserializationContext context,
			BsonDeserializationArgs args)
		{
			BsonType currentBsonType = context.Reader.GetCurrentBsonType();
			switch (currentBsonType)
			{
				case BsonType.String:
					return LocalDatePattern.Iso.Parse(context.Reader.ReadString()).Value;
				case BsonType.DateTime:
					var unixTimeMs = context.Reader.ReadDateTime();
					var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMs);
					return new LocalDate(dateTime.Year, dateTime.Month, dateTime.Day);
				case BsonType.Null:
					throw new InvalidOperationException("LocalDate is a value type, but the BsonValue is null.");
				default:
					throw new NotSupportedException($"Cannot convert a {(object) currentBsonType} to an LocalDate.");
			}
		}

		public override void Serialize(
			BsonSerializationContext context,
			BsonSerializationArgs args,
			LocalDate value)
		{
			var instant = Instant.FromUtc(value.Year, value.Month, value.Day, 0, 0, 0);
			context.Writer.WriteDateTime(instant.ToUnixTimeMilliseconds());
		}
	}
}