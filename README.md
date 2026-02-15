# TransGo Alpha — Architecture Overview

## System Summary
**TransGo Alpha** is a hybrid full-stack .NET solution that unifies web, desktop, and server environments into one modular ecosystem. It showcases real-time communication, consistent shared data models, and clean multi-project design.

**Please keep in mind this is one of many iterations, and the primary focus of this one was to get the technical side working. The designs were upgraded in a later version.**

### Architecture Layers
- **Server (ASP.NET Core Web API + SignalR):** Handles REST endpoints for users, courses, and lectures, and provides real-time streaming through SignalR.
- **Client (Blazor WebAssembly):** A single-page web app using Razor components for login, registration, transcript viewing, and course management.
- **DesktopClient (WPF):** A Windows desktop application mirroring course and transcript functionality for offline or instructor use.
- **Shared (Class Library):** Contains shared models (`User`, `Course`, `Lecture`) referenced by all other projects to maintain type safety and serialization consistency.

---

## Data Architecture
**Database:** SQL Server (database: `tgAlpha`)

### Entity Framework Core Contexts
| Context | Table | Entity |
|----------|--------|---------|
| `UserDbContext` | `tblUsers` | `User` |
| `CourseDbContext` | `tblCourses` | `Course` |
| `LectureDbContext` | `tblLectures` | `Lecture` |

### Shared Models
| Entity | Key Fields |
|---------|-------------|
| `User` | Id, Email, Firstname, Lastname, Password, Roles |
| `Course` | Id, CourseNum, CourseName, Semester, Instructors, Students, Transcripts |
| `Lecture` | Id, HostId, CourseId, DateOf, TimeStart, TimeEnd, Transcript, IsComplete |

> *Note:* Relationships are stored as delimited strings (e.g., course lists), which works for prototypes but could be normalized for production scalability.

---

## API Surface
### UserController
Handles authentication and profile operations.
```http
POST   /api/users/adduser       → Create new user
GET    /api/users/{email}/{pw}  → Verify credentials
GET    /api/users/profile/{id}  → Retrieve user profile
GET    /api/users/studentemail/{email} → Lookup user by email
PUT    /api/users/updatecourses/{id}   → Update course list
DELETE /api/users/{id}          → Delete user
```

### CourseController
Manages course CRUD operations and enrollment.
```http
GET    /api/courses/{id}       → Get course details
GET    /api/courses/check/{id} → Verify course exists
POST   /api/courses            → Add new course
PUT    /api/courses/updatestudents/{id} → Update student roster
PUT    /api/courses/removestudent/{id}  → Remove student
```

### LectureController
```http
GET /api/lectures/{lectureid} → Fetch single lecture details
```

### SignalR ChatHub
```csharp
AddClass(groupName)          // Join transcript group
RemoveFromGroup(groupName)   // Leave group
SendGroupMessage(group, isFinal, msg)  // Broadcast to course group
SendMessage(isFinal, msg)    // Broadcast to all clients
```

---

## Client (Blazor WebAssembly)
Implements a responsive SPA built with Razor components.

**Pages:**
- `Home.razor` — Landing
- `CourseAdminPage.razor` — Instructor view (manage students & transcripts)
- `CoursePage.razor` — Displays lectures and progress per course
- `TranscriptPage.razor` — Live transcript editing with SignalR
- `Profile.razor` — User profile management

**Components:**
- `AddLoginDialog`, `AddRegistrationDialog`, `AddTeachRegDialog`, etc. — interactive modals using EditForm binding and server calls.


---

## DesktopClient (WPF)
A lightweight Windows desktop interface for the same course and lecture data.
- Pages: `MainWindow`, `Home`, `ClassPage`, `TranscriptPage`
- Data layer: Local EF contexts (`CourseDbContext`, `LectureDbContext`) using same schema.
- Intended for local/teacher-side use cases.

---

## Design Highlights
  **Cross-platform architecture:** Blazor for web, WPF for desktop, shared C# models.  
  **SignalR integration:** Real-time text streaming for lectures/transcripts.  
  **EF Core ORM:** Simplifies SQL access while maintaining code-first schema parity.  
  **Reusable Shared Library:** Enables identical domain types across all layers.  
  **Clean project separation:** Each layer compiled and maintained independently.

---

