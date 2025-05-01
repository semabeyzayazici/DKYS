# DKYS
Dinamik konfigürasyon yönetimini ve konfigürasyon CRUD işlemlerini sağlayan bir ASP.NET MVC projesidir. ConfigurationConsoleTest ile ConfigurationReader kütüphanesi test edilebilir.
Bu uygulama ile farklı servislerin (örneğin SERVICE-A) yapılandırma ayarları merkezi bir veritabanında tutulur 
ve belirli aralıklarla otomatik olarak güncellenerek okunabilir.

__Kullanılan Teknolojiler__  

• .net 8  
• ASP.NET Core MVC  
• C#  
• Microsoft SQL Server  
• Entity Framework Core  
• Timer (background refresh)  
• Dependency Injection  
• Singleton Pattern  
• ConcurrentDictionary  
• Async/Await  

__Proje Yapısı__

__1. ConfigurationLibrary__

• ConfigurationReader sınıfını içerir.

• Veritabanından ApplicationName bazlı ayarları çeker.

• Verileri belirli aralıklarla yeniler (örneğin her 10 saniyede bir).

__2. ConfigurationWebApp (MVC Projesi)__

__ConfigurationItemsController:__

• ConfigurationSettings tablosuna CRUD işlemleri yapılmasını sağlar.

__ConfigViewerController:__

• ConfigurationReader sınıfını kullanarak yapılandırmaları listeler.

• Index View'ında ayarları tablo olarak gösterir.

__Veritabanı Yapısı__

<pre> CREATE TABLE [dbo].[ConfigurationSettings] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    [Value] NVARCHAR(500) NOT NULL,
    [IsActive] BIT NOT NULL,
    [ApplicationName] NVARCHAR(100) NOT NULL
);
 </pre>

__Kurulum Adımları__

1- Bu projeyi klonlayın:
<pre> git clone https://github.com/semabeyzayazici/DKYS.git  </pre>

2- SQL Server’da ConfigurationDb adında bir veritabanı oluşturun ve tabloyu aşağıdaki SQL komutu ile kurun.
<pre> CREATE DATABASE ConfigurationDb;

USE ConfigurationDb;

CREATE TABLE [dbo].[ConfigurationSettings] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Type] NVARCHAR(20) NOT NULL,
    [Value] NVARCHAR(500) NOT NULL,
    [IsActive] BIT NOT NULL,
    [ApplicationName] NVARCHAR(100) NOT NULL
);

INSERT INTO ConfigurationSettings (Name, Type, Value, IsActive, ApplicationName)
VALUES 
('SiteName', 'string', 'soty.io', 1, 'SERVICE-A'),
('IsBasketEnabled', 'bool', '1', 1, 'SERVICE-B'),
('MaxItemCount', 'int', '50', 0, 'SERVICE-A'); </pre>

3- ConfigurationWebApp projesi için Program.cs dosyasındaki ConfigurationReader yapılandırmasını ihtiyacınıza göre güncelleyin.
   appsettings.json dosyasındaki bağlantı cümlesinin yapılandırmasını da ihtiyacınıza göre güncelleyin.
<pre> 
builder.Services.AddSingleton(sp =>
{
    return new ConfigurationReader(
        "SERVICE-A", // Uygulamanın adı - veritabanındaki ApplicationName alanı ile eşleşmeli
        "Server=.;Database=ConfigurationDb;Integrated Security=True;TrustServerCertificate=True;", // Bağlantı cümlesi
        10000 // Yenileme süresi (ms) - burada 10 saniye
    );
}); </pre>
<pre> 
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ConfigurationDb;Trusted_Connection=True;TrustServerCertificate=True;"
},
</pre>
4- ConfigurationConsoleTest, console uygulamasını çalıştırmak için Program.cs dosyasını ihtiyacınıza göre güncelleyin. Bu proje kütüphanenin çalışıp çalışmadığını kontrol etmek için yazılmıştır.

__Test ve Kullanım__

ConfigurationItems ekranı üzerinden veri ekleyip düzenlemeler yapılabilir.

ConfigViewer altında tanımlı olan SERVICE-A ayarlarını görüntülenebilir. Proje şu anda SERVICE-A olarak ayarlı. Bu ayarı Program.cs dosyası içerisinden güncelleyebilirsiniz.

Şu ankı yapılandırma ile çalıştırıldığında sistem 10 saniyede bir SERVİCE-A uygulaması için cache belleğini günceller.
ConfigurationItems/Edit/1 ekranından günelleme yapıldığında, ConfigViewer sayfasını hemen yenilediğimizde ayarın güncellenmiş halini göremeyiz. Ama 10 saniye içerisinde sayfayı tekrar yenilediğimizde bu verilerin güncellenmiş hali ekranda gösterilir. 

