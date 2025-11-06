# 🎮 TPS Game - FSM ve Kamera Geçişleri ile Üçüncü Şahıs Nişancı Oyunu

## 1. Giriş  
Bu proje, Unity oyun motoru kullanılarak geliştirilen **Third Person Shooter (TPS)** türünde bir oyundur.  
Oyunun amacı, temel yapay zekâ (AI) davranışlarını, FSM (Finite State Machine) mantığını ve kamera geçiş sistemlerini uygulamalı olarak geliştirmektir.  

Proje sürecinde hem oyuncu (Player) kontrolleri hem de düşman (NPC) davranışları kodlanmış, Unity’nin **NavMesh** ve **AI Agent** sistemleri ile entegre edilmiştir.  
Oyun, oyuncunun hedef alarak düşmanları vurması üzerine kuruludur.  

---

## 2. Literatür Taraması  
Bu proje, oyun geliştirme literatüründe sıkça kullanılan **FSM tabanlı yapay zekâ** ve **kamera kontrolü** yaklaşımlarını uygulamaktadır.  
Benzer mekaniklere sahip örnek oyunlardan bazıları:  

- **Gears of War (2006):** TPS kamerası ve nişan geçişi (aim) arasındaki akıcı kamera kullanımı.  
- **The Last of Us (2013):** FSM tabanlı düşman yapay zekâsı (Idle → Patrol → Chase → Attack).  
- **Resident Evil 4 (2005):** Omuz üzerinden kamera geçişi ve hedefleme sistemi.  

Bu projede yukarıdaki sistemler örnek alınarak sadeleştirilmiş bir şekilde uygulanmıştır.  
FSM davranışları temel düzeyde tasarlanmış, karmaşık durumlar için **Behavior Tree** yerine **FSM (Finite State Machine)** kullanılmıştır.  

---

## 3. Oyun Mekanikleri  

### 🎮 Oyuncu (Player)  
- **Main Camera:** Oyuna üçüncü şahıs (third person) bakış açısı kazandırır.  
- **Aim Camera:** Sağ tık (Right Mouse Button) ile etkinleşir, kamera oyuncuya yaklaşır ve nişan alma modu açılır.  
- **Transition:** Ana kameradan aim kameraya geçiş yumuşak bir biçimde (lerp benzeri) yapılır.  
- **Saldırı (Attack):** Sol tık (Left Mouse Button) ile ateş edilir; mermi NPC’ye çarptığında hasar uygular.

### 🤖 Düşman (NPC)  
NPC’ler belirli durumlara sahip olacak şekilde FSM (Finite State Machine) yaklaşımıyla kodlanmıştır:  

| Durum | Açıklama |
|--------|-----------|
| **Idle** | NPC hareketsizdir ve bekleme durumundadır. |
| **Patrol** | NPC, belirlenen noktalar arasında devriye gezer. |
| **Chase** | Oyuncu belirli bir menzile girdiğinde NPC, oyuncuyu kovalamaya başlar. |
| **Attack** | Oyuncu yeterince yaklaştığında saldırı davranışına geçer. |

FSM yapısı sayesinde NPC durum geçişleri şu şekilde gerçekleşir:  
[Idle] → [Patrol] → [Chase] → [Attack] → [Idle]

### 🧭 Yol Bulma (Pathfinding)
Unity’nin **NavMesh** sistemi kullanılmıştır.  
NPC’lere birer **NavMeshAgent** bileşeni atanmış ve hedef olarak oyuncu gösterilmiştir.  
Bu sayede NPC, oyuncuya en kısa yoldan ulaşarak saldırı başlatır.  

---

## 4. Sistem Tasarımı  

### 📂 Kullanılan Araçlar ve Teknolojiler:
- **Oyun Motoru:** Unity 2022.x  
- **Programlama Dili:** C#  
- **IDE:** Visual Studio  
- **AI / Navigation:** Unity NavMesh + NavMesh Agent  
- **Kontrol Sistemi:** Unity New Input System  
- **Sürüm Kontrol:** Git & GitHub  

### 🧩 Sınıf Yapısı:
| Sınıf | Görevi |
|-------|---------|
| **PlayerControllerLogic** | Oyuncu hareketlerini, kamera geçişlerini ve atış işlemlerini yönetir. |
| **CameraController** | MainCamera ve AimCamera arasındaki geçişleri kontrol eder. |
| **EnemyFSM** | NPC’nin durum geçişlerini yönetir (Idle, Patrol, Chase, Attack). |
| **GameManager** | Genel oyun akışını ve skor yönetimini kontrol eder. |

---

## 5. Yazılım Mimarisi ve Teknikler  
Projede **Nesne Yönelimli Programlama (OOP)** ilkeleri uygulanmıştır:  

- **Encapsulation:** Her bileşenin sorumluluk alanı belirlenmiş, değişkenler koruma altına alınmıştır.  
- **Inheritance:** Ortak bileşenler (örneğin NPC türleri) için kalıtım yapısı kullanılmıştır.  
- **Polymorphism:** FSM davranışları farklı durumlarda override edilmiştir.  

Ek olarak kullanılan teknikler:  
- **Finite State Machine (FSM):** NPC’nin davranışlarının mantıksal olarak kontrolü.  
- **NavMesh Pathfinding:** NPC’nin oyuncuya ulaşmak için en kısa rotayı bulması.  
- **Camera Transition (Lerp):** Main → Aim geçişlerinin yumuşak yapılması.  
- **Event Trigger System:** Oyuncunun ateş etmesi veya NPC’nin ölmesi gibi olaylarda tetiklenen event yapısı.  

---

## 6. Karşılaşılan Sorunlar ve Çözümler  

| Sorun | Açıklama | Çözüm |
|--------|-----------|--------|
| Main Camera’yı Player altına koyunca takip problemi oluştu | Kamera pozisyonu yanlış referans aldı | MainCamera’yı Hierarchy’de ayrı tuttuk, Player altındaki kameraları takip ettirdik |
| FSM durum geçişlerinde gecikme | Update içinde yanlış koşul kontrolü vardı | Koşullar OnTriggerEnter / OnTriggerStay ile optimize edildi |
| Aim modunda kamera ani geçiş yapıyordu | Transition değerleri çok hızlıydı | Kamera geçişine Lerp yöntemi eklendi |
| Mermi NPC’ye çarptığında hata alınıyordu | Collision Layer yanlış atanmıştı | “Enemy” layer’ı oluşturulup scriptte kontrol eklendi |

---

## 7. Sonuç ve Katkılar  
Bu proje sayesinde:  
- FSM tabanlı yapay zekâ davranışlarının temelleri öğrenildi.  
- Unity NavMesh ve AI Agent kullanımı uygulamalı olarak denendi.  
- Kamera geçişleri (Third Person ↔ Aim) profesyonel oyunlarda olduğu gibi uygulanabildi.  
- GitHub üzerinden versiyon kontrolü ve ekip çalışması deneyimi kazanıldı.  

Bu proje, hem oyun mekaniği hem de yapay zekâ açısından temel ama gerçekçi bir **oyun geliştirme altyapısı** oluşturmuştur.  

---

## 8. Kaynakça  
- Unity Technologies. (2025). *Unity User Manual*. https://docs.unity3d.com/  
- Brackeys. (2022). *Third Person Movement in Unity* [YouTube Video].  
- GameDev.TV. (2024). *AI and Pathfinding in Unity Course*.  
- IEEE Explore Digital Library. (2024). *Finite State Machines in Game AI Systems*.  
