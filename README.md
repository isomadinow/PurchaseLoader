﻿# PurchaseLoaderApp

**PurchaseLoaderApp** — консольное приложение для загрузки данных о закупках из веб-страниц и XML-файлов, а также для их сохранения в базу данных PostgreSQL.

---

## Особенности

- Поддержка двух типов источников:
  - **Web**: Загрузка данных с веб-страницы с использованием Selenium.
  - **XML**: Загрузка данных из XML-файла.
- Логирование выполнения с разными уровнями: `Debug`, `Info`, `Error`.
- Интерактивный режим для удобного ввода данных.
- Возможность сохранения данных в PostgreSQL.

---

## Структура проекта

```
PurchaseLoaderApp
│   App.config                   # Конфигурация приложения (строки подключения).
│   Options.cs                   # Класс для работы с параметрами командной строки.
│   packages.config              # Зависимости проекта.
│   pathconfig.json              # Конфигурация путей по умолчанию (XML и Web).
│   Program.cs                   # Точка входа в приложение.
│
├───Factory
│       PurchaseLoaderFactory.cs # Фабрика для создания загрузчиков (Web/XML).
│
├───Interfaces
│       IPurchaseLoader.cs       # Интерфейс для загрузчиков данных.
│
├───Loaders
│       WebPurchaseLoader.cs     # Логика загрузки данных с веб-страницы.
│       XmlPurchaseLoader.cs     # Логика загрузки данных из XML-файла.
│
├───Models
│       Customer.cs              # Класс модели заказчика.
│       Purchase.cs              # Класс модели закупки.
│
├───Services
│       DatabaseService.cs       # Логика взаимодействия с базой данных PostgreSQL.
│
├───Utilities
│       OptionsHandler.cs        # Обработка недостающих параметров командной строки.
│
├───Utilities/Config
│       AppConfig.cs             # Класс для конфигурации приложения.
│       AppConfigLoader.cs       # Логика загрузки конфигурации из JSON.
│       DefaultPaths.cs          # Конфигурация путей по умолчанию.
│
└───Utilities/Logging
        Logger.cs                # Класс для логирования сообщений.
```

---

## Зависимости

Проект использует следующие библиотеки:

| **Библиотека**                     | **Версия**     | **Описание**                                                  |
|------------------------------------|----------------|--------------------------------------------------------------|
| **CommandLine**                    | 2.x            | Для обработки аргументов командной строки.                  |
| **Dapper**                         | 2.x            | Легковесный ORM для работы с базой данных PostgreSQL.        |
| **Npgsql**                         | 6.x            | ADO.NET-драйвер для работы с PostgreSQL.                     |
| **Selenium.WebDriver**             | 4.x            | Для автоматизации браузера (загрузка данных с веб-страниц).  |


### Установка зависимостей

Насколько сработает не знаю, у меня срабатывало, из за того что почти одинаковое окружение.

При сборке проекта все зависимости будут автоматически загружены из `packages.config`.
Если вы добавляете новый компьютер или окружение, выполните:

```bash
nuget restore PurchaseLoaderApp.sln
```
---

## Установка и запуск

### Сборка
1. Откройте проект в Visual Studio.
2. Выполните сборку:
   ```bash
   Build > Build Solution
   ```

### Запуск
Перейдите в папку `bin\Debug` и выполните команду:
```bash
PurchaseLoaderApp.exe --type web --source "https://example.com" --debug
```

---

## Использование

### Параметры запуска

| Параметр      | Обязательный | Описание                                                                 |
|---------------|--------------|-------------------------------------------------------------------------|
| `--type`      | Нет          | Тип источника данных (`web` или `xml`).                                 |
| `--source`    | Нет          | URL веб-страницы (для `web`) или путь к XML-файлу (для `xml`).          |
| `--debug`     | Нет          | Включает отладочный режим для подробного логирования.                   |

#### Примеры:
1. Загрузка данных с веб-страницы:
   ``shell
   .\PurchaseLoaderApp.exe --type web --source "https://tenmon.ru/1/0123200000319002908"
   ```

2. Загрузка данных из XML-файла:
   ```shell
   .\PurchaseLoaderApp.exe --type xml --source "C:\path\to\example.xml"
   ```

3. Интерактивный режим с отладкой (только `--debug`):
   ```shell
   .\PurchaseLoaderApp.exe --debug
   ```
4. Интерактивный режим (без отладки):
   ```shell
   .\PurchaseLoaderApp.exe
   ```

---

## Настройка конфигурации

Файл `pathconfig.json` содержит пути по умолчанию:

```json
{
  "DefaultPaths": {
    "XmlFilePath": "Files/0123200000319002908.xml",
    "WebUrl": "https://tenmon.ru/1/0123200000319002908"
  }
}

```

Можно изменить значения `XmlFilePath` и `WebUrl` для использования своих значений по умолчанию.

---

### Примечание

Для корректной работы Selenium убедитесь, что:
- Установлена последняя версия **Google Chrome**.
- Драйвер `chromedriver.exe` в папке `bin\Debug` соответствует вашей версии Chrome. 