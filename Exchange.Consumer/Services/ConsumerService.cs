using System.Net;
using System.Net.Mail;
using Amazon.SQS;
using Amazon.SQS.Model;
using Exchange.Models.Models;
using Newtonsoft.Json;

namespace Exchange.Consumer.Services;

public class ConsumerService: BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/093130468022/Gestor_Colas";
    
    public ConsumerService(IAmazonSQS sqs)
    {
        _sqs = sqs;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Recibe el mensaje de la cola
            var messages = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest()
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 5 // Espera de 5 segundos para revisión continua
            });

            foreach (var message in messages.Messages)
            {
                try
                {
                    // Deserializa el mensaje recibido de la cola
                    var emailMessage = JsonConvert.DeserializeObject<Mensaje>(message.Body);

                    // Envía el correo
                    await SendEmailAsync(emailMessage.Subject, emailMessage.Body, emailMessage.Recipient);

                    // Elimina el mensaje de la cola después de procesarlo correctamente
                    await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);
                }
                catch (Exception ex)
                {
                    // Manejo de errores (puedes loggear el error o manejar de otra manera)
                    Console.WriteLine($"Error al procesar el mensaje: {ex.Message}");
                }
            }
        }
    }
    
    private async Task SendEmailAsync(string subject, string body, string recipient)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("strongartt@gmail.com", "tufn eami jyyn xlqw"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("strongartt@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = false,
        };
        mailMessage.To.Add(recipient);

        await smtpClient.SendMailAsync(mailMessage);
    }
}