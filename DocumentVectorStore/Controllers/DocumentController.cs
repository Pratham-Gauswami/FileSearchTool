using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public DocumentController(ApplicationDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".txt", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
            {
                return BadRequest("File type not supported. Please upload PDF, TXT, DOC, or DOCX files.");
            }

            try
            {
                // Upload file to OpenAI
                string fileId;
                long fileSize;
                using (var stream = file.OpenReadStream())
                {
                    (fileId, fileSize) = await _openAIService.UploadFileAsync(stream, file.FileName);
                }

                // Create vector store
                var vectorStoreId = await _openAIService.CreateVectorStoreAsync($"Store_{file.FileName}");

                // Attach file to vector store
                await _openAIService.AttachFileToVectorStoreAsync(vectorStoreId, fileId);

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
                return StatusCode(500, $"Error uploading file: {ex.Message}");
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