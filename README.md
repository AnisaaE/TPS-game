🎯 Üçüncü Şahıs Nişancı (TPS) Oyunu — Unity Projesi
🕹️ Genel Bakış
---

Bu proje, Unity (C#) kullanılarak geliştirilen bir Üçüncü Şahıs Nişancı (TPS) oyunudur.
Oyun; taktiksel aksiyon, gizlilik ve yapay zekâ davranışlarını çöl köyü ortamında bir araya getirir.

Oyuncular, düşman bölgesine sızarak muhafızları etkisiz hâle getirmesi ve kaçırılan çocuğu kurtarması gereken özel bir ajanı kontrol ederler. Görev, çocuğun düşman üssüne transfer edilmeden önce tamamlanmalıdır.

🧠 Hikâye
---
Küçük bir çöl kasabasında yasadışı bir paramiliter örgüt saklanmaktadır.
Bu grup, tehlikeli bir silah veya virüs hakkında kritik bilgiye sahip bir bilim insanının oğlunu kaçırmıştır.

Genç bir özel ajan, yani oyunun kahramanı, köye gizlice sızmak, nöbetçileri ortadan kaldırmak ve çocuğu kurtarmak için gizli bir göreve gönderilir.

Görev, ajan çocuğa ulaştığında kısa bir bitiş animasyonu sekansı ile sona erer.

---
🧱 Proje Mimarisi

Proje, anlaşılabilirlik ve ölçeklenebilirlik esas alınarak yapılandırılmıştır:

![Proje Akış Şeması](https://github.com/AnisaaE/TPS-game/blob/main/Assets/Sema1.png?raw=true)

Assets/
│
├── Scripts/
│ ├── Player/
│ │ ├── PlayerControllerLogic.cs
│ │ └── PlayerHealth.cs
│ │
│ ├── NPC/
│ │ └── Npc_AI.cs
│ │
│ ├── Boy/
│ │ └── EndGameTGR.cs
│ │
│ ├── UI/
│ │ └── UIManager.cs
│ │
│ └── GameManager.cs
│
├── Scenes/
│ └── DesertVillage.unity
│
├── Prefabs/
│ ├── Player.prefab
│ ├── NPC.prefab
│ ├── Boy.prefab
│ └── Environment.prefab
│
└── Animations/
├── Player/
├── NPC/
└── Boy/

Ana karakterlerin her biri — Player (oyuncu), NPC (düşman) ve Boy (çocuk) — kendine ait bir Animator Controller ve bağımsız bir klasöre sahiptir.
Bu modüler yapı, kodun düzenli olmasını sağlar ve hem güncellemeleri hem de hata ayıklamayı kolaylaştırır.

---
⚙️ Oyun Mekanikleri
👨‍✈️ Oyuncu Kontrol Sistemi (PlayerControllerLogic.cs)
---
* Hareket, nişan alma ve ateş etme işlemlerini yönetir.

* Unity New Input System kullanılarak daha hassas ve duyarlı bir kontrol sağlanmıştır.

* Cinemachine ile normal ve nişan kameraları arasında yumuşak geçiş yapılır.

* Raycast tabanlı atış sistemi, isabet ve hasar tespiti için kullanılmıştır.

Öne Çıkan Özellikler:
---
* Kameraya göre yönlendirilmiş hareket

* Yumuşak dönüş (rotation smoothing)

* Crosshair yalnızca nişan modundayken görünür

* Yürüyüş → nişan alma → ateş etme animasyon geçişleri

* TakeDamage() fonksiyonu ile NPC etkileşimi

❤️ Oyuncu Sağlık Sistemi (PlayerHealth.cs)
---

* Oyuncunun can, hasar ve ölüm durumlarını yönetir.

* Hasar alındığında titreşim efektli sağlık çubuğu (health bar) gösterilir.

* Can sıfıra düştüğünde, ölüm animasyonu ve Game Over ekranı devreye girer.

Temel Fonksiyonlar:
---

* TakeDamage(int damage) — canı azaltır, animasyonu tetikler

* Heal(int amount) — canı yeniler

* Die() — ölüm animasyonunu başlatır, hareketi durdurur ve Game Over ekranını açar

🤖 NPC Yapay Zekâ Sistemi (Npc_AI.cs)
---
* NavMeshAgent ile düşman muhafızların hareket ve saldırı davranışlarını yönetir.

* NPC’ler dört duruma sahiptir:

* Devriye (Patrolling) — rastgele bölgelerde dolaşır

* Kovalama (Chasing) — oyuncuyu gördüğünde takip eder

* Saldırı (Attacking) — menzil içindeyken oyuncuya ateş eder

* Ölüm (Dead) — hareketi durdurur, belirli bir süre sonra sahneden silinir

Özellikler:
---
* NavMesh.SamplePosition ile geçerli devriye noktaları belirlenir

* Yürüyüş, koşu ve atış animasyonları arasında yumuşak geçişler

* Raycast ile oyuncuya isabet ve hasar tespiti

* Coroutine tabanlı saldırı hızı kontrolü

👦 Çocuk Kurtarma Sistemi (EndGameTGR.cs)
---

* Çocuk (Boy) objesine bağlıdır.

* Oyuncu belirli bir alana girdiğinde trigger collider ile algılama yapılır.

Oyuncu yaklaştığında:

* Oyuncu ve çocuk birbirlerine döner.

* Her ikisi de “dans” / kutlama animasyonu oynatır.

* End Canvas arayüzü aktifleşir ve görev başarı mesajı görüntülenir.(Gelecek sürümlerde bu sahne bir “sarma” veya kısa ara sahneye (cutscene) dönüştürülebilir.)

🎥 Animasyon Sistemi
---
* Her ana karakterin kendine ait bir Animator Controller vardır:

* PlayerAnimator: Idle, Walk, Aim, Shoot, Jump, Die

* NpcAnimator: Patrol, Run, Shoot, Death

* BoyAnimator: Idle, Dance (Final sahnesi)

Tüm geçişler, yumuşak animasyon karışımı (blend) sağlamak için Animator parametreleri ile kontrol edilir:
animator.SetBool("isRunning", true);
animator.SetTrigger("Dance");

## Game Flow diagram

🧱 Yazılım Mimarisi ve Kullanılan Teknikler
--- 
Bu proje, modüler mimari ve olay odaklı (event-driven) programlama yaklaşımını temel alır.

Kullanılan Başlıca Teknikler:

Bileşen Tabanlı Tasarım (Component-Based Design): Her sistem (Player, NPC, UI, GameManager) birbirinden bağımsız çalışır, yalnızca olaylar veya açık metod çağrılarıyla iletişim kurar.

Raycast ile Etkileşim: Gerçek mermi yerine Raycast kullanılarak, isabet tespiti performans açısından optimize edilmiştir.

Coroutine Tabanlı Zamanlama: Kan efekti, NPC saldırı gecikmeleri ve kamera geçişleri gibi zaman bağımlı işlemler Coroutine’lerle asenkron biçimde yönetilir.

Cinemachine ve NavMesh: Kamera kontrolü ve AI yön bulma sistemlerinde Unity’nin yerleşik çözümleri tercih edilmiştir.

🎮 Grafik ve Optimizasyon

Doku ve materyaller sıkıştırma (compression) ve mipmapping ile optimize edilmiştir.

Animator geçişleri kare düşüşlerini (frame drop) en aza indirmek için ayarlanmıştır.

Cinemachine kamera geçişleri, manuel kamera scriptlerinin yerini alarak daha akıcı bir deneyim sağlar.

Raycast menzili ve katmanları (layers), fizik hesaplamalarını optimize etmek için sınırlandırılmıştır.

NavMesh ile seviye tasarımı, AI hareketinin kararlılığını ve verimliliğini artırmıştır.

🔍 Literatür Taraması
📚 Kaynaklar ve Esinlenilen Çalışmalar
---

Literatürde yer alan birçok Unity tabanlı FPS/TPS projesi, genellikle tek bir bileşene odaklanmıştır — örneğin yalnızca karakter hareketi veya yalnızca yapay zekâ (AI) davranışları üzerine.
Ancak bu proje, bu yaklaşımlardan farklı olarak, AI tabanlı düşman davranışı, kamera sistemleri (Cinemachine) ve UI (kullanıcı arayüzü) entegrasyonunu tek bir bütünleşik oyun döngüsü içinde bir araya getirmiştir.

Bu çok katmanlı entegrasyon ve gerçek zamanlı savaş optimizasyonu, çalışmamızı yalnızca öğretici düzeydeki örneklerden ayırmakla kalmamış, aynı zamanda oyun mekaniği, yapay zekâ ve kullanıcı deneyimi arasındaki dengeyi profesyonel düzeyde kurmamızı sağlamıştır.

Kaynakça ve Esinlenilen Çalışmalar:
---

Unity Learn: Third-Person Controller with Cinemachine — Kamera geçişleri ve oyuncu kontrolü üzerine temel yapı

Brackeys: AI Patrol and Chase System Tutorial — NPC devriye ve kovalama davranışlarının mantıksal temeli

GDC Talks: Responsive Combat and Player Feedback in Action Games — Saldırı ve hasar geri bildirimi sistemlerinde etkileşim odaklı yaklaşım

💡 Özgün Katkı
---
Bu proje, yukarıdaki fikir ve yaklaşımları harmanlayarak, tek ve kapsamlı bir Üçüncü Şahıs Nişancı (TPS) deneyimi oluşturmuştur.

Projemizin Özgün Katkıları:

Gerçek zamanlı AI savaş ve devriye davranışlarını entegre etmesi

Oyuncu için kurtarma görevi (rescue objective) temalı özgün bir görev yapısı tasarlanması

Cinemachine ile sinematik kamera kontrolünün, hedef alma (aim) ve savaş sahneleriyle birleştirilmesi

Performans ve animasyon akışının optimize edilmesi

Bu çalışma, yalnızca savaş ve görev mekaniğini bir araya getirmekle kalmamış; hikâye anlatımı (storytelling) ve oyun mekaniğini (gameplay) uyum içinde harmanlayan, kompakt ve optimize bir Unity projesi olarak öne çıkmıştır.

🧩 Zorluklar ve Çözümler
---

Geliştirme sürecinde Unity’nin iç yapısını daha derin anlamamızı sağlayan birçok teknik ve tasarımsal zorlukla karşılaştık:

🎥 Kamera Sistemleri
---

Sorun: Normal ve nişan kamerası arasında geçiş senkronize değildi; nişangâh (crosshair) ve silah ucu (ShootOrigin) farklı yönlere bakıyordu.
Çözüm: Kamera sistemini Cinemachine öncelikleri (priorities) ile yeniden yapılandırdık, ayrıca LateUpdate() içinde AlignShootOriginWithCamera() fonksiyonunu ekleyerek tam hizalama sağladık.

🤖 NPC Yapay Zekâsı
---

Sorun: NPC’ler devriye sırasında sıkışıyor veya NavMesh dışına çıkıyordu.
Çözüm: NavMesh.SamplePosition() fonksiyonu ile yalnızca geçerli devriye noktaları seçilerek yapay zekâ hareketleri stabilize edildi.

💥 Raycast Tespiti
---

Sorun: İlk sürümlerde, oyuncu NPC’ye nişan alsa bile “hiçbir şey vurulmadı” mesajı alınıyordu.
Çözüm: Düşman katmanları (Enemy Layer) ve etiketleri düzeltildi, Ray kaynağı Camera.main yerine dinamik olarak izlenen cameraTransform olarak değiştirildi.

❤️ Sağlık ve Hasar Geri Bildirimi
---

Sorun: Kan efekti oyun başında sürekli görünüyordu veya hiç aktifleşmiyordu.
Çözüm: Coroutine mantığı damageEffect.gameObject.SetActive(true/false) şeklinde yeniden yazıldı. Böylece oyuncu 10 can kaldığında efekt kalıcı, normal hasarlarda 0.5 saniyelik geçici olarak görüntülendi.

🎞️ Animasyon Senkronizasyonu
---

Sorun: Nişan alma ve ateş etme animasyonları çakışarak tutarsızlık yaratıyordu.
Çözüm: isAiming ve isShooting parametreleriyle geçişler ayrıldı; her karakter (Player, NPC, Boy) için ayrı Animator Controller atandı.

⚙️ Arayüz Tepkileri
---

Sorun: Sağlık çubuğu hasar alındıktan sonra geç tepki veriyordu.
Çözüm: Mathf.Lerp() ile yumuşak geçiş sağlandı ve Coroutine tabanlı titreşim efekti eklendi.

🌟 Bireysel, Teknik ve Takım Çıktıları
---

Bu proje, sadece teknik becerilerimizi değil, aynı zamanda profesyonel ekip çalışması yetkinliğimizi de geliştirdi.

Ekip olarak Agile/Scrum benzeri bir süreç izledik; kısa geliştirme döngüleri (sprint) tanımladık ve her sprint’te belirli hedeflere (AI devriyesi, kamera geçişleri, sağlık sistemi vb.) odaklandık.
Görevleri küçük parçalara böldük, bireylerin güçlü yönlerine göre paylaştırdık ve her aşamada test, entegrasyon ve geri bildirim adımlarını uyguladık.

Tüm ekip çalışması GitHub üzerinden yürütüldü — her üye düzenli olarak commit ve pull request gönderdi, branch mantığıyla geliştirmeler yaptı, çakışmaları birlikte çözdü ve değişiklikleri belgeler hâline getirdi.
Bu versiyon kontrollü süreç, gerçek iş dünyasındaki yazılım geliştirme disiplinine oldukça benzer bir deneyim sağladı.

Bu, ekibimizin ilk büyük ölçekli Unity 3D projesiydi; ancak profesyonel bir geliştirme ekibi gibi çalıştık: haftalık toplantılar düzenledik, görevleri belirledik, zaman çizelgeleri oluşturduk ve birbirimizin kodlarını inceledik.
Bu süreç, takım koordinasyonumuzu, problem çözme hızımızı ve yeni teknolojilere uyum kabiliyetimizi büyük ölçüde artırdı.

Teknik olarak kazandıklarımız:

Nesne yönelimli (OOP) ve olay odaklı C# kodlama pratiği

Unity New Input System ile oyuncu girişi ve Cinemachine kamera geçişlerinde uzmanlaşma

NavMeshAgent ve durum temelli (state-based) AI geçişleri

GitHub üzerinden takım içi işbirliği ve çatışma (merge conflict) çözümü

Kırık prefablar, eksik referanslar ve optimizasyon hatalarıyla gerçek zamanlı hata ayıklama

Bu proje, hem teknik uzmanlığımızı hem de profesyonel disiplinimizi güçlendirerek bizi endüstriyel yazılım ve oyun geliştirme ortamlarına daha hazır hâle getirdi.

🏁 Sonuç
---
Sonuç olarak, geliştirilen bu oyun projesi;
işlevsel ve dengeli bir savaş mekaniği,
gerçekçi düşman yapay zekâsı,
akıcı kamera ve animasyon sistemleri,
ve optimize edilmiş bir oyun akışı ile birleşerek bütüncül bir oyun deneyimi ortaya koymuştur.

Geliştirme sürecinde karşılaşılan teknik sorunlar — kamera senkronizasyonu, animasyon geçişleri, yapay zekâ hareket hataları ve kullanıcı arayüzü gecikmeleri gibi — sistematik olarak analiz edilip çözülmüş; böylece ekip olarak hem teknik hem de metodolojik anlamda önemli bir ilerleme kaydedilmiştir.

Bu proje, yalnızca oynanabilir bir oyun ortaya koymakla kalmamış; aynı zamanda yazılım geliştirme süreçlerinde planlama, iletişim, sorumluluk paylaşımı ve sürekli iyileştirme (iteration) gibi profesyonel becerilerin ekip tarafından deneyimlenmesini sağlamıştır.

Sonuçta ortaya çıkan çalışma, teknik bilgi ile ekip uyumunun birleştiği, gerçek dünyadaki yazılım geliştirme süreçlerini başarıyla yansıtan profesyonel bir üretim sürecinin örneği olmuştur.
