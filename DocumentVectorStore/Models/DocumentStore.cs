using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentVectorStore.Models
{
    [Table("Document_Store")]
    public class DocumentStore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string VectorStoreId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileId { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public long FileSize { get; set; }
    }
}