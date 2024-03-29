# LagomCMS

> The documentation for the project will be in Russian only. 
> If you want to use my project, you need to know the Russian language.

- [Что это?](#что-это)
- [Использование](#использование)
  - [Сборка проекта](#сборка-проекта)
  - [Генерация сайта](#генерация-сайта)
  - [Создание контента](#создание-контента)
  - [Статические файлы на страницах сайта](#статические-файлы-на-страницах-сайта)
  - [Редактирование шаблона](#редактирование-шаблона)
  - [Деплой](#деплой)
- [Заключительная информация](#заключительная-информация)

## Что это?

Лагом - скандинавское слово, означающее "только нужное количество" 
или "не слишком много, но и не слишком мало". 

Это слово можно по-разному перевести как "в меру", "в балансе", 
"идеально-простой", "ровно столько", "идеальный" и "подходящий".

LagomCMS - это простой генератор статических сайтов.
Он позволяет создавать и хранить контент сайта в текстовых markdown-файлах,
создавая из них HTML-страницы вашего сайта по определенному заданному шаблону.

За счет этого:

- Нет нагрузки на сервер с сайтом (сервер отдаёт просто готовый HTML-файл)
- Быстрая загрузка сайта (страницу не нужно создавать из данных в СУБД при 
  каждом запросе от пользователя)
- Нет никаких зависимостей на сервере, которые могут сломаться
  (база данных, интерпретатор и т.д.)
- Сайт работает на любом самом дешевом хостинге, где нет поддержки 
  никаких технологий

> Но ведь уже есть другие генераторы статических сайтов?

Да, есть. Но все они не отвечают моим требованиям. Там либо не хватает 
функционала, либо его настолько много, что нет желания тратить 4 года
на изучение этого "готового" генератора.

Поэтому я сделал свой, который полностью удовлетворяет мои требования. 
Если он подходит вам - пользуйтесь! А ещё у него открыт исходный код,
поэтому вы можете форкать мой репозиторий и дорабатывать проект для
решения своих задач.

## Возможности

- Поддержка markdown
- Создание обычных страниц
- Создание страниц с постами (новости, блог и т.д.)
- Поддержка любого количества категорий постов
- Создание страниц со списком постов определенной категории
- Генерация RSS-ленты
- Деплой на FTP-сервер
- Все страницы после генерации могут быть открыты 
  локально в браузере, и на них будут работать стили и картинки
  (_при использовании стандартного шаблона_)

## Использование

### Сборка проекта

Если у вас нет бинарника под вашу ОС, необходимо собрать проект
из исходников. Для этого можно использовать любую IDE для C#, 
либо консольную тулу `dotnet`.

**Зависимости для сборки:**

- [Markdig](https://github.com/xoofx/markdig) - библиотека для 
  генерации markdown
- [Commandline](https://github.com/commandlineparser/commandline) - 
  библиотека для работы с аргументами командной строки
- [FluentFTP](https://github.com/robinrodricks/FluentFTP) - библиотека для работы с FTP


### Генерация сайта

Для генерации сайта необходимо в одной директории иметь:

- Бинарник самого генератора
- Файл `template.html` - шаблон, на основе которого будет создан ваш сайт
- Директории:
  - `categories` - файлы контента для страницы списка постов в категории
  - `pages` - файлы контента для страниц и постов
  - `static` - любые статичные файлы, необходимые для контента сайта,
    не является обязательной директорией.

Для запуска процесса генерации необходимо выполнить команду

```shell
.\LagomCms.exe build
```

Можно использовать необязательные флаги:
```
-u, --baseUrl        Базовый путь до сайта, например https://site.com/
                     используется для генерации RSS ленты и доступен
                     в виде переменной в шаблоне.
                     Без этого параметра RSS лента не 
                     будет создана, но сайт сгенерируется.
                     
-t, --title          Заголовок сайта. Используется для генерации
                     RSS ленты и доступен в виде переменной в шаблоне.
                     Если не указать - в RSS будет "RSS Feed", в
                     переменной шаблона - пустота.
                     
-d, --subtitle       Подзаголовок сайта. Используется для генерации
                     RSS ленты и доступен в виде переменной в шаблоне.
                     Если не указать - будет пустой.
```

Пример генерации с флагами:

```shell
.\LagomCms.exe build -u https://example.ru/ -t 'Название сайта' -d 'Описание сайта'
```

Готовый сайт кладется в директорию `_site`

### Создание контента

Весь основной контент сайта создается в директории `pages`.

- Если назвать файл `index.md` - он станет главной страницей сайта 
- Если назвать файл `page-name.md` - будет создана обычная страница, 
  доступная по адресу `https://example.ru/page-name/`
- Если назвать файл `2024-01-28-any-title.md` - будет создана 
  новостная страница с указанной в названии файла датой публикации,
  она будет доступна по адресу `https://example.ru/название-категории/any-title`

Любые файлы с другими расширениями и вложенные каталоги в директории `pages`
будут проигнорированы при генерации сайта.

В начале каждого файла контента страницы должен находится блок:

```
title: Заголовок страницы
category: notes
----------

```

- title - заголовок страницы, если не указать - будет использовано имя файла
- category - категория для новостных страниц, если не указать - будет `posts`

### Список новостных записей категории

Для каждой категории создается страница, доступная по адресу
`https://example.ru/название-категории/`

В ней находится список ссылок на все новостные страницы указанной категории, 
отсортированные по дате публикации в обратном порядке.

На этой странице можно создавать дополнительный контент, для этого 
необходимо в директории `categories` создать файл `название-категории.md`
со следующим содержимым:

```
title: Человеко-понятный заголовок категории
----------

Любое содержимое страницы в формате markdown
```

### Статические файлы на страницах сайта

При создании MD файла страницы можно использовать статические файлы из 
директории `static`. Для этого необходимо использовать относительную ссылку
через `../`, например

```markdown
![This is image](../static/image.png)
```

Таким образом, MD файл, открытый в вашем любимо редакторе будет
правильно отображать изображение, а при генерации сайта `../static/` 
заменится на другое необходимое относительное местоположение.

В директори `static` можно использовать вложенные каталоги.

### Редактирование шаблона

Для стилизации сайта можно изменить файл стандартного шаблона `template.html`

Это обычный HTML файл, в который будет вставлено содержимое ваших страниц.

В файле шаблона можно использовать следующие переменные:

| Переменная                  | Описание                                         | Пример места использования            |
|-----------------------------|--------------------------------------------------|---------------------------------------|
| `{{site_title}}`            | глобальный заголовок сайта                       | Название сайта в шапке страницы       |
| `{{site_subtitle}} `        | глобальный подзаголовок сайта                    | Описание сайта в шапке страницы       |
| `{{base_url}} `             | базовый адрес сайта                              | Абсолютный путь до статического файла |
| `{{page_title}} `           | заголовок текущей страницы                       | Название страницы в заголовке h1      |
| `{{page_date}} `            | дата публикации новостной страницы               | Описание новостной страницы           |
| `{{page_content}} `         | содержимое текущей страницы                      | Ну тут без слов всё понятно           |
| `{{relative_link_prefix}} ` | префикс относительного пути для текущей страницы | Ссылки в меню сайта                   |
| `{{last_build}} `           | дата и время сборки сайта                        | Справочная поле внизу страницы        |
| `{{timestamp}} `            | таймстамп времени сборки сайта                   | Версия для статических файлов         |
| `{{year}}`                  | текущий год                                      | Поле копирайта внизу страницы         |

Пример использования всех переменных есть в стандартном шаблоне.

### Деплой

Для публикации сайта на сервере, необходимо запустить приложение в режиме `deploy` с указанием
параметров для подключения к FTP-серверу:

```
  -s, --server      Адрес FTP сервера
  -p, --port        Порт FTP сервера (по умолчанию: 21)
  -f, --folder      Удаленный каталог (без символа / в начале и конце)
  -u, --user        Имя пользователя FTP сервера
  -w, --password    Пароль пользователя FTP сервера
```

Например

```shell
.\LagomCms.exe deploy -s ftp.mysite.ru -p 21 -f public_html -u superadmin -w superpassword
```

> ВНИМАНИЕ: удаленный каталог, указанные в параметре деплоя будет 
> очищен при публикации сайта!

## Заключительная информация

Если вам нравится проект и вы хотите его использовать - делайте 
это, но помните, что я не отвечаю ни за какие последствия использования 
этого приложения. Проект распространяется "как есть".

Проект с открытым исходным кодом, поэтому вы можете сделать его форк и 
делать любые исправления для решения своих задач.

Технической поддержки нет, запросы на доработку не принимаются. 
Это приложение я делал для поддержки работы своего личного сайта, поэтому, 
если вас в нём что-то не устраивает - найдите другую CMS.

Сделано в России ![Флаг России](https://raw.githubusercontent.com/sysolyatin/sysolyatin/8d05266b5cf4323e65554fa0ea0ed066446e3084/ru.svg)