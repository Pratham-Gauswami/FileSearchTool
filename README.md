# 🤖 AI Document Intelligence Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![OpenAI](https://img.shields.io/badge/OpenAI-412991?style=for-the-badge&logo=openai&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

**Transform your documents into intelligent conversations**

[Features](#-features) • [Demo](#-demo) • [Quick Start](#-quick-start) • [Documentation](#-documentation) • [Contributing](#-contributing)

</div>

---

## 🌟 Overview

AI Document Intelligence Platform is a cutting-edge web application that revolutionizes how you interact with your documents. Upload any document, and watch as advanced AI transforms it into an intelligent knowledge base you can chat with naturally.

Built with modern .NET technologies and powered by OpenAI's state-of-the-art language models, this platform provides seamless document vectorization, persistent storage, and contextual AI conversations.

### 💡 Why This Project?

- **🎯 Instant Knowledge Access**: No more scrolling through lengthy PDFs. Ask questions and get precise answers.
- **🔍 Smart Search**: Leverages OpenAI's vector search to find relevant information across all your documents.
- **💬 Natural Conversations**: Chat with your documents as if you're talking to an expert who has read them all.
- **🔒 Persistent Memory**: Your document knowledge base is saved and ready whenever you need it.

---

## ✨ Features

### 🚀 Core Capabilities

- **📤 Drag & Drop Upload**: Intuitive interface for uploading PDF, TXT, DOC, and DOCX files
- **🧠 AI-Powered Vectorization**: Automatic document processing using OpenAI's embedding models
- **💾 Smart Storage**: Persistent database tracking of all documents and vector stores
- **🤖 Intelligent Chat**: Context-aware conversations powered by GPT-4
- **🔗 Thread Management**: Maintains conversation context across multiple interactions
- **📊 Document Library**: Easy-to-navigate list of all uploaded documents

### 🎨 User Experience

- **Modern UI/UX**: Beautiful gradient design with smooth animations
- **Responsive Layout**: Works flawlessly on desktop, tablet, and mobile
- **Real-time Feedback**: Live status updates during upload and processing
- **Drag & Drop**: Effortless file uploads with visual feedback

### 🔧 Technical Excellence

- **RESTful API**: Clean, well-documented endpoints
- **Entity Framework Core**: Robust database management
- **Async/Await**: Non-blocking operations for optimal performance
- **Error Handling**: Comprehensive error management and user feedback
- **Scalable Architecture**: Built with enterprise-grade patterns

---

## 🎬 Demo

### Document Upload Flow
```
📄 Select Document → 🔄 Upload & Vectorize → 💾 Store in Database → ✅ Ready to Chat
```

### Chat Experience
```
👤 User: "What are the main topics discussed in this document?"
🤖 AI: "Based on the document, the main topics include..."
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or Express)
- [OpenAI API Key](https://platform.openai.com/api-keys)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/DocumentVectorStore.git
   cd DocumentVectorStore
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure your settings**
   
   Update `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your-openai-api-key-here"
     },
     "ConnectionStrings": {
       "DefaultConnection": "your-connection-string-here"
     }
   }
   ```

4. **Create the database**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Open your browser**
   ```
   Navigate to: https://localhost:5001
   ```

---

## 📁 Project Structure

```
DocumentVectorStore/
├── 📂 Controllers/          # API endpoints
│   ├── DocumentController.cs
│   └── ChatController.cs
├── 📂 Data/                 # Database context
│   └── ApplicationDbContext.cs
├── 📂 Models/               # Data models
│   └── DocumentStore.cs
├── 📂 Services/             # Business logic
│   └── OpenAIService.cs
├── 📂 Migrations/           # EF Core migrations
├── 📂 wwwroot/              # Static files
│   └── index.html
├── 📄 Program.cs            # App configuration
└── 📄 appsettings.json      # Settings
```

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **API**: RESTful architecture

### AI & ML
- **OpenAI API**: GPT-4 & Embeddings
- **Vector Search**: File Search Tool
- **Assistants API**: Contextual conversations

### Frontend
- **HTML5**: Semantic markup
- **CSS3**: Modern styling with gradients & animations
- **JavaScript**: Vanilla ES6+
- **Design**: Responsive & mobile-first

---

## 📚 Documentation

### API Endpoints

#### Document Management

**Upload Document**
```http
POST /api/document/upload
Content-Type: multipart/form-data

Returns: 
{
  "message": "File uploaded successfully",
  "documentId": 1,
  "vectorStoreId": "vs_xxx",
  "fileId": "file_xxx"
}
```

**List Documents**
```http
GET /api/document/list

Returns: Array of document objects
```

**Get Document**
```http
GET /api/document/{id}

Returns: Single document object
```

#### Chat

**Send Message**
```http
POST /api/chat/message
Content-Type: application/json

Body:
{
  "documentId": 1,
  "threadId": "thread_xxx",  // optional
  "message": "Your question here"
}

Returns:
{
  "threadId": "thread_xxx",
  "response": "AI response",
  "assistantId": "asst_xxx"
}
```

---

## 🔐 Security Best Practices

⚠️ **Important**: This is a demonstration project. For production deployment:

- ✅ Store API keys in environment variables or secure vaults (Azure Key Vault, AWS Secrets Manager)
- ✅ Implement authentication & authorization (JWT, OAuth 2.0)
- ✅ Add rate limiting to prevent abuse
- ✅ Validate and sanitize all user inputs
- ✅ Use HTTPS in production
- ✅ Implement proper CORS policies
- ✅ Add comprehensive logging and monitoring
- ✅ Set file upload size limits
- ✅ Scan uploaded files for malware

---

## 🎯 Roadmap

- [ ] Multi-document conversations
- [ ] Document comparison features
- [ ] Advanced search filters
- [ ] User authentication system
- [ ] Document sharing capabilities
- [ ] Export conversation history
- [ ] Support for more file formats
- [ ] Cloud storage integration
- [ ] Docker containerization
- [ ] Kubernetes deployment configs

---

## 🤝 Contributing

Contributions are what make the open-source community amazing! Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Write meaningful commit messages
- Add tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Pratham Gauswami**

- GitHub: [@prathamgauswami](https://github.com/Pratham-Gauswami)
- LinkedIn: [Pratham Gauswami](https://www.linkedin.com/in/pratham-goswami-331aa7240/)

---

## 🙏 Acknowledgments

- [OpenAI](https://openai.com/) for their incredible API
- [Microsoft](https://microsoft.com/) for the .NET platform
- The open-source community for continuous inspiration

---

## 📞 Support

If you found this project helpful, please give it a ⭐️!

For questions or issues, please open an issue on GitHub.

---

<div align="center">

**Made with ❤️ and ☕**

[⬆ Back to Top](#-ai-document-intelligence-platform)

</div>
