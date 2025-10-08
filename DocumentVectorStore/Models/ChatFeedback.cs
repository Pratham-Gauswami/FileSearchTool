using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentVectorStore.Models
{
    [Table("Chat_Feedback")]
    public class ChatFeedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ThreadId { get; set; } = string.Empty;

        [Required]
        public string UserMessage { get; set; } = string.Empty;

        [Required]
        public string AssistantResponse { get; set; } = string.Empty;

        [Required]
        public bool IsHelpful { get; set; }

        public string? UserFeedbackNote { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation property
        [ForeignKey("DocumentId")]
        public DocumentStore? Document { get; set; }
    }

    [Table("Learning_Preferences")]
    public class LearningPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PreferenceType { get; set; } = string.Empty;

        [Required]
        public string PreferenceValue { get; set; } = string.Empty;

        public int Weight { get; set; } = 1;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("DocumentId")]
        public DocumentStore? Document { get; set; }
    }
}