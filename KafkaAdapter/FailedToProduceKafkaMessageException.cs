using System;
using Confluent.Kafka;

namespace KafkaAdapter
{
    internal sealed class FailedToProduceKafkaMessageException : Exception
    {
        public DeliveryReport<string, string> DeliveryReport { get; }

        public FailedToProduceKafkaMessageException(DeliveryReport<string, string> deliveryReport)
            : base($"{deliveryReport.Error.Code}: {deliveryReport.Error.Reason}")
        {
            DeliveryReport = deliveryReport;
        }
    }
}