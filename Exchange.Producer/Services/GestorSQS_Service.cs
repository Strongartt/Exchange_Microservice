using Amazon.SQS;
using Amazon.SQS.Model;
using Exchange.Models.Models;
using Newtonsoft.Json;

namespace Exchange.Producer.Services;

public class GestorSQS_Service
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/093130468022/Gestor_Colas";
    
    public GestorSQS_Service(IAmazonSQS sqs)
    {
        _sqs = sqs;
    }
    
    public async Task SendMessageAsync(string subject, string body, string recipient)
    {
        var emailMessage = new Mensaje()
        {
            Subject = subject,
            Body = body,
            Recipient = recipient
        };

        var messageBody = JsonConvert.SerializeObject(emailMessage);

        var request = new SendMessageRequest()
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };

        await _sqs.SendMessageAsync(request);
    }
}