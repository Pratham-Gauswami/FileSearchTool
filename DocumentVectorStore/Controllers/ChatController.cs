using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentVectorStore.Data;
using DocumentVectorStore.Services;

namespace DocumentVectorStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;

        public ChatController(ApplicationDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is null" });
            }

            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            if (request.DocumentId <= 0)
            {
                return BadRequest(new { error = "Invalid DocumentId" });
            }

            try
            {
                // Get the document to determine vector store
                var document = await _context.DocumentStores
                    .FirstOrDefaultAsync(d => d.Id == request.DocumentId);

                if (document == null)
                {
                    return NotFound(new { error = $"Document with ID {request.DocumentId} not found" });
                }

                // Create assistant with the vector store
                var assistantId = await _openAIService.CreateAssistantAsync(document.VectorStoreId);

                // Create or use existing thread
                string threadId;
                if (string.IsNullOrEmpty(request.ThreadId))
                {
                    threadId = await _openAIService.CreateThreadAsync();
                }
                else
                {
                    threadId = request.ThreadId;
                }

                // Add message to thread
                await _openAIService.AddMessageToThreadAsync(threadId, request.Message);

                // Run assistant
                var runId = await _openAIService.RunAssistantAsync(threadId, assistantId);

                // Wait for completion
                string status;
                int maxAttempts = 30;
                int attempts = 0;
                do
                {
                    await Task.Delay(1000); // Wait 1 second
                    status = await _openAIService.GetRunStatusAsync(threadId, runId);
                    attempts++;
                } while (status != "completed" && status != "failed" && attempts < maxAttempts);

                if (status == "completed")
                {
                    // Get the response
                    var response = await _openAIService.GetLatestMessageAsync(threadId);
                    
                    return Ok(new
                    {
                        threadId = threadId,
                        response = response,
                        assistantId = assistantId
                    });
                }
                else
                {
                    return StatusCode(500, new { error = $"Failed to get response from assistant. Status: {status}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    error = ex.Message,
                    detail = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }

    public class ChatRequest
    {
        public int DocumentId { get; set; }
        public string? ThreadId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}