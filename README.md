# Message Service

## Kullanım Kılavuzu:

Servis içerisinde tüketicinin kullanabileceği 6 action bulunmaktadır. 
Bunlar:

- Register
- Login
- Blockuser
- Unblockuser
- Sendmessage
- Pastmessages

Bu action'ların kullanımları aşağıda detaylıca açıklanmaktadır.

## Register:

Register işlemi için kullanılması gereken endpoint:
```
api/armutmessage/register
```
Request tipi **POST** olmalıdır.

Register action için yapılan çağrı aşağıda belirtilen **RegisterModel**'i request body'sinde içermelidir:
```
{
  "UserName":"Exampleuser",
  "Password":"123Example@"
}
```
**Password**'un sahip olması gereken özellikler:
- 6 karakterden uzun olmalı
- Büyük ve küçük harf içermeli
- En az bir adet sembol içermeli

## Login:

Login işlemi için kullanılması gereken endpoint:
```
api/armutmessage/login
```
Request tipi **POST** olmalıdır.

Login action için yapılan çağrı aşağıda örneği verilen **LoginModel**'i request body'sinde içermelidir:
```
{
  "UserName":"Exampleuser",
  "Password":"123Example@"
}
```
**Password**'un sahip olması gereken özellikler:
- 6 karakterden uzun olmalı
- Büyük ve küçük harf içermeli
- En az bir adet sembol içermeli

Başarılı **Login** işlemi kullanıcıya Bearer Token döndürür. Bu token aşağıdaki authorization gerektiren işlemler için kullanılmalıdır. 
Aksi takdirde servis tarafından 401 Unauthorized hatası dönecektir.

## Sendmessage:

Sendmessage işlemi mesaj göndermek için kullanılmaktadır. Bu işlem için kullanılması gereken endpoint:
```
api/armutmessage/sendmessage
```
Request tipi **POST** olmalıdır.

Sendmessage işlemi için yapılan çağrı aşağıda örneği verilen **MessageSendingModel**'i request body'sinde içermelidir:
```
{
  "receivedBy":"Exampleuser",
  "text":"example text message string"
}
```
**text** alanı 500 karakterden fazla olmamalıdır.

Login işlemi sonrasında kullanıcıya döndürülen Bearer Token, Sendmessage işleminin request header'ı içerisinde bulunmalıdır:
Örnek bir header'a ekleme işlemi (Javascript-Fetch):
```
var myHeaders = new Headers();
myHeaders.append("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZmluYWx0ZXN0MSIsImp0aSI6ImUyZmE4ZWFmLWFlNGQtNDVlMC1iNzI0LTA0MGI3MzNjYWQ0YiIsImV4cCI6MTYxNTI5NDI2NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo0OTQ0MSIsImF1ZCI6IlVzZXIifQ.k5VekIxdLXLcXPjRp6mudN2g21G-l8NbQKcNJX_1ROk");
```

## Blockuser:

Blockuser işlemi mesaj almak istenilmeyen kullanıcının engellenmesi için kullanılmaktadır.
Bu işlem için kullanılması gereken endpoint:
```
api/armutmessage/blockuser/{usernameForBlock}
```
Request tipi **POST** olmalıdır.

Blockuser işlemi için yapılan çağrının request header'ı Login işleminden döndürülen Bearer Token'ı içermelidir.

> *Bearer Token'ın Header'a nasıl ekleneceği **Sendmessage** başlığındaki örnekte mevcuttur.*

## Unblockuser:

Unblockuser işlemi daha önce engellenen kullanıcının engelini kaldırmak için kullanılmaktadır.

Bu işlem için kullanılması gereken endpoint:
```
api/armutmessage/unblockuser/{usernameForUnblock}
```
Request tipi **POST** olmalıdır.

Unblockuser işlemi için yapılan çağrının request header'ı Login işleminden döndürülen Bearer Token'ı içermelidir.

> *Bearer Token'ın Header'a nasıl ekleneceği **Sendmessage** başlığındaki örnekte mevcuttur.*

## PastMessages:

Pastmessages işlemi kullanıcının başka bir kullanıcı ile olan mesajlaşma geçmişinin çağırılması için kullanılmaktadır.

Bu işlem için kullanılması gereken endpoint:
```
api/armutmessage/pastmessages/{username}
```
Request tipi **GET** olmalıdır.

Pastmessages işlemi için yapılan çağrının request header'ı Login işleminden döndürülen Bearer Token'ı içermelidir.

> *Bearer Token'ın Header'a nasıl ekleneceği **Sendmessage** başlığındaki örnekte mevcuttur.*

