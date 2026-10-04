# Azure FinOps CLI 🚀

**Local-First Azure Cost and Resource Leak Detector**

Özellikle öğrenciler, bağımsız geliştiriciler ve küçük start-up'lar Azure üzerinde denemeler yaparken açık unuttukları test veritabanları, IP'ler veya sanal makineler yüzünden ay sonunda sürpriz faturalarla karşılaşabilirler. Azure Portal içindeki bütçe uyarıları genellikle e-posta atar ama geç kalabilir ve kaynakları proaktif olarak analiz etmez.

**Azure FinOps CLI**, terminalden tek komutla çalışan, lokal Azure CLI kimlik doğrulamanıza bağlanan ve "zombi" kaynakları (örn. CPU kullanımı %1'in altında olan) tespit edip size saatlik/aylık ne kadar para kaybettirdiğini gösteren hafif bir araçtır.

## ✨ Özellikler

- **🔒 Local-First Kimlik Doğrulama:** Sizden hiçbir zaman şifre veya API key istemez. Bilgisayarınızdaki mevcut `az login` oturumunu güvenle kullanır.
- **🧟 Zombi Kaynak Tespiti:** Belirli bir eşiğin altında çalışan (örneğin %1 CPU) sanal makine ve veritabanlarını bulur.
- **💸 Gerçek Zamanlı Fiyatlandırma:** Azure Retail Prices API ile entegredir. Kaynağın bulunduğu bölgeye (Region) ve modeline (SKU) göre kuruşu kuruşuna güncel maliyetleri çeker.
- **🌍 Çoklu Para Birimi (Currency):** Maliyetleri sadece Dolar (USD) değil, Türk Lirası (TRY) veya Euro (EUR) gibi kurlarda da görüntüleyebilirsiniz.
- **🛠️ Anında Aksiyon:** Terminalden çıkmadan zombi kaynakları uykuya alabilir (Deallocate) veya silebilirsiniz.
- **🎨 Zengin Terminal Arayüzü:** `Spectre.Console` ile hazırlanmış okunabilir ve renkli tablolar.

## 🚀 Kurulum ve Kullanım

### Ön Koşullar
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)

### Adım 1: Azure'a Giriş Yapın
Terminalinizi açın ve Azure CLI üzerinden giriş yapın:
```bash
az login
```

### Adım 2: Projeyi Çalıştırın
Projeyi klonladıktan sonra proje dizinine gidip uygulamayı çalıştırın:
```bash
cd AzureFinOpsCLI

# Temel tarama (Varsayılan olarak %1 CPU altını ve USD kurunu kullanır)
dotnet run -- analyze

# Türk Lirası cinsinden ve özel CPU eşiği ile tarama
dotnet run -- analyze --cpu-threshold 2.0 --lookback-days 14 --currency TRY
```

### Adım 3: Kaynaklara Aksiyon Alın
Tarama sonucunda bulduğunuz bir kaynağı kapatmak veya silmek isterseniz:
```bash
dotnet run -- action --id "<KAYNAK_ID_BURAYA>" --type sleep
```

## 🏗️ Kullanılan Teknolojiler

- **C# / .NET 10**
- **System.CommandLine:** Modern komut satırı ayrıştırması için.
- **Spectre.Console:** Zengin terminal arayüzü (UI) ve tablolar için.
- **Azure Identity & Azure Resource Manager SDK:** Azure kimlik doğrulama ve yönetim işlemleri için.
- **Azure Retail Prices API:** Bölgesel anlık fiyat çekimleri için.

## 🤝 Katkıda Bulunma
Bu proje geliştirilmeye açıktır. Pull Request (PR) göndererek projeye katkıda bulunabilirsiniz. Özellikle yeni Azure kaynak türlerinin (Storage Accounts, App Services vb.) zombi tespiti için eklemeler yapabilirsiniz.

---
*Geliştiricileri sürpriz bulut faturalarından korumak için tasarlanmıştır. ❤️*
