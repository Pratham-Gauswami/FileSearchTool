using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DocumentVectorStore.Data;
using DocumentVectorStore.Models;
using DocumentVectorStore.Services;
using Microsoft.EntityFrameworkCore;

namespace DocumentVectorStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IConfiguration _configuration;

        public DocumentController(
            ApplicationDbContext context, 
            OpenAIService openAIService,
            ILogger<DocumentController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _openAIService = openAIService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".txt", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
            {
                return BadRequest(new { error = "File type not supported. Please upload PDF, TXT, DOC, or DOCX files." });
            }

            // Check API key is configured
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("OpenAI API Key is not configured");
                return StatusCode(500, new { error = "OpenAI API Key is not configured in appsettings.json" });
            }

            try
            {
                _logger.LogInformation("Starting file upload process for: {FileName}", file.FileName);

                // Upload file to OpenAI
                string fileId;
                long fileSize;
                using (var stream = file.OpenReadStream())
                {
                    _logger.LogInformation("Uploading file to OpenAI...");
                    (fileId, fileSize) = await _openAIService.UploadFileAsync(stream, file.FileName);
                    _logger.LogInformation("File uploaded successfully. FileId: {FileId}", fileId);
                }

                // Create vector store
                _logger.LogInformation("Creating vector store...");
                var vectorStoreId = await _openAIService.CreateVectorStoreAsync($"Store_{file.FileName}");
                _logger.LogInformation("Vector store created. VectorStoreId: {VectorStoreId}", vectorStoreId);

                // Attach file to vector store
                _logger.LogInformation("Attaching file to vector store...");
                await _openAIService.AttachFileToVectorStoreAsync(vectorStoreId, fileId);
                _logger.LogInformation("File attached to vector store successfully");

                // Save to database
                var document = new DocumentStore
                {
                    VectorStoreId = vectorStoreId,
                    FileId = fileId,
                    FileName = file.FileName,
                    UploadDate = DateTime.UtcNow,
                    FileSize = fileSize
                };

                _context.DocumentStores.Add(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document saved to database with Id: {DocumentId}", document.Id);

                return Ok(new
                {
                    message = "File uploaded successfully",
                    documentId = document.Id,
                    vectorStoreId = vectorStoreId,
                    fileId = fileId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {Message}", ex.Message);
                return StatusCode(500, new 
                { 
                    error = ex.Message,
                    detail = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetDocuments()
        {
            var documents = await _context.DocumentStores.ToListAsync();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(int id)
        {
            var document = await _context.DocumentStores.FindAsync(id);
            
            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }
    }
}