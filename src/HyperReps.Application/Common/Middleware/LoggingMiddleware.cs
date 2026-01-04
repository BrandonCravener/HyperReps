using Microsoft.Extensions.Logging;
using Wolverine;

namespace HyperReps.Application.Common.Middleware
{
    public static class LoggingMiddleware
    {
        public static void Before(Envelope envelope, ILogger logger)
        {
            logger.LogInformation(
                "Executing {MessageType} [CorrelationId: {CorrelationId}]", 
                envelope.MessageType, 
                envelope.CorrelationId);
        }

        public static void After(Envelope envelope, ILogger logger)
        {
            logger.LogInformation(
                "Successfully executed {MessageType} [CorrelationId: {CorrelationId}]", 
                envelope.MessageType, 
                envelope.CorrelationId);
        }

        public static void OnException(Envelope envelope, Exception ex, ILogger logger)
        {
            logger.LogError(
                ex, 
                "Failed to execute {MessageType} [CorrelationId: {CorrelationId}]", 
                envelope.MessageType, 
                envelope.CorrelationId);
        }
    }
}