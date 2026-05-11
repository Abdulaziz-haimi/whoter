# مشروع WaterCollector.BackendApi الكامل

هذه الحزمة تحتوي مشروع API كامل متوافق مع نظام `water3` و SQL Server.

## محتوى الحزمة

- `WaterCollector.BackendApi`: مشروع Class Library يحتوي:
  - Controllers
  - Contracts
  - Services
  - Security / JWT
  - OWIN Startup
  - ApiHostService للاستضافة داخل WinForms
- `WaterCollector.BackendApi.Host`: مشروع Console للتجربة المستقلة.
- `Scripts/01_MobileSync_RequiredProcedures.sql`: إجراءات SQL مطلوبة للمزامنة إذا لم تكن موجودة.

## طريقة التشغيل كتطبيق مستقل

1. افتح Visual Studio.
2. أنشئ Blank Solution.
3. أضف المشروعين:
   - `WaterCollector.BackendApi\WaterCollector.BackendApi.csproj`
   - `WaterCollector.BackendApi.Host\WaterCollector.BackendApi.Host.csproj`
4. اجعل `WaterCollector.BackendApi.Host` هو Startup Project.
5. عدّل الاتصال في `WaterCollector.BackendApi.Host\App.config`.
6. شغّل المشروع.
7. جرّب:

```txt
http://localhost:8085/api/mobile-sync/health
```

## طريقة دمجه مع WinForms water3

1. أضف مشروع `WaterCollector.BackendApi` إلى Solution الخاص بـ water3.
2. في مشروع water3 أضف Reference إلى `WaterCollector.BackendApi`.
3. في Program.cs أضف:

```csharp
private static WaterCollector.BackendApi.Hosting.ApiHostService _apiHost;
```

وقبل فتح LoginForm:

```csharp
_apiHost = new WaterCollector.BackendApi.Hosting.ApiHostService();
_apiHost.Start();
Application.ApplicationExit += (s, e) => _apiHost?.Dispose();
```

## روابط API

### Health

```txt
GET /api/mobile-sync/health
GET /api/health
```

### Login

```txt
POST /api/auth/login
```

Body:

```json
{
  "userName": "collector1",
  "password": "123456",
  "deviceCode": "PHONE-001",
  "deviceName": "Samsung A12",
  "deviceModel": "SM-A125F",
  "appVersion": "1.0.0"
}
```

### Receivables

```txt
GET /api/mobile-sync/receivables
Authorization: Bearer TOKEN
```

### Upload Batch

```txt
POST /api/mobile-sync/upload-batch
Authorization: Bearer TOKEN
```

### Import Decisions

```txt
GET /api/mobile-sync/import-decisions?deviceCode=PHONE-001
Authorization: Bearer TOKEN
```

## ملاحظات مهمة

- تسجيل الدخول يحتاج أن يكون المستخدم مربوطًا بمحصل في جدول `Collectors.UserID` أو عبر جدول `UserCollectors` إن وجد.
- المشروع يستخدم SHA256 افتراضيًا حتى يتوافق مع النظام الحالي. يمكن تغيير ذلك لاحقًا إلى PBKDF2.
- غيّر قيمة `Jwt:SecretKey` قبل الإنتاج.
- افتح المنفذ 8085 في جدار الحماية:

```cmd
netsh advfirewall firewall add rule name="water3 API 8085" dir=in action=allow protocol=TCP localport=8085
netsh http add urlacl url=http://+:8085/ user=Everyone
```

إذا كان ويندوز عربيًا وقد فشل الأمر الأخير، شغّل البرنامج كمسؤول أو استخدم المستخدم المناسب بدل `Everyone`.
