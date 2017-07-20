Pollman Hakkında
=
Uygulama ilk olarak AnketBaz ismiyle oluşturulmuş daha sonra PollMan ismini almıştır.
Bu uygulamayı sürekli olarak anket yapan ve anketin verilerini toplamakta sorun yaşayan kişiler ve ya kurumlar için geliştirdik. Uygulamanın çekirdeği yine Kod Ofisi ve yine çok keyif alarak geliştirdiğimiz bir uygulama oldu. Uygulamanın genel olarak teknolojilerinden bahsedeceğim. İlk olarak uygulama tamamen sunucu-istemci mimarisinde çalışıyor. Aslında temel mantık PollMan 2 çeşit üyelik esasına dayanıyor. Kişi anketlerini birden fazla kişiyle paylaşıp yönetecekse şirket, tek başına yapacaksa da kişisel üyelik tipiyle üye oluyor. Daha sonra panele girip anket oluşturuyor. Ankette sorular/soruların cevapları sınırsız eklenebilir. Ek olarak kullanıcılarından analiz yapmayacağı tamamen bilgilendirme amaçlı olarak özel alan da ekleyebiliyor. Örnek vermek gerekirse birkaç soru ile anket yapacaksınız ve anket yaptığınız kişilerin telefon numaraları ve mail adreslerini istiyorsunuz. İşte bu durumda sorularınızı ve cevaplarınızı ayarladıktan sonra alanlar bölümünden girip türü telefon olan başlığını da kendi belirlediğiniz bir alan ekleyebiliyorsunuz. Bu alanları verileri analiz ederken filtreleme için kullanamıyorsunuz. Yani sadece bilgilendirme amaçlı. Ayrıca şirket için olan hesaptan giriş yaptığınızda da eğer yöneticiyseniz şirketin personellerini düzenleyebiliyorsunuz. Personelseniz de sadece anketleri görüntüleyip sonuçları inceleyebiliyorsunuz.

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
