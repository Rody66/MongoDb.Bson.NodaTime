using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NodaTime;
using NodaTime.Text;
using System;
using NodaTime.Intervals;

namespace MongoDb.Bson.NodaTime
{
	public class TimeIntervalBsonSerializer : SerializerBase<TimeInterval>
	{
		public override TimeInterval Deserialize(
			BsonDeserializationContext context,
			BsonDeserializationArgs args)
		{
			BsonType currentBsonType = context.Reader.GetCurrentBsonType();
			switch (currentBsonType)
			{
				case BsonType.String:
					var sInterval = context.Reader.ReadString();
					return TimeInterval.Parse(sInterval);
				case BsonType.Null:
					throw new InvalidOperationException("TimeInterval is a value type, but the BsonValue is null.");
				default:
					throw new NotSupportedException($"Cannot convert a {(object) currentBsonType} to an TimeInterval.");
			}
		}

		public override void Serialize(
			BsonSerializationContext context,
			BsonSerializationArgs args,
			TimeInterval value)
		{
			
			context.Writer.WriteString(value.ToString());
		}
	}
}
