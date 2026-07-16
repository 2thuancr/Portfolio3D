PORTFOLIO 3D

Software Design & Delivery Blueprint

Angular + Three.js + ASP.NET Core + ABP Framework

| Mục tiêu: Xây dựng một portfolio 3D có trải nghiệm camera dẫn hướng, tải nhanh, dễ bảo trì và đủ chuyên nghiệp để giới thiệu năng lực kỹ thuật với nhà tuyển dụng. |
| --- |

Chủ sở hữu: Vi Quốc Thuận
Phiên bản tài liệu: 1.1
Ngày lập: 16/07/2026

Trạng thái: APPROVED FOR MVP IMPLEMENTATION

# 0. Kiểm soát tài liệu

| Thuộc tính | Giá trị |
| --- | --- |
| Tên tài liệu | Portfolio 3D — Software Design & Delivery Blueprint |
| Mục đích | Tài liệu nguồn để thiết kế, chia task, phát triển, kiểm thử và triển khai MVP |
| Phạm vi | Website portfolio cá nhân 3D + API quản trị nội dung |
| Kiến trúc mục tiêu | ABP Layered Monolith + Angular SPA |
| Đối tượng đọc | Developer, UI/3D Designer, Tester, DevOps và người bảo trì sau này |
| Nguyên tắc | Simple first; measurable performance; no speculative abstraction |
| Revision 1.1 | Bổ sung timeline, reorder, contact notification behavior, SEO decision và hosting budget. |

## 0.1 Quy ước ưu tiên

- MUST: bắt buộc có trong MVP.

- SHOULD: nên có nếu không ảnh hưởng timeline.

- COULD: để sau MVP.

- OUT: cố ý không làm trong giai đoạn hiện tại.

## 0.2 Nguyên tắc kiến trúc

- Monolith trước, module hóa vừa đủ; không chọn microservice cho portfolio cá nhân.

- Tách rõ UI 3D, nghiệp vụ ứng dụng và persistence nhưng không tạo lớp chỉ để chuyển tiếp dữ liệu.

- Ưu tiên convention có sẵn của ABP thay vì tự tạo framework nội bộ.

- Chỉ thêm hạ tầng mới khi có một yêu cầu đã đo được, không thêm vì 'có thể sau này cần'.

- Mọi quyết định quan trọng phải đi kèm lý do, tác động và điều kiện xem xét lại.

## 0.3 Mục lục

1. 1. Tóm tắt điều hành

1. 2. Bối cảnh và mục tiêu

1. 3. Phạm vi sản phẩm

1. 4. Personas và user journey

1. 5. Yêu cầu chức năng

1. 6. Yêu cầu phi chức năng

1. 7. UX 3D và storyboard

1. 8. Kiến trúc hệ thống

1. 9. Thiết kế backend theo ABP

1. 10. Thiết kế frontend và 3D engine

1. 11. Mô hình dữ liệu

1. 12. API contract

1. 13. Bảo mật

1. 14. Hiệu năng 3D

1. 15. Logging và observability

1. 16. Testing strategy

1. 17. Coding standards

1. 18. Git và CI/CD

1. 19. Kế hoạch triển khai

1. 20. Backlog MVP

1. 21. Rủi ro

1. 22. Definition of Done

1. 23. Tài liệu tham khảo

# 1. Tóm tắt điều hành

Dự án xây dựng một portfolio cá nhân dạng không gian 3D. Người xem bắt đầu ở bên ngoài một không gian công nghệ, sau đó camera di chuyển có dẫn hướng vào phòng làm việc. Các vật thể trong phòng đại diện cho About, Skills, Projects và Contact. Website phải tạo ấn tượng nhưng không được đánh đổi khả năng truy cập, tốc độ tải và khả năng xem nội dung trên thiết bị yếu.

| Quyết định kỹ thuật: Backend dùng ABP Layered Monolith trên ASP.NET Core; frontend dùng Angular và Three.js. Nội dung portfolio được quản trị từ backend; hành vi camera, render loop và tương tác vật thể nằm hoàn toàn ở frontend. |
| --- |

## 1.1 Kiến trúc được chọn

| Browser ├── Angular UI │   ├── Portfolio pages │   ├── Three.js scene │   ├── GSAP camera timeline │   └── Progressive fallback (2D) │ └── HTTPS / REST     └── ASP.NET Core + ABP         ├── HttpApi.Host         ├── Application         ├── Application.Contracts         ├── Domain         ├── Domain.Shared         └── EntityFrameworkCore → PostgreSQL |
| --- |

## 1.2 Những thứ cố ý không dùng trong MVP

| Hạng mục | Quyết định | Lý do |
| --- | --- | --- |
| Microservices | OUT | Không có quy mô tổ chức hoặc tải để bù chi phí vận hành. |
| Multi-tenancy | OUT | Portfolio chỉ có một chủ sở hữu. |
| Redis | OUT | Dữ liệu ít; browser/CDN cache và in-memory cache là đủ. |
| Distributed event bus | OUT | Không có bounded context độc lập cần giao tiếp bất đồng bộ. |
| Hangfire | OUT mặc định | Chỉ cân nhắc nếu cần gửi email retry hoặc lịch tác vụ. |
| Elasticsearch | OUT | Danh sách dự án nhỏ; PostgreSQL search hoặc lọc phía client đủ dùng. |
| CQRS/MediatR toàn dự án | OUT | Tạo thêm indirection không cần thiết cho CRUD nhỏ. |

# 2. Bối cảnh và mục tiêu

## 2.1 Vấn đề cần giải quyết

- Portfolio 2D phổ biến khó tạo khác biệt.

- Portfolio 3D thường gặp vấn đề tải chậm, camera gây khó chịu và nội dung bị phụ thuộc vào WebGL.

- Thông tin dự án thường hard-code nên việc cập nhật phải build lại toàn bộ frontend.

- Code 3D dễ trở thành một component khổng lồ, khó test và khó thay đổi.

## 2.2 Mục tiêu sản phẩm

| ID | Mục tiêu | Chỉ số chấp nhận |
| --- | --- | --- |
| G-01 | Tạo trải nghiệm mở đầu ấn tượng | Người xem hiểu cách khám phá trong ≤ 5 giây. |
| G-02 | Trình bày năng lực kỹ thuật rõ ràng | Projects, Skills và Contact truy cập không quá 2 thao tác. |
| G-03 | Hoạt động trên máy yếu/mobile | Có chế độ 2D fallback và nút giảm hiệu ứng. |
| G-04 | Dễ cập nhật nội dung | Project/Skill/Profile được chỉnh từ Admin, không sửa source frontend. |
| G-05 | Dễ maintain | Mỗi service/component có một trách nhiệm; không có God component. |

## 2.3 Tiêu chí thành công MVP

- Luồng loading → enter → camera vào phòng → mở Projects hoạt động ổn định.

- Có ít nhất 3 khu vực tương tác: About, Projects, Contact.

- Admin có thể CRUD Project và Skill.

- Lighthouse Performance desktop mục tiêu ≥ 80; Accessibility mục tiêu ≥ 90.

- Initial 3D payload mục tiêu ≤ 8 MB sau nén; thời gian interactive trên mạng phổ thông mục tiêu ≤ 5 giây.

- Có unit test cho nghiệp vụ trọng yếu và integration test cho API CRUD chính.

# 3. Phạm vi sản phẩm

## 3.1 MVP

| Nhóm | Chức năng MUST |
| --- | --- |
| Public | Landing/loading, chế độ 3D, điều hướng nội dung, About, Skills, Projects, Contact, CV link. |
| 3D | Load GLB, camera path, hover/click object, project monitor interaction, reduced motion. |
| Admin | Đăng nhập, CRUD Profile, Project, Skill; bật/tắt hiển thị; sắp xếp thứ tự. |
| Backend | REST API, authorization, validation, persistence, audit fields, error response. |
| Deployment | Docker, HTTPS, environment configuration, database migration. |

## 3.2 Sau MVP

- Project detail 3D riêng cho từng dự án.

- Âm thanh môi trường có opt-in.

- Blog hoặc technical notes.

- Analytics dashboard riêng.

- Contact email background job có retry.

- CMS/media library nâng cao.

## 3.3 Ngoài phạm vi

- Mạng xã hội, chat realtime, marketplace hoặc tính năng nhiều người dùng.

- Ứng dụng mobile native.

- Editor kéo-thả bố cục 3D từ trang admin.

- Cho admin upload model 3D tùy ý trong MVP.

# 4. Personas và user journey

| Persona | Nhu cầu | Hành vi mong muốn |
| --- | --- | --- |
| Nhà tuyển dụng | Đánh giá nhanh kỹ năng và dự án | Bỏ qua intro nếu cần, mở CV/Projects nhanh. |
| Tech Lead | Xem chất lượng kỹ thuật | Đọc vai trò, kiến trúc, công nghệ, repository. |
| Khách hàng | Xem sản phẩm đã làm và liên hệ | Xem demo, hiểu dịch vụ, gửi contact. |
| Chủ sở hữu/Admin | Cập nhật portfolio | CRUD nội dung đơn giản, không đụng code. |

## 4.1 User journey chính

| Visit   → Loading assets   → Display "Enter Workspace"   → User enters   → Guided camera moves inside   → Main room becomes interactive   ├── About object → profile panel   ├── Projects monitor → project gallery   ├── Skills wall → skill categories   └── Contact device → contact form   → External demo / GitHub / CV |
| --- |

## 4.2 Luồng thay thế

- WebGL không khả dụng → chuyển thẳng sang giao diện 2D.

- Thiết bị yếu → bật Low Quality, tắt shadow và post-processing.

- prefers-reduced-motion → không auto-fly dài; dùng chuyển cảnh ngắn hoặc nội dung 2D.

- Model lỗi tải → hiển thị fallback background và menu nội dung.

- Người dùng bấm Skip Intro → camera đặt ngay ở Main Room.

# 5. Yêu cầu chức năng

| ID | Yêu cầu | Actor | Priority |
| --- | --- | --- | --- |
| FR-01 | Load portfolio configuration | Public | MUST |
| FR-02 | Hiển thị tiến trình tải asset | Public | MUST |
| FR-03 | Enter/Skip intro | Public | MUST |
| FR-04 | Camera guided theo timeline | 3D | MUST |
| FR-05 | Hover/click hotspot | 3D | MUST |
| FR-06 | Hiển thị About | Public | MUST |
| FR-07 | Danh sách Project có thứ tự | Public | MUST |
| FR-08 | Project detail + links | Public | MUST |
| FR-09 | Skill theo category | Public | MUST |
| FR-10 | Gửi contact form | Public | MUST |
| FR-11 | Tải/xem CV | Public | MUST |
| FR-12 | Admin login | Admin | MUST |
| FR-13 | CRUD Project | Admin | MUST |
| FR-14 | CRUD Skill | Admin | MUST |
| FR-15 | Update Profile | Admin | MUST |
| FR-16 | Publish/unpublish | Admin | MUST |
| FR-17 | Reorder content | Admin | MUST |
| FR-18 | View contact messages | Admin | SHOULD |

## 5.1 Business rules

| ID | Quy tắc |
| --- | --- |
| BR-01 | Chỉ Project có IsPublished=true mới xuất hiện ở public API. |
| BR-02 | DisplayOrder phải không âm; thứ tự trùng nhau được resolve theo CreationTime. |
| BR-03 | Project phải có Name, Slug, Summary và ít nhất một thumbnail. |
| BR-04 | Slug là duy nhất, lowercase và dùng dấu gạch ngang. |
| BR-05 | Contact message phải qua validation và rate limit. |
| BR-06 | External URL chỉ chấp nhận HTTP/HTTPS hợp lệ. |
| BR-07 | Admin API yêu cầu permission tương ứng, không chỉ kiểm tra role ở frontend. |

# 6. Yêu cầu phi chức năng

| Nhóm | Yêu cầu | Mức |
| --- | --- | --- |
| Performance | Không block main thread kéo dài; render loop dừng khi tab ẩn | MUST |
| Performance | Lazy-load model/texture không cần cho cảnh đầu | MUST |
| Availability | Lỗi 3D không làm mất nội dung portfolio | MUST |
| Security | HTTPS, validation, permission, secret ngoài source | MUST |
| Maintainability | Không component/service > 400 dòng nếu không có lý do | SHOULD |
| Accessibility | Keyboard navigation cho UI overlay; reduced motion | MUST |
| Compatibility | Chrome/Edge/Firefox/Safari phiên bản hiện hành | SHOULD |
| SEO | Metadata, Open Graph, nội dung crawlable ở 2D shell | MUST |
| Observability | Structured logs, correlation id, health endpoint | MUST |

## 6.1 Ngân sách hiệu năng

| Metric | Mục tiêu MVP |
| --- | --- |
| Initial JavaScript (compressed) | ≤ 500 KB, không tính lazy chunks |
| Initial 3D asset | ≤ 8 MB |
| Texture max phổ biến | 2048×2048; ưu tiên WebP/KTX2 |
| Draw calls main room | Mục tiêu < 150 |
| FPS desktop | ≥ 50 trong cảnh chính |
| FPS mobile medium | ≥ 30 hoặc tự động fallback |
| LCP | ≤ 2.5 giây cho shell 2D |

# 7. UX 3D và Storyboard

## 7.1 Concept được chọn

| Concept: Developer Workspace — một phòng làm việc công nghệ hiện đại, sạch, ánh sáng xanh ấm; không dùng cyberpunk quá tối để đảm bảo khả năng đọc. |
| --- |

## 7.2 Bố cục không gian

| [ Skills Wall ]                               | [ About Board ] ---- [ Main Interaction Zone ] ---- [ Projects Monitor ]                               |                          [ Contact Device ]  Entrance → Corridor → Main Interaction Zone |
| --- |

## 7.3 Storyboard

| Scene | Camera | Nội dung | Tương tác |
| --- | --- | --- | --- |
| S0 Loading | Static | Logo, progress, quality auto-detect | Skip 3D |
| S1 Exterior | Wide shot | Tên + Software Engineer | Enter Workspace |
| S2 Approach | Dolly in | Cửa và ánh sáng dẫn hướng | Scroll/auto |
| S3 Corridor | Forward path | Tech icons tinh tế | Skip intro |
| S4 Main room | Reveal orbit nhẹ | 4 hotspot chính | Hover/click |
| S5 Focus | Move to target | Panel thông tin | Back/ESC |

## 7.4 Camera path

| Point | Ý nghĩa | Position gợi ý | LookAt |
| --- | --- | --- | --- |
| P0 | Ngoài công trình | (0, 3, 16) | (0, 2, 0) |
| P1 | Trước cửa | (0, 2.2, 8) | (0, 2, 0) |
| P2 | Qua hành lang | (0, 1.8, 2) | (0, 1.7, -4) |
| P3 | Toàn cảnh phòng | (0, 2.5, -5) | (0, 1.4, -8) |
| P4 | Projects | (3.5, 1.8, -8) | (4, 1.8, -10) |

## 7.5 Quy tắc UX

- Không khóa scroll vô thời hạn; luôn có menu overlay.

- Camera không xoay nhanh hoặc rung.

- Mỗi animation dẫn hướng chính tối đa khoảng 1.5–3 giây.

- Hotspot có cue rõ ràng nhưng không nhấp nháy liên tục.

- ESC hoặc Back luôn trả về Main Room.

- Không tự phát âm thanh.

- Nội dung quan trọng không chỉ tồn tại dưới dạng texture trong cảnh 3D.

# 8. Kiến trúc hệ thống

## 8.1 Architectural style

Chọn ABP Layered Monolith. Đây là lựa chọn cân bằng giữa structure và tốc độ phát triển: đủ tách biệt Application, Domain và Persistence; đồng thời triển khai như một application duy nhất. Kiến trúc này phù hợp với convention của ABP và không tạo chi phí vận hành của microservice.

## 8.2 Cấu trúc repository

| portfolio-3d/ ├── backend/ │   ├── src/ │   │   ├── Portfolio3D.Domain.Shared │   │   ├── Portfolio3D.Domain │   │   ├── Portfolio3D.Application.Contracts │   │   ├── Portfolio3D.Application │   │   ├── Portfolio3D.EntityFrameworkCore │   │   ├── Portfolio3D.HttpApi │   │   └── Portfolio3D.HttpApi.Host │   └── test/ │       ├── Portfolio3D.Domain.Tests │       ├── Portfolio3D.Application.Tests │       └── Portfolio3D.EntityFrameworkCore.Tests ├── angular/ │   └── src/app/ ├── deploy/ │   ├── docker/ │   └── nginx/ └── docs/ |
| --- |

## 8.3 Dependency rule

| HttpApi.Host → HttpApi → Application.Contracts              ↓          Application           EntityFrameworkCore → Domain → Domain.Shared  Angular → generated/static API proxy → HttpApi.Host |
| --- |

| Rule: Domain không tham chiếu Application, EF Core, HTTP hay Angular. Application không chứa Three.js hoặc chi tiết render. |
| --- |

# 9. Thiết kế backend theo ABP

## 9.1 Cách áp dụng DDD vừa đủ

| Khái niệm ABP | Cách dùng trong dự án |
| --- | --- |
| Aggregate Root | Project và Profile chỉ khi cần bảo vệ invariant; Skill có thể là aggregate đơn giản. |
| Entity | ContactMessage là entity lưu dữ liệu. |
| Value Object | ExternalLink có thể thêm sau; MVP dùng validated string để tránh over-engineering. |
| Domain Service | Chỉ tạo khi rule trải qua nhiều aggregate; MVP chưa cần. |
| Application Service | Điều phối use case, authorization, DTO mapping, transaction boundary. |
| Repository | Dùng IRepository<T, Guid> mặc định; không tạo custom repository cho CRUD. |
| Unit of Work | Dùng convention ABP; không tự mở transaction cho truy vấn thường. |

## 9.2 Project responsibilities

| Project | Trách nhiệm |
| --- | --- |
| Domain.Shared | Constants, enums, error codes, localization resources. |
| Domain | Entities, business methods, data seed contributor. |
| Application.Contracts | DTOs, service interfaces, permissions. |
| Application | Application services, mapping, orchestration. |
| EntityFrameworkCore | DbContext, entity configuration, migrations. |
| HttpApi | Conventional/explicit API controllers nếu cần custom route. |
| HttpApi.Host | Startup, auth, middleware, Swagger, deployment host. |

## 9.3 Application service convention

| public interface IProjectAppService : IApplicationService {     Task<ProjectDto> GetAsync(Guid id);     Task<PagedResultDto<ProjectListDto>> GetListAsync(ProjectListInput input);     Task<ProjectDto> CreateAsync(CreateProjectDto input);     Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto input);     Task DeleteAsync(Guid id); } |
| --- |

- Tên method theo convention GetAsync/GetListAsync/CreateAsync/UpdateAsync/DeleteAsync.

- DTO input riêng cho create/update nếu validation khác nhau.

- Application service không trả Entity.

- Không catch Exception chung chỉ để log và throw lại; dùng global exception handling của ABP.

- Dùng [Authorize(Permissions.X)] tại service/method.

## 9.4 Entity design mẫu

| public class Project : FullAuditedAggregateRoot<Guid> {     public string Name { get; private set; }     public string Slug { get; private set; }     public string Summary { get; private set; }     public bool IsPublished { get; private set; }     public int DisplayOrder { get; private set; }      protected Project() { }      public Project(Guid id, string name, string slug, string summary)         : base(id)     {         SetName(name);         SetSlug(slug);         SetSummary(summary);     }      public void Publish() => IsPublished = true;     public void Unpublish() => IsPublished = false; } |
| --- |

## 9.5 Không lạm dụng domain layer

| Simple rule: Validation định dạng, paging, mapping và authorization nằm ở Application. Domain chỉ giữ invariant có ý nghĩa nghiệp vụ như publish state, slug uniqueness workflow hoặc giới hạn dữ liệu. |
| --- |

# 10. Thiết kế frontend và 3D engine

## 10.1 Module structure

| src/app/ ├── core/ │   ├── api/ │   ├── guards/ │   └── services/ ├── features/ │   ├── portfolio-shell/ │   ├── about/ │   ├── projects/ │   ├── skills/ │   ├── contact/ │   └── admin/ ├── three/ │   ├── engine/ │   │   ├── scene-engine.service.ts │   │   ├── renderer.service.ts │   │   └── animation-loop.service.ts │   ├── camera/ │   │   ├── camera-path.service.ts │   │   └── camera-controller.service.ts │   ├── assets/ │   │   └── asset-loader.service.ts │   ├── interaction/ │   │   └── raycast-interaction.service.ts │   └── quality/ │       └── quality-manager.service.ts └── shared/ |
| --- |

## 10.2 Trách nhiệm lớp 3D

| Service | Trách nhiệm | Không làm |
| --- | --- | --- |
| SceneEngine | Khởi tạo scene, lifecycle, dispose | Không gọi API nghiệp vụ. |
| AssetLoader | Load GLB/texture, progress, cache | Không điều khiển camera. |
| CameraController | Camera path, focus target, back | Không mở dialog trực tiếp. |
| RaycastInteraction | Pointer → object id → event | Không chứa project data. |
| QualityManager | Auto quality, resize, DPR | Không biết layout nội dung. |
| PortfolioShell | Kết nối event 3D với UI panels | Không thao tác low-level renderer. |

## 10.3 Event flow

| Raycast hit "projects-monitor"   → InteractionService emits hotspotSelected("projects")   → PortfolioShell updates UI state   → CameraController.focus("projects")   → ProjectsPanel opens   → Project data comes from ProjectApiService |
| --- |

## 10.4 Lifecycle bắt buộc

- Khởi tạo Three.js sau khi canvas sẵn sàng.

- Cancel requestAnimationFrame khi component destroy.

- Dispose geometry, material, texture và renderer.

- Remove event listener và ResizeObserver.

- Pause animation khi document.hidden.

- Không tạo lại scene khi chỉ đổi route phụ nếu có thể giữ shell.

# 11. Mô hình dữ liệu

| Entity | Field chính | Ghi chú |
| --- | --- | --- |
| Profile | DisplayName, Headline, Bio, AvatarUrl, CvUrl, Email, SocialLinksJson | Một bản ghi active. |
| Project | Name, Slug, Summary, Description, ThumbnailUrl, DemoUrl, RepositoryUrl, IsPublished, IsFeatured, DisplayOrder | Aggregate chính. |
| ProjectTechnology | ProjectId, Name, DisplayOrder | Owned child hoặc entity phụ. |
| Skill | Name, Category, IconUrl, LevelLabel, DisplayOrder, IsPublished | CRUD đơn giản. |
| ContactMessage | FullName, Email, Subject, Message, Status, CreationTime | Không public list. |

## 11.1 Index

| Table | Index |
| --- | --- |
| Projects | UNIQUE(Slug) |
| Projects | INDEX(IsPublished, DisplayOrder) |
| Skills | INDEX(IsPublished, Category, DisplayOrder) |
| ContactMessages | INDEX(Status, CreationTime DESC) |

## 11.2 Quy tắc migration

- Mỗi thay đổi schema có migration riêng, tên mô tả rõ.

- Không chỉnh sửa migration đã chạy ở production.

- Migration được review cùng code thay đổi entity.

- Production chạy DbMigrator hoặc pipeline migration có kiểm soát; không auto-migrate âm thầm khi app start.

# 12. API contract

| Method | Endpoint | Auth | Mô tả |
| --- | --- | --- | --- |
| GET | /api/app/public-portfolio | Anonymous | Profile + featured projects + skills. |
| GET | /api/app/project | Anonymous | Danh sách project đã publish. |
| GET | /api/app/project/by-slug/{slug} | Anonymous | Chi tiết project. |
| POST | /api/app/contact-message | Anonymous | Gửi liên hệ. |
| GET | /api/app/project | Permission | Admin list gồm unpublished. |
| POST | /api/app/project | Permission | Tạo project. |
| PUT | /api/app/project/{id} | Permission | Cập nhật project. |
| DELETE | /api/app/project/{id} | Permission | Xóa mềm project. |

## 12.1 Response strategy

- Dùng DTO ổn định; không expose entity.

- Dùng PagedResultDto cho list admin.

- Public portfolio endpoint có thể aggregate dữ liệu để giảm round trip.

- Validation error và business exception đi qua standard ABP exception response.

- Không version API trong MVP; thêm version khi có consumer ngoài hoặc breaking changes.

## 12.2 PublicPortfolioDto

| {   "profile": { "displayName": "...", "headline": "...", "bio": "..." },   "featuredProjects": [     { "id": "...", "slug": "vievent", "name": "VIEvent", "thumbnailUrl": "..." }   ],   "skillGroups": [     { "category": "Backend", "items": [{ "name": ".NET", "iconUrl": "..." }] }   ] } |
| --- |

# 13. Bảo mật

| Control | Thiết kế |
| --- | --- |
| Authentication | ABP/OpenIddict cho admin; public API anonymous có giới hạn. |
| Authorization | Permission-based tại Application Service. |
| Validation | DataAnnotations/ABP validation; FluentValidation chỉ khi rule phức tạp. |
| Secrets | Environment variables hoặc secret store; không commit appsettings production. |
| Contact abuse | Rate limiting + honeypot hoặc reCAPTCHA nếu có spam thực tế. |
| XSS | Không render HTML từ admin nếu chưa sanitize; description ưu tiên plain text/Markdown an toàn. |
| Upload | MVP dùng URL có kiểm soát; upload file để phase sau. |
| CORS | Chỉ origin frontend production và dev cần thiết. |
| Headers | CSP, HSTS, X-Content-Type-Options, Referrer-Policy. |

## 13.1 Permission model

| Portfolio3D ├── Profile │   └── Update ├── Projects │   ├── Create │   ├── Update │   ├── Delete │   └── Publish ├── Skills │   ├── Create │   ├── Update │   └── Delete └── ContactMessages     ├── View     └── UpdateStatus |
| --- |

# 14. Hiệu năng 3D

## 14.1 Asset pipeline

1. Model trong Blender được clean hierarchy và đặt tên hotspot rõ ràng.

1. Apply transform, loại geometry không thấy, merge mesh hợp lý.

1. Xuất GLB.

1. Nén Draco hoặc Meshopt nếu tương thích pipeline.

1. Texture chuyển WebP/KTX2; giới hạn resolution.

1. Kiểm tra model bằng viewer trước khi tích hợp.

1. Đo bundle và FPS trên máy thật.

## 14.2 Quality profiles

| Profile | DPR | Shadow | Post-processing | Texture |
| --- | --- | --- | --- | --- |
| High | min(devicePixelRatio, 2) | On | Selective | 2K |
| Medium | ≤ 1.5 | Limited | Off/light | 1K–2K |
| Low | 1 | Off | Off | 1K |
| 2D Fallback | N/A | N/A | N/A | Images only |

## 14.3 Runtime rules

- Không render nếu canvas ngoài viewport hoặc tab ẩn.

- Raycast chỉ trên tập hotspot, không toàn scene.

- Dùng instancing cho vật thể lặp lại.

- Không update object mỗi frame nếu giá trị không đổi.

- GSAP điều khiển timeline; Three.js render frame hiện tại.

- Tránh nhiều transparent material và real-time lights.

# 15. Logging và observability

| Khu vực | Log/Event |
| --- | --- |
| Backend | Request failure, validation/business error, admin write operation, contact submit. |
| Frontend | 3D initialization failure, asset load failure, fallback activation. |
| Health | Database connectivity, application health endpoint. |
| Metrics tối thiểu | Page load, enter clicked, project opened, contact submitted. |

| Privacy: Không log toàn bộ nội dung contact message hoặc access token. Email có thể mask trong log. |
| --- |

## 15.1 Error handling UX

- Asset load lỗi → retry một lần → fallback 2D.

- API public lỗi → hiển thị cached/static essential profile nếu có.

- Contact lỗi → giữ nội dung form và cho gửi lại.

- Admin lỗi → hiển thị message thân thiện và correlation id khi phù hợp.

# 16. Testing strategy

| Layer | Test | Mục tiêu |
| --- | --- | --- |
| Domain | Entity method tests | Publish/unpublish, normalization, invariant. |
| Application | Integration tests | Permission, CRUD flow, filtering, validation. |
| EF Core | Repository/query tests | Index-sensitive query và mapping. |
| Angular | Unit tests | State services, mapper, camera progress calculation. |
| 3D | Component/integration smoke | Load scene, hotspot event, dispose. |
| E2E | Playwright/Cypress | Public journey và admin CRUD chính. |
| Performance | Lighthouse + manual FPS | Không regression asset/bundle. |

## 16.1 Test pyramid cho MVP

- Nhiều unit/integration test backend; ít E2E nhưng bao phủ luồng chính.

- Không cố unit-test trực tiếp WebGL renderer; test logic thuần và smoke test behavior.

- Mỗi bug production đáng kể phải có regression test nếu khả thi.

## 16.2 Các test case bắt buộc

| ID | Test |
| --- | --- |
| TC-01 | Anonymous chỉ thấy project đã publish. |
| TC-02 | User thiếu permission không tạo/sửa/xóa project. |
| TC-03 | Slug trùng bị từ chối. |
| TC-04 | Contact invalid email/message rỗng bị từ chối. |
| TC-05 | WebGL failure kích hoạt 2D fallback. |
| TC-06 | Skip intro đưa camera đến Main Room. |
| TC-07 | Destroy component không còn animation frame/listener. |

# 17. Coding standards

## 17.1 Backend

- Nullable reference types bật.

- Async suffix cho method bất đồng bộ.

- CancellationToken truyền xuống I/O khi API cho phép.

- Không dùng service locator; constructor injection.

- Không tạo generic base service nội bộ khi chỉ có 1–2 use case.

- Không trả null mơ hồ; dùng exception chuẩn hoặc nullable có chủ đích.

- Entity setter private/protected khi cần bảo vệ state.

- DTO không chứa logic nghiệp vụ.

## 17.2 Frontend

- Standalone components hoặc cấu trúc Angular thống nhất; không trộn nhiều pattern.

- Typed API models; tránh any.

- Service giữ state dùng signal/RxJS rõ ràng, không subscription lồng nhau.

- Không thao tác DOM trực tiếp trừ canvas/Three.js adapter.

- Component template không chứa logic tính toán phức tạp.

- Mọi listener/timer/animation có cleanup.

## 17.3 Guardrails chống over-engineering

| Dấu hiệu | Cách xử lý |
| --- | --- |
| Interface chỉ có một implementation và không cần test seam | Không tạo thêm interface ngoài convention ABP. |
| Mapper thủ công lặp nhiều | Dùng ObjectMapper; không dùng cho logic có điều kiện phức tạp. |
| Custom repository cho GetList đơn giản | Dùng IRepository + IQueryable/AsyncExecuter. |
| Event bus chỉ để gọi service cùng process | Gọi trực tiếp hoặc local event chỉ khi cần decouple thật. |
| Một abstraction cho 'future provider' chưa tồn tại | Hoãn cho đến khi provider thứ hai xuất hiện. |

# 18. Git, review và CI/CD

## 18.1 Branch strategy

- main luôn deploy được.

- Feature branch ngắn: feat/project-crud, feat/three-camera-path.

- Pull request nhỏ, một mục tiêu.

- Conventional commits: feat, fix, refactor, test, docs, chore.

## 18.2 Pull request checklist

- Build và test pass.

- Không commit secret hoặc file model nguồn quá lớn không cần thiết.

- Có migration nếu đổi schema.

- Có test hoặc lý do hợp lệ khi không test.

- Đã kiểm tra dispose/lifecycle nếu sửa 3D.

- Không tăng bundle/asset quá ngân sách mà chưa ghi nhận.

## 18.3 Pipeline

| Pull Request   → dotnet restore/build/test   → npm ci   → angular lint/test/build   → security/dependency scan   → optional Lighthouse smoke  main   → build Docker images   → migrate database   → deploy backend   → deploy frontend/CDN   → health check   → rollback on failure |
| --- |

# 19. Kế hoạch triển khai

| Phase | Đầu ra | Exit criteria |
| --- | --- | --- |
| 0. Discovery | Concept, scope, wireframe, asset list | MVP scope approved. |
| 1. Skeleton | ABP solution + Angular + DB + auth | Build/run local, login admin. |
| 2. Content backend | Profile/Project/Skill APIs | CRUD + tests pass. |
| 3. 2D portfolio | Public content shell | Nội dung usable không cần 3D. |
| 4. 3D vertical slice | Room + camera + Projects hotspot | End-to-end demo chạy. |
| 5. Complete scene | About/Skills/Contact hotspots | All MVP journeys. |
| 6. Hardening | Performance, a11y, security, tests | DoD pass. |
| 7. Deploy | Production environment | Health + smoke pass. |

## 19.1 Thứ tự code đề xuất

1. Khởi tạo ABP Layered Solution và PostgreSQL.

1. Định nghĩa entities, permissions và DTO.

1. Implement Project CRUD end-to-end.

1. Dựng public portfolio 2D bằng dữ liệu thật.

1. Dựng GLB room tối giản và integrate Three.js.

1. Thêm camera path và Projects hotspot.

1. Thêm About, Skills, Contact.

1. Tối ưu assets, fallback, testing và deployment.

# 20. Backlog MVP

| Epic | Story | Estimate tương đối | Priority |
| --- | --- | --- | --- |
| Foundation | Create ABP solution + PostgreSQL | M | MUST |
| Foundation | Configure Angular environment/proxy | S | MUST |
| Auth | Admin authentication + permission seed | M | MUST |
| Projects | Entity, migration, CRUD API, tests | L | MUST |
| Skills | Entity, CRUD API, tests | M | MUST |
| Profile | Single profile read/update | M | MUST |
| Public UI | 2D shell + API integration | L | MUST |
| 3D Core | Scene lifecycle + asset loader | L | MUST |
| 3D Camera | Intro path + skip + focus/back | L | MUST |
| Interaction | Raycast hotspot event | M | MUST |
| Contact | Form + API + validation/rate limit | M | MUST |
| Quality | Low mode + 2D fallback | L | MUST |
| Delivery | Docker + Nginx + CI | L | MUST |

# 21. Rủi ro và phương án xử lý

| Rủi ro | Xác suất | Tác động | Giảm thiểu |
| --- | --- | --- | --- |
| Model quá nặng | Cao | Cao | Asset budget, compression, low-poly, lazy load. |
| Camera gây chóng mặt | Trung bình | Cao | Reduced motion, skip intro, tốc độ giới hạn. |
| 3D che khuất nội dung | Trung bình | Cao | 2D overlay và fallback hoàn chỉnh. |
| Code Three.js khó maintain | Cao | Cao | Tách engine/camera/assets/interaction, lifecycle tests. |
| ABP bị dùng quá nặng | Trung bình | Trung bình | Giữ layered monolith, không thêm module/hạ tầng không cần. |
| Admin mất quá nhiều thời gian | Trung bình | Trung bình | CRUD tối giản, dùng convention/proxy có sẵn. |
| SEO yếu | Trung bình | Trung bình | 2D HTML shell, metadata, pre-render/SSR chỉ nếu cần. |

# 22. Definition of Done

- Acceptance criteria của story đạt.

- Code review hoàn tất.

- Unit/integration test liên quan pass.

- Không có lint/build error.

- Có migration và rollback consideration nếu đổi DB.

- UI kiểm tra desktop + mobile.

- 3D kiểm tra High/Medium/Low và fallback.

- Không leak listener, animation frame hoặc GPU resource.

- Security checklist đạt.

- Tài liệu/API note được cập nhật.

- Deployed environment smoke test pass.

## 22.1 MVP release gate

| Gate | Pass condition |
| --- | --- |
| Functional | Tất cả MUST story hoàn tất. |
| Performance | Không vượt asset budget nghiêm trọng; FPS và LCP đạt mức chấp nhận. |
| Accessibility | Keyboard/reduced motion/fallback hoạt động. |
| Security | Không secret, permission server-side, validation/rate limit. |
| Reliability | Lỗi asset/API không làm trang trắng. |
| Operations | Health check, migration, logs, rollback plan. |

# 23. Tài liệu tham khảo và quyết định nguồn

Thiết kế này tham khảo convention của ABP Framework nhưng điều chỉnh theo quy mô portfolio cá nhân. Các phần chính được đối chiếu với tài liệu chính thức ABP phiên bản latest tại thời điểm lập tài liệu.

| Nguồn | URL |
| --- | --- |
| ABP Documentation — Latest | https://abp.io/docs/latest |
| Layered Solution Template | https://abp.io/docs/latest/solution-templates/layered-web-application |
| Application Services | https://abp.io/docs/latest/framework/architecture/domain-driven-design/application-services |
| Entities & Aggregate Roots | https://abp.io/docs/latest/framework/architecture/domain-driven-design/entities |
| Entity Framework Core Integration | https://abp.io/docs/latest/framework/data/entity-framework-core |
| Automated Testing | https://abp.io/docs/latest/testing/overall |

## 23.1 Architectural Decision Records

| ADR | Quyết định | Lý do | Xem xét lại khi |
| --- | --- | --- | --- |
| ADR-001 | ABP Layered Monolith | Cân bằng maintainability và tốc độ | Team/tải/bounded context tăng đáng kể. |
| ADR-002 | Angular + Three.js | WebGL ecosystem và phù hợp kỹ năng hiện có | Frontend strategy thay đổi toàn diện. |
| ADR-003 | PostgreSQL | Ổn định, quen thuộc, phù hợp EF Core | Hosting buộc DB khác. |
| ADR-004 | 2D fallback là bắt buộc | Khả dụng, a11y và thiết bị yếu | Không bỏ; chỉ cải thiện cách triển khai. |
| ADR-005 | No Redis/Microservice MVP | Không có nhu cầu đo được | Có tải/cache/distribution requirement thực tế. |

## 23.2 Kết luận

| Implementation-ready: Tài liệu đủ để bắt đầu Phase 1. Việc đầu tiên là khởi tạo skeleton ABP + Angular, sau đó hoàn thành một vertical slice Project CRUD → public 2D → Projects hotspot 3D trước khi mở rộng toàn cảnh. |
| --- |

# 24. Bổ sung tính khả thi và vận hành MVP

Phần này khóa các quyết định còn để ngỏ trong phiên bản 1.0, đặc biệt đối với bối cảnh một developer tự thiết kế, phát triển và vận hành dự án.

## 24.1 Ước lượng thời gian tuyệt đối

Ước lượng giả định một developer đã quen .NET, Angular và ABP, nhưng chưa chuyên sâu Blender/Three.js; làm trung bình 15-20 giờ mỗi tuần. Tổng thời gian MVP dự kiến là 10-12 tuần, tương đương khoảng 180-230 giờ. Nếu làm toàn thời gian, có thể rút xuống khoảng 5-7 tuần.

| Tuần | Trọng tâm | Đầu ra | Giờ dự kiến |
| --- | --- | --- | --- |
| 1 | Discovery và setup | Chốt concept, wireframe, repo, ABP solution, Angular shell, PostgreSQL | 16-20 |
| 2 | Backend foundation | Auth admin, permissions, entities, migrations, seed | 18-22 |
| 3 | Project/Profile APIs | CRUD Project, Profile, validation, tests | 18-24 |
| 4 | Skill/Contact + Admin | CRUD Skill, Contact API, admin screens cơ bản | 18-24 |
| 5 | Public 2D shell | About, Projects, Skills, Contact usable không cần 3D | 16-22 |
| 6 | 3D vertical slice | Room GLB tối giản, loader, lifecycle, Projects hotspot | 20-28 |
| 7 | Camera và interaction | Intro path, skip, focus/back, raycast | 20-26 |
| 8 | Hoàn thiện scene | About/Skills/Contact hotspots, responsive overlay | 18-24 |
| 9 | Performance/fallback | Low quality, reduced motion, 2D fallback, asset optimization | 18-24 |
| 10 | SEO/security/testing | Pre-render public routes, permissions, E2E, Lighthouse | 18-24 |
| 11 | Deploy và hardening | Docker/Nginx, migration, logs, smoke test | 14-20 |
| 12 | Buffer | Fix bug, polish UX, cập nhật tài liệu | 12-20 |

- Mốc kiểm tra khả thi sớm nhất là cuối tuần 6: Project CRUD → public 2D → một hotspot 3D phải chạy end-to-end.

- Nếu chậm tiến độ, giảm độ chi tiết model và animation trước; không cắt 2D fallback, accessibility hoặc permission server-side.

- Blender asset là phần có độ bất định cao nhất; nên dùng asset low-poly hợp pháp hoặc prototype bằng primitive trước.

## 24.2 Reorder content là chức năng bắt buộc

FR-17 được nâng từ SHOULD lên MUST để đồng nhất với DisplayOrder trong data model. Admin không phải sửa database trực tiếp.

| Quyết định | Thiết kế MVP |
| --- | --- |
| UI | Mỗi Project/Skill có nút Move Up, Move Down hoặc trường thứ tự số; drag-and-drop là tùy chọn. |
| API | PUT /api/app/project/reorder và PUT /api/app/skill/reorder nhận danh sách { id, displayOrder }. |
| Transaction | Cập nhật toàn bộ thứ tự trong một Unit of Work. |
| Validation | DisplayOrder không âm; server chuẩn hóa về dãy 0..n-1 sau reorder. |
| Concurrency | MVP dùng last-write-wins; optimistic concurrency chỉ thêm khi có nhiều admin. |

## 24.3 Contact message và chiến lược thông báo

Hành vi MVP được chấp nhận: contact message được lưu trong database và chủ sở hữu chủ động kiểm tra trong Admin. Không có email notification tự động trong release đầu tiên nhằm tránh thêm SMTP provider, retry job và cấu hình spam trước khi có nhu cầu thực tế.

| Hạng mục | MVP | Sau MVP |
| --- | --- | --- |
| Lưu message | Có | Có |
| Admin inbox + unread count | Có | Có |
| Email notify tức thời | Không | Có thể thêm |
| Background retry/Hangfire | Không | Chỉ thêm cùng email notify |
| Retention | Giữ đến khi admin xóa mềm | Có thể thêm policy 12-24 tháng |

- Admin dashboard hiển thị số message New và sắp xếp mới nhất trước.

- ContactMessage.Status gồm New, Read, Archived.

- Khi mở chi tiết, message chuyển sang Read; admin có thể Archive.

- README vận hành ghi rõ cần kiểm tra Admin định kỳ trong giai đoạn MVP.

## 24.4 Quyết định SEO: pre-render public shell

SEO là yêu cầu MUST vì mục tiêu chính là được nhà tuyển dụng và khách hàng tìm thấy. Do đó kiến trúc chốt sử dụng HTML public shell có thể crawl, kết hợp Angular pre-render cho các route ổn định. Three.js chỉ được khởi tạo sau khi shell hiển thị và không phải nguồn nội dung duy nhất.

| Hạng mục | Quyết định |
| --- | --- |
| Rendering | Pre-render các route /, /projects và /projects/:slug trong pipeline build khi dữ liệu phù hợp; nếu dynamic quá nhiều, pre-render shell + fetch runtime. |
| Content | Tên, headline, bio, project summary và links tồn tại dưới dạng HTML text, không chỉ nằm trong canvas. |
| Metadata | Title, meta description, canonical, Open Graph và Twitter Card theo route. |
| Structured data | Person và CreativeWork/SoftwareApplication JSON-LD cho profile/project phù hợp. |
| Sitemap | Tạo sitemap.xml từ danh sách project publish. |
| Robots | Cho index public pages; chặn /admin và auth routes. |
| 3D loading | Canvas lazy-init sau first contentful render; crawler không phụ thuộc WebGL. |

- SSR runtime không bắt buộc cho MVP vì tăng chi phí vận hành; pre-render/static generation là lựa chọn mặc định.

- Chỉ chuyển sang SSR runtime khi nội dung thay đổi thường xuyên và đo được lợi ích SEO hoặc social preview không đáp ứng bởi pre-render.

- Project mới publish phải kích hoạt rebuild frontend hoặc cập nhật sitemap/meta theo cơ chế triển khai đã chọn.

## 24.5 Chi phí hosting và vận hành

Mục tiêu vận hành là giữ chi phí thấp, dễ backup và không yêu cầu quản trị nhiều dịch vụ. Con số dưới đây là khoảng ngân sách thiết kế, không phải báo giá cố định của nhà cung cấp.

| Phương án | Thành phần | Ước lượng/tháng | Phù hợp |
| --- | --- | --- | --- |
| A. Tối giản | Frontend static/CDN miễn phí + ASP.NET/PostgreSQL trên một VPS nhỏ | Khoảng 5-12 USD | Khuyến nghị khi muốn tự quản lý Docker. |
| B. Managed | Frontend static + managed app + managed PostgreSQL | Khoảng 10-30 USD | Ít vận hành hơn, dễ scale, có thể có free tier. |
| C. Demo tiết kiệm | Frontend static + backend/database free tier có sleep | 0-10 USD | Chấp nhận cold start; phù hợp giai đoạn portfolio ban đầu. |

| Khoản vận hành | Chiến lược MVP |
| --- | --- |
| Domain | Khoảng 10-20 USD/năm tùy tên miền. |
| Object storage/CDN | Ưu tiên static assets trên CDN; kiểm soát egress bằng nén GLB/texture. |
| Database backup | Backup tự động hằng ngày hoặc snapshot VPS; giữ ít nhất 7 bản gần nhất. |
| Monitoring | Health check và log retention ngắn; tránh hệ thống APM trả phí khi chưa cần. |
| Email | Không phát sinh trong MVP vì chưa có contact notification. |
| Budget guardrail | Mục tiêu tổng chi phí cơ bản không vượt khoảng 15 USD/tháng trong giai đoạn đầu. |

## 24.6 Baseline kế hoạch chính thức

| Hạng mục | Baseline v1.1 |
| --- | --- |
| Thời lượng MVP | 10-12 tuần part-time, 180-230 giờ. |
| Reorder | MUST, có UI và API; không sửa DB tay. |
| Contact notify | Admin inbox + unread count; chưa gửi email. |
| SEO | Public HTML shell + pre-render + metadata + sitemap. |
| Hosting | Ưu tiên static frontend + một backend/PostgreSQL nhỏ; budget mục tiêu ≤ 15 USD/tháng. |