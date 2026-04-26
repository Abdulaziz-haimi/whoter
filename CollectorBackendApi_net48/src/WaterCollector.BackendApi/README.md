# WaterCollector.BackendApi (.NET Framework 4.8 / Web API 2)

هذا المشروع نسخة Backend مستقلة تناسب بيئة WinForms .NET Framework 4.7.2 عندما لا يتوفر .NET 8.

## الهدف
- لا يتم دمج الـ Backend داخل مشروع WinForms نفسه.
- يتم إضافته داخل **نفس الـ Solution** كمشروع مستقل.
- الهاتف يتصل بهذا الـ API فقط.

## المسارات
- `POST /api/auth/login`
- `GET /api/mobile-sync/receivables`
- `POST /api/mobile-sync/upload-batch`
- `GET /api/mobile-sync/import-decisions`
- `GET /api/mobile-sync/health`

## المطلوب في قاعدة البيانات
- `usp_MobileSync_ExportReceivables`
- `usp_MobileSync_SaveBatch`
- `usp_MobileSync_GetImportDecisions`
- `dbo.MobileReceiptImportType`
- `dbo.MobileReceiptImportLineType`
- `dbo.CollectorDevices`
- `dbo.MobileReceiptImports`

## ربط المستخدم بالمحصل
الخدمة تحاول الربط بهذا الترتيب:
1. `Users.CollectorID`
2. جدول `UserCollectors`
3. `Users.Phone = Collectors.Phone`

## كلمة المرور
اضبط `Authentication:PasswordMode` داخل `Web.config`
- `Sha256Hex` للإنتاج
- `PlainText` للتجربة السريعة فقط

## التشغيل
1. افتح المشروع في Visual Studio
2. Restore NuGet packages
3. عدّل `Web.config`
4. شغّل المشروع على `http://localhost:5099/`
5. من Android emulator استخدم `http://10.0.2.2:5099/`
