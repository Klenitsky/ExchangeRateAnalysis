# ExchangeRateAnlysis
## Описание
Клиент серверное приложение для анализа курсов валют
## Использование 
1. Для работы должна быть запущена серверная часть приложения
2. Осуществление запроса осуществляется по нажатию кнопки "Получить"
## Замечания
1. Изначально проект настроен на тестирование на одном компьютере. При необходимости работы на разных комьютерах(клиента и сервера) нужно изменить connectionString и задать в нем IP адресс компьютера, где будет запущен сервер.
2. Бесплатный API key в CoinAPI(API для получения курсов криптовалют) позволяет делать не более 100 запросов в сутки, при исчерпании лимита попытка загрузить курс биткоина будет неудачной и будет выдано соответствующее сообщение.
3. json файл хранилища записей о курсах и файл логов создаются автоматически при запуске приложения(в случае если они не были созданы до этого)


