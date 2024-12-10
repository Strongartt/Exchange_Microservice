using Exchange.Models.Models;
using Exchange.Producer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProducerController: ControllerBase
{
    private readonly GestorSQS_Service _sqsService;
    
    public ProducerController(GestorSQS_Service sqsService)
    {
        _sqsService = sqsService;
    }
    
    [HttpPost]
    public async Task<IActionResult> SendEmailToQueue([FromBody] Mensaje emailMessage)
    {
        if (emailMessage == null || string.IsNullOrEmpty(emailMessage.Subject) ||
            string.IsNullOrEmpty(emailMessage.Body) || string.IsNullOrEmpty(emailMessage.Recipient))
        {
            return BadRequest("Subject, Body, and Recipient are required");
        }

        await _sqsService.SendMessageAsync(emailMessage.Subject, emailMessage.Body, emailMessage.Recipient);
        return Ok("Message sent to queue successfully.");
    }
}