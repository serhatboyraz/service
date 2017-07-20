Pollman Hakkında
=
Uygulama hakkında detaylı bilgi almak için blog'daki yazı dizisini okuyabilirsiniz. 
Bu sayfada sadece repo ile ilgili bilgi verilecektir.
Blog'a erişmek için : 
http://dotnetlife.com/pollman-anket-uygulamasi/

Service Hakkında
=
Pollman/service pollman uygulamasındaki tüm veri işlemlerinin yapıldığı servis uygulamasıdır. Bu uygulamanın detayları aşağıda verilmiştir.

Teknolojiler
=
Pollman uygulaması veritabanı olarak mysql kullanır. Servis kısmı C#.Net4.5 WCF teknolojilerinde NTier katmanlı mimari ile geliştirilmiştir.
Ek olarak Generic Data Repository Design Pattern kullanılmıştır bundan sonra yapılacak geliştirmelerde bu model kullanılacaktır.

Dizin Yapısı
=
+ PollMan Root
  * Data (Tüm veri girdi-çıktı işlemleri bu katmanda gerçekleşir)
    * Logical (Tüm mantıksal işlemler bu bölümde gerçekleşir. ör. Hesaplama,Analiz Etme)
    * Model (Üründeki modeller bu bölümde depolanır.)
   * Services (Bu bölümdeki metodların tamamı dış dünyaya açıktır.)
  *  Database(Veritabanı kaynakları bu katman tarafından sağlanır. Generic Data Repo. Pattern bu katman üzerinden çalışır.)
  *  Service (WCF servis ayarları bu katmanda yapılır.)

>Panel İçin Demo Kullanıcı Bilgileri:

>url : http://pollman.kodofisi.com/panel/

>user : demo@user.com

>pass : demo


>Demo Anket için demo bilgileri:

>url : http://pollman.kodofisi.com/#/main/10/3/P
