# PurchaseLoaderApp

**PurchaseLoaderApp** — консольное приложение для загрузки данных о закупках из веб-страниц и XML-файлов, а также для их сохранения в базу данных PostgreSQL. 

Название БД: **ProcurementDb** 

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

PurchaseLoader
│
├── .gitattributes              
├── .gitignore              
├── PurchaseLoader.sln           # Файл решения Visual Studio.
├── README.md                    # Документация проекта.
│
├── Files                     
│   ├── 0123200000319002908.xml 
│   ├── dbase.sql                # Дамп базы данных PostgreSQL.
│   └── Задача.txt               # Описание задачи.
│
├── PurchaseLoaderApp            # Основной проект приложения.
    ├── App.config               # Конфигурация приложения (строки подключения).
    ├── Options.cs               # Класс для работы с параметрами командной строки.
    ├── packages.config          # Зависимости проекта.
    ├── pathconfig.json          # Конфигурация путей по умолчанию (XML и Web).
    ├── Program.cs               # Точка входа в приложение.
    │
    ├── Factory                      # Паттерн "Фабрика" для создания загрузчиков.
    │   └── PurchaseLoaderFactory.cs # Фабрика для создания загрузчиков (Web/XML).
    │
    │
    ├── Interfaces                   # Интерфейсы для расширяемости проекта.
    │   └── IPurchaseLoader.cs       # Интерфейс для загрузчиков данных.
    │
    ├── Loaders                      # Логика загрузки данных.
    │   ├── WebPurchaseLoader.cs     # Логика загрузки данных с веб-страницы.
    │   └── XmlPurchaseLoader.cs     # Логика загрузки данных из XML-файла.
    │
    ├── Models                       # Классы моделей для работы с данными.
    │   ├── Customer.cs              # Класс модели заказчика.
    │   └── Purchase.cs              # Класс модели закупки.
    │
    │
    ├── Services                     # Логика работы с базой данных.
    │   └── DatabaseService.cs       # Логика взаимодействия с базой данных PostgreSQL.
    │
    ├── Utilities                    # Утилиты и вспомогательные классы.
    │   └── OptionsHandler.cs        # Обработка недостающих параметров командной строки.
    │
    ├── Config                       # Конфигурационные классы и их обработка.
    │   ├── AppConfig.cs             # Класс для конфигурации приложения.
    │   ├── AppConfigLoader.cs       # Логика загрузки конфигурации из JSON.
    │   └── DefaultPaths.cs          # Конфигурация путей по умолчанию.
    │
    └── Logging                      # Логирование выполнения.
        └── Logger.cs                # Класс для логирования сообщений.

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

---

## Установка зависимостей

### Обновление NuGet пакетов
Если вы клонируете проект из репозитория, сначала убедитесь, что все зависимости обновлены. Для этого:

1. Откройте **Visual Studio** и перейдите в **Tools > NuGet Package Manager > Manage NuGet Packages for Solution**.
2. Во вкладке **Installed** проверьте список пакетов. Нажмите **Update All**, чтобы обновить все зависимости.

Или воспользуйтесь **Package Manager Console**:

1. Откройте **Tools > NuGet Package Manager > Package Manager Console**.
2. Выполните команду для обновления всех пакетов:
   ```powershell
   Update-Package
   ```
3. Если требуется восстановить недостающие пакеты:
   ```powershell
   Update-Package -reinstall
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
.\PurchaseLoaderApp.exe --type web --source "https://tenmon.ru/1/0123200000319002908" --debug
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
   ```shell
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

## Примечания

### 1. Selenium и `chromedriver.exe`
- Для корректной работы Selenium убедитесь, что:
  - Драйвер `chromedriver.exe` находится в папке `bin\Debug`.
  - Драйвер соответствует установленной версии браузера Chrome.
- Если вы клонируете репозиторий, файл `chromedriver.exe` будет отсутствовать из-за настроек `.gitignore`. Загрузите соответствующий драйвер вручную [здесь](https://chromedriver.chromium.org/downloads) и поместите его в папку `bin\Debug`.

### 2. PostgreSQL
Убедитесь, что PostgreSQL настроен и доступен. Для этого проверьте строку подключения в файле `App.config`:
```xml
<connectionStrings>
	<add name="PurchaseLoaderDbConnection" connectionString="Host=localhost;Port=5432;Database=ProcurementDb;Username=postgres;Password=password;" providerName="Npgsql" />
</connectionStrings>
```

При необходимости обновите `Host`, `Database`, `Username`, и `Password` в зависимости от ваших настроек.